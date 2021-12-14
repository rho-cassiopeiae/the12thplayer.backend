using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Admin.Application.Common.Attributes;
using Admin.Application.Common.Enums;
using Admin.Application.Common.Interfaces;
using Admin.Application.Common.Results;
using Admin.Application.Profile.Common.Dto;
using Admin.Application.Common.Errors;

namespace Admin.Application.Profile.Commands.GrantSuperuserPermissions {
    [RequireAuthorization]
    public class GrantSuperuserPermissionsCommand : IRequest<VoidResult> {
        public string Payload { get; set; }
        public string Signature { get; set; }
    }

    public class GrantSuperuserPermissionsCommandHandler : IRequestHandler<
        GrantSuperuserPermissionsCommand, VoidResult
    > {
        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;
        private readonly ISuperuserSignatureVerifier _superuserSignatureVerifier;
        private readonly IProfilePermissionManager _profilePermissionManager;

        public GrantSuperuserPermissionsCommandHandler(
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            ISuperuserSignatureVerifier superuserSignatureVerifier,
            IProfilePermissionManager profilePermissionManager
        ) {
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _superuserSignatureVerifier = superuserSignatureVerifier;
            _profilePermissionManager = profilePermissionManager;
        }

        public async Task<VoidResult> Handle(
            GrantSuperuserPermissionsCommand command, CancellationToken cancellationToken
        ) {
            bool valid = _superuserSignatureVerifier.Verify(command.Payload, command.Signature);
            if (!valid) {
                return new VoidResult {
                    Error = new AuthorizationError("Invalid signature")
                };
            }

            long userId = _principalDataProvider.GetId(_authenticationContext.User);

            await _profilePermissionManager.GrantPermissionsTo(userId, new[] {
                new ProfilePermissionDto {
                    Scope = PermissionScope.AdminPanel,
                    Flags = (int) AdminPanelPermissions.LogIn
                },
                new ProfilePermissionDto {
                    Scope = PermissionScope.UserManagement,
                    Flags = (int) (
                        UserManagementPermissions.ListUsers |
                        UserManagementPermissions.GrantPermission |
                        UserManagementPermissions.RevokePermission
                    )
                },
                new ProfilePermissionDto {
                    Scope = PermissionScope.JobManagement,
                    Flags = (int) (
                        JobManagementPermissions.ExecuteOneOffJobs |
                        JobManagementPermissions.SchedulePeriodicJobs
                    )
                },
                new ProfilePermissionDto {
                    Scope = PermissionScope.Article,
                    Flags = (int) (
                        ArticlePermissions.Publish |
                        ArticlePermissions.Review |
                        ArticlePermissions.Edit |
                        ArticlePermissions.Delete
                    )
                }
            });

            return VoidResult.Instance;
        }
    }
}
