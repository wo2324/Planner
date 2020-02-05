using Planner.Utils;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Planner
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// </summary>
    public partial class SignUpWindow : Window
    {
        public SignUpWindow()
        {
            InitializeComponent();
        }

        private void SignUpButton_Click(object sender, RoutedEventArgs e)
        {
            SignUp(LoginTextBox.Text, PasswordBox.Password, PasswordBox_1.Password);
        }

        private void SignUp(string login, string password, string passwordSample)
        {
            if (login.Length != 0 && password.Length != 0 && passwordSample.Length != 0)
            {
                if (password == passwordSample)
                {
                    try
                    {
                        DbAdapter.ParticipantAdd(login, password);
                        MessageBox.Show($"Account {login} has been created");
                        LogInWindow logInWindow = new LogInWindow();
                        logInWindow.LogIn(login, password);
                        this.Close();
                    }
                    catch (SqlException sqlException) when (sqlException.Number == 2627)
                    {
                        string messageTextBox = $"Account {login} already exists";
                        MessageBox.Show(messageTextBox);
                        PasswordBoxClear();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        PasswordBoxClear();
                    }
                }
                else
                {
                    MessageBox.Show("Given passwords are non-identical");
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
            PasswordBox_1.Clear();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.Show();
            this.Close();
        }
    }
}
