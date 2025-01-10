namespace Application.Services;

public interface ITimeService
{
    DateTime Now { get; }
}

public class TimeService : ITimeService
{
    public DateTime Now => DateTime.Now;
}