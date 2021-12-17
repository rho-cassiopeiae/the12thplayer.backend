using System.Threading.Tasks;

using Feed.Domain.Base;

namespace Feed.Domain.Aggregates.Author {
    public interface IAuthorRepository : IRepository<Author> {
        Task Create(Author author);
        Task UpdatePermissions(Author author);
    }
}
