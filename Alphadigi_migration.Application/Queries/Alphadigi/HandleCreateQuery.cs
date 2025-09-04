using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class HandleCreateQuery : IRequest<Domain.DTOs.Alphadigi.AddWhiteListDTO>
{
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
}
