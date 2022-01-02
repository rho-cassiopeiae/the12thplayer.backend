using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using Identity.Application.Common.Integration;
using Identity.Domain.Base;

namespace Identity.Application.Account.DomainEvents.UserAccountCreated {
    public class UserAccountCreatedDomainEvent : INotification {
        public string Email { get; init; }
        public string Username { get; init; }
        public string ConfirmationCode { get; init; }
    }

    public class UserAccountCreatedDomainEventHandler : INotificationHandler<
        UserAccountCreatedDomainEvent
    > {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventRepository _integrationEventRepository;

        public UserAccountCreatedDomainEventHandler(
            IUnitOfWork unitOfWork,
            IIntegrationEventRepository integrationEventRepository
        ) {
            _unitOfWork = unitOfWork;
            _integrationEventRepository = integrationEventRepository;
        }

        public async Task Handle(
            UserAccountCreatedDomainEvent @event, CancellationToken cancellationToken
        ) {
            _integrationEventRepository.EnlistAsPartOf(_unitOfWork);

            using var integrationEvent = new IntegrationEvent {
                Id = NewId.NextGuid(),
                Type = IntegrationEventType.UserAccountCreated,
                Payload = JsonDocument.Parse(JsonSerializer.Serialize(@event)),
                Status = IntegrationEventStatus.Pending
            };

            _integrationEventRepository.Create(integrationEvent);

            await _integrationEventRepository.SaveChanges(cancellationToken);
        }
    }
}
