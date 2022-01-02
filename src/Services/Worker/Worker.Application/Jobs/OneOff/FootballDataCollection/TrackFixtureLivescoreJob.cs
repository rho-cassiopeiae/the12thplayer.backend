using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Quartz;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Application.Jobs.OneOff.FootballDataCollection {
    public class TrackFixtureLivescoreJob : OneOffJob {
        private readonly IFootballDataProvider _footballDataProvider;
        private readonly IFileHostingSeeder _fileHostingSeeder;
        private readonly IFixtureLivescoreNotifier _fixtureLivescoreNotifier;
        private readonly ILivescoreSvcQueryable _livescoreSvcQueryable;
        private readonly ILivescoreSeeder _livescoreSeeder;

        private long _fixtureId;
        private long _teamId;
        private long _opponentTeamId;
        private string _vimeoProjectId;

        private bool _emulateOngoing;
        private int _kickOffInMin;
        private int _emulateForMin;

        public TrackFixtureLivescoreJob(
            ILogger<TrackFixtureLivescoreJob> logger,
            IFootballDataProvider footballDataProvider,
            IFileHostingSeeder fileHostingSeeder,
            IFixtureLivescoreNotifier fixtureLivescoreNotifier,
            ILivescoreSvcQueryable livescoreSvcQueryable,
            ILivescoreSeeder livescoreSeeder
        ) : base(logger) {
            _footballDataProvider = footballDataProvider;
            _fileHostingSeeder = fileHostingSeeder;
            _fixtureLivescoreNotifier = fixtureLivescoreNotifier;
            _livescoreSvcQueryable = livescoreSvcQueryable;
            _livescoreSeeder = livescoreSeeder;
        }

        protected override async Task<IDictionary<string, object>> _execute() {
            _fixtureId = long.Parse(_context.MergedJobDataMap.GetString("FixtureId"));
            _teamId = long.Parse(_context.MergedJobDataMap.GetString("TeamId"));

            _emulateOngoing = false;
            if (_context.MergedJobDataMap.TryGetValue("EmulateOngoing", out var value)) {
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

            try {
                _vimeoProjectId = await _fileHostingSeeder.AddFoldersFor(_fixtureId, _teamId);
            } catch (Exception e) {
                _logger.LogError(
                    e,
                    "Fixture {FixtureId} Team {TeamId}: Failed to create a Vimeo project",
                    _fixtureId, _teamId
                );

                throw;
                // @@??: What to do if for some reason can't create a vimeo project ?
                // Send a notification via slack, create a project manually, submit the id via admin panel.
                // Wait here for it ?
            }

            await _fixtureLivescoreNotifier.NotifyFixtureActivated(_fixtureId, _teamId, _vimeoProjectId);

            await _monitorPrematch(startTime.Value);

            await _monitorLive();

            // @@TODO: Keep monitoring for some time after the final whistle.

            await _fixtureLivescoreNotifier.NotifyFixtureFinished(_fixtureId, _teamId);
            
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Finished",
                _fixtureId, _teamId
            );

            await _scheduleFinalizeFixtureJob();

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
                    _logger.LogError(
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
            if (startTime == DateTime.MinValue) {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: No start time",
                    _fixtureId, _teamId
                );

                return null;
            }
            if (startTime.Subtract(DateTime.UtcNow) > TimeSpan.FromMinutes(65)) {
                _logger.LogInformation(
                    "Fixture {FixtureId} Team {TeamId}: Start time moved to {StartTime}",
                    _fixtureId, _teamId, startTime
                );
                // @@TODO: Schedule for later.
                return null;
            }

            return startTime;
        }

        private async Task _monitorPrematch(DateTime startTime) {
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Monitor pre-match. Kick-off in {KickOffIn}",
                _fixtureId, _teamId, startTime.Subtract(DateTime.UtcNow)
            );

            var teamPlayers = (await _livescoreSvcQueryable.GetTeamPlayers(_teamId)).ToList();
            var opponentTeamPlayers = new List<PlayerDto>();

            var finalPrematchUpdate = false;
            while (!_jobCanceled) {
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
                            player.LastLineupAt = playerLineupEntry.FixtureStartTime;
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
                                "Fixture {FixtureId} Team {TeamId}: Error trying to add new players to the team",
                                _fixtureId, _teamId
                            );
                        }

                        teamPlayers.AddRange(unknownPlayers);
                    }

                    foreach (var playerLineupEntry in playerLineupEntries) {
                        var player = teamPlayers.First(p => p.Id == playerLineupEntry.Id);
                        playerLineupEntry.FirstName = player.FirstName;
                        playerLineupEntry.LastName = player.LastName;
                        playerLineupEntry.DisplayName = player.DisplayName;
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
                        var unknownOpponentPlayers = (await _footballDataProvider.GetPlayers(unknownOpponentPlayerIds))
                            .ToList();

                        opponentTeamPlayers.AddRange(unknownOpponentPlayers);
                    }

                    foreach (var opponentPlayerLineupEntry in opponentPlayerLineupEntries) {
                        var player = opponentTeamPlayers.First(p => p.Id == opponentPlayerLineupEntry.Id);
                        opponentPlayerLineupEntry.FirstName = player.FirstName;
                        opponentPlayerLineupEntry.LastName = player.LastName;
                        opponentPlayerLineupEntry.DisplayName = player.DisplayName;
                        opponentPlayerLineupEntry.ImageUrl = player.ImageUrl;
                    }

                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: New pre-match update",
                        _fixtureId, _teamId
                    );

                    await _fixtureLivescoreNotifier.NotifyFixturePrematchUpdated(
                        _fixtureId, _teamId, fixture
                    );
                }

                if (finalPrematchUpdate) {
                    return;
                }

                var sleepInterval = TimeSpan.FromMinutes(2); // @@TODO: Config.
                var now = DateTime.UtcNow;
                var startsIn = startTime.Subtract(now);
                if (startsIn < sleepInterval + TimeSpan.FromSeconds(30)) {
                    sleepInterval = startsIn > TimeSpan.FromSeconds(30) ?
                        startTime.Subtract(TimeSpan.FromSeconds(30)).Subtract(now) :
                        TimeSpan.Zero;

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

        private async Task _monitorLive() {
            _logger.LogInformation(
                "Fixture {FixtureId} Team {TeamId}: Monitor live",
                _fixtureId, _teamId
            );

            var fixtureFinished = false;
            while (!_jobCanceled) {
                var fixture = await _footballDataProvider.GetFixtureLivescore(
                    _fixtureId,
                    _teamId,
                    _emulateOngoing,
                    includeEventsAndStats: true
                );
                // @@TODO: Include lineups for the first N minutes of the game.

                if (fixture != null) {
                    _logger.LogInformation(
                        "Fixture {FixtureId} Team {TeamId}: New live update",
                        _fixtureId, _teamId
                    );

                    await _fixtureLivescoreNotifier.NotifyFixtureLiveUpdated(_fixtureId, _teamId, fixture);

                    if (
                        !_emulateOngoing && fixture.Status is ("FT" or "AET" or "FT_PEN") ||
                        _emulateOngoing && _emulateForMin == 0
                    ) {
                        fixtureFinished = true;
                    }
                }

                if (fixtureFinished) {
                    return;
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

        private Task _scheduleFinalizeFixtureJob() {
            var trigger = TriggerBuilder.Create()
                .ForJob("Finalize fixture")
                .WithSimpleSchedule(scheduleBuilder => scheduleBuilder.WithRepeatCount(0))
                .StartAt(DateTime.UtcNow.AddHours(double.Parse(
                    _context.MergedJobDataMap.GetString("FinishedFixtureStaysActiveForHours")
                )))
                .UsingJobData(new JobDataMap(
                    (IDictionary<string, object>)
                    new Dictionary<string, object> {
                        ["FixtureId"] = _fixtureId.ToString(),
                        ["TeamId"] = _teamId.ToString(),
                        ["VimeoProjectId"] = _vimeoProjectId
                    }
                ))
                .Build();

            return _context.Scheduler.ScheduleJob(trigger);
        }
    }
}
