using System.Reflection;
using Robusta.Runtime.Shared.Catalogs;
using Robusta.Runtime.Shared.Hosting;
using Robusta.Runtime.Shared.Sessions;
using Xunit;

namespace Robusta.Runtime.Tests.Hosting;

public sealed class IdentityContractTests
{
    private static readonly Type[] EphemeralIdentityTypes =
    [
        typeof(HostInstanceId),
        typeof(WorldInstanceId),
        typeof(SessionId),
        typeof(SessionWorldAttachmentId)
    ];

    [Fact]
    public void EphemeralIdentitiesAreInternalOpaqueNominalValues()
    {
        foreach (var identityType in EphemeralIdentityTypes)
        {
            Assert.False(identityType.IsPublic);
            Assert.Empty(identityType.GetConstructors(BindingFlags.Public | BindingFlags.Instance));
            Assert.DoesNotContain(
                identityType.GetFields(BindingFlags.Public | BindingFlags.Instance),
                field => field.FieldType == typeof(Guid));
            Assert.DoesNotContain(
                identityType.GetProperties(BindingFlags.Public | BindingFlags.Instance),
                property => property.PropertyType == typeof(Guid));
            Assert.DoesNotContain(
                identityType.GetMethods(BindingFlags.Public | BindingFlags.Static),
                method => method.Name is "Parse" or "TryParse" || UsesGuid(method));

            var equalityContract = typeof(IEquatable<>).MakeGenericType(identityType);
            Assert.Contains(equalityContract, identityType.GetInterfaces());
        }
    }

    [Fact]
    public async Task RuntimeScopesReceiveFreshKindSpecificIdentities()
    {
        await using var firstHost = new HostScope();
        await using var secondHost = new HostScope();
        var generation = new CatalogGeneration(CatalogGenerationId.NewPlaceholder());
        var firstWorld = firstHost.CreateWorld(generation);
        var secondWorld = firstHost.CreateWorld(generation);
        var firstSession = firstHost.CreateSession();
        var secondSession = firstHost.CreateSession();
        var firstAttachment = firstHost.Attach(firstSession, firstWorld);
        var secondAttachment = firstHost.Attach(secondSession, secondWorld);

        Assert.NotEqual(firstHost.Id, secondHost.Id);
        Assert.NotEqual(firstWorld.Id, secondWorld.Id);
        Assert.NotEqual(firstSession.Id, secondSession.Id);
        Assert.NotEqual(firstAttachment.Id, secondAttachment.Id);
        Assert.Equal(nameof(HostInstanceId), firstHost.Id.ToString());
        Assert.Equal(nameof(WorldInstanceId), firstWorld.Id.ToString());
        Assert.Equal(nameof(SessionId), firstSession.Id.ToString());
        Assert.Equal(nameof(SessionWorldAttachmentId), firstAttachment.Id.ToString());
    }

    [Fact]
    public void OwnershipBoundariesRejectUninitializedIdentities()
    {
        var generation = new CatalogGeneration(CatalogGenerationId.NewPlaceholder());

        Assert.Throws<ArgumentException>(() => new CatalogGeneration(default));
        Assert.Throws<ArgumentException>(() => generation.AcquireLease(default));
        Assert.Throws<ArgumentException>(() => new WorldAttachmentEndpoint(default));
        Assert.Throws<ArgumentException>(() => new SessionAttachmentEndpoint(default));
    }

    private static bool UsesGuid(MethodInfo method) =>
        method.ReturnType == typeof(Guid) ||
        method.GetParameters().Any(parameter => parameter.ParameterType == typeof(Guid));
}
