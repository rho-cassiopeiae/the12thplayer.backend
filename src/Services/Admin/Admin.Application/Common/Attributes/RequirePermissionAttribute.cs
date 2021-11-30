using System;

using Admin.Application.Common.Enums;

namespace Admin.Application.Common.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequirePermissionAttribute : Attribute {
        public RequirePermissionAttribute() { }

        public PermissionScope Scope { get; set; }

        public int Flags { get; set; }
    }
}
