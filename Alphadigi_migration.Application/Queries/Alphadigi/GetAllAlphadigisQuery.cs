using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class GetAllAlphadigisQuery : IRequest<List<Domain.EntitiesNew.Alphadigi>>
{
}
