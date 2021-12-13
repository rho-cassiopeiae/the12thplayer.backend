using System.Text.Json.Serialization;

using FileHostingGateway.Application.Common.Errors;

namespace FileHostingGateway.Application.Common.Results {
    public abstract class HandleResult {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HandleError Error { get; init; }
    }

    public class HandleResult<T> : HandleResult {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T Data { get; init; }
    }

    public class VoidResult : HandleResult<object> {
        public static readonly VoidResult Instance = new VoidResult();
    }
}
