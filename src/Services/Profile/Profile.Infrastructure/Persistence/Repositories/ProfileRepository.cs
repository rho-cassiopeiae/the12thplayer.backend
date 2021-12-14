using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MediatR;

using Profile.Domain.Base;
using Profile.Domain.Aggregates.Profile;
using ProfileDm = Profile.Domain.Aggregates.Profile.Profile;

namespace Profile.Infrastructure.Persistence.Repositories {
    public class ProfileRepository : IProfileRepository {
        private readonly ProfileDbContext _profileDbContext;
        private readonly IPublisher _mediator;

        private IUnitOfWork _unitOfWork;

        public ProfileRepository(
            ProfileDbContext profileDbContext,
            IPublisher mediator
        ) {
            _profileDbContext = profileDbContext;
            _mediator = mediator;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _profileDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _profileDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            var entities = _profileDbContext.ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents != null && entity.DomainEvents.Any())
                .ToList();

            var events = entities
                .SelectMany(entity => entity.DomainEvents)
                .ToList();

            foreach (var entity in entities) {
                entity.ClearDomainEvents();
            }

            foreach (var @event in events) {
                await _mediator.Publish(@event);
            }

            await _profileDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<ProfileDm> FindByUserId(long userId) {
            return _profileDbContext.Profiles
                .Include(p => p.Permissions)
                .SingleAsync(p => p.UserId == userId);
        }

        public void Create(ProfileDm profile) {
            _profileDbContext.Profiles.Add(profile);
        }

        public void UpdatePermissions(ProfileDm profile) {
            var trackedPermissions = _profileDbContext.ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .OfType<ProfilePermission>()
                .ToList();

            var newPermissions = profile.Permissions.Where(p => !trackedPermissions.Contains(p));
            _profileDbContext.AddRange(newPermissions);
        }
    }
}
