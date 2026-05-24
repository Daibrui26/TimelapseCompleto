using System.ComponentModel.DataAnnotations;

namespace TimelapseAPI.Models.DTOs
{
    public class PostArchivoCreateDTO
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public string Texto { get; set; } = string.Empty;

        [Required]
        public string Visibilidad { get; set; } = "publica";

        public IFormFile? Archivo { get; set; }
    }
}