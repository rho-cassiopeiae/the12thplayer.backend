using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Dto;
using Livescore.Application.Common.Results;
using Livescore.Domain.Aggregates.Player;

namespace Livescore.Application.Seed.Commands.AddTeamPlayers {
    public class AddTeamPlayersCommand : IRequest<VoidResult> {
        public long TeamId { get; init; }
        public IEnumerable<PlayerDto> Players { get; init; }
    }

    public class AddTeamPlayersCommandHandler : IRequestHandler<
        AddTeamPlayersCommand, VoidResult
    > {
        private readonly IPlayerRepository _playerRepository;

        public AddTeamPlayersCommandHandler(IPlayerRepository playerRepository) {
            _playerRepository = playerRepository;
        }

        public async Task<VoidResult> Handle(
            AddTeamPlayersCommand command, CancellationToken cancellationToken
        ) {
            var newPlayers = command.Players;

            var players = await _playerRepository.FindById(newPlayers.Select(p => p.Id));

            foreach (var newPlayer in newPlayers) {
                var player = players.FirstOrDefault(p => p.Id == newPlayer.Id);
                if (player == null) {
                    _playerRepository.Create(new Player(
                        id: newPlayer.Id,
                        teamId: command.TeamId,
                        firstName: newPlayer.FirstName,
                        lastName: newPlayer.LastName,
                        displayName: newPlayer.DisplayName,
                        birthDate: newPlayer.BirthDate != null ?
                            new DateTimeOffset(newPlayer.BirthDate.Value).ToUnixTimeMilliseconds() :
                            null,
                        countryId: newPlayer.CountryId,
                        number: newPlayer.Number,
                        position: newPlayer.Position,
                        imageUrl: newPlayer.ImageUrl,
                        lastLineupAt: new DateTimeOffset(newPlayer.LastLineupAt).ToUnixTimeMilliseconds()
                    ));
                } else {
                    if (player.TeamId != command.TeamId) {
                        player.ChangeTeam(command.TeamId);
                        player.ChangeNumber(newPlayer.Number);
                        player.SetPosition(newPlayer.Position);
                        player.SetLastLineupAt(new DateTimeOffset(newPlayer.LastLineupAt).ToUnixTimeMilliseconds());
                    }
                }
            }

            await _playerRepository.SaveChanges(cancellationToken);

            return VoidResult.Instance;
        }
    }
}
