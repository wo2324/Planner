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
        public Participant Participant;

        public SettingsWindow(Participant Participant)
        {
            this.Participant = Participant;
            InitializeComponent();
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePassword(OldPasswordBox.Password, NewPasswordBox.Password, NewPasswordBox_Retype.Password);
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
                            try
                            {
                                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Change password confirmation", System.Windows.MessageBoxButton.YesNo);
                                if (messageBoxResult == MessageBoxResult.Yes)
                                {
                                    DbAdapter.EditPassword(this.Participant.Id, newPassword);
                                    this.Participant.Password = newPassword;
                                    MessageBox.Show("Password has been edit");
                                    ClearPasswordBoxes();
                                }
                                else
                                {
                                    ClearPasswordBoxes();
                                }
                            }
                            catch (Exception exception)
                            {
                                MessageBox.Show(exception.Message);
                                ClearPasswordBoxes();
                            }
                        }
                        else
                        {
                            MessageBox.Show("The new password must be different from the current one");
                            ClearPasswordBoxes();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Given new passwords are non-identical");
                        ClearPasswordBoxes();
                    }
                }
                else
                {
                    MessageBox.Show("Bad password");
                    ClearPasswordBoxes();
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled");
                ClearPasswordBoxes();
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
        }

        private void DeleteAccount(string password)
        {
            if (password.Length != 0)
            {
                if (this.Participant.Password == password)
                {
                    try
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure?", "Delete confirmation", System.Windows.MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            DbAdapter.DeleteAccount(this.Participant.Id);
                            LogInWindow logInWindow = new LogInWindow();
                            logInWindow.Show();
                            CloseWindows();
                            MessageBox.Show($"Account {this.Participant.Name} has been deleted");
                        }
                        else if (messageBoxResult == MessageBoxResult.No)
                        {
                            PasswordBox.Clear();
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        PasswordBox.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Bad password");
                    PasswordBox.Clear();
                }
            }
            else
            {
                MessageBox.Show("Password field must be filled");
                PasswordBox.Clear();
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
