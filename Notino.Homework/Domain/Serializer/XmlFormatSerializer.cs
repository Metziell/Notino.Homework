using System.Runtime.Serialization;
using Notino.Homework.Domain.Interfaces;

namespace Notino.Homework.Domain.Serializer;
public class XmlFormatSerializer : ISerializer
{
    public string Serialize<T>(T data)
    {
        if (data == null)
        {
            return string.Empty;
        }
        
        var serializer = new DataContractSerializer(typeof(T));
        using var memoryStream = new MemoryStream();
        using var reader = new StreamReader(memoryStream);

        serializer.WriteObject(memoryStream, data);
        memoryStream.Position = 0;

        return reader.ReadToEnd();
    }
}