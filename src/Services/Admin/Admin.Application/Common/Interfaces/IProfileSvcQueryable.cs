using System.Collections.Generic;
using System.Threading.Tasks;

using Admin.Application.Common.Attributes;

namespace Admin.Application.Common.Interfaces {
    public interface IProfileSvcQueryable {
        Task<bool> CheckHasPermissions(
            long userId,
            IEnumerable<RequirePermissionAttribute> permissionAttributes
        );
    }
}
