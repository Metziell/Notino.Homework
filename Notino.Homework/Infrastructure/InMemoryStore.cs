using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;

namespace Notino.Homework.Infrastructure;

public class InMemoryStore : IDocumentStore
{
    private readonly List<Document> documents = new();
    private readonly object lockObject = new();

    public Task<Document?> GetDocument(string id)
    {
        lock (lockObject)
        {
            return Task.FromResult(documents.FirstOrDefault(x => x.Id == id));
        }
    }

    public Task InsertDocument(Document document)
    {
        lock (lockObject)
        {
            var existing = documents.FirstOrDefault(x => x.Id == document.Id);
            if (existing != null)
            {
                throw new DuplicateDocumentException(document.Id);
            }

            documents.Add(document);
            return Task.CompletedTask;
        }
    }

    public Task UpdateDocument(Document document)
    {
        lock (lockObject)
        {
            var index = documents.FindIndex(x => x.Id == document.Id);
            if (index == -1)
            {
                throw new MissingDocumentException(document.Id);
            }

            documents[index] = document;

            return Task.CompletedTask;
        }
    }
}
