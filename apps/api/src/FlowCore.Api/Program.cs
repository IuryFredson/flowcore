using FlowCore.Api.Contracts;
using FlowCore.Api.Data;
using FlowCore.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FlowCoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FlowCore")));
builder.Services.AddScoped<ProtocolWorkflowService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().SetIsOriginAllowed(_ => true));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.MapGet("/health", () => Results.Ok(new { status = "ok", service = "FlowCore.Api" }));

app.MapGet("/protocols", async (FlowCoreDbContext db, CancellationToken cancellationToken) =>
{
    var protocols = await db.Protocols
        .OrderByDescending(x => x.CreatedAt)
        .Select(x => new
        {
            x.Id,
            x.Number,
            x.Subject,
            Status = x.Status.ToString(),
            x.CurrentDepartmentId,
            x.AssignedUserId,
            x.CreatedAt,
            x.UpdatedAt
        })
        .ToListAsync(cancellationToken);

    return Results.Ok(protocols);
});

app.MapGet("/protocols/{id:guid}", async (Guid id, FlowCoreDbContext db, CancellationToken cancellationToken) =>
{
    var protocol = await db.Protocols
        .Include(x => x.Movements.OrderByDescending(m => m.CreatedAt))
        .Include(x => x.Attachments.OrderByDescending(a => a.UploadedAt))
        .Include(x => x.AuditLogs.OrderByDescending(a => a.CreatedAt))
        .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return protocol is null ? Results.NotFound() : Results.Ok(protocol);
});

app.MapPost("/protocols", async (
    CreateProtocolRequest request,
    ProtocolWorkflowService service,
    CancellationToken cancellationToken) =>
{
    var protocol = await service.CreateAsync(request, cancellationToken);
    return Results.Created($"/protocols/{protocol.Id}", protocol);
});

app.MapPost("/protocols/{id:guid}/transitions", async (
    Guid id,
    TransitionProtocolRequest request,
    ProtocolWorkflowService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await service.TransitionAsync(id, request, cancellationToken));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/protocols/{id:guid}/forward", async (
    Guid id,
    ForwardProtocolRequest request,
    ProtocolWorkflowService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Ok(await service.ForwardAsync(id, request, cancellationToken));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/protocols/{id:guid}/attachments", async (
    Guid id,
    AddAttachmentRequest request,
    ProtocolWorkflowService service,
    CancellationToken cancellationToken) =>
{
    try
    {
        return Results.Created($"/protocols/{id}/attachments", await service.AddAttachmentAsync(id, request, cancellationToken));
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.Run();

public partial class Program;

