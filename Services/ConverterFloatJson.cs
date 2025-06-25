using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SelicBCB___Pablo_Lipa.Services
{
    public class ConverterFloatJson : JsonConverter<float>
    {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (float.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue))
                {
                    return floatValue;
                }
                // Tentar com cultura brasileira (vírgula como separador decimal)
                if (float.TryParse(stringValue, NumberStyles.Any, new CultureInfo("pt-BR"), out float brFloatValue))
                {
                    return brFloatValue;
                }
                throw new JsonException($"Cannot convert string '{stringValue}' to float.");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.GetSingle(); // Lê diretamente como float se for um número
            }

            throw new JsonException($"Unexpected token type {reader.TokenType} when trying to convert to float.");
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
