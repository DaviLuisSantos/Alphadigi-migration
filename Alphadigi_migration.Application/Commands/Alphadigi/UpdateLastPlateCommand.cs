using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class UpdateLastPlateCommand : IRequest<bool>
{
    public Domain.EntitiesNew.Alphadigi Camera { get; set; }
    public string Plate { get; set; }
    public DateTime Timestamp { get; set; }


}
