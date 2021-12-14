using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Profile.Application.Common.Results;
using Profile.Application.Profile.Common.Dto;
using Profile.Application.Profile.DomainEvents.PermissionsGranted;
using Profile.Domain.Aggregates.Profile;
using Profile.Domain.Base;

namespace Profile.Application.Profile.Commands.GrantPermissions {
    public class GrantPermissionsCommand : IRequest<VoidResult> {
        public long UserId { get; init; }
        public IEnumerable<ProfilePermissionDto> Permissions { get; init; }
    }

    public class GrantPermissionsCommandHandler : IRequestHandler<GrantPermissionsCommand, VoidResult> {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProfileRepository _profileRepository;

        public GrantPermissionsCommandHandler(
            IUnitOfWork unitOfWork,
            IProfileRepository profileRepository
        ) {
            _unitOfWork = unitOfWork;
            _profileRepository = profileRepository;
        }

        public async Task<VoidResult> Handle(
            GrantPermissionsCommand command, CancellationToken cancellationToken
        ) {
            await _unitOfWork.Begin();
            try {
                _profileRepository.EnlistAsPartOf(_unitOfWork);

                var profile = await _profileRepository.FindByUserId(command.UserId);

                foreach (var permission in command.Permissions) {
                    profile.AddPermission((PermissionScope) permission.Scope, permission.Flags);
                }

                _profileRepository.UpdatePermissions(profile);

                profile.AddDomainEvent(new PermissionsGrantedDomainEvent {
                    UserId = command.UserId,
                    Permissions = command.Permissions
                });

                await _profileRepository.SaveChanges(cancellationToken);

                await _unitOfWork.Commit();
                
                return VoidResult.Instance;
            } catch {
                await _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
