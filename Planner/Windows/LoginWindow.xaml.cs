using Planner.Tools;
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
            PasswordBox.Clear();
        }

        public void LogIn(string login, string password)
        {
            if (login.Length != 0 && password.Length != 0)
            {
                try
                {
                    bool checkSentence = DbAdapter.ParticipantCheck(login, password);
                    if (checkSentence)
                    {
                        Participant participant = new Participant(login, password);
                        PanelWindow panelWindow = new PanelWindow(participant);
                        panelWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Bad user name or password");
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message);
                }
            }
            else
            {
                MessageBox.Show("All fields must be filled");
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
