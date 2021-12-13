using System.Collections.Generic;

using Livescore.Application.Common.Errors;

namespace Livescore.Application.Livescore.VideoReaction.Common.Errors {
    public class VideoReactionError : HandleError {
        public VideoReactionError(string message) : base(type: "VideoReaction") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
