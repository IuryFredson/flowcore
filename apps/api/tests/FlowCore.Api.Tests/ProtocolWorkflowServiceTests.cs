using FlowCore.Api.Contracts;
using FlowCore.Api.Data;
using FlowCore.Api.Domain;
using FlowCore.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Api.Tests;

public sealed class ProtocolWorkflowServiceTests
{
    [Fact]
    public void CanTransitionRejectsInvalidStatusJump()
    {
        Assert.False(ProtocolWorkflowService.CanTransition(ProtocolStatus.Draft, ProtocolStatus.Approved));
    }

    [Fact]
    public void CanTransitionAllowsApprovalToClose()
    {
        Assert.True(ProtocolWorkflowService.CanTransition(ProtocolStatus.Approved, ProtocolStatus.Closed));
    }

    [Fact]
    public async Task TransitionAsyncCreatesMovementAndAuditRecord()
    {
        await using var db = CreateDbContext();
        var protocol = SeedProtocol(db, ProtocolStatus.Open);
        var service = new ProtocolWorkflowService(db);

        await service.TransitionAsync(protocol.Id, new TransitionProtocolRequest(
            ProtocolStatus.InAnalysis,
            protocol.CreatedById,
            "Ready for analysis"), CancellationToken.None);

        var updated = await db.Protocols
            .Include(x => x.Movements)
            .Include(x => x.AuditLogs)
            .SingleAsync(x => x.Id == protocol.Id);

        Assert.Equal(ProtocolStatus.InAnalysis, updated.Status);
        Assert.Contains(updated.Movements, x => x.Action == "StatusChanged" && x.FromStatus == ProtocolStatus.Open);
        Assert.Contains(updated.AuditLogs, x => x.Action == "ProtocolStatusChanged");
    }

    [Fact]
    public async Task ForwardAsyncRejectsClosedProtocol()
    {
        await using var db = CreateDbContext();
        var protocol = SeedProtocol(db, ProtocolStatus.Closed);
        var service = new ProtocolWorkflowService(db);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.ForwardAsync(protocol.Id, new ForwardProtocolRequest(
                protocol.CreatedById,
                Guid.NewGuid(),
                null,
                "Cannot forward"), CancellationToken.None));

        Assert.Equal("A closed protocol cannot be modified.", ex.Message);
    }

    private static FlowCoreDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<FlowCoreDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new FlowCoreDbContext(options);
    }

    private static Protocol SeedProtocol(FlowCoreDbContext db, ProtocolStatus status)
    {
        var departmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var department = new Department { Id = departmentId, Name = "Protocol Desk", Code = "PROTO" };
        var user = new AppUser
        {
            Id = userId,
            Name = "Ana Silva",
            Email = "ana.silva@example.test",
            Role = UserRole.Analyst,
            DepartmentId = departmentId
        };
        var protocol = new Protocol
        {
            Id = Guid.NewGuid(),
            Number = ($"FC-{Guid.NewGuid():N}")[..20],
            Subject = "License request",
            Status = status,
            CreatedById = userId,
            CurrentDepartmentId = departmentId,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        db.Departments.Add(department);
        db.Users.Add(user);
        db.Protocols.Add(protocol);
        db.SaveChanges();
        return protocol;
    }
}
