using Notino.Homework.Domain;

namespace Notino.Homework.Domain.Interfaces;
public interface IFileFormatMapper
{
    FileFormat Map(string format);
}