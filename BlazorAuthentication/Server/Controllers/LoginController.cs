using BlazorAuthentication.Server.Data;
using BlazorAuthentication.Server.Services;
using BlazorAuthentication.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace BlazorAuthentication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenService tokenService;
        private readonly UserManager<User> userManager;

        public LoginController(ITokenService tokenService, UserManager<User> userManager)
        {
            this.tokenService = tokenService;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var user = await userManager.FindByNameAsync(login.Email);
            var validPassword = await userManager.CheckPasswordAsync(user, login.Password);

            if (user == null || validPassword == false)
            {
                return Unauthorized(new AuthResponse { Successful = false, Error = "Username and password are invalid." });
            } 
            
            var signingCredentials = tokenService.GetSigningCredentials();
            var claims = await tokenService.GetClaims(user);
            var tokenOptions = tokenService.GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            user.RefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await userManager.UpdateAsync(user);

            return Ok(new AuthResponse { Successful = true, Token = token, RefreshToken = user.RefreshToken });
        }
    }
}
