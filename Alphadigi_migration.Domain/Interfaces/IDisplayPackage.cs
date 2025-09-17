using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IDisplayPackage
{
    public string Mensagem { get; set; }
    public int Linha { get; set; }
    public string Cor { get; set; }
    public int Tempo { get; set; }
    public int Estilo { get; set; }

}
