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
            DataTable dataTable = DbAdapter.GetPlanner(participantId, plannerName); //Uzyskanie plannera
            Utils.Planner planner = new Utils.Planner(Int32.Parse(dataTable.Rows[0]["Planner_Id"].ToString()), dataTable.Rows[0]["Planner_Name"].ToString(),
                dataTable.Rows[0]["Planner_FirstDay"].ToString(),
                dataTable.Rows[0]["Planner_StartHour"].ToString(), dataTable.Rows[0]["Planner_StopHour"].ToString(),
                dataTable.Rows[0]["Planner_TimeSpan"].ToString());
            return planner;
        }

        //Tworzenie plannera
        private void CreatePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlanner(this.Participant.ParticipantId, NewPlannerNameTextBox.Text, null, FirstNameComboBox.Text, NewPlannerStartHourTextBox.Text, NewPlannerStopHourTextBox.Text, NewPlannerTimeSpanTextBox.Text);
            PlannerWindow plannerWindow = new PlannerWindow(GetPlanner(this.Participant.ParticipantId, NewPlannerNameTextBox.Text));
            plannerWindow.Show();
            AdjustPlannerListBox();
            NewPlannerNameTextBox.Clear();
        }

        private void CreatePlanner(int participantId, string plannerName, string plannerDescription, string firstDay, string startHour, string stopHour, string timeSpan)
        {
            DbAdapter.PlannerAdd(participantId, plannerName, plannerDescription, firstDay, startHour, stopHour, timeSpan);
            DataTable dataTable = DbAdapter.GetPlanner(participantId, plannerName); //Uzyskanie plannera
            Utils.Planner planner = new Utils.Planner(Int32.Parse(dataTable.Rows[0]["Planner_Id"].ToString()), dataTable.Rows[0]["Planner_Name"].ToString(),
                dataTable.Rows[0]["Planner_FirstDay"].ToString(),
                dataTable.Rows[0]["Planner_StartHour"].ToString(), dataTable.Rows[0]["Planner_StopHour"].ToString(),
                dataTable.Rows[0]["Planner_TimeSpan"].ToString());
            InitializeTask(planner);
        }

        private void ToPlanner()
        {

        }

        //Liczenie czasu
        private void InitializeTask(Utils.Planner planner)
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

            int startHour = Int32.Parse(planner.StartHour.Substring(0,2));
            int startMinute = Int32.Parse(planner.StartHour.Substring(3, 2));

            int stopHour = Int32.Parse(planner.StopHour.Substring(0, 2));
            int stopMinute = Int32.Parse(planner.StopHour.Substring(3, 2));

            int timeSpan = Int32.Parse(planner.TimeSpan.Substring(3, 2));

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

            DbAdapter.TaskAdd(planner.PlannerId, task);
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.Title == "PlannerWindow")
                {
                    item.Close();
                }
            }
        }
    }
}
