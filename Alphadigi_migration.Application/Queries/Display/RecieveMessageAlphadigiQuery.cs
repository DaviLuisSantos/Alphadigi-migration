using MediatR;


namespace Alphadigi_migration.Application.Queries.Display;

public class RecieveMessageAlphadigiQuery : IRequest<List<Domain.DTOs.Alphadigi.SerialData>>
{
    public string Linha1 { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }

    public string Tipo { get; set; } = "lista";
}