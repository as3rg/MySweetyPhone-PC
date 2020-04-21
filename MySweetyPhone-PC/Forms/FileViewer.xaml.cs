using MaterialDesignThemes.Wpf;
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

        class Respond
        {
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
                    Console.WriteLine(1);
                    Respond msg = JsonConvert.DeserializeObject<Respond>(Encoding.UTF8.GetString(bytes.ToArray()));
                    switch (msg.Type)
                    {
                        case "finish":
                            this.Close();
                            tcp.Close();
                            return;
                        case "deleteFile":
                            if (msg.State == 1)
                            {
                                InfoDialog id = new InfoDialog("Ошибка", "Нет доступа");
                                id.ShowDialog();
                            }
                            else
                            {
                                reloadFolder(null, null);
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
                                    Path.Text = msg.Dir;
                                    foreach (Respond.File f in msg.Inside)
                                    {
                                        Draw(f.Name, f.Type == "Folder", msg.Dir);
                                    }
                                }
                            }));
                            break;
                        case "newDirAnswer":
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
            sd.Dir = Path.Text;
            if (sc.mode != 0) sd.Code = App.code % sc.mode;
            writer.WriteLine(JsonConvert.SerializeObject(sd));
            writer.Flush();
        }

        public void newFolder(object sender, RoutedEventArgs e)
        {
            InputDialog id = new InputDialog("Имя папки","Введите имя папки");
            id.ShowDialog();
            Console.WriteLine(id.Result.Text);
            ShowDir sd = new ShowDir();
            sd.Type = "newDir";
            sd.Name = App.name;
            sd.Dir = Path.Text;
            sd.DirName = id.Result.Text;
            if (sc.mode != 0) sd.Code = App.code % sc.mode;
            writer.WriteLine(JsonConvert.SerializeObject(sd));
            writer.Flush();
        }

        public void uploadFile(object sender, RoutedEventArgs e)
        {
            /*
            FileChooser fc = new FileChooser();
            fc.setTitle("Выберите файл для загрузки");
            final File file = fc.showOpenDialog(null);
            if (file == null) return;

            new Thread(()-> {
            try
            {
                ServerSocket ss = new ServerSocket(0);
                JSONObject msg2 = new JSONObject();
                msg2.put("Type", "uploadFile");
                msg2.put("Name", name);
                if (!login.isEmpty()) msg2.put("Login", login);
                msg2.put("FileName", file.getName());
                msg2.put("FileSocketPort", ss.getLocalPort());
                msg2.put("Dir", Path.getText());
                writer.println(msg2.toJSONString());
                writer.flush();
                Socket socket = ss.accept();
                DataOutputStream fileout = new DataOutputStream(socket.getOutputStream());
                FileInputStream filein = new FileInputStream(file);
                IOUtils.copy(filein, fileout);
                fileout.flush();
                filein.close();
                socket.close();
                Platform.runLater(()->{
                    Main.trayIcon.displayMessage("Отправка завершена", "Файл \"" + file.getName() + "\" загружен", TrayIcon.MessageType.INFO);
                    reloadFolder(null);
                });
            }
            catch (IOException e)
            {
                e.printStackTrace();
            }
        }).start();*/
        }

        public void reloadFolder(object sender, RoutedEventArgs e)
        {
            ShowDir sd = new ShowDir();
            sd.Type = "showDir";
            sd.Name = App.name;
            sd.Dir = Path.Text;
            sd.DirName = "";
            if (sc.mode != 0) sd.Code = App.code % sc.mode;
            writer.WriteLine(JsonConvert.SerializeObject(sd));
            writer.Flush();
        }

        public void Draw(String fileName, bool isFolder, String dir)
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
                else
                {
                    //TODO доделать скачку
                }
            };
            /*
        Label folder = new Label(fileName);
        files.add(fileName);
        folder.setPadding(new Insets(20, 20, 20, 20));
        folder.setFont(new Font(14));
        Folders.getChildren().add(folder);

        final ContextMenu contextMenu = new ContextMenu();
        javafx.scene.control.MenuItem delete = new javafx.scene.control.MenuItem("Удалить");
        contextMenu.getItems().addAll(delete);
        delete.setOnAction(event -> {
        new Thread(()-> {
                JSONObject msg2 = new JSONObject();
        msg2.put("Type", "deleteFile");
        msg2.put("Name", name);
        if (!login.isEmpty()) msg2.put("Login", login);
        msg2.put("FileName", fileName);
        msg2.put("Dir", dir);
        writer.println(msg2.toJSONString());
        writer.flush();
        }).start();

        });

        if(isFolder) {
            folder.setText("📁 " + folder.getText());
            folder.setOnMouseClicked(v -> {
                if(v.getButton() == MouseButton.PRIMARY) new Thread(() -> {
            JSONObject msg3 = new JSONObject();
            msg3.put("Type", "showDir");
            msg3.put("Name", name);
            if (!login.isEmpty()) msg3.put("Login", login);
            msg3.put("Dir", dir);
            msg3.put("DirName", fileName);
            writer.println(msg3.toJSONString());
            writer.flush();
        }).start();
        });
        }else {
            folder.setText("📄 "+folder.getText());
            javafx.scene.control.MenuItem save = new javafx.scene.control.MenuItem("Сохранить как");
        contextMenu.getItems().addAll(save);
        save.setOnAction(v -> {
                DirectoryChooser fc = new DirectoryChooser();
        fc.setTitle("Выберите папку для сохранения");
                final File out = fc.showDialog(null);
                if (out == null) return;
                new Thread(() -> {
        try
        {
        File out3 = new File(out, "MySweetyPhone");
        out3.mkdirs();
        File out2 = new File(out3, fileName);

        ServerSocket ss = new ServerSocket(0);
        JSONObject msg2 = new JSONObject();
        msg2.put("Type", "downloadFile");
        msg2.put("Name", name);
        if (!login.isEmpty()) msg2.put("Login", login);
        msg2.put("FileName", fileName);
        msg2.put("FileSocketPort", ss.getLocalPort());
        msg2.put("Dir", dir);
        writer.println(msg2.toJSONString());
        writer.flush();
        Socket socket = ss.accept();
        DataInputStream filein = new DataInputStream(socket.getInputStream());
        FileOutputStream fileout = new FileOutputStream(out2);
        IOUtils.copy(filein, fileout);
        fileout.close();
        socket.close();
        Platform.runLater(()->Main.trayIcon.displayMessage("Загрузка завершена", "Файл \"" + out2.getName() + "\" загружен", TrayIcon.MessageType.INFO));
        }
        catch (IOException e)
        {
        e.printStackTrace();
        }
        }).start();
            });
        }
        folder.setOnContextMenuRequested((EventHandler<Event>) event -> contextMenu.show(folder, MouseInfo.getPointerInfo().getLocation().x, MouseInfo.getPointerInfo().getLocation().y));
        */
        }
    }
}
