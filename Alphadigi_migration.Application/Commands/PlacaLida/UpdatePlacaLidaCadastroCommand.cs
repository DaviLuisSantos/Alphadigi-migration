using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.PlacaLida;

public class UpdatePlacaLidaCadastroCommand : IRequest
{
    public Guid PlacaLidaId { get; set; }
    public bool Cadastrado { get; set; }
}
