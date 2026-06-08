namespace FlowCore.Api.Domain;

public sealed class AuditLog
{
    public Guid Id { get; set; }
    public Guid ProtocolId { get; set; }
    public Protocol? Protocol { get; set; }
    public Guid ActorUserId { get; set; }
    public AppUser? ActorUser { get; set; }
    public required string Action { get; set; }
    public required string Details { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

