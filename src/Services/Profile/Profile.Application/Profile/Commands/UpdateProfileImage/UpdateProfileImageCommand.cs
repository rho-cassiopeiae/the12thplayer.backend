using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using MediatR;

using Profile.Application.Common.Attributes;
using Profile.Application.Common.Interfaces;
using Profile.Application.Common.Results;
using Profile.Application.Profile.Common.Errors;

namespace Profile.Application.Profile.Commands.UpdateProfileImage {
    [RequireAuthorization]
    public class UpdateProfileImageCommand : IRequest<HandleResult<string>> {
        public HttpRequest Request { get; init; }
    }

    public class UpdateProfileImageCommandHandler : IRequestHandler<
        UpdateProfileImageCommand, HandleResult<string>
    > {
        private readonly ILogger<UpdateProfileImageCommandHandler> _logger;

        private readonly IAuthenticationContext _authenticationContext;
        private readonly IPrincipalDataProvider _principalDataProvider;

        private readonly IFileReceiver _fileReceiver;
        private readonly IFileHosting _fileHosting;

        public UpdateProfileImageCommandHandler(
            ILogger<UpdateProfileImageCommandHandler> logger,
            IAuthenticationContext authenticationContext,
            IPrincipalDataProvider principalDataProvider,
            IFileReceiver fileReceiver,
            IFileHosting fileHosting
        ) {
            _logger = logger;
            _authenticationContext = authenticationContext;
            _principalDataProvider = principalDataProvider;
            _fileReceiver = fileReceiver;
            _fileHosting = fileHosting;
        }

        public async Task<HandleResult<string>> Handle(
            UpdateProfileImageCommand command, CancellationToken cancellationToken
        ) {
            var outcome = await _fileReceiver.ReceiveImageAndFormValues(
                command.Request,
                maxSize: 2 * 1024 * 1024, // @@TODO: Config.
                filePrefix: "profile-images",
                fileName: _principalDataProvider.GetUsername(_authenticationContext.User)
            );
            if (outcome.IsError) {
                return new HandleResult<string> {
                    Error = outcome.Error
                };
            }

            var filePath = outcome.Data["filePath"];
            try {
                var imageUrl = await _fileHosting.UploadImage(filePath);

                return new HandleResult<string> {
                    Data = imageUrl
                };
            } catch (Exception e) {
                _logger.LogError("Error uploading user image", e);

                // @@TODO: Since here it's unclear what exactly caused uploading to fail (it could be just a MassTransit
                // error for all we know), we should not rely on FileHostingGateway deleting the file. Need to ensure that
                // it gets properly deleted.

                try {
                    _fileReceiver.DeleteFile(filePath);
                } catch (Exception ex) {
                    _logger.LogError("Error deleting user image", ex);
                }

                return new HandleResult<string> {
                    Error = new ProfileError("Error updating profile image")
                };
            }
        }
    }
}
