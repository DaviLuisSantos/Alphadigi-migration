using Alphadigi_migration.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IUnidadeRepository
{
    Task<QueryResult> GetUnidadeInfoAsync(int idUnidade);
    Task<Unidade> GetByIdAsync(int id);
    
}