using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Veiculo;

public class GetUnidadeByIdQuery : IRequest<Domain.EntitiesNew.Unidade>
{
    public string Unidade { get; set; }

    public GetUnidadeByIdQuery(string unidade)
    {
        Unidade = unidade;
    }
}
