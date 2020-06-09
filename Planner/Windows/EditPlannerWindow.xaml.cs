using Planner.Tools;
using System;
using System.Windows;

namespace Planner.Windows
{
    public delegate void AdjustPlannerListBox();

    /// <summary>
    /// Interaction logic for EditPlannerWindow.xaml
    /// </summary>
    public partial class EditPlannerWindow : Window
    {
        private Participant Participant;
        private Tools.Planner Planner;
        private AdjustPlannerListBox AdjustPlannerListBox;

        public EditPlannerWindow(Participant participant, Tools.Planner planner, AdjustPlannerListBox adjustPlannerListBox)
        {
            this.Participant = participant;
            this.Planner = planner;
            this.AdjustPlannerListBox = adjustPlannerListBox;
            InitializeComponent();
            AdjustControls();
        }

        private void AdjustControls()
        {
            PlannerNameTextBox.Text = this.Planner.Name;
        }

        private void RenamePlannerButton_Click(object sender, RoutedEventArgs e)
        {
            RenamePlanner(this.Participant.Name, this.Planner.Name, PlannerNewNameTextBox.Text, out bool rename);
            if (rename)
            {
                this.AdjustPlannerListBox();
            }
        }

        private void RenamePlanner(string participantName, string plannerName, string plannerNewName, out bool rename)
        {
            rename = false;
            if (plannerNewName.Length != 0)
            {
                if (plannerName != plannerNewName)
                {
                    if (!DbAdapter.ExtractPlanners(DbAdapter.GetPlanners(participantName)).Contains(plannerNewName))
                    {
                        try
                        {
                            DbAdapter.RenamePlanner(participantName, plannerName, plannerNewName);
                            this.Planner.Name = plannerNewName;
                            rename = true;
                            AdjustControls();
                            MessageBox.Show("Planner has been renamed");
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Planner {plannerNewName} already exists");
                    }
                }
                else
                {
                    MessageBox.Show("The planner new name must be different from the current one");
                }
            }
            else
            {
                MessageBox.Show("Planner new name field must be filled");
            }
        }
    }
}
