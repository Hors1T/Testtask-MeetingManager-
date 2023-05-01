using System.Net.Mime;
using MeetingManager;


public class Program
{
    
    public static void Main(string[] args)
    {
        var meetingManager = new MeetingManager.MeetingManager();
        meetingManager.Start();
    }
}