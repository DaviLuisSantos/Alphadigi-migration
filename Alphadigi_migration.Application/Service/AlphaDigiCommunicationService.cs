//using Alphadigi_migration.Domain.Interfaces;
//using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading.Tasks;

//namespace Alphadigi_migration.Application.Service;

//public class AlphaDigiCommunicationService : IAlphaDigiCommunicationService
//{
//    private readonly IHttpClientFactory _httpClientFactory;
//    private readonly ILogger<AlphaDigiCommunicationService> _logger;
//    private readonly HttpClient _httpClient;

//    public AlphaDigiCommunicationService(
//        IHttpClientFactory httpClientFactory,
//        ILogger<AlphaDigiCommunicationService> logger)
//    {
//        _httpClientFactory = httpClientFactory;
//        _logger = logger;
//        _httpClient = _httpClientFactory.CreateClient("AlphaDigi");
//        _httpClient.Timeout = TimeSpan.FromSeconds(10);
//    }

//    public async Task<bool> SendToAlphaDigiAsync(string ip, List<Domain.DTOs.Alphadigi.SerialData> serialData)
//    {
//        if (string.IsNullOrEmpty(ip) || serialData == null || !serialData.Any())
//        {
//            _logger.LogWarning("⚠️  Nenhum dado para enviar ao AlphaDigi");
//            return false;
//        }

//        try
//        {
//            _logger.LogInformation("🌐 ENVIANDO VIA HTTP para AlphaDigi: {Ip}", ip);

//            // Construir a URL da API do AlphaDigi
//            // URL comum: http://{ip}/api/display ou http://{ip}/send
//            string baseUrl = $"http://{ip}";

//            bool todosEnviados = true;

//            foreach (var data in serialData)
//            {
//                if (string.IsNullOrEmpty(data.data))
//                    continue;

//                // Converter de Base64 para string/bytes se necessário
//                string payload = data.data;

//                _logger.LogDebug("📤 Pacote Base64: {Base64}",
//                    payload.Length > 50 ? payload.Substring(0, 50) + "..." : payload);

//                // Tentar diferentes endpoints da API AlphaDigi
//                var endpoints = new[]
//                {
//                    "/api/display",
//                    "/api/serial",
//                    "/display",
//                    "/serial",
//                    "/send",
//                    "/api/send",
//                    "/api/v1/display"
//                };

//                bool enviado = false;

//                foreach (var endpoint in endpoints)
//                {
//                    if (await TentarEnvioHttp(baseUrl + endpoint, payload, data.serialChannel))
//                    {
//                        enviado = true;
//                        break;
//                    }
//                }

//                if (!enviado)
//                {
//                    _logger.LogError("❌ Não foi possível enviar pacote (canal {Canal})", data.serialChannel);
//                    todosEnviados = false;
//                }
//                else
//                {
//                    _logger.LogInformation("✅ Pacote enviado (canal {Canal})", data.serialChannel);
//                }
//            }

//            return todosEnviados;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "💥 Erro ao enviar para AlphaDigi via HTTP");
//            return false;
//        }
//    }

//    private async Task<bool> TentarEnvioHttp(string url, string payload, int canal)
//    {
//        try
//        {
//            _logger.LogDebug("🔄 Tentando endpoint: {Url}", url);

//            // Formato comum de API AlphaDigi
//            var content = new FormUrlEncodedContent(new[]
//            {
//                new KeyValuePair<string, string>("data", payload),
//                new KeyValuePair<string, string>("channel", canal.ToString()),
//                new KeyValuePair<string, string>("type", "display")
//            });

//            // Ou como JSON
//            var jsonContent = new
//            {
//                data = payload,
//                channel = canal,
//                type = "display"
//            };

//            var jsonString = System.Text.Json.JsonSerializer.Serialize(jsonContent);
//            var jsonContent2 = new StringContent(jsonString, Encoding.UTF8, "application/json");

//            // Tentar FormUrlEncoded primeiro
//            var response = await _httpClient.PostAsync(url, content);

//            if (response.IsSuccessStatusCode)
//            {
//                var responseBody = await response.Content.ReadAsStringAsync();
//                _logger.LogInformation("✅ HTTP {StatusCode}: {Response}",
//                    response.StatusCode, responseBody);
//                return true;
//            }
//            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
//            {
//                // Tentar como JSON
//                response = await _httpClient.PostAsync(url, jsonContent2);
//                if (response.IsSuccessStatusCode)
//                {
//                    _logger.LogInformation("✅ JSON enviado com sucesso");
//                    return true;
//                }
//            }

//            _logger.LogDebug("❌ Endpoint {Url} falhou: {StatusCode}",
//                url, response.StatusCode);
//            return false;
//        }
//        catch (Exception ex)
//        {
//            _logger.LogDebug("❌ Endpoint {Url} erro: {Erro}", url, ex.Message);
//            return false;
//        }
//    }
//}