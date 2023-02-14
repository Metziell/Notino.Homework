namespace Notino.Homework.Domain.Interfaces;
public interface ISerializerFactory
{
    ISerializer Create(FileFormat fileFormat);
}