using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

public class EmptyStringValueTypeResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(
        MemberInfo member,
        MemberSerialization memberSerialization)
    {
        var prop = base.CreateProperty(member, memberSerialization);

        // string篮 力寇
        if (prop.PropertyType == typeof(string))
            return prop;

        // Nullable<T> 贸府
        var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

        // value type父
        if (!type.IsValueType)
            return prop;

        // enum 力寇
        if (type.IsEnum)
            return prop;

        prop.Converter = new EmptyStringToDefaultConverter();
        return prop;
    }
}
