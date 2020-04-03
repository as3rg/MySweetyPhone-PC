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
    /// Логика взаимодействия для RegDevice.xaml
    /// </summary>
    public partial class RegDevice : Window
    {
        public RegDevice()
        {
            InitializeComponent();
            Next.GotFocus += delegate
            {
                ErrorBorder.Visibility = Visibility.Collapsed;
            };
            DeviceName.GotFocus += delegate
            {
                ErrorBorder.Visibility = Visibility.Collapsed;
            };
        }

        class Respond
        {
            public int code,regdate;
        }

        private void Reg(object sender, RoutedEventArgs e)
        {
            if(DeviceName.Text == "")
            {
                Error.Text = "Вы должны указать имя!";
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }
            if (!new Regex(@"\w+").IsMatch(DeviceName.Text))
            {
                Error.Text = "Имя устройства содержит недопустимые символы!";
                ErrorBorder.Visibility = Visibility.Visible;
                return;
            }
            if(App.login == "")
            {
                App.name = DeviceName.Text;
                App.regdate = System.DateTime.Now.Second;
                App.ini.WritePrivateString("Main", "name", DeviceName.Text);
                App.ini.WritePrivateString("Main", "regdate", App.regdate.ToString());
                Main main = new Main();
                main.Show();
                this.Close();
                return;
            }
            Next.IsEnabled = DeviceName.IsEnabled = false;
            String result2 = Request.Get("http://mysweetyphone.herokuapp.com/?Type=AddDevice&DeviceType=PC&Id=" + App.id + "&Login=" + WebUtility.UrlEncode(App.login) + "&Name=" + WebUtility.UrlEncode(DeviceName.Text));
            Respond respond = JsonConvert.DeserializeObject<Respond>(result2);
            switch (respond.code)
            {
                case 0:
                    App.ini.WritePrivateString("Main", "name", DeviceName.Text);
                    App.ini.WritePrivateString("Main", "regdate", respond.regdate.ToString());
                    App.name = DeviceName.Text;
                    App.regdate = respond.regdate;
                    Main main = new Main();
                    main.Show();
                    this.Close();
                    break;
                case 1:
                    Error.Text = "Это имя устройства уже используется!";
                    ErrorBorder.Visibility = Visibility.Visible;
                    Next.IsEnabled = DeviceName.IsEnabled = true;
                    break;
                case 3:
                    Error.Text = "Не удалось получить доступ к серверу";
                    ErrorBorder.Visibility = Visibility.Visible;
                    Next.IsEnabled = DeviceName.IsEnabled = true;
                    break;
            }
        }
    }
}
