using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Identity.Infrastructure.Account;
using Identity.Infrastructure.Account.Persistence;
using Identity.Infrastructure.Account.Persistence.Models;
using Identity.Application.Common.Interfaces;

namespace Identity.Infrastructure {
    public static class IServiceCollectionExtension {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration
        ) {
            services.AddDbContext<UserDbContext>(optionsBuilder =>
                optionsBuilder.UseNpgsql(
                    configuration.GetConnectionString("Identity"),
                    pgOptionsBuilder => pgOptionsBuilder.MigrationsHistoryTable(
                        "__EFMigrationsHistory", "identity"
                    )
                )
            );

            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            services
                .AddIdentityCore<User>(options => {
                    options.Tokens.EmailConfirmationTokenProvider =
                        "ConfirmationCodeProvider";
                    options.Tokens.PasswordResetTokenProvider =
                        "ConfirmationCodeProvider";
                    options.ClaimsIdentity.UserIdClaimType =
                        JwtRegisteredClaimNames.Sub;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._";
                    options.Password.RequiredLength = 8;
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<UserDbContext>()
                .AddDefaultTokenProviders()
                .AddConfirmationCodeProvider();

            services.AddDataProtection();
            services.AddHttpContextAccessor();
            services.TryAddScoped<SignInManager<User>>();

            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
