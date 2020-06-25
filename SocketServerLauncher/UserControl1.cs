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
using Grapevine.Interfaces.Server;
using System.Net.Http;
using Newtonsoft.Json;

namespace SocketServerLauncher
{

    public partial class UserControl1 : UserControl
    {
        #region ○ API관련
        public void APISetting()
        {
            try
            {
                client = new System.Net.Http.HttpClient
                {
                    //BaseAddress = new Uri("http://m.coever.co.kr:18080")
                    //BaseAddress = new Uri("http://192.168.0.221:8080")
                     BaseAddress = new Uri("http://192.168.0.133:48080")

                };
                //_pCoFASLog.WLog(Properties.Settings.Default.FTP_SERVER_PORT.ToString());
                //_pCoFASLog.WLog(Properties.Settings.Default.FTP_SERVER_IP.ToString());

            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region ○ 기본 변수
        private ServerEntity ser; //xml에서 받아오는 서버 정보
        ResourceManager rm = Resources.ResourceManager;
        
        //API
        HttpClient client;
        
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
        ByteSetting _bytesetting = new ByteSetting(); // 변수 선언
        Formatting format = new Formatting();
        #endregion
        #region 테스트
        //[RestResource]
        public class TestResource
        {
            //[RestRoute]
            public IHttpContext HelloWorld(IHttpContext context)
            {
                context.Response.SendResponse("Hello, world.");
                return context;
            }
        }
        #endregion
        #region ○ 타이머 선언
        delegate void TimerEventFiredDelegate();
        System.Threading.Timer _tmrATG_PLC0001;
        System.Threading.Timer _tmr;
        #endregion
        #region ○ 오토젠 관련 변수/함수


        #endregion
        #region ○ 기본 실행
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
            //lboxLog.Items.Insert(0, ser.Resource_code);
        }
        private void UserControl1_Load(object sender, EventArgs e)
        {
            txtName.Text = ser.Name;
        }
        #endregion
        #region ○ 리소스 타입 설정
        private void reSourceType_Setting(string RsType, bool _Bool)
        {
            #region 변수설정

            cnip = txtIP.Text.Split(' ')[0];
            cnport = txtIP.Text.Split(' ')[2];

            #endregion
            //lboxLog.Items.Insert(0, RsType.ToString());
            #region ○ 'connect'버튼 클릭
            if (_Bool)
            {
                switch (ser.Resource_code)
                {
                    case "ATG_RS0001":
                        break;
                    case "ATG_D3004":
                        //클라이언트 세팅
                        _psocketClient = new CoFAS_SocketClient(cnip, Convert.ToInt32(cnport));
                        _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);//저장하는부분 만들어야함
                        //타이머 세팅
                        _tmrATG_PLC0001 = new System.Threading.Timer(new TimerCallback(d3004CallBack), null, 2000, Timeout.Infinite);
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);
                        lboxLog.Items.Insert(0, "클라이언트 시작");
                        break;
                    case "ATG_RS0003"://PLC4번

                        //클라이언트 세팅
                        _psocketClient = new CoFAS_SocketClient(cnip, Convert.ToInt32(cnport));
                        _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);//저장하는부분 만들어야함

                        //타이머 세팅
                        _tmrATG_PLC0001 = new System.Threading.Timer(new TimerCallback(_tmrATG_PLC0001CallBack), null, 2000, Timeout.Infinite);
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);

                        lboxLog.Items.Insert(0, "클라이언트 시작");

