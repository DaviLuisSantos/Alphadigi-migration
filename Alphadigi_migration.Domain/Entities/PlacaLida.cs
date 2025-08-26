using Alphadigi_migration.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.Entities;

public class PlacaLida
{
    [Key]
    public int Id { get; set; }
    public string Placa { get; set; }
    public string? Carro_Img { get; set; }
    public string? Placa_Img { get; set; }
    public bool Liberado { get; set; }

    [ForeignKey("Alphadigi")]
    public int AlphadigiId { get; set; }
    public Alphadigi Alphadigi { get; set; }
    public DateTime DataHora { get; set; }

    [ForeignKey("Area")]
    public int AreaId { get; set; }
    public Area Area { get; set; }
    public bool Real { get; set; }
    public bool Cadastrado { get; set; }
    public bool Processado { get; set; }
    public string? Acesso { get; set; }
}
