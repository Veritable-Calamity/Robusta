using Robusta.Runtime.Shared.Catalogs;
using Robusta.Runtime.Shared.Hosting;

namespace Robusta.Runtime.Shared.Worlds;

internal sealed class WorldScope : OwnershipScope, IAsyncDisposable
{
    private readonly HostScope _host;

    internal WorldScope(
        HostScope host,
        WorldInstanceId id,
        CatalogGeneration catalogGeneration,
        CatalogGeneration.CatalogGenerationLease catalogLease)
        : base(OwnershipScopeKind.World)
    {
        _host = host;
        Id = id;
        CatalogGeneration = catalogGeneration;
        RegisterCleanup("catalog-generation-lease", _ => catalogLease.DisposeAsync());
    }

    public WorldInstanceId Id { get; }

    public CatalogGeneration CatalogGeneration { get; }

    public ValueTask<ScopeCloseResult> CloseAsync(CancellationToken cancellationToken = default) =>
        _host.CloseWorldAsync(this, cancellationToken);

    public async ValueTask DisposeAsync()
    {
        var result = await CloseAsync().ConfigureAwait(false);
        result.ThrowIfFailed();
    }
}
