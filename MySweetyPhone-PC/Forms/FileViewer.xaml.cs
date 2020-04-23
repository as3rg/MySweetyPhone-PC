using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MySweetyPhone_PC.Tools;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MySweetyPhone_PC.Forms
{
    /// <summary>
    /// Логика взаимодействия для FileViewer.xaml
    /// </summary>
    public partial class FileViewer : Window
    {
        class Start
        {
            public string Type, Name;
            public long Code;
        }

        class ShowDir
        {
            public string Type, Name, Dir, DirName;
            public long Code;
        }

        class Back
        {
            public string Type, Name, Dir;
            public long Code;
        }

        class Download
        {
            public string Type, Name, Dir, FileName;
            public long Code, FileSocketPort;
        }

        class Delete
        {
            public string Type, Name, Dir, FileName;
            public long Code;
        }

        class Rename
        {
            public string Type, Name, Dir, FileName, NewFileName;
            public long Code;
        }

        private string _Dir;

        string Directory {
            set {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    Path.Text = value;
                    _Dir = value;
                }));
            }

            get
            {
                return _Dir;
            }
        }

        class Respond
        {

            public class Comparer : IComparer<File>
            {
                public int Compare(File x, File y)
                {
                    return String.Compare(x.Name, y.Name);
                }
            }
            public class File
            {
                public String Name, Type;
            }
            public String Dir, DirName, Type;
            public int State;
            public File[] Inside;
        }

        SessionClient sc;
        StreamWriter writer;
        StreamReader reader;
        public FileViewer(SessionClient sc)
        {
            this.sc = sc;
            InitializeComponent();
            this.Closing += delegate
            {
                Start st = new Start();
                st.Type = "finish";
                st.Name = App.name;
                if (sc.mode != 0) st.Code = App.code % sc.mode;
                writer.WriteLine(JsonConvert.SerializeObject(st));
                writer.Flush();
            };

            Thread receiving = new Thread(() =>
            {
                Thread.Sleep(2000);
                TcpClient tcp = new TcpClient(sc.address.ToString(), sc.port);
                reader = new StreamReader(tcp.GetStream());
                writer = new StreamWriter(tcp.GetStream());
                Start st = new Start();
                st.Type = "start";
                st.Name = App.name;
                if (sc.mode != 0) st.Code = App.code % sc.mode;
                Thread starting = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            writer.WriteLine(JsonConvert.SerializeObject(st));
                            writer.Flush();
                            Thread.Sleep(2000);
                        }
                    }catch(ThreadInterruptedException _) {}
                });
                starting.Start();
                NetworkStream ns = tcp.GetStream();
                while (true)
                {
                    List<byte> bytes = new List<byte>();
                    string chr;
                    do
                    {
                        byte[] b = new byte[1];
                        ns.Read(b, 0, b.Length);
                        bytes.Add(b[0]);
                        chr = Encoding.UTF8.GetString(b);
                    } while (chr != "\n");

                    starting.Interrupt();
                    Respond msg = JsonConvert.DeserializeObject<Respond>(Encoding.UTF8.GetString(bytes.ToArray()));
                    switch (msg.Type)
                    {
                        case "finish":
                            this.Close();
                            tcp.Close();
                            return;
                        case "renameFile":
                        case "deleteFile":
                            if (Directory != msg.Dir) break;
                            if (msg.State == 1)
                            {
                                Application.Current.Dispatcher.Invoke(new Action(() =>
                                {
                                    InfoDialog id = new InfoDialog("Ошибка", "Нет доступа");
                                    id.ShowDialog();
                                }));
                            }
                            else
                            {
                                Application.Current.Dispatcher.Invoke(new Action(()=>reloadFolder(null, null)));
                            }
                            break;
                        case "showDir":
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {

                                if (msg.State == 1)
                                {
                                    InfoDialog id = new InfoDialog("Ошибка", "Нет доступа");
                                    id.ShowDialog();
                                }
                                else
                                {
                                    FilesList.Items.Clear();
                                    Directory = msg.Dir;
                                    Array.Sort(msg.Inside, new Respond.Comparer());
                                    foreach (Respond.File f in msg.Inside)
                                    {
                                        Draw(f.Name, f.Type == "Folder", msg.Dir);
                                    }
                                }
                            }));
                            break;
                        case "newDir":
                            if (Directory != msg.Dir) break;
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                Draw(msg.DirName, true, msg.Dir);
                            }));
                            break;
                    }
                }
            });
            receiving.Start();
        }

        public void back(object sender, RoutedEventArgs e)
        {
            Back sd = new Back();
            sd.Type = "back";
            sd.Name = App.name;
            sd.Dir = Directory;
            if (sc.mode != 0) sd.Code = App.code % sc.mode;
            writer.WriteLine(JsonConvert.SerializeObject(sd));
            writer.Flush();
        }

        public void newFolder(object sender, RoutedEventArgs e)
        {
            InputDialog id = new InputDialog("Имя папки", "Введите имя папки", InputDialog.Type.FILENAME);
            id.ShowDialog();
            if (id.GetResult() == null) return;
            ShowDir sd = new ShowDir();
            sd.Type = "newDir";
            sd.Name = App.name;
            sd.Dir = Directory;
            sd.DirName = id.GetResult();
            if (sc.mode != 0) sd.Code = App.code % sc.mode;
            writer.WriteLine(JsonConvert.SerializeObject(sd));
            writer.Flush();
        }

        public void uploadFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() != true)
                return;
            string fileName = ofd.FileName;
            Download download = new Download();
            download.Type = "uploadFile";
            download.Name = App.name;
            download.Dir = Directory;
            download.FileName = System.IO.Path.GetFileName(fileName);
            new Thread(() =>
            {
                using (FileStream fstream = new FileStream(fileName, FileMode.Open))
                {
                    Socket uploadSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    uploadSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                    download.FileSocketPort = (uploadSocket.LocalEndPoint as IPEndPoint).Port;
                    if (sc.mode != 0) download.Code = App.code % sc.mode;
                    writer.WriteLine(JsonConvert.SerializeObject(download));
                    writer.Flush();
                    uploadSocket.Listen(1);
                    Socket uploadSocketConnected = uploadSocket.Accept();
                    NetworkStream ns = new NetworkStream(uploadSocketConnected);
                    fstream.CopyTo(ns);
                    ns.Flush();
                    ns.Close();
                    uploadSocketConnected.Close();
                    uploadSocket.Close();
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        InfoDialog id = new InfoDialog("Отправка завершена", "Отправка файла " + System.IO.Path.GetFileName(fileName) + " завершена");
                        id.Show();
                        Draw(System.IO.Path.GetFileName(fileName), false, System.IO.Path.GetDirectoryName(fileName));
                    }));
                }
            }).Start();
        }

        public void reloadFolder(object sender, RoutedEventArgs e)
        {
            ShowDir sd = new ShowDir();
            sd.Type = "showDir";
            sd.Name = App.name;
            sd.Dir = Directory;
            sd.DirName = "";
            if (sc.mode != 0) sd.Code = App.code % sc.mode;
            writer.WriteLine(JsonConvert.SerializeObject(sd));
            writer.Flush();
        }

        public void Draw(string fileName, bool isFolder, String dir)
        {

            StackPanel sp = new StackPanel();
            sp.Margin = new Thickness(10);
            sp.Orientation = Orientation.Horizontal;
            PackIcon pi = new PackIcon();
            pi.Foreground = new SolidColorBrush(Colors.White);
            pi.Kind = isFolder ? PackIconKind.Folder : PackIconKind.File;
            pi.Height = pi.Width = 20;
            TextBlock tb = new TextBlock();
            tb.Text = fileName;
            tb.Foreground = new SolidColorBrush(Colors.White);
            tb.FontSize = 15;
            tb.Padding = new Thickness(10, 0, 0, 0);
            sp.Children.Add(pi);
            sp.Children.Add(tb);
            ListViewItem lvi = new ListViewItem();
            lvi.Content = sp;
            FilesList.Items.Add(lvi);

            ContextMenu menu = new ContextMenu();
            lvi.ContextMenu = menu;
            MenuItem delete = new MenuItem();
            delete.Header = "Удалить";
            menu.Items.Add(delete);
            delete.Click += delegate
            {
                Delete del = new Delete();
                del.Type = "deleteFile";
                del.Name = App.name;
                del.Dir = dir;
                del.FileName = fileName;
                if (sc.mode != 0) del.Code = App.code % sc.mode;
                writer.WriteLine(JsonConvert.SerializeObject(del));
                writer.Flush();
            };
            MenuItem rename = new MenuItem();
            rename.Header = "Переименовать";
            menu.Items.Add(rename);
            rename.Click += delegate
            {
                InputDialog id = new InputDialog("Имя файла", "Введите имя файла", InputDialog.Type.FILENAME, fileName);
                id.ShowDialog();
                if (id.GetResult() == null) return;
                Rename rn = new Rename();
                rn.Type = "renameFile";
                rn.Name = App.name;
                rn.Dir = Directory;
                rn.FileName = fileName;
                rn.NewFileName = id.GetResult();

                if (sc.mode != 0) rn.Code = App.code % sc.mode;
                writer.WriteLine(JsonConvert.SerializeObject(rn));
                writer.Flush();
            };

            if (!isFolder)
            {
                MenuItem Download = new MenuItem();
                Download.Header = "Загрузить";
                menu.Items.Add(Download);
                Download.Click += delegate
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    string ext = System.IO.Path.GetExtension(fileName);
                    sfd.Filter = ext.Substring(1,ext.Length-1).ToUpper()+" file|*" + ext + "|All files(*.*)|*.*";
                    if(sfd.ShowDialog() != true)
                        return;
                    new Thread(() =>
                    {
                        using (FileStream fstream = new FileStream(sfd.FileName, FileMode.Create))
                        {
                            Socket downloadSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            downloadSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
                            Download download = new Download();
                            download.Type = "downloadFile";
                            download.Name = App.name;
                            download.Dir = dir;
                            download.FileName = fileName;
                            download.FileSocketPort = (downloadSocket.LocalEndPoint as IPEndPoint).Port;
                            if (sc.mode != 0) download.Code = App.code % sc.mode;
                            writer.WriteLine(JsonConvert.SerializeObject(download));
                            writer.Flush();
                            downloadSocket.Listen(1);
                            Socket downloadSocketConnected = downloadSocket.Accept();
                            NetworkStream ns = new NetworkStream(downloadSocketConnected);
                            ns.CopyTo(fstream);
                            ns.Close();
                            downloadSocketConnected.Close();
                            downloadSocket.Close();
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                InfoDialog id = new InfoDialog("Загрузка завершена", "Загрузка файла " + fileName + " завершена");
                                id.Show();
                            }));
                        }
                    }).Start();
                };
            }

            lvi.PreviewMouseRightButtonDown += delegate (object sender, MouseButtonEventArgs e) { e.Handled = true;};
            lvi.Selected += delegate
            {
                if (isFolder)
                {
                    ShowDir sd = new ShowDir();
                    sd.Type = "start";
                    sd.Name = App.name;
                    sd.Dir = dir;
                    sd.DirName = fileName;
                    if (sc.mode != 0) sd.Code = App.code % sc.mode;
                    writer.WriteLine(JsonConvert.SerializeObject(sd));
                    writer.Flush();
                }
            };
        }
    }
}
