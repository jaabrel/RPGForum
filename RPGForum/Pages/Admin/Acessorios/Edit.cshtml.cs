using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Acessorios;

[Authorize(Roles = "Administrator")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
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
        Acessorio = acessorio;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Attach(Acessorio).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AcessorioExists(Acessorio.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        TempData["Sucesso"] = $"Acessório \"{Acessorio.Name}\" atualizado com sucesso!";
        return RedirectToPage("./Index");
    }

    private bool AcessorioExists(int id)
    {
        return _context.Acessorios.Any(e => e.Id == id);
    }
}