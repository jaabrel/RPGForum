using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;

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
    public async Task<ActionResult<IEnumerable<Build>>> GetBuild()
    {
        return await _context.Builds.ToListAsync();
    }

    /// <summary>
    /// Get de uma Build específica.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // GET: api/Build/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Build>> GetBuild(int id)
    {
        var build = await _context.Builds.FindAsync(id);

        if (build == null)
        {
            return NotFound(new { message = "Build não encontrada" });
        }

        return build;
    }

    // PUT: api/Build/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBuild(int? id, Build build)
    {
        if (id != build.Id)
        {
            return BadRequest();
        }

        _context.Entry(build).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BuildExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    public class BuildCreateDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public int CharacterId { get; set; }
        public List<int> WeaponsIds { get; set; } = new List<int>();
        public List<int> AccessoryIds { get; set; } = new List<int>();
    }

    /// <summary>
    /// Criar uma Build.
    /// </summary>
    /// <param name="build"></param>
    /// <returns></returns>
    // POST: api/Build
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Build>> PostBuild(BuildCreateDto dto)
    {
        var utilizador = await _userManager.GetUserAsync(User);
        if (utilizador == null) return Unauthorized();

        var build = new Build
        {
            Title = dto.Title,
            Description = dto.Description,
            Level = dto.Level,
            CharacterId = dto.CharacterId,
            UtilizadorID = utilizador.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Builds.Add(build);
        await _context.SaveChangesAsync();

        foreach (var wId in dto.WeaponsIds)
        {
            _context.BuildWeapons.Add(new BuildWeapon
            {
                BuildId = build.Id,
                WeaponId = wId
            });
        }

        foreach (var aId in dto.AccessoryIds)
        {
            _context.BuildAccessories.Add(new BuildAccessory { BuildId = build.Id, AccessoryId = aId });
        }

        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBuild), new { id = build.Id }, build);
    }

    /// <summary>
    /// Eliminar uma build (requer Autenticação e/ou Autorização de Administrador).
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // DELETE: api/Build/5
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteBuild(int? id)
    {
        var utilizador = await _userManager.GetUserAsync(User);

        if (utilizador == null)
        {
            return Unauthorized(new { message = "Utilizador não registado ou sem permissões" });
        }

        var build = await _context.Builds.FindAsync(id);
        if (build == null)
        {
            return NotFound(new { message = "Build não encontrada" });
        }

        var isAdmin = User.IsInRole("Administrator");

        if (build.UtilizadorID != utilizador.Id || isAdmin) 
        {
            return Forbid();
        }

        _context.Builds.Remove(build);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BuildExists(int? id)
    {
        return _context.Builds.Any(e => e.Id == id);
    }
}
