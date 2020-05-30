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
    /// Interaction logic for EditPlannerWindow.xaml
    /// </summary>
    public partial class EditPlannerWindow : Window
    {
        Participant Participant;
        private AdjustPlannerListBox AdjustPlannerListBox;

        public EditPlannerWindow(int participantId, string plannerName, AdjustPlannerListBox adjustPlannerListBox)
        {
            this.ParticipantId = participantId;
            this.PlannerName = plannerName;
            this.AdjustPlannerListBox = adjustPlannerListBox;

            InitializeComponent();
            AdjustControls(plannerName);
        }

        private void AdjustControls(string plannerName)
        {
            PlannerNameTextBox.Text = plannerName;
        }

        private void RenamePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            if (PlannerNewNameTextBox.Text.Length != 0)
            {
                if (!DbAdapter.ExtractPlannersNamesList(DbAdapter.GetPlannersNames(this.ParticipantId)).Contains(PlannerNewNameTextBox.Text))
                {
                    DbAdapter.RenamePlanner(ParticipantId, PlannerName, PlannerNewNameTextBox.Text);
                    AdjustControls(PlannerNewNameTextBox.Text);
                    this.PlannerName = PlannerNewNameTextBox.Text;
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
    }
}
