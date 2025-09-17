using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.Entities;

[Table("VEICULOSMORADORES")]
public class Veiculo
{
    [Key]
    [Column("IDVEICULOMORADOR")]
    public int Id { get; set; }

    [Column("UNIDADE")]
    public string Unidade { get; set; }

    [ForeignKey("Unidade")]
    public Unidade UnidadeNavigation { get; set; }

    [Column("PLACA")]
    public string Placa { get; set; }

    [Column("MARCA")]
    public string Marca { get; set; }

    [Column("MODELO")]
    public string Modelo { get; set; }

    [Column("COR")]
    public string Cor { get; set; }

    [Column("VEICULO_DENTRO")] // Remove private
    public int? VeiculoDentroInt { get; set; }

    [NotMapped]
    public bool VeiculoDentro
    {
        get => VeiculoDentroInt.HasValue && VeiculoDentroInt.Value == 1;
        set => VeiculoDentroInt = value ? 1 : 0;
    }

    [Column("MT_IP_CAM_ULT_ACESSO")]
    public string IpCamUltAcesso { get; set; }

    [Column("MT_DATAHORA_UTL_ACESSO")]
    public DateTime? DataHoraUltAcesso { get; set; }

    [Column("LPR_ID_ROTA")]
    public int? IdRota { get; set; }
    public Rota? Rota { get; set; }
}