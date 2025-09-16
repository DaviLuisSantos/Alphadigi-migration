//using Alphadigi_migration.Domain.DTOs.VehicleBroadcast;
//using Alphadigi_migration.Domain.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//[ApiController]
//[Route("api/teste")]
//public class TesteAcessoLinearController : ControllerBase
//{
//    private readonly IUdpBroadcastService _udpService;

//    public TesteAcessoLinearController(IUdpBroadcastService udpService)
//    {
//        _udpService = udpService;
//    }

//    [HttpPost("teste-acesso-linear")]
//    public async Task<IActionResult> TesteAcessoLinear()
//    {
//        try
//        {
//            var testData = new VehicleBroadcastDTO
//            {
//                Plate = "TEST1234",
//                Model = "HB20",
//                Color = "Vermelho",
//                Owner = "João Silva",
//                Access = "LIBERADO",
//                Timestamp = DateTime.Now
//            };

//            await _udpService.SendVehicleDataAsync(testData, "192.168.0.182");

//            return Ok(new
//            {
//                Success = true,
//                Message = "Teste enviado para o Acesso Linear",
//                Data = testData
//            });
//        }
//        catch (Exception ex)
//        {
//            return StatusCode(500, new
//            {
//                Success = false,
//                Error = ex.Message
//            });
//        }
//    }
//}