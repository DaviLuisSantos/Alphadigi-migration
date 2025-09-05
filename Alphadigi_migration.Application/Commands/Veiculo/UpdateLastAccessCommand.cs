using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Veiculo;

public class UpdateLastAccessCommand : IRequest<bool>
{
    public Guid IdVeiculo { get; set; }
    public string IpCamera { get; set; }
    public DateTime DataHoraAcesso { get; set; }
}
