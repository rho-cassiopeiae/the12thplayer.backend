using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Domain.Base {
    public interface IRepository<T> where T : IAggregateRoot {
        void EnlistConnectionFrom(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        void EnlistTransactionFrom(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        void EnlistAsPartOf(IUnitOfWork unitOfWork) => throw new NotSupportedException();
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
