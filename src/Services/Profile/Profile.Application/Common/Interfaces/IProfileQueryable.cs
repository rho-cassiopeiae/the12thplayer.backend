using System.Collections.Generic;
using System.Threading.Tasks;

using Profile.Domain.Aggregates.Profile;

namespace Profile.Application.Common.Interfaces {
    public interface IProfileQueryable {
        Task<IEnumerable<ProfilePermission>> GetPermissionsFor(long userId);
    }
}
