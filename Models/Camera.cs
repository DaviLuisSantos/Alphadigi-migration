using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Models;

[Table("LPR_MT_CAMERAS")]
public class Camera
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("LOCAL")]
    public string Nome { get; set; }
    [Column("IP")]
    public string Ip { get; set; }
    [Column("MODELO")]
    public string Modelo { get; set; }
    [Column("DIRECAO")]
    public string Direcao { get; set; }
    [Column("ID_AREA")]
    public int IdArea { get; set; }
}
