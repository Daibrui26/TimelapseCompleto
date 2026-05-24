using System;

namespace TimelapseAPI.Models
{
    public class ComentarioPost
    {
        public int IdComentarioPost { get; set; }
        public string Texto { get; set; } = string.Empty;
        public DateTime FechaComentario { get; set; }
        public int IdUsuario { get; set; }
        public int IdPost { get; set; }

        // Enriquecido
        public string NombreUsuario { get; set; } = string.Empty;
    }
}