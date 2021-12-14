using System.Collections.Generic;

using MediatR;

namespace Profile.Domain.Base {
    public abstract class Entity {
        private List<INotification> _domainEvents;
        public IReadOnlyList<INotification> DomainEvents => _domainEvents;

        public void AddDomainEvent(INotification @event) {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(INotification @event) {
            _domainEvents?.Remove(@event);
        }

        public void ClearDomainEvents() {
            _domainEvents?.Clear();
        }
    }
}
