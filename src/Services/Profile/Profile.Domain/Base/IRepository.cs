using System.Threading;
using System.Threading.Tasks;

namespace Profile.Domain.Base {
    public interface IRepository<T> where T : IAggregateRoot {
        Task SaveChanges(CancellationToken cancellationToken);
    }
}
