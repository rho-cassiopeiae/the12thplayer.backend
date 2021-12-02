using System;
using System.Threading;
using System.Threading.Tasks;

namespace Livescore.Domain.Base {
    public interface IRepository<T> where T : IAggregateRoot {
        void EnlistConnectionFrom(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        void EnlistTransactionFrom(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        void EnlistAsPartOf(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        Task SaveChanges(CancellationToken cancellationToken);
    }
}
