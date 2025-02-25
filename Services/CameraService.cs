using Alphadigi_migration.Data;

namespace Alphadigi_migration.Services;

public class CameraService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public CameraService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

}
