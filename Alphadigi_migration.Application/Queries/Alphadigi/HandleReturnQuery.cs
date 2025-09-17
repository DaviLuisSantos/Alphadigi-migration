using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Queries.Alphadigi;

public class HandleReturnQuery : IRequest<Domain.DTOs.Alphadigi.ResponsePlateDTO>
{
    public string Placa { get; set; }
    public string Acesso { get; set; }
    public bool Liberado { get; set; }
    public List<Domain.DTOs.Alphadigi.SerialData> MessageData { get; set; }

}
