using Alphadigi_migration.Application.Commands.Alphadigi;
using MediatR;

namespace Alphadigi_migration.Application.Service
{
    public interface IAlphadigiPlateMediatorService
    {
        Task<object> ProcessPlateAsync(string plate, string ip, string plateImage, string carImage, string modelo, bool isCad);
    }

    public class AlphadigiPlateMediatorService : IAlphadigiPlateMediatorService
    {
        private readonly IMediator _mediator;

        public AlphadigiPlateMediatorService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<object> ProcessPlateAsync(string plate, string ip, string plateImage, string carImage, string modelo, bool isCad)
        {
            return _mediator.Send(new ProcessPlateCommand(
              ip: ip,
              plate: plate,
              isRealPlate: false, 
              isCad: isCad,
              carImage: carImage,
              plateImage: plateImage,
              modelo: modelo
             ));    
        }
    }
}