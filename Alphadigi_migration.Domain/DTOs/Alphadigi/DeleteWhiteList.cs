using Alphadigi_migration.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.DTOs.Alphadigi;

public class DeleteWhiteList
{
    public List<DelDataDTO> DelData { get; set; }
}