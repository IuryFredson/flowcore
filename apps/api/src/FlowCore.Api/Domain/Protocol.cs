namespace FlowCore.Api.Domain;

public sealed class Protocol
{
    public Guid Id { get; set; }
    public required string Number { get; set; }
    public required string Subject { get; set; }
    public string? Description { get; set; }
    public ProtocolStatus Status { get; set; } = ProtocolStatus.Draft;
    public Guid CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public Guid? AssignedUserId { get; set; }
    public AppUser? AssignedUser { get; set; }
    public Guid CurrentDepartmentId { get; set; }
    public Department? CurrentDepartment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<ProtocolMovement> Movements { get; set; } = [];
    public List<AttachmentMetadata> Attachments { get; set; } = [];
    public List<AuditLog> AuditLogs { get; set; } = [];

    public bool IsClosed => Status == ProtocolStatus.Closed;
}

