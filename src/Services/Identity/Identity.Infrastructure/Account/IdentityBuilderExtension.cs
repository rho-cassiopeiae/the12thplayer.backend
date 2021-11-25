using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Account {
    public static class IdentityBuilderExtension {
        public static IdentityBuilder AddConfirmationCodeProvider(
            this IdentityBuilder builder
        ) {
            var userType = builder.UserType;
            var provider = typeof(ConfirmationCodeProvider<>)
                .MakeGenericType(userType);

            return builder.AddTokenProvider("ConfirmationCodeProvider", provider);
        }
    }
}
