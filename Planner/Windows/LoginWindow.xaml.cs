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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
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
                    int participantId = DbInterchanger.ParticipantIdGet(login, password);
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
