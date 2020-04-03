using MySweetyPhone_PC.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace MySweetyPhone_PC
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static String name;
        public static String login;
        public static long id;
        public static long regdate;
        public static IniManager ini;

        class Respond
        {
            public long code, result;
        }

        App()
        {
            ini = new IniManager("./Properties.ini");
            /*
            ini.WritePrivateString("Main", "name", "1");
            ini.WritePrivateString("Main", "login", "1223");
            ini.WritePrivateString("Main", "regdate", "1585775065");
            ini.WritePrivateString("Main", "id", "1585775063");
            */
            name = ini.GetPrivateString("Main", "name", "");
            login = ini.GetPrivateString("Main", "login", "");
            regdate = long.Parse(ini.GetPrivateString("Main", "regdate", "0"));
            id = long.Parse(ini.GetPrivateString("Main", "id", "0"));
            String result2 = Request.Get("http://mysweetyphone.herokuapp.com/?Type=Check&DeviceType=PC&RegDate=" + regdate + "&Login=" + WebUtility.UrlEncode(login) + "&Id=" + id + "&Name=" + WebUtility.UrlEncode(name));
            Respond respond = JsonConvert.DeserializeObject<Respond>(result2);
            if ((respond.code == 0 && respond.result == 1) || (App.login == "" && App.name != "" ))
                this.StartupUri = new Uri("Forms/Main.xaml", UriKind.Relative);
            else if(respond.code == 1 || respond.code == 3)
                this.StartupUri = new Uri("Forms/Login.xaml", UriKind.Relative);
            else if(respond.result == 2)
                this.StartupUri = new Uri("Forms/RegDevice.xaml", UriKind.Relative);
        }
    }
}
