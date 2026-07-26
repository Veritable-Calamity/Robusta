using Robusta.Runtime.Shared.Catalogs;
using Robusta.Runtime.Shared.Hosting;
using Xunit;

namespace Robusta.Runtime.Tests.Hosting;

public sealed class OwnershipScopeTests
{
    [Fact]
    public async Task HostOwnsSiblingWorldSessionAndAttachmentScopes()
    {
        await using var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var firstWorld = host.CreateWorld(generation);
        var secondWorld = host.CreateWorld(generation);
        var firstSession = host.CreateSession();
        var secondSession = host.CreateSession();

        var firstAttachment = host.Attach(firstSession, firstWorld);
        var secondAttachment = host.Attach(secondSession, firstWorld);
        var thirdAttachment = host.Attach(firstSession, secondWorld);

        Assert.Equal(2, host.WorldCount);
        Assert.Equal(2, host.SessionCount);
        Assert.Equal(3, host.AttachmentCount);
        Assert.Equal(firstSession.Id, firstAttachment.Session.SessionId);
        Assert.Equal(firstWorld.Id, firstAttachment.World.WorldId);
        Assert.Equal(firstWorld.Id, secondAttachment.World.WorldId);
        Assert.Equal(secondWorld.Id, thirdAttachment.World.WorldId);
        Assert.Equal(2, generation.LeaseCount);
    }

    [Fact]
    public async Task ClosingWorldClosesOnlyItsAttachments()
    {
        await using var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var closingWorld = host.CreateWorld(generation);
        var survivingWorld = host.CreateWorld(generation);
        var firstSession = host.CreateSession();
        var secondSession = host.CreateSession();
        var closingAttachment = host.Attach(firstSession, closingWorld);
        var survivingAttachment = host.Attach(secondSession, survivingWorld);

        var result = await closingWorld.CloseAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(OwnershipScopeState.Closed, closingWorld.State);
        Assert.Equal(OwnershipScopeState.Closed, closingAttachment.State);
        Assert.Equal(OwnershipScopeState.Open, survivingWorld.State);
        Assert.Equal(OwnershipScopeState.Open, survivingAttachment.State);
        Assert.Equal(OwnershipScopeState.Open, firstSession.State);
        Assert.Equal(OwnershipScopeState.Open, secondSession.State);
        Assert.Equal(1, host.WorldCount);
        Assert.Equal(2, host.SessionCount);
        Assert.Equal(1, host.AttachmentCount);
        Assert.Equal(1, generation.LeaseCount);
    }

    [Fact]
    public async Task ClosingSessionClosesOnlyItsAttachments()
    {
        await using var host = new HostScope();
        var world = host.CreateWorld(CreateCatalogGeneration());
        var closingSession = host.CreateSession();
        var survivingSession = host.CreateSession();
        var closingAttachment = host.Attach(closingSession, world);
        var survivingAttachment = host.Attach(survivingSession, world);

        var result = await closingSession.CloseAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(OwnershipScopeState.Closed, closingSession.State);
        Assert.Equal(OwnershipScopeState.Closed, closingAttachment.State);
        Assert.Equal(OwnershipScopeState.Open, survivingSession.State);
        Assert.Equal(OwnershipScopeState.Open, survivingAttachment.State);
        Assert.Equal(OwnershipScopeState.Open, world.State);
        Assert.Equal(1, host.WorldCount);
        Assert.Equal(1, host.SessionCount);
        Assert.Equal(1, host.AttachmentCount);
    }

    [Fact]
    public async Task ReattachmentUsesFreshAttachmentIdentity()
    {
        await using var host = new HostScope();
        var world = host.CreateWorld(CreateCatalogGeneration());
        var session = host.CreateSession();
        var firstAttachment = host.Attach(session, world);

        await firstAttachment.CloseAsync();
        var replacementAttachment = host.Attach(session, world);

        Assert.NotEqual(firstAttachment.Id, replacementAttachment.Id);
        Assert.Equal(OwnershipScopeState.Closed, firstAttachment.State);
        Assert.Equal(OwnershipScopeState.Open, replacementAttachment.State);
    }

    [Fact]
    public async Task DirectDetachLeavesBothEndpointsOpen()
    {
        await using var host = new HostScope();
        var world = host.CreateWorld(CreateCatalogGeneration());
        var session = host.CreateSession();
        var attachment = host.Attach(session, world);

        var result = await attachment.CloseAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(OwnershipScopeState.Closed, attachment.State);
        Assert.Equal(OwnershipScopeState.Open, world.State);
        Assert.Equal(OwnershipScopeState.Open, session.State);
        Assert.Equal(1, host.WorldCount);
        Assert.Equal(1, host.SessionCount);
        Assert.Equal(0, host.AttachmentCount);
    }

