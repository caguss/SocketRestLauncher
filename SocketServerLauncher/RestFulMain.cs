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
        {try
            {
                using (var server = ser)
                {
                    
                    server.UseHttps = false;
                    server.Host = "192.168.43.37";//Local_IP;
                    //server.Host = Local_IP;
                    //server.Port = "8090";
                    server.LogToConsole().Start();
                    Console.WriteLine("서버실행 성공!");
                    while (server.IsListening)
                    {
                        Thread.Sleep(300);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("서버실행 실패!");
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
