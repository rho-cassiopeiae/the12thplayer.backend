using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Worker.Infrastructure.FootballDataProvider.Utils {
    public class BirthDateJsonConverter : JsonConverter<DateTime?> {
        private const string format = "dd/MM/yyyy";

        public override DateTime? Read(
            ref Utf8JsonReader reader, Type _, JsonSerializerOptions __
        ) {
            var birthDate = reader.GetString();
            return birthDate != null ?
                DateTime.ParseExact(birthDate, format, CultureInfo.InvariantCulture) :
                null;
        }

        public override void Write(
            Utf8JsonWriter _, DateTime? __, JsonSerializerOptions ___
        ) {
            throw new NotSupportedException();
        }
    }
}
