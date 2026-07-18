using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RPGForum.Pages.Admin.Utilizadores
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "O nome de utilizador é obrigatório.")]
            [StringLength(30, ErrorMessage = "O nome de utilizador não pode exceder 30 caracteres.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "O email é obrigatório.")]
            [EmailAddress(ErrorMessage = "Formato de email inválido.")]
            [StringLength(150, ErrorMessage = "O email não pode exceder 150 caracteres.")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "A função (Role) é obrigatória.")]
            [StringLength(20)]
            public string Role { get; set; } = "Registered";
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(m => m.Id == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            Input = new InputModel
            {
                Id = utilizador.Id,
                Username = utilizador.Username,
                Email = utilizador.Email,
                Role = utilizador.Role
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var utilizador = await _context.Utilizadores.FirstOrDefaultAsync(u => u.Id == Input.Id);
            if (utilizador == null)
            {
                return NotFound();
            }

            // Validar e-mail duplicado
            var emailExists = await _context.Utilizadores.AnyAsync(u => u.Email == Input.Email && u.Id != Input.Id);
            if (emailExists)
            {
                ModelState.AddModelError("Input.Email", "Este endereço de email já está em uso.");
                return Page();
            }

            // Validar username duplicado
            var usernameExists = await _context.Utilizadores.AnyAsync(u => u.Username == Input.Username && u.Id != Input.Id);
            if (usernameExists)
            {
                ModelState.AddModelError("Input.Username", "Este nome de utilizador já está em uso.");
                return Page();
            }

            // Procurar IdentityUser correspondente pelo e-mail antigo
            var identityUser = await _userManager.FindByEmailAsync(utilizador.Email);
            if (identityUser != null)
            {
                // Atualizar e-mail e username do IdentityUser se mudou
                if (identityUser.Email != Input.Email)
                {
                    identityUser.Email = Input.Email;
                    identityUser.UserName = Input.Email;
                    identityUser.NormalizedEmail = Input.Email.ToUpper();
                    identityUser.NormalizedUserName = Input.Email.ToUpper();
                }

                var updateResult = await _userManager.UpdateAsync(identityUser);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                // Sincronizar as Roles no Identity
                var currentRoles = await _userManager.GetRolesAsync(identityUser);
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(identityUser, currentRoles);
                }

                if (Input.Role == "Administrator" || Input.Role == "Manager")
                {
                    await _userManager.AddToRoleAsync(identityUser, Input.Role);
                }
            }

            // Atualizar na tabela Utilizadores
            utilizador.Username = Input.Username;
            utilizador.Email = Input.Email;
            utilizador.Role = Input.Role;
            utilizador.IdentityUserName = Input.Email;

            _context.Attach(utilizador).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Utilizador \"{Input.Username}\" atualizado com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}
