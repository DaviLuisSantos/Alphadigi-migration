using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alphadigi_migration.Api.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CameraEndpoint : ControllerBase
{
    private readonly ILogger<CameraEndpoint> _logger;
    private readonly IMediator _mediator;

    public CameraEndpoint(ILogger<CameraEndpoint> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }




}
