using Alphadigi_migration.Application.Commands.Camera;
using Alphadigi_migration.Application.Queries.Camera;
using Alphadigi_migration.Domain.DTOs.Camera;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alphadigi_migration.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CameraController : ControllerBase
    {
        private readonly ILogger<CameraController> _logger;
        private readonly IMediator _mediator;

        public CameraController(ILogger<CameraController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            
            var cameras = await _mediator.Send(new GetAllCamerasQuery());
            return Ok(cameras);
        }

        // POST: api/camera/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCameraDTO cameraDto)
        {

            // Supondo que você tenha um command CreateCameraCommand

            var result = await _mediator.Send(new CreateCameraCommand(cameraDto));
            return CreatedAtAction(nameof(GetAll), new { id = result }, result);
        }

        // PUT: api/camera/update
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute]int id,  UpdateCameraDTO cameraDto)
        {
            var result = await _mediator.Send(new UpdateCameraCommand(id, cameraDto));

            return result ? Ok() : StatusCode(500, "Erro ao atualizar");
        }

        // DELETE: api/camera/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteCameraCommand(id));
            return result ? Ok() : StatusCode(500, "Erro ao deletar");
        }
    }
}
