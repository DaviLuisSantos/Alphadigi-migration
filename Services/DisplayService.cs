using Alphadigi_migration.DTO.Display;
using Alphadigi_migration.DTO.Alphadigi;
using Alphadigi_migration.Models;
using System.Drawing;

namespace Alphadigi_migration.Services;

public static class DisplayService
{

    public static List<SerialData> recieveMessageAlphadigi(Veiculo veiculo,string acesso)
    {
        var createPackageDisplayDTO = prepareCreatePackage(veiculo, acesso);
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

        // Convert the byte array to a hexadecimal string
        var hexString = BitConverter.ToString(package).Replace("-", "");
        Console.WriteLine(hexString);

        var package64 = Convert.ToBase64String(package);

        var returnDataDisplayDTO = new ReturnDataDisplayDTO
        {
            Message = package64,
            Size = package.Length
        };

        return returnDataDisplayDTO;
    }

    public static List<CreatePackageDisplayDTO> prepareCreatePackage(Veiculo veiculo, string acesso)
    {
        string cor = "red";
        if (acesso == "" || acesso == "CADASTRADO")
            cor = "green";
        else if (acesso == "NÃO CADASTRADO")
            acesso = "NAO CADADASTRADO";

        acesso = acesso == "" ? "LIBERADO" : acesso;
        int tempo = 10, estilo = 0;

        var serialData = new List<CreatePackageDisplayDTO>();

        var packageDisplayPlaca = new CreatePackageDisplayDTO
        {
            Mensagem = veiculo.Placa,
            Linha = 1,
            Cor = "yellow",
            Tempo = tempo,
            Estilo = estilo
        };
        serialData.Add(packageDisplayPlaca);

        if (acesso.Length > 8)
        {
            estilo = 15;
            tempo = 1;
        }
        var packageDisplayAcesso = new CreatePackageDisplayDTO
        {
            Mensagem = acesso,
            Linha = 2,
            Cor = cor,
            Tempo = tempo,
            Estilo = estilo
        };
        serialData.Add(packageDisplayAcesso);

        return serialData;
    }
}
