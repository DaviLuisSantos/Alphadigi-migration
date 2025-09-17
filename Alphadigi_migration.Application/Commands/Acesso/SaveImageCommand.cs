using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Acesso;

public class SaveImageCommand : IRequest<string>
{
    public string FotoBase64 { get; set; }
    public string Placa { get; set; }
}