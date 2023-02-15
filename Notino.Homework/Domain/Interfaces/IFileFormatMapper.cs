namespace Notino.Homework.Domain.Interfaces;
public interface IFileFormatMapper
{
    FileFormat Map(string format);
    string Map(FileFormat fileFormat);
}