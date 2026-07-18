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
    public class DeleteModel : PageModel
    {
        private readonly RPGForum.Data.ApplicationDbContext _context;

        public DeleteModel(RPGForum.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var build = await _context.Builds.FindAsync(id);
            if (build != null)
            {
                Build = build;
                _context.Builds.Remove(Build);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
