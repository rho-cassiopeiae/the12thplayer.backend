using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Worker.Infrastructure.FootballDataProvider.Utils {
    public class StartTimeJsonConverter : JsonConverter<DateTimeOffset?> {
        public override DateTimeOffset? Read(
            ref Utf8JsonReader reader, Type _, JsonSerializerOptions __
        ) {
            var startTime = reader.GetString();
            return startTime != null ? DateTimeOffset.Parse(startTime) : null;
        }

        public override void Write(
            Utf8JsonWriter _, DateTimeOffset? __, JsonSerializerOptions ___
        ) {
            throw new NotSupportedException();
        }
    }
}
