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

    [BindProperty]
    public Dictionary<string, int> SelectedStats { get; set; } = new();

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

        // Preencher dicionário a partir das strings de bónus existentes
        if (!string.IsNullOrEmpty(Acessorio.StatAfetada) && !string.IsNullOrEmpty(Acessorio.StatBonus))
        {
            var stats = Acessorio.StatAfetada.Split(',');
            var bonuses = Acessorio.StatBonus.Split(',');
            for (int i = 0; i < Math.Min(stats.Length, bonuses.Length); i++)
            {
                if (int.TryParse(bonuses[i], out int bVal))
                {
                    SelectedStats[stats[i]] = bVal;
                }
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var affectedList = new List<string>();
        var bonusList = new List<string>();

        if (SelectedStats != null)
        {
            foreach (var kvp in SelectedStats)
            {
                if (kvp.Value != 0)
                {
                    affectedList.Add(kvp.Key);
                    bonusList.Add(kvp.Value.ToString());
                }
            }
        }

        Acessorio.StatAfetada = affectedList.Any() ? string.Join(",", affectedList) : null;
        Acessorio.StatBonus = bonusList.Any() ? string.Join(",", bonusList) : null;

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