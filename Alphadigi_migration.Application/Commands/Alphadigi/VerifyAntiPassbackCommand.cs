using MediatR;


namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class VerifyAntiPassbackCommand : IRequest<bool>
{
    public Domain.EntitiesNew.Veiculo Veiculo { get; set; }
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
    public DateTime Timestamp { get; set; }

}
