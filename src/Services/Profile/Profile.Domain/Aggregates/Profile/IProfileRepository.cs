using Profile.Domain.Base;

namespace Profile.Domain.Aggregates.Profile {
    public interface IProfileRepository : IRepository<Profile> {
        void Create(Profile profile);
    }
}
