namespace Robusta.Runtime.Shared.Hosting;

internal readonly struct HostInstanceId : IEquatable<HostInstanceId>
{
    private readonly Guid _value;

    private HostInstanceId(Guid value)
    {
        _value = value != Guid.Empty
            ? value
            : throw new ArgumentException("An ephemeral identity cannot use the empty value.", nameof(value));
    }

    internal static HostInstanceId New() => new(Guid.NewGuid());

    internal bool IsInitialized => _value != Guid.Empty;

    public bool Equals(HostInstanceId other) => _value.Equals(other._value);

    public override bool Equals(object? obj) => obj is HostInstanceId other && Equals(other);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => IsInitialized ? nameof(HostInstanceId) : $"{nameof(HostInstanceId)}(Uninitialized)";

    public static bool operator ==(HostInstanceId left, HostInstanceId right) => left.Equals(right);

    public static bool operator !=(HostInstanceId left, HostInstanceId right) => !left.Equals(right);
}

internal readonly struct WorldInstanceId : IEquatable<WorldInstanceId>
{
    private readonly Guid _value;

    private WorldInstanceId(Guid value)
    {
        _value = value != Guid.Empty
            ? value
            : throw new ArgumentException("An ephemeral identity cannot use the empty value.", nameof(value));
    }

    internal static WorldInstanceId New() => new(Guid.NewGuid());

    internal bool IsInitialized => _value != Guid.Empty;

    internal void RequireInitialized(string parameterName)
    {
        if (!IsInitialized)
            throw new ArgumentException("A world identity must be initialized.", parameterName);
    }

    public bool Equals(WorldInstanceId other) => _value.Equals(other._value);

    public override bool Equals(object? obj) => obj is WorldInstanceId other && Equals(other);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => IsInitialized ? nameof(WorldInstanceId) : $"{nameof(WorldInstanceId)}(Uninitialized)";

    public static bool operator ==(WorldInstanceId left, WorldInstanceId right) => left.Equals(right);

    public static bool operator !=(WorldInstanceId left, WorldInstanceId right) => !left.Equals(right);
}

internal readonly struct SessionId : IEquatable<SessionId>
{
    private readonly Guid _value;

    private SessionId(Guid value)
    {
        _value = value != Guid.Empty
            ? value
            : throw new ArgumentException("An ephemeral identity cannot use the empty value.", nameof(value));
    }

    internal static SessionId New() => new(Guid.NewGuid());

    internal bool IsInitialized => _value != Guid.Empty;

    internal void RequireInitialized(string parameterName)
    {
        if (!IsInitialized)
            throw new ArgumentException("A session identity must be initialized.", parameterName);
    }

    public bool Equals(SessionId other) => _value.Equals(other._value);

    public override bool Equals(object? obj) => obj is SessionId other && Equals(other);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => IsInitialized ? nameof(SessionId) : $"{nameof(SessionId)}(Uninitialized)";

    public static bool operator ==(SessionId left, SessionId right) => left.Equals(right);

    public static bool operator !=(SessionId left, SessionId right) => !left.Equals(right);
}

internal readonly struct SessionWorldAttachmentId : IEquatable<SessionWorldAttachmentId>
{
    private readonly Guid _value;

    private SessionWorldAttachmentId(Guid value)
    {
        _value = value != Guid.Empty
            ? value
            : throw new ArgumentException("An ephemeral identity cannot use the empty value.", nameof(value));
    }

    internal static SessionWorldAttachmentId New() => new(Guid.NewGuid());

    internal bool IsInitialized => _value != Guid.Empty;

    public bool Equals(SessionWorldAttachmentId other) => _value.Equals(other._value);

    public override bool Equals(object? obj) => obj is SessionWorldAttachmentId other && Equals(other);

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString() => IsInitialized
        ? nameof(SessionWorldAttachmentId)
        : $"{nameof(SessionWorldAttachmentId)}(Uninitialized)";

    public static bool operator ==(SessionWorldAttachmentId left, SessionWorldAttachmentId right) => left.Equals(right);

    public static bool operator !=(SessionWorldAttachmentId left, SessionWorldAttachmentId right) => !left.Equals(right);
}

// TODO(ADR-0043): Generate nominal runtime identity declarations, diagnostic
// formatters, and explicit serialization prohibitions from one reviewed schema.
// These hand-written types are internal, process-incarnation values only: they
// have no raw-value access, parser, codec, or cross-kind identity abstraction.
