using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddTeamDetails : Message {
        public TeamDto Team { get; set; }
    }
}
