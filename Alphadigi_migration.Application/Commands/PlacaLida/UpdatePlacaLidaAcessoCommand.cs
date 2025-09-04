using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.PlacaLida;

public class UpdatePlacaLidaAcessoCommand : IRequest
{
    public Guid PlacaLidaId { get; set; }
    public bool Liberado { get; set; }
    public string Acesso { get; set; }

}
