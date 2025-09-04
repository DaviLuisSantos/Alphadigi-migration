using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Area;

public class GetAreaByIdGuidQuery : IRequest<Domain.EntitiesNew.Area>
{

    public Guid Id { get; set; }
}