                        break;
                    case "ATG_D3000":
                        //클라이언트 세팅
                        _psocketClient = new CoFAS_SocketClient(cnip, Convert.ToInt32(cnport));
                        _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);//저장하는부분 만들어야함
                        //타이머 세팅
                        _tmrATG_PLC0001 = new System.Threading.Timer(new TimerCallback(d3000CallBack), null, 2000, Timeout.Infinite);
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);
                        lboxLog.Items.Insert(0, "클라이언트 시작");
                        break;
                    case "TEST1":
                        newser.Port = "8080";
                        break;
                    case "TEST2":
                        newser.Port = "82";
                        break;
                    case "TEST3":
                        _psocketClient = new CoFAS_SocketClient("127.0.0.1", 8080);
                        APISetting();
                        _tmr = new System.Threading.Timer(new TimerCallback(APITestCallBack), null, 2000, Timeout.Infinite);
                        _tmr.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);

                        break;
                    default:
                        break;

                }
            }
            #endregion
            #region 'disconnect'버튼 클릭
            else if (!_Bool)
            {
                switch (ser.Resource_code)
                {
                    case "ATG_RS0001":
                        break;
                    case "ATG_RS0002":
                        break;
                    case "ATG_RS0003"://PLC4번

                        //타이머 세팅
                        _tmrATG_PLC0001.Change(Timeout.Infinite, Timeout.Infinite);

                        lboxLog.Items.Insert(0, "클라이언트 종료");

                        break;
                    case "ATG_D3004"://D3004

                        //타이머 세팅
                        _tmrATG_PLC0001.Change(Timeout.Infinite, Timeout.Infinite);

                        lboxLog.Items.Insert(0, "클라이언트 종료");

                        break;
                    case "ATG_D3000"://D3000

                        //타이머 세팅
                        _tmrATG_PLC0001.Change(Timeout.Infinite, Timeout.Infinite);

                        lboxLog.Items.Insert(0, "클라이언트 종료");

                        break;
                    default:
                        break;

                }

            }
            #endregion

        }

        #endregion
        #region ○ 타이머 실행함수
        private void _tmrATG_PLC0001CallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4Read));
                //Add_ListView("시작", "");
            }
            catch { }

        }
        private void d3004CallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(d3004Read));
                //Add_ListView("시작", "");
            }
            catch { }

        }
        private void d3000CallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(d3000Read));
                //Add_ListView("시작", "");
            }
            catch { }

        }
        private void APITestCallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(APITestRead));
                //Add_ListView("시작", "");
            }
            catch { }

        }

        private void PLC4Read()
        {
            try
            {
                if (!_psocketClient.isConnected)
                {
                    int FailCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        FailCnt++;
                        if (FailCnt > 10)
                        {
                            //lboxLog.Items.Insert(0, "서버 연결 실패");
                            //lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            ConnectionError("서버 연결 실패");
                            return;
                        }
                    }
                }
                _psocketClient.Send(_bytesetting.PLC_RW_PRESS);
            }
            catch (Exception ex)
            {

                ConnectionError(ex.Message);
            }
           
        }

        private void d3004Read()
        {
            try
            {
                if (!_psocketClient.isConnected)
                {
                    int FailCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        FailCnt++;
                        if (FailCnt > 10)
                        {
                            //lboxLog.Items.Insert(0, "서버 연결 실패");
                            //lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            ConnectionError("서버 연결 실패");
                            return;
                        }
                    }
                }
                _psocketClient.Send(_bytesetting.d3004);
          
            }
            catch (Exception ex)
            {
                ConnectionError(ex.Message);
            }

        }
        private void d3000Read()
        {
            try
            {
                if (!_psocketClient.isConnected)
                {
                    int FailCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        FailCnt++;
                        if (FailCnt > 10)
                        {
                            //lboxLog.Items.Insert(0, "서버 연결 실패");
                            //lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            ConnectionError("서버 연결 실패");
                            return;
                        }
                    }
                }
                _psocketClient.Send(_bytesetting.d3000);

            }
            catch (Exception ex)
            {
                ConnectionError(ex.Message);
            }

        }
        private void APITestRead()
        {
            try
            {
                DataToAPI("1234.56", "CODE");
            }
            catch(Exception ex)
            {

            }
        }
        private async void DataToAPI(string value, string code)
        {
            try
            {
                DateTime date = DateTime.Now;
                var param = new Controllers.SensorInterface();
                var httpContent = new System.Net.Http.StringContent("");
                try
                {
                    bool _isDec = false;
                    bool _isDecimal = false;
                    int _outInt = 0;
                    _isDec = int.TryParse(value, out _outInt);
                    decimal _outDecimal = 0;
                    _isDecimal = decimal.TryParse(value, out _outDecimal);

                    if (_isDec || _isDecimal)
                    {
                        // 다음 수정 대상 코드
                        date = DateTime.Now;
                        //code = sensor[j];
                        param = new Controllers.SensorInterface()
                        {
                            SensorId = code,
                            Category = "",
                            //OccurDate = date,
                            //TRX에서 128길이의 문자열 넘어오는데 빈값이 \0으로 변환되서 넘어옴
                            Value = value
                        };
                        httpContent = new System.Net.Http.StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                        //var aaa = client.PostAsync("/SensorInterfaceDevice", httpContent).Result;
                        try
                        {
                            await client.PostAsync("/SensorInterfaceDevice", httpContent);
                        }
                        catch (Exception ex)
                        {
                        }


                        //_pCoFASLog.WLog("CODE : " + code.ToString() + " / " + "VALUE : " + value.ToString());
                    }
                    else
                    {
                        // Add_ListView("값 오류(_isDec) : "+value + "/" + code + " ERROR!", "");
                        //  _pCoFASLog.WLog(value + "/" + code + " ERROR!");
                    }
                }

                catch (Exception ex)
                {
                    //Add_ListView("Exception! : "+value + "/" + code + " ERROR!", "");
                }


            }
            catch (Exception ex)
            {
                ConnectionError(ex.Message);
            }

        }

        #endregion
        #region ○ 버튼 클릭 이벤트
        #region ○ 설정 버튼 클릭 시
        private void btnSetting_Click(object sender, EventArgs e)
        {
            ServerEntity loadser = ReadXML(txtName.Text);
            AddForm addfrm = new AddForm(loadser);
            addfrm.Show();
        }
        #endregion
        #region ○ test 버튼 클릭 시
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
        #endregion
        #region ○ connect 버튼 클릭 시
        private void simpleButton1_Click(object sender, EventArgs e)//연결버튼을 눌러 연결 시작
        {
            cnip = txtIP.Text.Split(' ')[0];
            cnport = txtIP.Text.Split(' ')[2];
            Thread threadReceiveData = null;
            
            #region ○ 연결중일 경우
            if (simpleButton1.Text == "Disconnect") //연결중일때
            {
                btndel.Enabled = true;
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

                            lboxLog.Items.Insert(0,"Socket 서버 종료");
                            lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            lblUserCount.Text = "0";
                            ////log.wlog("Socket 서버 종료", txtName.Text);

                            break;
                        case "Gold": //client
                            reSourceType_Setting(ser.Resource_code, false);


                            #region 미사용
                            //if (ser.Sync == "sync")
                            //{
                            //    synccli.SendData(new byte[] { 0x24, 0x11, 0x02, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x35, 0x25 });
                            //    synccli.End();

                            //}
                            //else if (_psocketClient.isConnected)
                            //{
                            //    _psocketClient.Close();
                            //}

                            //lboxLog.Items.Insert(0,"클라이언트 종료");
                            //lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            //simpleButton2.Enabled = false;
                            #endregion

                            break;
                        case "LightGreen": //rest
                            newrest.Reststop(newser);
                            lboxLog.Items.Insert(0,"Rest 서버 종료");
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
                    lboxLog.Items.Insert(0,ex.Message);
                    //log.wlog_Exception(sender.ToString(),ex);
                }
            }
            #endregion
            #region ○ 연결일 경우
            else //연결
            {

                btndel.Enabled = false;

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
                                lboxLog.Items.Insert(0,"동기식 서버 Open");
                                lboxLog.Items.Insert(0,"대기중...");
                                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

                                Thread newser = new Thread(new ThreadStart(_syncser.Start));
                                newser.Start();
                                timer1.Start();
                            }

                            break;
                        case "Gold": //client
                            if (ser.Sync == "async")//비동기식 클라이언트
                            {
                                reSourceType_Setting(ser.Resource_code, true);
                                //로그 세팅
                                _psocketClient.Logname = txtName.Text;
                                _psocketClient._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", txtName.Text, 30, ceLog.Checked);
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
                            reSourceType_Setting(ser.Resource_code, true);
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
                    ConnectionError(ex.ToString());
                }
            }
            #endregion

        } //connect or disconnect
        #endregion
        #region ○ 삭제버튼 클릭 시
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
        #endregion
        #endregion

        #region ○ 연결오류 발생 시
        private void ConnectionError(string ErrorTxt)
        {
            Bitmap myImage = (Bitmap)rm.GetObject("YELLOW");
            pbStatus.BackgroundImage = myImage;
            simpleButton1.Text = "Connect";
            lboxLog.Items.Insert(0, ErrorTxt);
        }
        #endregion
        #region 비동기식 서버
        private void startclientTrades() // client
        {
            string test = "test";
            _psocketClient.Send(test);

            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Insert(0,"송신 : " + test);
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
                this.lboxLog.Items.Insert(0,"Socket 서버 작동중...");
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                timer1.Start();
            }));
        }


        private void startrestTrades() // rest
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            //string ClientIP = string.Empty;
            //for (int i = 0; i < host.AddressList.Length; i++)
            //{
            //    if (host.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
            //    {
            //        ClientIP = host.AddressList[i].ToString();
            //    }
            //}

            //newser = new RestServer {  Port = "8800", Host = "*" };
            //newser = new RestServer();
            newrest.Reststart(newser);
            //newser.Host = ClientIP;//"localhost";
            //newser.Port = "8800";
            //    newser.Start();

            //while (newser.IsListening)
            //    {
            //        Thread.Sleep(300);
            //    }

            
            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Insert(0,"Rest 서버 작동중...");
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
            }));
        }



        private void clientdisconnect(Socket soc)
        {

            this.Invoke(new Action(delegate ()
            {

                this.lboxLog.Items.Insert(0,"클라이언트 접속종료 : " + _pSocketServer.m_socWorker.RemoteEndPoint.ToString());
                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                _pSocketServer._pCoFAS_Log.WLog("client Disconnected : " + _pSocketServer.m_socWorker.RemoteEndPoint);

                lblUserCount.Text = (Convert.ToInt32(lblUserCount.Text) - 1).ToString();
            }));

        }

        private void receiverequest(Socket soc, byte[] bytData)
        {
            try
            {
                //this.lboxLog.Items.Insert(0,"리소스 : " +ser.Resource_code);

                if (this.InvokeRequired)

                {
                    #region ○ 받은 데이터 처리
                    #region 기준값 설정 (공통)
                    byte[] byteresult = bytData;  //테스트 bytData  // 실적용 bytData

                    byte[] ByteArray_ReceiveData = bytData;  //코아칩스 수집용 1006

                    string strCheck = ByteArray2HexString(byteresult, "");
                    string strSTR = System.Text.Encoding.Default.GetString(bytData);
                    //Message_Log("문자열원본 : " + strSTR, true, false);
                    //하드코딩 min max 데이터 10건 

                    string min1 = string.Empty;
                    string min2 = string.Empty;
                    string min3 = string.Empty;
                    string min4 = string.Empty;
                    string min5 = string.Empty;
                    string max1 = string.Empty;
                    string max2 = string.Empty;
                    string max3 = string.Empty;
                    string max4 = string.Empty;
                    string max5 = string.Empty;
                    bool minmaxcheck = false;



                    #endregion

                    switch (ser.Resource_code)
                    {
                        #region 오토젠 용접 데이터 X100
                        case "ATG_RS0001":
                            try
                            {
                                //if (this.InvokeRequired)
                                //{
                                //    #region 기본변수
                                //    string val = "10.001";
                                //    string code = "10_10";
                                //    var param = new Controllers.SensorInterface();
                                //    var httpContent = new System.Net.Http.StringContent("");

                                //    bool isError = false;
                                //    #endregion
                                //    if (bytData.Length > 36)
                                //    {


                                //        //string[] strba = strCheck.Split("s",);

                                //        string[] result = strCheck.Split(new string[] { "EF" }, StringSplitOptions.None);

                                //        //_pCoFASLog.WLog(Encoding.Default.GetString(bytData));


                                //        for (int i = 0; i < result.Length; i++)
                                //        {
                                //            if (result[i].Length > 70)
                                //            {
                                //                #region ○ 데이터 가공부

                                //                // result[1] => result[i] 변경 ( 20200311, 17:01 )
                                //                //4 n/t넘버
                                //                string wel_num = result[i].Substring(0, 8);
                                //                string v_num = ConvertHex(wel_num);

                                //                //3 계열
                                //                string wel_seq = result[i].Substring(8, 6);
                                //                string v_seq = ConvertHex(wel_seq);

                                //                //2 에러

                                //                string wel_error = result[i].Substring(14, 4);

                                //                //4 전류

                                //                string wel_power = result[i].Substring(18, 8);
                                //                string v_power = ConvertHex(wel_power);

                                //                //4 시간

                                //                string wel_time = result[i].Substring(26, 8);
                                //                string v_time = ConvertHex(wel_time);
                                //                //Add_ListView(v_time, "");


                                //                //3 통전각
                                //                //string wel_gg = result[1].Substring(34, 6);
                                //                //string v_num = ConvertHex(wel_num);

                                //                //4 공기압
                                //                string wel_air = result[i].Substring(40, 8);
                                //                string v_air = ConvertHex(wel_air).Replace("k", string.Empty);
                                //                //Add_ListView(v_air, "");


                                //                //4 온도

                                //                string wel_temp = result[i].Substring(48, 8);
                                //                string v_temp = ConvertHex(wel_temp).Replace("'c", string.Empty);
                                //                //Add_ListView(v_temp, "");

                                //                //값 표시
                                //                string values = "1. " + v_num + "2. " + v_seq + "3. " + v_power + "4. " + v_time + "5. " + v_air + "6. " + v_temp;
                                //                //Add_ListView(values, "");
                                //                //4 사용안함(20202020)
                                //                string v_num_str = "";
                                //                //if (Convert.ToInt32(v_num) <= 9)
                                //                //{
                                //                //    int num = Convert.ToInt32(v_num) + 1;
                                //                //    v_num_str = "LH_R" + num.ToString();
                                //                //}
                                //                //else if (Convert.ToInt32(v_num) > 9)
                                //                //{
                                //                //    int num = Convert.ToInt32(v_num) - 9;
                                //                //    v_num_str = "RH_R" + num.ToString();
                                //                //}
                                //                v_num_str = "XE01" + v_num.ToString();
                                //                #endregion

                                //                #region ○ 최수영팀장님 용접모니터링용 ( weld_sub, weld_sub2, weld_info2, weld_info, weld_history )
                                //                _pGatheringUcCtlEntity.ROUTING_NAME = v_num_str; // 설비명(RH_R1)
                                //                _pGatheringUcCtlEntity.TEMP_VALUE = v_temp; //결과 값( 형변환 문의)
                                //                _pGatheringUcCtlEntity.POWER_VALUE = v_power; //결과 값( 형변환 문의)
                                //                _pGatheringUcCtlEntity.TIME_VALUE = v_time; //결과 값( 형변환 문의)
                                //                _pGatheringUcCtlEntity.AIR_VALUE = v_air; //결과 값( 형변환 문의)
                                //                _pGatheringUcCtlEntity.WEL_SEQ = v_seq.ToString();

                                //                dtX100 = new GatheringUcCtlBusiness().USP_ucGatheringCtl_I11(_pGatheringUcCtlEntity);
                                //                #endregion

                                //                #region API서버쪽에 json 형태로 데이터 넘기는 부분 (2020-04-08 추가 nts)
                                //                try
                                //                {
                                //                    int parsNum;
                                //                    bool parsBool = int.TryParse(v_num, out parsNum);
                                //                    if (parsBool)
                                //                    {
                                //                        string weld_number = int.Parse(v_num).ToString().PadLeft(2, '0');
                                //                        WeldDataIntoAPIServer(v_power, weld_number + "_10");
                                //                        WeldDataIntoAPIServer(v_air, weld_number + "_20");
                                //                        WeldDataIntoAPIServer(v_time, weld_number + "_30");
                                //                        WeldDataIntoAPIServer(v_temp, weld_number + "_40");
                                //                        WeldDataIntoAPIServer(v_seq, weld_number + "_50");
                                //                    }
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    _pCoFASLog.WLog(ex.ToString());
                                //                }
                                //                #endregion


                                //            }
                                //        }
                                //        if (dtX100.Rows.Count > 0)
                                //        {
                                //            #region ○ 설비 알람상태에 따라 설비 정지신호 보내기
                                //            // Add_ListView(BitConverter.ToString(Encoding.ASCII.GetBytes(dtX100.Rows[0]["WELD_NUM"].ToString())) + "/" + BitConverter.ToString(Encoding.ASCII.GetBytes(dtX100.Rows[0]["WELD_ADR"].ToString())) + "/" + dtX100.Rows[0]["IP"].ToString(), "");
                                //            DataTable dtlist = dtX100.Copy();
                                //            //_pCoFASSocketClinet = new CoFAS_SocketClient(dtlist.Rows[0]["IP"].ToString(), Convert.ToInt32(dtlist.Rows[0]["PORT"].ToString()));
                                //            //_pCoFASSocketClinet.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);
                                //            //_pCoFASLog.WLog("Client 세팅 : "+dtlist.Rows[0]["IP"].ToString() + ":" + dtlist.Rows[0]["PORT"].ToString());
                                //            _pCoFASSocketClinet.strServerIP = dtlist.Rows[0]["IP"].ToString();
                                //            _pCoFASSocketClinet.iPort = Convert.ToInt32(dtlist.Rows[0]["PORT"].ToString());
                                //            for (int rc = 0; rc < dtX100.Rows.Count; rc++)
                                //            {
                                //                try
                                //                {
                                //                    #region ADR로 찾아서 ADR부분 바꾸기
                                //                    switch (Convert.ToInt32(dtlist.Rows[rc]["WELD_ADR"].ToString()))
                                //                    {
                                //                        case 0:
                                //                        case 1:
                                //                        case 2:
                                //                        case 3:
                                //                        case 4:
                                //                        case 5:
                                //                        case 6:
                                //                        case 7:
                                //                        case 8:
                                //                        case 9:
                                //                            X100_WELD_WR[35] = 0x30;
                                //                            X100_WELD_WR[36] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_ADR"].ToString())[0];
                                //                            X100_WELD_WR2[35] = 0x30;
                                //                            X100_WELD_WR2[36] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_ADR"].ToString())[0];
                                //                            break;
                                //                        case 10:
                                //                        case 11:
                                //                        case 12:
                                //                        case 13:
                                //                        case 14:
                                //                        case 15:
                                //                        case 16:
                                //                        case 17:
                                //                        case 18:
                                //                        case 19:
                                //                            X100_WELD_WR[35] = 0x31;
                                //                            X100_WELD_WR[36] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_ADR"].ToString().Substring(1, 1))[0];
                                //                            X100_WELD_WR2[35] = 0x31;
                                //                            X100_WELD_WR2[36] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_ADR"].ToString().Substring(1, 1))[0];

                                //                            break;

                                //                        case 20:
                                //                        case 21:
                                //                        case 22:
                                //                        case 23:
                                //                        case 24:
                                //                        case 25:
                                //                        case 26:
                                //                        case 27:
                                //                        case 28:
                                //                        case 29:
                                //                            X100_WELD_WR[35] = 0x32;
                                //                            X100_WELD_WR[36] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_ADR"].ToString().Substring(1, 1))[0];
                                //                            X100_WELD_WR2[35] = 0x32;
                                //                            X100_WELD_WR2[36] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_ADR"].ToString().Substring(1, 1))[0];
                                //                            break;


                                //                    }
                                //                    #endregion
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    _pCoFASLog.WLog("ARD로 찾아서 ADR부분 바꾸기 : " + ex.ToString());
                                //                }
                                //                try
                                //                {
                                //                    #region WELD_NUM로 찾아서 ID부분 바꾸기
                                //                    switch (Convert.ToInt32(dtlist.Rows[rc]["WELD_NUM"].ToString()))
                                //                    {
                                //                        case 0:
                                //                        case 1:
                                //                        case 2:
                                //                        case 3:
                                //                        case 4:
                                //                        case 5:
                                //                        case 6:
                                //                        case 7:
                                //                        case 8:
                                //                        case 9:
                                //                            X100_WELD_WR2[14] = 0x30;
                                //                            X100_WELD_WR2[15] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_NUM"].ToString())[0];
                                //                            break;

                                //                        case 10:
                                //                        case 11:
                                //                        case 12:
                                //                        case 13:
                                //                        case 14:
                                //                        case 15:
                                //                        case 16:
                                //                        case 17:
                                //                        case 18:
                                //                        case 19:
                                //                            X100_WELD_WR2[14] = 0x31;
                                //                            X100_WELD_WR2[15] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_NUM"].ToString().Substring(1, 1))[0];

                                //                            break;

                                //                        case 20:
                                //                        case 21:
                                //                        case 22:
                                //                        case 23:
                                //                        case 24:
                                //                        case 25:
                                //                        case 26:
                                //                        case 27:
                                //                        case 28:
                                //                        case 29:
                                //                            X100_WELD_WR2[14] = 0x32;
                                //                            X100_WELD_WR2[15] = Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_NUM"].ToString().Substring(1, 1))[0];
                                //                            // Add_ListView("WELD_NUM: " + BitConverter.ToString(Encoding.ASCII.GetBytes(dtlist.Rows[rc]["WELD_NUM"].ToString().Substring(1, 1))), "");

                                //                            break;


                                //                    }
                                //                    #endregion
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    _pCoFASLog.WLog("WELD_NUM로 찾아서 ID부분 바꾸기 : " + ex.ToString());
                                //                }
                                //                try
                                //                {
                                //                    #region 실제 신호 보내는부분
                                //                    if (_pCoFASSocketClinet != null)
                                //                    {
                                //                        if (_pCoFASSocketClinet.isConnected)
                                //                        {
                                //                            _pCoFASSocketClinet.Send(X100_WELD_WR);
                                //                            //_pCoFASLog.WLog("X100_WELD_WR" + BitConverter.ToString(X100_WELD_WR));

                                //                        }
                                //                        else
                                //                        {
                                //                            if (_pCoFASSocketClinet.Open())
                                //                            {
                                //                                _pCoFASSocketClinet.Send(X100_WELD_WR);
                                //                            }
                                //                            //_pCoFASLog.WLog("X100_WELD_WR" + BitConverter.ToString(X100_WELD_WR));

                                //                        }
                                //                        //설비정지 대기중으로 업데이트
                                //                        Thread.Sleep(1000);

                                //                        if (_pCoFASSocketClinet.isConnected)
                                //                        {
                                //                            _pCoFASSocketClinet.Send(X100_WELD_WR2);
                                //                            //_pCoFASLog.WLog("X100_WELD_WR2" + BitConverter.ToString(X100_WELD_WR2));
                                //                            new GatheringUcCtlBusiness().USP_ucGatheringCtl_weld_stop_U10(int.Parse(dtlist.Rows[rc]["WELD_NUM"].ToString()), dtlist.Rows[0]["IP"].ToString());
                                //                            //Add_ListView("전송완료", "");

                                //                        }
                                //                        else
                                //                        {
                                //                            if (_pCoFASSocketClinet.Open())
                                //                            {
                                //                                _pCoFASSocketClinet.Send(X100_WELD_WR2);
                                //                                //_pCoFASLog.WLog("X100_WELD_WR2" + BitConverter.ToString(X100_WELD_WR2));
                                //                                new GatheringUcCtlBusiness().USP_ucGatheringCtl_weld_stop_U10(int.Parse(dtlist.Rows[rc]["WELD_NUM"].ToString()), dtlist.Rows[0]["IP"].ToString());
                                //                                //Add_ListView("전송완료", "");
                                //                            }


                                //                        }
                                //                    }
                                //                    #endregion
                                //                }
                                //                catch (Exception ex)
                                //                {
                                //                    _pCoFASLog.WLog("실제 신호 보내는부분 : " + ex.ToString());
                                //                }


                                //            }
                                //            #endregion
                                //        }

                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                                //_pCoFASLog.WLog(ex.ToString());
                            }
                            finally
                            {

                            }

                            break;
                        #endregion
                        #region 오토젠 용접 데이터 C300
                        case "ATG_RS0002":
                            try
                            {
                                if (this.InvokeRequired)
                                {
                                    #region ○ 기본변수
                                    bool isError = false;
                                    DataSet dtreturn = new DataSet();
                                    #endregion
                                    if (bytData.Length > 36)
                                    {
                                        string[] result = strCheck.Split(new string[] { "EF" }, StringSplitOptions.None);
                                        for (int i = 0; i < result.Length; i++)
                                        {
                                            if (result[i].Length > 70)
                                            {
                                                #region ○ 데이터 가공부

                                                //4 n/t넘버
                                                string wel_num = result[i].Substring(0, 8);
                                                string v_num = ConvertHex(wel_num);

                                                //3 계열
                                                string wel_seq = result[i].Substring(8, 6);
                                                string v_seq = ConvertHex(wel_seq);
                                                //2 에러

                                                string wel_error = result[i].Substring(14, 4);
                                                //4 전류

                                                string wel_power = result[i].Substring(18, 8);
                                                string v_power = ConvertHex(wel_power);
                                                //4 시간

                                                string wel_time = result[i].Substring(26, 8);
                                                string v_time = ConvertHex(wel_time);

                                                //3 통전각
                                                //string wel_gg = result[1].Substring(34, 6);
                                                //string v_num = ConvertHex(wel_num);

                                                //4 공기압
                                                string wel_air = result[i].Substring(40, 8);
                                                string v_air = ConvertHex(wel_air).Replace("k", string.Empty);

                                                //4 온도

                                                string wel_temp = result[i].Substring(48, 8);
                                                string v_temp = ConvertHex(wel_temp).Replace("'c", string.Empty);
                                                string values = "1. " + v_num + "2. " + v_seq + "3. " + v_power + "4. " + v_time + "5. " + v_air + "6. " + v_temp;
                                                // Add_ListView(values, "");

                                                //4 사용안함(20202020)

                                                string v_num_str = "";
                                                //if (Convert.ToInt32(v_num) <= 9)
                                                //{
                                                //    int num = Convert.ToInt32(v_num) + 1;
                                                //    v_num_str = "LH_R" + num.ToString();
                                                //}
                                                //else if (Convert.ToInt32(v_num) > 9)
                                                //{
                                                //    int num = Convert.ToInt32(v_num) - 9;
                                                //    v_num_str = "RH_R" + num.ToString();
                                                //}
                                                v_num_str = "ME01" + (v_num).ToString();
                                                #endregion
                                                #region○ 저장부
                                                #endregion
                                                #region ○ 리스트박스에 띄우기
                                                this.lboxLog.Items.Insert(0,v_num_str + "/" + v_temp + "/" + v_power + "/" + v_time + "/" + v_air + "/" + v_seq);
                                                //this.lboxLog.Items.Insert(0,soc.RemoteEndPoint.ToString() + " : " + ByteToString(bytData));
                                                //lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                            finally
                            {

                            }

                            break;
                        #endregion
                        default:
                            this.lboxLog.Items.Insert(0,soc.RemoteEndPoint.ToString() + " : " + ByteToString(bytData));
                            lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                            break;

                    }
                    #endregion
                }


            }
            catch (Exception ex)
            {
                this.Invoke(new Action(delegate ()
                {
                    this.lboxLog.Items.Insert(0,ex.Message);
                    lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

                }));
                throw;
            }

        }
        private void evtReceiveSend(byte[] byt)
        {
            if(this.InvokeRequired)
            {
                switch(ser.Resource_code)
                {
                    case "ATG_RS0001":
                        break;
                    case "ATG_RS0002":
                        


                        break;

                    case "ATG_RS0003"://PLC4번 실적데이터
                        lboxLog.Items.Insert(0, BitConverter.ToString(byt));
                        break;
                    case "ATG_D3004" ://d3004
                    case "ATG_D3000" ://d3000
                        int[] datatostring = format.Format_PLC(byt[byt.Count() - 2]);
                        string _st = string.Join("", datatostring);
                        lboxLog.Items.Insert(0, _st);
                        

                        _psocketClient._pCoFAS_Log.WLog("receivedata : " + DateTime.Now + " : " + _st);
                        _psocketClient._pCoFAS_Log.WLog("receivedata : " + DateTime.Now + " : " + BitConverter.ToString( byt));

                        break;

                  
                }
            }
        }
        private void clientconnect(Socket soc)
        {
            this.Invoke(new Action(delegate ()
            {
                this.lboxLog.Items.Insert(0,"클라이언트 접속 : " + _pSocketServer.m_socWorker.RemoteEndPoint.ToString());
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
                        this.lboxLog.Items.Insert(0,"클라이언트 접속종료 : " + _pSocketServer.connectedClients[i].RemoteEndPoint.ToString());
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

        #region ○ 로그체크 시
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
        #endregion
        #region ○ XML 파일 읽기(ReadXML)
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
                        case "resource_code":
                            ser.Resource_code = node.InnerText;
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
        #endregion
        #region ○ 유용한 함수
        #region ○ Byte[] -> String
        private string ByteToString(byte[] strByte) { string str = Encoding.Default.GetString(strByte); return str; }
        #endregion
        #region ○ Byte[] -> Hex
        private static string ByteArray2HexString(Byte[] bytePacket, string strDelimiter)
        {
            string sReturn = "";
            try
            {
                int nCount = bytePacket.Length;

                for (int i = 0; i < nCount; i++)
                {
                    if (i == 0)
                        sReturn += String.Format("{0:X2}", bytePacket[i]);
                    else
                        sReturn += String.Format("{0}{1:X2}", strDelimiter, bytePacket[i]);
                }
            }
            catch
            {
                sReturn = "";
            }
            return sReturn;
        }
        #endregion
        #region ○오토젠 용접1용 ConvertHex
        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }
        #endregion

        #endregion
    }
}

