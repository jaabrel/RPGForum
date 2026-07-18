using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RPGForum.Data;
using RPGForum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGForum.Pages.Build
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizadores> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Build Build { get; set; } = default!;
        [BindProperty]
        public List<int> SelectedWeapons { get; set; } = new List<int>();
        [BindProperty]
        public List<int> SelectedAccessories { get; set; } = new List<int>();
        public SelectList PersonagensList { get; set; }
        public List<Armas> AvailableWeapons { get; set; }
        public List<Acessorios> AvailableAccessories { get; set; }
        public IActionResult OnGet()
        {
            LoadLists();
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var utilizador = await _userManager.GetUserAsync(User);
            if (utilizador == null)
            {
                return Challenge();
            }
            // Atribuições automáticas
            Build.UtilizadorID = utilizador.Id;
            Build.CreatedAt = DateTime.UtcNow;
            Build.UpdatedAt = DateTime.UtcNow;
            // Ignorar erros de validação das propriedades de navegação e campos automáticos
            ModelState.Remove("Build.Utilizador");
            ModelState.Remove("Build.Character");
            ModelState.Remove("Build.BuildWeapons");
            ModelState.Remove("Build.BuildAccessories");
            ModelState.Remove("Build.UtilizadorID");
            if (!ModelState.IsValid)
            {
                LoadLists();
                return Page();
            }
            _context.Builds.Add(Build);
            await _context.SaveChangesAsync();
            // Guardar as Armas selecionadas
            if (SelectedWeapons != null && SelectedWeapons.Any())
            {
                foreach (var weaponId in SelectedWeapons)
                {
                    _context.BuildWeapons.Add(new BuildWeapon { BuildId = Build.Id, WeaponId = weaponId });
                }
            }
            // Guardar os Acessórios selecionados
            if (SelectedAccessories != null && SelectedAccessories.Any())
            {
                foreach (var accessoryId in SelectedAccessories)
                {
                    _context.BuildAccessories.Add(new BuildAccessory { BuildId = Build.Id, AccessoryId = accessoryId });
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
        private void LoadLists()
        {
            PersonagensList = new SelectList(_context.Personagens, "Id", "Name");
            AvailableWeapons = _context.Armas.ToList();
            AvailableAccessories = _context.Acessorios.ToList();
        }
    }
}
