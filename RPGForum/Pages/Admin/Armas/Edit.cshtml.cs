using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Armas
{
    [Authorize(Roles = "Administrator")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Armas Arma { get; set; } = default!;

        [BindProperty]
        public Dictionary<string, int> SelectedStats { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arma = await _context.Armas.FirstOrDefaultAsync(m => m.Id == id);
            if (arma == null)
            {
                return NotFound();
            }
            Arma = arma;

            // Preencher dicionário a partir das strings de bónus existentes
            if (!string.IsNullOrEmpty(Arma.StatAfetada) && !string.IsNullOrEmpty(Arma.StatBonus))
            {
                var stats = Arma.StatAfetada.Split(',');
                var bonuses = Arma.StatBonus.Split(',');
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

            Arma.StatAfetada = affectedList.Any() ? string.Join(",", affectedList) : null;
            Arma.StatBonus = bonusList.Any() ? string.Join(",", bonusList) : null;

            _context.Attach(Arma).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArmaExists(Arma.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["Sucesso"] = $"Arma \"{Arma.Name}\" atualizada com sucesso!";
            return RedirectToPage("./Index");
        }

        private bool ArmaExists(int id)
        {
            return _context.Armas.Any(e => e.Id == id);
        }
    }
}
