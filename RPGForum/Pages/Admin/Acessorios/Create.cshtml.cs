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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Acessorios.Add(Acessorio);
            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Acessório \"{Acessorio.Name}\" criado com sucesso!";
            return RedirectToPage("./Index");
        }
    }
    