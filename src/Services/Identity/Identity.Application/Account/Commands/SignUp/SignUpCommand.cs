﻿using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MediatR;

using Identity.Application.Common.Results;
using Identity.Domain.Aggregates.User;
using Identity.Application.Common.Interfaces;

namespace Identity.Application.Account.Commands.SignUp {
    public class SignUpCommand : IRequest<VoidResult> {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class SignUpCommandHandler : IRequestHandler<SignUpCommand, VoidResult> {
        private readonly ILogger<SignUpCommandHandler> _logger;
        private readonly IUserService _userService;

        public SignUpCommandHandler(
            ILogger<SignUpCommandHandler> logger,
            IUserService userService
        ) {
            _logger = logger;
            _userService = userService;
        }

        public async Task<VoidResult> Handle(
            SignUpCommand command, CancellationToken cancellationToken
        ) {
            _logger.LogInformation("User {Email} is signing up", command.Email);

            var user = new User(
                email: command.Email,
                username: command.Username
            );

            var outcome = await _userService.Create(user, command.Password);
            if (outcome.IsError) {
                return new VoidResult {
                    Error = outcome.Error
                };
            }

            _logger.LogInformation("User {Email} signed up successfully", user.Email);

            return VoidResult.Instance;
        }
    }
}
