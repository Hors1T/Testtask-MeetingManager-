namespace MeetingManager;

public class Meeting
{
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public Guid Id { get; set; }
    public TimeSpan Span { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TimeSpan ReminderTime { get; set; }

    public Meeting()
    {
        ReminderTime = TimeSpan.FromMinutes(60);
        Id = Guid.NewGuid();

    }
    
}