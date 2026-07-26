using Robusta.Runtime.Shared.Hosting;

namespace Robusta.Runtime.Shared.Sessions;

internal sealed class SessionScope : OwnershipScope, IAsyncDisposable
{
    private readonly HostScope _host;

    internal SessionScope(HostScope host, SessionId id)
        : base(OwnershipScopeKind.Session)
    {
        _host = host;
        Id = id;
    }

    public SessionId Id { get; }

    public ValueTask<ScopeCloseResult> CloseAsync(CancellationToken cancellationToken = default) =>
        _host.CloseSessionAsync(this, cancellationToken);

    public async ValueTask DisposeAsync()
    {
        var result = await CloseAsync().ConfigureAwait(false);
        result.ThrowIfFailed();
    }
}
