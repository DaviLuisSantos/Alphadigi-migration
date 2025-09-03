using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IUnidadeRepository
{
    Task<QueryResult> GetUnidadeInfoAsync(Guid idUnidade);
    Task<Unidade> GetByIdAsync(Guid id);
    
}