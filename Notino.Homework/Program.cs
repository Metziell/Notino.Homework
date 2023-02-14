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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomExceptionHandler();

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