using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Models;

[Table("LPR_MT_AREAS")]
public class Area
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("DESCRICAO")]
    public string Nome { get; set; }

    [Column("CONTROLA_VAGA")]
    public bool ControlaVaga { get; set; }

    [Column("EXIBE_NAO_CAD")]
    public bool ExibeNaoCadastrado { get; set; }

    [Column("ENTRADA_VISITA")]
    public bool EntradaVisita { get; set; }

    [Column("SAIDA_VISITA")]
    public bool SaidaVisita { get; set; }

    [Column("SAIDA_SEMPRE_ABRE")]
    public bool SaidaSempreAbre { get; set; }

    [Column("EXIBE_NAO_CAD_SO_ENTRADA")]
    public bool ExibeNaoCadastradoSoEntrada { get; set; }

    [Column("TEMPO_ANTIPASSBACK")]
    public string TempoAntipassback { get; set; }

    // Propriedade não mapeada para obter o TempoAntipassback como TimeSpan
    [NotMapped]
    public TimeSpan? TempoAntipassbackTimeSpan
    {
        get
        {
            if (TimeSpan.TryParse(TempoAntipassback, out TimeSpan timeSpan))
            {
                return timeSpan;
            }
            else
            {
                // Tratar o caso em que a string não é um TimeSpan válido
                // Por exemplo, registrar um log ou retornar null
                Console.WriteLine($"TempoAntipassback inválido: {TempoAntipassback}");
                return null;
            }
        }
    }
}