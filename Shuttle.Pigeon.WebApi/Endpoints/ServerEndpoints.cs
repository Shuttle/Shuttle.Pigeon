﻿using System.Reflection;
using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Shuttle.Pigeon.WebApi;

public static class ServerEndpoints
{
    public static WebApplication MapServerEndpoints(this WebApplication app, ApiVersionSet versionSet)
    {
        var apiVersion1 = new ApiVersion(1, 0);

        app.MapGet("/v{version:apiVersion}/server/configuration", () =>
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);

                return Results.Ok(new { Version = $"{version.Major}.{version.Minor}.{version.Build}" });
            })
            .WithTags("Server")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(apiVersion1);

        return app;
    }
}