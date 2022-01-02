using System.Linq;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;

using Livescore.Application.Livescore.PlayerRating.Commands.RatePlayer;
using Livescore.Application.Livescore.Worker.Commands.ActivateFixture;
using Livescore.Application.Livescore.Worker.Commands.UpdateFixturePrematch;
using Livescore.Application.Livescore.PlayerRating.Queries.GetPlayerRatingsForFixture;
using Livescore.Application.Livescore.Worker.Commands.DeactivateFixture;
using Livescore.Application.Livescore.Worker.Commands.FinishFixture;

namespace Livescore.IntegrationTests.Livescore.PlayerRating.Queries {
    [Collection(nameof(LivescoreTestCollection))]
    public class Get_Player_Ratings_For_Fixture_Query_Tests {
        private readonly Sut _sut;

        private readonly long _fixtureId;
        private readonly long _teamId;
        private readonly string _participantKey;

        public Get_Player_Ratings_For_Fixture_Query_Tests(Sut sut) {
            _sut = sut;
            _sut.ResetState();

            (_fixtureId, _teamId) = _sut.SeedWithDummyUpcomingFixture();

            _sut.SendRequest(
                new ActivateFixtureCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    VimeoProjectId = "789456"
                }
            ).Wait();

            var fixture = _sut.GetSeededFixtureWithDummyPrematchData();
            var managerId = fixture.Lineups.First(l => l.TeamId == _teamId).Manager.Id;
            _participantKey = $"m:{managerId}";

            _sut.SendRequest(
                new UpdateFixturePrematchCommand {
                    FixtureId = _fixtureId,
                    TeamId = _teamId,
                    Fixture = fixture
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Retrieve_All_Player_Ratings_When_Fixture_Is_Still_Active() {
            _sut.RunAs(userId: 1, username: "user-1");

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey,
                Rating = 4.25f
            });

            _sut.RunAs(userId: 2, username: "user-2");

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey,
                Rating = 8.15f
            });

            var result = await _sut.SendRequest(new GetPlayerRatingsForFixtureQuery {
                FixtureId = _fixtureId,
                TeamId = _teamId
            });

            result.Data.RatingsFinalized.Should().BeFalse();
            result.Data.PlayerRatings.First(pr => pr.ParticipantKey == _participantKey).Should().BeEquivalentTo(
                new PlayerRatingWithUserVoteDto {
                    ParticipantKey = _participantKey,
                    TotalRating = 12,
                    TotalVoters = 2,
                    UserRating = 8.15f
                }
            );
        }

        [Fact]
        public async Task Should_Retrieve_All_Player_Ratings_When_Fixture_Is_No_Longer_Active() {
            _sut.RunAs(userId: 1, username: "user-1");

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey,
                Rating = 4.25f
            });

            _sut.RunAs(userId: 2, username: "user-2");

            await _sut.SendRequest(new RatePlayerCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId,
                ParticipantKey = _participantKey,
                Rating = 8.15f
            });

            _sut.RunAsGuest();

            await _sut.SendRequest(new FinishFixtureCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId
            });

            await _sut.SendRequest(new DeactivateFixtureCommand {
                FixtureId = _fixtureId,
                TeamId = _teamId
            });

            _sut.RunAs(userId: 1, username: "user-1");

            var result = await _sut.SendRequest(new GetPlayerRatingsForFixtureQuery {
                FixtureId = _fixtureId,
                TeamId = _teamId
            });

            result.Data.RatingsFinalized.Should().BeTrue();
            result.Data.PlayerRatings.First(pr => pr.ParticipantKey == _participantKey).Should().BeEquivalentTo(
                new PlayerRatingWithUserVoteDto {
                    ParticipantKey = _participantKey,
                    TotalRating = 12,
                    TotalVoters = 2,
                    UserRating = 4.25f
                }
            );
        }
    }
}
