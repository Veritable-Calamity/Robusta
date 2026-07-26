namespace Robusta.Runtime.Shared.Catalogs;

internal readonly struct CatalogGenerationId : IEquatable<CatalogGenerationId>
{
    private readonly Guid _placeholder;

    private CatalogGenerationId(Guid placeholder)
    {
        _placeholder = placeholder != Guid.Empty
            ? placeholder
            : throw new ArgumentException("A catalog-generation placeholder cannot be empty.", nameof(placeholder));
    }

    internal static CatalogGenerationId NewPlaceholder() => new(Guid.NewGuid());

    internal bool IsInitialized => _placeholder != Guid.Empty;

    internal void RequireInitialized(string parameterName)
    {
        if (!IsInitialized)
            throw new ArgumentException("A catalog-generation identity must be initialized.", parameterName);
    }

    public bool Equals(CatalogGenerationId other) => _placeholder.Equals(other._placeholder);

    public override bool Equals(object? obj) => obj is CatalogGenerationId other && Equals(other);

    public override int GetHashCode() => _placeholder.GetHashCode();

    public override string ToString() => IsInitialized
        ? nameof(CatalogGenerationId)
        : $"{nameof(CatalogGenerationId)}(Uninitialized)";

    public static bool operator ==(CatalogGenerationId left, CatalogGenerationId right) => left.Equals(right);

    public static bool operator !=(CatalogGenerationId left, CatalogGenerationId right) => !left.Equals(right);

    // TODO(ADR-0021/0043): Replace this internal placeholder factory with the
    // accepted canonical, domain-separated, versioned artifact identity once
    // the catalog descriptor and digest mechanism are selected. This identity
    // must never become an ephemeral runtime incarnation ID.
}
