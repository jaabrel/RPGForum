using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;

[Route("api/[controller]")]
[ApiController]
public class ArmasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ArmasController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get de todas as armas
    /// </summary>
    /// <returns></returns>
    // GET: api/Armas
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Armas>>> GetArmas()
    {
        return await _context.Armas.ToListAsync();
    }

    /// <summary>
    /// Get de uma arma específica pelo ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // GET: api/Armas/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Armas>> GetArmas(int id)
    {
        var armas = await _context.Armas.FindAsync(id);

        if (armas == null)
        {
            return NotFound();
        }

        return armas;
    }

    /// <summary>
    /// Editar uma arma existente (requer autorização de administrador)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="armas"></param>
    /// <returns></returns>
    // PUT: api/Armas/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PutArmas(int? id, Armas armas)
    {
        if (id != armas.Id)
        {
            return BadRequest(new { message = "ID não corresponde"});
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(armas).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ArmasExists(id))
            {
                return NotFound(new { message = "Arma não encontrada" });
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Criar uma nova arma (requer autorização de administrador)
    /// </summary>
    /// <param name="armas"></param>
    /// <returns></returns>

    // POST: api/Armas
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Armas>> PostArmas(Armas armas)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Armas.Add(armas);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetArmas), new { id = armas.Id }, armas);
    }

    /// <summary>
    /// Eliminar uma arma existente (requer autorização de administrador)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // DELETE: api/Armas/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteArmas(int? id)
    {
        var armas = await _context.Armas.FindAsync(id);
        if (armas == null)
        {
            return NotFound(new { message = "Arma não encontrada"});
        }

        var buildWeaponsCount = await _context.BuildWeapons.CountAsync(bw => bw.WeaponId == id);
        if (buildWeaponsCount > 0)
        {
            return BadRequest(new { message = "Não é possível eliminar uma arma que tem builds associadas" });
        }

        _context.Armas.Remove(armas);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ArmasExists(int? id)
    {
        return _context.Armas.Any(e => e.Id == id);
    }
}
