using System;
using EdAppTest.Database;

namespace EdAppTest.Features.Users
{
    public interface IAuthenticateService
    {
        AuthenticateResponse Handle(AuthenticateRequest request);
    }

    public class AuthenticateService: IAuthenticateService
    {
        private readonly IApiTokenService apiTokenService;
        private readonly IDatabaseAdapter databaseAdapter;

        public AuthenticateService(
                IApiTokenService apiTokenService,
                IDatabaseAdapter databaseAdapter)
        {
            this.apiTokenService = apiTokenService;
            this.databaseAdapter = databaseAdapter;
        }

        public AuthenticateResponse Handle(AuthenticateRequest request)
        {
            var user = databaseAdapter.GetUserByUserName(request.UserName);

            if (user == null)
            {
                user = new Domain.User
                {
                    Id = Guid.NewGuid(),
                    UserName = request.UserName
                };
            }

            var token = apiTokenService.GenerateToken(new GenerateTokenRequest
            {
                UserId = user.Id.ToString()
            });

            user.RefreshToken = token.RefreshToken.Token;
            user.RefreshTokenExpiresAt = token.RefreshToken.Expires;

            databaseAdapter.UpsertUser(user);

            return new AuthenticateResponse
            {
                Token = token.Token,
                RefreshToken = token.RefreshToken.Token
            };
        }
    }

    public class AuthenticateRequest
    {
        public string UserName { get; set; }
    }

    public class AuthenticateResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
