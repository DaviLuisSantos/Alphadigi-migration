using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alphadigi_migration.Domain.Entities;

[Table("LPR_MT_ROTAS_CAM")]
public class Rota
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("ID_ROTA")]
    public int RotaId { get; set; }
    [Column("ID_CAM")]
    public int CameraId { get; set; }
}
