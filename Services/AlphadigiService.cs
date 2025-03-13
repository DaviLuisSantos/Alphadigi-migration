using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Data;
using Microsoft.Extensions.Logging;

namespace Alphadigi_migration.Services;

public interface IAlphadigiService
{
    Task<Alphadigi> GetOrCreate(string ip);
    Task<bool> UpdateLastPlate(Alphadigi camera, string plate, DateTime timestamp);
    Task<bool> SyncAlphadigi();
    Task<Alphadigi> Get(string ip);
    Task<List<Alphadigi>> GetAll();
    Task<bool> Update(Alphadigi camera);
    Task<bool> updateStage(string stage);
    Task<bool> Delete(int id);
}

public class AlphadigiService:IAlphadigiService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<AlphadigiService> _logger;

    public AlphadigiService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird, ILogger<AlphadigiService> logger)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _logger = logger; // Atribui o ILogger
        _logger.LogInformation("AlphadigiService criado.");
    }

    public async Task<bool> SyncAlphadigi()
    {

        _logger.LogInformation("SyncAlphadigi chamado"); //Adicione logging
        var camerasFire = await _contextFirebird.Camera.ToListAsync();
        foreach (var cameraFire in camerasFire)
        {
            var cameraSqlite = await _contextSqlite.Alphadigi.FindAsync(cameraFire.Id);
            if (cameraSqlite == null)
            {
                var camera = new Alphadigi
                {
                    Id = cameraFire.Id,
                    Ip = cameraFire.Ip,
                    Nome = cameraFire.Nome,
                    AreaId = cameraFire.IdArea,
                    Sentido = cameraFire.Direcao == "ENTRADA" ? true : false,
                    Estado = "DELETE",
                };
                _contextSqlite.Alphadigi.Add(camera);
            }
            else
            {
                _contextSqlite.Alphadigi.Update(cameraSqlite);
            }
        }
        await _contextSqlite.SaveChangesAsync();
        return true;

    }

    public async Task<List<Alphadigi>> GetAll()
    {
        _logger.LogInformation("GetAll chamado");
        return await _contextSqlite.Alphadigi.Include(c=>c.Area).ToListAsync();
    }

    public async Task<bool> Update(Alphadigi camera)
    {
        _contextSqlite.Alphadigi.Update(camera);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

    public async Task<Alphadigi> Get(string ip)
    {
        _logger.LogInformation($"Get chamado para o IP: {ip}");
        try
        {
            _logger.LogInformation($"Consultando camera no SQLite para o IP: {ip}");
            var cameraSqlite = await _contextSqlite.Alphadigi
                .Where(c => c.Ip == ip)
                .Include(a => a.Area)
                .FirstOrDefaultAsync();
            if (cameraSqlite == null)
            {
                _logger.LogWarning($"Camera não encontrada no SQLite para o IP: {ip}");
                throw new Exception("Camera não encontrada");
            }
            return cameraSqlite;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao obter camera para o IP: {ip}");
            throw;
        }
    }

    public async Task<Alphadigi> GetOrCreate(string ip)
    {
        _logger.LogInformation($"GetOrCreate chamado para o IP: {ip}");

        try
        {
            _logger.LogInformation($"Consultando camera no Firebird para o IP: {ip}");
            var cameraFire = await _contextFirebird.Camera
                .Where(c => c.Ip == ip)
                .Include(c => c.Area)
                .FirstOrDefaultAsync();

            if (cameraFire == null)
            {
                _logger.LogWarning($"Camera não encontrada no Firebird para o IP: {ip}");
                throw new Exception("Camera não encontrada");
            }

            using (var transaction = _contextSqlite.Database.BeginTransaction())
            {
                try
                {
                    _logger.LogInformation($"Consultando camera no SQLite para o IP: {ip}");
                    var cameraSqlite = await _contextSqlite.Alphadigi
                        .Where(c => c.Ip == ip)
                        .Include(a => a.Area)
                        .FirstOrDefaultAsync();

                    if (cameraSqlite == null)
                    {
                        _logger.LogInformation($"Camera não encontrada no SQLite para o IP: {ip}. Criando nova entrada.");
                        var camera = new Alphadigi
                        {
                            Ip = cameraFire.Ip,
                            Nome = cameraFire.Nome,
                            AreaId = cameraFire.IdArea,
                            Sentido = cameraFire.Direcao == "ENTRADA" ? true : false,
                            Estado = "DELETE",
                        };
                        _contextSqlite.Alphadigi.Add(camera);
                        await _contextSqlite.SaveChangesAsync();
                        await transaction.CommitAsync();
                        _logger.LogInformation($"Nova camera criada no SQLite para o IP: {ip}");
                        return camera;
                    }
                    else
                    {
                        _logger.LogInformation($"Camera encontrada no SQLite para o IP: {ip}. Verificando alterações.");
                        bool sentidoFire = cameraFire.Direcao == "ENTRADA" ? true : false;
                        if (cameraSqlite.AreaId != cameraFire.IdArea || cameraSqlite.Sentido != sentidoFire)
                        {
                            _logger.LogInformation($"Alterações detectadas na camera no SQLite para o IP: {ip}. Atualizando.");
                            cameraSqlite.AreaId = cameraFire.IdArea;
                            cameraSqlite.Sentido = sentidoFire;
                            _contextSqlite.Alphadigi.Update(cameraSqlite);
                            await _contextSqlite.SaveChangesAsync();
                            await transaction.CommitAsync();
                            _logger.LogInformation($"Camera atualizada no SQLite para o IP: {ip}");
                        }
                        else
                        {
                            _logger.LogInformation($"Nenhuma alteração detectada na camera no SQLite para o IP: {ip}");
                        }
                    }
                    await transaction.CommitAsync();
                    return cameraSqlite;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Erro ao processar a transação no SQLite para o IP: {ip}");
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao obter ou criar camera para o IP: {ip}");
            throw;
        }
    }

    public async Task<bool> UpdateLastPlate(Alphadigi camera, string plate,DateTime timestamp)
    {
        camera.UltimaPlaca = plate;
        camera.UltimaHora = timestamp;
        _contextSqlite.Alphadigi.Update(camera);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

    public async Task<bool> updateStage(string stage)
    {
        var cameras = await _contextSqlite.Alphadigi.ToListAsync();
        foreach (var camera in cameras)
        {
            camera.Estado = stage;
            _contextSqlite.Alphadigi.Update(camera);
        }
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(int id)
    {
        var camera = await _contextSqlite.Alphadigi.FindAsync(id);
        if (camera == null)
        {
            return false;
        }
        _contextSqlite.Alphadigi.Remove(camera);
        await _contextSqlite.SaveChangesAsync();
        return true;
    }

}
