namespace Notino.Homework.Domain;

public class DuplicateDocumentException : Exception
{
	public DuplicateDocumentException(string documentId) : base($"A document with id {documentId} already exists.")
	{ }
}

public class MissingDocumentException : Exception
{
	public MissingDocumentException(string documentId) : base($"A document with id {documentId} doesn't exists.")
	{ }
}