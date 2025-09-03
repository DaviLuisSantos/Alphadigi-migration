using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alphadigi_migration.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogEndpointController : ControllerBase
{
    private readonly ILogger<LogEndpointController> _logger;
    private readonly IMediator _mediator;

    public LogEndpointController(ILogger<LogEndpointController> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
}
