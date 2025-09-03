using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Alphadigi_migration.Domain.Interfaces;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class CreateAlphadigiDTO : ICreateAlphadigiDTO
{
    [Required]
    [StringLength(15)]
    public string Ip { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; }
    public int AreaId { get; set; }
    public bool Sentido { get; set; }
    public bool Enviado { get; set; } = false;

    public int LinhasDisplay { get;  set; }
    public string Estado { get;  set; }

    public bool FotoEvento { get;  set; }


}
