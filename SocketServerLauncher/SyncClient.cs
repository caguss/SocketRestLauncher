using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SocketServerLauncher
{
    class SyncClient
    {
        ListBox log;
        Socket client_socket;

        static private string logname = "sp";
        private static bool uselog = false;
        public CoFAS_Log _pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", logname, 30, uselog);

        static byte[] sendBytes = new byte[1000];
        private int portnum;
        private string host;

        public ListBox Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        public int Portnum
        {
            get
            {
                return portnum;
            }

            set
            {
                portnum = value;
            }
        }

        public string Host
        {
            get
            {
                return host;
            }

            set
            {
                host = value;
            }
        }

        public static bool Uselog
        {
            get
            {
                return uselog;
            }

            set
            {
                uselog = value;
            }
        }

        public SyncClient(ListBox logbox, string cliname)
        {
            log = logbox;
            logname = cliname;
        }

        public void Start()
        {
            log.Invoke(new Action(delegate ()
            {
                log.Items.Add("동기식 서버 연결");
                log.SelectedIndex = log.Items.Count - 1;

            }));

            _pCoFAS_Log.StrFileName = logname;

            _pCoFAS_Log.WLog("Server Connected - " + host + " : " + portnum);

            client_socket = new Socket(
              AddressFamily.InterNetwork,
              SocketType.Stream,
              ProtocolType.Tcp);
            client_socket.Connect(host, portnum);



        }

        public void End()
        {
            client_socket.Close();
            client_socket = null;
        }

        public void SendData(string data)
        {
            try
            {
                sendBytes = StringToByte(data);
                client_socket.Send(sendBytes);
                _pCoFAS_Log.WLog("Send data : " + data);

                log.Invoke(new Action(delegate ()
                {
                    log.Items.Add("데이터 전송 : " + ByteToString((sendBytes))); //데이터 전송
                    log.SelectedIndex = log.Items.Count - 1;

                }));
            }
            catch (Exception ex)
            {

                log.Invoke(new Action(delegate ()
                {
                    log.Items.Add(ex.Message); //
                    log.SelectedIndex = log.Items.Count - 1;

                }));
            }
           
            
        }
        public void SendData(byte[] data)
        {
            try
            {
                client_socket.Send(data);
            }
            catch (Exception ex)
            {

                log.Invoke(new Action(delegate ()
                {
                    log.Items.Add(ex.Message); //
                    log.SelectedIndex = log.Items.Count - 1;

                }));
            }


        }

        private string ByteToString(byte[] strByte) { string str = Encoding.Default.GetString(strByte); return str; }
        private byte[] StringToByte(string str) { byte[] StrByte = Encoding.UTF8.GetBytes(str); return StrByte; }
    }
}
