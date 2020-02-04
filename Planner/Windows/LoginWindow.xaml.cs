using Planner.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Planner
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        public LogInWindow()
        {
            InitializeComponent();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LogIn(LoginTextBox.Text, PasswordBox.Password);
        }

        public void LogIn(string login, string password)
        {
            if (login.Length != 0 && password.Length != 0)
            {
                try
                {
                    int participantId = DbInterchanger.ParticipantGet(login, password);
                    if (participantId != 0)
                    {
                        PanelWindow panelWindow = new PanelWindow(new Participant(participantId, login, password));
                        panelWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Bad user name or password");
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
                MessageBox.Show("All fields must be filled");
                PasswordBox.Clear();
            }
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUpWindow signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            this.Close();
        }
    }
}
