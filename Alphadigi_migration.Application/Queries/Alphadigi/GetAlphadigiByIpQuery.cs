using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class GetAlphadigiByIpQuery : IRequest<Domain.EntitiesNew.Alphadigi>
{

    public string Ip { get; set; }

}
