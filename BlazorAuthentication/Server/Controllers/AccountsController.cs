using BlazorAuthentication.Server.Data;
using BlazorAuthentication.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuthentication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> userManager;

        public AccountsController(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisterRequest model)
        {
            var newUser = new User { UserName = model.Email, Email = model.Email };

            var result = await userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded == false)
            {
                var errors = result.Errors.Select(error => error.Description);

                return BadRequest(new RegisterResponse { Successful = false, Errors = errors });
            }

            await userManager.AddToRoleAsync(newUser, "User");

            return Ok(new RegisterResponse { Successful = true });
        }
    }
}
