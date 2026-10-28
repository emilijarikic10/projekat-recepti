using Recepti.Models;
using Recepti.Services;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace Recepti
{
   
    public partial class LoginWindow : Window
    {
        private UserService _userService;
        public LoginWindow()
        {
            InitializeComponent();
            _userService = new UserService();
            UsernameBox.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            UsernameError.Text = "";
            PasswordError.Text = "";

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(UsernameBox.Text))
            {
                UsernameError.Text = "Username is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                PasswordError.Text = "Password is required";
                isValid = false;
            }

            if (!isValid)
                return;

            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            var user = _userService.ValidateUser(username, password);

            if (user != null)
            {
                MainWindow mainWindow = new MainWindow(user);
                   mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show(
                    "Invalid username or password.",
                    "Login Failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(); 
        }
    }
}