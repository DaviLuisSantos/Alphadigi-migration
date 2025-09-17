using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Area;

public class GetAreaByIdQuery : IRequest<Domain.EntitiesNew.Area>
{
    public int id { get; set; }
}
