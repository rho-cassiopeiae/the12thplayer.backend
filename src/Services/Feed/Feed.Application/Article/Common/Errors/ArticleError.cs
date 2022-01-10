using System.Collections.Generic;

using Feed.Application.Common.Errors;

namespace Feed.Application.Article.Common.Errors {
    public class ArticleError : HandleError {
        public ArticleError(string message) : base(type: "Article") {
            Errors = new Dictionary<string, string[]> {
                [string.Empty] = new[] { message }
            };
        }
    }
}
