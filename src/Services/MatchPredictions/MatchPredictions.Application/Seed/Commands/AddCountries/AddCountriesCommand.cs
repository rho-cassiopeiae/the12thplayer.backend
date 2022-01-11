using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using MatchPredictions.Application.Common.Results;
using MatchPredictions.Application.Common.Dto;
using MatchPredictions.Domain.Aggregates.Country;

namespace MatchPredictions.Application.Seed.Commands.AddCountries {
    public class AddCountriesCommand : IRequest<VoidResult> {
        public IEnumerable<CountryDto> Countries { get; init; }
    }

    public class AddCountriesCommandHandler : IRequestHandler<AddCountriesCommand, VoidResult> {
        private readonly ICountryRepository _countryRepository;

        public AddCountriesCommandHandler(ICountryRepository countryRepository) {
            _countryRepository = countryRepository;
        }

        public async Task<VoidResult> Handle(
            AddCountriesCommand command, CancellationToken cancellationToken
        ) {
            var countries = command.Countries.Select(c => new Country(
                id: c.Id,
                name: c.Name,
                flagUrl: c.FlagUrl
            ));

            _countryRepository.Create(countries);

            await _countryRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
