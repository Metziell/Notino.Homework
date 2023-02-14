namespace Notino.Homework.Domain.Interfaces;

public interface IDocumentStore
{
    Task<Document?> GetDocument(string id);
    Task InsertDocument(Document document);
    Task UpdateDocument(Document document);
}
