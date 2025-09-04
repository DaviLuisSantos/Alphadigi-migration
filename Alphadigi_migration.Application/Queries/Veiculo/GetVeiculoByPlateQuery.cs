using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Veiculo;

public class GetVeiculoByPlateQuery : IRequest<Domain.EntitiesNew.Veiculo>
{
    public string Plate { get; set; }
    public int MinMatchingCharacters { get; set; }
}
