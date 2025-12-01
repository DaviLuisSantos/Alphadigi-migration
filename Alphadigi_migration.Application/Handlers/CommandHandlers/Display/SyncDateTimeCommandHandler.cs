using Alphadigi_migration.Application.Commands.Display;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Handlers.CommandHandlers.Display;

public class SyncDateTimeCommandHandler : IRequestHandler<SyncDateTimeCommand>
{
    private readonly ILogger<SyncDateTimeCommandHandler> _logger;

    public async Task Handle(SyncDateTimeCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.Now;

        // Formatos comuns de comando para displays
        string[] comandosDataHora = {
            $"SETTIME{now:yyyyMMddHHmmss}\r\n",
            $"TIME{now:ddMMyyyyHHmmss}\r\n",
            $"DT{now:dd/MM/yyyy HH:mm:ss}\r\n",
            $"DATE {now:dd/MM/yyyy} TIME {now:HH:mm:ss}\r\n"
        };

        foreach (var comando in comandosDataHora)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(request.IpTotem, request.Porta);
                var stream = client.GetStream();

                var bytes = Encoding.ASCII.GetBytes(comando);
                await stream.WriteAsync(bytes, 0, bytes.Length);

                _logger.LogInformation("🕒 Data/hora sincronizada: {Comando}", comando.Trim());
                return;
            }
            catch (Exception ex)
            {
                _logger.LogDebug("Tentativa falhou: {Erro}", ex.Message);
            }
        }
    }
}