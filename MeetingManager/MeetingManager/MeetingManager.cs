using System.Net;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MeetingManager;

public class MeetingManager
{
    private List<Meeting> meetings;
    private DateTime date = DateTime.Today;
    private bool isClear = true;
    private bool isN = true;
    private string message;
    private Timer reminderTimer;

    public MeetingManager()
    {
        meetings = new List<Meeting>();
        reminderTimer = new Timer();
        reminderTimer.AutoReset = true;
        reminderTimer.Interval = 5000;
        reminderTimer.Elapsed += OnReminderTimerElapsed;
        StartReminderTimer();
    }

    public void StartReminderTimer()
    {
        reminderTimer.Start();
    }

    public void StopReminderTimer()
    {
        reminderTimer.Stop();
    }

    private void OnReminderTimerElapsed(object sender, ElapsedEventArgs e)
    {
        var sb = new StringBuilder();
        foreach (var meeting in meetings)
        {
            var delta = meeting.StartDateTime - DateTime.Now;
            if (delta <= meeting.ReminderTime && delta > TimeSpan.Zero)
            {
                sb.Append(
                    $"{DateTime.Now.TimeOfDay} До встречи {meeting.Title} осталось {Convert.ToInt32(delta.TotalMinutes)} минут!\n");
            }

            GUI.Notify(sb.ToString());
        }
    }

    private void PrintMeetings(DateTime date)
    {
        var meetingsOnDate = meetings.Where(m => m.StartDateTime.Date == date.Date).ToList();

        GUI.Draw($"Встречи {date.ToShortDateString()}:", isClear = false, isN = true);

        if (meetingsOnDate.Count == 0)
        {
            GUI.Draw($"Встреч не запланировано.", isClear = false, isN = true);
            return;
        }

        foreach (var meeting in meetingsOnDate)
        {
            GUI.Draw(
                $"{meeting.Title} {meeting.StartDateTime.ToShortTimeString()} - {meeting.EndDateTime.ToShortTimeString()}\n Описание: {meeting.Description}",
                isClear = false, isN = true);
        }
    }

