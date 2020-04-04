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
        public static long code;
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
            code = long.Parse(ini.GetPrivateString("Main", "code", "0"));
            if (App.name != "" )
                this.StartupUri = new Uri("Forms/Main.xaml", UriKind.Relative);
            else
                this.StartupUri = new Uri("Forms/RegDevice.xaml", UriKind.Relative);
        }
    }
}
