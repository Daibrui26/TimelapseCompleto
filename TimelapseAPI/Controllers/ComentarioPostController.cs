using Microsoft.AspNetCore.Mvc;
using TimelapseAPI.Models;
using TimelapseAPI.Services;

namespace TimelapseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentarioPostController : ControllerBase
    {
        private readonly IComentarioPostService _service;

        public ComentarioPostController(IComentarioPostService service)
        {
            _service = service;
        }

        // GET api/ComentarioPost/post/{idPost}
        [HttpGet("post/{idPost:int}")]
        public async Task<ActionResult<List<ComentarioPost>>> GetByPost(int idPost)
        {
            var lista = await _service.GetByPostAsync(idPost);
            return Ok(lista);
        }

        // POST api/ComentarioPost
        [HttpPost]
        public async Task<ActionResult<ComentarioPost>> Create([FromBody] ComentarioPost comentario)
        {
            try
            {
                var nuevo = await _service.CreateAsync(comentario);
                return Ok(nuevo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // DELETE api/ComentarioPost/{id}?idUsuario=X&esAdmin=true/false
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int idUsuario, [FromQuery] bool esAdmin = false)
        {
            try
            {
                var result = await _service.DeleteAsync(id, idUsuario, esAdmin);
                if (!result) return NotFound(new { mensaje = "Comentario no encontrado." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}