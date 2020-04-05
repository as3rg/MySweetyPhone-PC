using MySweetyPhone_PC.Tools;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MySweetyPhone_PC.Forms
{
    /// <summary>
    /// Логика взаимодействия для MouseTracker.xaml
    /// </summary>
    public partial class MouseTracker : Window
    {
        class Start
        {
            public string Type, Name;
            public long Code;
        }
        public MouseTracker(SessionClient sc)
        {
            InitializeComponent();
            this.sc = sc;
            this.MouseDown += mousePressed;
            this.MouseUp += mouseReleased;
            this.MouseMove += mouseMoved;
            //this.MouseWheel += mouseWheelMoved;
            Text.PreviewKeyDown += keyPressed;
            Text.PreviewKeyUp += keyReleased;
            Text.TextChanged += textChanged;

            socket = new UdpClient();

            Start st = new Start();
            st.Type = "start";
            st.Name = App.name;
            if (sc.mode != 0) st.Code = App.code % sc.mode;
            Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(st)));
            Text.LostKeyboardFocus += delegate
            {
                if(this.IsActive) Keyboard.Focus(Text);
            };
            this.Activated += delegate
            {
                Keyboard.Focus(Text);
            };
        }

        public const int MESSAGESIZE = 100;

        SessionClient sc;

        UdpClient socket;

        Point lastPoint;

        class MousePressed
        {
            public string Type, Name;
            public MouseButton Key;
            public long Code;

        }
        public void mousePressed(object sender, MouseButtonEventArgs e)
        {
            MousePressed mp = new MousePressed();
            mp.Type = "mousePressed";
            mp.Key = e.ChangedButton;
            mp.Name = App.name;
            if (sc.mode != 0) mp.Code = App.code % sc.mode;
            Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mp)));
        }

        /*public void dragDropped(DragEvent e)
        {
            MouseEvent me = new MouseEvent(MouseEvent.MOUSE_RELEASED, 0, 0, 0, 0, MouseButton.PRIMARY, 0, true, true, true, true, true, true, true, true, true, true, null);
            mouseReleased(me);
        }*/

        public void mouseReleased(object sender, MouseButtonEventArgs e)
        {
            MousePressed mp = new MousePressed();
            mp.Type = "mouseRealised";
            mp.Key = e.ChangedButton;
            mp.Name = App.name;
            if (sc.mode != 0) mp.Code = App.code % sc.mode;
            Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mp)));
        }

        class MouseMoved
        {
            public string Type, Name;
            public long Code;
            public double X, Y;

        }

        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);


        private void mouseMoved(object sender, MouseEventArgs e)
        {
            MouseMoved mv = new MouseMoved();
            mv.Type = "mouseMoved";
            mv.Name = App.name;
            mv.Code = App.code % sc.mode;
            mv.X = e.GetPosition(this).X - lastPoint.X;
            mv.Y = e.GetPosition(this).Y - lastPoint.Y;
            if (sc.mode != 0) mv.Code = App.code % sc.mode;
            Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(mv)));
            lastPoint = e.GetPosition(this);

            if (e.GetPosition(this).X <= 1 || e.GetPosition(this).X >= this.Width - 1 || e.GetPosition(this).Y <= 1 || e.GetPosition(this).Y >= this.Height - 1)
            {
                lastPoint = new Point((int)Width / 2, (int)Height / 2);
                SetCursorPos((int)Width / 2, (int)Height / 2);
            }
        }

        /*public void mouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            JSONObject msg = new JSONObject();
            msg.put("Type", "mouseWheel");
            double value = -e.getDeltaY() / 10;
            value = value > 0 ? Math.ceil(value) : -Math.ceil(-value);
            msg.put("value", value);
            msg.put("Name", App.name);
            msg.put("Code", App.code % sc.mode);
            Send(msg.toJSONString().getBytes());
        }*/

        /*public void keyPressed(object sender, KeyboardEventArgs e)
        {
            JSONObject msg = new JSONObject();
            if (e.isControlDown() && e.getCode().equals(KeyCode.F4))
            {
                msg.put("Type", "finish");
                msg.put("Name", App.name);
                msg.put("Code", App.code % sc.mode);
                Send(msg.toJSONString().getBytes());
                s.close();
            }
            else if (e.isControlDown() && e.getCode().equals(KeyCode.PRINTSCREEN))
            {
                DirectoryChooser fc = new DirectoryChooser();
                fc.setTitle("Выберите папку для сохранения");
                final File out = fc.showDialog(null);
                if (out == null) return;
                new Thread(()-> {
                    try
                {
                    ServerSocket ss = new ServerSocket(0);
                    JSONObject msg2 = new JSONObject();
                    msg2.put("Type", "makeScreenshot");
                    msg2.put("Port", ss.getLocalPort());
                    msg2.put("Name", name);
                    if (!login.isEmpty()) msg2.put("Login", login);
                    Send(msg2.toJSONString().getBytes());
                    ss.setSoTimeout(10000);
                    Socket socket = ss.accept();
                    BufferedImage image = ImageIO.read(socket.getInputStream());

                    File out2 = new File(out, "MySweetyPhone");
                    out2.mkdirs();
                    String fileName = "Screenshot_" + new SimpleDateFormat("HH_mm_ss_dd_MM_yyyy").format(System.currentTimeMillis()) + ".png";
                    FileOutputStream fos = new FileOutputStream(new File(out2, fileName));
                    ImageIO.write(image, "png", fos);
                    fos.close();
                    socket.close();
                    ss.close();

                    Platform.runLater(()->Main.trayIcon.displayMessage("Скриншот получен", "Скриншот сохранен в файл \"" + fileName + "\"", TrayIcon.MessageType.INFO));
                }
                catch (IOException e2)
                {
                    e2.printStackTrace();
                }
            }).start();
        }else if (!(sc.getType() == Session.KEYBOARD) || e.getText().isEmpty())
            {
                msg.put("Type", "keyPressed");
                msg.put("value", e.getCode().getCode());
                msg.put("Name", name);
                if (!login.isEmpty()) msg.put("Login", login);
                Send(msg.toJSONString().getBytes());
    }
    textArea.setText("");
}*/

        class KeyPressed
        {
            public string Type, Name;
            public Key value;
            public long Code;

        }

        class KeysTyped
        {
            public string Type, Name, value;
            public long Code;

        }

        public void keyReleased(object sender, KeyEventArgs e)
        {
            KeyPressed kp = new KeyPressed();
            kp.Type = "keyReleased";
            kp.value = e.Key;
            kp.Name = App.name;
            if (sc.mode != 0) kp.Code = App.code % sc.mode;
            Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(kp)));
        }

        public void keyPressed(object sender, KeyEventArgs e)
        {
            String str = new KeyConverter().ConvertToString(e.Key);
            if (str.Length > 1 && !str.StartsWith("Oem") && str != "Space")
            {
                Text.Text = "";
                KeyPressed kp = new KeyPressed();
                kp.Type = "keyPressed";
                kp.value = e.Key;
                kp.Name = App.name;
                if (sc.mode != 0) kp.Code = App.code % sc.mode;
                Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(kp)));
            }
        }

        public void textChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {
            if (Text.Text != "")
            {
                KeysTyped kt = new KeysTyped();
                kt.Type = "keysTyped";
                kt.value = Text.Text;
                kt.Name = App.name;
                if (sc.mode != 0) kt.Code = App.code % sc.mode;
                Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(kt)));
                Text.Text = "";
            }
        }

        public void Send(byte[] b)
        {
            socket.Send(b, b.Length, new IPEndPoint(sc.address, sc.port));
        }
    }
}
