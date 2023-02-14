using FluentValidation;

using Microsoft.Extensions.Caching.Memory;

using Notino.Homework.API;
using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;
using Notino.Homework.Domain.Serializer;
using Notino.Homework.Infrastructure;
using Notino.Homework.Middleware;
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

app.MapDocumentGroup();

app.Run();