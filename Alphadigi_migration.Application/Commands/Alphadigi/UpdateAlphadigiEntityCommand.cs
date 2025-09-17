using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class UpdateAlphadigiEntityCommand : IRequest<Alphadigi_migration.Domain.EntitiesNew.Alphadigi>
{

    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }

}
