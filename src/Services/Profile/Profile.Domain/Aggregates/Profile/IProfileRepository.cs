using System.Threading.Tasks;

using Profile.Domain.Base;

namespace Profile.Domain.Aggregates.Profile {
    public interface IProfileRepository : IRepository<Profile> {
        Task<Profile> FindByUserId(long userId);
        void Create(Profile profile);
        void UpdatePermissions(Profile profile);
    }
}
