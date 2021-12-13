using System.Threading.Tasks;

namespace Worker.Application.Common.Interfaces {
    public interface IFileHostingSeeder {
        Task<string> AddFoldersFor(long fixtureId, long teamId);
    }
}
