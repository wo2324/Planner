using Planner.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataGridCell = System.Windows.Controls.DataGridCell;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Planner
{
    /// <summary>
    /// Interaction logic for PlannerWindow.xaml
    /// </summary>
    public partial class PlannerWindow : Window
    {
        public Utils.Planner Planner { get; }

        public PlannerWindow(Utils.Planner Planner)
        {
            this.Planner = Planner;
            InitializeComponent();
            Planner.AssignTask();
            PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;
            AdjustControls();
        }

        private void PlannerDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)  //Do poprawy!
        {
            e.Row.Header = (e.Row.GetIndex()).ToString() + "Test";

            int startHour = Int32.Parse(this.Planner.StartHour.Substring(0, 2));
            int startMinute = Int32.Parse(this.Planner.StartHour.Substring(3, 2));

            int stopHour = Int32.Parse(this.Planner.StopHour.Substring(0, 2));
            int stopMinute = Int32.Parse(this.Planner.StopHour.Substring(3, 2));

            int timeSpan = Int32.Parse(this.Planner.TimeSpan.Substring(3, 2));

            int hour = startHour;
            int minute = startMinute;

            string rowName;

            while (!(hour == stopHour && minute == stopMinute))
            {
                rowName = $"{hour}:{minute}";
                //e.Row.Header = $"{e.Row.GetIndex() + startHour == 24 ? 0 : 1}:{e.Row.GetIndex() * startMinute}";
                e.Row.Header = ((e.Row.GetIndex() / (60 / timeSpan)) + startHour >= 24 ? (e.Row.GetIndex() / (60 / timeSpan)) + startHour - 24 : (e.Row.GetIndex() / (60 / timeSpan)) + startHour).ToString("D2") +
                    ":" +
                    (e.Row.GetIndex() * timeSpan + startMinute - (e.Row.GetIndex() / (60 / timeSpan) * 60)).ToString("D2");


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
        }

        private void AdjustControls()
        {
            AdjustAssignedTasksListBox();
            //AdjustPlannerDetailsListBox();
        }

        private void AdjustAssignedTasksListBox()
        {
            AssignedTasksListBox.ItemsSource = GetTaskList(DbAdapter.GetTaskType(this.Planner.PlannerId));
        }

        private List<string> GetTaskList(DataTable dataTable)
        {
            List<string> task = new List<string>();
            if (dataTable == null)
            {
                AssignedTasksListBox.Visibility = Visibility.Hidden;
            }
            else
            {
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    task.Add(dataRow["TaskType_Name"].ToString());
                }
            }
            return task;
        }

        public void ColorPlanner()
        {
            DataTable dataTable = DbAdapter.GetTaskType(this.Planner.PlannerId);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                string name = dataRow["TaskType_Name"].ToString();
                string color = dataRow["TaskType_Color"].ToString();
                GetDataGridRows(name, color);
            }
        }

        public void GetDataGridRows(string task, string color)  //Do Poprawy!
        {
            for (int i = 0; i < PlannerDataGrid.Items.Count; i++)
            {
                for (int j = 0; j < PlannerDataGrid.Columns.Count; j++)
                {
                    //loop throught cell
                    DataGridCell cell = GetCell(i, j);
                    TextBlock tb = cell.Content as TextBlock;
                    if (tb.Text == task)
                    {
                        var converter = new System.Windows.Media.BrushConverter();
                        var brush = (Brush)converter.ConvertFromString(color);
                        cell.Background = brush;
                    }
                }
            }
        }

        public DataGridCell GetCell(int row, int column)    //Do Poprawy!
        {
            DataGridRow rowContainer = GetRow(row);

            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(rowContainer);

                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                if (cell == null)
                {
                    PlannerDataGrid.ScrollIntoView(rowContainer, PlannerDataGrid.Columns[column]);
                    cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(column);
                }
                return cell;
            }
            return null;
        }

        public DataGridRow GetRow(int index)    //Do Poprawy!
        {
            DataGridRow row = (DataGridRow)PlannerDataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                PlannerDataGrid.UpdateLayout();
                PlannerDataGrid.ScrollIntoView(PlannerDataGrid.Items[index]);
                row = (DataGridRow)PlannerDataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }

        public static T GetVisualChild<T>(Visual parent) where T : Visual   //Do Poprawy!
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

        //Liczenie czasu
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)  //Do poprawy!
        {
            try
            {
                DataView value = PlannerDataGrid.ItemsSource as DataView;
                DataTable dataTable = value.ToTable();
                DataTable result = new DataTable("Result");
                result.Columns.Add("task", typeof(string));
                result.Columns.Add("day", typeof(string));
                result.Columns.Add("time", typeof(string));

                string StartTime = "05:00";
                string StopTime = "00:00";
                string TimeSpan = "00:30";

                int counter = 0;
                string day;
                string hour;
                //value, day, hour
                int startHour = Int32.Parse(StartTime.Substring(0, 2));
                int startMinute = Int32.Parse(StartTime.Substring(3, 2));
                int stopHour = Int32.Parse(StopTime.Substring(0, 2));
                int stopMinute = Int32.Parse(StopTime.Substring(3, 2));
                int timeSpanHour = Int32.Parse(TimeSpan.Substring(0, 2));
                int timeSpanMinute = Int32.Parse(TimeSpan.Substring(3, 2));

                int actualHour = startHour;
                int actualMinute = startMinute;

                while (actualHour != stopHour)
                {
                    while (actualMinute != 60)
                    {
                        string time = $"{actualHour.ToString("D2")}:{actualMinute.ToString("D2")}";
                        result.Rows.Add(dataTable.Rows[counter]["Monday"], "Monday", time);
                        result.Rows.Add(dataTable.Rows[counter]["Tuesday"], "Tuesday", time);
                        result.Rows.Add(dataTable.Rows[counter]["Wedneday"], "Wednesday", time);
                        result.Rows.Add(dataTable.Rows[counter]["Thursday"], "Thursday", time);
                        result.Rows.Add(dataTable.Rows[counter]["Friday"], "Friday", time);
                        result.Rows.Add(dataTable.Rows[counter]["Saturday"], "Saturday", time);
                        result.Rows.Add(dataTable.Rows[counter]["Sunday"], "Sunday", time);
                        actualMinute += timeSpanMinute;
                        counter++;
                        if (counter == 38)
                        {
                            int s = 1;
                        }
                    }
                    actualMinute = 0;
                    actualHour++;
                    if (actualHour == 24)
                    {
                        actualHour = 0;
                    }
                }

                DbAdapter.EditTask(this.Planner.PlannerId, result);

                //GetDataGridRows();
                ColorPlanner();

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void ColorPickerButton_Click(object sender, RoutedEventArgs e)  //Do poprawy!
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            ColorPickerButton.Background = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)  //Do poprawy!   //Sprawdzić nazewnictwo kontrole TextBox
        {
            DbAdapter.TaskTypeAdd(this.Planner.PlannerId, TaskNameTextBox.Text, TextVisibility.IsEnabled, ColorPickerButton.Background.ToString());
            TaskNameTextBox.Clear();
            TextVisibility.IsChecked = false;
            var converter = new System.Windows.Media.BrushConverter();
            var brush = (Brush)converter.ConvertFromString("#D4D4D4");
            ColorPickerButton.Background = brush;
            AdjustAssignedTasksListBox();
            AssignedTasksExpander.IsExpanded = true;
        }

        private void AssignedTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssignedTasksListBox.SelectedItem != null)
            {
                IList<DataGridCellInfo> selectedCells = this.PlannerDataGrid.SelectedCells;
                //foreach (DataGridCellInfo cell in selectedCells)
                //{
                //    string column = cell.Column.Header.ToString();
                //    DataGridRow row = (DataGridRow)PlannerDataGrid.ItemContainerGenerator.ContainerFromItem(cell.Item);
                //    //MessageBox.Show($"column: {column}, row index: {row.Header.ToString()}");
                //}

                //sprawdzić który element ListBoxa został naciśnięty
                //sprawdzić które elementy plannera zostały wybrane
                //zasilić tymi danymi procedurę
                //odświerzyć planner

                //..
                string taskName = AssignedTasksListBox.SelectedItem.ToString();

                DataTable dataTable = new DataTable("Result");
                dataTable.Columns.Add("task", typeof(string));
                dataTable.Columns.Add("day", typeof(string));
                dataTable.Columns.Add("time", typeof(string));
                foreach (DataGridCellInfo cell in selectedCells)
                {
                    string column = cell.Column.Header.ToString();
                    DataGridRow row = (DataGridRow)PlannerDataGrid.ItemContainerGenerator.ContainerFromItem(cell.Item);
                    dataTable.Rows.Add(taskName, column, row.Header.ToString());
                }

                DbAdapter.EditTask(this.Planner.PlannerId, dataTable);
                //wczytaj wszystkie taski
                Planner.AssignTask();
                PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;

                ColorPlanner();

                AssignedTasksListBox.SelectedItem = null;
            }
        }
    }
}