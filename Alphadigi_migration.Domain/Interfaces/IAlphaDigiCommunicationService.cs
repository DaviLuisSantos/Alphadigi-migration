using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Domain.Interfaces;

public interface IAlphaDigiCommunicationService
{
    Task<bool> SendToAlphaDigiAsync(string ip, List<Domain.DTOs.Alphadigi.SerialData> serialData);
}
