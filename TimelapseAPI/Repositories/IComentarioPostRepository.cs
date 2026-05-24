using TimelapseAPI.Models;

namespace TimelapseAPI.Repositories
{
    public interface IComentarioPostRepository
    {
        Task<List<ComentarioPost>> GetByPostAsync(int idPost);
        Task<ComentarioPost?> GetByIdAsync(int id);
        Task<ComentarioPost> CreateAsync(ComentarioPost comentario);
        Task<bool> DeleteAsync(int id);
    }
}