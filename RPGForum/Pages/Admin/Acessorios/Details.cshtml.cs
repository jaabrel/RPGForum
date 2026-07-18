using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using System.Threading.Tasks;

namespace RPGForum.Pages.Admin.Acessorios
{
    [Authorize(Roles = "Administrator")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Acessorios Acessorio { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var acessorio = await _context.Acessorios.FirstOrDefaultAsync(m => m.Id == id);
            if (acessorio == null)
            {
                return NotFound();
            }
            else
            {
                Acessorio = acessorio;
            }
            return Page();
        }
    }
}
