using Planner.Tools;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Planner
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Participant Participant;

        public SettingsWindow(Participant participant)
        {
            this.Participant = participant;
            InitializeComponent();
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword(OldPasswordBox.Password, NewPasswordBox.Password, NewPasswordBox_Retype.Password);
            ClearPasswordBoxes();
        }

        private void ChangePassword(string password, string newPassword, string newPasswordRetype)
        {
            if (password.Length != 0 && newPassword.Length != 0 && newPasswordRetype.Length != 0)
            {
                if (this.Participant.Password == password)
                {
                    if (newPassword == newPasswordRetype)
                    {
                        if (password != newPassword)
                        {
                            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Change password confirmation", System.Windows.MessageBoxButton.YesNo);
                            if (messageBoxResult == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    DbAdapter.EditPassword(this.Participant.Name, newPassword);
                                    this.Participant.Password = newPassword;
                                    MessageBox.Show("Password has been edit");
                                }
                                catch (Exception exception)
                                {
                                    MessageBox.Show(exception.Message);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("The new password must be different from the current one");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Given new passwords are non-identical");
                    }
                }
                else
                {
                    MessageBox.Show("Bad password");
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled");
            }
        }

        private void ClearPasswordBoxes()
        {
            OldPasswordBox.Clear();
            NewPasswordBox.Clear();
            NewPasswordBox_Retype.Clear();
        }

        private void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteAccount(PasswordBox.Password);
            PasswordBox.Clear();
        }

        private void DeleteAccount(string password)
        {
            if (password.Length != 0)
            {
                if (this.Participant.Password == password)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete confirmation", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            DbAdapter.DeleteAccount(this.Participant.Name);
                            MessageBox.Show($"Account {this.Participant.Name} has been deleted");
                            LogInWindow logInWindow = new LogInWindow();
                            logInWindow.Show();
                            CloseWindows();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Bad password");
                }
            }
            else
            {
                MessageBox.Show("Password field must be filled");
            }
        }

        private void CloseWindows()
        {
            foreach (Window item in Application.Current.Windows)
            {
                if (item.Title == "PlannerWindow" || item.Title == "PanelWindow" || item.Title == "SettingsWindow")
                {
                    item.Close();
                }
            }
        }
    }
}
