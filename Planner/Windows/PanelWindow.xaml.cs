using Planner.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

        private void AdjustControls()
        {
            AdjustPlannerListBox();
            AdjustParticipantLabel();
        }

        private void AdjustPlannerListBox()
        {
            PlannerListBox.ItemsSource = GetPlanner(DbInterchanger.GetPlanner(this.Participant.ParticipantId));
        }

        private List<string> GetPlanner(DataSet dataSet)
        {
            List<string> planner = new List<string>();
            if (dataSet.Tables.Count == 0)
            {
                PlannerListBox.Visibility = Visibility.Hidden;
            }
            else if (dataSet.Tables.Count == 1)
            {
                DataTable dataTable = dataSet.Tables[0];
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    planner.Add(dataRow["Planner_Name"].ToString());
                }
            }
            return planner;
        }

        private void AdjustParticipantLabel()
        {
            ParticipantLabel.Content = this.Participant.ParticipantName;
        }

        private void PlannerListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void CreatePlannerButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogOutButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
