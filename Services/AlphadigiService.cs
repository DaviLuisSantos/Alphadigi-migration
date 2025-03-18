using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Data;
using Microsoft.Extensions.Logging;
using Alphadigi_migration.DTO.Alphadigi;
using AutoMapper;

namespace Alphadigi_migration.Services;

public interface IAlphadigiService
{
    Task<Alphadigi> GetOrCreate(string ip);
    Task<bool> UpdateLastPlate(Alphadigi camera, string plate, DateTime timestamp);
    Task<bool> SyncAlphadigi();
    Task<Alphadigi> Get(string ip);
    Task<List<Alphadigi>> GetAll();
    Task<bool> Update(UpdateAlphadigiDTO camera);
    Task<bool> updateStage(string stage);
    Task<bool> Delete(int id);
    Task<Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO);
}

public class AlphadigiService : IAlphadigiService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<AlphadigiService> _logger;
    private readonly IMapper _mapper;

    public AlphadigiService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird, ILogger<AlphadigiService> logger, IMapper mapper)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<bool> SyncAlphadigi()
    {
        _logger.LogInformation("####--Sincronizando todas as cameras--####");
        try
        {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao sincronizar câmeras");
            return false;
        }
    }

    public async Task<List<Alphadigi>> GetAll()
    {
        try
        {
            return await _contextSqlite.Alphadigi.Include(c => c.Area).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todas as câmeras");
            throw;
        }
    }

    public async Task<Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO)
    {
        try
        {
            var newAlphadigi = _mapper.Map<Alphadigi>(alphadigiDTO);
            _contextSqlite.Alphadigi.Add(newAlphadigi);
            await _contextSqlite.SaveChangesAsync();
            return newAlphadigi;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar nova câmera");
            throw;
        }
    }

    public async Task<bool> Update(UpdateAlphadigiDTO alphadigi)
    {
        try
        {
            var existingCamera = await _contextSqlite.Alphadigi.FindAsync(alphadigi.Id);

            if (existingCamera == null)
            {
                return false;
            }

            var camera = _mapper.Map<Alphadigi>(alphadigi);

            _contextSqlite.Entry(existingCamera).CurrentValues.SetValues(camera);

            foreach (var property in _contextSqlite.Entry(camera).Properties)
            {
                var currentValue = property.CurrentValue;
                var propertyType = property.Metadata.ClrType;

                if (currentValue == null ||
                    (propertyType == typeof(int) && (int)currentValue == 0) ||
                    (propertyType == typeof(double) && (double)currentValue == 0.0) ||
                    (propertyType == typeof(float) && (float)currentValue == 0.0f) ||
                    (propertyType == typeof(decimal) && (decimal)currentValue == 0.0m))
                {
                    _contextSqlite.Entry(existingCamera).Property(property.Metadata.Name).IsModified = false;
                }
            }

            await _contextSqlite.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar câmera");
            return false;
        }
    }

    public async Task<Alphadigi> Get(string ip)
    {
        try
        {
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

        try
        {
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

    public async Task<bool> UpdateLastPlate(Alphadigi camera, string plate, DateTime timestamp)
    {
        try
        {
            camera.UltimaPlaca = plate;
            camera.UltimaHora = timestamp;
            _contextSqlite.Alphadigi.Update(camera);
            await _contextSqlite.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar última placa");
            return false;
        }
    }

    public async Task<bool> updateStage(string stage)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar estágio");
            return false;
        }
    }

    public async Task<bool> Delete(int id)
    {
        try
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
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Erro ao deletar câmera com ID: {id}");
            return false;
        }
    }
}
