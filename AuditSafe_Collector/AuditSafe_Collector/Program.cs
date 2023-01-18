﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;


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
        private static readonly HttpClient client = new HttpClient();
        public static void Main(string[] args)
        {

            int counter_brut = 0;
            TimeSpan delta_brut = new TimeSpan(0, 0, 2);
            DateTime buf_time_brut = DateTime.Now;
            

            List<LogData> testString = new List<LogData>();
            
            EventLog log = new EventLog("Security");
            EventLog logApp = new EventLog("Application");
            log.EnableRaisingEvents = true;
            logApp.EnableRaisingEvents = true;

             void logCheck(EventLogEntry entry)
            {
                LogData data;
                
                switch (entry.InstanceId)
                
                {
                    case 4624://Вход
                        if (entry.ReplacementStrings[8] == "2" || entry.ReplacementStrings[8] == "7" || entry.ReplacementStrings[8] == "10") {
                            if (entry.TimeWritten.Hour < 8 || entry.TimeWritten.Hour > 18)//Вход в не рабочее время
                            {
                                data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                        "Была произведенна аунтефекация пользователя в не рабочее время", "Red Alert!");
                                testString.Add(data);
                            }

                        }
                        break;
                    
                    case 4625://Брутфорс
                        if (buf_time_brut + delta_brut > entry.TimeGenerated)
                        {
                            counter_brut++;
                        }
                        else
                        {
                            
                            counter_brut = 0;
                        }

                        if (counter_brut >= 4)
                        {
                            data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                "Была совершенна попытка брутфорса пароля", "Red Alert!");
                            testString.Add(data);
                        }
                            
                        buf_time_brut = entry.TimeGenerated;
                        break;
                    
                    case 4720: //Создание нового пользователя
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Был создан новый пользователь {entry.ReplacementStrings[8]}", "Незначительно");
                        testString.Add(data);
                        break;
                    case 4732: //Изменение группы пользователя
                        if (entry.ReplacementStrings[2] == "Administrators")
                        {
                            data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                $"Новый пользователь был добавлен в административную группу", "Red Alert!");
                            testString.Add(data);
                        }
                        else
                        {
                            data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                                $"Новый пользователь был добавлен в локальную группу", "Незначительно");
                            testString.Add(data);
                        }
                        break;
                    case 4726: //Удаление пользователя
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"{entry.ReplacementStrings[0]} пользователь был удален", "Warning");
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
                            $"Пароль пользователя {entry.ReplacementStrings[0]} {text} пользователем {entry.ReplacementStrings[4]}", "Warning");
                        testString.Add(data);
                        break;
                    case 1102: //Очистка аудита
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Журнал аудита был очищен пользователем {entry.ReplacementStrings[1]}", "Red Alert!");
                        testString.Add(data);
                        break;
                    case 4719: //Политика аудита изменена

                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Политика аудита системы была изменена {entry.ReplacementStrings[1]}", "Warning");
                        testString.Add(data);
                        break;
                    case 4950:// Параметры брандмауэра изменены
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"Параметры брандмауэра изменены", "Warning");
                        testString.Add(data);
                        break;
                    case 4698:
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"В планировщик задач было добавлено новая задача.", "Warning");
                        testString.Add(data);
                        break;
                }
            }

             void logAppCheck(EventLogEntry entry)
             {
                 LogData data;

                 switch (entry.InstanceId)
                 {
                     case 11707:
                         data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                             entry.Message, "Red Alert!");
                         testString.Add(data);
                         break;
                     case 1034:
                         data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                             entry.Message, "Red Alert!");
                         testString.Add(data);
                         break;
                 }
             }

             foreach (EventLogEntry entry in log.Entries)
            {
                logCheck(entry);
            }

            foreach (EventLogEntry entry in logApp.Entries)
            {
               logAppCheck(entry);
            }


            void LogEntry(object source, EntryWrittenEventArgs e)
            {
                logCheck(e.Entry);
                UpdateData();
            }
            void LogAppEntry(object source, EntryWrittenEventArgs e)
            {
                logAppCheck(e.Entry);
                UpdateData();
            }

            log.EntryWritten += new EntryWrittenEventHandler(LogEntry);
            logApp.EntryWritten += new EntryWrittenEventHandler(LogAppEntry);

            UpdateData();
            
            async void UpdateData()
            {
                foreach (var item in testString)
                {

                    string domain;
                    
                    if (item.pcName != String.Empty)
                    {
                        domain = item.pcName;
                        Console.WriteLine(domain);
                    }
                    else
                    {
                        domain = "";
                        Console.WriteLine("EMPTY");

                    }
                    
                    Console.WriteLine(item.message);
                    
                    Dictionary<string, string> data = new Dictionary<string, string>
                    {
                        ["domain"] = domain,
                        ["user"] = item.userName,
                        ["message"] = item.message,
                        ["date"] = item.time.ToString(),
                        ["type"] = item.type
                    };

                    HttpContent content = JsonContent.Create(data);
                    HttpResponseMessage response = await client.PostAsync("http://192.168.1.68:5000/api/data", content);
                    string responceText = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responceText);

                }
            }
            
            Console.ReadLine();

            
        }
    }
}