using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using Identity.Application.Common.Integration;
using Identity.Domain.Base;

namespace Identity.Application.Account.DomainEvents.UserAccountConfirmed {
    public class UserAccountConfirmedDomainEvent : INotification {
        public long UserId { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
    }

    public class UserAccountConfirmedDomainEventHandler : INotificationHandler<
        UserAccountConfirmedDomainEvent
    > {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventRepository _integrationEventRepository;

        public UserAccountConfirmedDomainEventHandler(
            IUnitOfWork unitOfWork,
            IIntegrationEventRepository integrationEventRepository
        ) {
            _unitOfWork = unitOfWork;
            _integrationEventRepository = integrationEventRepository;
        }

        public async Task Handle(
            UserAccountConfirmedDomainEvent @event,
            CancellationToken cancellationToken
        ) {
            _integrationEventRepository.EnlistAsPartOf(_unitOfWork);

            using var integrationEvent = new IntegrationEvent {
                Id = NewId.NextGuid(),
                Type = IntegrationEventType.UserAccountConfirmed,
                Payload = JsonDocument.Parse(JsonSerializer.Serialize(@event)),
                Status = IntegrationEventStatus.Pending
            };

            _integrationEventRepository.Create(integrationEvent);

            await _integrationEventRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
