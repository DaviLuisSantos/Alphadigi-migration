using Alphadigi_migration.Application.Commands.Acesso;
using MediatR;
using Microsoft.Extensions.Logging;
using SkiaSharp;


namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Acesso;

public class SaveImageCommandHandler : IRequestHandler<SaveImageCommand, string>
{
    private readonly ILogger<SaveImageCommandHandler> _logger;

    public SaveImageCommandHandler(ILogger<SaveImageCommandHandler> logger)
    {
        _logger = logger;
    }

    public async Task<string> Handle(SaveImageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrEmpty(request.FotoBase64))
                throw new ArgumentException("String Base64 inválida");

            byte[] imageBytes = Convert.FromBase64String(request.FotoBase64);

            using var inputStream = new SKMemoryStream(imageBytes);
            using var original = SKBitmap.Decode(inputStream);

            var data = DateTime.Now;

            // Parâmetros de compressão
            int quality = 75;
            float scale = 1.0f;

            int newWidth = (int)(original.Width * scale);
            int newHeight = (int)(original.Height * scale);

            using var resizedBitmap = original
                .Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High);

            using var image = SKImage.FromBitmap(resizedBitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Jpeg, quality);
            byte[] finalImageBytes = encoded.ToArray();

            // Caminho e nome do arquivo
            var ano = data.Year;
            var mes = data.Month;
            string fileName = $"{data:yyyyddMM_HHmmssfff}.jpg";
            string relativePath = $"FOTOLPR/{ano}/{mes}";
            string rootPath = $"{relativePath}/{fileName}";
            string directoryPath = Path.GetFullPath($"../../FOTOVISITA/{relativePath}");
            string fullPath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            await File.WriteAllBytesAsync(fullPath, finalImageBytes);

            _logger.LogInformation($"Imagem salva para placa {request.Placa}: {rootPath}");
            return rootPath;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao salvar imagem para placa {request.Placa}");
            return null;
        }
    }
}

