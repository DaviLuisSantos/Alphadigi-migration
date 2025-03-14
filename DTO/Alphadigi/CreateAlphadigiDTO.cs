using Alphadigi_migration.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Alphadigi_migration.DTO.Alphadigi
{
    public class CreateAlphadigiDTO
    {
        [Required]
        [StringLength(15)]
        public string Ip { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        public int AreaId { get; set; }
        public bool Sentido { get; set; }
    }
}
