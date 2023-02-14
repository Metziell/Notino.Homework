namespace Notino.Homework.Domain.Interfaces;
public interface ISerializer
{
    string Serialize<T>(T data);
}
