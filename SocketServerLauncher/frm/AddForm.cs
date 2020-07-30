using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace SocketServerLauncher
{
    public partial class AddForm : Form
    {

        ServerEntity newserver = new ServerEntity();
        
        public AddForm()
        {
            InitializeComponent();
            newserver.Server = "";
        }

        public AddForm(ServerEntity ser)
        {
            InitializeComponent();
            newserver = ser;
            txtName.Text = ser.Name;
            txtIP.Text = ser.Ip;
            txtPort.Text = ser.Port;
            radioGroup1.EditValue = ser.Sync;
            radioGroup2.EditValue = ser.Sql;


        }
        

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("서버 이름을 입력해 주세요.", "오류");
            }
            else if (newserver.Server == "")
            {
                MessageBox.Show("서버 형식을 선택해 주세요.", "오류");
            }
            else if (radioGroup1.EditValue == null && radioGroup2.EditValue == null)
            {
                MessageBox.Show("상세 정보를 클릭해 주세요.", "오류");
            }
            else if (txtPort.Text == "")
            {
                MessageBox.Show("포트 번호를 입력해 주세요.", "오류");
            }
            else if (_tbResourceCode.Text == "")
            {
                MessageBox.Show("리소스 코드를 입력해 주세요.", "오류");
            }

            else if (txtIP.Enabled == true)
            {
                if (checkIP())
                {
                    MessageBox.Show("IP주소를 확인해 주세요.", "오류");
                }
                else
                {
                    newserver.Name = txtName.Text;
                    newserver.Ip = txtIP.Text;
                    newserver.Port = txtPort.Text;
                    newserver.Resource_code = _tbResourceCode.Text; // 나머지 옵션은 클릭이벤트에서 일어남
                    //xml 파일 생성
                    CreateXML(newserver);


                    MessageBox.Show("저장이 완료되었습니다. 프로그램을 재시작 해주세요.", "저장완료", MessageBoxButtons.OK);
                    this.Close();
                }
            }
            else
            {
                newserver.Name = txtName.Text;
                newserver.Ip = txtIP.Text;
                newserver.Port = txtPort.Text;
                newserver.Resource_code = _tbResourceCode.Text; // 나머지 옵션은 클릭이벤트에서 일어남
                //xml 파일 생성
                CreateXML(newserver);


                MessageBox.Show("저장이 완료되었습니다. 프로그램을 재시작 해주세요.", "저장완료", MessageBoxButtons.OK);
                this.Close();
            }
           
               

        }
        private bool checkIP() // true = 문제있음, false = 문제없음
        {
            bool next = false;
            int result;

            string[] ip = txtIP.Text.Split('.');
            if (ip.Count() != 4)
            {
                next = true;
            }
            else
            {
                for (int i = 0; i < ip.Count(); i++)
                {
                    if (int.TryParse(ip[i], out result))
                    {
                        if (result > 255)
                        {
                            next = true;
                        }
                    }

                }
            }
            
            return next;
        }
        private void txtIP_Leave(object sender, EventArgs e)
        {
           
            //ip의 양식이 아니면 errorprovider 생성
            if (checkIP())
            {
                EP1.SetError(txtIP, "IP 주소 양식이 아닙니다. 다시 작성해주세요");
            }
            else
            {
                EP1.SetError(txtIP, "");
            }
        }
        

        private void btnCancel_MouseDown(object sender, MouseEventArgs e)
        {
            if (MessageBox.Show("정말 취소하시겠습니까?", "취소", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                this.Close();
            }
            else
            {
                return;
            }
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            btnServer.Appearance.BorderColor = Color.Empty;
            btnClient.Appearance.BorderColor = Color.Empty;
            btnRest.Appearance.BorderColor = Color.Empty;
            ((DevExpress.XtraEditors.SimpleButton)sender).Appearance.BorderColor = Color.Red;
            newserver.Server = ((DevExpress.XtraEditors.SimpleButton)sender).Tag.ToString();
            if (((DevExpress.XtraEditors.SimpleButton)sender).Text =="R")
            {
                radioGroup1.EditValue = null;
                newserver.Sync = "";
                radioGroup1.Enabled = false;

            }
            else
            {
                radioGroup1.Enabled = true;
            }

            if (((DevExpress.XtraEditors.SimpleButton)sender).Text == "S")
            {
                txtIP.Text = "";
                txtIP.Enabled = false;
            }
            else
            {
                txtIP.Enabled = true;
            }
        }

        private void radioGroup2_SelectedIndexChanged(object sender, EventArgs e)
        {
            newserver.Sql = radioGroup2.EditValue.ToString();
        }

        private void radioGroup1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (radioGroup1.EditValue != null)
            {
                newserver.Sync = radioGroup1.EditValue.ToString();
            }
        }


        // <summary>
        /// XML 생성
        /// </summary>

        private void CreateXML(ServerEntity ser)
        {
            // 생성할 XML 파일 경로와 이름, 인코딩 방식을 설정합니다.
            XmlTextWriter textWriter = new XmlTextWriter(Application.StartupPath.ToString()+ @"\server\" +txtName.Text+ ".xml", Encoding.UTF8);
            // 들여쓰기 설정
            textWriter.Formatting = System.Xml.Formatting.Indented;

            // 문서에 쓰기를 시작합니다.
            textWriter.WriteStartDocument();

            // 루트 설정
            textWriter.WriteStartElement("root");

            // 노드와 값 설정
            textWriter.WriteStartElement("name");
            textWriter.WriteString(ser.Name);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("resource_code");
            textWriter.WriteString(ser.Resource_code);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("server");
            textWriter.WriteString(ser.Server);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("ip");
            textWriter.WriteString(ser.Ip);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("port");
            textWriter.WriteString(ser.Port);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("sql");
            textWriter.WriteString(ser.Sql);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("sync");
            textWriter.WriteString(ser.Sync);
            textWriter.WriteEndElement();
            
            textWriter.WriteEndElement();

            textWriter.WriteEndDocument();
            textWriter.Close();

        }

        private void AddForm_Load(object sender, EventArgs e)
        {

            switch (newserver.Server)
            {
                case "server":
                    btnServer.PerformClick();
                    break;
                case "client":
                    btnClient.PerformClick();

                    break;
                case "rest":
                    btnRest.PerformClick();

                    break;
            }
        }
    }
}
