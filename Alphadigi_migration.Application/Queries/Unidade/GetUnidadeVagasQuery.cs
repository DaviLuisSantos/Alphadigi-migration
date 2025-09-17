using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Unidade;

public class GetUnidadeVagasQuery : IRequest<UnidadeVagasInfo>
{
    public string UnidadeId { get; set; }
}

public class UnidadeVagasInfo
{
    public int NumVagas { get; set; }
    public int VagasOcupadas { get; set; }
    public bool TemVagaDisponivel => NumVagas > VagasOcupadas;
}