using Alphadigi_migration.Domain.Entities;
using Alphadigi_migration.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using SkiaSharp;

namespace Alphadigi_migration.Application.Services;

public class AcessoService : IAcessoService 
{
    private readonly IAcessoRepository _acessoRepository;
    private readonly IVeiculoService _veiculoService;
    private readonly IAlphadigiService _alphadigiService;

    public AcessoService(
        IAcessoRepository acessoRepository,
        IVeiculoService veiculoService,
        IAlphadigiService alphadigiService)
    {
        _acessoRepository = acessoRepository;
        _veiculoService = veiculoService;
        _alphadigiService = alphadigiService;
    }

    public async Task<bool> SaveVeiculoAcesso(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi, Veiculo veiculo, DateTime timestamp, string? imagem)
    {
        bool estaNoAntiPassback = false;
        if (veiculo.Id != 0)
        {
            estaNoAntiPassback = await VerifyPassBack(veiculo, alphadigi, timestamp);
        }
        if (estaNoAntiPassback)
        {
            return false;
        }

        string caminhoImg = null;
        if (imagem != null && alphadigi.FotoEvento == true)
        {
            caminhoImg = await SaveImage(imagem, veiculo.Placa);
        }

        string local = PrepareLocalString(alphadigi);
        string dadosVeiculo = _veiculoService.PrepareVeiculoDataString(veiculo);
        string unidade = veiculo.UnidadeNavigation == null || string.IsNullOrEmpty(veiculo.UnidadeNavigation.Nome) ? "NAO CADASTRADO" : veiculo.UnidadeNavigation.Nome;

        var acesso = new Acesso
        {
            Local = local,
            DataHora = timestamp,
            Unidade = unidade,
            Placa = veiculo.Placa,
            DadosVeiculo = dadosVeiculo,
            GrupoNome = "",
            Foto = caminhoImg
        };

        await _acessoRepository.SaveAcessoAsync(acesso);
        return true;
    }

    private async Task<bool> VerifyPassBack(Veiculo veiculo, Alphadigi_migration.Domain.Entities.Alphadigi alphadigi, DateTime timestamp)
    {
        Alphadigi_migration.Domain.Entities.Alphadigi? camUlt = await _alphadigiService.Get(veiculo.IpCamUltAcesso);

        TimeSpan? tempoAntipassback = alphadigi.Area.TempoAntipassbackTimeSpan;

        if (camUlt?.AreaId != alphadigi.AreaId)
        {
            return false;
        }

        tempoAntipassback = (tempoAntipassback == null || tempoAntipassback == TimeSpan.Zero) ? TimeSpan.FromSeconds(10) : tempoAntipassback;
        var timeLimit = timestamp - tempoAntipassback;

        var recentAccesses = await _acessoRepository.VerifyAntiPassbackAsync(veiculo.Placa, timeLimit);

        return recentAccesses != null;
    }

    public string PrepareLocalString(Alphadigi_migration.Domain.Entities.Alphadigi alphadigi)
    {
        if (alphadigi == null)
        {
            return "Sem local";
        }
        else
        {
            string sentido = alphadigi.Sentido ? "ENTRADA" : "SAIDA";
            return $"{alphadigi.Area.Nome} - {alphadigi.Nome} - {sentido}";
        }
    }

    public async Task<string> SaveImage(string foto64, string placa)
    {
        if (string.IsNullOrEmpty(foto64))
            throw new ArgumentException("String Base64 inválida");

        byte[] imageBytes = Convert.FromBase64String(foto64);

        using var inputStream = new SKMemoryStream(imageBytes);
        using var original = SKBitmap.Decode(inputStream);

        var data = DateTime.Now;

        // Parâmetros fixos
        int quality = 75;                       // Aumentado de 20 para 75
        float scale = 1.0f;                     // Mantém o tamanho original

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

        return rootPath;
    }
}