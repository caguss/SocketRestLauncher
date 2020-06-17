using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using static SocketServerLauncher.UserControl1;
using System.Resources;
using SocketServerLauncher.Properties;
using static SocketServerLauncher.CoFAS_SocketServer;
using Grapevine.Server;
using System.Xml;
using System.IO;

namespace SocketServerLauncher
{

    public partial class UserControl1 : UserControl
    {

        private ServerEntity ser; //xml에서 받아오는 서버 정보
        ResourceManager rm = Resources.ResourceManager;

        //ip와 포트
        string cnip;
        string cnport;
        //비동기식 서버와 클라이언트
        CoFAS_SocketServer _pSocketServer = null;
        CoFAS_SocketClient _psocketClient = null;

        CSocketPacket theSocPkt = new CSocketPacket();

        RestFulMain newrest = new RestFulMain();
        RestServer newser = new RestServer(); // rest서버
        SyncServer _syncser; //동기식 서버
        SyncClient synccli; //동기식 클라이언트


        public UserControl1(ServerEntity ser)
        {
            this.ser = ser;
            InitializeComponent();

            switch (ser.Server)
            {
                case "server":
                    tableLayoutPanel1.BackColor = Color.LightSkyBlue;
                    lblUserCount.Visible = true;
                    lblcolon.Visible = true;
                    lblUsertext.Visible = true;
                    txtIP.Text = "Port : " + ser.Port;
                    simpleButton2.Visible = false;
                    break;
                case "client":
                    tableLayoutPanel1.BackColor = Color.Gold;
                    txtIP.Text = ser.Ip + " / " + ser.Port;
                    simpleButton2.Enabled = false;
                    break;
                case "rest":
                    tableLayoutPanel1.BackColor = Color.LightGreen;
                    txtIP.Text = "Port : " + ser.Port;
                    simpleButton2.Visible = false;
                    break;
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            ServerEntity loadser = ReadXML(txtName.Text);
            AddForm addfrm = new AddForm(loadser);
            addfrm.Show();
        }

        private void UserControl1_Load(object sender, EventArgs e)
        {
            txtName.Text = ser.Name;
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            cnip = txtIP.Text.Split(' ')[0];
            cnport = txtIP.Text.Split(' ')[2];
            Thread threadReceiveData = null;

            if (simpleButton1.Text == "Disconnect") //연결중일때
            {
                try
                {
                    switch (tableLayoutPanel1.BackColor.Name)
                    {
                        case "LightSkyBlue": //server
                            if (ser.Sync == "async")
                            {
                                _pSocketServer.Stop();
                            }
                            else
                            {
                                _syncser.End();
                            }
                            timer1.Stop();

                            lboxLog.Items.Add("Socket 서버 종료");
                            lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            lblUserCount.Text = "0";
                            ////log.wlog("Socket 서버 종료", txtName.Text);

                            break;
                        case "Gold": //client

                            if (ser.Sync == "sync")
                            {
                                synccli.SendData(new byte[] { 0x24, 0x11, 0x02, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x35, 0x25 });
                                synccli.End();

                            }
                            else if (_psocketClient.isConnected)
                            {
                                _psocketClient.Close();
                            }

                            lboxLog.Items.Add("클라이언트 종료");
                            lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            simpleButton2.Enabled = false;


                            break;
                        case "LightGreen": //rest
                            newrest.Reststop(newser);
                            lboxLog.Items.Add("Rest 서버 종료");
                            lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

                            break;
                    }



                    Bitmap myImage = (Bitmap)rm.GetObject("RED");
                    pbStatus.BackgroundImage = myImage;
                    simpleButton1.Text = "Connect";

                }
                catch (Exception ex)
                {
                    Bitmap myImage = (Bitmap)rm.GetObject("YELLOW");
                    pbStatus.BackgroundImage = myImage;
                    simpleButton1.Text = "Connect";
                    lboxLog.Items.Add(ex.Message);
                    //log.wlog_Exception(sender.ToString(),ex);
                }
            }
            else //연결
            {
                try
                {

                    switch (tableLayoutPanel1.BackColor.Name)
                    {
                        case "LightSkyBlue": //server
                            if (ser.Sync == "async")
                            {
                                timer1.Tick += timer1_Tick_async;
                                //비동기식 서버
                                threadReceiveData = new Thread(new ThreadStart(startserverTrades));
                                threadReceiveData.IsBackground = true;
                                threadReceiveData.Start();
                            }
                            else
                            {
                                timer1.Tick += timer1_Tick_sync;
                                timer1.Interval = 5000;
                                //동기식 서버
                                if (_syncser == null)
                                {
                                    _syncser = new SyncServer(lboxLog, lblUserCount);
                                    _syncser.Portnum = Convert.ToInt32(ser.Port);

                                }
                                _syncser._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
                                lboxLog.Items.Add("동기식 서버 Open");
                                lboxLog.Items.Add("대기중...");
                                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

                                Thread newser = new Thread(new ThreadStart(_syncser.Start));
                                newser.Start();
                                timer1.Start();
                            }

                            break;
                        case "Gold": //client
                            if (ser.Sync == "async")//비동기식 클라이언트
                            {
                                _psocketClient = new CoFAS_SocketClient(cnip, Convert.ToInt32(cnport));
                                _psocketClient.Logname = txtName.Text;
                                _psocketClient._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);

                                _psocketClient.Open();
                                threadReceiveData = new Thread(new ThreadStart(startclientTrades));

                                threadReceiveData.IsBackground = true;
                                threadReceiveData.Start();
                                lboxLog.Items.Add("서버 연결 완료");
                                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            }
                            else   //동기식 클라이언트
                            {

                                synccli = new SyncClient(lboxLog, txtName.Text);
                                synccli._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
                                synccli.Host = ser.Ip;
                                synccli.Portnum = Convert.ToInt32(ser.Port);
                                synccli.Start();
                                synccli.SendData("test");
                            }
                            simpleButton2.Enabled = true;


                            break;
                        case "LightGreen": //rest
                            threadReceiveData = new Thread(new ThreadStart(startrestTrades));
                            threadReceiveData.IsBackground = true;
                            threadReceiveData.Start();
                            break;
                    }




                    Bitmap myImage = (Bitmap)rm.GetObject("GREEN");
                    pbStatus.BackgroundImage = myImage;
                    simpleButton1.Text = "Disconnect";



                }
                catch (Exception ex)
                {
                    Bitmap myImage = (Bitmap)rm.GetObject("YELLOW");
                    pbStatus.BackgroundImage = myImage;
                    simpleButton1.Text = "Connect";
                    lboxLog.Items.Add(ex.Message);
                }
            }

        } //connect or disconnect
        #region 비동기식 서버
        private void startclientTrades() // client
        {
            string test = "test";
            _psocketClient.Send(test);

            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Add("송신 : " + test);
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

            }));


        }

        private void startserverTrades() // server
        {
            _pSocketServer = new CoFAS_SocketServer(Convert.ToInt32(cnport), 100);
            _pSocketServer.evtClentConnect = new CoFAS_SocketServer.delClientConnect(clientconnect);
            _pSocketServer.evtReceiveRequest = new CoFAS_SocketServer.delReceiveRequest(receiverequest);
            _pSocketServer.evtClientDisconnect = new CoFAS_SocketServer.delClientDisconnect(clientdisconnect);
            _pSocketServer.Logname = txtName.Text;
            _pSocketServer._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);


            _pSocketServer.Start();

            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Add("Socket 서버 작동중...");
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                timer1.Start();
            }));
        }


        private void startrestTrades() // rest
        {
            newser.Port = "80";//txtIP.Text.Split(' ')[2];

            newrest.Reststart(newser);
            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Add("Rest 서버 작동중...");
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
            }));
        }



        private void clientdisconnect(Socket soc)
        {

            this.Invoke(new Action(delegate ()
            {

                this.lboxLog.Items.Add("클라이언트 접속종료 : " + _pSocketServer.m_socWorker.RemoteEndPoint.ToString());
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                _pSocketServer._pCoFAS_Log.WLog("client Disconnected : " + _pSocketServer.m_socWorker.RemoteEndPoint);

                lblUserCount.Text = (Convert.ToInt32(lblUserCount.Text) - 1).ToString();
            }));

        }

        private void receiverequest(Socket soc, byte[] bytData)
        {
            try
            {
                this.Invoke(new Action(delegate ()
                {

                    this.lboxLog.Items.Add(soc.RemoteEndPoint.ToString() + " : " + ByteToString(bytData));
                    lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                }));


            }
            catch (Exception ex)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.lboxLog.Items.Add(ex.Message);
                    lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

                }));
                throw;
            }

        }

        private void clientconnect(Socket soc)
        {
            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Add("클라이언트 접속 : " + _pSocketServer.m_socWorker.RemoteEndPoint.ToString());
                lblUserCount.Text = (Convert.ToInt32(lblUserCount.Text) + 1).ToString();
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
            }));
        }


        private void timer1_Tick_async(object sender, EventArgs e)
        {
            for (int i = 0; i < _pSocketServer.connectedClients.Count; i++)
            {
                try
                {
                    _pSocketServer.connectedClients[i].Send(new byte[] { 0x24, 0x11, 0x02, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x35, 0x25 });
                }
                catch (Exception)
                {
                    this.Invoke(new Action(delegate ()
                    {
                        this.lboxLog.Items.Add("클라이언트 접속종료 : " + _pSocketServer.connectedClients[i].RemoteEndPoint.ToString());
                        lblUserCount.Text = (Convert.ToInt32(lblUserCount.Text) - 1).ToString();
                        lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                    }));
                    _pSocketServer.connectedClients[i].Close();
                    _pSocketServer.connectedClients.RemoveAt(i);
                }
            }
        }

        private void timer1_Tick_sync(object sender, EventArgs e)
        {
            _syncser.timerremove();
        }
        #endregion

        private string ByteToString(byte[] strByte) { string str = Encoding.Default.GetString(strByte); return str; }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (_psocketClient != null)
            {
                Thread threadReceiveData = null;
                threadReceiveData = new Thread(new ThreadStart(startclientTrades));

                threadReceiveData.IsBackground = true;
                threadReceiveData.Start();
            }
            else
            {
                synccli.SendData("test");
            }

        }


        private void ceLog_CheckedChanged(object sender, EventArgs e)
        {

            switch (tableLayoutPanel1.BackColor.Name)
            {
                case "LightSkyBlue": //server
                    if (ser.Sync == "async" && _pSocketServer != null)
                    {
                        //비동기식 서버
                        _pSocketServer._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
                    }
                    else// 동기식 서버
                    {
                        if (_syncser != null)
                        {
                            _syncser._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
                        }
                    }

                    break;
                case "Gold": //client
                    if (ser.Sync == "async" && _psocketClient != null) // 비동기식 client
                    {
                        _psocketClient._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
                    }
                    else// 동기식 client
                    {
                        if (synccli != null)
                        {
                            synccli._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
                        }
                    }

                    break;
                case "LightGreen": //rest
                    break;
            }

        }

        private ServerEntity ReadXML(string sername)
        {


            try
            {

                string filePath = Application.StartupPath + @"\server\" + sername + ".xml";
                XmlDocument xmldoc = new XmlDocument();

                ServerEntity ser = new ServerEntity();
                xmldoc.Load(filePath);
                XmlElement root = xmldoc.DocumentElement;
                // 노드 요소들
                XmlNodeList nodes = root.ChildNodes;

                // 노드 요소의 값을 읽어 옵니다.
                foreach (XmlNode node in nodes)
                {
                    switch (node.Name)
                    {
                        case "name":
                            ser.Name = node.InnerText;
                            break;
                        case "server":
                            ser.Server = node.InnerText;
                            break;
                        case "ip":
                            ser.Ip = node.InnerText;
                            break;
                        case "port":
                            ser.Port = node.InnerText;
                            break;
                        case "sql":
                            ser.Sql = node.InnerText;
                            break;
                        case "sync":
                            ser.Sync = node.InnerText;
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return ser;
        }

        private void btndel_Click(object sender, EventArgs e) // 서버 삭제
        {
            if ( MessageBox.Show("서버를 삭제하시겠습니까?", "경 고", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //서버 삭제
                string filePath = Application.StartupPath + @"\server\" + ser.Name + ".xml";
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    MessageBox.Show("삭제가 완료되었습니다.");

                }
                MessageBox.Show("이미 제거되었습니다.");
                this.Visible = false;
            }
        }
    }
}

