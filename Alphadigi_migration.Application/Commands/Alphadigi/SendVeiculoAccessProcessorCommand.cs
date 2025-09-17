using MediatR;


namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class SendVeiculoAccessProcessorCommand : IRequest<(bool ShouldReturn, string Acesso)>
{

    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    public DateTime Timestamp { get; set; }
    public Domain.EntitiesNew.PlacaLida Log { get; set; }
    public string Imagem { get; set; }


}
