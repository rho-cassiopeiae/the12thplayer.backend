using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using Identity.Application.Common.Results;

namespace Identity.Api.Controllers.Filters {
    public class ConvertHandleErrorToMvcResponseFilter : IResultFilter {
        public void OnResultExecuting(ResultExecutingContext context) {
            var handleResult = (HandleResult) ((ObjectResult) context.Result).Value;
            if (handleResult.Error != null) {
                context.Result = new BadRequestObjectResult(handleResult);
            }
        }

        public void OnResultExecuted(ResultExecutedContext context) { }
    }
}
