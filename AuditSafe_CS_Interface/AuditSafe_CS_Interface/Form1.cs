using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace AuditSafe_CS_Interface
{
    public partial class Form1 : Form
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
        
        List<LogData> testString = new List<LogData>();
        public Form1()
        {
            InitializeComponent();
            
            int counter_brut = 0;
            TimeSpan delta_brut = new TimeSpan(0, 0, 2);
            DateTime buf_time_brut = DateTime.Now;
            

            
            
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
                                        "Была произведенна аунтефекация пользователя в не рабочее время", "Warning");
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
                                "Была совершенна попытка брутфорса пароля", "red");
                            testString.Add(data);
                        }
                            
                        buf_time_brut = entry.TimeGenerated;
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
                    case 4698:
                        data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                            $"В планировщик задач было добавлено новая задача.", "warning");
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
                             entry.Message, "red");
                         testString.Add(data);
                         break;
                     case 1034:
                         data = new LogData(entry.TimeGenerated, entry.MachineName, entry.UserName,
                             entry.Message, "red");
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
                UpdateTable();
            }
            void LogAppEntry(object source, EntryWrittenEventArgs e)
            {
                logAppCheck(e.Entry);
                UpdateTable();
            }

            log.EntryWritten += new EntryWrittenEventHandler(LogEntry);
            logApp.EntryWritten += new EntryWrittenEventHandler(LogAppEntry);

                  
        }

        void UpdateTable()
        {
            dataGridView1.Rows.Clear();

            foreach (var item in testString)
            {

                

                DataGridViewRow row = new DataGridViewRow();

                DataGridViewCell Domain = new DataGridViewTextBoxCell();
                DataGridViewCell User = new DataGridViewTextBoxCell();
                DataGridViewCell Message = new DataGridViewTextBoxCell();
                DataGridViewCell Date = new DataGridViewTextBoxCell();
                DataGridViewCell Type = new DataGridViewTextBoxCell();

                Domain.Value = item.pcName;
                User.Value = item.userName;
                Message.Value = item.message;
                Date.Value = item.time.ToString();
                Type.Value = item.type;

                row.Cells.AddRange(Domain, User, Message, Date, Type);
                dataGridView1.Rows.Add(row);

            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateTable();
        }
    }
}