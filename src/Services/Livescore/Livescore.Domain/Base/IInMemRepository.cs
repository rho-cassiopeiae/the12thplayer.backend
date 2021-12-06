using System;
using System.Threading.Tasks;

namespace Livescore.Domain.Base {
    public interface IInMemRepository<T> where T : IAggregateRoot {
        void EnlistAsPartOf(IInMemUnitOfWork unitOfWork) => throw new NotSupportedException();
        ValueTask<bool> SaveChanges();
    }
}
