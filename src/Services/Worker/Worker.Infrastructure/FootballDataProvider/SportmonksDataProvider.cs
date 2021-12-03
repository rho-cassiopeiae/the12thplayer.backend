using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;
using Worker.Infrastructure.FootballDataProvider.Dto;
using Worker.Infrastructure.FootballDataProvider.Utils;
using FixtureDtoApp = Worker.Application.Jobs.OneOff.FootballDataCollection.Dto.FixtureDto;
using SeasonDtoApp = Worker.Application.Jobs.OneOff.FootballDataCollection.Dto.SeasonDto;

namespace Worker.Infrastructure.FootballDataProvider {
    public class SportmonksDataProvider : IFootballDataProvider {
        private readonly ILogger<SportmonksDataProvider> _logger;
        private readonly Mapper _mapper;

        private readonly Uri _baseUrl;
        private readonly QueryString _baseQueryString;
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public SportmonksDataProvider(
            ILogger<SportmonksDataProvider> logger,
            IConfiguration configuration,
            Mapper mapper
        ) {
            _logger = logger;
            _mapper = mapper;

            _baseUrl = new Uri(configuration["Sportmonks:BaseUrl"]);
            _baseQueryString = QueryString.Create(
                "api_token",
                configuration["Sportmonks:ApiToken"]
            );

            _jsonSerializerOptions = new JsonSerializerOptions {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
            };
        }

        private HttpClient _createClient() {
            var client = new HttpClient();
            client.BaseAddress = _baseUrl;

            return client;
        }

        private async Task<T> _get<T>(
            HttpClient client, string pathAndQuery
        ) where T : ResponseDto {
            using var responseStream = await client.GetStreamAsync(pathAndQuery);

            var response = await JsonSerializer.DeserializeAsync<T>(
                responseStream, _jsonSerializerOptions
            );

            if (response.Error != null) {
                var message = response.Error.Message;
                var code = response.Error.Code;
                if (code >= 400 && code < 500) {
                    _logger.LogError(message);
                    // @@TODO: Custom exception, abort policy.
                    throw new Exception(message);
                } else {
                    _logger.LogWarning(message);
                    // @@TODO: Custom exception, repeat policy.
                    throw new Exception(message);
                }
            }

            return response;
        }

        public async Task<IEnumerable<CountryDto>> GetCountries(
        ) {
            using var client = _createClient();

            var queryString = _baseQueryString.Add("per_page", "100");

            var response = await _get<GetCountriesResponseDto>(
                client, $"countries{queryString}"
            );
            var responses = new List<GetCountriesResponseDto> { response };

            var pagination = response.Meta.Pagination;
            if (pagination != null) {
                for (int i = pagination.CurrentPage + 1; i <= pagination.TotalPages; ++i) {
                    var nextPageQueryString = queryString.Add("page", i.ToString());
                    var nextResponse = await _get<GetCountriesResponseDto>(
                        client, $"countries{nextPageQueryString}"
                    );
                    responses.Add(nextResponse);
                }
            }

            return _mapper.Map<CountryDto>(
                responses.SelectMany(response => response.Data)
            );
        }

        public async Task<TeamDto> GetTeamDetails(long teamId) {
            using var client = _createClient();

            var queryString = _baseQueryString.Add("include", "coach,venue");
            var response = await _get<GetTeamDetailsResponseDto>(
                client, $"teams/{teamId}{queryString}"
            );

            return _mapper.Map(response.Data);
        }

        public async Task<IEnumerable<SeasonDtoApp>> GetSeasons(IEnumerable<long> seasonIds) {
            using var client = _createClient();

            var queryString = _baseQueryString.Add("include", "league");
            var tasks = seasonIds.Select(
                seasonId => _get<GetSeasonResponseDto>(client, $"seasons/{seasonId}{queryString}")
            );
            var responses = await Task.WhenAll(tasks);

            return _mapper.Map<SeasonDtoApp>(responses.Select(response => response.Data));
        }

