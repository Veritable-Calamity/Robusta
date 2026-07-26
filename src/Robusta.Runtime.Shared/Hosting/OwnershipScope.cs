namespace Robusta.Runtime.Shared.Hosting;

internal enum OwnershipScopeState
{
    Open,
    Closing,
    Closed
}

internal abstract class OwnershipScope
{
    private readonly object _gate = new();
    private readonly List<CleanupRegistration> _cleanup = [];
    private OwnershipScopeState _state = OwnershipScopeState.Open;

    protected OwnershipScope(OwnershipScopeKind kind)
    {
        Kind = kind;
    }

    public OwnershipScopeKind Kind { get; }

    public OwnershipScopeState State
    {
        get
        {
            lock (_gate)
                return _state;
        }
    }

    internal void EnsureOpen()
    {
        lock (_gate)
        {
            if (_state != OwnershipScopeState.Open)
                throw new InvalidOperationException($"The {Kind} scope is {_state} and cannot admit new work.");
        }
    }

    internal void RegisterCleanup(
        string resourceName,
        Func<CancellationToken, ValueTask> cleanup)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resourceName);
        ArgumentNullException.ThrowIfNull(cleanup);

        lock (_gate)
        {
            if (_state != OwnershipScopeState.Open)
                throw new InvalidOperationException($"Cleanup cannot be registered after the {Kind} scope starts closing.");

            _cleanup.Add(new CleanupRegistration(resourceName, cleanup));
        }
    }

    internal IReadOnlyList<CleanupRegistration>? BeginClose()
    {
        lock (_gate)
        {
            if (_state != OwnershipScopeState.Open)
                return null;

            _state = OwnershipScopeState.Closing;
            return _cleanup.ToArray();
        }
    }

    internal void CompleteClose()
    {
        lock (_gate)
        {
            if (_state != OwnershipScopeState.Closing)
                throw new InvalidOperationException($"The {Kind} scope cannot complete from state {_state}.");

            _state = OwnershipScopeState.Closed;
        }
    }

    internal async ValueTask<ScopeCloseResult> CloseResourcesAsync(
        IReadOnlyList<CleanupRegistration> registrations,
        CancellationToken cancellationToken)
    {
        var failures = new List<ScopeCleanupFailure>();

        for (var index = registrations.Count - 1; index >= 0; index--)
        {
            var registration = registrations[index];

            try
            {
                await registration.Cleanup(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                failures.Add(new ScopeCleanupFailure(Kind, registration.ResourceName, exception));
            }
        }

        return failures.Count == 0 ? ScopeCloseResult.Success : new ScopeCloseResult(failures);
    }

    // TODO(ADR-0017): Add generated lifetime-capture validation, cleanup budgets,
    // leak detection, and structured scope identifiers to cleanup diagnostics.
    internal sealed record CleanupRegistration(
        string ResourceName,
        Func<CancellationToken, ValueTask> Cleanup);
}
