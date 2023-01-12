using System;
using System.Collections.Generic;
using System.Diagnostics;



namespace AuditSafe_Collector
{
    class LogData
    {
        public DateTime time;
        public string pcName, userName, message, type;

        public LogData(DateTime time, string pcName, string userName, string message, string type)
        {
            this.time = time;
            this.pcName = pcName;
            this.userName = userName;
            this.message = message;
            this.type = type;
        }
    }
    
    
    internal class Program
    {
        public static void Main(string[] args)
        {

            List<LogData> testString = new List<LogData>();
            
            EventLog log = new EventLog("Security");
            log.EnableRaisingEvents = true;

            Console.WriteLine(log.Entries.Count);
            
            foreach (EventLogEntry entry in log.Entries)
            {

                LogData data;
                
                switch (entry.InstanceId)
                
                {
                    case 4624: //Вход в не рабочее время
                        if (entry.ReplacementStrings[8] == "2" || entry.ReplacementStrings[8] == "7" || entry.ReplacementStrings[8] == "10")
                        {
                            if (entry.TimeWritten.Hour < 8 || entry.TimeWritten.Hour > 18)
                            {
                                data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                    "Была произведенна аунтефекация пользователя в не рабочее время", "Warning");
                                testString.Add(data);
                            }
                            
                        }
                        break;
                    case 4720: //Создание нового пользователя
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                                            $"Был создан новый пользователь {entry.ReplacementStrings[8]}", "Ligth");
                        testString.Add(data);
                        break;
                    case 4732: //Изменение группы пользователя
                        if (entry.ReplacementStrings[2] == "Administrators")
                        {
                            data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                $"Новый пользователь был добавлен в административную группу", "red");
                            testString.Add(data);
                        }
                        else
                        {
                            data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                $"Новый пользователь был добавлен в локальную группу", "Warning");
                            testString.Add(data);
                        }
                        break;
                    case 4726: //Удаление пользователя
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"{entry.ReplacementStrings[0]} пользователь был удален", "warning");
                        testString.Add(data);
                        break;
                    case 4724: //Попытка сброса пороля
                        string text = "";
                        if (entry.EntryType.ToString() == "SuccessAudit")
                        {
                            text = "был сброшен";
                        }
                        else
                        {
                            text = "неудачно был сброшен";
                        }
                        
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Пароль пользователя {entry.ReplacementStrings[0]} {text} пользователем {entry.ReplacementStrings[4]}", "warning");
                        testString.Add(data);
                        break;
                    case 1102: //Очистка аудита
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Журнал аудита был очищен пользователем {entry.ReplacementStrings[1]}", "red");
                        testString.Add(data);
                        break;
                    case 4719: //Политика аудита изменена

                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Политика аудита системы была изменена {entry.ReplacementStrings[1]}", "Warning");
                        testString.Add(data);
                        break;
                    case 4950:// Параметры брандмауэра изменены
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Параметры брандмауэра изменены", "red");
                        testString.Add(data);
                        break;
                }
                
                
                
            }

            foreach (LogData strin in testString)
            {
                Console.WriteLine(strin.message);
            }
            Console.ReadLine();
            
        }
    }
}