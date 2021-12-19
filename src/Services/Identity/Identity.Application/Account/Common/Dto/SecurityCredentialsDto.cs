using System.Text.Json.Serialization;

namespace Identity.Application.Account.Common.Dto {
    public class SecurityCredentialsDto {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Username { get; init; }
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
    }
}
