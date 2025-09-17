using Alphadigi_migration.Application.DTOs.Display;
using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.Interfaces;


namespace Alphadigi_migration.Application.Service;


public interface IDisplayProtocolService
{
    byte[] CreatePackage(IDisplayPackage packageParams);
    byte[] CreateMultiLinePackage(List<CreatePackageDisplayDTO> lines, string voiceText = null, bool saveToFlash = false);
    byte[] CreateTimeSyncPackage(byte deviceAddress = 0x00, DateTime? customDateTime = null);
}




