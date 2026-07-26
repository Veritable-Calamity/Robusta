using Robusta.Runtime.Shared.Hosting;

namespace Robusta.Runtime.Shared.Sessions;

internal readonly record struct WorldAttachmentEndpoint
{
    public WorldAttachmentEndpoint(WorldInstanceId worldId)
    {
        worldId.RequireInitialized(nameof(worldId));
        WorldId = worldId;
    }

    public WorldInstanceId WorldId { get; }
}

internal readonly record struct SessionAttachmentEndpoint
{
    public SessionAttachmentEndpoint(SessionId sessionId)
    {
        sessionId.RequireInitialized(nameof(sessionId));
        SessionId = sessionId;
    }

    public SessionId SessionId { get; }
}
