using FlowCore.Api.Contracts;
using FlowCore.Api.Data;
using FlowCore.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Api.Services;

public sealed class ProtocolWorkflowService(FlowCoreDbContext db)
{
    private static readonly IReadOnlyDictionary<ProtocolStatus, ProtocolStatus[]> AllowedTransitions =
        new Dictionary<ProtocolStatus, ProtocolStatus[]>
        {
            [ProtocolStatus.Draft] = [ProtocolStatus.Open, ProtocolStatus.Closed],
            [ProtocolStatus.Open] = [ProtocolStatus.InAnalysis, ProtocolStatus.WaitingForDocuments, ProtocolStatus.Closed],
            [ProtocolStatus.InAnalysis] = [ProtocolStatus.WaitingForDocuments, ProtocolStatus.Approved, ProtocolStatus.Rejected],
            [ProtocolStatus.WaitingForDocuments] = [ProtocolStatus.InAnalysis, ProtocolStatus.Closed],
            [ProtocolStatus.Approved] = [ProtocolStatus.Closed],
            [ProtocolStatus.Rejected] = [ProtocolStatus.Closed],
            [ProtocolStatus.Closed] = []
        };

    public static bool CanTransition(ProtocolStatus from, ProtocolStatus to)
    {
        return AllowedTransitions.TryGetValue(from, out var targets) && targets.Contains(to);
    }

    public async Task<Protocol> CreateAsync(CreateProtocolRequest request, CancellationToken cancellationToken)
    {
        var now = DateTimeOffset.UtcNow;
        var protocol = new Protocol
        {
            Id = Guid.NewGuid(),
            Number = $"FC-{now:yyyyMMddHHmmss}",
            Subject = request.Subject.Trim(),
            Description = request.Description?.Trim(),
            CreatedById = request.CreatedById,
            CurrentDepartmentId = request.CurrentDepartmentId,
            AssignedUserId = request.AssignedUserId,
            CreatedAt = now,
            UpdatedAt = now
        };

        protocol.Movements.Add(new ProtocolMovement
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.CreatedById,
            DestinationDepartmentId = request.CurrentDepartmentId,
            Action = "Created",
            ToStatus = ProtocolStatus.Draft,
            CreatedAt = now
        });

        protocol.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.CreatedById,
            Action = "ProtocolCreated",
            Details = $"Protocol {protocol.Number} created as Draft.",
            CreatedAt = now
        });

        db.Protocols.Add(protocol);
        await db.SaveChangesAsync(cancellationToken);
        return protocol;
    }

    public async Task<Protocol> TransitionAsync(Guid protocolId, TransitionProtocolRequest request, CancellationToken cancellationToken)
    {
        var protocol = await LoadProtocolAsync(protocolId, cancellationToken);

        if (!CanTransition(protocol.Status, request.Status))
        {
            throw new InvalidOperationException($"Cannot transition protocol from {protocol.Status} to {request.Status}.");
        }

        var previousStatus = protocol.Status;
        var now = DateTimeOffset.UtcNow;
        protocol.Status = request.Status;
        protocol.UpdatedAt = now;
        protocol.Movements.Add(new ProtocolMovement
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.ActorUserId,
            FromStatus = previousStatus,
            ToStatus = request.Status,
            Action = "StatusChanged",
            Note = request.Note,
            CreatedAt = now
        });
        protocol.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.ActorUserId,
            Action = "ProtocolStatusChanged",
            Details = $"Status changed from {previousStatus} to {request.Status}.",
            CreatedAt = now
        });

        await db.SaveChangesAsync(cancellationToken);
        return protocol;
    }

    public async Task<Protocol> ForwardAsync(Guid protocolId, ForwardProtocolRequest request, CancellationToken cancellationToken)
    {
        var protocol = await LoadProtocolAsync(protocolId, cancellationToken);
        EnsureMutable(protocol);

        var originDepartmentId = protocol.CurrentDepartmentId;
        var now = DateTimeOffset.UtcNow;
        protocol.CurrentDepartmentId = request.DestinationDepartmentId;
        protocol.AssignedUserId = request.AssignedUserId;
        protocol.UpdatedAt = now;
        protocol.Movements.Add(new ProtocolMovement
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.ActorUserId,
            OriginDepartmentId = originDepartmentId,
            DestinationDepartmentId = request.DestinationDepartmentId,
            Action = "Forwarded",
            Note = request.Note,
            CreatedAt = now
        });
        protocol.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.ActorUserId,
            Action = "ProtocolForwarded",
            Details = $"Forwarded from department {originDepartmentId} to {request.DestinationDepartmentId}.",
            CreatedAt = now
        });

        await db.SaveChangesAsync(cancellationToken);
        return protocol;
    }

    public async Task<AttachmentMetadata> AddAttachmentAsync(Guid protocolId, AddAttachmentRequest request, CancellationToken cancellationToken)
    {
        var protocol = await LoadProtocolAsync(protocolId, cancellationToken);
        EnsureMutable(protocol);

        var attachment = new AttachmentMetadata
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            FileName = request.FileName.Trim(),
            ContentType = request.ContentType.Trim(),
            SizeInBytes = request.SizeInBytes,
            UploadedById = request.UploadedById,
            UploadedAt = DateTimeOffset.UtcNow
        };

        db.Attachments.Add(attachment);
        db.AuditLogs.Add(new AuditLog
        {
            Id = Guid.NewGuid(),
            ProtocolId = protocol.Id,
            ActorUserId = request.UploadedById,
            Action = "AttachmentMetadataAdded",
            Details = $"Attachment metadata added for {attachment.FileName}.",
            CreatedAt = attachment.UploadedAt
        });
        await db.SaveChangesAsync(cancellationToken);
        return attachment;
    }

    private async Task<Protocol> LoadProtocolAsync(Guid protocolId, CancellationToken cancellationToken)
    {
        return await db.Protocols
            .Include(x => x.Movements)
            .Include(x => x.AuditLogs)
            .SingleOrDefaultAsync(x => x.Id == protocolId, cancellationToken)
            ?? throw new KeyNotFoundException($"Protocol {protocolId} was not found.");
    }

    private static void EnsureMutable(Protocol protocol)
    {
        if (protocol.IsClosed)
        {
            throw new InvalidOperationException("A closed protocol cannot be modified.");
        }
    }
}

