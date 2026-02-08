using Newtonsoft.Json;
using System;

public class EmptyStringToDefaultConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true; // 실제 적용 여부는 Resolver에서 제어
    }

    public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
    {
        // "" 인 경우
        if (reader.TokenType == JsonToken.String &&
            string.IsNullOrEmpty(reader.Value?.ToString()))
        {
            var targetType = Nullable.GetUnderlyingType(objectType) ?? objectType;
            return Activator.CreateInstance(targetType);
        }

        return serializer.Deserialize(reader, objectType);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
