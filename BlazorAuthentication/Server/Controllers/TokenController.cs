using BlazorAuthentication.Server.Services;
using BlazorAuthentication.Server.Data;
using BlazorAuthentication.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorAuthentication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly ITokenService tokenService;

        public TokenController(UserManager<User> userManager, ITokenService tokenService)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            if (refreshTokenRequest is null)
            {
                return BadRequest(new AuthResponse { Successful = false, Error = "Invalid client request" });
            }

            var principal = tokenService.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var username = principal.Identity.Name;
            var user = await userManager.FindByEmailAsync(username);

            if (user == null || user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest(new AuthResponse { Successful = false, Error = "Invalid client request" });
            }

            var signingCredentials = tokenService.GetSigningCredentials();
            var claims = await tokenService.GetClaims(user);
            var tokenOptions = tokenService.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            user.RefreshToken = tokenService.GenerateRefreshToken();
            await userManager.UpdateAsync(user);

            return Ok(new AuthResponse { Successful = true, Token = token, RefreshToken = user.RefreshToken });
        }
    }
}
