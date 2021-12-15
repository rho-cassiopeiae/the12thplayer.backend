using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Author {
    public interface IAuthorRepository : IRepository<Author> {
        Task<Author> FindByUserId(long userId);
        void Create(Author author);
        void UpdatePermissions(Author author);
    }
}
