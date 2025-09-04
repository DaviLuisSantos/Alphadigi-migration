using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Acesso;

public class PrepareLocalStringQuery : IRequest<string>
{
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
}