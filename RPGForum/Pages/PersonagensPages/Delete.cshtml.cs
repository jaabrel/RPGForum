using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;

namespace RPGForum.Pages.PersonagensPages;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var personagens = await _context.Personagens.FindAsync(id);
        if (personagens != null)
        {
            Personagens = personagens;
            _context.Personagens.Remove(Personagens);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
