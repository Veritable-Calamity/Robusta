using Robusta.Runtime.Shared.Hosting;

namespace Robusta.Runtime.Shared.Sessions;

internal sealed class SessionWorldAttachmentScope : OwnershipScope, IAsyncDisposable
{
    private readonly HostScope _host;

    internal SessionWorldAttachmentScope(
        HostScope host,
        SessionWorldAttachmentId id,
        SessionAttachmentEndpoint session,
        WorldAttachmentEndpoint world)
        : base(OwnershipScopeKind.SessionWorldAttachment)
    {
        _host = host;
        Id = id;
        Session = session;
        World = world;
    }

    public SessionWorldAttachmentId Id { get; }

    public SessionAttachmentEndpoint Session { get; }

    public WorldAttachmentEndpoint World { get; }

    public ValueTask<ScopeCloseResult> CloseAsync(CancellationToken cancellationToken = default) =>
        _host.CloseAttachmentAsync(this, cancellationToken);

    public async ValueTask DisposeAsync()
    {
        var result = await CloseAsync().ConfigureAwait(false);
        result.ThrowIfFailed();
    }

    // TODO(ADR-0023/0028): Add declared endpoint capabilities, participation role,
    // avatar association, interest, baselines, acknowledgements, and attachment-
    // local network identity mappings when their owning contracts are implemented.
}
