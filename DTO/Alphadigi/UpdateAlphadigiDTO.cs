using System.ComponentModel.DataAnnotations;

namespace Alphadigi_migration.DTO.Alphadigi
{
    public class UpdateAlphadigiDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(15)]
        public string Ip { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        public bool Sentido { get; set; }

        [Required]
        [StringLength(50)]
        public string? Estado { get; set; } = "DELETE";

    }
}
