using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json;
using Notino.Homework.Domain;
using System.Net;

namespace Notino.Homework.Middleware;

public static class ExceptionMiddlewareExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app) => app.UseExceptionHandler(builder =>
    {
        builder.Run(async context =>
        {
            context.Response.ContentType = "application/json";

            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
            if (exceptionHandlerFeature is not null)
            {
                var exception = exceptionHandlerFeature.Error;
                var statusCode = exception switch
                {
                    MissingDocumentException _ => HttpStatusCode.NotFound,
                    DuplicateDocumentException _ => HttpStatusCode.BadRequest,
                    ValidationException _ => HttpStatusCode.BadRequest,
                    _ => HttpStatusCode.InternalServerError,
                };

                var logger = context.RequestServices.GetService<ILogger>();
                logger?.LogError(
                    exception,
                    "{RequestMethod} {RequestPath} {RequestQuery}: {Message}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    exception.Message);

                context.Response.StatusCode = (int)statusCode;
                var response = JsonConvert.SerializeObject(new
                {
                    errorCode = exception.GetType().Name,
                    message = exception.Message
                });

                await context.Response.WriteAsync(response);
            }
        });
    });
}
