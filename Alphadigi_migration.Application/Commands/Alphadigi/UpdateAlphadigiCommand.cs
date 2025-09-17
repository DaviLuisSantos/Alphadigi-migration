using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class UpdateAlphadigiCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Ip { get; set; }
    public string Nome { get; set; }
    public int AreaId { get; set; }
    public bool Sentido { get; set; }
    public string Estado { get; set; }
    public int LinhasDisplay { get; set; }
    public bool FotoEvento { get; set; }




}
