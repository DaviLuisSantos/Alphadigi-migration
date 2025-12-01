//using Alphadigi_migration.Domain.DTOs.Alphadigi;
//using Microsoft.AspNetCore.Mvc;

//[HttpPost("testar-alpha")]
//public async Task<IActionResult> TestarAlphaDigi([FromBody] TestAlphaRequest request)
//{
//    // Gerar SerialData de teste
//    var serialData = new List<SerialData>
//    {
//        new SerialData
//        {
//            serialChannel = 0,
//            data = "TESTE_DIRETO",  // Teste com texto simples primeiro
//            dataLen = 12
//        }
//    };

//    var responses = new Dictionary<string, object>
//    {
//        // Formato 1: Com underline (provavelmente o correto)
//        ["formato1_com_underline"] = new
//        {
//            Response_AlarmInfoPlate = new
//            {
//                info = "ok",
//                serialData = serialData
//            }
//        },

//        // Formato 2: Sem underline (o que você está usando)
//        ["formato2_sem_underline"] = new
//        {
//            ResponseAlarmInfoPlate = new
//            {
//                info = "ok",
//                serialData = serialData
//            }
//        },

//        // Formato 3: Propriedades lowercase
//        ["formato3_lowercase"] = new
//        {
//            response_alarminfoplate = new
//            {
//                info = "ok",
//                serialdata = serialData
//            }
//        },

//        // Formato 4: Resposta direta (sem wrapper)
//        ["formato4_direto"] = new
//        {
//            info = "ok",
//            serialData = serialData
//        },

//        // Formato 5: Igual ao código antigo
//        ["formato5_antigo"] = new ResponsePlateDTO
//        {
//            Response_AlarmInfoPlate = new ResponseAlarmInfoPlate
//            {
//                info = "ok",
//                serialData = serialData
//            }
//        }
//    };

//    return Ok(new
//    {
//        mensagem = "Teste cada formato no AlphaDigi",
//        formatos = responses
//    });
//}