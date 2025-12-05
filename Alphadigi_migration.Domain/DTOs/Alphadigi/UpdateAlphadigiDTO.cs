using Alphadigi_migration.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class UpdateAlphadigiDTO : IUpdateAlphadigiDTO
{
    public int Id { get; set; }

    [Required]
    [StringLength(15)]
    public string Ip { get; set; }

    [Required]
    [StringLength(100)]
    public string Nome { get; set; }
    public bool Sentido { get; set; }

    public int LinhasDisplay { get; set; } = 2;
    [Required]
    [StringLength(50)]
    public string? Estado { get; set; } = "DELETE";
}