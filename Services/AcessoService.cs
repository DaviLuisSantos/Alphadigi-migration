using Alphadigi_migration.Data;
using Alphadigi_migration.Interfaces;
using Alphadigi_migration.Models;
using Alphadigi_migration.Repositories;
using Microsoft.AspNetCore.Identity;
using SkiaSharp;

namespace Alphadigi_migration.Services;

public class AcessoService
{
    private readonly AcessoRepository _acessoRepository;
    private readonly IVeiculoService _veiculoService;
    private readonly IAlphadigiService _alphadigi;

    public AcessoService(AcessoRepository acessoRepository, IVeiculoService veiculoService, IAlphadigiService alphadigi)
    {
        _acessoRepository = acessoRepository;
        _veiculoService = veiculoService;
        _alphadigi = alphadigi;
    }

    public async Task<bool> saveVeiculoAcesso(Alphadigi alphadigi, Veiculo veiculo, DateTime timestamp, string? imagem)
    {
        bool estaNoAntiPassback = false;
        if (veiculo.Id != 0)
        {
            estaNoAntiPassback = await verifyPassBack(veiculo, alphadigi, timestamp);
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

        string local = prepareLocalString(alphadigi);
        string dadosVeiculo = _veiculoService.prepareVeiculoDataString(veiculo);
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
        await _acessoRepository.SaveAcesso(acesso);
        return true;
    }

    private async Task<bool> verifyPassBack(Veiculo veiculo, Alphadigi alphadigi, DateTime timestamp)
    {
        Alphadigi? camUlt = await _alphadigi.Get(veiculo.IpCamUltAcesso);

        string? placa = veiculo.Placa;

        TimeSpan? tempoAntipassback = alphadigi.Area.TempoAntipassbackTimeSpan;

        if (camUlt.AreaId != alphadigi.AreaId)
        {
            return false;
        }
        tempoAntipassback = (tempoAntipassback == null || tempoAntipassback == TimeSpan.Zero) ? TimeSpan.FromSeconds(10) : tempoAntipassback;
        var timeLimit = timestamp - tempoAntipassback;

        var recentAccesses = await _acessoRepository.VerifyAntiPassback(veiculo, timeLimit);

        return recentAccesses != null;
    }

    public string prepareLocalString(Alphadigi alphadigi)
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
