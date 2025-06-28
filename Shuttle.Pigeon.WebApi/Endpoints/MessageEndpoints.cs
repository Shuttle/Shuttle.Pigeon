using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shuttle.Access.AspNetCore;
using Shuttle.Esb;
using Shuttle.Extensions.EFCore;
using Shuttle.Pigeon.Data;
using Shuttle.Pigeon.Data.Models;
using Shuttle.Pigeon.Messages.v1;

namespace Shuttle.Pigeon.WebApi;

public static class MessageEndpoints
{
    public static WebApplication MapMessageEndpoints(this WebApplication app, ApiVersionSet versionSet)
    {
        var apiVersion1 = new ApiVersion(1, 0);

        app.MapPost("/v{version:apiVersion}/messages", async (IDbContextFactory<PigeonDbContext> dbContextFactory, [FromBody] SendMessage model) =>
            {
                using var scope = new DbContextScope();
                var dbContext = await dbContextFactory.CreateDbContextAsync();

                dbContext.Messages.Add(new()
                {
                    Id = model.Id,
                    Channel = model.Channel,
                    Sender = model.Sender,
                    Subject = model.Subject,
                    Content = model.Content,
                    ContentType = model.ContentType,
                    DateRegistered = DateTime.Now.ToUniversalTime(),
                    Recipients = model.Recipients.Select(item => new MessageRecipient { Identifier = item.Identifier, Type = item.Type }).ToList()
                });

                await dbContext.SaveChangesAsync();
            })
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        app.MapPost("/v{version:apiVersion}/messages/{id:guid}/attachment", async (IDbContextFactory<PigeonDbContext> dbContextFactory, [FromBody] Attachment model, Guid id) =>
            {
                using var scope = new DbContextScope();
                var dbContext = await dbContextFactory.CreateDbContextAsync();

                var message = await dbContext.Messages
                    .Include(message => message.Attachments)
                    .FirstOrDefaultAsync(item => item.Id == id);

                if (message == null)
                {
                    return Results.NotFound();
                }

                if (message.Attachments.Any(item => item.Name == model.Name))
                {
                    return Results.BadRequest();
                }

                message.Attachments.Add(new()
                {
                    Name = model.Name,
                    ContentType = model.ContentType,
                    Content = model.Content
                });

                await dbContext.SaveChangesAsync();

                return Results.Ok();
            })
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        app.MapPost("/v{version:apiVersion}/messages/{id:guid}/send", async (IServiceBus serviceBus, IDbContextFactory<PigeonDbContext> dbContextFactory, Guid id) =>
            {
                using var scope = new DbContextScope();
                var dbContext = await dbContextFactory.CreateDbContextAsync();

                var message = await dbContext.Messages.FirstOrDefaultAsync(item => item.Id == id);

                if (message == null)
                {
                    return Results.NotFound();
                }

                if (message.DateAccepted.HasValue)
                {
                    return Results.Accepted();
                }

                message.DateAccepted = DateTime.Now.ToUniversalTime();

                await serviceBus.SendAsync(new SendRegisteredMessage
                {
                    Id = id
                });

                await dbContext.SaveChangesAsync();

                return Results.Accepted();
            })
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        app.MapPost("/v{version:apiVersion}/messages/send", async (IServiceBus serviceBus, [FromBody] SendMessage model) =>
            {
                await serviceBus.SendAsync(model);

                return Results.Accepted();
            })
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        return app;
    }
}