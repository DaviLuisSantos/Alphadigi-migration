using MediatR;


namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class ProcessHeartbeatCommand : IRequest<object>
{
    public string Ip { get; set; }

    public string Body { get; }

    public ProcessHeartbeatCommand(string ip, string body)
    {
        Ip = ip;
        Body = body;
    }
}
