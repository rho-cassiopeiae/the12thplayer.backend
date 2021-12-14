using System;

namespace Profile.Application.Common.Attributes {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireAuthorizationAttribute : Attribute {
        public string Policy { get; set; }

        public string Roles { get; set; }
    }
}
