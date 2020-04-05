using System;
using System.Net;
using System.Threading;

namespace MySweetyPhone_PC.Tools
{
    public class Session
    {
        public IPAddress address { get; protected set; }
        public int port { get; protected set; }
        public int type { get; protected set; }
        public Thread t{ get; protected set; }
        public const int BroadcastingPort = 9000;
        public const int
            NONE = 0,
            MOUSE = 1,
            FILEVIEW = 2,
            SMSVIEW = 3,
            KEYBOARD = 4;



        public static String decodeType(int i)
        {
            switch (i)
            {
                default:
                case NONE:
                    return "Пусто";
                case MOUSE:
                    return "Эмуляция ввода";
                case FILEVIEW:
                    return "Просмотр файлов";
                case SMSVIEW:
                    return "Просмотр СМС";
                case KEYBOARD:
                    return "Удаленная клавиатура Android";
            }
        }
    }
}
