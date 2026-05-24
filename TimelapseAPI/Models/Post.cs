using System;

namespace TimelapseAPI.Models
{
    public class Post
    {
        public int IdPost { get; set; }
        public string Texto { get; set; } = string.Empty;
        public string? UrlArchivo { get; set; }
        public string? PublicId { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public string Visibilidad { get; set; } = "publica";
        public int IdUsuario { get; set; }

        // Datos enriquecidos (no columnas BD, se rellenan en el servicio)
        public string NombreUsuario { get; set; } = string.Empty;
        public int TotalComentarios { get; set; }
    }
}