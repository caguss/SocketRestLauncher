using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

using System.Threading.Tasks;

namespace SocketServerLauncher
{
    /// <summary>
    /// 소켓 통신 클라이언트 클래스..
    /// </summary>
    public class CoFAS_SocketClient : IDisposable
    {
        static private string logname = "sp";
        public static bool uselog = false;
        public CoFAS_Log _pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", logname, 30, uselog);
        IPEndPoint remoteEP;

        /// <summary>
        /// Client Socket 
        /// </summary>
        Socket objClient;


        /// <summary>
        /// Server의 IP
        /// </summary>
        public string strServerIP = "";

        /// <summary>
        /// 서버 포트 번호
        /// </summary>
        public int iPort = 0;

        // Client 및 Server의 Send Buffer
        //const int SEND_BUFFER_SIZE = 7168;
        const int SEND_BUFFER_SIZE = 14336;
        private byte[] sendBuffer = new byte[SEND_BUFFER_SIZE];

        // Client 및 Server의 Receive Buffer
        //const int READ_BUFFER_SIZE = 7168;
        const int READ_BUFFER_SIZE = 14336;

        private byte[] readBuffer = new byte[READ_BUFFER_SIZE];

        public delegate void delReceive(byte[] yReceiveData);
        /// <summary>
        /// Receive 이벤트를 사용하기 위한 변수를 선언한다. 
        /// </summary>
        public delReceive evtReceived;

        public void Dispose()
        {
            this.Close();

            objClient = null;

        }

