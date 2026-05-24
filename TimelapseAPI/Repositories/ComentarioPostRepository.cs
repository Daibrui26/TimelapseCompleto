using Microsoft.Data.SqlClient;
using TimelapseAPI.Models;

namespace TimelapseAPI.Repositories
{
    public class ComentarioPostRepository : IComentarioPostRepository
    {
        private readonly string _connectionString;

        public ComentarioPostRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TimelapseDB")
                ?? throw new Exception("Connection string not found");
        }

        private static ComentarioPost Map(SqlDataReader r) => new ComentarioPost
        {
            IdComentarioPost = r.GetInt32(0),
            Texto            = r.GetString(1),
            FechaComentario  = r.GetDateTime(2),
            IdUsuario        = r.GetInt32(3),
            IdPost           = r.GetInt32(4),
            NombreUsuario    = r.IsDBNull(5) ? string.Empty : r.GetString(5)
        };

        public async Task<List<ComentarioPost>> GetByPostAsync(int idPost)
        {
            var list = new List<ComentarioPost>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            const string query = @"
                SELECT cp.id_comentariopost, cp.texto, cp.fecha_comentario,
                       cp.id_usuario, cp.id_post, u.nombre
                FROM ComentarioPost cp
                INNER JOIN Usuario u ON cp.id_usuario = u.id_usuario
                WHERE cp.id_post = @idPost
                ORDER BY cp.fecha_comentario DESC";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@idPost", idPost);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<ComentarioPost?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            const string query = @"
                SELECT cp.id_comentariopost, cp.texto, cp.fecha_comentario,
                       cp.id_usuario, cp.id_post, u.nombre
                FROM ComentarioPost cp
                INNER JOIN Usuario u ON cp.id_usuario = u.id_usuario
                WHERE cp.id_comentariopost = @id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? Map(r) : null;
        }

        public async Task<ComentarioPost> CreateAsync(ComentarioPost comentario)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            const string query = @"
                INSERT INTO ComentarioPost (texto, fecha_comentario, id_usuario, id_post)
                OUTPUT INSERTED.id_comentariopost
                VALUES (@texto, @fecha, @idUsuario, @idPost)";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@texto",    comentario.Texto);
            cmd.Parameters.AddWithValue("@fecha",    comentario.FechaComentario);
            cmd.Parameters.AddWithValue("@idUsuario", comentario.IdUsuario);
            cmd.Parameters.AddWithValue("@idPost",   comentario.IdPost);
            comentario.IdComentarioPost = (int)await cmd.ExecuteScalarAsync();
            return comentario;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new SqlCommand(
                "DELETE FROM ComentarioPost WHERE id_comentariopost = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            return await cmd.ExecuteNonQueryAsync() > 0;
        }
    }
}