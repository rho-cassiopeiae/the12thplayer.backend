﻿using System.Text.Json.Serialization;

using Feed.Application.Common.Errors;

namespace Feed.Application.Common.Results {
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
