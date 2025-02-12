using Alphadigi_migration.Data;
using System;

namespace Alphadigi_migration.Services;

public class AlphadigiHearthBeatService
{
    private readonly AppDbContextSqlite _contextSqlite;
    private readonly AppDbContextFirebird _contextFirebird;

    public AlphadigiHearthBeatService(AppDbContextSqlite contextSqlite, AppDbContextFirebird contextFirebird)
    {
        _contextSqlite = contextSqlite;
        _contextFirebird = contextFirebird;
    }

}
