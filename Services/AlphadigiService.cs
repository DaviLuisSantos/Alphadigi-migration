using Alphadigi_migration.Data;
using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;

namespace Alphadigi_migration.Services;

public class AlphadigiService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public AlphadigiService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

    public async Task<Alphadigi> getOrCreate(string ip)
    {
        var cameraSqlite = await _contextSqlite.Alphadigi.Where(c => c.Ip == ip).FirstOrDefaultAsync();
        if (cameraSqlite == null)
        {
            var cameraFire = await _contextFirebird.Cameras.Where(c => c.Ip == ip).FirstOrDefaultAsync() ?? throw new Exception("Camera não encontrada");

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
            return camera;
        }
        return cameraSqlite;

    }

}
