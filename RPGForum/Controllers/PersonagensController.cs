using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForum.Models;
using RPGForum.Data;

[Route("api/[controller]")]
[ApiController]
public class PersonagensController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PersonagensController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get de todas as Personagens
    /// </summary>
    /// <returns></returns>
    // GET: api/Personagens
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Personagens>>> GetPersonagens()
    {
        return await _context.Personagens.ToListAsync();
    }

    /// <summary>
    /// Get de uma Personagem específica
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // GET: api/Personagens/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Personagens>> GetPersonagens(int id)
    {
        var personagens = await _context.Personagens.FindAsync(id);

        if (personagens == null)
        {
            return NotFound(new { message = "Personagem não encontrada" });
        }

        return personagens;
    }

    /// <summary>
    /// Editar uma Personagem (requer Autorização de Administração)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="personagens"></param>
    /// <returns></returns>
    // PUT: api/Personagens/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> PutPersonagens(int? id, Personagens personagens)
    {
        if (id != personagens.Id)
        {
            return BadRequest(new { message = "Id não corresponde" });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Entry(personagens).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PersonagensExists(id))
            {
                return NotFound(new { message = "Personagem não encontrada" });
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// Criar uma nova Personagem (requer Autorização de Administração)
    /// </summary>
    /// <param name="personagens"></param>
    /// <returns></returns>
    // POST: api/Personagens
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<Personagens>> PostPersonagens(Personagens personagens)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        _context.Personagens.Add(personagens);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPersonagens), new { id = personagens.Id }, personagens);
    }

    /// <summary>
    /// Eliminar uma Personagem (requer Autorização de Administração)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // DELETE: api/Personagens/5
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeletePersonagens(int? id)
    {
        var personagens = await _context.Personagens.FindAsync(id);
        if (personagens == null)
        {
            return NotFound(new { message = "Personagem não encontrada" });
        }

        var buildsCount = await _context.Builds.CountAsync(b => b.CharacterId == id);
        if (buildsCount > 0)
        {
            return BadRequest(new { message = "Não é possível eliminar uma personagem que está associada a builds existentes" });
        }

        _context.Personagens.Remove(personagens);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool PersonagensExists(int? id)
    {
        return _context.Personagens.Any(e => e.Id == id);
    }
}
