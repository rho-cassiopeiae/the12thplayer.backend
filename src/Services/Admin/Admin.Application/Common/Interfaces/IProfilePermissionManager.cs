using System.Collections.Generic;
using System.Threading.Tasks;

using Admin.Application.Profile.Common.Dto;

namespace Admin.Application.Common.Interfaces {
    public interface IProfilePermissionManager {
        Task GrantPermissionsTo(long userId, IEnumerable<ProfilePermissionDto> permissions);
    }
}
