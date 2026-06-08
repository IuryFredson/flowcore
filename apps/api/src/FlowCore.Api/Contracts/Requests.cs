using FlowCore.Api.Domain;

namespace FlowCore.Api.Contracts;

public sealed record CreateProtocolRequest(
    string Subject,
    string? Description,
    Guid CreatedById,
    Guid CurrentDepartmentId,
    Guid? AssignedUserId);

public sealed record TransitionProtocolRequest(
    ProtocolStatus Status,
    Guid ActorUserId,
    string? Note);

public sealed record ForwardProtocolRequest(
    Guid ActorUserId,
    Guid DestinationDepartmentId,
    Guid? AssignedUserId,
    string? Note);

public sealed record AddAttachmentRequest(
    string FileName,
    string ContentType,
    long SizeInBytes,
    Guid UploadedById);

