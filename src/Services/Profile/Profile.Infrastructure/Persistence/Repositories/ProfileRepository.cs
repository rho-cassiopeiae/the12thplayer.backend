using System.Threading;
using System.Threading.Tasks;

using Profile.Domain.Aggregates.Profile;
using ProfileDm = Profile.Domain.Aggregates.Profile.Profile;

namespace Profile.Infrastructure.Persistence.Repositories {
    public class ProfileRepository : IProfileRepository {
        private readonly ProfileDbContext _profileDbContext;

        public ProfileRepository(ProfileDbContext profileDbContext) {
            _profileDbContext = profileDbContext;
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            await _profileDbContext.SaveChangesAsync(cancellationToken);
        }

        public void Create(ProfileDm profile) {
            _profileDbContext.Profiles.Add(profile);
        }
    }
}
