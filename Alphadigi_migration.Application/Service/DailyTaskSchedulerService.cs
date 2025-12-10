using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Service;

public class DailyTaskSchedulerService
{
    private Timer timer;
    private Action taskToExecute;
    private readonly ILogger<DailyTaskSchedulerService> _logger;


    public DailyTaskSchedulerService(Action task, ILogger<DailyTaskSchedulerService> logger)
    {
        taskToExecute = task;

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("🏗️ DailyTaskSchedulerService criado");
    }


    public void Start()
    {
        _logger.LogInformation("🚀 Iniciando DailyTaskSchedulerService...");
        DateTime now = DateTime.Now;
        DateTime next3AM = now.Date.AddHours(3);

        _logger.LogInformation("🕐 Hora atual: {Now:HH:mm:ss}", now);
        _logger.LogInformation("📅 Próxima execução inicial: {NextRun:HH:mm:ss}", next3AM);

        if (now >= next3AM)
        {
            next3AM = next3AM.AddDays(1);
            _logger.LogInformation("⏭️ Ajustado para amanhã: {NextRun:dd/MM/yyyy HH:mm:ss}", next3AM);


        }
        TimeSpan timeUntil3AM = next3AM - now;
        _logger.LogInformation("⏳ Delay inicial: {TotalMinutes:F2} minutos", timeUntil3AM.TotalMinutes);

        timer = new Timer(ExecuteTask, null, timeUntil3AM, TimeSpan.FromDays(1));

        _logger.LogInformation("✅ Timer agendado com sucesso!");
        _logger.LogInformation("⏰ Próxima execução em: {NextRun:dd/MM/yyyy HH:mm:ss} ", next3AM);


        Console.WriteLine("✅ DailyTaskSchedulerService iniciado");
    }

    public void Stop()
    {
        _logger.LogInformation("🛑 Parando DailyTaskSchedulerService...");
        timer?.Dispose();
        _logger.LogInformation("✅ DailyTaskSchedulerService parado");
    }

    private void ExecuteTask(object state)
    {
        _logger.LogInformation("🎯 [INÍCIO] Executando tarefa agendada - {StartTime:HH:mm:ss} UTC");

        taskToExecute?.Invoke();

        _logger.LogInformation("✅ [FIM] Tarefa executada com sucesso - {EndTime:HH:mm:ss}", DateTime.Now);



        DateTime now = DateTime.Now;
        DateTime next3AM = now.Date.AddHours(3);

        _logger.LogInformation("🕐 Hora atual (UTC): {Now:HH:mm:ss}", now);
        _logger.LogInformation("📅 Próxima execução inicial: {NextRun:HH:mm:ss}", next3AM);
        if (now >= next3AM)
        {
            next3AM = next3AM.AddDays(1);
        }
        TimeSpan timeUntil3AM = next3AM - now;

       
        timer.Change(timeUntil3AM, TimeSpan.FromDays(1));
       

    }
}
