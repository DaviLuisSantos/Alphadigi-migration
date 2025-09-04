using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.DTOs.Rota;

public class RotaDto
{
    public Guid Id { get; set; }
    public int RotaId { get; set; }
    public int CameraId { get; set; }
    public string CameraNome { get; set; }
}
