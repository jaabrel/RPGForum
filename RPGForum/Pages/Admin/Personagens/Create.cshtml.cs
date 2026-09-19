using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using RPGForum.Data;
using RPGForum.Models;
using RPGForum.Models.ViewModels;
using System.ComponentModel;

namespace RPGForum.Pages.Admin.Personagens
{
    [Authorize(Roles = "Administrator")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CreateModel(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public PersonagemCreateViewModel PersonagemVM { get; set; } = new();

        public void OnGet()
        {
        }

        [BindProperty]
        public Models.Personagens Personagem { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string nomeFicheiro = "default.png";

            if (PersonagemVM.ImagemUpload != null)
            {
                string pastaDestino = Path.Combine(_hostEnvironment.WebRootPath, "images", "uploads");

                Directory.CreateDirectory(pastaDestino);

                nomeFicheiro = Guid.NewGuid().ToString() + Path.GetExtension(PersonagemVM.ImagemUpload.FileName);

                string caminhoFinal = Path.Combine(pastaDestino, nomeFicheiro);

                using (var stream = new FileStream(caminhoFinal, FileMode.Create))
                {
                    await PersonagemVM.ImagemUpload.CopyToAsync(stream);
                }
            }

            var novaPersonagem = new Models.Personagens
            {
                Name = PersonagemVM.Name,
                Description = PersonagemVM.Description,
                ImageUrl = "/images/uploads/" + nomeFicheiro
            };

            _context.Personagens.Add(Personagem);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
