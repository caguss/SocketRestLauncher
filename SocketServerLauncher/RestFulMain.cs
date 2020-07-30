using Grapevine.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static SocketServerLauncher.api.checkin;
using static SocketServerLauncher.api.datain;
namespace SocketServerLauncher
{

    public class RestFulMain
    {
        static private string logname = "sp";
        public CoFAS_Log _pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", logname, 30, false);
        RestServer server;
        ListBox logbox; 
        private static string strrr = AppDomain.CurrentDomain.BaseDirectory;
        public string str { get { return strrr; } }

        public static string Local_IP
        {
            get
            {

                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                string ClientIP = string.Empty;
                for (int i = 0; i < host.AddressList.Length; i++)
                {
                    if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        ClientIP = host.AddressList[i].ToString();
                    }
                }
                return ClientIP;
            }
        }


        public void Reststart(RestServer ser, ListBox logbox)
        {
            this.logbox = logbox;
            api.datain.TestResource._pCoFAS_Log = _pCoFAS_Log;
            api.checkin.TestResource._pCoFAS_Log = _pCoFAS_Log;
            api.checkin.TestResource.Logbox = logbox;
            api.datain.TestResource.Logbox = logbox;

            try
            {


                server = ser;
                server.UseHttps = false;
                server.Host = Local_IP;//Local_IP;
                                       //server.Host = Local_IP;
                                       //server.Port = "8090";

                server.LogToConsole().Start();

                logbox.Invoke(new Action(delegate ()
                {
                    logbox.Items.Insert(0, "서버실행 성공!");
                            //logbox.SelectedIndex = logbox.Items.Count - 1;
                        }));
                _pCoFAS_Log.WLog("server open : " + Local_IP);

                while (server.IsListening)
                {
                    Thread.Sleep(300);
                }


            }
            catch (Exception ex)
            {
                logbox.Invoke(new Action(delegate ()
                {
                    logbox.Items.Insert(0, "서버실행 실패!");
                    logbox.Items.Insert(0, ex.Message);
                    //logbox.SelectedIndex = logbox.Items.Count - 1;
                }));
                _pCoFAS_Log.WLog("server failed : " + ex.Message);
            }
        }

        public void Reststop(RestServer ser)
        {
            try
            {
                //ser.ThreadSafeStop();
                ser.Stop();
                //ser.Dispose();
                _pCoFAS_Log.WLog("server stop : " + Local_IP);

            }
            catch
            { }
        }
        public void Reststop()
        {
            try
            {
                //ser.ThreadSafeStop();
                server.Stop();
                //ser.Dispose();
                _pCoFAS_Log.WLog("server stop : " + Local_IP);

            }
            catch
            { }
        }
    }
}
