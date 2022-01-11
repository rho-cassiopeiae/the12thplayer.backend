using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Worker.Infrastructure.FootballDataProvider.Utils {
    public class RoundDateJsonConverter : JsonConverter<DateTime?> {
        private const string format = "yyyy-MM-dd";

        public override DateTime? Read(ref Utf8JsonReader reader, Type _, JsonSerializerOptions __) {
            var date = reader.GetString();
            return date != null ?
                DateTime.ParseExact(date, format, CultureInfo.InvariantCulture) :
                null;
        }

        public override void Write(Utf8JsonWriter _, DateTime? __, JsonSerializerOptions ___) {
            throw new NotSupportedException();
        }
    }
}
