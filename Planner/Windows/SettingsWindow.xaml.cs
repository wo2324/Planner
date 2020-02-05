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

namespace Planner
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public Participant Participant { get; set; }

        public SettingsWindow(Participant Participant)
        {
            this.Participant = Participant;
            InitializeComponent();
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword(PasswordBox.Password, NewPasswordBox.Password, NewPasswordBox_1.Password);
        }

        private void ChangePassword(string password, string newPassword, string newPasswordSample)
        {
            if (password.Length != 0 && newPassword.Length != 0 && newPasswordSample.Length != 0)
            {
                if (this.Participant.ParticipantPassword == password)
                {
                    if (newPassword == newPasswordSample)
                    {
                        if (password != newPassword)
                        {
                            try
                            {
                                DbAdapter.EditPassword(this.Participant.ParticipantId, newPassword);
                                MessageBox.Show("Password has been edit");
                                this.Close();
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.Message);
                                PasswordBoxClear();
                            }
                        }
                        else
                        {
                            MessageBox.Show("The new password must be different from the current one");
                            PasswordBoxClear();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Given new passwords are non-identical");
                        PasswordBoxClear();
                    }
                }
                else
                {
                    MessageBox.Show("Bad password");
                    PasswordBoxClear();
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled");
                PasswordBoxClear();
            }
        }

        private void PasswordBoxClear()
        {
            PasswordBox.Clear();
            NewPasswordBox.Clear();
            NewPasswordBox_1.Clear();
        }
    }
}
