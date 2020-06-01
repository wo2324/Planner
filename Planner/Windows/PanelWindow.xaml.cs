using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Planner.Utils;
using Planner.Windows;

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

        #region Controls adjustment

        private void AdjustControls()
        {
            AdjustPlannerListBox();
            AdjustNewPlannerCustomizationControls();
            AdjustParticipantLabel();
        }

        private void AdjustPlannerListBox()
        {
            List<string> PlannerList = DbAdapter.ExtractPlannersNamesList(DbAdapter.GetPlannersNames(this.Participant.Id));
            if (PlannerList.Count == 0)
            {
                PlannerListBox.Visibility = Visibility.Hidden;
            }
            else
            {
                PlannerListBox.ItemsSource = PlannerList;
            }
        }

        private void AdjustNewPlannerCustomizationControls()
        {
            AdjustFirstDayComboBox();
            AdjustIncludedDaysListBox();
            AdjustNewPlannerStartHourTextBox();
            AdjustNewPlannerStopHourTextBox();
            AdjustNewPlannerTimeSpanTextBox();
        }

        private void AdjustFirstDayComboBox()
        {
            List<DayOfWeek> WeekDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList();
            ChangeWeekDaysOrder(WeekDays);
            FirstDayComboBox.ItemsSource = WeekDays;
            FirstDayComboBox.SelectedIndex = 0;
        }

        private void ChangeWeekDaysOrder(List<DayOfWeek> WeekDays)
        {
            WeekDays.Add(WeekDays[0]);
            WeekDays.RemoveAt(0);
        }

        private void AdjustIncludedDaysListBox()
        {
            List<DayOfWeek> WeekDays = Enum.GetValues(typeof(DayOfWeek)).OfType<DayOfWeek>().ToList();
            ChangeWeekDaysOrder(WeekDays);
            IncludedDaysListBox.ItemsSource = WeekDays;
            IncludedDaysListBox.SelectAll();
        }
        private void AdjustNewPlannerStartHourTextBox()
        {
            ClockTime clockTime = new ClockTime(5, 0);
            NewPlannerStartTimeTextBox.Text = clockTime.ToString();
        }
        private void AdjustNewPlannerStopHourTextBox()
        {
            ClockTime clockTime = new ClockTime(0, 0);
            NewPlannerStopTimeTextBox.Text = clockTime.ToString();
        }
        private void AdjustNewPlannerTimeSpanTextBox()
        {
            ClockTime clockTime = new ClockTime(0, 15);
            NewPlannerIntervalTextBox.Text = clockTime.ToString();
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
                    PlannerWindow plannerWindow = new PlannerWindow(GetPlanner(this.Participant.Id, PlannerListBox.SelectedItem.ToString()));
                    plannerWindow.Show();
                    plannerWindow.ColorPlanner();

                    PlannerListBox.SelectedItem = null;
                }
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

        private void OpenPlanner(int participantId, string plannerName)
        {
            PlannerWindow plannerWindow = new PlannerWindow(GetPlanner(this.Participant.Id, NewPlannerNameTextBox.Text));
            plannerWindow.Show();
        }

        #endregion

        #region PlannerListBox ContextMenu handle

        private void MenuItem_Click_Copy(object sender, RoutedEventArgs e)
        {
            string plannerCopyName = GetPlannerCopyName(PlannerListBox.SelectedItem.ToString());
            CopyPlanner(this.Participant.Id, PlannerListBox.SelectedItem.ToString(), plannerCopyName);
            AdjustPlannerListBox();
        }

        private string GetPlannerCopyName(string plannerName)
        {
            string plannerCopyName = $"{plannerName} - copy";
            int counter = 1;
            do
            {
                if (DbAdapter.ExtractPlannersNamesList(DbAdapter.GetPlannersNames(this.Participant.Id)).Contains(plannerCopyName))
                {
                    plannerCopyName = $"{plannerName} - copy ({++counter})";
                }
                else
                {
                    return plannerCopyName;
                }
            } while (true);
        }

        private void CopyPlanner(int participantId, string plannerName, string plannerCopyName)
        {
            DbAdapter.CopyPlanner(participantId, plannerName, plannerCopyName);
        }

        private void MenuItem_Click_Edit(object sender, RoutedEventArgs e)
        {
            EditPlannerWindow editPlannerWindow = new EditPlannerWindow(this.Participant, PlannerListBox.SelectedItem.ToString(), AdjustPlannerListBox);
            editPlannerWindow.ShowDialog();
        }

        private void MenuItem_Click_Delete(object sender, RoutedEventArgs e)
        {
            DeletePlanner(this.Participant.Id, PlannerListBox.SelectedItem.ToString());
            AdjustPlannerListBox();
        }

        private void DeletePlanner(int participantId, string plannerName)
        {
            DbAdapter.DeletePlanner(participantId, plannerName);
        }

        #endregion

        #region Create Planner

        private void CreatePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPlannerNameTextBox.Text.Length == 0 || NewPlannerStartTimeTextBox.Text.Length == 0 || NewPlannerStopTimeTextBox.Text.Length == 0 || NewPlannerIntervalTextBox.Text.Length == 0)
            {
                if (DbAdapter.ExtractPlannersNamesList(DbAdapter.GetPlannersNames(this.Participant.Id)).Contains(NewPlannerNameTextBox.Text))
                {
                    ClockTime clockStartTime, clockStopTime;
                    ClockTimeInterval interval;
                    if (IsTimeFormatCorrect(NewPlannerStartTimeTextBox.Text, out clockStartTime) && IsTimeFormatCorrect(NewPlannerStopTimeTextBox.Text, out clockStopTime) && IsTimeFormatCorrect(NewPlannerIntervalTextBox.Text, out interval))
                    {
                        if (IsTimeOverlap(clockStartTime, clockStopTime, interval))
                        {
                            try
                            {
                                CreatePlanner(this.Participant.Id, NewPlannerNameTextBox.Text, (DayOfWeek)Enum.Parse(typeof(DayOfWeek), FirstDayComboBox.Text), GetListBoxSelectedItems(IncludedDaysListBox), clockStartTime, clockStopTime, interval);
                                AdjustPlannerListBox();
                                OpenPlanner(this.Participant.Id, NewPlannerNameTextBox.Text);
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
                    MessageBox.Show($"Planner {NewPlannerNameTextBox.Text} already exists");
                }
            }
            else
            {
                MessageBox.Show("All fields must be non-empty");
            }

            NewPlannerNameTextBox.Clear();
            AdjustNewPlannerCustomizationControls();
        }

        private bool IsTimeFormatCorrect(string timeExpression, out ClockTime clockTime)
        {
            if (timeExpression.Length == 5)
            {
                if (timeExpression[2] == ':')
                {   //walidacja na liczby w Substring(0, 2) i Substring(3, 2)?
                    int hourExpression = Convert.ToInt32(timeExpression.Substring(0, 2));
                    int minuteExpression = Convert.ToInt32(timeExpression.Substring(3, 2));
                    if (hourExpression >= 0 && hourExpression <= 23 && minuteExpression >= 0 && minuteExpression <= 59)
                    {
                        clockTime = new ClockTime(hourExpression, minuteExpression);
                        return true;
                    }
                }
            }
            clockTime = null;
            return false;
        }

        private bool IsTimeFormatCorrect(string intervalExpression, out ClockTimeInterval interval)
        {
            if (intervalExpression.Length == 5)
            {
                if (intervalExpression[2] == ':')
                {   //walidacja na liczby w Substring(0, 2) i Substring(3, 2)?
                    int hourExpression = Convert.ToInt32(intervalExpression.Substring(0, 2));
                    int minuteExpression = Convert.ToInt32(intervalExpression.Substring(3, 2));
                    if (hourExpression >= 0 && minuteExpression >= 0 && minuteExpression <= 59)
                    {
                        interval = new ClockTimeInterval(hourExpression, minuteExpression);
                        return true;
                    }
                }
            }
            interval = null;
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

        private List<DayOfWeek> GetListBoxSelectedItems(ListBox listBox)
        {
            List<DayOfWeek> ListBoxSelectedItems = new List<DayOfWeek>();
            foreach (var item in listBox.SelectedItems)
            {
                ListBoxSelectedItems.Add((DayOfWeek)item);
            }
            return ListBoxSelectedItems;
        }

        private void CreatePlanner(int participantId, string plannerName, DayOfWeek firstDay, List<DayOfWeek> includedDays, ClockTime startHour, ClockTime stopHour, ClockTimeInterval timeSpan)
        {
            Utils.Planner planner = new Utils.Planner(plannerName, firstDay, startHour, stopHour, timeSpan);
            DataTable plannerTasks = GeneratePlannerTasks(planner, includedDays);
            CreatePlanner(participantId, planner, plannerTasks);
        }

        private DataTable GeneratePlannerTasks(Utils.Planner planner, List<DayOfWeek> includedDays)
        {
            DataTable plannerTasks = DbAdapter.GetPlannerTasksDataTable();
            ClockTime clockTime = planner.StopTime;
            foreach (var day in includedDays)
            {
                while (clockTime != planner.StartTime)
                {
                    plannerTasks.Rows.Add(null, null, day, clockTime.ToString(), null);
                    clockTime.AddInterval(planner.Interval);
                }
            }
            return plannerTasks;
        }

        private void CreatePlanner(int participantId, Utils.Planner planner, DataTable plannerTasks)
        {
            DbAdapter.CreatePlanner(participantId, planner.PlannerName, planner.FirstDayX.ToString(), planner.StartTime.ToString(), planner.StopTime.ToString(), planner.Interval.ToString(), plannerTasks);
        }

        #endregion

        #region Sample

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

        #endregion
    }
}
