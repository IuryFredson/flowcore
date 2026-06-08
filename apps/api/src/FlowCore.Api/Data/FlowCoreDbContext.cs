using FlowCore.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlowCore.Api.Data;

public sealed class FlowCoreDbContext(DbContextOptions<FlowCoreDbContext> options) : DbContext(options)
{
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Protocol> Protocols => Set<Protocol>();
    public DbSet<ProtocolMovement> ProtocolMovements => Set<ProtocolMovement>();
    public DbSet<AttachmentMetadata> Attachments => Set<AttachmentMetadata>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(x => x.Code).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Code).HasMaxLength(20).IsRequired();
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Name).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(30);
            entity.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<Protocol>(entity =>
        {
            entity.HasIndex(x => x.Number).IsUnique();
            entity.Property(x => x.Number).HasMaxLength(32).IsRequired();
            entity.Property(x => x.Subject).HasMaxLength(180).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(40);
            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.AssignedUser)
                .WithMany()
                .HasForeignKey(x => x.AssignedUserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.CurrentDepartment)
                .WithMany()
                .HasForeignKey(x => x.CurrentDepartmentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ProtocolMovement>(entity =>
        {
            entity.Property(x => x.Action).HasMaxLength(60).IsRequired();
            entity.Property(x => x.Note).HasMaxLength(1000);
            entity.Property(x => x.FromStatus).HasConversion<string>().HasMaxLength(40);
            entity.Property(x => x.ToStatus).HasConversion<string>().HasMaxLength(40);
            entity.HasOne(x => x.Protocol)
                .WithMany(x => x.Movements)
                .HasForeignKey(x => x.ProtocolId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.ActorUser)
                .WithMany()
                .HasForeignKey(x => x.ActorUserId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.OriginDepartment)
                .WithMany()
                .HasForeignKey(x => x.OriginDepartmentId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.DestinationDepartment)
                .WithMany()
                .HasForeignKey(x => x.DestinationDepartmentId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AttachmentMetadata>(entity =>
        {
            entity.Property(x => x.FileName).HasMaxLength(240).IsRequired();
            entity.Property(x => x.ContentType).HasMaxLength(120).IsRequired();
            entity.HasOne(x => x.Protocol)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.ProtocolId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedById)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.Property(x => x.Action).HasMaxLength(80).IsRequired();
            entity.Property(x => x.Details).HasMaxLength(1200).IsRequired();
            entity.HasOne(x => x.Protocol)
                .WithMany(x => x.AuditLogs)
                .HasForeignKey(x => x.ProtocolId)
                .OnDelete(DeleteBehavior.NoAction);
            entity.HasOne(x => x.ActorUser)
                .WithMany()
                .HasForeignKey(x => x.ActorUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
