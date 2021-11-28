using System.Threading;
using System.Threading.Tasks;

namespace Identity.Domain.Base {
    public interface IRepository<T> where T : IAggregateRoot {
        void EnlistAsPartOf(IUnitOfWork unitOfWork);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
