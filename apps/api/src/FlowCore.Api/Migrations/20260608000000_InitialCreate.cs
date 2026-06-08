using System;
using FlowCore.Api.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlowCore.Api.Migrations;

[DbContext(typeof(FlowCoreDbContext))]
[Migration("20260608000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Departments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Departments", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                Email = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
                table.ForeignKey(
                    name: "FK_Users_Departments_DepartmentId",
                    column: x => x.DepartmentId,
                    principalTable: "Departments",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Protocols",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Number = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                Subject = table.Column<string>(type: "nvarchar(180)", maxLength: 180, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                Status = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                CreatedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                AssignedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                CurrentDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Protocols", x => x.Id);
                table.ForeignKey(
                    name: "FK_Protocols_Departments_CurrentDepartmentId",
                    column: x => x.CurrentDepartmentId,
                    principalTable: "Departments",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Protocols_Users_AssignedUserId",
                    column: x => x.AssignedUserId,
                    principalTable: "Users",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Protocols_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Attachments",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProtocolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FileName = table.Column<string>(type: "nvarchar(240)", maxLength: 240, nullable: false),
                ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                SizeInBytes = table.Column<long>(type: "bigint", nullable: false),
                UploadedById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UploadedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Attachments", x => x.Id);
                table.ForeignKey(
                    name: "FK_Attachments_Protocols_ProtocolId",
                    column: x => x.ProtocolId,
                    principalTable: "Protocols",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Attachments_Users_UploadedById",
                    column: x => x.UploadedById,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "AuditLogs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProtocolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                Details = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuditLogs", x => x.Id);
                table.ForeignKey(
                    name: "FK_AuditLogs_Protocols_ProtocolId",
                    column: x => x.ProtocolId,
                    principalTable: "Protocols",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_AuditLogs_Users_ActorUserId",
                    column: x => x.ActorUserId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "ProtocolMovements",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ProtocolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FromStatus = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                ToStatus = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                OriginDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                DestinationDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Action = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProtocolMovements", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProtocolMovements_Departments_DestinationDepartmentId",
                    column: x => x.DestinationDepartmentId,
                    principalTable: "Departments",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_ProtocolMovements_Departments_OriginDepartmentId",
                    column: x => x.OriginDepartmentId,
                    principalTable: "Departments",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_ProtocolMovements_Protocols_ProtocolId",
                    column: x => x.ProtocolId,
                    principalTable: "Protocols",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_ProtocolMovements_Users_ActorUserId",
                    column: x => x.ActorUserId,
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(name: "IX_Attachments_ProtocolId", table: "Attachments", column: "ProtocolId");
        migrationBuilder.CreateIndex(name: "IX_Attachments_UploadedById", table: "Attachments", column: "UploadedById");
        migrationBuilder.CreateIndex(name: "IX_AuditLogs_ActorUserId", table: "AuditLogs", column: "ActorUserId");
        migrationBuilder.CreateIndex(name: "IX_AuditLogs_ProtocolId", table: "AuditLogs", column: "ProtocolId");
        migrationBuilder.CreateIndex(name: "IX_Departments_Code", table: "Departments", column: "Code", unique: true);
        migrationBuilder.CreateIndex(name: "IX_ProtocolMovements_ActorUserId", table: "ProtocolMovements", column: "ActorUserId");
        migrationBuilder.CreateIndex(name: "IX_ProtocolMovements_DestinationDepartmentId", table: "ProtocolMovements", column: "DestinationDepartmentId");
        migrationBuilder.CreateIndex(name: "IX_ProtocolMovements_OriginDepartmentId", table: "ProtocolMovements", column: "OriginDepartmentId");
        migrationBuilder.CreateIndex(name: "IX_ProtocolMovements_ProtocolId", table: "ProtocolMovements", column: "ProtocolId");
        migrationBuilder.CreateIndex(name: "IX_Protocols_AssignedUserId", table: "Protocols", column: "AssignedUserId");
        migrationBuilder.CreateIndex(name: "IX_Protocols_CreatedById", table: "Protocols", column: "CreatedById");
        migrationBuilder.CreateIndex(name: "IX_Protocols_CurrentDepartmentId", table: "Protocols", column: "CurrentDepartmentId");
        migrationBuilder.CreateIndex(name: "IX_Protocols_Number", table: "Protocols", column: "Number", unique: true);
        migrationBuilder.CreateIndex(name: "IX_Users_DepartmentId", table: "Users", column: "DepartmentId");
        migrationBuilder.CreateIndex(name: "IX_Users_Email", table: "Users", column: "Email", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Attachments");
        migrationBuilder.DropTable(name: "AuditLogs");
        migrationBuilder.DropTable(name: "ProtocolMovements");
        migrationBuilder.DropTable(name: "Protocols");
        migrationBuilder.DropTable(name: "Users");
        migrationBuilder.DropTable(name: "Departments");
    }
}
