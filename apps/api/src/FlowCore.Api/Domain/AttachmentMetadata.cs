namespace FlowCore.Api.Domain;

public sealed class AttachmentMetadata
{
    public Guid Id { get; set; }
    public Guid ProtocolId { get; set; }
    public Protocol? Protocol { get; set; }
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public long SizeInBytes { get; set; }
    public Guid UploadedById { get; set; }
    public AppUser? UploadedBy { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}

