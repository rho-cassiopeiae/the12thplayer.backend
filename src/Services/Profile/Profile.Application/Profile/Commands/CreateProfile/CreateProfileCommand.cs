using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using Profile.Application.Common.Results;
using Profile.Domain.Aggregates.Profile;
using ProfileDm = Profile.Domain.Aggregates.Profile.Profile;

namespace Profile.Application.Profile.Commands.CreateProfile {
    public class CreateProfileCommand : IRequest<VoidResult> {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }

    public class CreateProfileCommandHandler : IRequestHandler<
        CreateProfileCommand, VoidResult
    > {
        private readonly ILogger<CreateProfileCommandHandler> _logger;
        private readonly IProfileRepository _profileRepository;

        public CreateProfileCommandHandler(
            ILogger<CreateProfileCommandHandler> logger,
            IProfileRepository profileRepository
        ) {
            _logger = logger;
            _profileRepository = profileRepository;
        }

        public async Task<VoidResult> Handle(
            CreateProfileCommand command, CancellationToken cancellationToken
        ) {
            var profile = new ProfileDm(
                userId: command.UserId,
                email: command.Email,
                username: command.Username
            );

            profile.AddPermission(
                PermissionScope.AdminPanel,
                (int) AdminPanelPermissions.LogIn
            );

            profile.AddPermission(
                PermissionScope.UserManagement,
                (int) (
                    UserManagementPermissions.ListUsers |
                    UserManagementPermissions.GrantPermission |
                    UserManagementPermissions.RevokePermission
                )
            );

            profile.AddPermission(
                PermissionScope.JobManagement,
                (int) (
                    JobManagementPermissions.ExecuteOneOffJobs |
                    JobManagementPermissions.SchedulePeriodicJobs
                )
            );

            _profileRepository.Create(profile);

            await _profileRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
