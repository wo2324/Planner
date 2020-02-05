using Planner.Utils;
using System;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
            PlannerDataGrid.ItemsSource = Planner.Task.DefaultView;
        }

        public void Color()
        {
            DataTable dataTable = DbInterchanger.GetTaskType(this.Planner.PlannerId);

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

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AssignedTasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
