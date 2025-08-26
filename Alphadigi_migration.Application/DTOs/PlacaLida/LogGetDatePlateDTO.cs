using Alphadigi_migration.Domain.Interfaces;

namespace Alphadigi_migration.Application.DTOs.PlacaLida;

public class LogGetDatePlateDTO : ILogGetDatePlateDTO
{
    public string Date { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? Search { get; set; }
}
