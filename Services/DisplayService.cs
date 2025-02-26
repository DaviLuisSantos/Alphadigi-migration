using Alphadigi_migration.DTO.Display;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;
using System.Drawing;

namespace Alphadigi_migration.Services;

public static class DisplayService
{

    public static List<SerialData> recieveMessageAlphadigi(List<CreatePackageDisplayDTO> createPackageDisplayDTO)
    {
        var serialData = new List<SerialData>();
        foreach (var item in createPackageDisplayDTO)
        {
            var package = PrepareMessage(item);
            var serialDataItem = new SerialData
            {
                serialChannel = 0,
                data = package.Message,
                dataLen = package.Size
            };

            serialData.Add(serialDataItem);
        }
        return serialData;
    }

    public static ReturnDataDisplayDTO PrepareMessage(CreatePackageDisplayDTO createPackageDisplayDTO)
    {
        var package = Display.createPackage(createPackageDisplayDTO);
        var package64 = Convert.ToBase64String(package);

        var returnDataDisplayDTO = new ReturnDataDisplayDTO
        {
            Message = package64,
            Size = package.Length
        };

        return returnDataDisplayDTO;
    }
}
