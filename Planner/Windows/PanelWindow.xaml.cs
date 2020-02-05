using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Planner.Utils;

namespace Planner
{
    /// <summary>
    /// Interaction logic for PanelWindow.xaml
    /// </summary>
    public partial class PanelWindow : Window
    {
        public Participant Participant { get; }

        public PanelWindow(Participant participant)
        {
            this.Participant = participant;
            InitializeComponent();
            AdjustControls();
        }

        private void AdjustControls()
        {
            AdjustPlannerListBox();
            AdjustParticipantLabel();
        }

        private void AdjustPlannerListBox()
        {
            PlannerListBox.ItemsSource = GetPlannerList(DbAdapter.GetPlannerList(this.Participant.ParticipantId));
        }

        private List<string> GetPlannerList(DataTable dataTable)
        {
            List<string> planner = new List<string>();
            if (dataTable == null)
            {
                PlannerListBox.Visibility = Visibility.Hidden;
            }
            else
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    planner.Add(dataRow["Planner_Name"].ToString());
                }
            }
            return planner;
        }

        private void AdjustParticipantLabel()
        {
            ParticipantLabel.Content = this.Participant.ParticipantName;
        }

        private void PlannerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (PlannerListBox.SelectedItem != null)
            {
                PlannerWindow plannerWindow = new PlannerWindow(GetPlanner(this.Participant.ParticipantId, PlannerListBox.SelectedItem.ToString()));
                plannerWindow.Show();
                plannerWindow.ColorPlanner();

                PlannerListBox.SelectedItem = null;
            }
        }

        private Utils.Planner GetPlanner(int participantId, string plannerName)
        {
            int plannerId = DbAdapter.GetPlannerId(participantId, plannerName);
            DataTable task = AdjustTask(DbAdapter.GetPlannerTask(plannerId));
            return new Utils.Planner(plannerId, plannerName, task);
        }

        private DataTable AdjustTask(DataTable dataTable)   //Do poprawy!
        {
            DataTable result = new DataTable("Result");
            result.Columns.Add("Monday", typeof(string));
            result.Columns.Add("Tuesday", typeof(string));
            result.Columns.Add("Wedneday", typeof(string));
            result.Columns.Add("Thursday", typeof(string));
            result.Columns.Add("Friday", typeof(string));
            result.Columns.Add("Saturday", typeof(string));
            result.Columns.Add("Sunday", typeof(string));

            string time;
            int hour;
            int minute;

            int startHour = 5;
            int startMinute = 0;
            int stopHour = 0;
            int stopMinute = 0;

            int timeSpan = 30;

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

            return result;
        }

        private void CreatePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlanner(this.Participant.ParticipantId, NewPlannerNameTextBox.Text, null);
            PlannerWindow plannerWindow = new PlannerWindow(GetPlanner(this.Participant.ParticipantId, NewPlannerNameTextBox.Text));
            plannerWindow.Show();
            AdjustPlannerListBox();
            NewPlannerNameTextBox.Clear();
        }

        private void CreatePlanner(int participantId, string plannerName, string plannerDescription)
        {
            DbAdapter.PlannerAdd(participantId, plannerName, plannerDescription);
            int plannerId = DbAdapter.GetPlannerId(participantId, plannerName);
            InitializeTask(plannerId);
        }

        private void InitializeTask(int plannerId)
        {
            DataTable task = new DataTable("EmptyPlanner");
            task.Columns.Add("tvp_Task_Name", typeof(string));
            task.Columns.Add("tvp_Task_Description", typeof(string));
            task.Columns.Add("tvp_Task_Day", typeof(string));
            task.Columns.Add("tvp_Task_Time", typeof(string));
            task.Columns.Add("tvp_Task_Color", typeof(string));

            DayOfWeek dayOfWeek = (DayOfWeek)0;
            string time;
            int hour;
            int minute;

            int startHour = 5;
            int startMinute = 0;

            int stopHour = 0;
            int stopMinute = 0;

            int timeSpan = 30;

            hour = startHour;
            minute = startMinute;

            while ((int)dayOfWeek < 7)
            {
                while (!(hour == stopHour && minute == stopMinute))
                {
                    time = $"{hour.ToString("D2")}:{minute.ToString("D2")}";
                    task.Rows.Add(null, null, dayOfWeek.ToString(), time, null);
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
                hour = startHour;
                minute = startMinute;
                dayOfWeek++;
            }

            DbAdapter.TaskAdd(plannerId, task);
        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.Show();
            CloseWindows();
        }

        private void CloseWindows()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.Title == "PlannerWindow" || item.Title == "PanelWindow")
                {
                    item.Close();
                }
            }
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow(this.Participant);
            settingsWindow.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
