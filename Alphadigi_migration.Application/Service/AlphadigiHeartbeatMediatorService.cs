using Alphadigi_migration.Application.Commands.Alphadigi;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Service;

public interface IAlphadigiHeartbeatMediatorService
{
    Task<object> ProcessHeartbeatAsync(string ip);
    Task<bool> HandleDeleteReturnAsync(string ip);
    Task<bool> HandleCreateReturnAsync(string ip);
}

public class AlphadigiHeartbeatMediatorService : IAlphadigiHeartbeatMediatorService
{
    private readonly IMediator _mediator;

    public AlphadigiHeartbeatMediatorService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task<object> ProcessHeartbeatAsync(string ip) =>
        _mediator.Send(new ProcessHeartbeatCommand { Ip = ip });

    public Task<bool> HandleDeleteReturnAsync(string ip) =>
        _mediator.Send(new HandleDeleteReturnCommand { Ip = ip });

    public Task<bool> HandleCreateReturnAsync(string ip) =>
        _mediator.Send(new HandleCreateReturnCommand { Ip = ip });
}
