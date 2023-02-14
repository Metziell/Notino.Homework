namespace Notino.Homework.Domain.Interfaces;
public interface IDocumentService
{
    Task CreateDocument(Document document);
    Task<string?> GetSerializedDocument(string id, string targetFormat);
    Task UpdateDocument(Document document);
}