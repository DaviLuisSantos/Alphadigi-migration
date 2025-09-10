using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;


public interface ICreateAlphadigiDTO
{
    string Ip { get; set; }
    public string Nome { get; set; }
    public int AreaId { get; set; }
    public bool Sentido { get; set; }
    public bool Enviado { get; set; }

}