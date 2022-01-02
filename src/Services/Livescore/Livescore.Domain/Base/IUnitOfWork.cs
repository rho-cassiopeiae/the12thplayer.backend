using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Livescore.Domain.Base {
    public interface IUnitOfWork : IDisposable {
        DbConnection Connection { get; }
        DbTransaction Transaction { get; }
        ValueTask Setup();
        Task Begin(IsolationLevel isolationLevel = IsolationLevel.Serializable);
        Task Commit();
        Task Rollback();
    }
}
