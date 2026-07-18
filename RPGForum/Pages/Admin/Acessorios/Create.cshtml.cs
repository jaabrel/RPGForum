using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RPGForum.Models;
using RPGForum.Data;

namespace RPGForum.Pages.Admin.Acessorios;

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
        public Models.Acessorios Acessorio { get; set; } = default!;

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

            Acessorio.StatAfetada = affectedList.Any() ? string.Join(",", affectedList) : null;
            Acessorio.StatBonus = bonusList.Any() ? string.Join(",", bonusList) : null;

            _context.Acessorios.Add(Acessorio);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Acessório \"{Acessorio.Name}\" criado com sucesso!";
            return RedirectToPage("./Index");
        }
    }
    