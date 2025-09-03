using Alphadigi_migration.Application.DTOs.Display;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;

using System.Text;


namespace Alphadigi_migration.Application.Service;


public class DisplayProtocolService : IDisplayProtocolService
{
    // --- Constantes do Protocolo ---
    private const byte DisplayAddress = 0x00; // DA
    private const byte ProtocolVersion = 0x64; // VR
    private const byte PnHigh = 0xFF;          // PN (alto)
    private const byte PnLow = 0xFF;           // PN (baixo)
    private const byte CommandSendText = 0x62; // CMD - Comando para enviar texto/página
    private const byte DefaultEntrySpeed = 0x01; // ETS - Velocidade padrão de entrada
    private const byte DefaultDisplayMode = 0x00; // DM - Modo padrão de exibição (parado)
    private const byte DefaultExitMode = 0x00;    // EXM - Modo padrão de saída
    private const byte DefaultExitSpeed = 0x00;   // EXS - Velocidade padrão de saída
    private const byte DefaultFontIndex = 0x00;   // FINDEX - Fonte padrão
    private const byte DefaultDisplayRepeats = 0x00; // DRS - Repetições padrão (0 = contínuo?)
    private static readonly byte[] DefaultBackgroundColor = { 0x00, 0x00, 0x00, 0x00, 0x00 }; // BC - Fixo como preto (5 bytes)
    private const int FixedHeaderAndParamSize = 19; // Tamanho dos dados fixos antes da mensagem (bytes 6 a 24)
    private const int MaxMessageLengthForDL = 255 - FixedHeaderAndParamSize; // Máximo tamanho da mensagem para DL caber em 1 byte

    private const byte CommandMultiLineText = 0x6E; // CMD for multi-line text
    private const byte DefaultDisplaySpeed = 0x00;  // DS: Recommended value 0
    private const int MaxLines = 4;                  // Maximum lines supported by TEXT_CONTEXT_NUMBER
    private const int MaxTextLengthPerLine = 32;    // Maximum TEXT length per line
    private const int MaxDlPayloadSize = 255;        // Maximum value for the DL field
    private const byte TextLineSeparator = 0x0D;     // Separator between TEXT_CONTEXT blocks
    private const byte TextLastLineTerminator = 0x00;// Terminator for the last TEXT_CONTEXT block
    private const byte VoiceFlag = 0x0A;
    private const int MaxVoiceLength = 64;

    public byte[] CreatePackage(IDisplayPackage packageParams)
    {
        if (packageParams == null)
            throw new ArgumentNullException(nameof(packageParams));
        if (packageParams.Mensagem == null)
            throw new ArgumentNullException(nameof(packageParams.Mensagem));

        // Valida e converte linha (DTO usa 1 ou 2, protocolo usa 0 ou 1)
        byte line = packageParams.Linha switch
        {
            1 => 0,
            2 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(packageParams.Linha), "Linha deve ser 1 ou 2.")
        };

        byte duration = (byte)packageParams.Tempo;
        byte styleIn = (byte)packageParams.Estilo;

        byte[] message = Encoding.UTF8.GetBytes(packageParams.Mensagem);
        byte messageLength = (byte)message.Length;

        if (message.Length > MaxMessageLengthForDL)
        {
            throw new ArgumentException($"A mensagem excede o tamanho máximo de {MaxMessageLengthForDL} bytes para este protocolo.", nameof(packageParams.Mensagem));
        }

        byte lengthPlusFixed = (byte)(messageLength + FixedHeaderAndParamSize);

        byte[] colorBytes = ColorToRgbBytes(packageParams.Cor);

        byte[] buffer = new byte[25 + messageLength + 2];

        buffer[0] = DisplayAddress;
        buffer[1] = ProtocolVersion;
        buffer[2] = PnHigh;
        buffer[3] = PnLow;
        buffer[4] = CommandSendText;
        buffer[5] = lengthPlusFixed;
        buffer[6] = line;
        buffer[7] = styleIn;
        buffer[8] = DefaultEntrySpeed;
        buffer[9] = DefaultDisplayMode;
        buffer[10] = duration;
        buffer[11] = DefaultExitMode;
        buffer[12] = DefaultExitSpeed;
        buffer[13] = DefaultFontIndex;
        buffer[14] = DefaultDisplayRepeats;

        buffer[15] = colorBytes[0];
        buffer[16] = colorBytes[1];
        buffer[17] = colorBytes[2];

        buffer[18] = DefaultBackgroundColor[0];
        buffer[19] = DefaultBackgroundColor[1];
        buffer[20] = DefaultBackgroundColor[2];
        buffer[21] = DefaultBackgroundColor[3];
        buffer[22] = DefaultBackgroundColor[4];

        buffer[23] = messageLength;
        buffer[24] = 0x00;

