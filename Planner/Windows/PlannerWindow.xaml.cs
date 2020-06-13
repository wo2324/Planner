﻿using Planner.Tools;
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
            PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;
            AdjustTaskCreationControls();
            AdjustAssignedTaskTypeListBox();
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

        private void AdjustAssignedTaskTypeListBox()
        {
            try
            {
                List<string> TasksTypes = ExtractTasksTypes(DbAdapter.GetTasksTypes(this.Participant.Name, this.Planner.Name));
                if (TasksTypes.Count == 0)
                {
                    AssignedTaskTypeListBox.Visibility = Visibility.Hidden;
                }
                else
                {
                    if (AssignedTaskTypeListBox.Visibility == Visibility.Hidden)
                    {
                        AssignedTaskTypeListBox.Visibility = Visibility.Visible;
                    }
                    AssignedTaskTypeListBox.ItemsSource = TasksTypes;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<string> ExtractTasksTypes(DataTable dataTable)
        {
            List<string> TasksTypes = new List<string>();
            foreach (DataRow dataRow in dataTable.Rows)
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

        #endregion

        private void PlannerDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (Planner.StartTime + Planner.Interval * e.Row.GetIndex()).ToString();
        }

        private void MenuItem_Click_PlannerDataGrid_Delete(object sender, RoutedEventArgs e)
        {
            AssignTaskType(this.Participant.Name, this.Planner.Name, this.PlannerDataGrid.SelectedCells, null);
            AdjustPlannerDataGrid();
            AdjustPlannerDetailsTextBox();
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
            AdjustAssignedTaskTypeListBox();
        }

        private void AssignedTaskTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (AssignedTaskTypeListBox.SelectedItem != null)
                {
                    AssignTaskType(this.Participant.Name, this.Planner.Name, this.PlannerDataGrid.SelectedCells, AssignedTaskTypeListBox.SelectedItem.ToString());
                    AdjustPlannerDataGrid();
                    AssignedTaskTypeListBox.SelectedItem = null;
                    AdjustPlannerDetailsTextBox();
                }
            }
        }

        private void AssignTaskType(string participantName, string plannerName, IList<DataGridCellInfo> selectedCells, string taskTypeName)
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

        private void MenuItem_Click_AssignedTaskTypeListBox_Delete(object sender, RoutedEventArgs e)
        {
            DbAdapter.DeleteTaskType(this.Participant.Name, this.Planner.Name, AssignedTaskTypeListBox.SelectedItem.ToString());
            AdjustPlannerDataGrid();
            AdjustAssignedTaskTypeListBox();
            AdjustPlannerDetailsTextBox();
        }
    }
}