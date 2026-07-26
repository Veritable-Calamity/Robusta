using Robusta.Runtime.Shared.Hosting;

namespace Robusta.Runtime.Shared.Catalogs;

internal sealed class CatalogGeneration
{
    private readonly object _gate = new();
    private int _leaseCount;
    private bool _retired;

    public CatalogGeneration(CatalogGenerationId id)
    {
        id.RequireInitialized(nameof(id));
        Id = id;
    }

    public CatalogGenerationId Id { get; }

    public int LeaseCount
    {
        get
        {
            lock (_gate)
                return _leaseCount;
        }
    }

    public bool IsCollectible
    {
        get
        {
            lock (_gate)
                return _retired && _leaseCount == 0;
        }
    }

    public CatalogGenerationLease AcquireLease(WorldInstanceId owner)
    {
        owner.RequireInitialized(nameof(owner));

        lock (_gate)
        {
            if (_retired)
                throw new InvalidOperationException("A retired catalog generation cannot acquire a new world lease.");

            _leaseCount++;
            return new CatalogGenerationLease(this, owner);
        }
    }

    public void Retire()
    {
        lock (_gate)
            _retired = true;
    }

    private void ReleaseLease()
    {
        lock (_gate)
        {
            if (_leaseCount <= 0)
                throw new InvalidOperationException("The catalog generation lease count is already zero.");

            _leaseCount--;
        }
    }

    // TODO(ADR-0021/0024/0037): Move catalog creation, retention, collection, and
    // transactional adoption into the installation-owned catalog repository.
    internal sealed class CatalogGenerationLease : IAsyncDisposable
    {
        private CatalogGeneration? _generation;

        internal CatalogGenerationLease(CatalogGeneration generation, WorldInstanceId owner)
        {
            _generation = generation;
            Owner = owner;
        }

        public WorldInstanceId Owner { get; }

        public ValueTask DisposeAsync()
        {
            Interlocked.Exchange(ref _generation, null)?.ReleaseLease();
            return ValueTask.CompletedTask;
        }
    }
}
