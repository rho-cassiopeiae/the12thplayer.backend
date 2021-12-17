using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Feed.Application.Author.Common.Dto;
using Feed.Application.Common.Results;
using Feed.Domain.Aggregates.Author;
using AuthorDm = Feed.Domain.Aggregates.Author.Author;

namespace Feed.Application.Author.Commands.AddPermissions {
    public class AddPermissionsCommand : IRequest<VoidResult> {
        public long UserId { get; init; }
        public IEnumerable<AuthorPermissionDto> Permissions { get; init; }
    }

    public class AddPermissionsCommandHandler : IRequestHandler<AddPermissionsCommand, VoidResult> {
        private readonly IAuthorRepository _authorRepository;

        public AddPermissionsCommandHandler(IAuthorRepository authorRepository) {
            _authorRepository = authorRepository;
        }

        public async Task<VoidResult> Handle(
            AddPermissionsCommand command, CancellationToken cancellationToken
        ) {
            var author = new AuthorDm(userId: command.UserId);
            foreach (var permission in command.Permissions) {
                author.AddPermission((PermissionScope) permission.Scope, (short) permission.Flags);
            }

            await _authorRepository.UpdatePermissions(author);

            return VoidResult.Instance;
        }
    }
}
