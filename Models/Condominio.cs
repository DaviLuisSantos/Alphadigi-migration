using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Models;

[Table("DADOS_CLIENTE")]
public class Condominio
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("NOME")]
    public string Nome { get; set; }
    [Column("CNPJ")]
    public string Cnpj { get; set; }
    [Column("FANTASIA")]
    public string Fantasia { get; set; }
}
