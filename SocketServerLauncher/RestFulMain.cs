using Grapevine.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketServerLauncher
{
    public class RestFulMain
    {
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
        public void Reststart(RestServer ser)
        {
            using (var server = ser)
            {
                server.Host = Local_IP;
                server.UseHttps = false;
                server.Start();

                while (server.IsListening)
                {
                    Thread.Sleep(300);
                }
            }
            
        }

        public void Reststop(RestServer ser)
        {
            using (var server = ser)
            {
                server.Stop();
            }
        }
    }
}
