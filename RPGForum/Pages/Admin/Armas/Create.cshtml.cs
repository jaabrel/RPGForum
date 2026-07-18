using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Armas
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Models.Armas Arma { get; set; } = default!;

        [BindProperty]
        public Dictionary<string, int> SelectedStats { get; set; } = new();

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

            _context.Armas.Add(Arma);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Arma \"{Arma.Name}\" criada com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}
