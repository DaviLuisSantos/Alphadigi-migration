using Alphadigi_migration.Domain.DTOs.Alphadigi;
using MediatR;

namespace Alphadigi_migration.Application.Commands.Alphadigi;

public class SendDefaultDisplayCommand : IRequest<List<SerialData>>
{
    public Domain.EntitiesNew.Alphadigi Alphadigi { get; set; }
}