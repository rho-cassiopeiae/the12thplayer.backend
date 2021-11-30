using System.Collections.Generic;
using System.Threading.Tasks;

using Admin.Application.Common.Attributes;

namespace Admin.Application.Common.Interfaces {
    public interface IProfilePermissionChecker {
        Task<bool> HasPermissions(
            long userId,
            IEnumerable<RequirePermissionAttribute> permissionAttributes
        );
    }
}
