using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class CreateAlphadigiCommand : IRequest<Alphadigi_migration.Domain.EntitiesNew.Alphadigi>
{
    public string Ip { get; set; }
    public string Nome { get; set; }
    public Guid AreaId { get; set; }
    public bool Sentido { get; set; }
    public string Estado { get; set; }
    public int LinhasDisplay { get; set; } = 2;
    public bool FotoEvento { get; set; } = false;




}
