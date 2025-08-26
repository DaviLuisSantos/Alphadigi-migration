using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

    public class ResponseAlarmInfoPlate
    {
       public string info { get; set; }
       public List<SerialData>? serialData { get; set; }


    }

