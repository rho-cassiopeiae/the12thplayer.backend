using System.Threading.Tasks;

namespace Livescore.Application.Common.Interfaces {
    public interface IFixtureLivescoreStatusInMemQueryable {
        Task<bool> CheckActive(long fixtureId, long teamId);
    }
}
