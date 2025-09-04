using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.PlacaLida;

public class CreatePlacaLidaCommand : IRequest<Domain.EntitiesNew.PlacaLida>
{
    public Guid AlphadigiId { get; set; }
    public string Placa { get; set; }
    public DateTime DataHora { get; set; }
    public int AreaId { get; set; }
    public string PlacaImg { get; set; }
    public string CarroImg { get; set; }

}
