namespace FlowCore.Api.Domain;

public sealed class ProtocolMovement
{
    public Guid Id { get; set; }
    public Guid ProtocolId { get; set; }
    public Protocol? Protocol { get; set; }
    public ProtocolStatus? FromStatus { get; set; }
    public ProtocolStatus? ToStatus { get; set; }
    public Guid? OriginDepartmentId { get; set; }
    public Department? OriginDepartment { get; set; }
    public Guid? DestinationDepartmentId { get; set; }
    public Department? DestinationDepartment { get; set; }
    public Guid ActorUserId { get; set; }
    public AppUser? ActorUser { get; set; }
    public required string Action { get; set; }
    public string? Note { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

