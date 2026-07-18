using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace RPGForum.Areas.Identity.Pages.Account
{
    public class ConfirmCodeModel : PageModel
    {
        private readonly UserManager<Utilizadores> _userManager;
        private readonly SignInManager<Utilizadores> _signInManager;
        private readonly ApplicationDbContext _context;

        public ConfirmCodeModel(
            UserManager<Utilizadores> userManager, 
            SignInManager<Utilizadores> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
            public string Username { get; set; }

            [Required(ErrorMessage = "O código é obrigatório.")]
            [StringLength(6, MinimumLength = 6, ErrorMessage = "O código deve ter exatamente 6 dígitos.")]
            [RegularExpression(@"^[0-9]+$", ErrorMessage = "O código deve conter apenas números.")]
            public string Code { get; set; }
        }

        public void OnGet(string email, string username, string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            Input = new InputModel 
            { 
                Email = email,
                Username = username
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Utilizador não encontrado.");
                return Page();
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, "Email", "ConfirmEmail", Input.Code);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "O código inserido é inválido ou já expirou.");
                return Page();
            }

            // Confirmar o e-mail do utilizador no Identity
            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // Criar o registo na tabela Utilizadores agora que a conta está confirmada
                var exists = await _context.Utilizadores.AnyAsync(u => u.Email == Input.Email);
                if (!exists)
                {
                    var utilizador = new Utilizadores
                    {
                        UserName = Input.Username,
                        Email = Input.Email,
                        Role = "Registered",
                        CreatedAt = DateTime.UtcNow,
                    };

                    _context.Utilizadores.Add(utilizador);
                    await _context.SaveChangesAsync();
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
