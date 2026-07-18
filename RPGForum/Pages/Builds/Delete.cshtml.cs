using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Pages.Builds
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Build Build { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Index");

            var build = await _context.Builds
                .Include(b => b.CharClass)
                .Include(b => b.Likes)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null) return NotFound();

            // Apenas o autor ou administrador pode apagar
            if (build.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            Build = build;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Index");

            var build = await _context.Builds
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null) return NotFound();

            if (build.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            var titulo = build.Title;
            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Build \"{titulo}\" apagada com sucesso.";
            return RedirectToPage("Index");
        }

        private async Task<Utilizadores?> ObterUtilizadorAtualAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserName == identityUser.UserName);
        }
    }

}
