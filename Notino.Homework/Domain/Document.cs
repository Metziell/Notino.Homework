namespace Notino.Homework.Domain;
public class Document
{
    public string Id { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = new();
    public Dictionary<string, string> Data { get; init; } = new();
}