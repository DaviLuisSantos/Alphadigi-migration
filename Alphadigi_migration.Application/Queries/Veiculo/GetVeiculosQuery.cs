using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Veiculo;

public class GetVeiculosQuery : IRequest<List<Domain.EntitiesNew.Veiculo>>
{
}
