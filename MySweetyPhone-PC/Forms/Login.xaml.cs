using MySweetyPhone_PC.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MySweetyPhone_PC.Forms
{
    /// <summary>
    /// Логика взаимодействия для Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            Nick.GotFocus += delegate{ ErrorBorder.Visibility = Visibility.Collapsed; };
            Pass.GotFocus += delegate { ErrorBorder.Visibility = Visibility.Collapsed; };
            LogIn.GotFocus += delegate { ErrorBorder.Visibility = Visibility.Collapsed; };
            Offline.GotFocus += delegate { ErrorBorder.Visibility = Visibility.Collapsed; };
            Reg.GotFocus += delegate { ErrorBorder.Visibility = Visibility.Collapsed; };
        }

        class Respond
        {
            public long code;
            public long id;
        }
        private void RegOrLogin(object sender, RoutedEventArgs e)
        {
            if(sender == LogIn)
            {
                if (Nick.Text == "" || Pass.Password == "")
                {
                    Error.Text = "Поля должны быть заполнены!";
                    ErrorBorder.Visibility = Visibility.Visible;
                    return;
                }
                if(!new Regex(@"\w+").IsMatch(Nick.Text) || !new Regex(@"\w+").IsMatch(Pass.Password))
                {
                    Error.Text = "Имя содержит недопустимые символы!";
                    ErrorBorder.Visibility = Visibility.Visible;
                    return;
                }
                Nick.IsEnabled = Pass.IsEnabled = LogIn.IsEnabled = Reg.IsEnabled = Offline.IsEnabled = false;
                String result2 = Request.Get("http://mysweetyphone.herokuapp.com/?Type=Login&Login=" + WebUtility.UrlEncode(Nick.Text) + "&Pass=" + WebUtility.UrlEncode(Pass.Password));
                Respond respond = JsonConvert.DeserializeObject<Respond>(result2);
                switch (respond.code)
                {
                    case 0:
                        App.ini.WritePrivateString("Main", "login", Nick.Text);
                        App.ini.WritePrivateString("Main", "id", respond.id.ToString());
                        App.login = Nick.Text;
                        App.id = respond.id;
                        RegDevice regDevice = new RegDevice();
                        regDevice.Show();
                        this.Close();
                        break;
                    case 1:
                        Error.Text = "Неверное имя или пароль!";
                        ErrorBorder.Visibility = Visibility.Visible;
                        Nick.IsEnabled = Pass.IsEnabled = LogIn.IsEnabled = Reg.IsEnabled = Offline.IsEnabled = true;
                        break;
                    case 3:
                        Error.Text = "Не удалось получить доступ к серверу";
                        ErrorBorder.Visibility = Visibility.Visible;
                        Nick.IsEnabled = Pass.IsEnabled = LogIn.IsEnabled = Reg.IsEnabled = Offline.IsEnabled = true;
                        break;
                }
            }
            else if (sender == Reg)
            {
                if (Nick.Text == "" || Pass.Password == "")
                {
                    Error.Text = "Поля должны быть заполнены!";
                    ErrorBorder.Visibility = Visibility.Visible;
                    return;
                }
                if (!new Regex(@"\w+").IsMatch(Nick.Text) || !new Regex(@"\w+").IsMatch(Pass.Password))
                {
                    Error.Text = "Имя содержит недопустимые символы!";
                    ErrorBorder.Visibility = Visibility.Visible;
                    return;
                }
                Nick.IsEnabled = Pass.IsEnabled = LogIn.IsEnabled = Reg.IsEnabled = Offline.IsEnabled = false;
                String result2 = Request.Get("http://mysweetyphone.herokuapp.com/?Type=Reg&Login=" + WebUtility.UrlEncode(Nick.Text) + "&Pass=" + WebUtility.UrlEncode(Pass.Password));
                Respond respond = JsonConvert.DeserializeObject<Respond>(result2);
                switch (respond.code)
                {
                    case 0:
                        App.ini.WritePrivateString("Main", "login", Nick.Text);
                        App.ini.WritePrivateString("Main", "id", respond.id.ToString());
                        App.login = Nick.Text;
                        App.id = respond.id;
                        RegDevice regDevice = new RegDevice();
                        regDevice.Show();
                        this.Close();
                        break;
                    case 1:
                        Error.Text = "Это имя уже используется!";
                        ErrorBorder.Visibility = Visibility.Visible;
                        Nick.IsEnabled = Pass.IsEnabled = LogIn.IsEnabled = Reg.IsEnabled = Offline.IsEnabled = true;
                        break;
                    case 3:
                        Error.Text = "Не удалось получить доступ к серверу";
                        ErrorBorder.Visibility = Visibility.Visible;
                        Nick.IsEnabled = Pass.IsEnabled = LogIn.IsEnabled = Reg.IsEnabled = Offline.IsEnabled = true;
                        break;
                }
            }
            else
            {
                App.login = "";
                App.id = -1;
                RegDevice regDevice = new RegDevice();
                regDevice.Show();
                this.Close();
            }
        }
    }
}
