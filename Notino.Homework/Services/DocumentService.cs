using Microsoft.Extensions.Caching.Memory;

using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;

namespace Notino.Homework.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentStore documentStore;
    private readonly ISerializerFactory serializerFactory;
    private readonly ILogger<IDocumentService> logger;
    private readonly IMemoryCache memoryCache;

    public DocumentService(IDocumentStore documentStore, ISerializerFactory serializerFactory, ILogger<IDocumentService> logger, IMemoryCache memoryCache)
    {
        this.documentStore = documentStore;
        this.serializerFactory = serializerFactory;
        this.logger = logger;
        this.memoryCache = memoryCache;
    }

    public async Task CreateDocument(Document document)
    {
        await documentStore.InsertDocument(document);
        CacheDocument(document);      

        logger.LogInformation("Inserted document {document}", document);
    }

    public async Task UpdateDocument(Document document)
    {
        await documentStore.UpdateDocument(document);
        CacheDocument(document);

        logger.LogInformation("Updated document with id {id}: {document}", document.Id, document);
    }

    public async Task<string?> GetSerializedDocument(string id, FileFormat targetFormat)
    {
        var isCached = memoryCache.TryGetValue<Document?>(id, out var document);
        if (!isCached)
        {
            document = await documentStore.GetDocument(id);
        }
        
        if (document == null)
        {
            throw new MissingDocumentException(id);
        }

        CacheDocument(document);

        var serializer = serializerFactory.Create(targetFormat);
        var serializedData = serializer.Serialize(document);
        if (string.IsNullOrWhiteSpace(serializedData))
        {
            logger.LogWarning("Serialized document {document} in file format {format} is empty", document, targetFormat);
        }

        return serializedData;
    }

    private void CacheDocument(Document document)
    {
        using var entry = memoryCache.CreateEntry(document.Id);
        entry.SetSlidingExpiration(TimeSpan.FromMinutes(3));
        entry.SetValue(document);
    }
}
