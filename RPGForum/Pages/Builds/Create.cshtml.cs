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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public BuildPost Build { get; set; } = new();

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

            if (!ModelState.IsValid)
            {
                await CarregarListasAsync();
                return Page();
            }

            var utilizador = await ObterOuCriarUtilizadorAsync();
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

            _context.Builds.Add(Build);
            await _context.SaveChangesAsync();

            // Adicionar estatísticas
            Stats.BuildId = Build.Id;
            _context.Estatisticas.Add(Stats);

            // Associar armas (muitos-para-muitos)
            foreach (var armaId in ArmasSelecionadas)
            {
                _context.BuildWeapons.Add(new BuildWeapon
                {
                    BuildId = Build.Id,
                    WeaponId = armaId
                });
            }

            // Associar acessórios (muitos-para-muitos)
            for (int i = 0; i < AcessoriosSelecionados.Count; i++)
            {
                _context.BuildAccessories.Add(new BuildAccessory
                {
                    BuildId = Build.Id,
                    AccessoryId = AcessoriosSelecionados[i],
                    SlotPosition = i + 1
                });
            }

            await _context.SaveChangesAsync();

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

        private async Task<Utilizadores?> ObterOuCriarUtilizadorAsync()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null) return null;

            var utilizador = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.UserId == identityUser.UserName);

            if (utilizador == null)
            {
                utilizador = new Utilizadores
                {
                    Username = identityUser.UserName!.Split('@')[0],
                    Email = identityUser.Email!,
                    Password = "",
                    UserId = identityUser.UserName,
                    Role = "Registered"
                };
                _context.Utilizadores.Add(utilizador);
                await _context.SaveChangesAsync();
            }

            return utilizador;
        }
    }

}
