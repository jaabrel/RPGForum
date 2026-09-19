using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;
using System.ComponentModel.DataAnnotations;
using RPGForum.Models.DTOs;

[Route("api/[controller]")]
[ApiController]
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
    /// Get de todas as Builds.
    /// </summary>
    /// <returns></returns>
    // GET: api/Build
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Build>>> GetBuilds()
    {
        return await _context.Builds
            .Include(b => b.CharClass)
            .Include(b => b.User)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Obtém os detalhes de uma build específica, incluindo personagem, armas e acessórios.
    /// </summary>
    // GET: api/Build/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Build>> GetBuild(int id)
    {
        var build = await _context.Builds
            .Include(b => b.CharClass)
            .Include(b => b.User)
            .Include(b => b.BuidWeapons).ThenInclude(bw => bw.Weapon)
            .Include(b => b.BuidAccessories).ThenInclude(ba => ba.Accessory)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (build == null)
        {
            return NotFound(new { message = "Build não encontrada" });
        }

        return build;
    }



    /// <summary>
    /// Atualiza uma build existente (Apenas o Criador ou Administrador).
    /// </summary>
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> PutBuild(int id, [FromBody] BuildDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var utilizador = await _userManager.GetUserAsync(User);
        if (utilizador == null)
        {
            return Unauthorized(new { message = "Utilizador não autenticado." });
        }

        var build = await _context.Builds
                .Include(b => b.BuidWeapons)
                .Include(b => b.BuildAccessories)
                .FirstOrDefaultAsync(b => b.Id == id);

        if (build == null)
        {
            return NotFound(new { message = "Build não encontrada." });
        }

        var isAdmin = User.IsInRole("Administrator");
        if (build.UtilizadorID != utilizador.Id && !isAdmin)
        {
            return Forbid();
        }

        if (!await _context.Personagens.AnyAsync(p => p.Id == dto.CharacterId))
        {
            return BadRequest(new { message = "A personagem especificada não existe." });
        }

        build.Title = dto.Title;
        build.Description = dto.Description;
        build.Level = dto.Level;
        build.CharacterId = dto.CharacterId;
        build.UpdatedAt = DateTime.UtcNow;

        _context.BuildWeapons.RemoveRange(build.BuidWeapons);
        foreach (var wId in dto.WeaponsIds.Distinct())
        {
            if (await _context.Armas.AnyAsync(a => a.Id == wId))
            {
                _context.BuildWeapons.Add(new BuildWeapon { BuildId = id, WeaponId = wId });
            }
        }

        _context.BuildAccessories.RemoveRange(build.BuildAccessories);
        int slot = 1;
        foreach (var aId in dto.AccessoryIds.Distinct())
        {
            if (await _context.Acessorios.AnyAsync(a => a.Id == aId))
            {
                _context.BuildAccessories.Add(new BuildAccessory { BuildId = id, AccessoryId = aId, SlotPosition = slot++ });
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }


    /// <summary>
    /// Cria uma nova build associando personagem, armas e acessórios.
    /// </summary>
    // POST: api/Builds
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Build>> PostBuild([FromBody] BuildDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var utilizador = await _userManager.GetUserAsync(User);
        if (utilizador == null)
        {
            return Unauthorized(new { message = "Utilizador não autenticado." });
        }

        if (!await _context.Personagens.AnyAsync(p => p.Id == dto.CharacterId))
        {
            return BadRequest(new { message = "A personagem especificada não existe." });
        }

        var build = new Build
        {
            Title = dto.Title,
            Description = dto.Description,
            Level = dto.Level,
            CharacterId = dto.CharacterId,
            UtilizadorID = utilizador.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Builds.Add(build);
        await _context.SaveChangesAsync();

        foreach (var wId in dto.WeaponsIds.Distinct())
        {
            if (await _context.Armas.AnyAsync(a => a.Id == wId))
            {
                _context.BuildWeapons.Add(new BuildWeapon { BuildId = build.Id, WeaponId = wId });
            }
        }

        int slot = 1;
        foreach (var aId in dto.AccessoryIds.Distinct())
        {
            if (await _context.Acessorios.AnyAsync(a => a.Id == aId))
            {
                _context.BuildAccessories.Add(new BuildAccessory { BuildId = build.Id, AccessoryId = aId, SlotPosition = slot++ });
            }
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBuild), new { id = build.Id }, build);
    }

    /// <summary>
    /// Elimina uma build (Apenas o Criador ou Administrador).
    /// </summary>
    // DELETE: api/Builds/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBuild(int id)
    {
        var utilizador = await _userManager.GetUserAsync(User);
        
        if (utilizador == null)
        {
            return Unauthorized(new { message = "Utilizador não autenticado." });
        }
        
        var build = await _context.Builds.FindAsync(id);
        
        if (build == null)
        {
            return NotFound(new { message = "Build não encontrada" });
        }

        var isAdmin = User.IsInRole("Administrator");
        if (build.UtilizadorID != utilizador.Id && !isAdmin)
        {
            return Forbid();
        }

        _context.Builds.Remove(build);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Objeto padrão para a entrada de dados da Build via a API (Create e Update).
    /// </summary>
    public class BuildDto
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(2500)]
        public string? Description { get; set; }
        [Range(1, 100)]
        public int Level { get; set; } = 1;
        [Required]
        public int CharacterId { get; set; }
        public List<int> WeaponsIds { get; set; } = new();
        public List<int> AccessoryIds { get; set; } = new();
    }
}
