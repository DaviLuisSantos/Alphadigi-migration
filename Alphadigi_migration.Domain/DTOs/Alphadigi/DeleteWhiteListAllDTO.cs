using Alphadigi_migration.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class DeleteWhiteListAllDTO : IDeleteWhiteListAllDTO
{
    public int DeleteWhiteListAll { get ; set ; }
}