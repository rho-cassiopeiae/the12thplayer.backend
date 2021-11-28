using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Identity.Domain.Base {
    public interface IUnitOfWork : IDisposable {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }
        ValueTask Setup();
        Task Begin();
        Task Commit();
        Task Rollback();
    }
}
