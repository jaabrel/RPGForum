using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPGForum.Data;
using RPGForum.Models;

namespace RPGForum.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonagensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PersonagensController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obter todas as personagens / classes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Personagens>>> GetPersonagens()
        {
            return await _context.Personagens.ToListAsync();
        }

        /// <summary>
        /// Obter uma personagem / classe específica
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Personagens>> GetPersonagem(int id)
        {
            var personagem = await _context.Personagens.FindAsync(id);

            if (personagem == null)
            {
                return NotFound(new { message = "Personagem não encontrada" });
            }

            return personagem;
        }

        /// <summary>
        /// Criar uma nova personagem / classe (requer autorização de Administrador)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Personagens>> PostPersonagem(Personagens personagem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Personagens.Add(personagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPersonagem), new { id = personagem.Id }, personagem);
        }

        /// <summary>
        /// Atualizar uma personagem / classe (requer autorização de Administrador)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutPersonagem(int id, Personagens personagem)
        {
            if (id != personagem.Id)
            {
                return BadRequest(new { message = "ID não corresponde" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(personagem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonagemExists(id))
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
        /// Eliminar uma personagem / classe (requer autorização de Administrador)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeletePersonagem(int id)
        {
            var personagem = await _context.Personagens.FindAsync(id);
            if (personagem == null)
            {
                return NotFound(new { message = "Personagem não encontrada" });
            }

            // Verificar se existem builds associadas
            var buildsCount = await _context.Builds.CountAsync(b => b.CharacterId == id);
            if (buildsCount > 0)
            {
                return BadRequest(new { message = "Não é possível eliminar uma personagem que possui builds associadas" });
            }

            _context.Personagens.Remove(personagem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonagemExists(int id)
        {
            return _context.Personagens.Any(e => e.Id == id);
        }
    }
}
