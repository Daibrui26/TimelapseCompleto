using TimelapseAPI.Models;
using TimelapseAPI.Repositories;

namespace TimelapseAPI.Services
{
    public class ComentarioPostService : IComentarioPostService
    {
        private readonly IComentarioPostRepository _repo;

        public ComentarioPostService(IComentarioPostRepository repo)
        {
            _repo = repo;
        }

        public Task<List<ComentarioPost>> GetByPostAsync(int idPost) =>
            _repo.GetByPostAsync(idPost);

        public async Task<ComentarioPost> CreateAsync(ComentarioPost comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario.Texto))
                throw new ArgumentException("El texto del comentario no puede estar vacío.");

            if (comentario.IdUsuario <= 0)
                throw new ArgumentException("IdUsuario no válido.");

            if (comentario.IdPost <= 0)
                throw new ArgumentException("IdPost no válido.");

            comentario.FechaComentario = DateTime.UtcNow;
            return await _repo.CreateAsync(comentario);
        }

        public async Task<bool> DeleteAsync(int id, int idUsuarioSolicitante, bool esAdmin)
        {
            var comentario = await _repo.GetByIdAsync(id);
            if (comentario == null) return false;

            if (!esAdmin && comentario.IdUsuario != idUsuarioSolicitante)
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este comentario.");

            return await _repo.DeleteAsync(id);
        }
    }
}