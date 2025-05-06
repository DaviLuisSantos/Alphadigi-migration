using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alphadigi_migration.Services;

public class DailyTaskSchedulerService
{
    private Timer timer;
    private Action taskToExecute;

    public DailyTaskSchedulerService(Action task)
    {
        taskToExecute = task;
    }

    public void Start()
    {
        // Calcula o tempo até a próxima meia-noite
        DateTime now = DateTime.Now;
        DateTime nextMidnight = now.Date.AddDays(1);
        TimeSpan timeUntilMidnight = nextMidnight - now;

        // Cria e inicia o timer
        timer = new Timer(ExecuteTask, null, timeUntilMidnight, TimeSpan.FromDays(1));
    }

    public void Stop()
    {
        timer?.Dispose();
    }

    private void ExecuteTask(object state)
    {
        // Executa a tarefa
        taskToExecute?.Invoke();

        // Recalcula e ajusta o timer para a próxima meia-noite (opcional, mas pode ser mais preciso)
        DateTime now = DateTime.Now;
        DateTime nextMidnight = now.Date.AddDays(1);
        TimeSpan timeUntilMidnight = nextMidnight - now;
        timer.Change(timeUntilMidnight, TimeSpan.FromDays(1));
    }
}
