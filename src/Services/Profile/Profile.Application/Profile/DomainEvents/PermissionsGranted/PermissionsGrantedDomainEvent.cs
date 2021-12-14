using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;
using MediatR;

using Profile.Application.Common.Integration;
using Profile.Application.Profile.Common.Dto;
using Profile.Domain.Base;

namespace Profile.Application.Profile.DomainEvents.PermissionsGranted {
    public class PermissionsGrantedDomainEvent : INotification {
        public long UserId { get; init; }
        public IEnumerable<ProfilePermissionDto> Permissions { get; init; }
    }

    public class PermissionsGrantedDomainEventHandler : INotificationHandler<PermissionsGrantedDomainEvent> {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventRepository _integrationEventRepository;

        public PermissionsGrantedDomainEventHandler(
            IUnitOfWork unitOfWork,
            IIntegrationEventRepository integrationEventRepository
        ) {
            _unitOfWork = unitOfWork;
            _integrationEventRepository = integrationEventRepository;
        }

        public async Task Handle(
            PermissionsGrantedDomainEvent @event, CancellationToken cancellationToken
        ) {
            _integrationEventRepository.EnlistAsPartOf(_unitOfWork);

            using var integrationEvent = new IntegrationEvent {
                Id = NewId.NextGuid(),
                Type = IntegrationEventType.ProfilePermissionsGranted,
                Payload = JsonDocument.Parse(JsonSerializer.Serialize(@event)),
                Status = IntegrationEventStatus.Pending
            };

            _integrationEventRepository.Create(integrationEvent);

            await _integrationEventRepository.SaveChanges(cancellationToken);
        }
    }
}
