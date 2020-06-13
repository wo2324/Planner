using Planner.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WpfExtensions;
using DataGridCell = System.Windows.Controls.DataGridCell;
using MessageBox = System.Windows.MessageBox;

namespace Planner
{
    /// <summary>
    /// Interaction logic for PlannerWindow.xaml
    /// </summary>
    public partial class PlannerWindow : Window
    {
        private Participant Participant;
        private Tools.Planner Planner;

        public PlannerWindow(Participant participant, Tools.Planner planner)
        {
            this.Participant = participant;
            this.Planner = planner;
            InitializeComponent();
            AdjustControls();
        }

        #region Controls adjustment

        private void AdjustControls()
        {
            try
            {
                PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;
                AdjustTaskCreationControls();
                AdjustTaskTypeListBox();
                AdjustPlannerDetailsTextBox();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        public void AdjustPlannerDataGrid()
        {
            try
            {
                DataTable taskType = DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name);
                for (int i = 0; i < PlannerDataGrid.Items.Count; i++)
                {
                    for (int j = 0; j < PlannerDataGrid.Columns.Count; j++)
                    {
                        DataGridCell dataGridCell = DataGridExtension.GetCell(PlannerDataGrid, i, j);
                        TextBlock textBlock = dataGridCell.Content as TextBlock;
                        if (String.IsNullOrEmpty(textBlock.Text))
                        {
                            dataGridCell.Foreground = GetBrush("#FF000000");
                            dataGridCell.Background = GetBrush("#FFF0F0F0");
                            continue;
                        }
                        bool match = false;
                        foreach (DataRow dataRow in taskType.Rows)
                        {
                            if (textBlock.Text == dataRow["TaskType_Name"].ToString())
                            {
                                match = true;
                                dataGridCell.Foreground = GetBrush(dataRow["TaskType_Foreground"].ToString());
                                dataGridCell.Background = GetBrush(dataRow["TaskType_Background"].ToString());
                                break;
                            }
                        }
                        if (!match)
                        {
                            textBlock.Text = null;
                            dataGridCell.Foreground = GetBrush("#FF000000");
                            dataGridCell.Background = GetBrush("#FFF0F0F0");
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AdjustTaskCreationControls()
        {
            TaskTypeNameTextBox.Clear();
            ForegroundPickerButton.Background = GetBrush("#FFDDDDDD");
            BackgroundPickerButton.Background = GetBrush("#FFDDDDDD");
        }

        private Brush GetBrush(string sample)
        {
            BrushConverter brushConverter = new BrushConverter();
            Brush brush = (Brush)brushConverter.ConvertFromString(sample);
            return brush;
        }

        private void AdjustTaskTypeListBox()
        {
            try
            {
                List<string> TasksTypes = ExtractTasksTypes(DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name));
                if (TasksTypes.Count == 0)
                {
                    TaskTypeListBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    if (TaskTypeListBox.Visibility == Visibility.Hidden)
                    {
                        TaskTypeListBox.Visibility = Visibility.Visible;
                    }
                    TaskTypeListBox.ItemsSource = TasksTypes;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> ExtractTasksTypes(DataTable sample)
        {
            List<string> TasksTypes = new List<string>();
            foreach (DataRow dataRow in sample.Rows)
            {
                TasksTypes.Add(dataRow["TaskType_Name"].ToString());
            }
            return TasksTypes;
        }

        private void AdjustPlannerDetailsTextBox()
        {
            try
            {
                string details = GetDetails(this.Participant.Name, Planner.Name);
                if (String.IsNullOrEmpty(details))
                {
                    PlannerDetailsTextBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    if (PlannerDetailsTextBox.Visibility == Visibility.Hidden)
                    {
                        PlannerDetailsTextBox.Visibility = Visibility.Visible;
                    }
                    PlannerDetailsTextBox.Text = details;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GetDetails(string participantName, string plannerName)
        {
            try
            {
                string details = "";
                List<string> TasksTypes = ExtractTasksTypes(DbAdapter.GetTasksTypes(participantName, plannerName));
                foreach (var taskType in TasksTypes)
                {
                    DataTable dataTable = DbAdapter.GetOccurrencesNumber(participantName, plannerName, taskType);
                    int occurrencesNumber = Convert.ToInt32(dataTable.Rows[0]["OccurrencesNumber"].ToString());
                    details += $"{taskType}: {this.Planner.Interval * occurrencesNumber}\n";
                }
                if (!String.IsNullOrEmpty(details))
                {
                    return details.Substring(0, details.Length - 1);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        private void PlannerDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (Planner.StartTime + Planner.Interval * e.Row.GetIndex()).ToString();
        }

        private void PlannerDataGrid_Delete(object sender, RoutedEventArgs e)
        {

            try
            {
                AssignTaskType(this.Participant.Name, this.Planner.Name, this.PlannerDataGrid.SelectedCells, null);
                AdjustPlannerDataGrid();
                AdjustPlannerDetailsTextBox();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        #region Task type creation events

        private void ForegroundPickerButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            ForegroundPickerButton.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
        }

        private void BackgroundPickerButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            BackgroundPickerButton.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
        }

        private void AddTaskTypeButton_Click(object sender, RoutedEventArgs e)
        {
            AddTaskType(this.Participant.Name, this.Planner.Name, TaskTypeNameTextBox.Text, ForegroundPickerButton.Background.ToString(), BackgroundPickerButton.Background.ToString());
            AdjustTaskCreationControls();
        }

        private void AddTaskType(string participantName, string plannerName, string taskTypeName, string foregroundSample, string backgroundSample)
        {
            if (taskTypeName.Length != 0)
            {
                if (!ExtractTasksTypes(DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name)).Contains(taskTypeName))
                {
                    try
                    {
                        DbAdapter.TaskTypeAdd(participantName, plannerName, taskTypeName, foregroundSample, backgroundSample);
                        AdjustTaskTypeListBox();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
                else
                {
                    MessageBox.Show($"Task type {taskTypeName} already exists");
                }
            }
            else
            {
                MessageBox.Show("Task type name field must be filled");
            }
        }

        #endregion

        private void TaskTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (TaskTypeListBox.SelectedItem != null)
                {
                    try
                    {
                        AssignTaskType(this.Participant.Name, this.Planner.Name, this.PlannerDataGrid.SelectedCells, TaskTypeListBox.SelectedItem.ToString());
                        AdjustPlannerDataGrid();
                        TaskTypeListBox.SelectedItem = null;
                        AdjustPlannerDetailsTextBox();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                    TaskTypeListBox.SelectedItem = null;
                }
            }
        }

        private void AssignTaskType(string participantName, string plannerName, IList<DataGridCellInfo> selectedCells, string taskTypeName)
        {
            try
            {
                string day, time;
                DataTable task = new DataTable();
                task.Columns.Add("tvp_Task_Day");
                task.Columns.Add("tvp_Task_Time");
                task.Columns.Add("tvp_Task_TaskType_Name");
                foreach (DataGridCellInfo selectedCell in selectedCells)
                {
                    day = selectedCell.Column.Header.ToString();
                    DataGridRow dataGridRow = (DataGridRow)PlannerDataGrid.ItemContainerGenerator.ContainerFromItem(selectedCell.Item);
                    time = dataGridRow.Header.ToString();
                    DataGridCell dataGridCell = DataGridExtension.GetCell(PlannerDataGrid, dataGridRow.GetIndex(), selectedCell.Column.DisplayIndex);
                    TextBlock textBlock = dataGridCell.Content as TextBlock;
                    textBlock.Text = taskTypeName;
                    task.Rows.Add(day, time, taskTypeName);
                }
                DbAdapter.EditTasks(participantName, plannerName, task);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void TaskTypeListBox_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete task type confirmation", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                try
                {
                    DbAdapter.DeleteTaskType(this.Participant.Name, this.Planner.Name, TaskTypeListBox.SelectedItem.ToString());
                    AdjustPlannerDataGrid();
                    AdjustTaskTypeListBox();
                    AdjustPlannerDetailsTextBox();
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
        }
    }
}