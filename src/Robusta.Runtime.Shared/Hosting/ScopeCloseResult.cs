namespace Robusta.Runtime.Shared.Hosting;

internal enum OwnershipScopeKind
{
    Host,
    World,
    Session,
    SessionWorldAttachment
}

internal sealed record ScopeCleanupFailure(
    OwnershipScopeKind ScopeKind,
    string ResourceName,
    Exception Exception);

internal sealed class ScopeCloseResult
{
    public static ScopeCloseResult Success { get; } = new([]);

    public ScopeCloseResult(IReadOnlyList<ScopeCleanupFailure> failures)
    {
        Failures = failures;
    }

    public IReadOnlyList<ScopeCleanupFailure> Failures { get; }

    public bool IsSuccess => Failures.Count == 0;

    public void ThrowIfFailed()
    {
        if (IsSuccess)
            return;

        throw new AggregateException(
            "One or more ownership-scope cleanup operations failed.",
            Failures.Select(failure => failure.Exception));
    }

    internal static ScopeCloseResult Combine(IEnumerable<ScopeCloseResult> results)
    {
        var failures = results.SelectMany(result => result.Failures).ToArray();
        return failures.Length == 0 ? Success : new ScopeCloseResult(failures);
    }
}
