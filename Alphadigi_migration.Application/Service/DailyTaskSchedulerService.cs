using System;
using System.Threading;
using System.Threading.Tasks;

namespace Alphadigi_migration.Application.Service;

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
        DateTime now = DateTime.Now;
        DateTime next3AM = now.Date.AddHours(3);
        if (now >= next3AM)
        {
            next3AM = next3AM.AddDays(1);
        }
        TimeSpan timeUntil3AM = next3AM - now;

        timer = new Timer(ExecuteTask, null, timeUntil3AM, TimeSpan.FromDays(1));
    }

    public void Stop()
    {
        timer?.Dispose();
    }

    private void ExecuteTask(object state)
    {
        taskToExecute?.Invoke();

        DateTime now = DateTime.Now;
        DateTime next3AM = now.Date.AddHours(3);
        if (now >= next3AM)
        {
            next3AM = next3AM.AddDays(1);
        }
        TimeSpan timeUntil3AM = next3AM - now;
        timer.Change(timeUntil3AM, TimeSpan.FromDays(1));
    }
}
