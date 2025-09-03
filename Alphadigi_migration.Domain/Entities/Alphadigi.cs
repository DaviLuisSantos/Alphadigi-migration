using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.Entities;


public class Alphadigi
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(15)]
    public string Ip { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; }

    [ForeignKey("Area")]
    public int AreaId { get; set; }
    public Area Area { get; set; }

    public bool Sentido { get; set; }

    [Required]
    [StringLength(50)]
    public string? Estado { get; set; } = "DELETE";

    public int? UltimoId { get; set; }
    [StringLength(10)]
    public string? UltimaPlaca { get; set; }
    public DateTime? UltimaHora { get; set; }
    public int LinhasDisplay { get; set; } = 2;
    public bool Enviado { get; set; } = false;
    public bool? FotoEvento { get; set; } = false;
}
