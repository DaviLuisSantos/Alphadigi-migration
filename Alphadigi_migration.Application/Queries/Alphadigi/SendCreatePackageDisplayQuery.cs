using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class SendCreatePackageDisplayQuery : IRequest<List<Domain.DTOs.Alphadigi.SerialData>>
{
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public string Acesso { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
}
