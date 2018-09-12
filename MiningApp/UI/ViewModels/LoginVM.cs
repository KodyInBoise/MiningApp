using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class LoginVM
    {
        Grid ViewGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Login", 40);

        TextBox EmailTextBox { get; set; } = ElementHelper.CreateTextBox("Email");

        PasswordBox PasswordBox { get; set; } = ElementHelper.CreatePasswordBox("Password");

        Button LoginButton { get; set; } = ElementHelper.CreateButton("Login", style:ButtonStyleEnum.New, height: 50, width: 150);

        Label EmailLabel { get; set; }

        Label PasswordLabel { get; set; }


        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;

        double labelRight = 750;

        double labelOffset = -5;


        public LoginVM()
        {
            Show();
        }

        private void Show()
        {
            DisplayElement(TitleTextBlock);

            nextLeft += padding * 20;
            nextTop += padding * 15;
            DisplayElement(EmailTextBox);
            EmailTextBox.KeyDown += (s, e) => {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    PasswordBox.Focus();
                }
            };

            if (!String.IsNullOrEmpty(Bootstrapper.Settings.User.Email))
            {
                EmailTextBox.Text = Bootstrapper.Settings.User.Email;
            }

            DisplayElement(PasswordBox, topPadding: 5);
            PasswordBox.KeyDown += (s, e) =>
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    LoginButton_Clicked();
                }
            };

            nextLeft = PasswordBox.Margin.Left + PasswordBox.Width - LoginButton.Width;
            DisplayElement(LoginButton);
            LoginButton.Click += (s, e) => LoginButton_Clicked();

            nextTop = EmailTextBox.Margin.Top;
            EmailLabel = ElementHelper.CreateLabel("Email", EmailTextBox);
            EmailLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
            DisplayElement(EmailLabel, ignoreMargin: true);

            nextTop = PasswordBox.Margin.Top;
            PasswordLabel = ElementHelper.CreateLabel("Password", PasswordBox);
            PasswordLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
            DisplayElement(PasswordLabel, ignoreMargin: true);

            if (String.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmailTextBox.Focus();
            }
            else
            {
                PasswordBox.Focus();
            }
        }

        public void Dispose()
        {
            WindowController.Instance.LoginView = null;
        }


        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0, bool ignoreMargin = false)
        {
            if (!ignoreMargin)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);
            }

            ViewGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        async void LoginButton_Clicked()
        {
            var email = EmailTextBox.Text;
            var pass = PasswordBox.Password;

            if (!String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(pass))
            {
                LoginButton.Visibility = Visibility.Collapsed;

                Bootstrapper.Settings.Server.UserAuthenticated = await Task.Run(() => ServerHelper.AuthenticateUser(email, pass));

                if (Bootstrapper.Settings.Server.UserAuthenticated)
                {
                    Bootstrapper.Instance.SetUser(await Task.Run(() => ServerHelper.GetUserByEmail(email)));

                    WindowController.Instance.ShowHome();
                }
                else
                {
                    MessageBox.Show("Authentication failed!");

                    LoginButton.Visibility = Visibility.Visible;
                    PasswordBox.Focus();
                    PasswordBox.SelectAll();
                }
            }
            else if (String.IsNullOrEmpty(email))
            {
                EmailTextBox.Focus();
            }
            else if (String.IsNullOrEmpty(pass))
            {
                PasswordBox.Focus();
            }
        }
    }
}

