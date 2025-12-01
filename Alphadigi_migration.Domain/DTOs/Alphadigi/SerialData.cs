using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class SerialData
{
    public int serialChannel { get; set; }
    public string data { get; set; }
    public int dataLen { get; set; }
}