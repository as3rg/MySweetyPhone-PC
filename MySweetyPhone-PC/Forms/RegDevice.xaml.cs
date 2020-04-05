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
            App.name = DeviceName.Text;
            App.code = new DateTime().Millisecond % 1000000;
            App.ini.WritePrivateString("Main", "name", DeviceName.Text);
            App.ini.WritePrivateString("Main", "code", App.code.ToString());
            Main main = new Main();
            main.Show();
            this.Close();
            return;
        }
    }
}