        public async Task<IEnumerable<PlayerDto>> GetPlayers(IEnumerable<long> playerIds) {
            using var client = _createClient();

            var queryString = _baseQueryString.Add("include", "position");
            var tasks = playerIds.Select(playerId => _get<GetPlayerResponseDto>(client, $"players/{playerId}{queryString}"));
            var responses = await Task.WhenAll(tasks);

            return _mapper.Map<PlayerDto>(responses.Select(response => response.Data));
        }

        public async Task<IEnumerable<FixtureDtoApp>> GetTeamFinishedFixtures(
            long teamId, string startDate, string endDate
        ) {
            using var client = _createClient();

            var queryString = _baseQueryString.Add(
                QueryString.Create(new[] {
                    KeyValuePair.Create(
                        "include",
                        "referee,venue,localTeam,visitorTeam,localCoach,visitorCoach,lineup,bench,stats,events"
                    ),
                    KeyValuePair.Create(
                        "per_page",
                        "10"
                    )
                })
            );

            var response = await _get<GetTeamFinishedFixturesResponseDto>(
                client,
                $"fixtures/between/{startDate}/{endDate}/{teamId}{queryString}"
            );
            var responses = new List<GetTeamFinishedFixturesResponseDto> { response };

            var pagination = response.Meta.Pagination;
            if (pagination != null) {
                for (int i = pagination.CurrentPage + 1; i <= pagination.TotalPages; ++i) {
                    var nextPageQueryString = queryString.Add("page", i.ToString());
                    var nextResponse = await _get<GetTeamFinishedFixturesResponseDto>(
                        client,
                        $"fixtures/between/{startDate}/{endDate}/{teamId}{nextPageQueryString}"
                    );
                    responses.Add(nextResponse);
                }
            }

            return _mapper.Map<FixtureDtoApp>(responses.SelectMany(response => response.Data), arg: teamId);
        }

        public async Task<IEnumerable<FixtureDtoApp>> GetTeamUpcomingFixtures(long teamId) {
            using var client = _createClient();

            var queryString = _baseQueryString.Add(
                "include",
                "upcoming.referee,upcoming.venue,upcoming.localTeam,upcoming.visitorTeam,upcoming.localCoach,upcoming.visitorCoach,upcoming.lineup,upcoming.bench,upcoming.stats,upcoming.events"
            );
            var response = await _get<GetTeamUpcomingFixturesResponseDto>(
                client, $"teams/{teamId}{queryString}"
            );

            return _mapper.Map<FixtureDtoApp>(response.Data.Upcoming.Data, arg: teamId);
        }

        //public async Task<FixtureDto> GetFixtureLivescore(
        //    long fixtureId,
        //    bool emulateOngoing,
        //    bool includeReferee = false,
        //    bool includeLineups = false,
        //    bool includeEventsAndStats = false
        //) {
        //    _includeBuilder.Clear();
        //    _includeBuilder.Append("localTeam,visitorTeam");
        //    if (includeReferee) {
        //        _includeBuilder.Append(",referee");
        //    }
        //    if (includeLineups) {
        //        _includeBuilder.Append(",localCoach,visitorCoach,lineup,bench");
        //    }
        //    if (includeEventsAndStats) {
        //        _includeBuilder.Append(",events,stats");
        //    }

        //    using var client = createClient();

        //    if (emulateOngoing) {
        //        var queryString = _baseQueryString.Add(
        //            "include", _includeBuilder.ToString()
        //        );
        //        var response = await get<FixtureResponseDto>(
        //            client, $"fixtures/{fixtureId}{queryString}"
        //        );

        //        return response.Data;
        //    } else {
        //        var queryString = _baseQueryString.Add(
        //            QueryString.Create(new[] {
        //                KeyValuePair.Create("fixtures", fixtureId.ToString()),
        //                KeyValuePair.Create("include", _includeBuilder.ToString())
        //            })
        //        );
        //        var response = await get<FixtureLivescoreResponseDto>(
        //            client, $"livescores{queryString}"
        //        );

        //        return response.Data?.SingleOrDefault();
        //    }
        //}
    }
}
