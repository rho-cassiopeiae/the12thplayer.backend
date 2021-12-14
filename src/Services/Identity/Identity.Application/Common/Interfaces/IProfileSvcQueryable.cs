using System.Collections.Generic;
using System.Threading.Tasks;

using Identity.Application.Account.Commands.LogInAsAdmin;

namespace Identity.Application.Common.Interfaces {
    public interface IProfileSvcQueryable {
        Task<IEnumerable<ProfilePermissionDto>> GetPermissionsFor(long userId);
    }
}
