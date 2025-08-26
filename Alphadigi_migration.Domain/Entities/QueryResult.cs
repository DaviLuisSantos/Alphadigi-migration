using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Entities;

public class QueryResult
{
    public int NumVagas { get; set; }
    public int? VagasOcupadasVisitantes { get; set; }
    public int VagasOcupadasMoradores { get; set; }
}