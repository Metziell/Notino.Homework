namespace Notino.Homework.Domain.Interfaces;
public interface IDocumentService
{
    Task CreateDocument(Document document);
    Task<string?> GetSerializedDocument(string id, FileFormat targetFormat);
    Task UpdateDocument(Document document);
}