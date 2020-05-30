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
        string PlannerName;
        AdjustPlannerListBox AdjustPlannerListBox;

        public EditPlannerWindow(Participant participant, string plannerName, AdjustPlannerListBox adjustPlannerListBox)
        {
            this.Participant = participant;
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
            RenamePlanner(this.Participant.Id, this.PlannerName, PlannerNewNameTextBox.Text, out bool rename);
            if (rename)
            {
                this.AdjustPlannerListBox();
            }
        }

        private void RenamePlanner(int participantId, string plannerName, string plannerNewName, out bool rename)
        {
            rename = false;
            if (plannerNewName.Length != 0)
            {
                if (plannerName != plannerNewName)
                {
                    if (!DbAdapter.ExtractPlannersNamesList(DbAdapter.GetPlannersNames(participantId)).Contains(plannerNewName))
                    {
                        DbAdapter.RenamePlanner(participantId, plannerName, plannerNewName);
                        rename = true;
                        AdjustControls(plannerNewName);
                        PlannerNewNameTextBox.Clear();
                        MessageBox.Show("Planner has been renamed");
                    }
                    else
                    {
                        MessageBox.Show($"Planner {plannerNewName} already exists");
                        PlannerNewNameTextBox.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("The planner new name must be different from the current one");
                    PlannerNewNameTextBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Planner new name field must be filled");
            }
        }
    }
}