        if (messageLength > 0)
        {
            Array.Copy(message, 0, buffer, 25, messageLength);
        }

        int lengthForCrc = 25 + messageLength;
        ushort crc = Crc16Modbus(buffer, lengthForCrc);

        buffer[lengthForCrc] = (byte)(crc & 0xFF);
        buffer[lengthForCrc + 1] = (byte)((crc >> 8) & 0xFF);

        return buffer;
    }

    public byte[] CreateMultiLinePackage(List<CreatePackageDisplayDTO> lines, string voiceText = null, bool saveToFlash = false)
    {
        if (lines == null)
            throw new ArgumentNullException(nameof(lines));
        if (!lines.Any())
            throw new ArgumentException("The list of lines cannot be empty.", nameof(lines));
        if (lines.Count > MaxLines)
            throw new ArgumentException($"The number of lines ({lines.Count}) exceeds the maximum allowed ({MaxLines}).", nameof(lines));

        using var textContextStream = new MemoryStream();
        int lineIndex = 0;

        foreach (var dto in lines)
        {
            if (dto == null)
                throw new ArgumentException($"DTO at index {lineIndex} is null.", nameof(lines));
            if (dto.Mensagem == null)
                throw new ArgumentException($"Mensagem in DTO at index {lineIndex} is null.", nameof(lines));
            if (dto.Linha < 1 || dto.Linha > MaxLines)
                throw new ArgumentException($"Invalid Linha number ({dto.Linha}) in DTO at index {lineIndex}. Must be 1-{MaxLines}.", nameof(lines));
            if (dto.Tempo < 0 || dto.Tempo > 255)
                throw new ArgumentOutOfRangeException(nameof(lines), $"Tempo ({dto.Tempo}) in DTO at index {lineIndex} must be between 0 and 255.");
            if (dto.Estilo < 0 || dto.Estilo > 255)
                throw new ArgumentOutOfRangeException(nameof(lines), $"Estilo ({dto.Estilo}) in DTO at index {lineIndex} must be between 0 and 255.");

            byte[] textBytes = Encoding.UTF8.GetBytes(dto.Mensagem);

            byte lineId = (byte)(dto.Linha - 1);
            byte displayMode = (byte)dto.Estilo;
            byte displaySpeed = DefaultDisplaySpeed;
            byte dwellTime = (byte)dto.Tempo;
            byte displayRepeats = DefaultDisplayRepeats;
            byte[] textColor = ColorToRgbaBytes(dto.Cor);
            byte textLength = (byte)textBytes.Length;

            textContextStream.WriteByte(lineId);
            textContextStream.WriteByte(displayMode);
            textContextStream.WriteByte(displaySpeed);
            textContextStream.WriteByte(dwellTime);
            textContextStream.WriteByte(displayRepeats);
            textContextStream.Write(textColor, 0, 4);
            textContextStream.WriteByte(textLength);
            textContextStream.Write(textBytes, 0, textLength);
            textContextStream.WriteByte(lineIndex == lines.Count - 1 ? TextLastLineTerminator : TextLineSeparator);

            lineIndex++;
        }

        byte[] textContextData = textContextStream.ToArray();

        byte[] voiceData = Array.Empty<byte>();
        if (!string.IsNullOrEmpty(voiceText))
        {
            byte[] voiceTextBytes = Encoding.UTF8.GetBytes(voiceText);
            if (voiceTextBytes.Length > MaxVoiceLength)
                throw new ArgumentException($"Voice text is too long ({voiceTextBytes.Length} bytes). Maximum is {MaxVoiceLength}.");

            voiceData = new byte[2 + voiceTextBytes.Length + 1];
            voiceData[0] = VoiceFlag;
            voiceData[1] = (byte)voiceTextBytes.Length;
            Array.Copy(voiceTextBytes, 0, voiceData, 2, voiceTextBytes.Length);
            voiceData[voiceData.Length - 1] = 0x00;
        }

        int dlPayloadSize = textContextData.Length + FixedHeaderAndParamSize;
        if (dlPayloadSize > MaxDlPayloadSize)
            throw new ArgumentException($"Payload too large ({dlPayloadSize} bytes). Maximum is {MaxDlPayloadSize}.");

        int bufferSize = 9 + dlPayloadSize;
        byte[] buffer = new byte[bufferSize];

        int currentIndex = 0;

        buffer[currentIndex++] = DisplayAddress;
        buffer[currentIndex++] = ProtocolVersion;
        buffer[currentIndex++] = PnHigh;
        buffer[currentIndex++] = PnLow;
        buffer[currentIndex++] = CommandMultiLineText;

        buffer[currentIndex++] = (byte)dlPayloadSize;
        buffer[currentIndex++] = saveToFlash ? (byte)0x01 : (byte)0x00;

        buffer[currentIndex++] = (byte)lines.Count;
        Array.Copy(textContextData, 0, buffer, currentIndex, textContextData.Length);
        currentIndex += textContextData.Length;

        if (voiceData.Length > 0)
        {
            Array.Copy(voiceData, 0, buffer, currentIndex, voiceData.Length);
            currentIndex += voiceData.Length;
        }

        ushort crc = Crc16Modbus(buffer, currentIndex);
        buffer[currentIndex++] = (byte)(crc & 0xFF);
        buffer[currentIndex++] = (byte)((crc >> 8) & 0xFF);

        return buffer;
    }

    public byte[] CreateTimeSyncPackage(byte deviceAddress = 0x00, DateTime? customDateTime = null)
    {
        DateTime dateTime = customDateTime ?? DateTime.Now;

        const byte ProtocolVersion = 0x64;
        const byte PnHigh = 0xFF;
        const byte PnLow = 0xFF;
        const byte TimeSyncCommand = 0x05;
        const byte DataLength = 0x08;

        int year = dateTime.Year;
        if (year < 1970 || year > 2099)
            throw new ArgumentOutOfRangeException("Year must be between 1970 and 2099");

        byte month = (byte)dateTime.Month;
        byte day = (byte)dateTime.Day;
        byte hour = (byte)dateTime.Hour;
        byte minute = (byte)dateTime.Minute;
        byte second = (byte)dateTime.Second;

        byte yearLow = (byte)(year & 0xFF);
        byte yearHigh = (byte)((year >> 8) & 0xFF);

        byte dayOfWeek = (byte)(dateTime.DayOfWeek == DayOfWeek.Sunday ? 1 : (int)dateTime.DayOfWeek + 1);

        byte[] packet = new byte[16];

        int index = 0;
        packet[index++] = deviceAddress;
        packet[index++] = ProtocolVersion;
        packet[index++] = PnHigh;
        packet[index++] = PnLow;
        packet[index++] = TimeSyncCommand;

        packet[index++] = DataLength;

        packet[index++] = yearLow;
        packet[index++] = yearHigh;
        packet[index++] = month;
        packet[index++] = day;
        packet[index++] = dayOfWeek;
        packet[index++] = hour;
        packet[index++] = minute;
        packet[index++] = second;

        ushort crc = Crc16Modbus(packet, index);
        packet[index++] = (byte)(crc & 0xFF);
        packet[index++] = (byte)(crc >> 8);

        return packet;
    }

    #region Private Helper Methods

    private static byte[] ColorToRgbBytes(string color)
    {
        if (string.Equals(color, "RED", StringComparison.InvariantCultureIgnoreCase))
            return new byte[] { 0xFF, 0x00, 0x00 };
        if (string.Equals(color, "GREEN", StringComparison.InvariantCultureIgnoreCase))
            return new byte[] { 0x00, 0xFF, 0x00 };
        if (string.Equals(color, "YELLOW", StringComparison.InvariantCultureIgnoreCase))
            return new byte[] { 0xFF, 0xFF, 0x00 };

        return new byte[] { 0x00, 0x00, 0x00 };
    }

    private static byte[] ColorToRgbaBytes(string color)
    {
        byte r = 0, g = 0, b = 0;

        if (string.Equals(color, "RED", StringComparison.InvariantCultureIgnoreCase))
        { r = 0xFF; g = 0x00; b = 0x00; }
        else if (string.Equals(color, "GREEN", StringComparison.InvariantCultureIgnoreCase))
        { r = 0x00; g = 0xFF; b = 0x00; }
        else if (string.Equals(color, "YELLOW", StringComparison.InvariantCultureIgnoreCase))
        { r = 0xFF; g = 0xFF; b = 0x00; }
        else if (string.Equals(color, "BLUE", StringComparison.InvariantCultureIgnoreCase))
        { r = 0x00; g = 0x00; b = 0xFF; }
        else if (string.Equals(color, "WHITE", StringComparison.InvariantCultureIgnoreCase))
        { r = 0xFF; g = 0xFF; b = 0xFF; }
        else if (string.Equals(color, "CYAN", StringComparison.InvariantCultureIgnoreCase))
        { r = 0x00; g = 0xFF; b = 0xFF; }
        else if (string.Equals(color, "MAGENTA", StringComparison.InvariantCultureIgnoreCase))
        { r = 0xFF; g = 0x00; b = 0xFF; }

        return new byte[] { r, g, b, 0x00 };
    }

    private static ushort Crc16Modbus(byte[] bytes, int length)
    {
        const ushort polynomial = 0xA001;
        ushort crc = 0xFFFF;

        for (int i = 0; i < length; i++)
        {
            crc ^= bytes[i];
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 0x0001) != 0)
                {
                    crc >>= 1;
                    crc ^= polynomial;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }
        return crc;
    }

    #endregion
}
