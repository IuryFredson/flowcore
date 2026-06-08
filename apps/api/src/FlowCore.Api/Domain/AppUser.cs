namespace FlowCore.Api.Domain;

public sealed class AppUser
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public UserRole Role { get; set; }
    public Guid DepartmentId { get; set; }
    public Department? Department { get; set; }
}

