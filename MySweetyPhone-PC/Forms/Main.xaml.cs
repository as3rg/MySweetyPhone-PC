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
            NameNav.Text = App.name;
            SClientNav.IsSelected = true;
        }

        private void Selected(object sender, RoutedEventArgs e)
        {
            if (Content.Content != null) ((IDisposable)Content.Content).Dispose();
            if (sender == SClientNav)
            {
                Content.Content = new SClient();
            }
            else if (sender == SServerNav)
            {
                Content.Content = new SServer();
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"./Properties.ini"))
            {
                File.Delete(@"./Properties.ini");
            }
            System.Windows.Application.Current.Shutdown();
        }

        private void NewKey(object sender, RoutedEventArgs e)
        {
            InputDialog id = new InputDialog("Новый код", "Введите новый код", InputDialog.Type.CODE, App.code.ToString());
            id.ShowDialog();
            if (id.GetResult() == null) return;
            App.code = int.Parse(id.GetResult());
            App.ini.WritePrivateString("Main", "code", App.code.ToString());
        }
    }
}
