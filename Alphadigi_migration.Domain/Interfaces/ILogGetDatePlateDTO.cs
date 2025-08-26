using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface ILogGetDatePlateDTO
{


    public string Date { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}
