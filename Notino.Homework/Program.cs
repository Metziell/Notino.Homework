using System.Net;

using FluentValidation;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using Newtonsoft.Json;

using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;
using Notino.Homework.Domain.Serializer;
using Notino.Homework.Infrastructure;
using Notino.Homework.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddSingleton<ISerializerFactory, SerializerFactory>();
builder.Services.AddSingleton<IDocumentStore, InMemoryStore>();
builder.Services.AddSingleton<IFileFormatMapper, FileFormatMapper>();
builder.Services.AddScoped<IValidator<Document>, DocumentValidator>();
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

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
            var statusCode = exception switch
            {
                MissingDocumentException _ => HttpStatusCode.NotFound,
                DuplicateDocumentException _ => HttpStatusCode.BadRequest,
                ValidationException _ => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError,
            };

            var logger = context.RequestServices.GetRequiredService<ILogger>();
            logger.LogError(
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

var documentsGroup = app.MapGroup("documents").WithOpenApi();
documentsGroup.MapPost("", async ([FromBody] Document doc, IDocumentService documentService, IValidator<Document> validator) => 
{
    validator.ValidateAndThrow(doc);
    await documentService.CreateDocument(doc);
    return Results.Ok();
});
documentsGroup.MapPut("", async ([FromBody] Document doc, IDocumentService documentService, IValidator<Document> validator) => 
{
    validator.ValidateAndThrow(doc);
    await documentService.UpdateDocument(doc);
    return Results.Ok();
});
documentsGroup.MapGet("{id}", async ([FromQuery] string id, HttpContext context, IDocumentService documentService) =>
{
    var targetFormat = context.Request.Headers.Accept.FirstOrDefault() ?? "application/json"; // TODO check if Accept can be null or if it's empty instead
    var response = await documentService.GetSerializedDocument(id, targetFormat);
    return Results.Ok(response);    
});

app.Run();