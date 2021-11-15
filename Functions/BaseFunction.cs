namespace Demo.Function;

public class BaseFunction
{
    [FunctionName("Base_FunctionPulse")]
    public void CheckFunctionPulse([TimerTrigger("0 */10 * * * *", RunOnStartup = true)]
            TimerInfo pulseTimer)
        => Console.WriteLine($"Demo function pulse checked at: {DateTime.UtcNow}. " +
            $"Next will be at {pulseTimer.ScheduleStatus.Next.ToString("yyyy-MM-dd HH:mm")}");
}