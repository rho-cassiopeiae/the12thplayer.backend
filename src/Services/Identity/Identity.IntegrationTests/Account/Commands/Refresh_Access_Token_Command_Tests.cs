using System;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

using Identity.Application.Account.Commands.ConfirmEmail;
using Identity.Application.Account.Commands.SignIn;
using Identity.Application.Account.Commands.SignUp;
using Identity.Application.Account.Common.Dto;
using Identity.Application.Account.Commands.RefreshAccessToken;
using Identity.Application.Account.Common.Errors;

namespace Identity.IntegrationTests.Account.Commands {
    [Collection(nameof(IdentityTestCollection))]
    public class Refresh_Access_Token_Command_Tests {
        private readonly Sut _sut;
        private readonly ITestOutputHelper _output;

        private readonly string _email;
        private readonly string _username;
        private readonly string _password;

        public Refresh_Access_Token_Command_Tests(Sut sut, ITestOutputHelper output) {
            _sut = sut;
            _output = output;

            _sut.ResetState();

            _email = "john@email.com";
            _username = "john";
            _password = "password";

            _sut.SendRequest(
                new SignUpCommand {
                    Email = _email,
                    Username = _username,
                    Password = _password
                }
            ).Wait();

            _sut.SendRequest(
                new ConfirmEmailCommand {
                    Email = _email,
                    ConfirmationCode = "000111"
                }
            ).Wait();
        }

        [Fact]
        public async Task Should_Fail_Refreshing_Access_Token_When_Provide_Invalid_Old_Access_Token() {
            var deviceId = Guid.NewGuid().ToString();

            var credentials = (await _sut.SendRequest(new SignInCommand {
                DeviceId = deviceId,
                Email = _email,
                Password = _password
            })).Data;

            var result = await _sut.SendRequest(new RefreshAccessTokenCommand {
                DeviceId = deviceId,
                AccessToken = credentials.AccessToken + "something",
                RefreshToken = credentials.RefreshToken
            });

            result.Error.Should().BeOfType<AccountError>().And.NotBeNull();

            _output.WriteLine(result.Error.Errors.Values.First().First());
        }

        [Fact]
        public async Task Should_Refresh_Access_Token_When_Provide_Valid_Credentials() {
            var deviceId = Guid.NewGuid().ToString();
            var otherDeviceId = Guid.NewGuid().ToString();

            await _sut.SendRequest(new SignInCommand {
                DeviceId = deviceId,
                Email = _email,
                Password = _password
            });

            await _sut.SendRequest(new SignInCommand {
                DeviceId = otherDeviceId,
                Email = _email,
                Password = _password
            });

            var credentials = (await _sut.SendRequest(new SignInCommand {
                DeviceId = deviceId,
                Email = _email,
                Password = _password
            })).Data;

            var result = await _sut.SendRequest(new RefreshAccessTokenCommand {
                DeviceId = deviceId,
                AccessToken = credentials.AccessToken,
                RefreshToken = credentials.RefreshToken
            });

            var shouldBeSecurityCredentials = result.Data.Should().BeOfType<SecurityCredentialsDto>();
            shouldBeSecurityCredentials.Which.Username.Should().BeNull();
            shouldBeSecurityCredentials.Which.AccessToken.Should().NotBeNull();
            shouldBeSecurityCredentials.Which.RefreshToken.Should().NotBeNull();
        }
    }
}
