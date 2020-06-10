using Planner.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
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
            AdjustPlannerPanel(true);
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
                        DataGridCell dataGridCell = GetCell(PlannerDataGrid, i, j);
                        TextBlock TextBlock = dataGridCell.Content as TextBlock;
                        if (TextBlock.Text == null)
                        {
                            break;
                        }
                        foreach (DataRow dataRow in taskType.Rows)
                        {
                            if (TextBlock.Text == dataRow["TaskType_Name"].ToString())
                            {
                                if ((bool)dataRow["TaskType_TextVisibility"])
                                {
                                    dataGridCell.Visibility = Visibility.Visible;
                                }
                                else
                                {
                                    dataGridCell.Visibility = Visibility.Hidden;
                                }
                                dataGridCell.Background = GetBrush(dataRow["TaskType_Color"].ToString());
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

        private DataGridCell GetCell(System.Windows.Controls.DataGrid dataGrid, int row, int column)
        {
            DataGridRow rowContainer = GetRow(dataGrid, row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        private DataGridRow GetRow(System.Windows.Controls.DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        private void AdjustPlannerPanel(bool initializeCall)
        {
            try
            {
                AdjustTaskCreationControls();
                AdjustAssignedTasksListBox();
                AdjustPlannerDetailsListBox();
                if (initializeCall)
                {
                    AdjustExpanders();
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
            TextVisibilityRadioButton.IsChecked = false;
            ColorPickerButton.Background = GetBrush("#FFDDDDDD");
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

        //TODO
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

        private void AdjustExpanders()
        {
            if (AssignedTasksTypesListBox.Items.Count == 0)
            {
                CreateTaskTypeExpander.IsExpanded = true;
                AssignedTasksTypesExpander.IsExpanded = false;
            }
            else
            {
                CreateTaskTypeExpander.IsExpanded = false;
                AssignedTasksTypesExpander.IsExpanded = true;
            }
            PlannerDetailsExpander.IsExpanded = false;
        }

        #endregion

        private void PlannerDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (Planner.StartTime + Planner.Interval * e.Row.GetIndex()).ToString();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Save(this.Participant.Name, this.Planner.Name, this.Planner.StartTime, this.Planner.StopTime, this.Planner.Interval, this.Planner.Task);
        }

        private void Save(string participantName, string plannerName, ClockTime startTime, ClockTime stopTime, ClockTimeInterval interval, DataTable taskSample)
        {
            try
            {
                DataTable task = DbAdapter.GetTasksDataTable();
                ClockTime clockTime = new ClockTime(startTime.Hour, startTime.Minute);
                int number = 0;
                while (number < taskSample.Rows.Count)
                {
                    foreach (DataColumn dataColumn in taskSample.Columns)
                    {
                        task.Rows.Add(dataColumn.ColumnName, clockTime, taskSample.Rows[number++][dataColumn]);
                    }
                    clockTime.AddInterval(interval);
                }
                DbAdapter.EditTasks(participantName, plannerName, task);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void ColorPickerButton_Click(object sender, RoutedEventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            ColorPickerButton.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
        }

        private void AddTaskTypeButton_Click(object sender, RoutedEventArgs e)  //Do poprawy!   //Sprawdzić nazewnictwo kontrole TextBox
        {
            DbAdapter.TaskTypeAdd(this.Participant.Name, this.Planner.Name, TaskTypeNameTextBox.Text, (bool)TextVisibilityRadioButton.IsChecked, ColorPickerButton.Background.ToString());
            AdjustPlannerPanel(false);
        }

        private void AssignedTasksTypesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssignedTasksTypesListBox.SelectedItem != null)
            {
                string taskName = AssignedTasksTypesListBox.SelectedItem.ToString();
                DataTable dataTable = DbAdapter.GetTaskType(this.Participant.Name, this.Planner.Name, taskName);
                IList<DataGridCellInfo> selectedCells = this.PlannerDataGrid.SelectedCells;
                foreach (DataGridCellInfo cell in selectedCells)
                {
                    DataGridCell dataGridCell = (DataGridCell)cell.Column.GetCellContent(cell.Item).Parent;
                    TextBlock TextBlock = dataGridCell.Content as TextBlock;
                    TextBlock.Text = taskName;
                    TextBlock.Background = GetBrush(dataTable.Rows[0]["TaskType_Color"].ToString());
                    //if ((bool)dataTable.Rows[0]["TaskType_TextVisibility"])
                    //{
                    //    TextBlock.Visibility = Visibility.Visible;
                    //}
                    //else
                    //{
                    //    TextBlock.Visibility = Visibility.Hidden;
                    //}
                }

                AssignedTasksTypesListBox.SelectedItem = null;
            }

            AdjustPlannerDetailsListBox();
        }
    }
}