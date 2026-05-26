using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;

namespace RPGForum.Pages.PersonagensPages;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Personagens Personagens { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var personagens = await _context.Personagens.FirstOrDefaultAsync(m => m.Id == id);
        if (personagens is null)
        {
            return NotFound();
        }
        else
        {
            Personagens = personagens;
        }

        return Page();
    }
}
