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
using static SocketServerLauncher.ucGatheringControl;
using System.Resources;
using SocketServerLauncher.Properties;
using static SocketServerLauncher.CoFAS_SocketServer;
using Grapevine.Server;
using System.Xml;
using System.IO;
using Grapevine.Interfaces.Server;
using System.Net.Http;
using Newtonsoft.Json;
using SocketServerLauncher.Provider;
using SocketServerLauncher.api;
using System.IO.BACnet;

namespace SocketServerLauncher
{
    public partial class ucGatheringControl : UserControl
    {
        #region ○ API관련
        /// <summary>
        /// reSourceType_Setting() 안에서 호출하여 API서버를 이용할 수 있도록 함
        /// </summary>
        public void APISetting()
        {
            try
            {
                if (ser.Api_ip != "" && ser.Api_port != "")
                {
                    client = new System.Net.Http.HttpClient
                    {
                        //BaseAddress = new Uri("http://m.coever.co.kr:18080")
                        //BaseAddress = new Uri("http://192.168.0.221:8080")
                        BaseAddress = new Uri(ser.Api_ip + ":" + ser.Api_port)

                    };
                    //_pCoFASLog.WLog(Properties.Settings.Default.FTP_SERVER_PORT.ToString());
                    //_pCoFASLog.WLog(Properties.Settings.Default.FTP_SERVER_IP.ToString());
                }
                else
                {
                    lboxLog.Items.Insert(0, "API세팅에 실패하였습니다. API를 세팅한 후 재시작해주세요.");
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
        public event OnCloseEventHandler OnClose;
        public delegate void OnCloseEventHandler(object sender, EventArgs e);
        

        #region ○ 기본 변수
        private ServerEntity ser; //xml에서 받아오는 서버 정보
        private ProviderEntity _pProviderEntity;
        ResourceManager rm = Resources.ResourceManager;
        
        //API
        HttpClient client;
        public string _ApiIp ;
        public string _Apiport;
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
        DateTime ToDay = DateTime.Now;

        delegate void TimerEventFiredDelegate();

        //엔티티, 조회데이터테이블, 데이터셋
        private DataSet _dsPress = null; // 오토젠 프레스 신규설치 가열로 기준정보 가져오는 데이터셋 (비트만)
        private DataSet _dsPress2 = null; // 오토젠 프레스 신규설치 가열로 기준정보 가져오는 데이터셋 (값만)

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
        System.Threading.Timer _tmrATG_PLC0001;
        System.Threading.Timer _tmr;

        public RestServer Newser
        {
            get
            {
                return newser;
            }

            set
            {
                newser = value;
            }
        }
        #endregion

        #region ○ 오토젠 관련 ○
        #region ○ 냉각수 변수
        static BacnetClient bacnet_client;
        // All the present Bacnet Device List
        static List<BacNode> DevicesList = new List<BacNode>();
        BacnetValue Value;
        bool ret;

        int[] device_type = { 1, 1, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };//, 4,  4,4,4,4,5, 5,5,5,5,2, 2,5,2,2,5, 2,5,2,2,5 ,2 };
        uint[] device_adrs = { 0, 1, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16, 17, 18, 19, 108, 109 };//,20 ,21,22,23,25,20 ,21,22,23,25,1 ,2,1,3,4,2 ,6,3,8,11,4 ,12 };
        decimal[] get_value = new decimal[41];
        string[] device_name = { "OBJECT_ANALOG_INPUT", "OBJECT_ANALOG_OUTPUT", "OBJECT_ANALOG_VALUE", "OBJECT_BINARY_INPUT", "OBJECT_BINARY_OUTPUT", "OBJECT_BINARY_VALUE" };
        /* OBJECT_ANALOG_INPUT = 0,
         OBJECT_ANALOG_OUTPUT = 1,
         OBJECT_ANALOG_VALUE = 2,
         OBJECT_BINARY_INPUT = 3,
         OBJECT_BINARY_OUTPUT = 4,
         OBJECT_BINARY_VALUE = 5,*/
        #endregion
        #region ○ PLC 변수
        private byte[] PLC1_RW1; // PLC1 16개 값 읽어오기
        private byte[] PLC1_RW2; // PLC1 7개 값 읽어오기
        private byte[] byt_Press2Line;
        private byte[] byt_Press2Line2;
        //PLC1 총 23개
        private byte[] PLC2_RW1; // PLC1 16개 값 읽어오기
        private byte[] PLC2_RW2; // PLC1 4개 값 읽어오기
        //PLC2 총 20개
        private byte[] PLC3_RW1; // PLC1 2개 값 읽어오기
        //PLC3 총 2개
        private byte[] PLC_WRITE; // PLC에 쓰기
        private byte[] PLC_RW; // PLC 값 읽기변수

        private byte[] PLC_RW_PRESS; // PLC 값 읽기변수
        private byte[] PLC_RW_LINE; // PLC 값 읽기변수
        private byte[] PLC_RW_LASER; // PLC 값 읽기변수

        private byte[] PLC_RW_X100_WORK_SEQ1;
        private byte[] PLC_RW_X100_WORK_SEQ2;

        private byte[] PLC_RW_C300QTR_WORK_SEQ1;
        private byte[] PLC_RW_C300QTR_WORK_SEQ2;

        private byte[] PLC_RW_C300SUB_WORK_SEQ1;

        private byte[] WR; // 용접모니터링 NG버튼
        private byte[] X100_WELD_WR; // 품질데이터에 따른 정지신호 (01)
        private byte[] C300QTR_WELD_WR; // 품질데이터에 따른 정지신호 (01)
        private byte[] C300SUB_WELD_WR; // 품질데이터에 따른 정지신호 (01)
        private byte[] X100_WELD_WR2; // 품질데이터에 따른 정지신호 (00)
        private byte[] C300QTR_WELD_WR2; // 품질데이터에 따른 정지신호 (00)
        private byte[] C300SUB_WELD_WR2; // 품질데이터에 따른 정지신호 (00)

        //오토젠 인주공장 plc4 차종/생산실적 받아오기위한 BYTE[]
        private byte[] HOTPRESS;
        private byte[] X100DASH;
        private byte[] X100FRT;
        private byte[] Y400QTR;
        private byte[] C200FRT;
        private byte[] GSUV;
        private byte[] LASER1_LH;
        private byte[] LASER1_RH;
        private byte[] LASER2_LH;
        private byte[] LASER2_RH;
        private byte[] LASER3_LH;
        private byte[] LASER3_RH;

        private int PlcType = 1;
        private int PlcTypeHotPress1 = 1;
        private int PlcTypeX100Dash1 = 1;
        private int PlcTypeX100Frt1 = 1;
        private int PlcTypeY400Qtr1 = 1;
        private int PlcTypeC200Frt1 = 1;
        private int PlcTypeGSUV1 = 1;
        private int PlcTypeHotPress10 = 0;
        private int PlcTypeHotPress100 = 0;
        private int PlcTypeX100Dash10 = 0;
        private int PlcTypeX100Frt10 = 0;
        private int PlcTypeY400Qtr10 = 0;
        private int PlcTypeC200Frt10 = 0;
        private int PlcTypeGSUV10 = 0;
        private int PlcTypeLaser1LH1 = 1;
        private int PlcTypeLaser1LH10 = 0;
        private int PlcTypeLaser1RH1 = 1;
        private int PlcTypeLaser1RH10 = 0;
        private int PlcTypeLaser2LH1 = 1;
        private int PlcTypeLaser2LH10 = 0;
        private int PlcTypeLaser2RH1 = 1;
        private int PlcTypeLaser2RH10 = 0;
        private int PlcTypeLaser3LH1 = 1;
        private int PlcTypeLaser3LH10 = 0;
        private int PlcTypeLaser3RH1 = 1;
        private int PlcTypeLaser3RH10 = 0;
        private int PlcTypeMst = 1; //PLC 타입 ( 핫프레스, x100 . . .)
                                    //번지수 설정한 변수(십의자리, 일의자리)

        DataTable dtX100 = new DataTable();
        DataTable dtC300QTR = new DataTable();
        DataTable dtC300SUB = new DataTable();
        // 정지할 설비의 IP, PORT정보
        int PLC4Cnt = 12;
        // 읽어올 PLC 번지(1000~9000번 총 12개, LASER는 LH RH가 있음)
        int WeldCnt = 8;
        //용접데이터 읽어올 갯수(x001~x008 까지 총 8개를 읽어야함)
        string PLCAddress = ""; // PLC Send한 번지 번호 저장변수
        bool PLCSaveYN = true; // plc 데이터 저장이 완료되었는지 확인

        //오토젠 프레스 신규설치 가열로 관련 변수
        string _strADR = "";
        int _intSEQ = 0;
        #endregion
        #region ○ 타이머 설정 변수
        System.Threading.Timer _tmrWork;

        System.Threading.Timer _tmrAlarm;  //Q-light 경광등 알람 

        System.Threading.Timer _tmrAlarmHighWorld;  // 경광등 및 PLC 제어

        System.Threading.Timer _tmrStart;  //Clinet 설정 

        System.Threading.Timer _tmrPLC1; // PLC1데이터 받아오기
        System.Threading.Timer _tmrPLC2; // PLC2데이터 받아오기
        System.Threading.Timer _tmrPLC3; // PLC3데이터 받아오기
        System.Threading.Timer _tmrPLC4; // PLC4 PRESS공정 데이터 받아오기
        System.Threading.Timer _tmrPLCLaser; // PLC4 LASER공정 데이터 받아오기
        System.Threading.Timer _tmrPLCLine; // PLC4 각 Line공정 데이터 받아오기
        System.Threading.Timer _tmrCoolant; // 냉각수 데이터 받아오기
        System.Threading.Timer _tmrPLCWeldSEQ1; // 용접타점X100 1 데이터 받아오기
        System.Threading.Timer _tmrPLCWeldSEQ2; // 용접타점X100 2 데이터 받아오기
        System.Threading.Timer _tmrPLCWeldSEQ3; // 용접타점 C300 QTR 데이터 받아오기
        System.Threading.Timer _tmrPLCWeldSEQ4; // 용접타점 C300 SUB 데이터 받아오기
        System.Threading.Timer _tmrTEST; //테스트용 타이머
        System.Threading.Timer _tmrTEST2; //테스트용 타이머
        System.Threading.Timer _tmrTipClear; //오토젠 하루 지날때마다 팁 횟수 초기화  타이머
        System.Threading.Timer _tmrPress2; // 오토젠 프레스 신규설치 가열로 정보
        System.Threading.Timer _tmrPress2_2; // 오토젠 프레스 신규설치 가열로 정보

        #endregion

        #region ○ PLC 개별읽기 명령 - 응답포맷 검증
        private bool PLCReadRequestCheck(byte[] byt, string type)
        {
            try
            {
                int A = Convert.ToInt32(byt[28].ToString());
                int B = Convert.ToInt32(byt[29].ToString());
                int bCnt = A + (B * 16); // 블럭수
                int dataLength = 2;//데이터 개수는 2개로가정

                //오류부분 검출
                if (byt[26].ToString() != "0" || byt[27].ToString() != "0")
                {
                    _psocketClient._pCoFAS_Log.WLog(type + "오류부분 검출 : " + byt[26].ToString() + "/" + byt[27].ToString() + " / " + BitConverter.ToString(byt));
                    return false;
                }
                if (byt.Length != (bCnt * 4) + 30)
                {
                    _psocketClient._pCoFAS_Log.WLog(type + "데이터 길이 검출 : " + byt.Length.ToString() + "/" + (bCnt * dataLength).ToString() + " / " + BitConverter.ToString(byt));
                    return false;
                }
                //_psocketClient._pCoFAS_Log.WLog("데이터 정상 : " + BitConverter.ToString(byt));
                return true;
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog("PLCReadRequestCheck Exception : " + ex.ToString());
                return false;
            }
        }
        #endregion
        #region ○냉각수 데이터 읽기 실행부분
        public void StartActivity()
        {
            bacnet_client = new BacnetClient(new BacnetIpUdpProtocolTransport(0xBAC0, false));
            bacnet_client.Start();    // go
            bacnet_client.OnIam += new BacnetClient.IamHandler(handler_OnIam);
            bacnet_client.RemoteWhoIs("192.168.22.202");
            Thread.Sleep(500); // Wait a fiew time for WhoIs responses (managed in handler_OnIam)
            //ReadWriteExample();
            //CoFAS_DevExpressManager.ShowInformationMessage("StartActivity");

        }
        private void ReadWriteExample()
        {
            try
            {
                if (this.InvokeRequired)
                {


                    // CoFAS_DevExpressManager.ShowInformationMessage("길이 : " + device_type.Length.ToString());
                    for (int i = 0; i < device_type.Length; i++)
                    {
                        switch (device_type[i])
                        {
                            #region ○ 디아비스 주소 및 형 지정해서 값 받아오기 값이나 주소 없을시 0으로 받아옴
                            case 0:
                                ret = ReadScalarValue(1, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_INPUT, device_adrs[i]), BacnetPropertyIds.PROP_PRESENT_VALUE, out Value);
                                if (ret == true)
                                {
                                    get_value[i] = Convert.ToDecimal(Value.Value.ToString());
                                }
                                else
                                    get_value[i] = 0;
                                break;
                            case 1:
                                ret = ReadScalarValue(1, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_OUTPUT, device_adrs[i]), BacnetPropertyIds.PROP_PRESENT_VALUE, out Value);
                                if (ret == true)
                                {
                                    get_value[i] = Convert.ToDecimal(Value.Value.ToString());
                                }
                                else
                                    get_value[i] = 0;
                                break;
                            case 2:
                                ret = ReadScalarValue(1, new BacnetObjectId(BacnetObjectTypes.OBJECT_ANALOG_VALUE, device_adrs[i]), BacnetPropertyIds.PROP_PRESENT_VALUE, out Value);
                                if (ret == true)
                                {
                                    get_value[i] = Convert.ToDecimal(Value.Value.ToString());
                                }
                                else
                                    get_value[i] = 0;
                                break;
                            case 3:
                                ret = ReadScalarValue(1, new BacnetObjectId(BacnetObjectTypes.OBJECT_BINARY_INPUT, device_adrs[i]), BacnetPropertyIds.PROP_PRESENT_VALUE, out Value);
                                if (ret == true)
                                {
                                    get_value[i] = Convert.ToDecimal(Value.Value.ToString());
                                }
                                else
                                    get_value[i] = 0;
                                break;
                            case 4:
                                ret = ReadScalarValue(1, new BacnetObjectId(BacnetObjectTypes.OBJECT_BINARY_OUTPUT, device_adrs[i]), BacnetPropertyIds.PROP_PRESENT_VALUE, out Value);
                                if (ret == true)
                                {
                                    get_value[i] = Convert.ToDecimal(Value.Value.ToString());
                                }
                                else
                                    get_value[i] = 0;
                                break;
                            case 5:
                                ret = ReadScalarValue(1, new BacnetObjectId(BacnetObjectTypes.OBJECT_BINARY_VALUE, 0), BacnetPropertyIds.PROP_PRESENT_VALUE, out Value);
                                if (ret == true)
                                {
                                    get_value[i] = Convert.ToDecimal(Value.Value.ToString());
                                }
                                else
                                    get_value[i] = 0;
                                break;
                                //

                                //
                                //CoFAS_DevExpressManager.ShowInformationMessage(get_value[i].ToString());
                                #endregion
                                /* OBJECT_ANALOG_INPUT = 0,
                                   OBJECT_ANALOG_OUTPUT = 1,
                                   OBJECT_ANALOG_VALUE = 2,
                                   OBJECT_BINARY_INPUT = 3,
                                   OBJECT_BINARY_OUTPUT = 4,
                                   OBJECT_BINARY_VALUE = 5,*/

                        }

                        try
                        {
                            bool isError;
                            _pProviderEntity.Resource_server = "RS30" + (i + 1).ToString().PadLeft(4, '0');//device_name[device_type[i]] + device_adrs[i].ToString();  //Name
                            _pProviderEntity.Value = Convert.ToDecimal(get_value[i].ToString()); //결과 값( 형변환 문의)
                            _pProviderEntity.Attr1 = get_value[i].ToString(); //미사용컬럼
                            _pProviderEntity.Attr2 = "";
                            string code = "RS30" + (i + 1).ToString().PadLeft(4, '0');
                            string value = Math.Round(Convert.ToDecimal(get_value[i].ToString()), 2).ToString();
                            // CoFAS_DevExpressManager.ShowInformationMessage(_pGatheringUcCtlEntity.VALUES.ToString() + " / " + _pGatheringUcCtlEntity.RESOURCE_SERVER.ToString());
                            isError = new DpsProvider().DQ_Data_Save(_pProviderEntity);
                            #region ○ 냉각수데이터 API서버로 넘기기
                            WeldDataIntoAPIServer(value, code);
                            #endregion

                        }
                        catch (Exception ex)
                        {
                            _psocketClient._pCoFAS_Log.WLog(ex.ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {
            }

        }
        static bool ReadScalarValue(int device_id, BacnetObjectId BacnetObjet, BacnetPropertyIds Propriete, out BacnetValue Value)
        {
            BacnetAddress adr;
            IList<BacnetValue> NoScalarValue;
            Value = new BacnetValue(null);
            // Looking for the device
            adr = DeviceAddr((uint)device_id);
            if (adr == null) return false;  // not found
            // Property Read
            if (bacnet_client.ReadPropertyRequest(adr, BacnetObjet, Propriete, out NoScalarValue) == false)
                return false;
            Value = NoScalarValue[0];
            return true;
        }
        static BacnetAddress DeviceAddr(uint device_id)
        {
            BacnetAddress ret;

            lock (DevicesList)
            {
                foreach (BacNode bn in DevicesList)
                {
                    ret = bn.getAdd(device_id);
                    if (ret != null) return ret;
                }
                // not in the list
                return null;
            }
        }
        static void handler_OnIam(BacnetClient sender, BacnetAddress adr, uint device_id, uint max_apdu, BacnetSegmentations segmentation, ushort vendor_id)
        {
            lock (DevicesList)
            {
                // Device already registred ?
                foreach (BacNode bn in DevicesList)
                    if (bn.getAdd(device_id) != null) return;   // Yes

                // Not already in the list
                DevicesList.Add(new BacNode(adr, device_id));   // add it
            }
        }

        #endregion
        #region ○ 오토젠 용접데이터 API서버로 넘기는 부분 ○
        private async void WeldDataIntoAPIServer(string value, string code)
        {
            #region 기본변수
            DateTime date = DateTime.Now;
            var param = new Controllers.SensorInterface();
            var httpContent = new System.Net.Http.StringContent("");
            #endregion
            #region 추가변수

            #endregion

            #region API서버쪽에 json 형태로 데이터 넘기는 부분 (2020-04-08 추가 nts) - 용접 품질 데이터
            //if (!InvokeRequired)
            {
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
                            _psocketClient._pCoFAS_Log.WLog("API서버 오류! : " + ex.ToString());
                        }


                        //_psocketClient._pCoFAS_Log.WLog("CODE : " + code.ToString() + " / " + "VALUE : " + value.ToString());
                    }
                    else
                    {
                        // Add_ListView("값 오류(_isDec) : "+value + "/" + code + " ERROR!", "");
                        //  _psocketClient._pCoFAS_Log.WLog(value + "/" + code + " ERROR!");
                    }
                }

                catch (Exception ex)
                {
                    //Add_ListView("Exception! : "+value + "/" + code + " ERROR!", "");
                    _psocketClient._pCoFAS_Log.WLog(ex.ToString(), "");
                }
            }
            #endregion
        }

        #endregion
        #region ○ 오토젠 PLC 관련 함수 ○
        #region ○ PLC 명령어 바이트 설정(byte_setting())
        private void byte_setting()
        {
            #region ○PLC읽기 ( 소스상에서 변수를 변경해서 날려주면됨 )
            PLC_RW = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x11,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x01,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x31,0x30,0x30,0x30//변수  %RW1000
            };
            #endregion


            #region HOT PRESS - 0x01
            HOTPRESS = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x01, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x31,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x31,0x32,0x30,0x30
            };
            #endregion 
            #region X100 DASH - 0x02
            X100DASH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x02, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x32,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x32,0x30,0x31,0x30
            };
            #endregion
            #region X100 FRT FLR - 0x03
            X100FRT = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x03, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x33,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x33,0x30,0x31,0x30
            };
            #endregion
            #region Y400 QTR - 0x04
            Y400QTR = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x04, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x34,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x34,0x30,0x31,0x30
            };
            #endregion
            #region C200 FRT FLR - 0x05
            C200FRT = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x05, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x35,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x35,0x30,0x31,0x30
            };
            #endregion
            #region GSUV - 0x06
            GSUV = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x06, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x36,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x36,0x30,0x31,0x30
            };
            #endregion
            #region LASER#1 LH - 0x07
            LASER1_LH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x07, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x37,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x37,0x31,0x30,0x30
            };
            #endregion
            #region LASER#1 RH - 0x08
            LASER1_RH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x08, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x37,0x31,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x37,0x32,0x30,0x30
            };
            #endregion
            #region LASER#2 LH - 0x09
            LASER2_LH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x09, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x38,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x38,0x31,0x30,0x30
            };
            #endregion
            #region LASER#2 RH - 0x0A
            LASER2_RH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x0A, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x38,0x31,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x38,0x32,0x30,0x30
            };
            #endregion
            #region LASER#3 LH - 0x0B
            LASER3_LH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x0B, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x39,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x39,0x31,0x30,0x30
            };
            #endregion
            #region LASER#3 RH - 0x0C
            LASER3_RH = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x0C, 0x00, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x39,0x31,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52,0x57,0x39,0x32,0x30,0x30
            };
            #endregion

            #region ○PRESS읽기
            PLC_RW_PRESS = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x0A, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x02,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x31,0x30,0x30,0x31//수량읽기(기본 1001)
                ,0x07,0x00
                ,0x25,0x52,0x57,0x31,0x32,0x30,0x30//차종읽기(1200)
            };
            #endregion
            #region ○LINE읽기
            PLC_RW_LINE = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x0B, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x62,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x0A,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW

                0x25,0x52, 0x57,0x32,0x30,0x30,0x31//수량읽기(기본 2001)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x33,0x30,0x30,0x31//수량읽기(기본 3001)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x34,0x30,0x30,0x31//수량읽기(기본 4001)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x35,0x30,0x30,0x31//수량읽기(기본 5001)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x36,0x30,0x30,0x31//수량읽기(기본 6001)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x32,0x30,0x31,0x30//차종읽기(2010) 65

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x33,0x30,0x31,0x30//차종읽기(2010)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x34,0x30,0x31,0x30//차종읽기(2010)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x35,0x30,0x31,0x30//차종읽기(2010)

                ,0x07,0x00//변수 길이 2byte 30개
                ,0x25,0x52, 0x57,0x36,0x30,0x31,0x30//차종읽기(2010)
            };
            #endregion
            #region ○LASER읽기
            PLC_RW_LASER = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x0C, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x74,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x0C,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x25,0x52, 0x57,0x37,0x30,0x30,0x31//1LH수량읽기(기본 7001)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x37,0x31,0x30,0x31//1RH수량읽기(기본 7101)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x38,0x30,0x30,0x31//2LH수량읽기(기본 8001)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x38,0x31,0x30,0x31//2RH수량읽기(기본 8101)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x39,0x30,0x30,0x31//3LH수량읽기(기본 9001)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x39,0x31,0x30,0x31//3RH수량읽기(기본 9101)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x37,0x31,0x30,0x30//차종읽기(7100)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x37,0x32,0x30,0x30//차종읽기(7200)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x38,0x31,0x30,0x30//차종읽기(8100)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x38,0x32,0x30,0x30//차종읽기(8200)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x39,0x31,0x30,0x30//차종읽기(9100)
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x39,0x32,0x30,0x30//차종읽기(9200)
            };
            #endregion
            #region ○X100타점 수집
            PLC_RW_X100_WORK_SEQ1 = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x01, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x98,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x10,0x00, // 블록수(16개) 
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x07,0x00,//변수 길이 2byte 30개 28,29bit
                0x25,0x52, 0x57,0x31,0x30,0x30,0x30
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x32
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x33
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x34
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x35
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x36
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x37
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x38
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x39
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x30
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x31
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x32
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x33
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x34
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x35

            };
            #endregion
            #region ○X100타점 수집2
            PLC_RW_X100_WORK_SEQ2 = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x02, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x47,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x07,0x00, // 블록수(16개) 2byte
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x07,0x00,//변수 길이 2byte 30개
                0x25,0x52, 0x57,0x31,0x30,0x31,0x36
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x37
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x38
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x39
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x32,0x30
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x32,0x31
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x32,0x32
            };
            #endregion
            #region ○C300 QTR 타점 수집 1
            PLC_RW_C300QTR_WORK_SEQ1 = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x01, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x59,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x09,0x00, // 블록수(16개) 
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x07,0x00,//변수 길이 2byte 30개 28,29bit
                0x25,0x52, 0x57,0x31,0x30,0x30,0x30
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x32
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x33
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x34
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x35
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x36
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x37
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x38

            };

            #endregion
            #region ○C300 QTR 타점 수집 2
            PLC_RW_C300QTR_WORK_SEQ2 = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x02, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x59,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x09,0x00, // 블록수(16개) 
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x07,0x00,//변수 길이 2byte 30개 28,29bit
                0x25,0x52, 0x57,0x31,0x30,0x31,0x30
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x31
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x32
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x33
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x34
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x35
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x36
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x37
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x31,0x38

            };

            #endregion
            #region ○C300 SUB 타점 수집
            PLC_RW_C300SUB_WORK_SEQ1 = new byte[] {
               0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x03, // Invoke id(header) 16 ( 14, 15번 bit ) // 별칭 지어주는거임. 
                0x2C,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x04,0x00, // 블록수(16개) 
                //0x25,//0x44,0x42,0x30,//직접 변수 3byte(ex - %RW) = %RW
                0x07,0x00,//변수 길이 2byte 30개 28,29bit
                0x25,0x52, 0x57,0x31,0x30,0x30,0x30
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x31
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x32
                ,0x07,0x00
                ,0x25,0x52, 0x57,0x31,0x30,0x30,0x33

            };

            #endregion
            #region ○ 오토젠 X100 품질데이터에 따른 설비제어 - 정지신호(01) 
            X100_WELD_WR = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33,
                0xFF, 0x00,//invoke
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x01, 0x00,
                    0x01, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
            #region ○ 오토젠 C300QTR 품질데이터에 따른 설비제어 - 정지신호(01) 
            C300QTR_WELD_WR = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0xFF, 0x01,
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x02, 0x00,
                    0x01, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
            #region ○ 오토젠 C300SUB 품질데이터에 따른 설비제어 - 정지신호(01) 
            C300QTR_WELD_WR = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0xFF, 0x02,
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x01, 0x00,
                    0x01, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
            #region ○ 오토젠 X100 품질데이터에 따른 설비제어 - 정지신호(00) 
            X100_WELD_WR2 = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0xFF, 0x03,
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x02, 0x00,
                    0x00, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
            #region ○ 오토젠 C300QTR 품질데이터에 따른 설비제어 - 정지신호(00) 
            C300QTR_WELD_WR2 = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0xFF, 0x04,
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x01, 0x00,
                    0x00, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
            #region ○ 오토젠 C300SUB 품질데이터에 따른 설비제어 - 정지신호(00) 
            C300QTR_WELD_WR2 = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0xFF, 0x05,
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x01, 0x00,
                    0x00, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
            #region ○ 오토젠 프레스 신규설치 가열로 정보 읽어오기 비트값만
            byt_Press2Line = new byte[] {
                0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x99, 0x99, // Invoke id(header) 16
                0x11,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x01,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                0x25,0x44, 0x57,0x31,0x30,0x30,0x30//변수  %DW1000
            };
            #endregion
            #region ○ 오토젠 프레스 신규설치 가열로 정보 읽어오기 밸류값만
            byt_Press2Line2 = new byte[] {
                0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0xA0,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x99, 0x99, // Invoke id(header) 16
                0x11,0x00,//length 2byte
                0x00,//예약영역 1byte // Module Position, Bit0~3 : Ethernet I/F 모듈의 슬롯번호
                                      //                  Bit 4~7 : Ethernet I/F 모듈의 베이스 번호
                0x00,//BCC 1byte
                //헤더 끝

                0x54,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x01,0x00, // 블록수(16개) 2byte
                0x07,0x00,//변수 길이 2byte 30개
                0x25,0x44, 0x57,0x31,0x30,0x30,0x30//변수  %DW1000
            };
            #endregion

            #region ○ 오토젠 용접모니터링에서 설비제어(NG버튼)
            WR = new byte[] {
                    0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00,
                    0x15,
                    0x00, 0x00, 0x00,
                    0x58, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x07, 0x00, 0x25, 0x52, 0x57,
                    0x32, 0x30, 0x30, 0x30,
                    0x01, 0x00,
                    0x01, 0x00 };  // 실제 값 영역 (4) 좌우 바뀜.  12 34 -> 34 12


            //4C 53 49 53 2D 58 47 54 00 00 00 00 00 33 00 00 
            //    15   // 데이터 길이 숫자 (21개 -> Hex)
            //    00 00 00  // 헤더 끝..
            //    58 00 02 00 00 00 01 00 07 00 25 52 57 
            //    32 30 30 30
            //    01 00 
            //    FF FF  // 41
            #endregion
        }
        #endregion
        #region ○ PLC에 쓰기

        private void byte_write(byte[] dataAdr)
        {
            PLC_WRITE = new byte[] {
                0x4C, 0x53, 0x49, 0x53, 0x2D, 0x58, 0x47, 0x54,//Company ID 8byte
                0x00, 0x00,//Reserved(0x0000 : 예약영역) 2byte
                0x00, 0x00,//PLC Info (클라이언트 -> 서버 = 0x00) 2byte
                0x00,//CPU Info 1byte
                0x33,//Source of frame ( 클라이언트 -> 서버 : 0x33, 서버 -> 클라이언트 : 0x11)
                0x00, 0x00, // header 16
                0x1A,0x00,//length 2byte
                0x00,//예약영역 1byte
                0x00,//BCC 1byte

                0x58,0x00, // 명령어(Read Request) 2byte
                0x02,0x00, // 데이터타입 2btte
                0x00,0x00, // 예약영역 2byte
                0x01,0x00, // 블록수(16개) 2byte
                0x08,0x00,//변수 길이 2byte
                0x25,0x52,0x57,//직접 변수 3byte(ex - %RW) = %RW
                0x52,0x32,0x30,dataAdr[0],dataAdr[1]//변수  R20ㅇㅇ
            };

        }
        #endregion
        #region  ○ plc 데이터 보내기
        private void Plc4TotalSendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_LASER);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_LASER);
                //}
                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("Plc4Laser 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(LASER1_LH);
                Thread.Sleep(100);
                _psocketClient.Send(HOTPRESS);
                Thread.Sleep(100);
                _psocketClient.Send(X100DASH);
                Thread.Sleep(100);
                _psocketClient.Send(X100FRT);
                Thread.Sleep(100);
                _psocketClient.Send(Y400QTR);
                Thread.Sleep(100);
                _psocketClient.Send(C200FRT);
                Thread.Sleep(100);
                _psocketClient.Send(GSUV);
                Thread.Sleep(100);
                _psocketClient.Send(LASER1_RH);
                Thread.Sleep(100);
                _psocketClient.Send(LASER2_LH);
                Thread.Sleep(100);
                _psocketClient.Send(LASER2_RH);
                Thread.Sleep(100);
                _psocketClient.Send(LASER3_LH);
                Thread.Sleep(100);
                _psocketClient.Send(LASER3_RH);
            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }

        private void Plc4SendData()
        {
            try
            {
                //Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_PRESS);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_PRESS);
                //}
                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("Plc4 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_PRESS);
                //Thread.Sleep(100);

                //Add_ListView(PLC_RW_PRESS[15].ToString() + " : SEND OK", "");
            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4LineSendData()
        {
            try
            {
                //Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_LINE);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_LINE);
                //}
                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("Plc4Line 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_LINE);
                //Thread.Sleep(100);


            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4LaserSendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_LASER);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_LASER);
                //}
                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("Plc4Laser 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_LASER);

            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4X100_1SendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_X100_WORK_SEQ1);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_X100_WORK_SEQ1);
                //}

                int failCnt = 0;
                if (!_psocketClient.isConnected)
                {
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("PlcX100_1 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_X100_WORK_SEQ1);
                //_psocketClient._pCoFAS_Log.WLog(BitConverter.ToString(PLC_RW_X100_WORK_SEQ1));
            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4X100_2SendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_X100_WORK_SEQ2);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_X100_WORK_SEQ2);
                //}

                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("PlcX100_2 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_X100_WORK_SEQ2);
                //Thread.Sleep(100);


            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4C300QTR_1SendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_C300QTR_WORK_SEQ1);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_C300QTR_WORK_SEQ1);
                //}
                int failCnt = 0;
                if (!_psocketClient.isConnected)
                {

                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("PlcC300QTR_2 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_C300QTR_WORK_SEQ1);
            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4C300QTR_2SendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_C300QTR_WORK_SEQ2);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_C300QTR_WORK_SEQ2);
                //}
                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("PlcC300QTR_2 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }

                _psocketClient.Send(PLC_RW_C300QTR_WORK_SEQ2);
                //Thread.Sleep(100);


            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }
        private void Plc4C300SUB_1SendData()
        {
            try
            {
                // Add_ListView(PLC_RW_LINE[15].ToString(), "");
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC_RW_C300SUB_WORK_SEQ1);
                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC_RW_C300SUB_WORK_SEQ1);
                //}
                if (!_psocketClient.isConnected)
                {

                    int failCnt = 0;
                    while (!_psocketClient.Open())
                    {
                        if (failCnt > 9)
                        {
                            _psocketClient._pCoFAS_Log.WLog("PlcC300SUB 연결 총" + failCnt.ToString() + "회 실패");
                            return;
                        }
                        failCnt++;
                    }
                }
                _psocketClient.Send(PLC_RW_C300SUB_WORK_SEQ1);
                //Thread.Sleep(100);


            }
            catch (Exception ex)
            {
            }
            //Add_ListView("SEND OK", "");
        }

        #endregion
        #endregion
        #region ○ 타이머설정 그룹 ○
        #region ○ 냉각수 타이머 설정
        private void CoolantCallBack(object obj)
        {
            try
            {
                ReadWriteExample();
                //BeginInvoke(new TimerEventFiredDelegate(ReadWriteExample));
                //CoFAS_DevExpressManager.ShowInformationMessage("coolantcallback ");
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
        #region ○ PLC4 타이머 설정
        private void PLC4CallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4Read));
                //Add_ListView("시작", "");

            }
            catch { }
        }
        private void PLC4LineCallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4LineRead));
                //Add_ListView("시작", "");

            }
            catch { }
        }

        private void PLC4LaserCallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4LaserRead));
                //Add_ListView("시작", "");

            }
            catch { }
        }
        private void PLC4X100CallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4X100Read));
                //Add_ListView("시작", "");

            }
            catch { }
        }
        private void PLC4X100CallBack2(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4X100Read2));
                //Add_ListView("시작", "");

            }
            catch { }
        }
        private void PLC4C300QTRCallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4C300QTRRead));
                //Add_ListView("시작", "");

            }
            catch { }
        }
        private void TipClearCallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(TipClearTimer));
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog(ex.ToString());
            }
        }
        private void Press2CallBack(object obj) // 오토젠 프레스 신규설치 가열로 정보
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(Press2Timer));
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog(ex.ToString());
            }
        }
        private void Press2CallBack2(object obj) // 오토젠 프레스 신규설치 가열로 정보
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(Press2Timer2));
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog(ex.ToString());
            }
        }

        private void PLC4C300SUBCallBack(object obj)
        {
            try
            {
                BeginInvoke(new TimerEventFiredDelegate(PLC4C300SUBRead));
                //Add_ListView("시작", "");

            }
            catch { }
        }
        #endregion


        #endregion
        #region ○ 타이머 실행함수 그룹 ○
        #region ○ PLC1 타이머 실행함수
        private void PLC1Read()
        {
            try
            {
                //if (_psocketClient.isConnected)
                //{
                //    _psocketClient.Send(PLC1_RW1);
                //     Thread.Sleep(700);
                //    _psocketClient.Send(PLC1_RW2);

                //}
                //else
                //{
                //    _psocketClient.Open();
                //    _psocketClient.Send(PLC1_RW1);
                //    Thread.Sleep(700);
                //    _psocketClient.Send(PLC1_RW2);
                //}
                int wcnt = 0;
                while (!_psocketClient.isConnected)
                {

                    wcnt++;
                    if (!_psocketClient.isConnected)
                    {
                        _psocketClient.Open();
                    }
                    else
                        break;
                    if (wcnt > 100)
                    {
                        _psocketClient._pCoFAS_Log.WLog("총" + wcnt.ToString() + "회 연결 실패");
                        break;
                    }

                }
                if (_psocketClient.isConnected)
                {
                    _psocketClient.Send(PLC1_RW1);
                    Thread.Sleep(700);
                    _psocketClient.Send(PLC1_RW2);
                    //Thread.Sleep(100);
                }

            }
            catch (Exception ex) { }
        }
        #endregion
        #region ○ PLC2 타이머 실행함수
        private void PLC2Read()
        {
            //if (_psocketClient.isConnected)
            //{
            //    _psocketClient.Send(PLC2_RW1);
            //    //Thread.Sleep(1000);
            //    _psocketClient.Send(PLC2_RW2);

            //}
            //else
            //{
            //    _psocketClient.Open();
            //    _psocketClient.Send(PLC2_RW1);
            //    //Thread.Sleep(1000);
            //    _psocketClient.Send(PLC2_RW2);
            //}
            int wcnt = 0;
            while (!_psocketClient.isConnected)
            {

                wcnt++;
                if (!_psocketClient.isConnected)
                {
                    _psocketClient.Open();
                }
                else
                    break;
                if (wcnt > 100)
                {
                    _psocketClient._pCoFAS_Log.WLog("총" + wcnt.ToString() + "회 연결 실패");
                    break;
                }

            }
            if (_psocketClient.isConnected)
            {
                _psocketClient.Send(PLC2_RW1);
                Thread.Sleep(700);
                _psocketClient.Send(PLC2_RW2);
                //Thread.Sleep(100);
            }

        }
        #endregion
        #region ○ PLC3 타이머 실행함수
        private void PLC3Read()
        {
            //Server_Open();
            //if (_psocketClient.isConnected)
            //{
            //    _psocketClient.Send(PLC3_RW1);

            //}
            //else
            //{
            //    _psocketClient.Open();
            //    _psocketClient.Send(PLC3_RW1);
            //}
            int wcnt = 0;
            while (!_psocketClient.isConnected)
            {

                wcnt++;
                if (!_psocketClient.isConnected)
                {
                    _psocketClient.Open();
                }
                else
                    break;
                if (wcnt > 100)
                {
                    _psocketClient._pCoFAS_Log.WLog("총" + wcnt.ToString() + "회 연결 실패");
                    break;
                }

            }
            if (_psocketClient.isConnected)
            {
                _psocketClient.Send(PLC3_RW1);
                //Thread.Sleep(100);
            }

        }
        #endregion
        #region ○ PLC4 타이머 실행함수
        private void PLC4Read()
        {


            //Plc4SendData();
            Plc4TotalSendData();
            #region 미사용
            if (false)
            {
                switch (PLC4Cnt)
                {
                    #region ○ 데이터 조회 & 차종설정
                    #region ○ HOT PRESS
                    case 1:

                        PlcTypeMst = 1;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x31;
                        PLC_RW[34] = Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeHotPress100.ToString())[0]).ToString("X"));
                        PLC_RW[35] = Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeHotPress10.ToString())[0]).ToString("X"));
                        PLC_RW[36] = Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeHotPress1.ToString())[0]).ToString("X"));
                        PlcType = PlcTypeHotPress1 + (PlcTypeHotPress10 * 10) + (PlcTypeHotPress100 + 100);
                        Plc4SendData();

                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x31;
                        PLC_RW[34] = 0x32;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");
                        break;
                    #endregion
                    #region ○ X100 DASH LINE
                    case 2:
                        PlcTypeMst = 2;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x32;
                        PLC_RW[34] = 0X30;
                        PLC_RW[35] = 0X30;//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeX100Dash10.ToString())[0]).ToString("X"));
                        PLC_RW[36] = 0X31; // 우선 1번만 받는다.(차종을 안줌)//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeX100Dash1.ToString())[0]).ToString("X"));
                        PlcType = 1;//PlcTypeX100Dash1 + (PlcTypeX100Dash10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x32;
                        PLC_RW[34] = 0x30;
                        PLC_RW[35] = 0x31;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ X100 FRT FLR LINE
                    case 3:
                        PlcTypeMst = 3;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x33; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = 0X30;//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeX100Frt10.ToString())[0]).ToString("X"));
                        PLC_RW[36] = 0X31; // 우선 1번만 받는다.(차종을 안줌)//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeX100Frt1.ToString())[0]).ToString("X"));
                        PlcType = 1;//PlcTypeX100Dash1 + (PlcTypeX100Dash10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x33;
                        PLC_RW[34] = 0x30;
                        PLC_RW[35] = 0x31;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ Y400 QTR LINE
                    case 4:
                        PlcTypeMst = 4;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x34; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = 0X30;//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeY400Qtr10.ToString())[0]).ToString("X"));
                        PLC_RW[36] = 0X31; // 우선 1번만 받는다.(차종을 안줌)//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeY400Qtr1.ToString())[0]).ToString("X"));
                        PlcType = 1;//PlcTypeX100Dash1 + (PlcTypeX100Dash10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x34;
                        PLC_RW[34] = 0x30;
                        PLC_RW[35] = 0x31;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ C200 FRT FLR LINE
                    case 5:
                        PlcTypeMst = 5;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x35; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = 0X30;//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeC200Frt10.ToString())[0]).ToString("X"));
                        PLC_RW[36] = 0X31; // 우선 1번만 받는다.(차종을 안줌)//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeC200Frt1.ToString())[0]).ToString("X"));
                        PlcType = 1;//PlcTypeX100Dash1 + (PlcTypeX100Dash10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x35;
                        PLC_RW[34] = 0x30;
                        PLC_RW[35] = 0x31;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ GSUV LINE
                    case 6:
                        PlcTypeMst = 6;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x36; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = 0X30;//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeGSUV10.ToString())[0]).ToString("X"));
                        PLC_RW[36] = 0X31; // 우선 1번만 받는다.(차종을 안줌)//Convert.ToByte(Convert.ToInt32(Encoding.ASCII.GetBytes(PlcTypeGSUV1.ToString())[0]).ToString("X"));
                        PlcType = 1;//PlcTypeX100Dash1 + (PlcTypeX100Dash10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x36;
                        PLC_RW[34] = 0x30;
                        PLC_RW[35] = 0x31;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ LASER #1 LH
                    case 7:
                        PlcTypeMst = 7;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x37; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = Encoding.ASCII.GetBytes(PlcTypeLaser1LH10.ToString())[0];
                        PLC_RW[36] = Encoding.ASCII.GetBytes(PlcTypeLaser1LH1.ToString())[0];
                        PlcType = PlcTypeLaser1LH1 + (PlcTypeLaser1LH10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x37;
                        PLC_RW[34] = 0x31;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ LASER #1 RH
                    case 8:
                        PlcTypeMst = 71;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x37; // 라인설정
                        PLC_RW[34] = 0X31; // 라인설정
                        PLC_RW[35] = Encoding.ASCII.GetBytes(PlcTypeLaser1RH10.ToString())[0];
                        PLC_RW[36] = Encoding.ASCII.GetBytes(PlcTypeLaser1RH1.ToString())[0];
                        PlcType = PlcTypeLaser1RH1 + (PlcTypeLaser1RH10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x37;
                        PLC_RW[34] = 0x32;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ LASER #2 LH
                    case 9:
                        PlcTypeMst = 8;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x38; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = Encoding.ASCII.GetBytes(PlcTypeLaser2LH10.ToString())[0];
                        PLC_RW[36] = Encoding.ASCII.GetBytes(PlcTypeLaser2LH1.ToString())[0];
                        PlcType = PlcTypeLaser2LH1 + (PlcTypeLaser2LH10 * 10);
                        // Add_ListView(PLC_RW[33].ToString() + PLC_RW[34].ToString() + PLC_RW[35].ToString() + PLC_RW[36].ToString(), "");

                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x38;
                        PLC_RW[34] = 0x31;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ LASER #2 RH
                    case 10:
                        PlcTypeMst = 81;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x38; // 라인설정
                        PLC_RW[34] = 0X31; // 라인설정
                        PLC_RW[35] = Encoding.ASCII.GetBytes(PlcTypeLaser2RH10.ToString())[0];
                        PLC_RW[36] = Encoding.ASCII.GetBytes(PlcTypeLaser2RH1.ToString())[0];
                        PlcType = PlcTypeLaser2RH1 + (PlcTypeLaser2RH10 * 10);
                        // Add_ListView(PLC_RW[33].ToString() + PLC_RW[34].ToString() + PLC_RW[35].ToString() + PLC_RW[36].ToString(), "");
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x38;
                        PLC_RW[34] = 0x32;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ LASER #3 LH
                    case 11:
                        PlcTypeMst = 9;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x39; // 라인설정
                        PLC_RW[34] = 0X30; // 라인설정
                        PLC_RW[35] = Encoding.ASCII.GetBytes(PlcTypeLaser3LH10.ToString())[0];
                        PLC_RW[36] = Encoding.ASCII.GetBytes(PlcTypeLaser3LH1.ToString())[0];
                        PlcType = PlcTypeLaser3LH1 + (PlcTypeLaser3LH10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x39;
                        PLC_RW[34] = 0x31;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                    #endregion
                    #region ○ LASER #3 RH
                    case 12:
                        PlcTypeMst = 91;
                        PLC_RW[15] = 0x01;//데이터 조회부분 아이디설정
                        PLC_RW[33] = 0x39; // 라인설정
                        PLC_RW[34] = 0X31; // 라인설정
                        PLC_RW[35] = Encoding.ASCII.GetBytes(PlcTypeLaser3RH10.ToString())[0];
                        PLC_RW[36] = Encoding.ASCII.GetBytes(PlcTypeLaser3RH1.ToString())[0];
                        PlcType = PlcTypeLaser3RH1 + (PlcTypeLaser3RH10 * 10);
                        Plc4SendData();
                        //Add_ListView("실적데이터 전송", "");
                        PLC_RW[15] = 0x02;//차종조회 아이디설정
                        PLC_RW[33] = 0x39;
                        PLC_RW[34] = 0x32;
                        PLC_RW[35] = 0x30;
                        PLC_RW[36] = 0x30;
                        Plc4SendData();
                        //Add_ListView("차종데이터 전송", "");

                        break;
                        #endregion

                        #endregion

                }
            }
            #endregion
            //if (PlcTypeMst < 10)
            //    {
            //        PLCAddress = "R" + PlcTypeMst.ToString() + PlcType.ToString().PadLeft(3, '0');
            //    }
            //    else if (PlcTypeMst >= 10)
            //    {
            //        PLCAddress = "R" + PlcTypeMst.ToString() + PlcType.ToString().PadLeft(2, '0');
            //    }
            //}
        }
        #endregion
        #region ○ PLC4 타이머 실행함수
        private void PLC4LaserRead()
        {
            Plc4LaserSendData();
        }
        #endregion
        #region ○ PLC4 LINE 타이머 실행함수
        private void PLC4LineRead()
        {

            //Add_ListView("PLC4Read", "");
            //처음에는 기본값으로 읽어옴
            Plc4LineSendData();
            //리시브쪽에서 차종 바꿔줌

            //if (PlcTypeMst < 10)
            //    {
            //        PLCAddress = "R" + PlcTypeMst.ToString() + PlcType.ToString().PadLeft(3, '0');
            //    }
            //    else if (PlcTypeMst >= 10)
            //    {
            //        PLCAddress = "R" + PlcTypeMst.ToString() + PlcType.ToString().PadLeft(2, '0');
            //    }
            //}
        }
        #endregion
        #region ○ 팁 초기화 타이머 실행함수
        private void TipClearTimer()
        {
            try
            {
                if (ToDay.Day != DateTime.Now.Day)
                {
                    new DpsProvider().USP_ucGatheringCtl_TipClear_U10();
                    ToDay = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog(ex.ToString());
            }

        }
        #endregion
        #region ○ 오토젠 프레스 신규설치 가열로 정보 타이머 실행함수 BIT자리로 알아보는
        private void Press2Timer()
        {
            try
            {
                char[] cArray;
                int invokeID = 0;//번지값 ID로해서 보내야함
                byte[] bytInvokeID = null;
                //Add_ListView(_dsPress.Tables[0].Rows[0]["ADR"].ToString().Substring(1, 4) + "/" + _dsPress.Tables[0].Rows.Count.ToString(), "");
                //번지 등록되어있는대로 for문돌면서 신호 보내기
                for (int i = 0; i < _dsPress.Tables[0].Rows.Count; i++)
                {
                    cArray = _dsPress.Tables[0].Rows[i]["ADR"].ToString().ToCharArray();
                    invokeID = int.Parse(_dsPress.Tables[0].Rows[i]["ADR"].ToString().Substring(1, 4));
                    if (invokeID >= 0)
                    {
                        // invokeID부분에 번지 넣기 (앞뒤 바뀜)
                        bytInvokeID = BitConverter.GetBytes(invokeID);
                        byt_Press2Line[14] = bytInvokeID[0];
                        byt_Press2Line[15] = bytInvokeID[1];

                        // 보내는 변수값에 번지 넣기( D3001 )
                        byt_Press2Line[31] = (byte)cArray[0];
                        byt_Press2Line[33] = (byte)cArray[1];
                        byt_Press2Line[34] = (byte)cArray[2];
                        byt_Press2Line[35] = (byte)cArray[3];
                        byt_Press2Line[36] = (byte)cArray[4];

                        int wcnt = 0;
                        //_psocketClient._pCoFAS_Log.WLog("보내기 작업시작");
                        if (!_psocketClient.isConnected)
                        {

                            int failCnt = 0;
                            while (!_psocketClient.Open())
                            {
                                if (failCnt > 9)
                                {
                                    _psocketClient._pCoFAS_Log.WLog("Press2Timer() 연결 총" + failCnt.ToString() + "회 실패");
                                    return;
                                }
                                failCnt++;
                            }
                        }
                        _psocketClient.Send(byt_Press2Line);
                        Thread.Sleep(100);


                        // _psocketClient._pCoFAS_Log.WLog(wcnt.ToString() + "회 연결 실패");

                        // _psocketClient._pCoFAS_Log.WLog("보내기 작업끝 " + _psocketClient.isConnected.ToString());


                        //if (_psocketClient.isConnected)
                        //{
                        //    _psocketClient.Send(byt_Press2Line);
                        //    //_psocketClient._pCoFAS_Log.WLog("->" +BitConverter.ToString(byt_Press2Line));
                        //}
                        //else
                        //{
                        //    _psocketClient.Open();
                        //    _psocketClient.Send(byt_Press2Line);
                        //    //_psocketClient._pCoFAS_Log.WLog("->" + BitConverter.ToString(byt_Press2Line));
                        //}
                        //Application.DoEvents();
                        //Add_ListView("전송", "");
                    }
                }
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog(ex.ToString());
            }

        }
        #endregion
        #region ○ 오토젠 프레스 신규설치 가열로 정보 타이머 실행함수 밸류로 알아보는
        private void Press2Timer2()
        {
            try
            {
                char[] cArray;
                int invokeID = 0;//번지값 ID로해서 보내야함
                byte[] bytInvokeID = null;
                //번지 등록되어있는대로 for문돌면서 신호 보내기
                for (int i = 0; i < _dsPress2.Tables[0].Rows.Count; i++)
                {
                    cArray = _dsPress2.Tables[0].Rows[i]["ADR"].ToString().ToCharArray();
                    invokeID = int.Parse(_dsPress2.Tables[0].Rows[i]["ADR"].ToString().Substring(1, 4));
                    //Add_ListView(invokeID.ToString()+"/"+_dsPress2.Tables[0].Rows[i]["ADR"].ToString(), "");
                    if (invokeID >= 0)
                    {
                        // invokeID부분에 번지 넣기 (앞뒤 바뀜)
                        bytInvokeID = BitConverter.GetBytes(invokeID);
                        byt_Press2Line2[14] = bytInvokeID[0];
                        byt_Press2Line2[15] = bytInvokeID[1];

                        // 보내는 변수값에 번지 넣기( D3001 )
                        byt_Press2Line2[31] = (byte)cArray[0];
                        byt_Press2Line2[33] = (byte)cArray[1];
                        byt_Press2Line2[34] = (byte)cArray[2];
                        byt_Press2Line2[35] = (byte)cArray[3];
                        byt_Press2Line2[36] = (byte)cArray[4];
                        if (!_psocketClient.isConnected)
                        {

                            int failCnt = 0;
                            while (!_psocketClient.Open())
                            {
                                if (failCnt > 9)
                                {
                                    _psocketClient._pCoFAS_Log.WLog("Press2Timer2() 연결 총" + failCnt.ToString() + "회 실패");
                                    return;
                                }
                                failCnt++;
                            }
                        }
                        _psocketClient.Send(byt_Press2Line2);
                        Thread.Sleep(100);

                        //if (_psocketClient.isConnected)
                        //{
                        //    _psocketClient.Send(byt_Press2Line2);
                        //    _psocketClient._pCoFAS_Log.WLog("->" + BitConverter.ToString(byt_Press2Line2));
                        //}
                        //else
                        //{
                        //    _psocketClient.Open();
                        //    _psocketClient.Send(byt_Press2Line2);
                        //    _psocketClient._pCoFAS_Log.WLog("->" + BitConverter.ToString(byt_Press2Line2));
                        //}
                        //Add_ListView("전송", "");
                    }
                }
            }
            catch (Exception ex)
            {
                _psocketClient._pCoFAS_Log.WLog(ex.ToString());
            }

        }
        #endregion

        #endregion
        #region PLC4 X100,C300QTR,C300SUB 설비상태 타이머 실행함수
        private void PLC4X100Read()
        {
            Plc4X100_1SendData();

        }
        private void PLC4X100Read2()
        {
            Plc4X100_2SendData();

        }
        private void PLC4C300QTRRead()
        {
            Plc4C300QTR_1SendData();
            Plc4C300QTR_2SendData();
        }
        private void PLC4C300SUBRead()
        {
            Plc4C300SUB_1SendData();
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

        #region ○ 기본 실행
        public ucGatheringControl(ServerEntity ser)
        {
            this.ser = ser;

            InitializeComponent();
            lblName.Text = ser.Name;

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
            //lboxLog.Items.Insert(0, ser.Api_ip+"/"+ser.Api_port);
            //simpleButton1.PerformClick();
        }
        private void UserControl1_Load(object sender, EventArgs e)
        {
           // lblName.Text = ser.Name;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RsType"></param>
        /// <param name="_Bool">connect클릭=true / disconnect클릭 = false</param>
        #region ○ 리소스 타입 설정
        private void reSourceType_Setting(string RsType, bool _Bool)
        {
            #region 변수설정

            cnip = txtIP.Text.Split(' ')[0];
            cnport = txtIP.Text.Split(' ')[2];

            #endregion
            //lboxLog.Items.Insert(0, RsType.ToString());
            #region ○ 리소스별 세팅


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
                    
                    if (_Bool)
                    {
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);
                        lboxLog.Items.Insert(0, "클라이언트 시작");
                    }
                    else
                    {
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, Timeout.Infinite);
                        lboxLog.Items.Insert(0, "클라이언트 종료");
                    }
                    break;
                case "ATG_RS0003"://PLC4번

                    //클라이언트 세팅
                    _psocketClient = new CoFAS_SocketClient(cnip, Convert.ToInt32(cnport));
                    _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);//저장하는부분 만들어야함

                    //타이머 세팅
                    _tmrATG_PLC0001 = new System.Threading.Timer(new TimerCallback(_tmrATG_PLC0001CallBack), null, 2000, Timeout.Infinite);
                    if (_Bool)
                    {
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);
                        lboxLog.Items.Insert(0, "클라이언트 시작");
                    }
                    else
                    {
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, Timeout.Infinite);
                        lboxLog.Items.Insert(0, "클라이언트 종료");
                    }



                    break;
                case "ATG_D3000":
                    //클라이언트 세팅
                    _psocketClient = new CoFAS_SocketClient(cnip, Convert.ToInt32(cnport));
                    _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);//저장하는부분 만들어야함
                                                                                                   //타이머 세팅
                    _tmrATG_PLC0001 = new System.Threading.Timer(new TimerCallback(d3000CallBack), null, 2000, Timeout.Infinite);
                    if (_Bool)
                    {
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);
                        lboxLog.Items.Insert(0, "클라이언트 시작");
                    }
                    else
                    {
                        _tmrATG_PLC0001.Change(int.Parse("0") * 1000, Timeout.Infinite);
                        lboxLog.Items.Insert(0, "클라이언트 종료");
                    }

                    break;
                case "TEST1":
                    Newser = new RestServer();
                    Newser.Port = "80";
                    break;
                case "TEST2":
                    Newser.Port = "82";
                    break;
                case "TEST3":
                    _psocketClient = new CoFAS_SocketClient("127.0.0.1", 8080);
                    APISetting();
                    _tmr = new System.Threading.Timer(new TimerCallback(APITestCallBack), null, 2000, Timeout.Infinite);
                    if (_Bool)
                    {
                        _tmr.Change(int.Parse("0") * 1000, int.Parse("3") * 1000);
                        lboxLog.Items.Insert(0, "클라이언트 시작");
                    }
                    else
                    {
                        _tmr.Change(int.Parse("0") * 1000, Timeout.Infinite);
                        lboxLog.Items.Insert(0, "클라이언트 종료");
                    }


                    break;
                #region ○ 하이월드 (구)온습도 센서
                case "RS01":
                    ser.Port = "8705";
                    break;
                #endregion
                #region 오토젠 용접1(X100)
                case "RS0031":
                    {
                        APISetting();

                        byte_setting();
                        _pSocketServer = new CoFAS_SocketServer(Convert.ToInt32(ser.Port), 300);
                        _pSocketServer.evtReceiveRequest = new CoFAS_SocketServer.delReceiveRequest(receiverequest);
                        _pSocketServer.evtClentConnect = new CoFAS_SocketServer.delClientConnect(clientconnect);
                        #region ○ PLC에 신호 보내기위해 이벤트생성
                        _psocketClient = new CoFAS_SocketClient("192.168.21.31", 2004);
                        _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);
                        //_psocketClient.Open();
                        #endregion
                        //_pSocketServer._pCoFAS_Log.WLog(ser.Port);
                    }
                    break;
                #endregion
                #region 오토젠 용접2(C300)
                case "RS0032":
                    {
                        APISetting();

                        byte_setting();
                        _pSocketServer = new CoFAS_SocketServer(Convert.ToInt32(ser.Port), 300);
                        _pSocketServer.evtReceiveRequest = new CoFAS_SocketServer.delReceiveRequest(receiverequest);
                        _pSocketServer.evtClentConnect = new CoFAS_SocketServer.delClientConnect(clientconnect);
                        //_pGatheringUcCtlEntity.IS_COMPLETE_C300 = "Y";
                        #region ○ PLC에 신호 보내기위해 Client로 연결 // C300은 QTR, SUB나눠야해서 recieve 이벤트에 클라이언트 설정
                        _psocketClient = new CoFAS_SocketClient("192.168.21.131", 2004);
                        _psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);
                        //_psocketClient.Open();
                        #endregion
                    }
                    break;
                #endregion

                default:
                    break;

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
        /// <summary>
        /// 데이터값, 센서코드를 이용하여 API에 데이터를 주는 부분
        /// </summary>
        /// <param name="value">int 혹은 decimal값만 이용할것.</param>
        /// <param name="code">resource_sensor테이블에 센서 등록 필요</param>
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
            ServerEntity loadser = ReadXML(lblName.Text);
            AddForm addfrm = new AddForm(loadser);
            addfrm.Show();
        }
        #endregion
        #region ○ test 버튼 클릭 시
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            
            MessageBox.Show(ser.Api_ip + "/" + ser.Api_port);
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
                            ////log.wlog("Socket 서버 종료", lblName.Text);

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

                            newrest.Reststop(Newser);
                            lboxLog.Items.Insert(0,"Rest 서버 종료");
                            //lboxLog.SelectedIndex = lboxLog.Items.Count - 1;

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
                        #region ○서버모드
                        case "LightSkyBlue": //server
                            #region 서버모드 - 비동기식
                            if (ser.Sync == "async")
                            {
                                timer1.Tick += timer1_Tick_async;
                                //비동기식 서버
                                threadReceiveData = new Thread(new ThreadStart(startserverTrades));
                                threadReceiveData.IsBackground = true;
                                threadReceiveData.Start();
                            }
                            #endregion
                            #region 서버모드 - 동기식
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
                                _syncser._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                                lboxLog.Items.Insert(0,"동기식 서버 Open");
                                lboxLog.Items.Insert(0,"대기중...");
                                lboxLog.SelectedIndex = lboxLog.Items.Count - 1;
                                Thread newser = new Thread(new ThreadStart(_syncser.Start));
                                newser.IsBackground = true;

                                newser.Start();
                                timer1.Start();
                            }
                            #endregion
                            break;
                        #endregion
                        #region ○클라이언트 모드
                        case "Gold": //client
                            #region 클라이언트 모드 - 비동기식
                            if (ser.Sync == "async")//비동기식 클라이언트
                            {
                                reSourceType_Setting(ser.Resource_code, true);
                                //로그 세팅
                                _psocketClient.Logname = lblName.Text;
                                _psocketClient._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                            }
                            #endregion
                            #region 클라이언트 모드 - 동기식
                            else   //동기식 클라이언트
                            {

                                synccli = new SyncClient(lboxLog, lblName.Text);
                                synccli._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                                synccli.Host = ser.Ip;
                                synccli.Portnum = Convert.ToInt32(ser.Port);
                                synccli.Start();
                                synccli.SendData("test");
                            }
                            #endregion
                            simpleButton2.Enabled = true;


                            break;
                        #endregion
                        #region ○레스트 서버
                        case "LightGreen": //rest
                            reSourceType_Setting(ser.Resource_code, true);
                            //startrestTrades();
                            threadReceiveData = new Thread(new ThreadStart(startrestTrades));
                            threadReceiveData.IsBackground = true;
                            threadReceiveData.Start();
                            //simpleButton1.Enabled = false;

                            break;
                            #endregion
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
                else
                {
                    MessageBox.Show("이미 제거되었습니다.");

                }
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
            _pSocketServer.Logname = lblName.Text;
            _pSocketServer._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);


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
            //if (newser.Host != null)
            //{

            //    //newser.LogToConsole().Dispose();
            //    newser.LogToConsole().Start();
            //    lboxLog.Invoke(new Action(delegate ()
            //    {
            //        lboxLog.Items.Insert(0, "서버실행 성공!");
            //        //logbox.SelectedIndex = logbox.Items.Count - 1;
            //    }));
            //    newrest._pCoFAS_Log.WLog("server open");
            //    while (newser.IsListening)
            //    {
            //        Thread.Sleep(300);
            //    }
            //}
            //else
            //{
            newrest._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            newrest.Reststart(Newser, lboxLog);
            
            
            //}
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
            //newser.Host = ClientIP;//"localhost";
            //newser.Port = "8800";
            //    newser.Start();

            //while (newser.IsListening)
            //    {
            //        Thread.Sleep(300);
            //    }


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
                                //            //_psocketClient = new CoFAS_SocketClient(dtlist.Rows[0]["IP"].ToString(), Convert.ToInt32(dtlist.Rows[0]["PORT"].ToString()));
                                //            //_psocketClient.evtReceived = new CoFAS_SocketClient.delReceive(evtReceiveSend);
                                //            //_pCoFASLog.WLog("Client 세팅 : "+dtlist.Rows[0]["IP"].ToString() + ":" + dtlist.Rows[0]["PORT"].ToString());
                                //            _psocketClient.strServerIP = dtlist.Rows[0]["IP"].ToString();
                                //            _psocketClient.iPort = Convert.ToInt32(dtlist.Rows[0]["PORT"].ToString());
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
                                //                    if (_psocketClient != null)
                                //                    {
                                //                        if (_psocketClient.isConnected)
                                //                        {
                                //                            _psocketClient.Send(X100_WELD_WR);
                                //                            //_pCoFASLog.WLog("X100_WELD_WR" + BitConverter.ToString(X100_WELD_WR));

                                //                        }
                                //                        else
                                //                        {
                                //                            if (_psocketClient.Open())
                                //                            {
                                //                                _psocketClient.Send(X100_WELD_WR);
                                //                            }
                                //                            //_pCoFASLog.WLog("X100_WELD_WR" + BitConverter.ToString(X100_WELD_WR));

                                //                        }
                                //                        //설비정지 대기중으로 업데이트
                                //                        Thread.Sleep(1000);

                                //                        if (_psocketClient.isConnected)
                                //                        {
                                //                            _psocketClient.Send(X100_WELD_WR2);
                                //                            //_pCoFASLog.WLog("X100_WELD_WR2" + BitConverter.ToString(X100_WELD_WR2));
                                //                            new GatheringUcCtlBusiness().USP_ucGatheringCtl_weld_stop_U10(int.Parse(dtlist.Rows[rc]["WELD_NUM"].ToString()), dtlist.Rows[0]["IP"].ToString());
                                //                            //Add_ListView("전송완료", "");

                                //                        }
                                //                        else
                                //                        {
                                //                            if (_psocketClient.Open())
                                //                            {
                                //                                _psocketClient.Send(X100_WELD_WR2);
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
                        #region ○하이월드 (구)온습도 센서
                        case "RS01":
                            if (this.InvokeRequired)
                            {
                                try
                                {
                                    bool isError = false;
                                    string resource_server = "";
                                    decimal values = 0;
                                    string resource_code = "";
                                    if (bytData.Length >= 20)
                                    {
                                        string str_result = System.Text.Encoding.Default.GetString(bytData);

                                        string[] result = str_result.Split(new string[] { "$" }, StringSplitOptions.None);
                                        if (result == null)
                                            return;
                                        if (result.Length < 2)
                                            return;
                                        //$32,0001111,Humi,13
                                        result = result[1].Split(new string[] { "," }, StringSplitOptions.None);
                                        //result = result[1].Split(',');

                                        //_pGatheringUcCtlEntity.RESOURCE_CODE = "RS" + result[0].ToString().PadLeft(2, '0'); //v_resource_mst
                                        resource_code = "RS" + result[0].ToString().PadLeft(2, '0'); //v_resource_mst
                                        #region hwt_sensor_type 설정,  센서 타입(온도, 습도, PM10, PM2.5) // v_resource_code
                                        //Add_ListView(result[2].ToString().Trim(), "");
                                        switch (result[2].ToString().Trim())
                                        {
                                            case "Humi":
                                                //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "1".PadLeft(4, '0');
                                                resource_server = resource_code + "1".PadLeft(4, '0');
                                                break;

                                            case "Temp":
                                                //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "2".PadLeft(4, '0');
                                                resource_server = resource_code + "2".PadLeft(4, '0');

                                                break;

                                            case "PM10":
                                                //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "3".PadLeft(4, '0');
                                                resource_server = resource_code + "3".PadLeft(4, '0');

                                                //PM수치가 기준치보다 높을경우 경광등을 주황불로 점등시킨다. 반대로 낮을경우 녹색등으로 바꿔준다.
                                                break;

                                            case "PM2.5":
                                                //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "4".PadLeft(4, '0');
                                                resource_server = resource_code + "4".PadLeft(4, '0');

                                                //for (int i = 0; i < _HWDtMinMax.Rows.Count; i++)
                                                //{
                                                //    if (Convert.ToInt32(result[3].ToString()) > Convert.ToInt32(_HWDtMinMax.Rows[i]["m_limit_high"].ToString()) && _HWDtMinMax.Rows[i]["resource_code"].ToString() == "RA800001")
                                                //    {
                                                //        fristalarm1 = "2";
                                                //        //                                            Add_ListView("Yellow Setting", "");
                                                //    }
                                                //    else if (Convert.ToInt32(result[3].ToString()) < Convert.ToInt32(_HWDtMinMax.Rows[i]["m_limit_low"].ToString()) && _HWDtMinMax.Rows[i]["resource_code"].ToString() == "RA800001")
                                                //    {
                                                //        fristalarm1 = "0";
                                                //        //                                           Add_ListView("Green", "");
                                                //    }
                                                //}

                                                break;
                                        }

                                        #endregion
                                        //센서값
                                        //_pGatheringUcCtlEntity.VALUES = Convert.ToDecimal(result[3].ToString());
                                        values = Convert.ToDecimal(result[3].ToString());
                                        //_pGatheringUcCtlEntity.ATTR1 = "";
                                        //_pGatheringUcCtlEntity.ATTR2 = "";
                                        //isError = new GatheringUcCtlBusiness().DQ_Data_Save(_pGatheringUcCtlEntity);  //String 값으로 저장 프로시저
                                        //this.lboxLog.Items.Insert(0, resource_code+" / "+resource_server+" / "+values);
                                        Console.WriteLine(resource_code + " / " + resource_server + " / " + values);
                                        isError = new DpsProvider().USP_SvrSensor_I42(resource_code, "", "", values, resource_server);
                                        if (!isError)
                                        {

                                            // Add_ListView(str_result.ToString(), "");
                                        }
                                        else
                                        {
                                            //Add_ListView("ERROR!!!", "");
                                        }
                                        
                                        #region  12개인데 6개밖에없어서 가라데이터 넣기 - 미사용(20200629nts)
                                        ////_pGatheringUcCtlEntity.RESOURCE_CODE = "RS" + (Convert.ToInt32( result[0].ToString())-10).ToString().PadLeft(2, '0'); //v_resource_mst
                                        //resource_code = "RS" + (Convert.ToInt32(result[0].ToString()) - 10).ToString().PadLeft(2, '0'); //v_resource_mst

                                        //#region hwt_sensor_type 설정,  센서 타입(온도, 습도, PM10, PM2.5) // v_resource_code
                                        //switch (result[2].ToString().Trim())
                                        //{
                                        //    case "Humi":
                                        //        //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "1".PadLeft(4, '0');
                                        //        resource_server = resource_code + "1".PadLeft(4, '0');
                                        //        break;

                                        //    case "Temp":
                                        //        //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "2".PadLeft(4, '0');
                                        //        resource_server = resource_code + "2".PadLeft(4, '0');

                                        //        break;

                                        //    case "PM10":
                                        //        //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "3".PadLeft(4, '0');
                                        //        resource_server = resource_code + "3".PadLeft(4, '0');

                                        //        //PM수치가 기준치보다 높을경우 경광등을 주황불로 점등시킨다. 반대로 낮을경우 녹색등으로 바꿔준다.
                                        //        break;

                                        //    case "PM2.5":
                                        //        //_pGatheringUcCtlEntity.RESOURCE_SERVER = _pGatheringUcCtlEntity.RESOURCE_CODE + "4".PadLeft(4, '0');
                                        //        resource_server = resource_code + "4".PadLeft(4, '0');
                                        //        //for (int i = 0; i < _HWDtMinMax.Rows.Count; i++)
                                        //        //{
                                        //        //    if (Convert.ToInt32(result[3].ToString()) > Convert.ToInt32(_HWDtMinMax.Rows[i]["m_limit_high"].ToString()) && _HWDtMinMax.Rows[i]["resource_code"].ToString() == "RA800001")
                                        //        //    {
                                        //        //        fristalarm1 = "2";
                                        //        //        //Add_ListView("Yellow Setting", "");
                                        //        //    }
                                        //        //    else if (Convert.ToInt32(result[3].ToString()) < Convert.ToInt32(_HWDtMinMax.Rows[i]["m_limit_low"].ToString()) && _HWDtMinMax.Rows[i]["resource_code"].ToString() == "RA800001")
                                        //        //    {
                                        //        //        fristalarm1 = "0";
                                        //        //        //Add_ListView("Green", "");
                                        //        //    }
                                        //        //}

                                        //        break;
                                        //}

                                        //#endregion
                                        ////센서값
                                        ////_pGatheringUcCtlEntity.VALUES = Convert.ToDecimal(result[3].ToString());
                                        ////_pGatheringUcCtlEntity.ATTR1 = "";
                                        ////_pGatheringUcCtlEntity.ATTR2 = "";
                                        ////isError = new GatheringUcCtlBusiness().DQ_Data_Save(_pGatheringUcCtlEntity);  //String 값으로 저장 프로시저

                                        //values = Convert.ToDecimal(result[3].ToString());
                                        //isError = new DpsProvider().USP_SvrSensor_I42(resource_code,"","", values, resource_server);
                                        ////if (!isError)
                                        ////{

                                        ////    // Add_ListView("no.2 : " + str_result.ToString(), "");
                                        ////    _pGatheringUcCtlEntity.RESOURCE_CODE = "RS" + result[0].ToString().PadLeft(2, '0'); //v_resource_mst
                                        ////}
                                        ////else
                                        ////{
                                        ////    //Add_ListView("ERROR!!!", "");
                                        ////}
                                        #endregion
                                    }
                                }
                                catch (IndexOutOfRangeException Ioe)
                                {
                                    _pSocketServer._pCoFAS_Log.WLog("하이월드 (구)온습도 센서 IndexOutOfRangeException");
                                }

                                catch (Exception ex)
                                {
                                    _pSocketServer._pCoFAS_Log.WLog("하이월드 (구)온습도 센서");

                                }
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
                    //_pSocketServer.connectedClients[i].Send(new byte[] { 0x24, 0x11, 0x02, 0x02, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x35, 0x25 });
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
                        _pSocketServer._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                    }
                    else// 동기식 서버
                    {
                        if (_syncser != null)
                        {
                            _syncser._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                        }
                    }

                    break;
                case "Gold": //client
                    if (ser.Sync == "async" && _psocketClient != null) // 비동기식 client
                    {
                        _psocketClient._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                    }
                    else// 동기식 client
                    {
                        if (synccli != null)
                        {
                            synccli._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                        }
                    }

                    break;
                case "LightGreen": //rest
                    newrest._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                    checkin.TestResource._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
                    datain.TestResource._pCoFAS_Log = new CoFAS_Log(Application.StartupPath + "\\LOG\\", lblName.Text, 30, ceLog.Checked);
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
        #endregion

        #endregion

    }
    #region ○ 백노드 설정
    class BacNode
    {
        BacnetAddress adr;
        uint device_id;

        public BacNode(BacnetAddress adr, uint device_id)
        {
            this.adr = adr;
            this.device_id = device_id;
        }

        public BacnetAddress getAdd(uint device_id)
        {
            if (this.device_id == device_id)
                return adr;
            else
                return null;
        }
    }

    #endregion

}

