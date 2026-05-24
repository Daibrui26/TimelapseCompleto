using TimelapseAPI.Models;

namespace TimelapseAPI.Repositories
{
    public interface IPostRepository
    {
        Task<List<Post>> GetAllPublicosAsync();
        Task<List<Post>> GetByUsuarioAsync(int idUsuario);
        Task<Post?> GetByIdAsync(int id);
        Task<Post> CreateAsync(Post post);
        Task<bool> DeleteAsync(int id);
    }
}