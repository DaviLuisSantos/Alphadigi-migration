using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Alphadigi_migration.Domain.Entities;

public class MensagemDisplay
{
    [Key]
    public int Id { get; set; }
    public string Placa { get; set; }
    public string Mensagem { get; set; }
    public DateTime DataHora { get; set; }

    [ForeignKey("Alphadigi")]
    public int AlphadigiId { get; set; }
    public Alphadigi_migration.Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    
}
