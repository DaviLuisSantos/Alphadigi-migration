using MediatR;


namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class UpdateStageCommand : IRequest<bool>
{

    public string Stage { get; set; }

    public UpdateStageCommand(string stage)
    {
        Stage = stage;
    }
}
