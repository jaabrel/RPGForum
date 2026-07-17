using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Utilizadores> _userManager;

        public BuildsController(ApplicationDbContext context, UserManager<Utilizadores> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Obter todas as builds
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetBuilds()
        {
            var builds = await _context.Builds
                .Include(b => b.User)
                .Include(b => b.CharClass)
                .Select(b => new
                {
                    b.Id,
                    b.Title,
                    b.Description,
                    b.Level,
                    b.CreatedAt,
                    b.UpdatedAt,
                    Autor = b.User.UserName,
                    Classe = b.CharClass.Name
                })
                .ToListAsync();

            return Ok(builds);
        }

        /// <summary>
        /// Obter os detalhes de uma build específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBuild(int id)
        {
            var build = await _context.Builds
                .Include(b => b.User)
                .Include(b => b.CharClass)
                .Include(b => b.Stats)
                .Include(b => b.BuidWeapons)
                    .ThenInclude(bw => bw.Weapon)
                .Include(b => b.BuildAccessories)
                    .ThenInclude(ba => ba.Accessory)
                .Include(b => b.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (build == null)
            {
                return NotFound(new { message = "Build não encontrada" });
            }

            var result = new
            {
                build.Id,
                build.Title,
                build.Description,
                build.Level,
                build.CreatedAt,
                build.UpdatedAt,
                Autor = build.User.UserName,
                Classe = build.CharClass.Name,
                Estatisticas = build.Stats != null ? new
                {
                    build.Stats.Hp,
                    build.Stats.Strength,
                    build.Stats.Defense,
                    build.Stats.Magic,
                    build.Stats.Endurance,
                    build.Stats.Speed
                } : null,
                Armas = build.BuidWeapons.Select(bw => new
                {
                    bw.Weapon.Id,
                    bw.Weapon.Name,
                    bw.Weapon.Type,
                    bw.Weapon.Description
                }),
                Acessorios = build.BuildAccessories.Select(ba => new
                {
                    ba.Accessory.Id,
                    ba.Accessory.Name,
                    ba.Accessory.Type,
                    ba.Accessory.Description,
                    ba.SlotPosition
                }),
                Comentarios = build.Comments.Select(c => new
                {
                    c.Id,
                    Autor = c.User.UserName,
                    c.Content,
                    c.CreatedAt,
                    c.ParentId
                })
            };

            return Ok(result);
        }

        /// <summary>
        /// Eliminar uma build (requer autenticação)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBuild(int id)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null)
            {
                return Challenge();
            }

            var utilizador = identityUser;

            if (utilizador == null)
            {
                return Unauthorized(new { message = "Utilizador do fórum não registado." });
            }

            var build = await _context.Builds.FindAsync(id);
            if (build == null)
            {
                return NotFound(new { message = "Build não encontrada" });
            }

            // Apenas o autor ou um administrador pode apagar
            var isAdmin = User.IsInRole("Administrator");
            if (build.UtilizadorID != utilizador.Id && !isAdmin)
            {
                return Forbid();
            }

            _context.Builds.Remove(build);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
