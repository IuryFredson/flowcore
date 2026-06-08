namespace FlowCore.Api.Domain;

public sealed class Department
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
}

