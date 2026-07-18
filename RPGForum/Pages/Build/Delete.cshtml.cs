using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System.Threading.Tasks;

namespace RPGForum.Pages.Build
{
    [Authorize]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Utilizadores> _userManager;

        public DeleteModel(ApplicationDbContext context, UserManager<Models.Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Build Build { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var utilizador = await _userManager.GetUserAsync(User);
            if (utilizador == null) return RedirectToPage("/Index");

            var build = await _context.Builds
                .Include(b => b.CharClass)
                .Include(b => b.Likes)
                .Include(b => b.Comments)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (build == null) return NotFound();

            // Apenas o autor ou administrador pode apagar
            if (build.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            Build = build;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var utilizador = await _userManager.GetUserAsync(User);
            if (utilizador == null) return RedirectToPage("/Index");

            var build = await _context.Builds.FirstOrDefaultAsync(m => m.Id == id);
            if (build == null) return NotFound();

            // Apenas o autor ou administrador pode apagar
            if (build.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Build \"{build.Title}\" eliminada com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}
