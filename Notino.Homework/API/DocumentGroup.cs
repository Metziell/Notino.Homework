using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;

namespace Notino.Homework.API;

public static class DocumentGroup
{
    public static void MapDocumentGroup(this IEndpointRouteBuilder endpointRouteBuilder)
    {
        var documentsGroup = endpointRouteBuilder.MapGroup("document").WithOpenApi();

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
    }
}
