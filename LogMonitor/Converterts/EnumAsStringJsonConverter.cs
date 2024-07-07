using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogMonitor
{
    public class EnumAsStringJsonConverter<T> : JsonConverter<T> where T : Enum
    {
        public EnumAsStringJsonConverter()
        {
        }

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                if (string.Equals(enumValue.ToString(), value, StringComparison.InvariantCultureIgnoreCase))
                {
                    return enumValue;
                }
            }

            throw new InvalidCastException($"Can`t convert value \"{value}\" to \"{typeof(T).Name}\" type");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLower());
        }
    }
}
