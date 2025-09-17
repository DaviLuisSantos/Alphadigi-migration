using Alphadigi_migration.Domain.DTOs.Display;
using Alphadigi_migration.Domain.Interfaces;
using System.Text;

namespace Alphadigi_migration.Domain.Entities;

// Presumindo que o DTO seja algo assim:
// namespace Alphadigi_migration.DTO.Display
// {
//     public class CreatePackageDisplayDTO
//     {
//         public int Linha { get; set; } = 1; // 1 ou 2
//         public int Tempo { get; set; } = 5;  // Duração em segundos (ou unidade definida pelo protocolo)
//         public int Estilo { get; set; } = 0; // Estilo de entrada
//         public string Mensagem { get; set; } = string.Empty;
//         public string Cor { get; set; } = "GREEN"; // "RED", "GREEN", "YELLOW"
//         // Adicionar outros parâmetros se necessário (e.g., BackgroundColor, VelocidadeEntrada, etc.)
//     }
// }

public static class Display
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

    public static byte[] CreatePackage(IDisplayPackage packageParams)
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

        byte duration = (byte)packageParams.Tempo; // Assumindo que Tempo cabe em 1 byte
        byte styleIn = (byte)packageParams.Estilo; // Assumindo que Estilo cabe em 1 byte

        // Confirme se UTF8 é o encoding correto para o dispositivo!
        byte[] message = Encoding.UTF8.GetBytes(packageParams.Mensagem);
        byte messageLength = (byte)message.Length; // Cuidado: message.Length pode ser > 255

        if (message.Length > MaxMessageLengthForDL)
        {
            // Tratar erro: mensagem muito longa para o campo DL do protocolo
            throw new ArgumentException($"A mensagem excede o tamanho máximo de {MaxMessageLengthForDL} bytes para este protocolo.", nameof(packageParams.Mensagem));
            // Ou truncar a mensagem, se apropriado:
            // messageLength = MaxMessageLengthForDL;
            // Array.Resize(ref message, messageLength);
        }

        byte lengthPlusFixed = (byte)(messageLength + FixedHeaderAndParamSize); // DL

        // Converte cor (Nome implica RGBA, mas retorna RGB - 3 bytes conforme uso)
        byte[] colorBytes = ColorToRgbBytes(packageParams.Cor);

        // Alocação correta do buffer: 25 bytes (0-24) + messageLength bytes + 2 bytes CRC
        byte[] buffer = new byte[25 + messageLength + 2]; // Tamanho total = 27 + messageLength

        buffer[0] = DisplayAddress;     // Endereço do display-DA
        buffer[1] = ProtocolVersion;    // Versão do protocolo-VR
        buffer[2] = PnHigh;             // PN (alto)
        buffer[3] = PnLow;              // PN (baixo)
        buffer[4] = CommandSendText;    // Comando-CMD
        buffer[5] = lengthPlusFixed;    // Comprimento dos dados (DL = 19 + tamanho da msg)
        buffer[6] = line;               // Linha do texto-TWID (0 ou 1)
        buffer[7] = styleIn;            // Forma como o texto entra na tela-ETM
        buffer[8] = DefaultEntrySpeed;  // Velocidade que o texto entra-ETS (fixo 0x01 aqui)
        buffer[9] = DefaultDisplayMode; // A maneira de ficar para o texto-DM (fixo 0x00 aqui)
        buffer[10] = duration;          // Tempo em que o texto fica na tela-DT
        buffer[11] = DefaultExitMode;   // Forma como o texto sai da tela-EXM (fixo 0x00 aqui)
        buffer[12] = DefaultExitSpeed;  // Velocidade que o texto sai-EXS (fixo 0x00 aqui)
        buffer[13] = DefaultFontIndex;  // O valor do índice da fonte do texto-FINDEX (fixo 0x00 aqui)
        buffer[14] = DefaultDisplayRepeats; // O número de vezes exibido-DRS (fixo 0x00 aqui)

        // Cor do Texto (TC - RGB - 3 bytes)
        buffer[15] = colorBytes[0];     // R
        buffer[16] = colorBytes[1];     // G
        buffer[17] = colorBytes[2];     // B

        // Cor de Fundo (BC - 5 bytes - fixo como preto aqui)
        buffer[18] = DefaultBackgroundColor[0];
        buffer[19] = DefaultBackgroundColor[1];
        buffer[20] = DefaultBackgroundColor[2];
        buffer[21] = DefaultBackgroundColor[3];
        buffer[22] = DefaultBackgroundColor[4];

        buffer[23] = messageLength;     // Tamanho do texto-TL
        buffer[24] = 0x00;              // Byte reservado ou parte final da configuração? (Confirmar)

        // Copia a Mensagem para o buffer a partir da posição 25
        if (messageLength > 0) // Evita erro se a mensagem for vazia
        {
            Array.Copy(message, 0, buffer, 25, messageLength);
        }

        // Cálculo do CRC16 sobre os dados antes do CRC (índices 0 a 24 + messageLength)
        int lengthForCrc = 25 + messageLength;
        ushort crc = Crc16Modbus(buffer, lengthForCrc);

        // Adiciona o CRC ao final do buffer
        buffer[lengthForCrc] = (byte)(crc & 0xFF);       // CRC (baixo) - índice 25 + messageLength
        buffer[lengthForCrc + 1] = (byte)((crc >> 8) & 0xFF); // CRC (alto)  - índice 26 + messageLength

        return buffer;
    }
    public static byte[] CreateMultiLinePackage(List<CreatePackageDisplayDTO> lines, string voiceText = null, bool saveToFlash = false)
    {
        // Constantes
        const int MaxLines = 4;
        const int MaxTextLengthPerLine = 32;
        const int MaxVoiceLength = 64;
        const byte DefaultDisplaySpeed = 0;
        const byte DefaultDisplayRepeats = 0;
        const byte TextLineSeparator = 0x0D;
        const byte TextLastLineTerminator = 0x00;
        const byte VoiceFlag = 0x0A;
        const byte DisplayAddress = 0x00;
        const byte ProtocolVersion = 0x64;
        const byte PnHigh = 0xFF;
        const byte PnLow = 0xFF;
        const byte CommandMultiLineText = 0x6E;
        const int FixedHeaderAndParamSize = 19;
        const int MaxDlPayloadSize = 255 - FixedHeaderAndParamSize;

        // Validações iniciais
        if (lines == null)
            throw new ArgumentNullException(nameof(lines));
        if (!lines.Any())
            throw new ArgumentException("The list of lines cannot be empty.", nameof(lines));
        if (lines.Count > MaxLines)
            throw new ArgumentException($"The number of lines ({lines.Count}) exceeds the maximum allowed ({MaxLines}).", nameof(lines));

        // Construir TEXT_CONTEXT
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
            //if (textBytes.Length > MaxTextLengthPerLine)
            //  throw new ArgumentException($"Text for line {dto.Linha} ('{dto.Mensagem}') is {textBytes.Length} bytes, exceeding the maximum of {MaxTextLengthPerLine}.", nameof(lines));

            byte lineId = (byte)(dto.Linha - 1); // LID (0-based)
            byte displayMode = (byte)dto.Estilo; // DM
            byte displaySpeed = DefaultDisplaySpeed; // DS
            byte dwellTime = (byte)dto.Tempo; // DT
            byte displayRepeats = DefaultDisplayRepeats; // DR
            byte[] textColor = ColorToRgbaBytes(dto.Cor); // TC[4]
            byte textLength = (byte)textBytes.Length; // TL

            textContextStream.WriteByte(lineId); // LID
            textContextStream.WriteByte(displayMode); // DM
            textContextStream.WriteByte(displaySpeed); // DS
            textContextStream.WriteByte(dwellTime); // DT
            textContextStream.WriteByte(displayRepeats); // DR
            textContextStream.Write(textColor, 0, 4); // TC[4]
            textContextStream.WriteByte(textLength); // TL
            textContextStream.Write(textBytes, 0, textLength); // TEXT[...]
            textContextStream.WriteByte(lineIndex == lines.Count - 1 ? TextLastLineTerminator : TextLineSeparator);

            lineIndex++;
        }

        byte[] textContextData = textContextStream.ToArray();

        // Preparar dados de voz
        byte[] voiceData = Array.Empty<byte>();
        if (!string.IsNullOrEmpty(voiceText))
        {
            byte[] voiceTextBytes = Encoding.UTF8.GetBytes(voiceText);
            if (voiceTextBytes.Length > MaxVoiceLength)
                throw new ArgumentException($"Voice text is too long ({voiceTextBytes.Length} bytes). Maximum is {MaxVoiceLength}.");

            voiceData = new byte[2 + voiceTextBytes.Length + 1]; // VF + VTL + VT + 0x00
            voiceData[0] = VoiceFlag; // 0x0A
            voiceData[1] = (byte)voiceTextBytes.Length; // VTL
            Array.Copy(voiceTextBytes, 0, voiceData, 2, voiceTextBytes.Length);
            voiceData[voiceData.Length - 1] = 0x00; // Terminator
        }

        // Calcular DL (tamanho do texto + 19)
        int dlPayloadSize = textContextData.Length + FixedHeaderAndParamSize;
        if (dlPayloadSize > MaxDlPayloadSize)
            throw new ArgumentException($"Payload too large ({dlPayloadSize} bytes). Maximum is {MaxDlPayloadSize}.");

        // Construir buffer completo
        int bufferSize = 9 + dlPayloadSize; // Header(5) + DL(1) + SaveFlag(1) + Payload + CRC(2)
        byte[] buffer = new byte[bufferSize];

        int currentIndex = 0;

        // Preencher cabeçalho
        buffer[currentIndex++] = DisplayAddress;
        buffer[currentIndex++] = ProtocolVersion;
        buffer[currentIndex++] = PnHigh;
        buffer[currentIndex++] = PnLow;
        buffer[currentIndex++] = CommandMultiLineText;

        // DL e SaveFlag
        buffer[currentIndex++] = (byte)dlPayloadSize;
        buffer[currentIndex++] = saveToFlash ? (byte)0x01 : (byte)0x00;

        // Payload
        buffer[currentIndex++] = (byte)lines.Count; // TEXT_CONTEXT_NUMBER
        Array.Copy(textContextData, 0, buffer, currentIndex, textContextData.Length);
        currentIndex += textContextData.Length;

        // Adicionar voz se existir
        if (voiceData.Length > 0)
        {
            Array.Copy(voiceData, 0, buffer, currentIndex, voiceData.Length);
            currentIndex += voiceData.Length;
        }

        // Calcular CRC
        ushort crc = Crc16Modbus(buffer, currentIndex);
        buffer[currentIndex++] = (byte)(crc & 0xFF);
        buffer[currentIndex++] = (byte)((crc >> 8) & 0xFF);

        // Verificação final
        //if (currentIndex != bufferSize)
        // throw new InvalidOperationException($"Buffer size mismatch. Expected: {bufferSize}, Actual: {currentIndex}");

        return buffer;
    }

    public static byte[] CreateTimeSyncPackage(byte deviceAddress = 0x00, DateTime? customDateTime = null)
    {
        // Obter data/hora atual ou usar a fornecida
        DateTime dateTime = customDateTime ?? DateTime.Now;

        // Constantes do protocolo
        const byte ProtocolVersion = 0x64;
        const byte PnHigh = 0xFF;
        const byte PnLow = 0xFF;
        const byte TimeSyncCommand = 0x05;
        const byte DataLength = 0x08; // Fixo para pacote de tempo

        // Extrair componentes da data/hora
        int year = dateTime.Year;
        if (year < 1970 || year > 2099)
            throw new ArgumentOutOfRangeException("Year must be between 1970 and 2099");

        byte month = (byte)dateTime.Month;
        byte day = (byte)dateTime.Day;
        byte hour = (byte)dateTime.Hour;
        byte minute = (byte)dateTime.Minute;
        byte second = (byte)dateTime.Second;

        // Converter ano para little-endian (2 bytes)
        byte yearLow = (byte)(year & 0xFF);
        byte yearHigh = (byte)((year >> 8) & 0xFF);

        // Calcular dia da semana (1=Domingo, 2=Segunda,...,7=Sábado)
        byte dayOfWeek = (byte)(dateTime.DayOfWeek == DayOfWeek.Sunday ? 1 : (int)dateTime.DayOfWeek + 1);

        // Tamanho total do pacote: 5 (header) + 1 (DL) + 8 (dados) + 2 (CRC) = 16 bytes
        byte[] packet = new byte[16]; // Corrigido o tamanho do array

        // Preencher cabeçalho
        int index = 0;
        packet[index++] = deviceAddress;      // DA
        packet[index++] = ProtocolVersion;    // VR
        packet[index++] = PnHigh;            // PN High
        packet[index++] = PnLow;             // PN Low
        packet[index++] = TimeSyncCommand;   // 0x05

        // Comprimento dos dados
        packet[index++] = DataLength;        // DL

        // Dados de tempo
        packet[index++] = yearLow;           // Y (low byte)
        packet[index++] = yearHigh;          // Y (high byte)
        packet[index++] = month;             // M
        packet[index++] = day;               // D
        packet[index++] = dayOfWeek;         // W
        packet[index++] = hour;              // H
        packet[index++] = minute;            // N
        packet[index++] = second;            // S

        // Calcular CRC (dos bytes desde DA até S)
        ushort crc = Crc16Modbus(packet, index);
        packet[index++] = (byte)(crc & 0xFF);  // CRC Low
        packet[index++] = (byte)(crc >> 8);    // CRC High

        return packet;
    }


    /// <summary>
    /// Converte um nome de cor conhecido em seus componentes RGB (3 bytes).
    /// </summary>
    private static byte[] ColorToRgbBytes(string color)
    {
        // Usar InvariantCultureIgnoreCase para comparação de string robusta
        if (string.Equals(color, "RED", StringComparison.InvariantCultureIgnoreCase))
            return new byte[] { 0xFF, 0x00, 0x00 };
        if (string.Equals(color, "GREEN", StringComparison.InvariantCultureIgnoreCase))
            return new byte[] { 0x00, 0xFF, 0x00 };
        if (string.Equals(color, "YELLOW", StringComparison.InvariantCultureIgnoreCase))
            return new byte[] { 0xFF, 0xFF, 0x00 };
        // Adicionar outras cores se necessário...
        // Laranja: return new byte[] { 0xFF, 0xA5, 0x00 };
        // Azul: return new byte[] { 0x00, 0x00, 0xFF };
        // Branco: return new byte[] { 0xFF, 0xFF, 0xFF };

        // Default para Preto (ou outra cor segura) se desconhecido
        return new byte[] { 0x00, 0x00, 0x00 };
    }

    private static byte[] ColorToRgbaBytes(string color)
    {
        byte r = 0, g = 0, b = 0;
        // Using InvariantCultureIgnoreCase for robust comparison
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
        // Add more colors as needed...
        else // Default to Black (or White might be better?)
        { r = 0x00; g = 0x00; b = 0x00; }

        return new byte[] { r, g, b, 0x00 }; // R, G, B, A (reserved as 0)
    }

    /// <summary>
    /// Calcula o CRC16-Modbus para um array de bytes.
    /// </summary>
    private static ushort Crc16Modbus(byte[] bytes, int length)
    {
        const ushort polynomial = 0xA001; // Polinômio para Modbus CRC
        ushort crc = 0xFFFF; // Valor inicial

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
}