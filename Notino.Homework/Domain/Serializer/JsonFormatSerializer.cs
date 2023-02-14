using Notino.Homework.Domain.Interfaces;

using Newtonsoft.Json;

namespace Notino.Homework.Domain.Serializer;

public class JsonFormatSerializer : ISerializer
{
    public string Serialize<T>(T data)
    {
        if (data == null)
        {
            return string.Empty;
        }

        return JsonConvert.SerializeObject(data);
    }
}