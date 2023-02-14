using System.Xml.Serialization;

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

        var serializer = new XmlSerializer(typeof(T));
        using var writer = new StringWriter();
        serializer.Serialize(writer, data);

        return writer.ToString();
    }
}