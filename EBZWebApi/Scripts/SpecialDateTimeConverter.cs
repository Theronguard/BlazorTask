using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EBZWebApi.Scripts
{
    /// <summary>
    /// For parsing the special date time format
    /// </summary>
    public class SpecialDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "yyyy-MM-ddTHH:mm:ss zzz";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="_"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override DateTime Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? date = reader.GetString();
                if (DateTimeOffset.TryParseExact(date, Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateOffset))
                    return dateOffset.DateTime;
            }

            throw new JsonException($"Couldn't parse the DateTime format! {Format}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="dateTime"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime dateTime, JsonSerializerOptions options)
        {
            writer.WriteStringValue(dateTime.ToString());
        }
    }
}
