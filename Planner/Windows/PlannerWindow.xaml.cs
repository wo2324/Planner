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
            PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;
            AdjustTaskCreationControls();
            AdjustAssignedTasksListBox();
            AdjustPlannerDetailsTextBox();
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
                        if (textBlock.Text == null)
                        {
                            break;
                        }
                        foreach (DataRow dataRow in taskType.Rows)
                        {
                            if (textBlock.Text == dataRow["TaskType_Name"].ToString())
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
                    if (AssignedTasksTypesListBox.Visibility == Visibility.Hidden)
                    {
                        AssignedTasksTypesListBox.Visibility = Visibility.Visible;
                    }
                    AssignedTasksTypesListBox.ItemsSource = TasksTypes;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AdjustPlannerDetailsTextBox()
        {
            try
            {
                List<string> TasksTypes = DbAdapter.ExtractTasksTypes(DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name));
                DataTable task = DbAdapter.GetTask(this.Participant.Name, this.Planner.Name);
                List<PlannerDetails> PlannerDetails = new List<PlannerDetails>();

                for (int i = 0; i < task.Rows.Count; i++)
                {
                    for (int j = 0; j < task.Columns.Count; j++)
                    {
                        if (task.Rows[i].ItemArray[j] == null)
                        {
                            break;
                        }
                        foreach (var taskType in TasksTypes)
                        {
                            if (taskType == task.Rows[i].ItemArray[j].ToString())
                            {
                                if (PlannerDetails.Count != 0)
                                {
                                    for (int k = 0; k < PlannerDetails.Count; k++)
                                    {
                                        if (PlannerDetails[k].TaskTypeName == taskType)
                                        {
                                            PlannerDetails[k].OccurrencesNumber = 12;
                                        }
                                    }
                                    PlannerDetails.Add(new Planner.PlannerDetails(taskType));
                                }
                                else
                                {
                                    PlannerDetails.Add(new Planner.PlannerDetails(taskType));
                                }
                            }
                        }
                    }
                }

                //definiuję strukturę
                //pętla przez wszystkie komórki
                //jeżeli wartość komórki nie jest null to foreach dla każdego typu; jak się spasuje to dodaj wartość i brake
                //zbierz wyniki
                //przekaż wyniki do funkcji wypisania podsumownaia
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
                AdjustPlannerDataGrid();
                AssignedTasksTypesListBox.SelectedItem = null;
                AdjustPlannerDetailsTextBox();
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
                DataGridCell dataGridCell = DataGridExtension.GetCell(PlannerDataGrid, dataGridRow.GetIndex(), selectedCell.Column.DisplayIndex);
                TextBlock textBlock = dataGridCell.Content as TextBlock;
                textBlock.Text = taskTypeName;
                task.Rows.Add(day, time, taskTypeName);
            }
            DbAdapter.EditTasks(participantName, plannerName, task);
        }
    }

    public class PlannerDetails
    {
        public string TaskTypeName { get; }
        public int OccurrencesNumber { get; set; }

        public PlannerDetails(string taskTypeName)
        {
            TaskTypeName = taskTypeName;
            OccurrencesNumber = 1;
        }
    }
}