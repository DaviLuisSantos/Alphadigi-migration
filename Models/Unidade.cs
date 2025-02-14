using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Models;

[Table("UNIDADE")]
public class Unidade
{
    [Key]
    [Column("IDUNIDADE")]
    public int Id { get; set; }
    [Column("NUMVAGAS")]
    public int Vagas { get; set; }
    [Column("UNIDADE")]
    public string Nome { get; set; }
}
