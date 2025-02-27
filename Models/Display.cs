using Alphadigi_migration.DTO.Display;
using System.Text;

namespace Alphadigi_migration.Models;

public static class Display
{

    public static byte[] createPackage(CreatePackageDisplayDTO packageParams)
    {
        byte line = (byte)(packageParams.Linha == 1 ? 0 : 1);
        byte duration = (byte)(packageParams.Tempo);
        byte styleIn = (byte)(packageParams.Estilo);

        byte[] message = Encoding.UTF8.GetBytes(packageParams.Mensagem);

        byte length = (byte)(message.Length);

        byte lengthPlus19 = (byte)(length + 19);

        string colorHex = ColorToRgba(packageParams.Cor).Replace("#", "");

        byte[] colorBytes = StringToByteArray(colorHex);

        byte[] backgroundColor = { 0x00, 0x00, 0x00, 0x00, 0x00 };

        List<byte> buffer = new List<byte>
        {
            0x00,                           // Endereço do display-DA
            0x64,                           // Versão do protocolo-VR
            0xff,                           // PN (alto)
            0xff,                           // PN (baixo)
            0x62,                           // Comando-CMD
            lengthPlus19,                   // Comprimento dos dados + 19-DL
            line,                           // Linha do texto-TWID
            styleIn,                        // Forma como o texto entra na tela-ETM
            0x01,                           // Velocidade que o texto entra-ETS
            0x00,                           // A maneira de ficar para o texto-DM
            duration,                       // Tempo em que o texto fica na tela-DT
            0x00,                           // Forma como o texto sai da tela-EXM
            0x00,                           // Velocidade que o texto sai-EXS
            0x00,                           // O valor do índice da fonte do texto-FINDEX
            0x00,                           // O número de vezes exibido-DRS
            colorBytes[0], colorBytes[1], colorBytes[2],   // Cor a ser exibida na tela-TC (RGBA)
            backgroundColor[0], backgroundColor[1], backgroundColor[2], backgroundColor[3],backgroundColor[4], // Cor de fundo-BC
            length,                         // Tamanho do texto-TL
            0x00,
        };

        buffer.AddRange(message);      // Mensagem

        // 3. Cálculo do CRC16
        byte[] packetWithoutCrc = buffer.ToArray(); // Cria array sem CRC
        ushort crc = Crc16Modbus(packetWithoutCrc);     // Calcula CRC16

        buffer.Add((byte)(crc & 0xff));       // CRC (baixo)
        buffer.Add((byte)((crc >> 8) & 0xff));  // CRC (alto)

        // 4. Retorna o buffer final como pacote
        return buffer.ToArray();
    }

    private static string ColorToRgba(string color)
    {
        return color.ToUpper() switch
        {
            "RED" => "#FF0000",
            "GREEN" => "#00FF00",
            "YELLOW" => "#FFFF00",
        };
    }

    private static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    private static ushort Crc16Modbus(byte[] bytes)
    {
        ushort crc = 0xFFFF;
        byte[] array = bytes;
        foreach (byte value in array)
        {
            crc ^= value;

            for (int i = 0; i < 8; i++)
            {
                if ((crc & 0x0001) != 0)
                {
                    crc >>= 1;
                    crc ^= 0xA001;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }
        return crc;
    }

}
