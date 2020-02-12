using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Utils
{
    public class Planner
    {
        public int PlannerId { get; }
        public string PlannerName { get; }
        public DataTable Task { get; set; }
        public string FirstDay { get; set; }
        public string StartHour { get; set; }
        public string StopHour { get; set; }
        public string TimeSpan { get; set; }

        public Planner(int PlannerId, string PlannerName, string FirstDay, string StartHour, string StopHour, string TimeSpan)
        {
            this.PlannerId = PlannerId;
            this.PlannerName = PlannerName;
            this.FirstDay = FirstDay;
            this.StartHour = StartHour;
            this.StopHour = StopHour;
            this.TimeSpan = TimeSpan;
        }

        //pozyskanie Task
        public void AssignTask()
        {
            DataTable dataTable = DbAdapter.GetPlannerTask(this.PlannerId);




            DataTable result = new DataTable("Result");

            //algorytm do ustawiania kolejności kolumn
            //result.Columns.Add("Monday", typeof(string));
            //result.Columns.Add("Tuesday", typeof(string));
            //result.Columns.Add("Wedneday", typeof(string));
            //result.Columns.Add("Thursday", typeof(string));
            //result.Columns.Add("Friday", typeof(string));
            //result.Columns.Add("Saturday", typeof(string));
            //result.Columns.Add("Sunday", typeof(string));

            MyDayOfWeek myDayOfWeek = (MyDayOfWeek)Enum.Parse(typeof(MyDayOfWeek), this.FirstDay, true);

            do
            {
                result.Columns.Add(myDayOfWeek.ToString(), typeof(string));
                if (myDayOfWeek.ToString() == "Sunday")
                {
                    myDayOfWeek = 0;
                }
                else
                {
                    myDayOfWeek++;
                }
            } while (myDayOfWeek != (MyDayOfWeek)Enum.Parse(typeof(MyDayOfWeek), this.FirstDay, true));
            //koniec tego algorytmu


            string time;
            int hour;
            int minute;

            int startHour = Int32.Parse(this.StartHour.Substring(0, 2));
            int startMinute = Int32.Parse(this.StartHour.Substring(3, 2));

            int stopHour = Int32.Parse(this.StopHour.Substring(0, 2));
            int stopMinute = Int32.Parse(this.StopHour.Substring(3, 2));

            int timeSpan = Int32.Parse(this.TimeSpan.Substring(3, 2));

            hour = startHour;
            minute = startMinute;

            while (!(hour == stopHour && minute == stopMinute))
            {
                time = $"{hour.ToString("D2")}:{minute.ToString("D2")}";
                var results =
                    from row in dataTable.AsEnumerable()
                    where row.Field<string>("Task_Time") == time
                    select row;

                TaskConverter taskConverter = new TaskConverter();

                foreach (var item in results)
                {

                    if (item["Task_Day"].ToString() == "Monday")
                    {
                        taskConverter.MondayTask = item["Task_Name"].ToString();
                    }
                    else if (item["Task_Day"].ToString() == "Tuesday")
                    {
                        taskConverter.TuesdayTask = item["Task_Name"].ToString();
                    }
                    else if (item["Task_Day"].ToString() == "Wednesday")
                    {
                        taskConverter.WednesdayTask = item["Task_Name"].ToString();
                    }
                    else if (item["Task_Day"].ToString() == "Thursday")
                    {
                        taskConverter.ThursdayTask = item["Task_Name"].ToString();
                    }
                    else if (item["Task_Day"].ToString() == "Friday")
                    {
                        taskConverter.FridayTask = item["Task_Name"].ToString();
                    }
                    else if (item["Task_Day"].ToString() == "Saturday")
                    {
                        taskConverter.SaturdayTask = item["Task_Name"].ToString();
                    }
                    else if (item["Task_Day"].ToString() == "Sunday")
                    {
                        taskConverter.SundayTask = item["Task_Name"].ToString();
                    }
                }

                result.Rows.Add(taskConverter.MondayTask, taskConverter.TuesdayTask, taskConverter.WednesdayTask, taskConverter.ThursdayTask
                        , taskConverter.FridayTask, taskConverter.SaturdayTask, taskConverter.SundayTask);

                if (minute < 60 - timeSpan)
                {
                    minute += timeSpan;
                }
                else
                {
                    if (hour != 23)
                    {
                        hour++;
                    }
                    else
                    {
                        hour = 0;
                    }
                    minute = 0;
                }
            }
            this.Task = result;
        }
    }
}
