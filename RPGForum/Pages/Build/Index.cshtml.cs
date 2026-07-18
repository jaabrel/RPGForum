using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Pages.Build
{
    public class IndexModel : PageModel
    {
        private readonly RPGForum.Data.ApplicationDbContext _context;

        public IndexModel(RPGForum.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Build> Build { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Build = await _context.Builds.ToListAsync();
        }
    }
}
