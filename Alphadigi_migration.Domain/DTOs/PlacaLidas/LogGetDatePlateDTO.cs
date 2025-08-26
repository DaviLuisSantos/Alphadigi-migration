using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.PlacaLidas;

public class LogGetDatePlateDTO
{
    public string Search { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
