using Asp.Versioning;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shuttle.Access.AspNetCore;
using Shuttle.Hopper;
using Shuttle.Pigeon.Messages.v1;
using Shuttle.Pigeon.SqlServer;
using Shuttle.Pigeon.SqlServer.Models;

namespace Shuttle.Pigeon.WebApi;

public static class MessageEndpoints
{
    public static WebApplication MapMessageEndpoints(this WebApplication app, ApiVersionSet versionSet)
    {
        var apiVersion1 = new ApiVersion(1, 0);

        app.MapPost("/v{version:apiVersion}/messages", Post)
            .WithTags("Messages")
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        app.MapPost("/v{version:apiVersion}/messages/{id:guid}/attachment", PostAttachment)
            .WithTags("Messages")
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        app.MapPost("/v{version:apiVersion}/messages/{id:guid}/send", PostSendMessageId)
            .WithTags("Messages")
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        app.MapPost("/v{version:apiVersion}/messages/send", PostSendMessage)
            .WithTags("Messages")
            .RequirePermission(Permissions.Messages.Register)
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        return app;
    }

    private static async Task<IResult> PostSendMessage(IBus bus, [FromBody] SendMessage model)
    {
        await bus.SendAsync(model);

        return Results.Accepted();
    }

    private static async Task<IResult> PostSendMessageId(IBus bus, PigeonDbContext dbContext, Guid id)
    {
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

        await bus.SendAsync(new SendRegisteredMessage { Id = id });

        await dbContext.SaveChangesAsync();

        return Results.Accepted();
    }

    private static async Task<IResult> PostAttachment(PigeonDbContext dbContext, [FromBody] Attachment model, Guid id)
    {
        var message = await dbContext.Messages.Include(message => message.Attachments)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (message == null)
        {
            return Results.NotFound();
        }

        if (message.Attachments.Any(item => item.Name == model.Name))
        {
            return Results.BadRequest();
        }

        message.Attachments.Add(new() { Name = model.Name, ContentType = model.ContentType, Content = model.Content });

        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task Post(PigeonDbContext dbContext, [FromBody] SendMessage model)
    {
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
    }
}