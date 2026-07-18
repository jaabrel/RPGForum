using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGForum.Pages.Admin.Utilizadores
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Utilizadores> Utilizadores { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public async Task OnGetAsync()
        {
            var query = _context.Utilizadores
                .Include(u => u.Builds)
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                query = query.Where(u => u.UserName.Contains(SearchString) || u.Email.Contains(SearchString));
            }

            Utilizadores = await query.OrderByDescending(u => u.CreatedAt).ToListAsync();
        }
    }
}
