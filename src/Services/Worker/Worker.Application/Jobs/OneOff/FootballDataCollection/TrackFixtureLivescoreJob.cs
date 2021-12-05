﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class TrackFixtureLivescoreJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly IFixtureLivescoreNotifier _fixtureLivescoreNotifier;
        private readonly ILivescoreSvcQueryable _livescoreSvcQueryable;
        private readonly ILivescoreSeeder _livescoreSeeder;

        private long _fixtureId;
        private long _teamId;
        private long _opponentTeamId;

        private bool _emulateOngoing;
        private int _kickOffInMin;
        private int _emulateForMin;

        public TrackFixtureLivescoreJob(
            ILogger<TrackFixtureLivescoreJob> logger,
            IFootballDataProvider footballDataProvider,
            IFixtureLivescoreNotifier fixtureLivescoreNotifier,
            ILivescoreSvcQueryable livescoreSvcQueryable,
            ILivescoreSeeder livescoreSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _fixtureLivescoreNotifier = fixtureLivescoreNotifier;
            _livescoreSvcQueryable = livescoreSvcQueryable;
            _livescoreSeeder = livescoreSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            _fixtureId = long.Parse(_context.MergedJobDataMap.GetString("FixtureId"));
            _teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));

            _emulateOngoing = false;
            if (_context.MergedJobDataMap.TryGetValue(
                "EmulateOngoing", out var value
            )) {
                _emulateOngoing = bool.Parse((string) value);
            }

            if (_emulateOngoing) {
                _kickOffInMin = int.Parse(_context.MergedJobDataMap.GetString("KickOffInMin"));
                _emulateForMin = int.Parse(_context.MergedJobDataMap.GetString("EmulateForMin"));
            }

            var startTime = await _checkMatchStartsOnSchedule();
            if (startTime == null) {
                // @@TODO: Publish an event notifying that the fixture wouldn't start when expected.
                return null;
            }

            await _fixtureLivescoreNotifier.NotifyFixtureActivated(_fixtureId, _teamId);

            await _monitorPrematch(startTime.Value);

            await _monitorLive();

            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Full-time",
                _fixtureId, _teamId
            );

            return null;
        }

        private async Task<DateTime?> _checkMatchStartsOnSchedule() {
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Check match starts on schedule",
                _fixtureId, _teamId
            );

            // @@TODO: Use Polly.
            FixtureDto fixture = null;
            int maxRepeats = 5;
            int totalRepeats = 0;
            while (true) {
                fixture = await _footballDataProvider.GetFixtureLivescore(
                    _fixtureId, _teamId, _emulateOngoing
                );
                if (fixture != null) {
                    break;
                }

                if (totalRepeats++ < maxRepeats) {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                } else {
                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: No livescore data",
                        _fixtureId, _teamId
                    );

                    return null;
                }
            }

            _opponentTeamId = fixture.OpponentTeam.Id;

            if (_emulateOngoing) {
                return DateTime.UtcNow.AddMinutes(_kickOffInMin);
            }

            var status = fixture.Status.ToUpperInvariant();
            var startTime = fixture.StartTime;
            
            if (status == "CANCL") {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: Canceled",
                    _fixtureId, _teamId
                );

                return null;
            }
            if (status == "POSTP") {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: Postponed",
                    _fixtureId, _teamId
                );
                // @@TODO: Schedule for later.
                return null;
            }
            if (status == "DELAYED") {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: Delayed",
                    _fixtureId, _teamId
                );
                // @@TODO: Schedule for later.
                return null;
            }
            if (startTime == null) {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: No start time",
                    _fixtureId, _teamId
                );

                return null;
            }
            if (startTime.Value.Subtract(DateTime.UtcNow) > TimeSpan.FromMinutes(65)) {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: Start time moved to {StartTime}",
                    _fixtureId, _teamId, startTime.Value
                );
                // @@TODO: Schedule for later.
                return null;
            }

            return startTime.Value;
        }

        private async Task _monitorPrematch(DateTime startTime) {
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Monitor pre-match",
                _fixtureId, _teamId
            );
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Kick-off in {KickOffIn}",
                _fixtureId, _teamId, startTime.Subtract(DateTime.UtcNow)
            );

            var teamPlayers = (await _livescoreSvcQueryable.GetTeamPlayers(_teamId)).ToList();
            var opponentTeamPlayers = new List<PlayerDto>();

            var finalPrematchUpdate = false;
            while (!_isCanceled) {
                var fixture = await _footballDataProvider.GetFixtureLivescore(
                    _fixtureId,
                    _teamId,
                    _emulateOngoing,
                    includeReferee: true,
                    includeLineups: true
                );

                if (fixture != null) {
                    var teamLineup = fixture.Lineups.First(l => l.TeamId == _teamId);

                    var playerLineupEntries =
                        (teamLineup.StartingXI ?? new List<TeamLineupDto.PlayerDto>())
                        .Concat(teamLineup.Subs ?? new List<TeamLineupDto.PlayerDto>())
                        .ToList();

                    var playerIds = playerLineupEntries
                        .Select(player => player.Id)
                        .Distinct();

                    var unknownPlayerIds = playerIds
                        .Where(playerId => !teamPlayers.Any(p => p.Id == playerId))
                        .ToList();

                    if (unknownPlayerIds.Count > 0) {
                        var unknownPlayers = (await _footballDataProvider.GetPlayers(unknownPlayerIds)).ToList();

                        foreach (var player in unknownPlayers) {
                            var playerLineupEntry = playerLineupEntries.First(p => p.Id == player.Id);
                            player.Number = playerLineupEntry.Number;
                            player.LastLineupAt = playerLineupEntry.FixtureStartTime.Value;
                        }

                        try {
                            _logger.LogInformation(
                                "Fixture {FixtureId} Team {TeamId}: Adding new players",
                                _fixtureId, _teamId
                            );

                            await _livescoreSeeder.AddTeamPlayers(_teamId, unknownPlayers);
                        } catch (Exception e) {
                            _logger.LogError(
                                e,
                                "Fixture {FixtureId} Team {TeamId}: Error trying to add previously unknown players",
                                _fixtureId, _teamId
                            );
                        }

                        teamPlayers.AddRange(unknownPlayers);
                    }

                    foreach (var playerLineupEntry in playerLineupEntries) {
                        var player = teamPlayers.First(p => p.Id == playerLineupEntry.Id);
                        playerLineupEntry.FirstName = player.FirstName;
                        playerLineupEntry.LastName = player.LastName;
                        playerLineupEntry.ImageUrl = player.ImageUrl;
                    }

                    var opponentTeamLineup = fixture.Lineups.First(l => l.TeamId == _opponentTeamId);

                    var opponentPlayerLineupEntries =
                        (opponentTeamLineup.StartingXI ?? new List<TeamLineupDto.PlayerDto>())
                        .Concat(opponentTeamLineup.Subs ?? new List<TeamLineupDto.PlayerDto>())
                        .ToList();

                    var opponentPlayerIds = opponentPlayerLineupEntries
                        .Select(player => player.Id)
                        .Distinct();

                    var unknownOpponentPlayerIds = opponentPlayerIds
                        .Where(playerId => !opponentTeamPlayers.Any(p => p.Id == playerId))
                        .ToList();

                    if (unknownOpponentPlayerIds.Count > 0) {
                        var unknownOpponentPlayers = (
                            await _footballDataProvider.GetPlayers(unknownOpponentPlayerIds)
                        ).ToList();

                        opponentTeamPlayers.AddRange(unknownOpponentPlayers);
                    }

                    foreach (var opponentPlayerLineupEntry in opponentPlayerLineupEntries) {
                        var player = opponentTeamPlayers.First(p => p.Id == opponentPlayerLineupEntry.Id);
                        opponentPlayerLineupEntry.FirstName = player.FirstName;
                        opponentPlayerLineupEntry.LastName = player.LastName;
                        opponentPlayerLineupEntry.ImageUrl = player.ImageUrl;
                    }

                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: Notifying about a new pre-match update",
                        _fixtureId, _teamId
                    );

                    await _fixtureLivescoreNotifier.NotifyFixturePrematchUpdated(
                        _fixtureId, _teamId, fixture
                    );
                }

                if (finalPrematchUpdate) {
                    return;
                }

                var sleepInterval = TimeSpan.FromMinutes(2);
                var now = DateTime.UtcNow;
                var startsIn = startTime.Subtract(now);
                if (startsIn <= TimeSpan.FromMinutes(3)) {
                    sleepInterval = startsIn > TimeSpan.FromSeconds(90) ?
                        startTime.Subtract(TimeSpan.FromMinutes(1)).Subtract(now) :
                        TimeSpan.Zero
                    ;
                    finalPrematchUpdate = true;
                }

                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: Sleep for {SleepInterval}",
                    _fixtureId, _teamId, sleepInterval
                );

                try {
                    await Task.Delay(sleepInterval, _context.CancellationToken);
                } catch {
                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: Job canceled",
                        _fixtureId, _teamId
                    );
                }
            }
        }

        private bool _isFixtureCompleted(FixtureDto fixture) =>
            fixture.Status == "FT" || fixture.Status == "AET" || fixture.Status == "FT_PEN";

        private async Task _monitorLive() {
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Monitor live",
                _fixtureId, _teamId
            );

            var fixtureCompleted = false;
            while (!_isCanceled && !fixtureCompleted) {
                var fixture = await _footballDataProvider.GetFixtureLivescore(
                    _fixtureId,
                    _teamId,
                    _emulateOngoing,
                    includeEventsAndStats: true
                );
                // @@TODO: Include lineups for the first N minutes of the game.

                if (fixture != null) {
                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: Notifying about a new live update",
                        _fixtureId, _teamId
                    );

                    await _fixtureLivescoreNotifier.NotifyFixtureLiveUpdated(_fixtureId, _teamId, fixture);

                    if (
                        !_emulateOngoing && _isFixtureCompleted(fixture) ||
                        _emulateOngoing && _emulateForMin == 0
                    ) {
                        fixtureCompleted = true;
                    }
                }

                if (_emulateOngoing) {
                    --_emulateForMin;
                }

                try {
                    await Task.Delay(TimeSpan.FromMinutes(1), _context.CancellationToken);
                } catch {
                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: Job canceled",
                        _fixtureId, _teamId
                    );
                }
            }
        }
    }
}
