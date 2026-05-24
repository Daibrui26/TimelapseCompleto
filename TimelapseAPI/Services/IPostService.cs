using TimelapseAPI.Models;
using TimelapseAPI.Models.DTOs;

namespace TimelapseAPI.Services
{
    public interface IPostService
    {
        Task<List<Post>> GetAllPublicosAsync();
        Task<List<Post>> GetByUsuarioAsync(int idUsuario);
        Task<Post?> GetByIdAsync(int id);
        Task<Post> CreateAsync(PostArchivoCreateDTO dto);
        Task<bool> DeleteAsync(int id, int idUsuarioSolicitante, bool esAdmin);
    }
}