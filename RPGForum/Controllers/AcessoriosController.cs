using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AcessoriosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AcessoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get de todos os Acessórios.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Acessorios>>> GetAcessorios()
        {
            return await _context.Acessorios.ToListAsync();
        }

        /// <summary>
        /// Get de um Acessório pelo ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Acessorios>> GetAcessorios(int id)
        {
            var acessorios = await _context.Acessorios.FindAsync(id);

            if (acessorios == null)
            {
                return NotFound();
            }

            return acessorios;
        }

        /// <summary>
        /// Criar um novo Acessório. (Requer autorização de Administrador)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Acessorios>> PostAcessorio(Acessorios acessorio)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Acessorios.Add(acessorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAcessorio", new { id = acessorio.Id }, acessorio);
        }

        /// <summary>
        ///  Atualizar um acessório (Requer autorização de Administrador)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutAcessorio(int id, Acessorios acessorio)
        {
            if (id != acessorio.Id)
            {
                return BadRequest(new { message = "ID não corresponde" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(acessorio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AcessorioExists(id))
                {
                    return NotFound(new { message = "Acessório não encontrado" });
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        /// <summary>
        /// Eliminar um acessório (Requer autorização de Administrador)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAcessorio(int id)
        {
            var acessorio = await _context.Acessorios.FindAsync(id);
            if (acessorio == null)
            {
                return NotFound(new { message = "Acessório não encontrado" });
            }

            var buildAcessoriesCount = await _context.BuildAccessories.CountAsync(ba => ba.AccessoryId == id);
            if (buildAcessoriesCount > 0)
            {
                return BadRequest(new { message = "Não é possível eliminar um acessório que tem builds associadas" });
            }

            _context.Acessorios.Remove(acessorio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Verifica se um acessório existe pelo ID.
        /// </summary>
        private bool AcessorioExists(int id)
        {
            return _context.Acessorios.Any(e => e.Id == id);
        }
    }
}
