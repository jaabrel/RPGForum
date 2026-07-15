using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForumApi.Data;
using RPGForumApi.Models;

namespace RPGForumApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArmasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArmasController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todas as armas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Armas>>> GetArmas()
        {
            return await _context.Armas.ToListAsync();
        }

        /// <summary>
        /// Obter uma arma específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Armas>> GetArma(int id)
        {
            var arma = await _context.Armas.FindAsync(id);

            if (arma == null)
            {
                return NotFound(new { message = "Arma não encontrada" });
            }

            return arma;
        }

        /// <summary>
        /// Criar uma nova arma (requer autorização de Administrador)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Armas>> PostArma(Armas arma)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Armas.Add(arma);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetArma), new { id = arma.Id }, arma);
        }

        /// <summary>
        /// Atualizar uma arma (requer autorização de Administrador)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutArma(int id, Armas arma)
        {
            if (id != arma.Id)
            {
                return BadRequest(new { message = "ID não corresponde" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(arma).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArmaExists(id))
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
        /// Eliminar uma arma (requer autorização de Administrador)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteArma(int id)
        {
            var arma = await _context.Armas.FindAsync(id);
            if (arma == null)
            {
                return NotFound(new { message = "Arma não encontrada" });
            }

            // Verificar se existem builds associados
            var buildWeaponsCount = await _context.BuildWeapons.CountAsync(bw => bw.WeaponId == id);
            if (buildWeaponsCount > 0)
            {
                return BadRequest(new { message = "Não é possível eliminar uma arma que tem builds associadas" });
            }

            _context.Armas.Remove(arma);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArmaExists(int id)
        {
            return _context.Armas.Any(e => e.Id == id);
        }
    }
}
