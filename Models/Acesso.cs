using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Models;

[Table("LPR_MT_ACESSO")]
public class Acesso
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("LOCAL")]
    public string Local { get; set; }
    [Column("DATA_HORA")]
    public DateTime DataHora { get; set; }
    [Column("UNIDADE")]
    public string Unidade { get; set; }
    [Column("PLACA_LPR")]
    public string Placa { get; set; }
    [Column("DADOS_VEICULO")]
    public string DadosVeiculo { get; set; }
    [Column("GRUPO_NOME")]
    public string GrupoNome { get; set; }
}
