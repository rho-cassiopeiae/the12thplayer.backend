using System;
using System.Text.Json;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using MessageBus.Contracts.Requests.Admin;
using MessageBus.Contracts.Responses.Identity;

using Identity.Application.Account.Commands.LogInAsAdmin;

namespace Identity.Api.Consumers.Admin {
    public class AuthRequestsConsumer : IConsumer<LogInAsAdmin> {
        private readonly ISender _mediator;

        public AuthRequestsConsumer(ISender mediator) {
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<LogInAsAdmin> context) {
            var command = new LogInAsAdminCommand {
                Email = context.Message.Email,
                Password = context.Message.Password
            };

            var result = await _mediator.Send(command);
            if (result.Error != null) {
                await context.RespondAsync(new LogInAsAdminError {
                    CorrelationId = Guid.NewGuid(),
                    Message = JsonSerializer.Serialize(result.Error.Errors)
                });
            } else {
                await context.RespondAsync(new LogInAsAdminSuccess {
                    CorrelationId = Guid.NewGuid(),
                    AccessToken = result.Data
                });
            }
        }
    }
}