    [Fact]
    public async Task CatalogGenerationRemainsLeasedUntilItsLastWorldCloses()
    {
        await using var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var firstWorld = host.CreateWorld(generation);
        var secondWorld = host.CreateWorld(generation);
        generation.Retire();

        Assert.False(generation.IsCollectible);
        await firstWorld.CloseAsync();
        Assert.False(generation.IsCollectible);
        await secondWorld.CloseAsync();
        Assert.True(generation.IsCollectible);
    }

    [Fact]
    public async Task WorldsCanPinDifferentCatalogGenerations()
    {
        await using var host = new HostScope();
        var firstGeneration = CreateCatalogGeneration();
        var secondGeneration = CreateCatalogGeneration();

        var firstWorld = host.CreateWorld(firstGeneration);
        var secondWorld = host.CreateWorld(secondGeneration);

        Assert.Equal(firstGeneration.Id, firstWorld.CatalogGeneration.Id);
        Assert.Equal(secondGeneration.Id, secondWorld.CatalogGeneration.Id);
        Assert.NotEqual(firstWorld.CatalogGeneration.Id, secondWorld.CatalogGeneration.Id);
        Assert.Equal(1, firstGeneration.LeaseCount);
        Assert.Equal(1, secondGeneration.LeaseCount);
    }

    [Fact]
    public async Task RetiredCatalogRejectsNewWorldWithoutDisturbingExistingLease()
    {
        await using var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var existingWorld = host.CreateWorld(generation);
        generation.Retire();

        Assert.Throws<InvalidOperationException>(() => host.CreateWorld(generation));
        Assert.Equal(1, host.WorldCount);
        Assert.Equal(1, generation.LeaseCount);
        Assert.Equal(OwnershipScopeState.Open, existingWorld.State);
        Assert.False(generation.IsCollectible);
    }

    [Fact]
    public async Task AttachmentRejectsAnEndpointOwnedByAnotherHost()
    {
        await using var firstHost = new HostScope();
        await using var secondHost = new HostScope();
        var world = firstHost.CreateWorld(CreateCatalogGeneration());
        var foreignSession = secondHost.CreateSession();

        Assert.Throws<InvalidOperationException>(() => firstHost.Attach(foreignSession, world));
        Assert.Equal(0, firstHost.AttachmentCount);
        Assert.Equal(0, secondHost.AttachmentCount);
    }

    [Fact]
    public async Task HostShutdownClosesAttachmentsBeforeEndpointsAndReleasesCatalogs()
    {
        var order = new List<string>();
        var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var world = host.CreateWorld(generation);
        var session = host.CreateSession();
        var attachment = host.Attach(session, world);
        generation.Retire();

        attachment.RegisterCleanup("attachment", _ => RecordAsync(order, "attachment"));
        session.RegisterCleanup("session", _ => RecordAsync(order, "session"));
        world.RegisterCleanup("world", _ => RecordAsync(order, "world"));
        host.RegisterCleanup("host", _ => RecordAsync(order, "host"));

        var result = await host.CloseAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(["attachment", "session", "world", "host"], order);
        Assert.Equal(OwnershipScopeState.Closed, host.State);
        Assert.Equal(OwnershipScopeState.Closed, attachment.State);
        Assert.Equal(OwnershipScopeState.Closed, session.State);
        Assert.Equal(OwnershipScopeState.Closed, world.State);
        Assert.Equal(0, host.AttachmentCount);
        Assert.Equal(0, host.SessionCount);
        Assert.Equal(0, host.WorldCount);
        Assert.True(generation.IsCollectible);
    }

    [Fact]
    public async Task ScopeCleanupRunsInReverseRegistrationOrder()
    {
        await using var host = new HostScope();
        var session = host.CreateSession();
        var order = new List<string>();

        session.RegisterCleanup("first", _ => RecordAsync(order, "first"));
        session.RegisterCleanup("second", _ => RecordAsync(order, "second"));
        session.RegisterCleanup("third", _ => RecordAsync(order, "third"));

        var result = await session.CloseAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(["third", "second", "first"], order);
    }

    [Fact]
    public async Task CleanupAggregatesFailuresAndContinuesClosing()
    {
        await using var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var world = host.CreateWorld(generation);
        var cleanupContinued = false;
        generation.Retire();

        world.RegisterCleanup("continued", _ =>
        {
            cleanupContinued = true;
            return ValueTask.CompletedTask;
        });
        world.RegisterCleanup("failed", _ => ValueTask.FromException(new InvalidOperationException("injected")));

        var result = await world.CloseAsync();

        Assert.False(result.IsSuccess);
        var failure = Assert.Single(result.Failures);
        Assert.Equal(OwnershipScopeKind.World, failure.ScopeKind);
        Assert.Equal("failed", failure.ResourceName);
        Assert.True(cleanupContinued);
        Assert.Equal(OwnershipScopeState.Closed, world.State);
        Assert.True(generation.IsCollectible);
    }

