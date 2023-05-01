using System.Text;
using Microsoft.VisualBasic;
using static System.DateTime;

namespace MeetingManager;

public static class Calendar
{
    public static string GetCalendar(DateTime month)
    {
        var calendar = new StringBuilder();
        month = new DateTime(month.Year, month.Month, 1);
        var headingSpaces = new string(' ', 16 - month.ToString("MMMM").Length);
        calendar.Append($"{month.ToString("MMMM")}{headingSpaces}{month.Year}\n");
        calendar.Append(new string('-', 20));
        calendar.Append('\n');
        calendar.Append("Пн Вт Ср Чт Пт Сб Вс\n");

        var padLeftDays = (int)month.DayOfWeek - 1;
        var currentDay = month;
        var iterations = DaysInMonth(month.Year, month.Month) + padLeftDays;
        for (var i = 0; i < iterations; i++)
        {
            if (i < padLeftDays)
            {
                calendar.Append("   ");
            }
            else
            {
                
                calendar.Append($"{currentDay.Day.ToString().PadLeft(2, ' ')} ");
                if ((i + 1) % 7 == 0)
                {
                    calendar.Append('\n');
                }

                currentDay = currentDay.AddDays(1);
            }
        }
        calendar.Append('\n');
        return calendar.ToString();
    }
}