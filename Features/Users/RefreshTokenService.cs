using System;
using EdAppTest.Database;
using EdAppTest.Exceptions;

namespace EdAppTest.Features.Users
{
    public interface IRefreshTokenService
    {
        RefreshTokenResponse Handle(RefreshTokenRequest request);
    }

    public class RefreshTokenService: IRefreshTokenService
    {
        private readonly IApiTokenService apiTokenService;
        private readonly IDatabaseAdapter databaseAdapter;

        public RefreshTokenService(
            IApiTokenService apiTokenService,
            IDatabaseAdapter databaseAdapter)
        {
            this.apiTokenService = apiTokenService;
            this.databaseAdapter = databaseAdapter;
        }

        public RefreshTokenResponse Handle(RefreshTokenRequest request)
        {
            var tokenResult = apiTokenService.RefreshToken(request.Token);

            if (!tokenResult.Success)
            {
                throw new UnauthorizedException(request, tokenResult.Message);
            }

            var user = databaseAdapter.GetUser(Guid.Parse(tokenResult.UserId));

            if (user == null ||
               !string.Equals(user.RefreshToken, request.RefreshToken, StringComparison.OrdinalIgnoreCase) ||
               user.RefreshTokenExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedException(request, "Invalid token");
            }

            user.RefreshToken = tokenResult.Result.RefreshToken.Token;
            user.RefreshTokenExpiresAt = tokenResult.Result.RefreshToken.Expires;

            databaseAdapter.UpsertUser(user);

            return new RefreshTokenResponse
            {
                Token = tokenResult.Result.Token,
                RefreshToken = tokenResult.Result.RefreshToken.Token
            };
        }

    }

    public class RefreshTokenRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}