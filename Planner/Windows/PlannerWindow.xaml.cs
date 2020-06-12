using Planner.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
            AdjustPlannerDataGrid();
            AdjustTaskCreationControls();
            AdjustAssignedTasksListBox();
            AdjustPlannerDetailsListBox();
        }

        private void AdjustPlannerDataGrid()
        {
            try
            {
                PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;
                DataTable taskType = DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name);
                for (int i = 0; i < PlannerDataGrid.Items.Count; i++)
                {
                    for (int j = 0; j < PlannerDataGrid.Columns.Count; j++)
                    {
                        DataGridCell dataGridCell = DataGridExtension.GetCell(PlannerDataGrid, i, j);
                        TextBlock TextBlock = dataGridCell.Content as TextBlock;
                        if (TextBlock.Text == null)
                        {
                            break;
                        }
                        foreach (DataRow dataRow in taskType.Rows)
                        {
                            if (TextBlock.Text == dataRow["TaskType_Name"].ToString())
                            {
                                dataGridCell.Foreground = GetBrush(dataRow["TaskType_Foreground"].ToString());
                                dataGridCell.Background = GetBrush(dataRow["TaskType_Background"].ToString());
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
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

        private void AdjustAssignedTasksListBox()
        {
            try
            {
                List<string> TasksTypes = DbAdapter.ExtractTasksTypes(DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name));
                if (TasksTypes.Count == 0)
                {
                    AssignedTasksTypesListBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    AssignedTasksTypesListBox.ItemsSource = TasksTypes;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AdjustPlannerDetailsListBox()
        {
            try
            {

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
            DbAdapter.TaskTypeAdd(this.Participant.Name, this.Planner.Name, TaskTypeNameTextBox.Text, ForegroundPickerButton.Background.ToString(), BackgroundPickerButton.Background.ToString());
            AdjustTaskCreationControls();
            AdjustAssignedTasksListBox();
        }

        private void AssignedTasksTypesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssignedTasksTypesListBox.SelectedItem != null)
            {
                AssignTaskType(this.Participant.Name, this.Planner.Name, this.PlannerDataGrid.SelectedCells);
                AssignedTasksTypesListBox.SelectedItem = null;
                AdjustPlannerDetailsListBox();
            }
        }

        private void AssignTaskType(string participantName, string plannerName, IList<DataGridCellInfo> selectedCells)
        {
            string day, time, taskTypeName;
            DataTable task = new DataTable();
            task.Columns.Add("tvp_Task_Day");
            task.Columns.Add("tvp_Task_Time");
            task.Columns.Add("tvp_Task_TaskType_Name");
            foreach (DataGridCellInfo selectedCell in selectedCells)
            {
                day = selectedCell.Column.Header.ToString();
                DataGridRow dataGridRow = (DataGridRow)PlannerDataGrid.ItemContainerGenerator.ContainerFromItem(selectedCell.Item);
                time = dataGridRow.Header.ToString();
                taskTypeName = AssignedTasksTypesListBox.SelectedItem.ToString();
                task.Rows.Add(day, time, taskTypeName);
            }
            DbAdapter.EditTasks(participantName, plannerName, task);
        }
    }
}