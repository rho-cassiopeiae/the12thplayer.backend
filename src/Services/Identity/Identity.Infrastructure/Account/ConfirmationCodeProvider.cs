using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Account {
    public class ConfirmationCodeProvider<TUser> :
        TotpSecurityStampBasedTokenProvider<TUser> where TUser : class {
        public override Task<bool> CanGenerateTwoFactorTokenAsync(
            UserManager<TUser> userManager, TUser user
        ) => Task.FromResult(false);

        public override async Task<string> GetUserModifierAsync(
            string purpose, UserManager<TUser> userManager, TUser user
        ) {
            var email = await userManager.GetEmailAsync(user);
            return "ConfirmationCode:" + purpose + ":" + email;
        }
    }
}
