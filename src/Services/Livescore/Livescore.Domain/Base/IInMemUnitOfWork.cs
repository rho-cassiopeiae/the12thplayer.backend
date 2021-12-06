using System.Threading.Tasks;

using StackExchange.Redis;

namespace Livescore.Domain.Base {
    public interface IInMemUnitOfWork {
        ITransaction Transaction { get; }
        void Begin();
        Task<bool> Commit();
    }
}
