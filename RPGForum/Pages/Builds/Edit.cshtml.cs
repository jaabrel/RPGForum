using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Pages.Builds
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public BuildPost Build { get; set; } = null!;

        [BindProperty]
        public Estatisticas Stats { get; set; } = null!;

        [BindProperty]
        public List<int> ArmasSelecionadas { get; set; } = new();

        [BindProperty]
        public List<int> AcessoriosSelecionados { get; set; } = new();

        public SelectList PersonagensSelect { get; set; } = null!;
        public IList<Armas> TodasArmas { get; set; } = new List<Armas>();
        public IList<Acessorios> TodosAcessorios { get; set; } = new List<Acessorios>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var utilizador = await ObterUtilizadorAtualAsync();
            if (utilizador == null) return RedirectToPage("/Index");

            var build = await _context.Builds
                .Include(b => b.Stats)
                .Include(b => b.BuidWeapons)
                .Include(b => b.BuildAccessories)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null) return NotFound();

            // Apenas o autor ou administrador pode editar
            if (build.UtilizadorID != utilizador.Id && !User.IsInRole("Administrator"))
                return Forbid();

            Build = build;
            Stats = build.Stats ?? new Estatisticas { BuildId = id };

            ArmasSelecionadas = build.BuidWeapons.Select(bw => bw.WeaponId).ToList();
            AcessoriosSelecionados = build.BuildAccessories
                .OrderBy(ba => ba.SlotPosition)
                .Select(ba => ba.AccessoryId)
                .ToList();

            await CarregarListasAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            ModelState.Remove("Build.User");
            ModelState.Remove("Build.CharClass");
            ModelState.Remove("Build.Stats");

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

            // Atualizar armas: remover antigas e adicionar novas
            _context.BuildWeapons.RemoveRange(buildExistente.BuidWeapons);
            foreach (var armaId in ArmasSelecionadas)
            {
                _context.BuildWeapons.Add(new BuildWeapon
                {
                    BuildId = id,
                    WeaponId = armaId
                });
            }

            // Atualizar acessórios
            _context.BuildAccessories.RemoveRange(buildExistente.BuildAccessories);
            for (int i = 0; i < AcessoriosSelecionados.Count; i++)
            {
                _context.BuildAccessories.Add(new BuildAccessory
                {
                    BuildId = id,
                    AccessoryId = AcessoriosSelecionados[i],
                    SlotPosition = i + 1
                });
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

        private async Task<Utilizadores?> ObterUtilizadorAtualAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            return await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.IdentityUserName == identityUser.UserName);
        }
    }

}
