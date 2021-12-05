#pragma warning disable CS4014

using System;
using System.Linq;
using System.Threading.Tasks;

using StackExchange.Redis;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Dto;

namespace Livescore.Infrastructure.Livescore {
    public class RedisStore : IInMemoryStore {
        private readonly ConnectionMultiplexer _redis;

        private ITransaction _txn;

        public RedisStore(ConnectionMultiplexer redis) {
            _redis = redis;
        }

        public async Task SaveChanges() {
            if (_txn != null) {
                await _txn.ExecuteAsync();
            }
        }

        private void _ensureTransaction() {
            if (_txn == null) {
                _txn = _redis.GetDatabase().CreateTransaction();
            }
        }

        public void SetFixtureActiveAndOngoing(long fixtureId, long teamId) {
            _ensureTransaction();

            var fixtureIdentifier = $"fixture:{fixtureId}.team:{teamId}";
            _txn.SetAddAsync("fixtures.active", fixtureIdentifier);
            _txn.SetAddAsync("fixtures.ongoing", fixtureIdentifier);
        }

        public void CreateDiscussionsFor(long fixtureId, long teamId) {
            _ensureTransaction();

            var fixtureIdentifier = $"fixture:{fixtureId}.team:{teamId}";

            var preMatchDiscussionId = Guid.NewGuid();
            var matchDiscussionId = Guid.NewGuid();
            var postMatchDiscussionId = Guid.NewGuid();

            var preMatchDiscussionName = "pre-match";
            var matchDiscussionName = "match";
            var postMatchDiscussionName = "post-match";

            _txn.HashSetAsync(
                $"{fixtureIdentifier}.discussion-details",
                new[] {
                    new HashEntry($"discussion:{preMatchDiscussionId}.name", preMatchDiscussionName),
                    new HashEntry($"discussion:{preMatchDiscussionId}.is-active", 1),
                    new HashEntry($"discussion:{matchDiscussionId}.name", matchDiscussionName),
                    new HashEntry($"discussion:{matchDiscussionId}.is-active", 1),
                    new HashEntry($"discussion:{postMatchDiscussionId}.name", postMatchDiscussionName),
                    new HashEntry($"discussion:{postMatchDiscussionId}.is-active", 1)
                }
            );

            _txn.StreamAddAsync(
                $"{fixtureIdentifier}.discussion:{preMatchDiscussionId}",
                new[] {
                    new NameValueEntry("username", "The12thPlayer"),
                    new NameValueEntry(
                        "body",
                        $"Welcome to the {preMatchDiscussionName} discussion room. Please keep it civil. " +
                        "Calling players useless dickheads and swearing at the referee is fine though :) " +
                        "You can post once every 30 seconds. Thank you."
                    )
                },
                messageId: "0-1"
            );

            _txn.StreamAddAsync(
                $"{fixtureIdentifier}.discussion:{matchDiscussionId}",
                new[] {
                    new NameValueEntry("username", "The12thPlayer"),
                    new NameValueEntry(
                        "body",
                        $"Welcome to the {matchDiscussionName} discussion room. Please keep it civil. " +
                        "Calling players useless dickheads and swearing at the referee is fine though :) " +
                        "You can post once every 30 seconds. Thank you."
                    )
                },
                messageId: "0-1"
            );

            _txn.StreamAddAsync(
                $"{fixtureIdentifier}.discussion:{postMatchDiscussionId}",
                new[] {
                    new NameValueEntry("username", "The12thPlayer"),
                    new NameValueEntry(
                        "body",
                        $"Welcome to the {postMatchDiscussionName} discussion room. Please keep it civil. " +
                        "Calling players useless dickheads and swearing at the referee is fine though :) " +
                        "You can post once every 30 seconds. Thank you."
                    )
                },
                messageId: "0-1"
            );

            _txn.StreamAddAsync(
                "discussions",
                new[] {
                    new NameValueEntry("identifier", $"{fixtureIdentifier}.discussion:{preMatchDiscussionId}"),
                    new NameValueEntry("command", "sub")
                }
            );

            _txn.StreamAddAsync(
                "discussions",
                new[] {
                    new NameValueEntry("identifier", $"{fixtureIdentifier}.discussion:{matchDiscussionId}"),
                    new NameValueEntry("command", "sub")
                }
            );

            _txn.StreamAddAsync(
                "discussions",
                new[] {
                    new NameValueEntry("identifier", $"{fixtureIdentifier}.discussion:{postMatchDiscussionId}"),
                    new NameValueEntry("command", "sub")
                }
            );
        }

        public void AddFixtureParticipantsFromLineup(
            long fixtureId, long teamId, TeamLineupDto lineup
        ) {
            var manager = lineup.Manager;
            if (manager != null || lineup.StartingXI != null && lineup.StartingXI.Any()) {
                _ensureTransaction();

                var fixtureIdentifier = $"fixture:{fixtureId}.team:{teamId}";

                if (manager != null) {
                    _txn.HashSetAsync(
                        $"{fixtureIdentifier}.performance-ratings",
                        $"manager:{manager.Id}.total-rating",
                        0,
                        When.NotExists
                    );
                    _txn.HashSetAsync(
                        $"{fixtureIdentifier}.performance-ratings",
                        $"manager:{manager.Id}.total-voters",
                        0,
                        When.NotExists
                    );
                }

                if (lineup.StartingXI != null) {
                    foreach (var player in lineup.StartingXI) {
                        _txn.HashSetAsync(
                            $"{fixtureIdentifier}.performance-ratings",
                            $"player:{player.Id}.total-rating",
                            0,
                            When.NotExists
                        );
                        _txn.HashSetAsync(
                            $"{fixtureIdentifier}.performance-ratings",
                            $"player:{player.Id}.total-voters",
                            0,
                            When.NotExists
                        );
                    }
                }
            }
        }

        public void AddFixtureParticipantsFromMatchEvents(
            long fixtureId, long teamId, TeamMatchEventsDto teamMatchEvents
        ) {
            var subs = teamMatchEvents.Events?.Where(e => e.Type.ToLowerInvariant() == "substitution");
            if (subs != null && subs.Count() > 0) {
                _ensureTransaction();

                var fixtureIdentifier = $"fixture:{fixtureId}.team:{teamId}";
                
                foreach (var sub in subs) {
                    _txn.HashSetAsync(
                        $"{fixtureIdentifier}.performance-ratings",
                        $"sub:{sub.PlayerId}.total-rating",
                        0,
                        When.NotExists
                    );
                    _txn.HashSetAsync(
                        $"{fixtureIdentifier}.performance-ratings",
                        $"sub:{sub.PlayerId}.total-voters",
                        0,
                        When.NotExists
                    );
                }
            }
        }
    }
}
