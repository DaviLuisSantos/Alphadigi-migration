using MediatR;

namespace Alphadigi_migration.Application.Queries.PlacaLida;

public class GetDatePlateQuery : IRequest<List<Domain.EntitiesNew.PlacaLida>>
{
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public string Placa { get; set; }
    public int? AlphadigiId { get; set; }
    public int? AreaId { get; set; }
    public bool? Liberado { get; set; }
    public bool? Processado { get; set; }
}