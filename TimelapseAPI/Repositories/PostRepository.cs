using Microsoft.Data.SqlClient;
using TimelapseAPI.Models;

namespace TimelapseAPI.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly string _connectionString;

        public PostRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TimelapseDB")
                ?? throw new Exception("Connection string not found");
        }

        private static Post Map(SqlDataReader r) => new Post
        {
            IdPost           = r.GetInt32(0),
            Texto            = r.GetString(1),
            UrlArchivo       = r.IsDBNull(2) ? null : r.GetString(2),
            PublicId         = r.IsDBNull(3) ? null : r.GetString(3),
            FechaPublicacion = r.GetDateTime(4),
            Visibilidad      = r.GetString(5),
            IdUsuario        = r.GetInt32(6),
            NombreUsuario    = r.IsDBNull(7) ? string.Empty : r.GetString(7),
            TotalComentarios = r.IsDBNull(8) ? 0 : r.GetInt32(8)
        };

        private const string SelectBase = @"
            SELECT 
                p.id_post, p.texto, p.url_archivo, p.public_id,
                p.fecha_publicacion, p.visibilidad, p.id_usuario,
                u.nombre,
                (SELECT COUNT(*) FROM ComentarioPost cp WHERE cp.id_post = p.id_post) AS total_comentarios
            FROM Post p
            INNER JOIN Usuario u ON p.id_usuario = u.id_usuario";

        public async Task<List<Post>> GetAllPublicosAsync()
        {
            var list = new List<Post>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                SelectBase + " WHERE p.visibilidad = 'publica' ORDER BY p.fecha_publicacion DESC", conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<List<Post>> GetByUsuarioAsync(int idUsuario)
        {
            var list = new List<Post>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                SelectBase + " WHERE p.id_usuario = @id ORDER BY p.fecha_publicacion DESC", conn);
            cmd.Parameters.AddWithValue("@id", idUsuario);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(SelectBase + " WHERE p.id_post = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? Map(r) : null;
        }

        public async Task<Post> CreateAsync(Post post)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            const string query = @"
                INSERT INTO Post (texto, url_archivo, public_id, fecha_publicacion, visibilidad, id_usuario)
                OUTPUT INSERTED.id_post
                VALUES (@texto, @urlArchivo, @publicId, @fechaPublicacion, @visibilidad, @idUsuario)";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@texto",            post.Texto);
            cmd.Parameters.AddWithValue("@urlArchivo",       (object?)post.UrlArchivo  ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@publicId",         (object?)post.PublicId    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fechaPublicacion", post.FechaPublicacion);
            cmd.Parameters.AddWithValue("@visibilidad",      post.Visibilidad);
            cmd.Parameters.AddWithValue("@idUsuario",        post.IdUsuario);
            post.IdPost = (int)await cmd.ExecuteScalarAsync();
            return post;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();
            try
            {
                using (var cmd = new SqlCommand(
                    "DELETE FROM ComentarioPost WHERE id_post = @id", conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
                int rows;
                using (var cmd = new SqlCommand(
                    "DELETE FROM Post WHERE id_post = @id", conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    rows = await cmd.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}