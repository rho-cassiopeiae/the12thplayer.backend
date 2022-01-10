using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using MediatR;

using Feed.Application.Article.Commands.PostVideoArticle;
using Feed.Application.Common.Results;
using Feed.Api.Controllers.Filters;

namespace Feed.Api.Controllers {
    [Route("feed/teams/{teamId}")]
    public class FeedController : ControllerBase {
        private readonly ISender _mediator;

        public FeedController(ISender mediator) {
            _mediator = mediator;
        }

        [DisableFormValueModelBinding]
        [RequestSizeLimit(2 * 1024 * 1024)] // @@TODO: Config.
        [HttpPost("video-article")]
        public Task<HandleResult<long>> PostVideoArticle(long teamId) => _mediator.Send(
            new PostVideoArticleCommand {
                TeamId = teamId,
                Request = Request
            }
        );
    }
}
