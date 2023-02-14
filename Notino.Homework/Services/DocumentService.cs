using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;

namespace Notino.Homework.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentStore documentStore;
    private readonly ISerializerFactory serializerFactory;
    private readonly IFileFormatMapper fileFormatMapper;
    private readonly ILogger<IDocumentService> logger;

    public DocumentService(IDocumentStore documentStore, ISerializerFactory serializerFactory, IFileFormatMapper fileFormatMapper, ILogger<IDocumentService> logger)
    {
        this.documentStore = documentStore;
        this.serializerFactory = serializerFactory;
        this.fileFormatMapper = fileFormatMapper;
        this.logger = logger;
    }

    public async Task CreateDocument(Document document)
    {
        await documentStore.InsertDocument(document);
        logger.LogInformation("Inserted document {document}", document);
    }

    public async Task UpdateDocument(Document document)
    {
        await documentStore.UpdateDocument(document);
        logger.LogInformation("Updated document with id {id}: {document}", document.Id, document);
    }

    public async Task<string?> GetSerializedDocument(string id, string targetFormat)
    {
        var document = await documentStore.GetDocument(id);
        if (document == null)
        {
            throw new MissingDocumentException(id);
        }

        var fileFormat = fileFormatMapper.Map(targetFormat);
        var serializer = serializerFactory.Create(fileFormat);
        var serializedData = serializer.Serialize(document);
        if (string.IsNullOrWhiteSpace(serializedData))
        {
            logger.LogWarning("Serialized document {document} in file format {format} is empty", document, fileFormat);
        }

        return serializedData;
    }
}
