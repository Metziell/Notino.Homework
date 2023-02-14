using System.Net;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(builder => builder.AddConsole());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(builder =>
{
    builder.Run(async context =>
    {
        context.Response.ContentType = "application/json";

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature is not null)
        {
            var exception = exceptionHandlerFeature.Error;
            var logger = context.RequestServices.GetRequiredService<ILogger>();
            logger.LogError(
                exception,
                "{RequestMethod} {RequestPath} {RequestQuery}: {Message}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString,
                exception.Message);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 
            var response = JsonConvert.SerializeObject(new
            {
                errorCode = exception.GetType().Name,
                message = exception.Message
            });

            await context.Response.WriteAsync(response);
        }
    });
});

var documentsGroup = app.MapGroup("documents").WithOpenApi();

app.Run();