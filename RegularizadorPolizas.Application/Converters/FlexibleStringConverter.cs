using AutoMapper;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Converters
{
    public class FlexibleStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.TokenType switch
            {
                JsonTokenType.String => reader.GetString() ?? string.Empty,
                JsonTokenType.Number => reader.GetInt32().ToString(),
                JsonTokenType.Null => string.Empty,
                _ => string.Empty
            };
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value ?? string.Empty);
        }
    }

    public class IntStringValueResolver : IValueResolver<object, object, string>
    {
        public string Resolve(object source, object destination, string destMember, ResolutionContext context)
        {
            if (source == null) return "0";

            var sourceType = source.GetType();
            var property = sourceType.GetProperty("No_utiles");
            if (property == null) return "0";

            var value = property.GetValue(source);
            return value?.ToString() ?? "0";
        }
    }

    public class StringIntValueResolver : IValueResolver<object, object, int>
    {
        public int Resolve(object source, object destination, int destMember, ResolutionContext context)
        {
            if (source == null) return 0;

            var sourceType = source.GetType();
            var property = sourceType.GetProperty("No_utiles");
            if (property == null) return 0;

            var value = property.GetValue(source);
            if (value == null) return 0;

            if (value is int intValue) return intValue;
            if (value is string stringValue)
            {
                return int.TryParse(stringValue, out var result) ? result : 0;
            }

            return 0;
        }
    }
}