        public CoFAS_SocketClient()
        {
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="pIP">서버 IP</param>
        /// <param name="pPort">서버 Port</param>
        public CoFAS_SocketClient(string pIP, int pPort)
        {
            try
            {
                strServerIP = pIP;
                iPort = pPort;
            }
            catch(Exception)
            {
                _pCoFAS_Log.WLog("CoFAS_SocketClient : " + pIP+":"+pPort.ToString());
            }
        }

        public bool isConnected
        {
            get
            {
                if (objClient == null)
                    return false;
                else
                    return objClient.Connected;
            }
        }

        public string Logname
        {
            get
            {
                return logname;
            }

            set
            {
                logname = value;
            }
        }

        public bool Open()
        {
            try
            {
                // Server의 IP Address와 Port 을 변수에 담자.
                remoteEP = new IPEndPoint(IPAddress.Parse(strServerIP), iPort);
                _pCoFAS_Log.StrFileName = logname;
                objClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _pCoFAS_Log.WLog("Client Open : " + strServerIP + ":" + iPort.ToString());

                // 일단 여기서는 Async 을 사용하지 말자
                // 따라서 Client_ConnectCallback 함수는 만들어만 놓고 사용하지는 않는다.
                //objClient.BeginConnect(remoteEP, new AsyncCallback(Client_ConnectCallback), objClient);
                objClient.Connect(remoteEP);
                try
                {
                    objClient.BeginReceive(readBuffer, 0, readBuffer.Length, 0, new AsyncCallback(ReceiveThread), objClient);
                }
                catch(ObjectDisposedException Objexception)
                {
                    _pCoFAS_Log.WLog("ObjectDisposedException1 : " + Objexception.ToString());
                    return false;
                }

                //strMessage = "Client 연결 성공";
                return true;
            }
            catch (ObjectDisposedException ObjException)
            {
                //_pCoFAS_Log.WLog("ObjectDisposedException : " + ObjException.ToString());
                return false;
            }
          
        }

        public bool Close()
        {
            try
            {

                // Client 을 종료하자.
                if (objClient != null)
                {
                    
                    objClient.Shutdown(SocketShutdown.Both);
                    objClient.BeginDisconnect(true, null, objClient);

                    // Wait for the disconnect to complete.
                    //disconnectDone.WaitOne();
                    if (objClient.Connected)
                        Console.WriteLine("We're still connected");
                    else
                        Console.WriteLine("We're disconnected");
                    objClient.Close();
                    objClient = null;
                }
                return true;
            }
            catch (Exception ex)
            {
                _pCoFAS_Log.WLog("Close Exception :" + ex.ToString());
                //log.WLog_Exception("Close : Exception", ex);
                return false;
                //throw ex;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // socket에서 데이터를 수신하는 부분임
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void ReceiveThread(IAsyncResult ar)
        {
            int intBytesRead = 0;

            try
            {
                intBytesRead = objClient.EndReceive(ar);
                if (intBytesRead < 1)
                {

                    //if no bytes were read server has close.  Disable input window.
                    // Start a new asynchronous read into readBuffer.
                    objClient.BeginReceive(readBuffer, 0, readBuffer.Length, 0, new AsyncCallback(ReceiveThread), objClient);
                    return;
                }

                //수신한 정보를 byte[]에 담자.
                byte[] tmpByte = new byte[intBytesRead];

                //Array.Copy(readBuffer, 0, tmpByte, 0, intBytesRead);
                for (int i = 0; i < intBytesRead; i++)
                {
                    tmpByte[i] = readBuffer[i];
                }


                evtReceived?.Invoke(tmpByte);
                //_pCoFAS_Log.WLog(readBuffer.Count().ToString());
                // Ensure that no other threads try to use the stream at the same time.
                objClient.BeginReceive(readBuffer, 0, readBuffer.Length, 0, new AsyncCallback(ReceiveThread), objClient);
            }
            catch(ObjectDisposedException ObjException)
            {
                //_pCoFAS_Log.WLog("ReceiveThread ObjectDisposedException : " + ObjException.ToString());
            }
            catch (NullReferenceException NullException)
            {
                if (objClient != null) objClient.Dispose();
                //_pCoFAS_Log.WLog("ReceiveThread NullReferenceException :" + NullException.ToString());

            }
            catch (SocketException pSocketException)
            {
                _pCoFAS_Log.WLog("ReceiveThread SocketException :" + pSocketException.ToString());
                //log.WLog_Exception("ReceiveThread : SocketException", pSocketException);
            }
            catch (Exception ex)
            {
                // objTcpClient 가 null 이 아니면 ReceiveThread 을 실행해야 한다.
                //if (objClient != null)
                //{
                //    objClient.Dispose();
                //    // Start a new asynchronous read into readBuffer.
                //    objClient.BeginReceive(readBuffer, 0, readBuffer.Length, 0, new AsyncCallback(ReceiveThread), objClient);
                //}

                ar = null;

                _pCoFAS_Log.WLog("ReceiveThread Exception :" + ex.ToString());
                return;
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Client을 통해 데이터를 전송하면서 그 Receive 을 선언하는 부분
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void SendCallback(IAsyncResult ar)
        {
            //objClient = (Socket) ar.AsyncState;

            try
            {
                int bytesSent = objClient.EndSend(ar);
                //objClient.EndAccept
                _pCoFAS_Log.WLog("test : " +bytesSent.ToString() /*+  " / "+ BitConverter.ToString(readBuffer)*/);
                objClient.BeginReceive(readBuffer, 0, readBuffer.Length, 0, new AsyncCallback(ReceiveThread), objClient);
            }
            catch (SocketException pSocketException)
            {
                _pCoFAS_Log.WLog("SendCallback SocketException :" + pSocketException.ToString());

                //throw new Exception(ex.Message, ex);
                //log.WLog_Exception("SendCallback : SocketException", pSocketException);
            }
            catch (Exception ex)
            {
                _pCoFAS_Log.WLog("SendCallback Exception :" + ex.ToString());

                //log.WLog_Exception("SendCallback : Exception", ex);
                //throw;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Client을 통해 데이터를 전송하는 부분
        // Send 함수는 4개의 오버로드 임
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // 데이터를 socket 을 사용하여 전송하자
        // 데이터만 전송하는 경우
        public bool Send(string strSendData)
        {
            try
            {
                //socket을 사용하여 Server에 Data 보내기  
                sendBuffer = Encoding.Default.GetBytes(strSendData);
                objClient.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), objClient);

                return true;
            }
            catch (SocketException pSocketException)
            {
                _pCoFAS_Log.WLog("Send SocketException :" + pSocketException.ToString());

                //log.WLog_Exception("Send_string : SocketException", pSocketException);
                return false;
                //throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _pCoFAS_Log.WLog("Send Exception :" + ex.ToString());

                //log.WLog_Exception("Send_string : Exception", ex);
                return false;
                //throw;
            }
        }

        // 데이터를 socket 을 사용하여 전송하자
        // 데이터만 전송하는 경우
        public bool Send(byte[] byteSendData)
        {
            try
            {
                //socket을 사용하여 Server에 Data 보내기  
                sendBuffer = byteSendData;
                objClient.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, new AsyncCallback(SendCallback), objClient);

                return true;
            }
            catch (SocketException pSocketException)
            {
                _pCoFAS_Log.WLog("Send byteSendData SocketException :" + pSocketException.ToString());

                //log.WLog_Exception("Send_byte : SocketException", pSocketException);
                return false;
                //throw new Exception(ex.Message, ex);
            }
            catch (Exception ex)
            {
                _pCoFAS_Log.WLog("Send byteSendData Exception :" + ex.ToString());

                //log.WLog_Exception("Send_byte : Exception", ex);
                return false;
                //throw;
            }
            finally
            {
                sendBuffer.Initialize();
                //sendBuffer = null;
            }
        }

        public bool Send_Sync(byte[] byteSendData)
        {
            try
            {
                objClient.Send(byteSendData);

                return true;
            }
            catch (SocketException)
            {
                //log.WLog_Exception("Send_byte : SocketException", pSocketException);
                //throw new Exception(ex.Message, ex);
                return false;
            }
            catch (Exception)
            {
                //log.WLog_Exception("Send_Sync : Exception", ex);
                // throw;
                return false;
            }
        }

        private string byteToString(byte[] yDATA)
        {
            int yLen = yDATA.Length;
            string strData = string.Empty;
            for (int i = 0; i < yLen; i++)
            {
                strData += yDATA[i].ToString("X2");
            }

            return strData;
        }

        private static void DisconnectCallback(IAsyncResult ar)
        {
            // Complete the disconnect request.
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);

            // Signal that the disconnect is complete.
        }

    }
}
