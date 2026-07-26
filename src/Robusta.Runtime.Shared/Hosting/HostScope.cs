using Robusta.Runtime.Shared.Catalogs;
using Robusta.Runtime.Shared.Sessions;
using Robusta.Runtime.Shared.Worlds;

namespace Robusta.Runtime.Shared.Hosting;

internal sealed class HostScope : OwnershipScope, IAsyncDisposable
{
    private readonly object _gate = new();
    private readonly SemaphoreSlim _lifecycleGate = new(1, 1);
    private readonly Dictionary<WorldInstanceId, WorldScope> _worlds = [];
    private readonly Dictionary<SessionId, SessionScope> _sessions = [];
    private readonly Dictionary<SessionWorldAttachmentId, SessionWorldAttachmentScope> _attachments = [];

    public HostScope()
        : base(OwnershipScopeKind.Host)
    {
        Id = HostInstanceId.New();
    }

    public HostInstanceId Id { get; }

    public int WorldCount
    {
        get
        {
            lock (_gate)
                return _worlds.Count;
        }
    }

    public int SessionCount
    {
        get
        {
            lock (_gate)
                return _sessions.Count;
        }
    }

    public int AttachmentCount
    {
        get
        {
            lock (_gate)
                return _attachments.Count;
        }
    }

    public WorldScope CreateWorld(CatalogGeneration catalogGeneration)
    {
        ArgumentNullException.ThrowIfNull(catalogGeneration);

        lock (_gate)
        {
            EnsureOpen();
            var id = WorldInstanceId.New();
            var lease = catalogGeneration.AcquireLease(id);
            var world = new WorldScope(this, id, catalogGeneration, lease);
            _worlds.Add(id, world);
            return world;
        }
    }

    public SessionScope CreateSession()
    {
        lock (_gate)
        {
            EnsureOpen();
            var session = new SessionScope(this, SessionId.New());
            _sessions.Add(session.Id, session);
            return session;
        }
    }

    public SessionWorldAttachmentScope Attach(SessionScope session, WorldScope world)
    {
        ArgumentNullException.ThrowIfNull(session);
        ArgumentNullException.ThrowIfNull(world);

        lock (_gate)
        {
            EnsureOpen();
            RequireOwnedOpenSession(session);
            RequireOwnedOpenWorld(world);

            var attachment = new SessionWorldAttachmentScope(
                this,
                SessionWorldAttachmentId.New(),
                new SessionAttachmentEndpoint(session.Id),
                new WorldAttachmentEndpoint(world.Id));

            _attachments.Add(attachment.Id, attachment);
            return attachment;
        }
    }

