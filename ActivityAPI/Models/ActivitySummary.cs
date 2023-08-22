public class ActivitySummary
{
    public string Type { get; set; }
    public TimeSpan TotalElapsedTime { get; set; }
    public List<Activity> Activities { get; set; }
}