    private void SetStartDay(Meeting meeting)
    {
        DateTime selectedDate;
        while (true)
        {
            GUI.Draw(TextForGUI.DayForMeeting, isClear = false, isN = false);
            selectedDate = SetDay(TextForGUI.DayForMeeting);
            if (selectedDate == DateTime.Today)
            {
                meeting.StartDateTime = selectedDate.Add(TimeSpan.FromSeconds(1));
            }
            else
            {
                meeting.StartDateTime = selectedDate;
            }

            if (IsMeetingConflict(meeting, out string message))
            {
                GUI.Draw(message, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            break;
        }
    }

    private DateTime SetDay(string text)
    {
        while (true)
        {
            if (DateTime.TryParse(Console.ReadLine(), out DateTime selectDay))
            {
                return selectDay;
            }

            GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
            GUI.ClearMissingString(1);
            GUI.Draw(text, isClear = false, isN = false);
        }
    }

    private void SetStartDateTime(Meeting meeting)
    {
        bool isValid;
        while (true)
        {
            GUI.Draw("Выберите время начала встречи: ", isClear = false, isN = false);
            isValid = TimeOnly.TryParse(Console.ReadLine(), out var startTime);
            if (!isValid)
            {
                GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            meeting.StartDateTime = meeting.StartDateTime + startTime.ToTimeSpan();
            if (IsMeetingConflict(meeting, out message))
            {
                meeting.StartDateTime = meeting.StartDateTime - startTime.ToTimeSpan();
                GUI.Draw(message, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            break;
        }
    }

    private void SetEndDateTime(Meeting meeting)
    {
        while (true)
        {
            bool isValid;
            GUI.Draw("Укажите продолжительность встречи в минутах: ", isClear = false, isN = false);
            isValid = int.TryParse(Console.ReadLine(), out var minutes);
            if (!isValid || minutes <= 0)
            {
                GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            meeting.Span = TimeSpan.FromMinutes(minutes);
            meeting.EndDateTime = meeting.StartDateTime + meeting.Span;
            if (IsMeetingConflict(meeting, out message))
            {
                GUI.Draw(message, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            break;
        }
    }

    private void SetTitle(Meeting meeting)
    {
        while (true)
        {
            GUI.Draw("Укажите название встречи: ", isClear = false, isN = false);
            var title = Console.ReadLine() ?? "";
            meeting.Title = title;
            if (IsMeetingConflict(meeting, out message))
            {
                GUI.Draw(message, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            break;
        }
    }

    private void SetDescription(Meeting meeting)
    {
        GUI.Draw("Напишите заметки к встрече (не обязательно): ", isClear = false, isN = false);
        var description = Console.ReadLine() ?? "";
        meeting.Description = description;
    }

    private void AddMeeting()
    {
        GUI.Draw(Calendar.GetCalendar(date), isClear = true, isN = false);
        var meeting = new Meeting();
        SetStartDay(meeting);
        PrintMeetings(meeting.StartDateTime.Date);
        SetStartDateTime(meeting);
        SetEndDateTime(meeting);
        SetTitle(meeting);
        SetDescription(meeting);
        meetings.Add(meeting);
        GUI.Draw($"Встреча {meeting.Title} успешно добавлена!", isClear = false, isN = true);
        GUI.Draw(TextForGUI.NextStep, isClear = false, isN = false);
        GUI.Wait();
        Start();
    }

    private void CheckMeeting()
    {
        GUI.Draw(Calendar.GetCalendar(date) + TextForGUI.DayForMeeting, isClear = true, isN = false);
        var selectDay = SetDay(TextForGUI.DayForMeeting);
        PrintMeetings(selectDay);

        var meetingsOnSelectedDay = meetings.Where(m => m.StartDateTime.Date == selectDay.Date);

        if (meetingsOnSelectedDay.Count() == 0)
        {
            GUI.Draw(TextForGUI.NextStep, isClear = false, isN = false);
            GUI.Wait();
            Start();
        }

        while (true)
        {
            GUI.Draw(TextForGUI.UpdateAndRemove, isClear = false, isN = false);
            var option = Console.ReadLine();
            var isValid = int.TryParse(option, out var num);

            if (!isValid)
            {
                GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
                GUI.ClearMissingString(3);
                continue;
            }

            switch (num)
            {
                case 0:
                    Start();
                    break;
                case 1:
                    UpdateMeeting(selectDay);
                    break;
                case 2:
                    RemoveMeeting(selectDay);
                    break;
                default:
                    GUI.Draw(TextForGUI.MissingCase, isClear = false, isN = false);
                    GUI.ClearMissingString(3);
                    continue;
            }

            break;
        }
    }

    private Meeting Search(DateTime dateTime, string text)
    {
        while (true)
        {
            GUI.Draw(text, isClear = false, isN = false);
            var title = Console.ReadLine();
            var count = meetings.Count(meeting =>
                meeting.StartDateTime.Date == dateTime.Date && meeting.Title == title);
            if (count == 0)
            {
                continue;
            }

            return meetings.Where(meeting => meeting.StartDateTime.Date == dateTime.Date && meeting.Title == title)
                .First();
        }
    }

    private void RemoveMeeting(DateTime dateTime)
    {
        GUI.ClearMissingString(5);

        var meetingForRemove = Search(dateTime, TextForGUI.RemoveMeeting);
        meetings.Remove(meetingForRemove);

        GUI.Draw($"Удаление встречи {meetingForRemove.Title} успешно завершено", isClear = false, isN = true);
        GUI.Draw(TextForGUI.NextStep, isClear = false, isN = false);
        GUI.Wait();

        Start();
    }

    private void EnableNotification()
    {
        if (reminderTimer.Enabled)
            StopReminderTimer();
        else
            StartReminderTimer();
        GUI.Draw(reminderTimer.Enabled ? "Уведомления включены" : "Уведомления выключены", isClear = false, isN = true);
    }

    private void SetTimeToNotification(Meeting meeting)
    {
        while (true)
        {
            var isValid = int.TryParse(Console.ReadLine(), out var minutes);
            if (!isValid || minutes <= 0)
            {
                GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            meeting.ReminderTime = TimeSpan.FromMinutes(minutes);
            break;
        }
    }

    private void UpdateStartDateTime(Meeting meeting, Meeting old)
    {
        while (true)
        {
            SetStartDay(meeting);
            SetStartDateTime(meeting);
            meeting.EndDateTime = meeting.StartDateTime + meeting.Span;
            if (IsMeetingConflict(meeting, out string message))
            {
                GUI.Draw(message, isClear = false, isN = false);
                GUI.ClearMissingString(1);
                continue;
            }

            old.EndDateTime = meeting.EndDateTime;
            old.StartDateTime = meeting.StartDateTime;
            break;
        }
    }

    private void UpdateEndDateTime(Meeting meeting, Meeting old)
    {
        SetEndDateTime(meeting);
        old.EndDateTime = meeting.EndDateTime;
        old.Span = meeting.Span;
    }

    private void UpdateSuccess()
    {
        GUI.Draw("Обновление успешно", isClear = false, isN = true);
        GUI.Draw(TextForGUI.NextStep, isClear = false, isN = false);
        GUI.Wait();
    }

    private void UpdateMeeting(DateTime dateTime)
    {
        GUI.ClearMissingString(5);
        var meetingForUpdate = new Meeting();
        var original = Search(dateTime, TextForGUI.UpdateMeeting);
        meetingForUpdate.StartDateTime = original.StartDateTime;
        meetingForUpdate.EndDateTime = original.EndDateTime;
        meetingForUpdate.Span = original.Span;
        meetingForUpdate.Title = original.Title;
        meetingForUpdate.Description = original.Description;
        meetingForUpdate.ReminderTime = original.ReminderTime;
        meetingForUpdate.Id = original.Id;
        int option;
        while (true)
        {
            GUI.Draw(TextForGUI.UpdateParams, isClear = false, isN = false);
            var isValidInput = int.TryParse(Console.ReadLine(), out option);
            if (!isValidInput)
            {
                GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
                GUI.ClearMissingString(9);
                continue;
            }

            switch (option)
            {
                case 0:
                    Start();
                    break;
                case 1:
                    GUI.ClearMissingString(9);
                    UpdateStartDateTime(meetingForUpdate, original);
                    UpdateSuccess();
                    break;
                case 2:
                    GUI.ClearMissingString(9);
                    UpdateEndDateTime(meetingForUpdate, original);
                    UpdateSuccess();
                    break;
                case 3:
                    GUI.ClearMissingString(9);
                    EnableNotification();
                    UpdateSuccess();
                    break;
                case 4:
                    GUI.ClearMissingString(9);
                    SetTimeToNotification(original);
                    UpdateSuccess();
                    break;
                case 5:
                    GUI.ClearMissingString(9);
                    SetTitle(original);
                    UpdateSuccess();
                    break;
                case 6:
                    GUI.ClearMissingString(9);
                    SetDescription(original);
                    UpdateSuccess();
                    break;
                default:
                    GUI.Draw(TextForGUI.MissingCase, isClear = false, isN = false);
                    GUI.ClearMissingString(9);
                    continue;
            }

            break;
        }

        Start();
    }

    private void EditCalendar(int delta)
    {
        date = date.AddMonths(delta);
        Start();
    }

    public void Start()
    {
        while (true)
        {
            GUI.Draw(Calendar.GetCalendar(date) + TextForGUI.Main, isClear = true, isN = false);

            int option;
            var isValidInput = int.TryParse(Console.ReadLine(), out option);

            if (!isValidInput)
            {
                GUI.Draw(TextForGUI.IncorrectInput, isClear = false, isN = false);
                continue;
            }

            switch (option)
            {
                case 0:
                    Environment.Exit(0);
                    break;
                case 1:
                    AddMeeting();
                    break;
                case 2:
                    CheckMeeting();
                    break;
                case 3:
                    EditCalendar(-1);
                    break;
                case 4:
                    EditCalendar(1);
                    break;
                case 5:
                    ExportMeetings();
                    break;
                default:
                    GUI.Draw(TextForGUI.MissingCase, isClear = false, isN = false);
                    continue;
            }

            break;
        }
    }

    private void ExportMeetings()
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var fileName = $"MyMeetings.txt";
        var filePath = Path.Combine(desktopPath, fileName);
        Console.WriteLine("0");
        File.Delete(filePath);
        using (var fs = new FileStream(filePath, FileMode.OpenOrCreate))
        {
            Console.WriteLine("2");
            using (var file = new StreamWriter(fs))
            {
                file.WriteLine($"Встречи:");
                if (meetings.Count == 0)
                {
                    file.WriteLine("Встреч не запланированно.");
                }

                foreach (var meeting in meetings)
                {
                    file.WriteLine(
                        $"{meeting.Title}\n Дата{meeting.StartDateTime.ToShortDateString()}\nВремя: {meeting.StartDateTime.ToShortTimeString()} - {meeting.EndDateTime.ToShortTimeString()}\nОписание: {meeting.Description}\n");
                }

                file.Flush();
                file.Close();
            }
        }


        GUI.Draw($"Встречи экспортированы в файл - {fileName}.", isClear = false, isN = true);
        GUI.Wait();
        Start();
    }

    private bool IsMeetingConflict(Meeting newMeeting, out string message)
    {
        message = "";

        if (newMeeting.StartDateTime < DateTime.Now)
        {
            message = "Ошибка! Встреча не может быть создана в прошлом! Попробуйте снова.";
            return true;
        }

        if (newMeeting.EndDateTime - newMeeting.StartDateTime > TimeSpan.FromHours(24))
        {
            message = "Ошибка! Встреча не может быть более 24-х часов! Попробуйте снова.";
            return true;
        }

        foreach (var meeting in meetings)
        {
            if (newMeeting.Id == meeting.Id) continue;
            if (newMeeting.Title == meeting.Title && newMeeting.StartDateTime.Date == meeting.StartDateTime.Date)
            {
                message = $"Ошибка! Встреча c таким названием на эту дату уже существует!";
            }

            if (newMeeting.StartDateTime == meeting.StartDateTime ||
                newMeeting.EndDateTime == meeting.EndDateTime ||
                (newMeeting.StartDateTime > meeting.StartDateTime && newMeeting.StartDateTime < meeting.EndDateTime) ||
                (newMeeting.EndDateTime > meeting.StartDateTime && newMeeting.EndDateTime < meeting.EndDateTime)
                || newMeeting.StartDateTime == meeting.EndDateTime || newMeeting.EndDateTime == meeting.StartDateTime ||
                newMeeting.StartDateTime <= meeting.EndDateTime && meeting.StartDateTime <= newMeeting.EndDateTime)
            {
                message = $"Ошибка! Встреча накладывается на встречу {meeting.Title}! Попробуйте снова.";
                return true;
            }
        }

        return false;
    }
}