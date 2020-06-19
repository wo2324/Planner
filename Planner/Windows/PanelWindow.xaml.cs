using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Planner.Tools;
using Planner.Windows;

namespace Planner
{
    /// <summary>
    /// Interaction logic for PanelWindow.xaml
    /// </summary>
    public partial class PanelWindow : Window
    {
        private Participant Participant;

        public PanelWindow(Participant participant)
        {
            this.Participant = participant;
            InitializeComponent();
            AdjustControls();
        }

        #region Controls adjustment

        private void AdjustControls()
        {
            try
            {
                AdjustPlannerListBox();
                AdjustPlannerCustomizationControls();
                AdjustParticipantLabel();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void AdjustPlannerListBox()
        {
            try
            {
                List<string> Planners = ExtractPlanners(DbAdapter.GetPlanners(this.Participant.Name));
                if (Planners.Count == 0)
                {
                    PlannerListBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    if (PlannerListBox.Visibility == Visibility.Hidden)
                    {
                        PlannerListBox.Visibility = Visibility.Visible;
                    }
                    PlannerListBox.ItemsSource = Planners;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> ExtractPlanners(DataTable dataTable)
        {
            List<string> Planners = new List<string>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                Planners.Add(dataRow["Planner_Name"].ToString());
            }
            return Planners;
        }

        private void AdjustPlannerCustomizationControls()
        {
            PlannerNameTextBox.Clear();
            AdjustFirstDayComboBox();
            IncludedDaysListBox.SelectAll();
            StartTimeTextBox.Text = "05:00";
            StopTimeTextBox.Text = "00:00";
            IntervalTextBox.Text = "00:15";
            PlannerCustomizationExpander.IsExpanded = false;
        }

        private void AdjustFirstDayComboBox()
        {
            List<DayOfWeek> WeekDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList();
            ChangeWeekDaysOrder(WeekDays);
            FirstDayComboBox.ItemsSource = WeekDays;
            FirstDayComboBox.SelectedIndex = 0;
        }

        public static void ChangeWeekDaysOrder(List<DayOfWeek> WeekDays, DayOfWeek firstDay = DayOfWeek.Monday)
        {
            WeekDays.Sort();
            int counter = WeekDays.IndexOf(firstDay);
            while (counter > 0)
            {
                WeekDays.Add(WeekDays[0]);
                WeekDays.RemoveAt(0);
                counter--;
            }
        }

        private void AdjustIncludedDaysListBox(DayOfWeek firstDay = DayOfWeek.Monday)
        {
            List<DayOfWeek> WeekDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList();
            ChangeWeekDaysOrder(WeekDays, firstDay);
            IncludedDaysListBox.ItemsSource = WeekDays;
        }

        private void AdjustParticipantLabel()
        {
            ParticipantLabel.Content = this.Participant.Name;
        }

        #endregion

        private void FirstDayComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AdjustIncludedDaysListBox((DayOfWeek)Enum.Parse(typeof(DayOfWeek), FirstDayComboBox.SelectedItem.ToString()));
        }

        #region Open Planner

        private void PlannerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (PlannerListBox.SelectedItem != null)
                {
                    OpenPlanner(this.Participant, PlannerListBox.SelectedItem.ToString());
                    PlannerListBox.SelectedItem = null;
                }
                PlannerListBox.SelectedItem = null;
            }
        }

        private void OpenPlanner(Participant participant, string plannerName)
        {
            try
            {
                PlannerWindow plannerWindow = new PlannerWindow(this.Participant, GetPlanner(participant, plannerName));
                plannerWindow.Show();
                plannerWindow.AdjustPlannerDataGrid();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private Tools.Planner GetPlanner(Participant participant, string plannerName)
        {
            try
            {
                DataTable plannerSample = DbAdapter.GetPlanner(participant.Name, plannerName);
                DayOfWeek firstDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), plannerSample.Rows[0]["Planner_FirstDay"].ToString());
                ClockTime startTime = ExtractClockTime(plannerSample.Rows[0]["Planner_StartTime"].ToString());
                ClockTime stopTime = ExtractClockTime(plannerSample.Rows[0]["Planner_StopTime"].ToString());
                ClockTimeInterval interval = ExtractClockTimeInterval(plannerSample.Rows[0]["Planner_Interval"].ToString());
                DataTable task = ExtractTask(firstDay, startTime, stopTime, interval, DbAdapter.GetTask(participant.Name, plannerName));
                return new Tools.Planner(participant, plannerName, firstDay, startTime, stopTime, interval, task);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ClockTime ExtractClockTime(string timeExpression)
        {
            int hourExpression = Convert.ToInt32(timeExpression.Substring(0, 2));
            int minuteExpression = Convert.ToInt32(timeExpression.Substring(3, 2));
            return new ClockTime(hourExpression, minuteExpression);
        }

        private ClockTimeInterval ExtractClockTimeInterval(string timeExpression)
        {
            int hourExpression = Convert.ToInt32(timeExpression.Substring(0, 2));
            int minuteExpression = Convert.ToInt32(timeExpression.Substring(3, 2));
            return new ClockTimeInterval(hourExpression, minuteExpression);
        }

        private DataTable ExtractTask(DayOfWeek firstDay, ClockTime startTime, ClockTime stopTime, ClockTimeInterval interval, DataTable taskSample)
        {
            DataTable task = new DataTable();

            #region columns definition

            List<string> IncludedDaysSample = (taskSample.AsEnumerable().Select(x => x["Task_Day"].ToString()).Distinct().ToList());
            List<DayOfWeek> IncludedDays = IncludedDaysSample.ConvertAll(new Converter<string, DayOfWeek>(StringToDayOfWeek));
            ChangeWeekDaysOrder(IncludedDays, firstDay);
            task.Columns.AddRange(IncludedDays.ConvertAll(new Converter<DayOfWeek, DataColumn>(DayOfWeekToDataColumn)).ToArray());

            #endregion

            #region table values definition

            List<object> DataRowEquivalent = new List<object>();
            ClockTime clockTime = new ClockTime(startTime.Hour, startTime.Minute);
            while (clockTime != stopTime)
            {
                DataRow[] dataRows = taskSample.Select($"Task_Time = '{clockTime.ToString()}'");
                foreach (DataColumn dataColumn in task.Columns)
                {
                    foreach (DataRow dataRow in dataRows)
                    {
                        if (dataColumn.ColumnName == dataRow["Task_Day"].ToString())
                        {
                            DataRowEquivalent.Add(dataRow["TaskType_Name"]);
                            break;
                        }
                    }
                }
                task.Rows.Add(DataRowEquivalent.ToArray());
                DataRowEquivalent.Clear();
                clockTime.AddInterval(interval);
            }

            #endregion

            return task;
        }

        private DayOfWeek StringToDayOfWeek(string sample)
        {
            return (DayOfWeek)Enum.Parse(typeof(DayOfWeek), sample);
        }

        private DataColumn DayOfWeekToDataColumn(DayOfWeek sample)
        {
            return new DataColumn(sample.ToString());
        }

        #endregion

        #region PlannerListBox ContextMenu handle

        private void PlannerListBox_Copy(object sender, RoutedEventArgs e)
        {
            try
            {
                string plannerCopyName = GeneratePlannerCopyName(PlannerListBox.SelectedItem.ToString());
                DbAdapter.CopyPlanner(this.Participant.Name, PlannerListBox.SelectedItem.ToString(), plannerCopyName);
                AdjustPlannerListBox();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private string GeneratePlannerCopyName(string plannerName)
        {
            try
            {
                string plannerCopyName = $"{plannerName} - copy";
                int counter = 1;
                do
                {
                    if (ExtractPlanners(DbAdapter.GetPlanners(this.Participant.Name)).Contains(plannerCopyName))
                    {
                        plannerCopyName = $"{plannerName} - copy ({++counter})";
                    }
                    else
                    {
                        return plannerCopyName;
                    }
                } while (true);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void PlannerListBox_Edit(object sender, RoutedEventArgs e)
        {
            EditPlannerWindow editPlannerWindow = new EditPlannerWindow(this.Participant, GetPlanner(this.Participant, PlannerListBox.SelectedItem.ToString()), AdjustPlannerListBox);
            editPlannerWindow.ShowDialog();
        }

        private void PlannerListBox_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete planner confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    DbAdapter.DeletePlanner(this.Participant.Name, PlannerListBox.SelectedItem.ToString());
                    AdjustPlannerListBox();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }

        #endregion

        #region Create Planner

        private void CreatePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlanner(this.Participant, PlannerNameTextBox.Text, (DayOfWeek)Enum.Parse(typeof(DayOfWeek), FirstDayComboBox.Text), IncludedDaysListBox.SelectedItems.Cast<DayOfWeek>().ToList(), StartTimeTextBox.Text, StopTimeTextBox.Text, IntervalTextBox.Text);
            AdjustPlannerCustomizationControls();
        }

        private void CreatePlanner(Participant participant, string plannerName, DayOfWeek firstDay, List<DayOfWeek> IncludedDays, string startTimeSample, string stopTimeSample, string intervalSample)
        {
            if (plannerName.Length != 0 && startTimeSample.Length != 0 && stopTimeSample.Length != 0 && intervalSample.Length != 0)
            {
                if (!ExtractPlanners(DbAdapter.GetPlanners(this.Participant.Name)).Contains(plannerName))
                {
                    if (IsTimeFormatCorrect(startTimeSample) && IsTimeFormatCorrect(stopTimeSample) && IsTimeFormatCorrect(intervalSample))
                    {
                        ClockTime startTime = ExtractClockTime(startTimeSample);
                        ClockTime stopTime = ExtractClockTime(stopTimeSample);
                        ClockTimeInterval interval = ExtractClockTimeInterval(intervalSample);
                        if (IsTimeOverlap(startTime, stopTime, interval))
                        {
                            try
                            {
                                DataTable task = GenerateTasks(startTime, stopTime, interval, IncludedDays);
                                DbAdapter.CreatePlanner(participant.Name, plannerName, firstDay.ToString(), startTime.ToString(), stopTime.ToString(), interval.ToString(), task);
                                AdjustPlannerListBox();
                                OpenPlanner(participant, plannerName);
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.Message);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Cannot create planner of given time values");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Time values must be expressed as hh:mm");
                    }
                }
                else
                {
                    MessageBox.Show($"Planner {plannerName} already exists");
                }
            }
            else
            {
                MessageBox.Show("All fields must be non-empty");
            }
        }

        private bool IsTimeFormatCorrect(string timeExpression)
        {
            if (timeExpression.Length == 5)
            {
                if (timeExpression[2] == ':')
                {
                    int hourExpression = Convert.ToInt32(timeExpression.Substring(0, 2));
                    int minuteExpression = Convert.ToInt32(timeExpression.Substring(3, 2));
                    if (hourExpression >= 0 && hourExpression <= 23 && minuteExpression >= 0 && minuteExpression <= 59)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsTimeOverlap(ClockTime startTime, ClockTime stopTime, ClockTimeInterval interval)
        {
            ClockTimeInterval differentialInterval = ClockTimeInterval.GetInterval(startTime, stopTime);
            while (true)
            {
                if (differentialInterval.Hour == 0 && differentialInterval.Minute == 0)
                {
                    return true;
                }
                else if (differentialInterval.Hour < 0 || differentialInterval.Minute < 0)
                {
                    return false;
                }
                differentialInterval.SubtractInterval(interval);
            }
        }

        private DataTable GenerateTasks(ClockTime startTime, ClockTime stopTime, ClockTimeInterval interval, List<DayOfWeek> IncludedDays)
        {
            DataTable task = GetTasksDataTable();
            ClockTime clockTime = new ClockTime();
            foreach (var day in IncludedDays)
            {
                clockTime.Hour = startTime.Hour;
                clockTime.Minute = startTime.Minute;
                while (clockTime != stopTime)
                {
                    task.Rows.Add(day, clockTime.ToString(), null);
                    clockTime.AddInterval(interval);
                }
            }
            return task;
        }

        public static DataTable GetTasksDataTable()
        {
            DataTable plannerTasks = new DataTable();
            plannerTasks.Columns.Add("tvp_Task_Day", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_Time", typeof(string));
            plannerTasks.Columns.Add("tvp_Task_TaskType_Id", typeof(int));
            return plannerTasks;
        }

        #endregion

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
