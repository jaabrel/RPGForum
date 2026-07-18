using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGForum.Pages.Build
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Utilizadores> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<Models.Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Build Build { get; set; } = new();

        [BindProperty]
        public Estatisticas Stats { get; set; } = new();

        [BindProperty]
        public List<int> ArmasSelecionadas { get; set; } = new();

        [BindProperty]
        public List<int> AcessoriosSelecionados { get; set; } = new();

        public SelectList PersonagensSelect { get; set; } = null!;
        public IList<Armas> TodasArmas { get; set; } = new List<Armas>();
        public IList<Acessorios> TodosAcessorios { get; set; } = new List<Acessorios>();

        public async Task<IActionResult> OnGetAsync()
        {
            await CarregarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Remover validações de navegação que o ModelState não consegue preencher
            ModelState.Remove("Build.User");
            ModelState.Remove("Build.CharClass");
            ModelState.Remove("Build.Stats");
            ModelState.Remove("Stats.Build");

            if (!ModelState.IsValid)
            {
                await CarregarListasAsync();
                return Page();
            }

            var utilizador = await _userManager.GetUserAsync(User);
            if (utilizador == null)
            {
                ModelState.AddModelError("", "Erro ao identificar o utilizador. Por favor tente novamente.");
                await CarregarListasAsync();
                return Page();
            }

            // Associar utilizador e timestamps
            Build.UtilizadorID = utilizador.Id;
            Build.CreatedAt = DateTime.UtcNow;
            Build.UpdatedAt = DateTime.UtcNow;

            // Associar estatísticas diretamente ao objeto Build (necessário porque Stats é obrigatório na relação 1-1)
            Build.Stats = Stats;

            // Inicializar coleções da Build
            Build.BuidWeapons = new List<BuildWeapon>();
            Build.BuildAccessories = new List<BuildAccessory>();

            // Associar armas selecionadas
            foreach (var armaId in ArmasSelecionadas)
            {
                Build.BuidWeapons.Add(new BuildWeapon
                {
                    WeaponId = armaId
                });
            }

            // Associar acessórios selecionados
            for (int i = 0; i < AcessoriosSelecionados.Count; i++)
            {
                Build.BuildAccessories.Add(new BuildAccessory
                {
                    AccessoryId = AcessoriosSelecionados[i],
                    SlotPosition = i + 1
                });
            }

            try
            {
                _context.Builds.Add(Build);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                ModelState.AddModelError("", $"Erro ao gravar a build na base de dados: {message}");
                await CarregarListasAsync();
                return Page();
            }

            TempData["Sucesso"] = $"Build \"{Build.Title}\" criada com sucesso!";
            return RedirectToPage("Index");
        }

        private async Task CarregarListasAsync()
        {
            var personagens = await _context.Personagens.OrderBy(p => p.Name).ToListAsync();
            PersonagensSelect = new SelectList(personagens, "Id", "Name");
            TodasArmas = await _context.Armas.OrderBy(a => a.Name).ToListAsync();
            TodosAcessorios = await _context.Acessorios.OrderBy(a => a.Name).ToListAsync();
        }
    }
}
