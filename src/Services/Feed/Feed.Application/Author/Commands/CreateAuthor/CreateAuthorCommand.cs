using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Common.Results;
using Feed.Domain.Aggregates.Author;
using AuthorDm = Feed.Domain.Aggregates.Author.Author;

namespace Feed.Application.Author.Commands.CreateAuthor {
    public class CreateAuthorCommand : IRequest<VoidResult> {
        public long UserId { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
    }

    public class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, VoidResult> {
        private readonly IAuthorRepository _authorRepository;

        public CreateAuthorCommandHandler(IAuthorRepository authorRepository) {
            _authorRepository = authorRepository;
        }

        public async Task<VoidResult> Handle(
            CreateAuthorCommand command, CancellationToken cancellationToken
        ) {
            var author = new AuthorDm(
                userId: command.UserId,
                email: command.Email,
                username: command.Username
            );

            _authorRepository.Create(author);

            await _authorRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
