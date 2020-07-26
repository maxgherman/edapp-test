using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EdAppTest.Features.Users
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAuthenticateService authenticateService;
        private readonly IRefreshTokenService refreshTokenService;

        public UsersController(
            IAuthenticateService authenticateService,
            IRefreshTokenService refreshTokenService)
        {
            this.authenticateService = authenticateService;
            this.refreshTokenService = refreshTokenService;
        }
        
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = authenticateService.Handle(new EdAppTest.Features.Users.AuthenticateRequest
            {
                UserName = request.UserName
            });

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenRefreshRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = refreshTokenService.Handle(new RefreshTokenRequest
            {
                Token = request.Token,
                RefreshToken = request.RefreshToken
            });

            return Ok(result);
        }

        public class AuthenticateRequest
        {
            [Required]
            [MinLength(1)]
            public string UserName { get; set; }

            //TODO: Add password field
        }

        public class TokenRefreshRequest
        {
            [Required]
            [MinLength(1)]
            public string Token { get; set; }

            [Required]
            [MinLength(1)]
            public string RefreshToken { get; set; }
        }
    }
}