    public async ValueTask<ScopeCloseResult> CloseAsync(CancellationToken cancellationToken = default)
    {
        await _lifecycleGate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            IReadOnlyList<CleanupRegistration>? registrations;
            SessionWorldAttachmentScope[] attachments;
            SessionScope[] sessions;
            WorldScope[] worlds;

            lock (_gate)
            {
                registrations = BeginClose();
                if (registrations is null)
                    return ScopeCloseResult.Success;

                attachments = _attachments.Values.ToArray();
                sessions = _sessions.Values.ToArray();
                worlds = _worlds.Values.ToArray();
            }

            var results = new List<ScopeCloseResult>();

            foreach (var attachment in attachments)
                results.Add(await CloseAttachmentCoreAsync(attachment, cancellationToken).ConfigureAwait(false));

            foreach (var session in sessions)
                results.Add(await CloseSessionCoreAsync(session, cancellationToken).ConfigureAwait(false));

            foreach (var world in worlds)
                results.Add(await CloseWorldCoreAsync(world, cancellationToken).ConfigureAwait(false));

            results.Add(await CloseResourcesAsync(registrations, cancellationToken).ConfigureAwait(false));
            CompleteClose();
            return ScopeCloseResult.Combine(results);
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    internal ValueTask<ScopeCloseResult> CloseWorldAsync(
        WorldScope world,
        CancellationToken cancellationToken) =>
        RunLifecycleOperationAsync(() => CloseWorldCoreAsync(world, cancellationToken), cancellationToken);

    internal ValueTask<ScopeCloseResult> CloseSessionAsync(
        SessionScope session,
        CancellationToken cancellationToken) =>
        RunLifecycleOperationAsync(() => CloseSessionCoreAsync(session, cancellationToken), cancellationToken);

    internal ValueTask<ScopeCloseResult> CloseAttachmentAsync(
        SessionWorldAttachmentScope attachment,
        CancellationToken cancellationToken) =>
        RunLifecycleOperationAsync(() => CloseAttachmentCoreAsync(attachment, cancellationToken), cancellationToken);

    public async ValueTask DisposeAsync()
    {
        var result = await CloseAsync().ConfigureAwait(false);
        result.ThrowIfFailed();
    }

    private async ValueTask<ScopeCloseResult> CloseWorldCoreAsync(
        WorldScope world,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CleanupRegistration>? registrations;
        SessionWorldAttachmentScope[] attachments;
        lock (_gate)
        {
            registrations = world.BeginClose();
            if (registrations is null)
                return ScopeCloseResult.Success;

            attachments = _attachments.Values
                .Where(attachment => attachment.World.WorldId == world.Id)
                .ToArray();
        }

        var results = new List<ScopeCloseResult>();
        foreach (var attachment in attachments)
            results.Add(await CloseAttachmentCoreAsync(attachment, cancellationToken).ConfigureAwait(false));

        results.Add(await world.CloseResourcesAsync(registrations, cancellationToken).ConfigureAwait(false));
        world.CompleteClose();

        lock (_gate)
            _worlds.Remove(world.Id);

        return ScopeCloseResult.Combine(results);
    }

    private async ValueTask<ScopeCloseResult> CloseSessionCoreAsync(
        SessionScope session,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CleanupRegistration>? registrations;
        SessionWorldAttachmentScope[] attachments;
        lock (_gate)
        {
            registrations = session.BeginClose();
            if (registrations is null)
                return ScopeCloseResult.Success;

            attachments = _attachments.Values
                .Where(attachment => attachment.Session.SessionId == session.Id)
                .ToArray();
        }

        var results = new List<ScopeCloseResult>();
        foreach (var attachment in attachments)
            results.Add(await CloseAttachmentCoreAsync(attachment, cancellationToken).ConfigureAwait(false));

        results.Add(await session.CloseResourcesAsync(registrations, cancellationToken).ConfigureAwait(false));
        session.CompleteClose();

        lock (_gate)
            _sessions.Remove(session.Id);

        return ScopeCloseResult.Combine(results);
    }

    private async ValueTask<ScopeCloseResult> CloseAttachmentCoreAsync(
        SessionWorldAttachmentScope attachment,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<CleanupRegistration>? registrations;
        lock (_gate)
        {
            registrations = attachment.BeginClose();
            if (registrations is null)
                return ScopeCloseResult.Success;
        }

        var result = await attachment.CloseResourcesAsync(registrations, cancellationToken).ConfigureAwait(false);
        attachment.CompleteClose();

        lock (_gate)
            _attachments.Remove(attachment.Id);

        return result;
    }

    private async ValueTask<ScopeCloseResult> RunLifecycleOperationAsync(
        Func<ValueTask<ScopeCloseResult>> operation,
        CancellationToken cancellationToken)
    {
        await _lifecycleGate.WaitAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            return await operation().ConfigureAwait(false);
        }
        finally
        {
            _lifecycleGate.Release();
        }
    }

    private void RequireOwnedOpenWorld(WorldScope world)
    {
        RequireOwnedWorld(world);
        world.EnsureOpen();
    }

    private void RequireOwnedWorld(WorldScope world)
    {
        lock (_gate)
        {
            if (!_worlds.TryGetValue(world.Id, out var owned) || !ReferenceEquals(owned, world))
                throw new InvalidOperationException("The world is not owned by this host.");
        }
    }

    private void RequireOwnedOpenSession(SessionScope session)
    {
        RequireOwnedSession(session);
        session.EnsureOpen();
    }

    private void RequireOwnedSession(SessionScope session)
    {
        lock (_gate)
        {
            if (!_sessions.TryGetValue(session.Id, out var owned) || !ReferenceEquals(owned, session))
                throw new InvalidOperationException("The session is not owned by this host.");
        }
    }

    // TODO(ADR-0017/0028): Replace manual composition with generated activation
    // metadata and typed capability validation without introducing a service locator.
}
