using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RPGForumApi.Models;
using RPGForumApi.Services;

namespace RPGForumApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TokenService _tokenService;

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            TokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Efetuar login na API e obter um token JWT
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ApiLoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // Tenta procurar por UserName se não encontrar por email
                user = await _userManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    return Unauthorized(new { message = "Credenciais inválidas" });
                }
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Credenciais inválidas" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return Ok(new
            {
                token = token,
                expiration = DateTime.UtcNow.AddHours(2),
                username = user.UserName,
                email = user.Email,
                roles = roles
            });
        }
    }
}
