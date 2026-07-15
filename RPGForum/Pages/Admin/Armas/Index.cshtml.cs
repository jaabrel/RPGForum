using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Pages.Admin.Armas
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Armas> Armas { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Armas = await _context.Armas.ToListAsync();
        }
    }
}
