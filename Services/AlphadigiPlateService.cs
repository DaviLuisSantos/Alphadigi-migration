using Alphadigi_migration.DTO.Alphadigi;

namespace Alphadigi_migration.Services;

public class AlphadigiPlateService
{
    private readonly IAlphadigiService _alphadigiService;
    private readonly IVeiculoService _veiculoService;

    public AlphadigiPlateService(IAlphadigiService alphadigiService, IVeiculoService veiculoService)
    {
        _alphadigiService = alphadigiService;
        _veiculoService = veiculoService;
    }

    public async Task<Object> ProcessPlate(ProcessPlateDTO plateReaded)
    {
        DateTime timeStamp = DateTime.Now;
        var veiculo = await _veiculoService.getByPlate(plateReaded.plate);
        return veiculo;
    }

}
