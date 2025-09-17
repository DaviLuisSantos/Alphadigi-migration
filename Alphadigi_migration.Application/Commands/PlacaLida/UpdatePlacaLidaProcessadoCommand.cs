using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.PlacaLida;

public class UpdatePlacaLidaProcessadoCommand : IRequest
{
    public int PlacaLidaId { get; set; }
    public bool Processado { get; set; }
}