    [Fact]
    public async Task AttachmentCleanupFailureDoesNotCorruptItsEndpoints()
    {
        await using var host = new HostScope();
        var world = host.CreateWorld(CreateCatalogGeneration());
        var session = host.CreateSession();
        var attachment = host.Attach(session, world);
        attachment.RegisterCleanup(
            "injected-failure",
            _ => ValueTask.FromException(new InvalidOperationException("injected")));

        var result = await attachment.CloseAsync();

        Assert.False(result.IsSuccess);
        var failure = Assert.Single(result.Failures);
        Assert.Equal(OwnershipScopeKind.SessionWorldAttachment, failure.ScopeKind);
        Assert.Equal("injected-failure", failure.ResourceName);
        Assert.Equal(OwnershipScopeState.Closed, attachment.State);
        Assert.Equal(OwnershipScopeState.Open, session.State);
        Assert.Equal(OwnershipScopeState.Open, world.State);

        var replacement = host.Attach(session, world);
        Assert.NotEqual(attachment.Id, replacement.Id);
        Assert.Equal(OwnershipScopeState.Open, replacement.State);
    }

    [Fact]
    public async Task ClosingScopeRejectsCleanupRegistrationAndNewAttachment()
    {
        await using var host = new HostScope();
        var world = host.CreateWorld(CreateCatalogGeneration());
        var session = host.CreateSession();
        var cleanupEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseCleanup = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        world.RegisterCleanup("barrier", async _ =>
        {
            cleanupEntered.SetResult();
            await releaseCleanup.Task.ConfigureAwait(false);
        });

        var closeTask = world.CloseAsync().AsTask();
        await cleanupEntered.Task.WaitAsync(TimeSpan.FromSeconds(5));

        try
        {
            Assert.Equal(OwnershipScopeState.Closing, world.State);
            Assert.Throws<InvalidOperationException>(() =>
                world.RegisterCleanup("too-late", _ => ValueTask.CompletedTask));
            Assert.Throws<InvalidOperationException>(() => host.Attach(session, world));
        }
        finally
        {
            releaseCleanup.SetResult();
        }

        var result = await closeTask;
        Assert.True(result.IsSuccess);
        Assert.Equal(OwnershipScopeState.Closed, world.State);
    }

    [Fact]
    public async Task ConcurrentCloseRequestsRunCleanupExactlyOnce()
    {
        await using var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var world = host.CreateWorld(generation);
        var cleanupCount = 0;
        world.RegisterCleanup("counted", _ =>
        {
            Interlocked.Increment(ref cleanupCount);
            return ValueTask.CompletedTask;
        });

        var closeTasks = Enumerable.Range(0, 8)
            .Select(_ => world.CloseAsync().AsTask())
            .ToArray();
        var results = await Task.WhenAll(closeTasks);

        Assert.All(results, result => Assert.True(result.IsSuccess));
        Assert.Equal(1, cleanupCount);
        Assert.Equal(OwnershipScopeState.Closed, world.State);
        Assert.Equal(0, generation.LeaseCount);
        Assert.Equal(0, host.WorldCount);
    }

    [Fact]
    public async Task ClosingScopesRejectNewAdmissionAndCloseIdempotently()
    {
        var host = new HostScope();
        var generation = CreateCatalogGeneration();
        var world = host.CreateWorld(generation);
        var session = host.CreateSession();
        var attachment = host.Attach(session, world);
        var closedWorld = host.CreateWorld(generation);

        Assert.True((await attachment.CloseAsync()).IsSuccess);
        Assert.True((await attachment.CloseAsync()).IsSuccess);
        Assert.True((await closedWorld.CloseAsync()).IsSuccess);
        Assert.Throws<InvalidOperationException>(() => host.Attach(session, closedWorld));

        Assert.True((await host.CloseAsync()).IsSuccess);
        Assert.True((await host.CloseAsync()).IsSuccess);
        Assert.Throws<InvalidOperationException>(() => host.CreateSession());
    }

    private static CatalogGeneration CreateCatalogGeneration() =>
        new(CatalogGenerationId.NewPlaceholder());

    private static ValueTask RecordAsync(List<string> order, string value)
    {
        order.Add(value);
        return ValueTask.CompletedTask;
    }
}
