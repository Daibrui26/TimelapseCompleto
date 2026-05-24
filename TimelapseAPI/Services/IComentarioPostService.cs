using TimelapseAPI.Models;

namespace TimelapseAPI.Services
{
    public interface IComentarioPostService
    {
        Task<List<ComentarioPost>> GetByPostAsync(int idPost);
        Task<ComentarioPost> CreateAsync(ComentarioPost comentario);
        Task<bool> DeleteAsync(int id, int idUsuarioSolicitante, bool esAdmin);
    }
}