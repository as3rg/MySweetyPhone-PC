using MaterialDesignThemes.Wpf;
using MySweetyPhone_PC.Tools;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MySweetyPhone_PC.Forms
{
    /// <summary>
    /// Логика взаимодействия для DevicesList.xaml
    /// </summary>
    public partial class DevicesList : Page
    {
        class Devices
        {
            public int code;
            public String[] PCs, Phones;
        }

        class Respond
        {
            public int code;
        }
        
        public DevicesList()
        {
            InitializeComponent();
            String result = Request.Get("http://mysweetyphone.herokuapp.com/?Type=ShowDevices&RegDate=" + App.regdate + "&Login=" + WebUtility.UrlEncode(App.login) + "&Id=" + App.id + "&MyName=" + WebUtility.UrlEncode(App.name));
            Devices devices = JsonConvert.DeserializeObject<Devices>(result);
            if (devices.code == 3)
            {
                MessageBox.Show("Не удалось получить доступ к серверу");
            }
            else if (devices.code == 4)
            {
                MessageBox.Show("Ваше устройство не зарегистрировано!");
                System.Windows.Application.Current.Shutdown();
            }
            else if(devices.code == 0)
            {
                foreach (String s in devices.PCs)
                {
                    ListViewItem lvi = new ListViewItem();
                    StackPanel sp2 = new StackPanel();
                    StackPanel sp = new StackPanel();
                    sp.Margin = new Thickness(10);
                    sp.Orientation = Orientation.Horizontal;
                    PackIcon pi = new PackIcon();
                    pi.Foreground = new SolidColorBrush(Colors.White);
                    pi.Kind = PackIconKind.Computer;
                    pi.Height = pi.Width = 40;
                    TextBlock tb = new TextBlock();
                    tb.Text = s + (App.name == s ? "\nЭто устройство" : "");
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.FontSize = 15;
                    tb.Padding = new Thickness(10,0,0,0);
                    sp.Children.Add(pi);
                    sp.Children.Add(tb);
                    sp2.Children.Add(sp);
                    Button b = new Button();
                    b.Content = (s==App.name ? "Удалить ваше устройство" : "Удалить устройство");
                    b.Visibility = Visibility.Collapsed;
                    b.Background = new SolidColorBrush(Colors.Gray);
                    b.BorderThickness = new Thickness(0);
                    b.Style = (Style)Resources["SquareButton"];
                    b.Padding = new Thickness(7);
                    b.Foreground = new SolidColorBrush(Colors.White);
                    b.FontSize = 15;
                    b.Margin = new Thickness(360, 10, 10, 10);
                    b.Click += delegate
                    {
                        String result2 = Request.Get("http://mysweetyphone.herokuapp.com/?Type=RemoveDevice&RegDate=" + App.regdate + "&Login=" + WebUtility.UrlEncode(App.login) + "&MyName=" + WebUtility.UrlEncode(App.name) + "&Id=" + App.id + "&Name=" + WebUtility.UrlEncode(s));
                        Respond respond = JsonConvert.DeserializeObject<Respond>(result2);
                        if (respond.code == 3)
                        {
                            MessageBox.Show("Не удалось получить доступ к серверу");
                        }
                        else if (respond.code != 0 || s == App.name)
                        {
                            System.Windows.Application.Current.Shutdown();
                        }
                        else
                        {
                            Table.Items.Remove(lvi);
                        }
                    };
                    sp2.Children.Add(b);
                    lvi.Content = sp2;
                    lvi.LostFocus += delegate{ b.Visibility = Visibility.Collapsed; lvi.IsSelected = false; };
                    lvi.GotFocus += delegate { b.Visibility = Visibility.Visible; };
                    Table.Items.Add(lvi);
                }
                foreach (String s in devices.Phones)
                {
                    ListViewItem lvi = new ListViewItem();
                    StackPanel sp2 = new StackPanel();
                    StackPanel sp = new StackPanel();
                    sp.Margin = new Thickness(10);
                    sp.Orientation = Orientation.Horizontal;
                    PackIcon pi = new PackIcon();
                    pi.Foreground = new SolidColorBrush(Colors.White);
                    pi.Kind = PackIconKind.Smartphone;
                    pi.Height = pi.Width = 40;
                    TextBlock tb = new TextBlock();
                    tb.Text = s;
                    tb.Foreground = new SolidColorBrush(Colors.White);
                    tb.FontSize = 15;
                    tb.Padding = new Thickness(10, 0, 0, 0);
                    sp.Children.Add(pi);
                    sp.Children.Add(tb);
                    sp2.Children.Add(sp);
                    Button b = new Button();
                    b.Content = "Удалить";
                    b.Visibility = Visibility.Collapsed;
                    b.Background = new SolidColorBrush(Colors.Gray);
                    b.BorderThickness = new Thickness(0);
                    b.Style = (Style)Resources["SquareButton"];
                    b.Padding = new Thickness(7);
                    b.Foreground = new SolidColorBrush(Colors.White);
                    b.FontSize = 15;
                    b.Margin = new Thickness(360, 10, 10, 10);
                    b.Click += delegate
                    {
                        String result2 = Request.Get("http://mysweetyphone.herokuapp.com/?Type=RemoveDevice&RegDate=" + App.regdate + "&Login=" + WebUtility.UrlEncode(App.login) + "&MyName=" + WebUtility.UrlEncode(App.name) + "&Id=" + App.id + "&Name=" + WebUtility.UrlEncode(s));
                        Respond respond = JsonConvert.DeserializeObject<Respond>(result2);
                        if (respond.code != 0)
                        {
                            MessageBox.Show("Ваше устройство не зарегистрировано!");
                            System.Windows.Application.Current.Shutdown();
                        }
                        else
                        {
                            Table.Items.Remove(lvi);
                        }
                    };
                    sp2.Children.Add(b);
                    lvi.Content = sp2;
                    lvi.LostFocus += delegate { b.Visibility = Visibility.Collapsed; };
                    lvi.GotFocus += delegate { b.Visibility = Visibility.Visible; };
                    Table.Items.Add(lvi);
                }
            }
            else
            {
                MessageBox.Show("Ошибка!");
                System.Windows.Application.Current.Shutdown();
            }
        }
    }
}
