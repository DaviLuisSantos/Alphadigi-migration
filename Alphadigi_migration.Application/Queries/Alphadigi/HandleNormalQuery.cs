using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class HandleNormalQuery : IRequest<Domain.DTOs.Alphadigi.ResponseHeathbeatDTO>
{
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }


}
