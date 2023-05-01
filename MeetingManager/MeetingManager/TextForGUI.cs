namespace MeetingManager;

struct TextForGUI
{
    public const string Main = "Какое действие хотите выполнить?\n" +
                                "1.Назначить встречу\n" +
                                "2.Посмотреть встречи\n" +
                                "3.Предыдущий месяц\n" +
                                "4.Следующий месяц\n" +
                                "5.Экспортировать встречи\n"+
                                "0.Выход\n" +
                                "Введите номер действия и нажмите Enter: ";
    public const string UpdateAndRemove = "Какое действие хотите выполнить?\n" +
                               "1.Изменить встречу                                         \n" +
                               "2.Удалить встречу                                           \n" +
                               "0.На главную                                                 \n" +
                               "Введите номер действия и нажмите Enter: ";
    public const string DayForMeeting = "Выберите день для встречи (дд.мм.гггг): ";
    public const string UpdateMeeting = "Выберите встречу которую хотите изменить (Название): ";
    public const string RemoveMeeting = "Выберите встречу которую хотите удалить (Название): ";
    public const string IncorrectInput = "Ошибка некорретный ввод!";
    public const string NextStep = "Для продолжения нажмите любую клавишу...";
    public const string MissingCase = "Неверный выбор. Попробуйте еще раз.";
    public const string UpdateParams = "Что хотите обновить?\n" +
                                       "1.Дата и время начала встерчи\n"+
                                       "2.Продолжительность встречи\n"+
                                       "3.Включить/Выключить напоминание(по умолчанию вкл)\n"+
                                       "4.Изменить время напоминания(по умолчанию 60 минут до начала встречи)\n"+
                                       "5.Изменить название\n"+
                                       "6.изменить описание\n"+
                                       "0.На главную\n"+
                                       "Введите номер действия и нажмите Enter: ";
    
}
