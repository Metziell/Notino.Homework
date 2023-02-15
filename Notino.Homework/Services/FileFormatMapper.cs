using Notino.Homework.Domain;
using Notino.Homework.Domain.Interfaces;

namespace Notino.Homework.Services;

public class FileFormatMapper : IFileFormatMapper
{
    public FileFormat Map(string format) => format switch
    {
        "application/json" => FileFormat.Json,
        "application/xml" => FileFormat.Xml,
        "text/xml" => FileFormat.Xml,
        "*/*" => FileFormat.Json,
        _ => throw new NotImplementedException($"{nameof(format)}: {format}")
    };

    public string Map(FileFormat fileFormat) => fileFormat switch
    {
        FileFormat.Json => "application/json",
        FileFormat.Xml => "application/xml",
        _ => throw new NotImplementedException($"{nameof(fileFormat)}: {fileFormat}")
    };
}
