using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using MediatR;

using Feed.Domain.Base;
using Feed.Domain.Aggregates.Author;
using AuthorDm = Feed.Domain.Aggregates.Author.Author;

namespace Feed.Infrastructure.Persistence.Repositories {
    public class AuthorRepository : IAuthorRepository {
        private readonly FeedDbContext _feedDbContext;
        private readonly IPublisher _mediator;

        private IUnitOfWork _unitOfWork;

        public AuthorRepository(
            FeedDbContext feedDbContext,
            IPublisher mediator
        ) {
            _feedDbContext = feedDbContext;
            _mediator = mediator;
        }

        public void EnlistAsPartOf(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
            _feedDbContext.Database.SetDbConnection(unitOfWork.Connection);
            _feedDbContext.Database.UseTransaction(unitOfWork.Transaction);
        }

        public async Task SaveChanges(CancellationToken cancellationToken) {
            var entities = _feedDbContext.ChangeTracker
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

            await _feedDbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<AuthorDm> FindByUserId(long userId) {
            return _feedDbContext.Authors
                .Include(a => a.Permissions)
                .SingleAsync(a => a.UserId == userId);
        }

        public void Create(AuthorDm author) {
            _feedDbContext.Authors.Add(author);
        }

        public void UpdatePermissions(AuthorDm author) {
            var trackedPermissions = _feedDbContext.ChangeTracker
                .Entries<Entity>()
                .Select(entry => entry.Entity)
                .OfType<AuthorPermission>()
                .ToList();

            var newPermissions = author.Permissions.Where(p => !trackedPermissions.Contains(p));
            _feedDbContext.AddRange(newPermissions);
        }
    }
}
