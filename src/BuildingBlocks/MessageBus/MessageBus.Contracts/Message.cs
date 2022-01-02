using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace MessageBus.Contracts {
    public abstract class Message {
        public Guid CorrelationId { get; set; }

        public static T FromJson<T>(JsonDocument doc, Guid correlationId) where T : Message {
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            
            doc.WriteTo(writer);
            writer.Flush();

            var message = JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(stream.ToArray()));
            message.CorrelationId = correlationId;

            return message;
        }
    }
}
