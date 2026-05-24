using TimelapseAPI.Models;
using TimelapseAPI.Models.DTOs;
using TimelapseAPI.Repositories;

namespace TimelapseAPI.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _repo;
        private readonly IUploadService _uploadService;

        public PostService(IPostRepository repo, IUploadService uploadService)
        {
            _repo          = repo;
            _uploadService = uploadService;
        }

        public Task<List<Post>> GetAllPublicosAsync() => _repo.GetAllPublicosAsync();
        public Task<List<Post>> GetByUsuarioAsync(int idUsuario) => _repo.GetByUsuarioAsync(idUsuario);
        public Task<Post?> GetByIdAsync(int id) => _repo.GetByIdAsync(id);

        public async Task<Post> CreateAsync(PostArchivoCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Texto))
                throw new ArgumentException("El texto del post no puede estar vacío.");

            if (dto.IdUsuario <= 0)
                throw new ArgumentException("El IdUsuario debe ser válido.");

            var visibilidadesValidas = new[] { "publica", "privada" };
            if (!visibilidadesValidas.Contains(dto.Visibilidad.ToLower()))
                throw new ArgumentException("La visibilidad debe ser 'publica' o 'privada'.");

            string? urlArchivo = null;
            if (dto.Archivo != null && dto.Archivo.Length > 0)
                urlArchivo = await _uploadService.UploadAsync(dto.Archivo);

            var post = new Post
            {
                Texto            = dto.Texto.Trim(),
                UrlArchivo       = urlArchivo,
                FechaPublicacion = DateTime.UtcNow,
                Visibilidad      = dto.Visibilidad.ToLower(),
                IdUsuario        = dto.IdUsuario
            };

            return await _repo.CreateAsync(post);
        }

        public async Task<bool> DeleteAsync(int id, int idUsuarioSolicitante, bool esAdmin)
        {
            var post = await _repo.GetByIdAsync(id);
            if (post == null) return false;

            if (!esAdmin && post.IdUsuario != idUsuarioSolicitante)
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este post.");

            return await _repo.DeleteAsync(id);
        }
    }
}