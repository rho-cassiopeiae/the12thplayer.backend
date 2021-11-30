using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Profile.Application.Common.Interfaces;
using Profile.Domain.Aggregates.Profile;

namespace Profile.Infrastructure.Persistence.Queryables {
    public class ProfileQueryable : IProfileQueryable {
        private readonly ProfileDbContext _profileDbContext;

        public ProfileQueryable(ProfileDbContext profileDbContext) {
            _profileDbContext = profileDbContext;
        }

        public async Task<IEnumerable<ProfilePermission>> GetPermissionsFor(
            long userId
        ) {
            var profile = await _profileDbContext.Profiles
                .AsNoTracking()
                .Include(p => p.Permissions)
                .SingleAsync(p => p.UserId == userId);

            return profile.Permissions;
        }
    }
}
