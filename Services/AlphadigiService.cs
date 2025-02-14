﻿using Alphadigi_migration.Models;
using Microsoft.EntityFrameworkCore;
using Alphadigi_migration.Data;

namespace Alphadigi_migration.Services;

public class AlphadigiService:IAlphadigiService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public AlphadigiService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

    public async Task<Alphadigi> GetOrCreate(string ip) 
    {
        var cameraSqlite = await _contextSqlite.Alphadigi.Where(c => c.Ip == ip).FirstOrDefaultAsync();
        var cameraFire = await _contextFirebird.Cameras
    .Where(c => c.Ip == ip)
    .Include(c => c.Area)
    .FirstOrDefaultAsync() ?? throw new Exception("Camera não encontrada");

        if (cameraSqlite == null)
        {
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
        else
        {
            bool sentidoFire = cameraFire.Direcao == "ENTRADA" ? true : false;
            if (cameraSqlite.AreaId != cameraFire.IdArea || cameraSqlite.Sentido != sentidoFire)
            {
                cameraSqlite.AreaId = cameraFire.IdArea;
                cameraSqlite.Sentido = sentidoFire;
                _contextSqlite.Alphadigi.Update(cameraSqlite);
                await _contextSqlite.SaveChangesAsync();
            }
        }
        cameraSqlite.Area = cameraFire.Area;
        return cameraSqlite;
    }



}
