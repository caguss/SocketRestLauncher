﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SocketServerLauncher
{
    public partial class SocketServerLauncher : Form
    {

        private int tlpCount = 0;
        private int publicMargin = 10;
        private int totalviewcnt = 0; // xml count
        
        public SocketServerLauncher()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            ReadXML();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddForm addfrm = new AddForm();
            addfrm.ShowDialog();
        }

        private void SocketServerLauncher_Load(object sender, EventArgs e)
        {
            lblStarttime.Text = DateTime.Now.ToString();
            //xml 로드
            //xml 개수만큼 uc 생성
            //데이터 추가
        }


        


       



        /// <summary>

        /// XML 파일 읽기

        /// </summary>

        private void ReadXML()
        {
            tlpList.Controls.Clear();
            tlpList.RowCount = 2;
            tlpList.ColumnCount = 1;
            tlpList.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            try
            {
                string Api_ip="";
                string Api_port="";
                #region
                if (!Directory.Exists((Application.StartupPath.ToString() + @"\server\")))
                {
                    Directory.CreateDirectory(Application.StartupPath.ToString() + @"\server\");
                }
                if (!Directory.Exists((Application.StartupPath.ToString() + @"\ApiSetting\")))
                {
                    Directory.CreateDirectory(Application.StartupPath.ToString() + @"\ApiSetting\");
                }

                string[] filePaths = Directory.GetFiles(Application.StartupPath.ToString() + @"\server\", "*.xml",
                                         SearchOption.TopDirectoryOnly);
                XmlDocument xmldoc = new XmlDocument();
                totalviewcnt = filePaths.Count();

                #region API 세팅파일 호출
                string[] filePaths2 = Directory.GetFiles(Application.StartupPath.ToString() + @"\ApiSetting\", "ApiSettingFile.xml",
                         SearchOption.TopDirectoryOnly);
                if (filePaths2.Count() > 0)
                {
                    xmldoc.Load(filePaths2[0].ToString());
                    XmlElement root1 = xmldoc.DocumentElement;
                    XmlNodeList nodes1 = root1.ChildNodes;
                    // 노드 요소의 값을 읽어 옵니다.
                    foreach (XmlNode node in nodes1)
                    {
                        switch (node.Name)
                        {
                            case "api_ip":
                                Api_ip = node.InnerText;
                                break;
                            case "api_port":
                                Api_port = node.InnerText;
                                break;
                        }
                    }
                }


                #endregion
                foreach (string filepath in filePaths)
                {
                    ServerEntity ser = new ServerEntity();
                    ser.Api_ip = Api_ip;
                    ser.Api_port = Api_port;
                    xmldoc = new XmlDocument();
                    xmldoc.Load(filepath);
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
                    #endregion
                    //uc 생성
                    ucGatheringControl mot = new ucGatheringControl(ser);
                    mot.Name = "mot_" + ser.Name;

                    mot.Disposed += ucDisposed;
                    if (tlpCount < 1)
                    {
                        tlpCount = (int)Math.Floor((decimal)Screen.PrimaryScreen.Bounds.Width / (mot.Width + (publicMargin * 2)));
                    }

                    if (tlpList.RowCount - 1 < 1)
                    {

                        tlpList.RowCount = tlpList.RowCount + 1;
                        tlpList.RowStyles.Clear();
                        tlpList.RowStyles.Add(new RowStyle(SizeType.Absolute, mot.Height + (publicMargin * 2)));
                        tlpList.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                    }

                    if (tlpList.ColumnCount - 1 < 1)
                    {
                        tlpList.ColumnCount = tlpList.ColumnCount + tlpCount;
                        tlpList.ColumnStyles.Clear();
                        for (int j = 0; j < tlpCount; j++)
                        {
                            tlpList.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, mot.Width + (publicMargin * 2)));
                        }
                        tlpList.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                    }

                    for (int j = 0; j < tlpList.ColumnCount - 1; j++)
                    {
                        if (tlpList.GetControlFromPosition(j, tlpList.RowCount - 2) == null)
                        {
                            // 해당 위치에 컨트롤이 없으므로 해당 위치에 추가해 준다.
                            tlpList.Controls.Add(mot, j, tlpList.RowCount - 2);   // 컬럼, 로우
                            break;
                        }
                        // 마지막 컬럼까지 돌았는데 컨트롤 추가할 영역이 없다면 신규 Row를 생성하고 i 값 초기화
                        if (j == tlpList.ColumnCount - 2)
                        {
                            tlpList.RowCount = tlpList.RowCount + 1;
                            tlpList.RowStyles.Clear();
                            for (int jj = 0; jj < tlpList.RowCount - 1; jj++)
                            {
                                tlpList.RowStyles.Add(new RowStyle(SizeType.Absolute, mot.Height + (publicMargin * 2)));
                            }
                            tlpList.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                            j = -1;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void lblStarttime_DoubleClick(object sender, EventArgs e)
        {
          frm.frmApiSetting ApiSetting = new frm.frmApiSetting();
            ApiSetting.ShowDialog();

        }
        void ucDisposed(object sender, EventArgs e)
        {
            //ucGatheringControl ucGC = (ucGatheringControl)sender;
            //ucGC.Newser.Stop();
        }


        private void SocketServerLauncher_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //컨트롤 안에 꺼지지않는거 생기면 추가수정
                for (int i = 0; i < this.tlpList.Controls.Count; i++)
                {
                    ucGatheringControl mot = (ucGatheringControl)this.tlpList.Controls[i];
                    if (mot.Newser != null && mot.Newser.IsListening)
                        mot.Newser.Stop();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

