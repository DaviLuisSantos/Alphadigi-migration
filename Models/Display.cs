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

        byte[] colorBytes = ColorToRgbaBytes(packageParams.Cor);
        byte[] backgroundColor = { 0x00, 0x00, 0x00, 0x00, 0x00 };

        byte[] buffer = new byte[26 + length + 2]; // 26 bytes fixed + message length + 2 bytes CRC

        buffer[0] = 0x00; // Endereço do display-DA
        buffer[1] = 0x64; // Versão do protocolo-VR
        buffer[2] = 0xff; // PN (alto)
        buffer[3] = 0xff; // PN (baixo)
        buffer[4] = 0x62; // Comando-CMD
        buffer[5] = lengthPlus19; // Comprimento dos dados + 19-DL
        buffer[6] = line; // Linha do texto-TWID
        buffer[7] = styleIn; // Forma como o texto entra na tela-ETM
        buffer[8] = 0x01; // Velocidade que o texto entra-ETS
        buffer[9] = 0x00; // A maneira de ficar para o texto-DM
        buffer[10] = duration; // Tempo em que o texto fica na tela-DT
        buffer[11] = 0x00; // Forma como o texto sai da tela-EXM
        buffer[12] = 0x00; // Velocidade que o texto sai-EXS
        buffer[13] = 0x00; // O valor do índice da fonte do texto-FINDEX
        buffer[14] = 0x00; // O número de vezes exibido-DRS
        buffer[15] = colorBytes[0]; // Cor a ser exibida na tela-TC (RGBA)
        buffer[16] = colorBytes[1];
        buffer[17] = colorBytes[2];
        buffer[18] = backgroundColor[0]; // Cor de fundo-BC
        buffer[19] = backgroundColor[1];
        buffer[20] = backgroundColor[2];
        buffer[21] = backgroundColor[3];
        buffer[22] = backgroundColor[4];
        buffer[23] = length; // Tamanho do texto-TL
        buffer[24] = 0x00;

        Array.Copy(message, 0, buffer, 25, length); // Mensagem

        // Cálculo do CRC16
        ushort crc = Crc16Modbus(buffer, 25 + length); // Calcula CRC16 até o final da mensagem

        buffer[25 + length] = (byte)(crc & 0xff); // CRC (baixo)
        buffer[26 + length] = (byte)((crc >> 8) & 0xff); // CRC (alto)

        return buffer;
    }

    private static byte[] ColorToRgbaBytes(string color)
    {
        return color.ToUpper() switch
        {
            "RED" => new byte[] { 0xFF, 0x00, 0x00 },
            "GREEN" => new byte[] { 0x00, 0xFF, 0x00 },
            "YELLOW" => new byte[] { 0xFF, 0xFF, 0x00 },
            _ => new byte[] { 0x00, 0x00, 0x00 } // Default to black if color is unknown
        };
    }

    private static ushort Crc16Modbus(byte[] bytes, int length)
    {
        ushort crc = 0xFFFF;
        for (int i = 0; i < length; i++)
        {
            crc ^= bytes[i];
            for (int j = 0; j < 8; j++)
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
