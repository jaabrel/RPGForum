using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace RPGForum.Pages.Admin.Utilizadores
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Utilizadores> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<Models.Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
            [StringLength(30, ErrorMessage = "O nome de utilizador não pode exceder 30 caracteres.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            [StringLength(150, ErrorMessage = "O email não pode exceder 150 caracteres.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A palavra-passe é obrigatória.")]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "A palavra-passe deve ter pelo menos 6 caracteres.")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "A função (Role) é obrigatória.")]
            [StringLength(20)]
            public string Role { get; set; } = "Registered";
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validar e-mail duplicado
            var emailExists = await _context.Utilizadores.AnyAsync(u => u.Email == Input.Email) ||
                              await _userManager.FindByEmailAsync(Input.Email) != null;
            if (emailExists)
            {
                ModelState.AddModelError("Input.Email", "Este endereço de email já está em uso.");
                return Page();
            }

            // Validar username duplicado
            var usernameExists = await _context.Utilizadores.AnyAsync(u => u.UserName == Input.Username);
            if (usernameExists)
            {
                ModelState.AddModelError("Input.Username", "Este nome de utilizador já está em uso.");
                return Page();
            }

            // Criar utilizador no ASP.NET Identity
            var user = new Models.Utilizadores
            {
                UserName = Input.Username,
                Email = Input.Email,
                EmailConfirmed = true,
                Role = Input.Role,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                if (Input.Role == "Administrator")
                {
                    await _userManager.AddToRoleAsync(user, Input.Role);
                }

                TempData["Sucesso"] = $"Utilizador \"{Input.Username}\" criado com sucesso!";
                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
