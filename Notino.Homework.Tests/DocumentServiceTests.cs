using AutoFixture;

using FluentAssertions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using Moq;

using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;
using Notino.Homework.Services;

using Xunit;

namespace Notino.Homework.Tests;
public class DocumentServiceTests
{
    private readonly DocumentService documentService;
	private readonly Mock<IDocumentStore> documentStoreMock = new();
	private readonly Mock<ISerializerFactory> serializerFactoryMock = new();
	private readonly ILogger<IDocumentService> logger = Mock.Of<ILogger<IDocumentService>>();
	private readonly Mock<IMemoryCache> memoryCacheMock = new();
	private readonly Mock<ICacheEntry> cacheEntryMock = new();
	private readonly Mock<ISerializer> serializerMock = new();
	private readonly Fixture fixture = new();

    public DocumentServiceTests()
	{
		documentService = new(documentStoreMock.Object, serializerFactoryMock.Object, logger, memoryCacheMock.Object);

        memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);
    }

	[Fact]
	public async Task CreateDocument_OK()
	{
		var document = fixture.Create<Document>();

		await documentService.CreateDocument(document);

		memoryCacheMock.Verify(x => x.CreateEntry(document.Id), Times.Once);
	}

	[Fact]
	public async Task UpdateDocument_OK()
	{
		var document = fixture.Create<Document>();

		await documentService.UpdateDocument(document);

		memoryCacheMock.Verify(x => x.CreateEntry(document.Id), Times.Once);
	}

	[Fact]
	public async Task GetDocument_NotCached_OK()
	{
		var id = fixture.Create<string>();
		var fileFormat = fixture.Create<FileFormat>();
		var cachedDocument = null as object;
		var document = fixture.Build<Document>().With(x => x.Id, id).Create();
		var serializedDocument = fixture.Create<string>();

		memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedDocument)).Returns(false);
		documentStoreMock.Setup(x => x.GetDocument(It.IsAny<string>())).ReturnsAsync(document);
		serializerFactoryMock.Setup(x => x.Create(It.IsAny<FileFormat>())).Returns(serializerMock.Object);
		serializerMock.Setup(x => x.Serialize(It.IsAny<Document>())).Returns(serializedDocument);

		var expected = serializedDocument;

		var actual = await documentService.GetSerializedDocument(id, fileFormat);

		actual.Should().Be(expected);
        memoryCacheMock.Verify(x => x.TryGetValue(id, out cachedDocument), Times.Once);
        documentStoreMock.Verify(x => x.GetDocument(id), Times.Once);
        memoryCacheMock.Verify(x => x.CreateEntry(id), Times.Once);
        serializerFactoryMock.Verify(x => x.Create(fileFormat), Times.Once);
		serializerMock.Verify(x => x.Serialize(document), Times.Once);
    }

	[Fact]
	public async Task GetDocument_Cached_OK()
	{
		var id = fixture.Create<string>();
		var fileFormat = fixture.Create<FileFormat>();
		var cachedDocument = fixture.Build<Document>().With(x => x.Id, id).Create();
		var cachedDocumentAsObject = (object)cachedDocument;
		var serializedDocument = fixture.Create<string>();
		
		memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedDocumentAsObject)).Returns(true);
		serializerFactoryMock.Setup(x => x.Create(It.IsAny<FileFormat>())).Returns(serializerMock.Object);
		serializerMock.Setup(x => x.Serialize(It.IsAny<Document>())).Returns(serializedDocument);

		var expected = serializedDocument;

		var actual = await documentService.GetSerializedDocument(id, fileFormat);

		actual.Should().Be(expected);
        memoryCacheMock.Verify(x => x.TryGetValue(id, out cachedDocumentAsObject), Times.Once);
		documentStoreMock.Verify(x => x.GetDocument(It.IsAny<string>()), Times.Never);
        memoryCacheMock.Verify(x => x.CreateEntry(id), Times.Once);
        serializerFactoryMock.Verify(x => x.Create(fileFormat), Times.Once);
		serializerMock.Verify(x => x.Serialize(cachedDocument), Times.Once);
    }

	[Fact]
	public async Task GetDocument_DocumentNotFound()
	{
        var id = fixture.Create<string>();
        var fileFormat = fixture.Create<FileFormat>();
        var cachedDocument = null as object;

        memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedDocument)).Returns(false);
        documentStoreMock.Setup(x => x.GetDocument(It.IsAny<string>())).ReturnsAsync(null as Document);

		var action = async () => await documentService.GetSerializedDocument(id, fileFormat);

		await action.Should().ThrowAsync<MissingDocumentException>();
    }
}
