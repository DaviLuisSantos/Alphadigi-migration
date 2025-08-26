using Alphadigi_migration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAcessoService
{
    Task<bool> SaveVeiculoAcesso(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi, Veiculo veiculo, DateTime timestamp, string? imagem);
    string PrepareLocalString(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi);
    Task<string> SaveImage(string foto64, string placa);
}