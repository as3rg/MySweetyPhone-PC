using MaterialDesignThemes.Wpf;
using MySweetyPhone_PC.Tools;
using Newtonsoft.Json;
using System;
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

        class Respond
        {
            public class File
            {
                public String Name, Type;
            }
            public String Dir, Type;
            public int State;
            public File[] Inside;
        }

        SessionClient sc;
        public FileViewer(SessionClient sc)
        {
            this.sc = sc;
            InitializeComponent();

            Thread receiving = new Thread(() =>
            {
                TcpClient tcp = new TcpClient(sc.address.ToString(), sc.port);
                StreamReader reader = new StreamReader(tcp.GetStream());
                StreamWriter writer = new StreamWriter(tcp.GetStream());
                Start st = new Start();
                st.Type = "start";
                st.Name = App.name;
                if (sc.mode != 0) st.Code = App.code % sc.mode;
                writer.WriteLine(JsonConvert.SerializeObject(st));
                writer.Flush();

                while (true)
                {
                    String line = reader.ReadLine();
                    Console.WriteLine(line);
                    Respond msg = JsonConvert.DeserializeObject<Respond>(line);
                    //JSONObject msg = (JSONObject)JSONValue.parse(line);
                    switch (msg.Type)
                    {
                        case "finish":
                            this.Close();

                            break;
                        case "deleteFile":
                            //if (((Long)msg.get("State")).intValue() == 1)
                            //    Platform.runLater(()->Main.trayIcon.displayMessage("Ошибка", "Нет доступа", TrayIcon.MessageType.INFO));
                            //else reloadFolder(null);
                            break;
                        case "showDir":
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                FilesList.Items.Clear();
                                foreach (Respond.File f in msg.Inside)
                                {
                                    StackPanel sp = new StackPanel();
                                    sp.Margin = new Thickness(10);
                                    sp.Orientation = Orientation.Horizontal;
                                    PackIcon pi = new PackIcon();
                                    pi.Foreground = new SolidColorBrush(Colors.White);
                                    pi.Kind = f.Type == "File" ? PackIconKind.File : PackIconKind.Folder;
                                    pi.Height = pi.Width = 20;
                                    TextBlock tb = new TextBlock();
                                    tb.Text = f.Name;
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
                                        
                                    };
                                }
                            /*JSONArray values = (JSONArray)msg.get("Inside");
                            files.clear();
                            Platform.runLater(()-> {
                                if (((Long)msg.get("State")).intValue() == 1)
                                {
                                    Main.trayIcon.displayMessage("Ошибка", "Нет доступа", TrayIcon.MessageType.INFO);
                                    return;
                                }
                                Folders.getChildren().clear();
                                Path.setText((String)msg.get("Dir"));
                                Back.setVisible(!Path.getText().isEmpty());
                                NewFolder.setVisible(!Path.getText().isEmpty());
                                Upload.setVisible(!Path.getText().isEmpty());
                                Reload.setVisible(!Path.getText().isEmpty());
                                for (int i = 0; i < values.size(); i++)
                                {
                                    JSONObject folder = (JSONObject)values.get(i);
                                    Draw((String)folder.get("Name"), folder.get("Type").equals("Folder"), (String)msg.get("Dir"));
                                }
                            });*/
                            }));
                            break;
                        case "newDirAnswer":
                            //Platform.runLater(()->Draw((String)msg.get("DirName"), true, (String)msg.get("Dir")));
                            break;
                    }
                }
            });
            receiving.Start();
        }

        public void back(object sender, RoutedEventArgs e)
        {
            /*
            JSONObject msg2 = new JSONObject();
            msg2.put("Type", "back");
            msg2.put("Name", name);
            if (!login.isEmpty()) msg2.put("Login", login);
            msg2.put("Dir", Path.getText());
            writer.println(msg2.toJSONString());
            writer.flush();
            */
        }

        public void newFolder(object sender, RoutedEventArgs e)
        {
            /*while (true)
            {
                TextInputDialog dialog = new TextInputDialog();
                dialog.setTitle("Имя папки");
                dialog.setHeaderText("Введите имя папки");
                Optional<String> s = dialog.showAndWait();
                if (s.isPresent() && !s.get().isEmpty())
                {
                    if (
                            s.get().contains("\\")
                                    || s.get().contains("/")
                                    || s.get().contains(":")
                                    || s.get().contains("*")
                                    || s.get().contains("?")
                                    || s.get().contains("\"")
                                    || s.get().contains("<")
                                    || s.get().contains(">")
                                    || s.get().contains("|")
                    )
                    {
                        dialog.setContentText("Имя содержит недопустимые символы");
                    }
                    else if (files.contains(s.get()))
                    {
                        dialog.setContentText("Такая папка уже существует");
                    }
                    else if (s.get().isEmpty())
                    {
                        dialog.setContentText("Имя файла не может быть пустым");
                    }
                    else
                    {
                        new Thread(()-> {
                        JSONObject msg2 = new JSONObject();
                        msg2.put("Type", "newDir");
                        msg2.put("DirName", s.get());
                        msg2.put("Name", name);
                        if (!login.isEmpty()) msg2.put("Login", login);
                        msg2.put("Dir", Path.getText());
                        writer.println(msg2.toString());
                        writer.flush();
                    }).start();
                    break;
                }
            }else break;
        }*/
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
            /*
        new Thread(()-> {
            JSONObject msg3 = new JSONObject();
        msg3.put("Type", "showDir");
        msg3.put("Name", name);
        if (!login.isEmpty()) msg3.put("Login", login);
        msg3.put("Dir", Path.getText());
        writer.println(msg3.toJSONString());
        writer.flush();
        }).start();*/
        }

        public void Draw(String fileName, bool isFolder, String dir)
        {
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
