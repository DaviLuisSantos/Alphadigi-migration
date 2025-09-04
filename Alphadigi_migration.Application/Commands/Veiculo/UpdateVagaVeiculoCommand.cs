using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Veiculo;

public class UpdateVagaVeiculoCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public bool Dentro { get; set; }
   
}
