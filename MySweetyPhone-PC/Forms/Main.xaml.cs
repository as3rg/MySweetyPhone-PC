using MySweetyPhone_PC.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

namespace MySweetyPhone_PC.Forms
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            LoginNav.Text = App.login;
            if (App.login == "")
            {
                DevicesListNav.Visibility = SavedNav.Visibility = Visibility.Collapsed;
                SClientNav.IsSelected = true;
            }
            else
                DevicesListNav.IsSelected = true;
        }

        private void Selected(object sender, RoutedEventArgs e)
        {
            if (sender == DevicesListNav)
            {
                Content.Content = new DevicesList();
            }
            else if(sender == SavedNav)
            {
                Content.Content = new Saved();
            }
            else if (sender == SClientNav)
            {
                Content.Content = new SClient();
            }
            else if (sender == SServerNav)
            {
                Content.Content = new SServer();
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            Selected(Menu.SelectedItem, null);
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Request.Get("http://mysweetyphone.herokuapp.com/?Type=RemoveDevice&RegDate=" + App.regdate + "&Login=" + WebUtility.UrlEncode(App.login) + "&MyName=" + WebUtility.UrlEncode(App.name) + "&Id=" + App.id + "&Name=" + App.name);
            if (File.Exists(@"./Properties.ini"))
            {
                File.Delete(@"./Properties.ini");
            }
            System.Windows.Application.Current.Shutdown();
        }
    }
}
