using Planner.Utils;
using System;
using System.Collections.Generic;
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

namespace Planner.Windows
{
    public delegate void AdjustPlannerListBox();

    /// <summary>
    /// Interaction logic for RenamePlannerWindow.xaml
    /// </summary>
    public partial class RenamePlannerWindow : Window
    {
        int ParticipantId;
        string PlannerName;
        private AdjustPlannerListBox AdjustPlannerListBox;

        public RenamePlannerWindow(int participantId, string plannerName, AdjustPlannerListBox adjustPlannerListBox)
        {
            this.ParticipantId = participantId;
            this.PlannerName = plannerName;
            this.AdjustPlannerListBox = adjustPlannerListBox;

            InitializeComponent();
            AdjustControls(plannerName);
        }

        private void AdjustControls(string plannerName)
        {
            CurrentPlannerNameTextBox.Text = plannerName;
            CurrentPlannerNameTextBox.IsEnabled = false;
        }

        private void RenamePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPlannerNameTextBox.Text.Length != 0)
            {
                if (!DbAdapter.ExtractPlannersNamesList(DbAdapter.GetPlannersNames(this.ParticipantId)).Contains(NewPlannerNameTextBox.Text))
                {
                    DbAdapter.RenamePlanner(ParticipantId, PlannerName, NewPlannerNameTextBox.Text);
                    AdjustControls(NewPlannerNameTextBox.Text);
                    this.PlannerName = NewPlannerNameTextBox.Text;
                    this.AdjustPlannerListBox();
                }
                else
                {
                    //
                }
            }
            else
            {
                //
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
