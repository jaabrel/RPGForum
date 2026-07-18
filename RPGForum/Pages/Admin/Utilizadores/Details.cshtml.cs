using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System.Threading.Tasks;

namespace RPGForum.Pages.Admin.Utilizadores
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Utilizadores Utilizador { get; set; } = default!;
        
        public int TotalBuilds { get; set; }
        public int TotalComments { get; set; }
        public int TotalLikes { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizadores
                .Include(u => u.Builds)
                .Include(u => u.Comments)
                .Include(u => u.Likes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (utilizador == null)
            {
                return NotFound();
            }

            Utilizador = utilizador;
            TotalBuilds = utilizador.Builds.Count;
            TotalComments = utilizador.Comments.Count;
            TotalLikes = utilizador.Likes.Count;

            return Page();
        }
    }
}
