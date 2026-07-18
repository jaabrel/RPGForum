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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.Utilizadores> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<Models.Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public Models.Build Build { get; set; } = null!;

        [BindProperty]
        public Estatisticas Stats { get; set; } = null!;

        [BindProperty]
        public string? ArmasNomes { get; set; }

        [BindProperty]
        public string? AcessoriosNomes { get; set; }

        public SelectList PersonagensSelect { get; set; } = null!;
        public IList<Armas> TodasArmas { get; set; } = new List<Armas>();
        public IList<Acessorios> TodosAcessorios { get; set; } = new List<Acessorios>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Index");

            var build = await _context.Builds
                .Include(b => b.Stats)
                .Include(b => b.BuidWeapons).ThenInclude(bw => bw.Weapon)
                .Include(b => b.BuildAccessories).ThenInclude(ba => ba.Accessory)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null) return NotFound();

            // Apenas o autor ou administrador pode editar
            if (build.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            Build = build;
            Stats = build.Stats ?? new Estatisticas { BuildId = id };

            ArmasNomes = string.Join(", ", build.BuidWeapons.Select(bw => bw.Weapon?.Name).Where(name => name != null));
            AcessoriosNomes = string.Join(", ", build.BuildAccessories.OrderBy(ba => ba.SlotPosition).Select(ba => ba.Accessory?.Name).Where(name => name != null));

            await CarregarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ModelState.Remove("Build.User");
            ModelState.Remove("Build.CharClass");
            ModelState.Remove("Build.Stats");
            ModelState.Remove("Stats.Build");

            if (!ModelState.IsValid)
            {
                await CarregarListasAsync();
                return Page();
            }

            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Index");

            var buildExistente = await _context.Builds
                .Include(b => b.Stats)
                .Include(b => b.BuidWeapons)
                .Include(b => b.BuildAccessories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (buildExistente == null) return NotFound();

            if (buildExistente.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            // Atualizar campos da build
            buildExistente.Title = Build.Title;
            buildExistente.Description = Build.Description;
            buildExistente.CharacterId = Build.CharacterId;
            buildExistente.Level = Build.Level;
            buildExistente.UpdatedAt = DateTime.UtcNow;

            // Atualizar estatísticas
            if (buildExistente.Stats == null)
            {
                Stats.BuildId = id;
                _context.Estatisticas.Add(Stats);
            }
            else
            {
                buildExistente.Stats.Hp = Stats.Hp;
                buildExistente.Stats.Strength = Stats.Strength;
                buildExistente.Stats.Defense = Stats.Defense;
                buildExistente.Stats.Magic = Stats.Magic;
                buildExistente.Stats.Endurance = Stats.Endurance;
                buildExistente.Stats.Speed = Stats.Speed;
            }

            // Parse e associar armas
            var armasNomesLista = string.IsNullOrWhiteSpace(ArmasNomes)
                ? new List<string>()
                : ArmasNomes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            var armasParaAdicionar = new List<BuildWeapon>();
            foreach (var nomeArma in armasNomesLista)
            {
                var arma = await _context.Armas.FirstOrDefaultAsync(a => a.Name.ToLower() == nomeArma.ToLower());
                if (arma == null)
                {
                    ModelState.AddModelError("ArmasNomes", $"A arma '{nomeArma}' não existe no sistema.");
                }
                else
                {
                    armasParaAdicionar.Add(new BuildWeapon { BuildId = id, WeaponId = arma.Id });
                }
            }

            // Parse e associar acessórios
            var acessoriosNomesLista = string.IsNullOrWhiteSpace(AcessoriosNomes)
                ? new List<string>()
                : AcessoriosNomes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

            var acessoriosParaAdicionar = new List<BuildAccessory>();
            for (int i = 0; i < acessoriosNomesLista.Count; i++)
            {
                var nomeAcessorio = acessoriosNomesLista[i];
                var acessorio = await _context.Acessorios.FirstOrDefaultAsync(a => a.Name.ToLower() == nomeAcessorio.ToLower());
                if (acessorio == null)
                {
                    ModelState.AddModelError("AcessoriosNomes", $"O acessório '{nomeAcessorio}' não existe no sistema.");
                }
                else
                {
                    acessoriosParaAdicionar.Add(new BuildAccessory
                    {
                        BuildId = id,
                        AccessoryId = acessorio.Id,
                        SlotPosition = i + 1
                    });
                }
            }

            if (!ModelState.IsValid)
            {
                await CarregarListasAsync();
                Build = buildExistente;
                return Page();
            }

            // Atualizar armas: remover antigas e adicionar novas
            _context.BuildWeapons.RemoveRange(buildExistente.BuidWeapons);
            foreach (var bw in armasParaAdicionar)
            {
                _context.BuildWeapons.Add(bw);
            }

            // Atualizar acessórios
            _context.BuildAccessories.RemoveRange(buildExistente.BuildAccessories);
            foreach (var ba in acessoriosParaAdicionar)
            {
                _context.BuildAccessories.Add(ba);
            }

            await _context.SaveChangesAsync();

            TempData["Sucesso"] = $"Build \"{buildExistente.Title}\" atualizada com sucesso!";
            return RedirectToPage("Index");
        }

        private async Task CarregarListasAsync()
        {
            var personagens = await _context.Personagens.OrderBy(p => p.Name).ToListAsync();
            PersonagensSelect = new SelectList(personagens, "Id", "Name");
            TodasArmas = await _context.Armas.OrderBy(a => a.Name).ToListAsync();
            TodosAcessorios = await _context.Acessorios.OrderBy(a => a.Name).ToListAsync();
        }

        private async Task<Models.Utilizadores?> ObterUtilizadorAtualAsync()
        {
            return await _userManager.GetUserAsync(User);
        }
    }
}
