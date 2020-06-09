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
            AdjustPlannerListBox();
            AdjustPlannerCustomizationControls();
            AdjustParticipantLabel();
        }

        private void AdjustPlannerListBox()
        {
            List<string> Planners = DbAdapter.ExtractPlanners(DbAdapter.GetPlanners(this.Participant.Name));
            if (Planners.Count == 0)
            {
                PlannerListBox.Visibility = Visibility.Hidden;
            }
            else
            {
                PlannerListBox.ItemsSource = Planners;
            }
        }

        private void AdjustPlannerCustomizationControls()
        {
            PlannerNameTextBox.Clear();
            AdjustFirstDayComboBox();
            AdjustIncludedDaysListBox();
            AdjustStartHourTextBox();
            AdjustStopHourTextBox();
            AdjustTimeSpanTextBox();
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
            int index = WeekDays.IndexOf(firstDay);
            while (index > 0)
            {
                WeekDays.Add(WeekDays[index - 1]);
                WeekDays.RemoveAt(index - 1);
                index--;
            }
        }

        private void AdjustIncludedDaysListBox()
        {
            List<DayOfWeek> WeekDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList();
            ChangeWeekDaysOrder(WeekDays);
            IncludedDaysListBox.ItemsSource = WeekDays;
            IncludedDaysListBox.SelectAll();
        }
        private void AdjustStartHourTextBox()
        {
            ClockTime clockTime = new ClockTime(5, 0);
            StartTimeTextBox.Text = clockTime.ToString();
        }
        private void AdjustStopHourTextBox()
        {
            ClockTime clockTime = new ClockTime(0, 0);
            StopTimeTextBox.Text = clockTime.ToString();
        }
        private void AdjustTimeSpanTextBox()
        {
            ClockTime clockTime = new ClockTime(0, 15);
            IntervalTextBox.Text = clockTime.ToString();
        }

        private void AdjustParticipantLabel()
        {
            ParticipantLabel.Content = this.Participant.Name;
        }

        #endregion

        #region Open Planner

        private void PlannerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (PlannerListBox.SelectedItem != null)
                {
                    OpenPlanner(this.Participant.Name, PlannerListBox.SelectedItem.ToString());
                    PlannerListBox.SelectedItem = null;
                }
            }
        }

        private void OpenPlanner(string participantName, string plannerName)
        {
            PlannerWindow plannerWindow = new PlannerWindow(this.Participant, GetPlanner(this.Participant, plannerName));
            plannerWindow.Show();
        }

        private Tools.Planner GetPlanner(Participant participant, string plannerName)
        {
            DataTable dataTable = DbAdapter.GetPlanner(participant.Name, plannerName);
            DayOfWeek firstDay = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), dataTable.Rows[0]["Planner_FirstDay"].ToString());
            ClockTime startTime = ExtractClockTime(dataTable.Rows[0]["Planner_StartTime"].ToString());
            ClockTime stopTime = ExtractClockTime(dataTable.Rows[0]["Planner_StopTime"].ToString());
            ClockTimeInterval interval = ExtractClockTimeInterval(dataTable.Rows[0]["Planner_Interval"].ToString());
            DataTable task = ExtractTask(firstDay, startTime, stopTime, interval, DbAdapter.GetTask(participant.Name, plannerName));
            Tools.Planner planner = new Tools.Planner(participant, plannerName, firstDay, startTime, stopTime, interval, task);
            return planner;
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
            DataTable task = new DataTable("Task");

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
                            DataRowEquivalent.Add(dataRow["Task_TaskType_Id"]);
                            break;
                        }
                    }
                    task.Rows.Add(DataRowEquivalent.ToArray());
                    DataRowEquivalent.Clear();
                }
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

        private void MenuItem_Click_Copy(object sender, RoutedEventArgs e)
        {
            string plannerCopyName = GeneratePlannerCopyName(PlannerListBox.SelectedItem.ToString());
            CopyPlanner(this.Participant.Name, PlannerListBox.SelectedItem.ToString(), plannerCopyName);
            AdjustPlannerListBox();
        }

        private string GeneratePlannerCopyName(string plannerName)
        {
            string plannerCopyName = $"{plannerName} - copy";
            int counter = 1;
            do
            {
                if (DbAdapter.ExtractPlanners(DbAdapter.GetPlanners(this.Participant.Name)).Contains(plannerCopyName))
                {
                    plannerCopyName = $"{plannerName} - copy ({++counter})";
                }
                else
                {
                    return plannerCopyName;
                }
            } while (true);
        }

        private void CopyPlanner(string participantName, string plannerName, string plannerCopyName)
        {
            DbAdapter.CopyPlanner(participantName, plannerName, plannerCopyName);
        }

        private void MenuItem_Click_Edit(object sender, RoutedEventArgs e)
        {
            EditPlannerWindow editPlannerWindow = new EditPlannerWindow(this.Participant, GetPlanner(this.Participant, PlannerListBox.SelectedItem.ToString()), AdjustPlannerListBox);
            editPlannerWindow.ShowDialog();
        }

        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            DeletePlanner(this.Participant.Name, PlannerListBox.SelectedItem.ToString());
            AdjustPlannerListBox();
        }

        private void DeletePlanner(string participantName, string plannerName)
        {
            DbAdapter.DeletePlanner(participantName, plannerName);
        }

        #endregion

        #region Create Planner

        private void CreatePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            CreatePlanner();
            AdjustPlannerCustomizationControls();
        }

        private void CreatePlanner()
        {
            if (PlannerNameTextBox.Text.Length != 0 && StartTimeTextBox.Text.Length != 0 && StopTimeTextBox.Text.Length != 0 && IntervalTextBox.Text.Length != 0)
            {
                if (!DbAdapter.ExtractPlanners(DbAdapter.GetPlanners(this.Participant.Name)).Contains(PlannerNameTextBox.Text))
                {
                    if (IsTimeFormatCorrect(StartTimeTextBox.Text) && IsTimeFormatCorrect(StopTimeTextBox.Text) && IsTimeFormatCorrect(IntervalTextBox.Text))
                    {
                        ClockTime clockStartTime = ExtractClockTime(StartTimeTextBox.Text);
                        ClockTime clockStopTime = ExtractClockTime(StopTimeTextBox.Text);
                        ClockTimeInterval interval = ExtractClockTimeInterval(IntervalTextBox.Text);
                        if (IsTimeOverlap(clockStartTime, clockStopTime, interval))
                        {
                            try
                            {
                                CreatePlanner(this.Participant.Name, PlannerNameTextBox.Text, (DayOfWeek)Enum.Parse(typeof(DayOfWeek), FirstDayComboBox.Text), ExtractIncludedDays(IncludedDaysListBox), clockStartTime, clockStopTime, interval);
                                AdjustPlannerListBox();
                                OpenPlanner(this.Participant.Name, PlannerNameTextBox.Text);
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
                    MessageBox.Show($"Planner {PlannerNameTextBox.Text} already exists");
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

        private bool IsTimeOverlap(ClockTime clockStartTime, ClockTime clockStopTime, ClockTimeInterval interval)
        {
            ClockTimeInterval differentialInterval = ClockTimeInterval.GetInterval(clockStartTime, clockStopTime);
            while (true)
            {
                if (differentialInterval.Hour == 0 && differentialInterval.Minute == 0)
                {
                    return true;
                }
                try
                {
                    differentialInterval.SubtractInterval(interval);
                }
                catch (NegativeIntervalException negativeIntervalException)
                {
                    return false;
                }
            }
        }

        private List<DayOfWeek> ExtractIncludedDays(ListBox listBox)
        {
            List<DayOfWeek> IncludedDays = new List<DayOfWeek>();
            foreach (var item in listBox.SelectedItems)
            {
                IncludedDays.Add((DayOfWeek)item);
            }
            return IncludedDays;
        }

        private void CreatePlanner(string participantName, string plannerName, DayOfWeek firstDay, List<DayOfWeek> includedDays, ClockTime startHour, ClockTime stopHour, ClockTimeInterval timeSpan)
        {
            Tools.Planner planner = new Tools.Planner(this.Participant, plannerName, firstDay, startHour, stopHour, timeSpan);
            DataTable Tasks = GenerateTasks(planner, includedDays);
            CreatePlanner(participantName, planner, Tasks);
        }

        private DataTable GenerateTasks(Tools.Planner planner, List<DayOfWeek> includedDays)
        {
            DataTable plannerTasks = DbAdapter.GetTasksDataTable();
            ClockTime clockTime = new ClockTime();
            foreach (var day in includedDays)
            {
                clockTime.Hour = planner.StartTime.Hour;
                clockTime.Minute = planner.StartTime.Minute;
                while (clockTime != planner.StopTime)
                {
                    plannerTasks.Rows.Add(day, clockTime.ToString(), null);
                    clockTime.AddInterval(planner.Interval);
                }
            }
            return plannerTasks;
        }

        private void CreatePlanner(string participantName, Tools.Planner planner, DataTable plannerTasks)
        {
            DbAdapter.CreatePlanner(participantName, planner.Name, planner.FirstDay.ToString(), planner.StartTime.ToString(), planner.StopTime.ToString(), planner.Interval.ToString(), plannerTasks);
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
