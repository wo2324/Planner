﻿using Planner.Utils;
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
            SignUp(LoginTextBox.Text, PasswordBox.Password, PasswordBox_Retype.Password);
        }

        private void SignUp(string login, string password, string passwordRetype)
        {
            if (login.Length != 0 && password.Length != 0 && passwordRetype.Length != 0)
            {
                if (password == passwordRetype)
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
                        ClearPasswordBoxes();
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        ClearPasswordBoxes();
                    }
                }
                else
                {
                    MessageBox.Show("Given passwords are non-identical");
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
            PasswordBox.Clear();
            PasswordBox_Retype.Clear();
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            LogInWindow logInWindow = new LogInWindow();
            logInWindow.Show();
            this.Close();
        }
    }
}
