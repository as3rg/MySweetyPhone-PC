using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MySweetyPhone_PC.Tools
{
    class SessionClient : Session
    {
        public int mode { get; private set; }
        public SessionClient(IPAddress address, int Port, int type, int mode)
        {
            this.address = address;
            this.port = Port;
            this.type = type;
            this.mode = mode;

            switch (type)
            {
                case KEYBOARD:
                case MOUSE:

                    break;
                case FILEVIEW:

                    break;
                case SMSVIEW:
                    break;
                default:
                    throw new Exception("Неизвестный тип сессии");
            }
        }
    }
}
