using System.Text.Json;
using System.Text.Json.Serialization;

namespace RegularizadorPolizas.Application.Converters
{
    public class FlexibleIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetInt32();
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                if (string.IsNullOrEmpty(stringValue))
                    return 0;

                return int.TryParse(stringValue, out var result) ? result : 0;
            }

            if (reader.TokenType == JsonTokenType.Null)
            {
                return 0;
            }

            return 0;
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}