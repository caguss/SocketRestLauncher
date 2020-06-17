using DevExpress.XtraEditors;
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
    public class SyncServer
    {
        ListBox log;
        LabelControl clicount;

        Dictionary<string, DateTime> client_log = new Dictionary<string, DateTime>(); //클라이언트 확인용 딕셔너리
        Dictionary<string, DateTime> list;
        Socket syncserver = null;
        Socket syncclient = null;
        static byte[] receiveBytes = new byte[1000];
        private int portnum;
        bool finish_accept = true;
        IPEndPoint ipep;


        static private string logname = "sp";
        public static bool uselog = false;
        public CoFAS_Log _pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", logname, 30, uselog);
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

        public SyncServer(ListBox logbox, LabelControl lblcount)
        {
            this.log = logbox;
            this.clicount = lblcount;
            syncserver = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp);


        }


        public void Start()
        {
            bool finish_receive = true;
            if (ipep == null)
            {
                syncserver = new Socket(
                     AddressFamily.InterNetwork,
                     SocketType.Stream,
                     ProtocolType.Tcp);
                syncclient = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp);
                ipep = new IPEndPoint(IPAddress.Any, portnum);
                syncserver.Bind(ipep);
                _pCoFAS_Log.WLog("server open : " + portnum.ToString());

            }

            syncserver.Listen(100);

            finish_accept = true;
            while (finish_accept) // accept
            {
                try
                {
                    syncclient = syncserver.Accept();
                    syncclient.ReceiveTimeout = 5000; // 5초 지나면 exception 으로 예외처리.

                    log.Invoke(new Action(delegate ()
                    {
                        log.Items.Add("클라이언트 연결요청 : " + syncclient.RemoteEndPoint.ToString());//데이터 처리
                        log.SelectedIndex = log.Items.Count - 1;
                    }));

                    _pCoFAS_Log.WLog("client connected : " + syncclient.RemoteEndPoint.ToString());
                    if (checklog(syncclient))
                    {
                        client_log.Add(syncclient.RemoteEndPoint.ToString().Split(':')[0], DateTime.Now);
                    }
                    finish_receive = true;
                }
                catch (Exception)
                {
                }
                
                while (finish_receive) // 클라이언트 receive
                {
                    try
                    {
                        receiveBytes = new byte[1000];
                        int receiveLength = syncclient.Receive(receiveBytes, receiveBytes.Length, SocketFlags.None);
                        string data = ByteToString(receiveBytes);


                        byte[] tmpByte = new byte[receiveLength];
                        for (int i = 0; i < receiveLength; i++)
                        {
                            tmpByte[i] = receiveBytes[i];
                        }
                        byte[] endByte = new byte[] { 0x24, 0x11, 0x02, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x35, 0x25 };
                        if (tmpByte.SequenceEqual(endByte))
                        {
                            break;
                        }
                        else
                        {
                            _pCoFAS_Log.WLog(syncclient.RemoteEndPoint.ToString() + " : " + data);

                            log.Invoke(new Action(delegate ()
                            {
                                log.Items.Add(syncclient.RemoteEndPoint.ToString() + " : " + data);//데이터 처리
                                log.SelectedIndex = log.Items.Count - 1;

                            }));
                        }

                    }
                    catch (SocketException ex)
                    {
                        finish_receive = false;
                        ipep = null;
                        _pCoFAS_Log.WLog("SocketException : " + ex.Message);
                        syncclient.Close();
                    }
                }
            }
            syncserver.Close();

        }

        private bool checklog(Socket soc)
        {
            foreach (string item in client_log.Keys)
            {
                if (item == soc.RemoteEndPoint.ToString().Split(':')[0])
                {
                    return false;
                }
            }
            list = client_log;
            return true;
        }

        public void timerremove()
        {
            if (list != null)
            {
                try
                {
                    foreach (KeyValuePair<string, DateTime> item in list.ToList())
                    {
                        if (item.Value.AddSeconds(5) < DateTime.Now)
                        {
                            client_log.Remove(item.Key);
                            log.Invoke(new Action(delegate ()
                            {
                                log.Items.Add("disconnect : " + item.Key);
                                log.SelectedIndex = log.Items.Count - 1;
                                clicount.Text = (Convert.ToInt32(clicount.Text) - 1).ToString();
                                _pCoFAS_Log.WLog("disconnect : " + item.Key);
                            }));
                        }
                    }

                }
                catch (InvalidOperationException ex)
                {

                }
                clicount.Invoke(new Action(delegate ()
                {
                    clicount.Text = client_log.Count().ToString();
                }));
            }
        }
        public void End()
        {
            ipep = null;
            finish_accept = false;
            _pCoFAS_Log.WLog("server close : " + portnum.ToString());
            syncserver.Close();

        }


        private string ByteToString(byte[] strByte) { string str = Encoding.Default.GetString(strByte); return str; }

    }
}
