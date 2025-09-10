using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Alphadigi_migration.Infrastructure.Data;
using Alphadigi_migration.Domain.EntitiesNew;
using Alphadigi_migration.Domain.DTOs.Alphadigi;
using Alphadigi_migration.Domain.Interfaces;

namespace Alphadigi_migration.Infrastructure.Repositories;

//public interface IAlphadigiRepository
//{
//    Task<Alphadigi_migration.Domain.Entities.Alphadigi> GetOrCreate(string ip);
//    Task<bool> UpdateLastPlate(Alphadigi_migration.Domain.Entities.Alphadigi camera, string plate, DateTime timestamp);
//    Task<bool> SyncAlphadigi();
//    Task<Alphadigi_migration.Domain.Entities.Alphadigi> Get(string ip);
//    Task<List<Alphadigi_migration.Domain.Entities.Alphadigi>> GetAll();
//    Task<bool> Update(UpdateAlphadigiDTO camera);
//    Task<Alphadigi_migration.Domain.Entities.Alphadigi> Update(Alphadigi_migration.Domain.Entities.Alphadigi camera);
//    Task<bool> UpdateStage(string stage);
//    Task<bool> Delete(int id);
//    Task<Alphadigi_migration.Domain.Entities.Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO);
//}

public class AlphadigiRepository : IAlphadigiRepository
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;
    private readonly ILogger<AlphadigiRepository> _logger;
    private readonly IMapper _mapper;

    public AlphadigiRepository(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird, ILogger<AlphadigiRepository> logger, IMapper mapper)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<List<SerialData>> ProcessReceivedMessageAsync(string message, string cameraIp)
    {
        _logger.LogInformation("Processando mensagem: {Message} da câmera: {CameraIp}", message, cameraIp);

        // Implemente a lógica real de processamento da mensagem aqui
        var serialDataList = new List<SerialData>();

        if (!string.IsNullOrEmpty(message))
        {
            serialDataList.Add(new SerialData
            {
                serialChannel = 1, 
                data = message,    
                dataLen = message.Length 
            });
        }

        return await Task.FromResult(serialDataList);
    }

    public async Task<bool> SyncAlphadigi()
    {
        _logger.LogInformation("####--Sincronizando todas as cameras--####");
        try
        {
            var camerasFire = await _contextFirebird.Camera.ToListAsync();
            foreach (var cameraFire in camerasFire)
            {
                await SyncCamera(cameraFire);
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

    private async Task SyncCamera(Camera cameraFire)
    {
        var cameraSqlite = await _contextSqlite.Alphadigi.FindAsync(cameraFire.Id);
        if (cameraSqlite == null)
        {
           
            var camera = new Alphadigi_migration.Domain.EntitiesNew.Alphadigi(
                ip: cameraFire.Ip,
                nome: cameraFire.Nome,
                areaId: cameraFire.IdArea,
                sentido: cameraFire.Direcao == "ENTRADA"
            );
        }
        else
        {
            _contextSqlite.Alphadigi.Update(cameraSqlite);
        }
    }

    public async Task<List<Alphadigi_migration.Domain.EntitiesNew.Alphadigi>> GetAll()
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

    public async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> Create(CreateAlphadigiDTO alphadigiDTO)
    {
        try
        {
            var newAlphadigi = _mapper.Map<Alphadigi_migration.Domain.EntitiesNew.Alphadigi>(alphadigiDTO);
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

            var camera = _mapper.Map<Alphadigi_migration.Domain.EntitiesNew.Alphadigi>(alphadigi);

            _contextSqlite.Entry(existingCamera).CurrentValues.SetValues(camera);

            SetUnmodifiedProperties(existingCamera, camera);

            await _contextSqlite.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar câmera");
            return false;
        }
    }
    public async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> Update(Alphadigi_migration.Domain.EntitiesNew.Alphadigi camera)
    {
        try
        {
            _contextSqlite.Update(camera);
            await _contextSqlite.SaveChangesAsync();
            return camera;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar câmera");
            throw;
        }
    }


    private void SetUnmodifiedProperties(Alphadigi_migration.Domain.EntitiesNew.Alphadigi existingCamera, Alphadigi_migration.Domain.EntitiesNew.Alphadigi camera)
    {
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
    }

    public async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> Get(string ip)
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

    public async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> GetOrCreate(string ip)
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
                        return await CreateNewCamera(ip, cameraFire, transaction);
                    }
                    else
                    {
                        return await UpdateExistingCamera(ip, cameraFire, cameraSqlite, transaction);
                    }
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

    private async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> CreateNewCamera(string ip, 
                                                                                         Camera cameraFire, 
                                                                                         IDbContextTransaction transaction)
    {
        _logger.LogInformation($"Camera não encontrada no SQLite para o IP: {ip}. Criando nova entrada.");

        var camera = new Alphadigi_migration.Domain.EntitiesNew.Alphadigi(
        ip: cameraFire.Ip,
        nome: cameraFire.Nome,
        areaId: cameraFire.IdArea,
        sentido: cameraFire.Direcao == "ENTRADA",
        linhasDisplay: 2 
    );
        _contextSqlite.Alphadigi.Add(camera);
        await _contextSqlite.SaveChangesAsync();
        await transaction.CommitAsync();
        _logger.LogInformation($"Nova camera criada no SQLite para o IP: {ip}");
        return camera;
    }

    private async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> UpdateExistingCamera(string ip, Camera cameraFire,
                                                                                           Alphadigi_migration.Domain.EntitiesNew.Alphadigi cameraSqlite, 
                                                                                           IDbContextTransaction transaction)
    {
        bool sentidoFire = cameraFire.Direcao == "ENTRADA";
        if (cameraSqlite.AreaId != cameraFire.IdArea || cameraSqlite.Sentido != sentidoFire)
        {
            _logger.LogInformation($"Alterações detectadas na camera no SQLite para o IP: {ip}. Atualizando.");

            cameraSqlite.AtualizarDadosBasicos(cameraFire.IdArea, sentidoFire);

            _contextSqlite.Alphadigi.Update(cameraSqlite);
            await _contextSqlite.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation($"Camera atualizada no SQLite para o IP: {ip}");
        }
        await transaction.CommitAsync();
        return cameraSqlite;
    }

    public async Task<bool> UpdateLastPlate(Alphadigi_migration.Domain.EntitiesNew.Alphadigi camera, string plate, DateTime timestamp)
    {
        try
        {
            camera.AtualizarUltimaPlaca(plate, timestamp);
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

    public async Task<bool> UpdateStage(string stage)
    {
        try
        {
            var cameras = await _contextSqlite.Alphadigi.ToListAsync();
            foreach (var camera in cameras)
            {
                camera.AtualizarEstado(stage);
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

    public async Task<Alphadigi_migration.Domain.EntitiesNew.Alphadigi> GetById(int id)
    {
        return await _contextSqlite.Alphadigi
            .Include(a => a.Area)
            .FirstOrDefaultAsync(a => a.Id == id);
    }



}
