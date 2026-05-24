using Microsoft.AspNetCore.Mvc;
using TimelapseAPI.Models;
using TimelapseAPI.Models.DTOs;
using TimelapseAPI.Services;

namespace TimelapseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        // GET api/Post  → todos los públicos
        [HttpGet]
        public async Task<ActionResult<List<Post>>> GetAll()
        {
            var posts = await _postService.GetAllPublicosAsync();
            return Ok(posts);
        }

        // GET api/Post/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Post>> GetById(int id)
        {
            var post = await _postService.GetByIdAsync(id);
            if (post == null) return NotFound();
            return Ok(post);
        }

        // GET api/Post/usuario/{idUsuario}  → todos los posts del usuario (propios)
        [HttpGet("usuario/{idUsuario:int}")]
        public async Task<ActionResult<List<Post>>> GetByUsuario(int idUsuario)
        {
            var posts = await _postService.GetByUsuarioAsync(idUsuario);
            return Ok(posts);
        }

        // POST api/Post  → multipart/form-data
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<Post>> Create([FromForm] PostArchivoCreateDTO dto)
        {
            try
            {
                var nuevo = await _postService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = nuevo.IdPost }, nuevo);
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

        // DELETE api/Post/{id}?idUsuario=X&esAdmin=true/false
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] int idUsuario, [FromQuery] bool esAdmin = false)
        {
            try
            {
                var result = await _postService.DeleteAsync(id, idUsuario, esAdmin);
                if (!result) return NotFound(new { mensaje = "Post no encontrado." });
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