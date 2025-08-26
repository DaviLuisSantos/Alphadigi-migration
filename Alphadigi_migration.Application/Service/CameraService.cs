

using Alphadigi_migration.Domain.Interfaces;

namespace Alphadigi_migration.Application.Service;

public class CameraService
{
    private readonly ICameraRepository _cameraRepository;

    public CameraService(ICameraRepository cameraRepository)
    {
        _cameraRepository = cameraRepository;
    }

}
