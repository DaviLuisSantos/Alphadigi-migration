using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class HandleCreateReturnCommand : IRequest<bool>
{
    public string  Ip { get; set; }
}
