using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;

[Route("api/[controller]")]
[ApiController]
public class AcessoriosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public AcessoriosController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get de todos os Acessórios existentes.
    /// </summary>
    /// <returns></returns>
    // GET: api/Acessorios
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Acessorios>>> GetAcessorios()
    {
        return await _context.Acessorios.ToListAsync();
    }

    /// <summary>
    /// Get de um Acessório específico pelo Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // GET: api/Acessorios/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Acessorios>> GetAcessorios(int id)
    {
        var acessorios = await _context.Acessorios.FindAsync(id);

        if (acessorios == null)
        {
            return NotFound(new { message = "Acessório não encontrado"});
        }

        return acessorios;
    }

    /// <summary>
    /// Editar um Acessório (Requer autorização de Administrador).
    /// </summary>
    /// <param name="id"></param>
    /// <param name="acessorios"></param>
    /// <returns></returns>
    // PUT: api/Acessorios/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PutAcessorios(int? id, Acessorios acessorios)
    {
        if (id != acessorios.Id)
        {
            return BadRequest(new { message = "Id não corresponde"});
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(acessorios).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AcessoriosExists(id))
            {
                return NotFound(new { message = "Acessório não encontrado"});
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Criar um novo Acessório (Requer autorização de Administrador).
    /// </summary>
    /// <param name="acessorios"></param>
    /// <returns></returns>
    // POST: api/Acessorios
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<Acessorios>> PostAcessorios(Acessorios acessorios)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Acessorios.Add(acessorios);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetAcessorios", new { id = acessorios.Id }, acessorios);
    }

    /// <summary>
    /// Eliminar um acessório (Requer autorização de Administrador).
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // DELETE: api/Acessorios/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteAcessorios(int? id)
    {
        var acessorios = await _context.Acessorios.FindAsync(id);
        if (acessorios == null)
        {
            return NotFound(new { message = "Acessório não encontrado" });
        }

        var buildAcessoriesCount = await _context.BuildAccessories.CountAsync(ba => ba.AccessoryId == id);
        if (buildAcessoriesCount > 0)
        {
            return BadRequest(new { message = "Não é possível eliminar um acessório com builds associadas" });
        }

        _context.Acessorios.Remove(acessorios);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Verifica se um acessório existe.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool AcessoriosExists(int? id)
    {
        return _context.Acessorios.Any(e => e.Id == id);
    }
}
