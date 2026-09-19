using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RPGForum.Models;
using System.ComponentModel.DataAnnotations;

namespace RPGForum.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Utilizadores> _userManager;
        public ResetPasswordModel(UserManager<Utilizadores> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "A nova palavra-passe é obrigatória.")]
            [StringLength(100, ErrorMessage = "A palavra-passe deve ter pelo menos {2} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "As palavras-passe não coincidem.")]
            public string ConfirmPassword { get; set; } = string.Empty;
            [Required]
            public string Token { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                return BadRequest("O pedido de redefinição de palavra-passe é inválido.");
            }
            Input = new InputModel
            {
                Token = token,
                Email = email
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Redireciona para o login mesmo que não exista (evita enumeração de utilizadores)
                return RedirectToPage("./Login");
            }
            var result = await _userManager.ResetPasswordAsync(user, Input.Token, Input.Password);
            if (result.Succeeded)
            {
                TempData["Sucesso"] = "Palavra-passe alterada com sucesso! Podes agora iniciar sessão.";
                return RedirectToPage("./Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return Page();
        }
    }
}
