using System.Collections.Generic;

using MessageBus.Contracts.Requests.Worker.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddCountries : Message {
        public IEnumerable<CountryDto> Countries { get; set; }
    }
}
