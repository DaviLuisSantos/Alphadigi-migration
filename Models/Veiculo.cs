using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Models;

[Table("VEICULOSMORADORES")]
public class Veiculo
{
    [Key]
    [Column("IDVEICULOMORADOR")]
    public int Id { get; set; }
    [Column("UNIDADE")]
    public string Unidade { get; set; }
    [Column("PLACA")]
    public string Placa { get; set; }
    [Column("MARCA")]
    public string Marca { get; set; }
    [Column("MODELO")]
    public string Modelo { get; set; }
    [Column("COR")]
    public string Cor { get; set; }
    [Column("VEICULO_DENTRO")]
    public bool VeiculoDentro { get; set; }
}
