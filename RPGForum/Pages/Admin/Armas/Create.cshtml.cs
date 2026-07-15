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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Armas.Add(Arma);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Arma \"{Arma.Name}\" criada com sucesso!";
            return RedirectToPage("./Index");
        }
    }
}
