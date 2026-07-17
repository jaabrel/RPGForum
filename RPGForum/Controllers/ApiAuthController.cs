using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RPGForum.Models;
using RPGForum.Services.Jwt;

namespace RPGForum.Controllers;

/// <summary>
/// ApiAuthController - Classe para gerir autenticação da API.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ApiAuthController : ControllerBase
{
    private readonly UserManager<Utilizadores> _userManager;
    private readonly SignInManager<Utilizadores> _signInManager;
    private readonly TokenService _tokenService;

    /// <summary>
    /// Construtor do ApiAuthController.
    /// </summary>
    public ApiAuthController(UserManager<Utilizadores> userManager, SignInManager<Utilizadores> signInManager, TokenService tokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    /// <summary>
    /// Login - Endpoint para fazer login.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            
            if (result.Succeeded)
            {
                // Usa o teu TokenService para gerar o JWT
                var token = _tokenService.GenerateToken(user); 
                return Ok(new { token = token });
            }
        }
        return Unauthorized(new { message = "Credenciais inválidas" });
    }
}

/// <summary>
/// LoginDto - Estrutura de login.
/// </summary>
public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}