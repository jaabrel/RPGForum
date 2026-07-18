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
    public class DetailsModel : PageModel
    {
        private readonly RPGForum.Data.ApplicationDbContext _context;

        public DetailsModel(RPGForum.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Models.Build Build { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = await _context.Builds.FirstOrDefaultAsync(m => m.Id == id);

            if (build is not null)
            {
                Build = build;

                return Page();
            }

            return NotFound();
        }
    }
}
