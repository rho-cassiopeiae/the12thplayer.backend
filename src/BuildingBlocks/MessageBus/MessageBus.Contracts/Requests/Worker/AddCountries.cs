using System.Collections.Generic;

using MessageBus.Contracts.Common.Dto;

namespace MessageBus.Contracts.Requests.Worker {
    public class AddCountries : Message {
        public IEnumerable<CountryDto> Countries { get; set; }
    }
}
