﻿using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Livescore.Domain.Aggregates.Team;

namespace Livescore.Infrastructure.Persistence.Repositories {
    public class TeamRepository : ITeamRepository {
        private readonly LivescoreDbContext _livescoreDbContext;

        public TeamRepository(LivescoreDbContext livescoreDbContext) {
            _livescoreDbContext = livescoreDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _livescoreDbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<Team> FindById(long id) {
            var team = await _livescoreDbContext.Teams.SingleOrDefaultAsync(
                t => t.Id == id
            );

            return team;
        }

        public async Task<IEnumerable<Team>> FindById(IEnumerable<long> ids) {
            var teams = await _livescoreDbContext.Teams
                .AsNoTracking()
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            return teams;
        }

        public void Create(Team team) {
            _livescoreDbContext.Teams.Add(team);
        }
    }
}
