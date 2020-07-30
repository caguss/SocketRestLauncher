using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Xml;

namespace SocketServerLauncher.frm
{
    public partial class frmApiSetting : DevExpress.XtraEditors.XtraForm
    {
        ServerEntity _pApiEntity = new ServerEntity();
        public frmApiSetting()
        {
            InitializeComponent();
        }
        public frmApiSetting(ServerEntity _ApiEntity)
        {
            _pApiEntity = _ApiEntity;
            InitializeComponent();
        }

        /// <summary>
        /// true = 문제있음, false = 문제없음
        /// </summary>
        /// <returns></returns>
        private bool checkIP(string _Ip)
        {
            bool next = false;
            int result;

            string[] ip = _Ip.Split('.');
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

        private void _btnSave_Click(object sender, EventArgs e)
        {
            //데이터 검증
            if (_tbIp.Text == "")
            {
                MessageBox.Show("IP와 PORT를 모두 입력해주세요.", "경고", MessageBoxButtons.OK);
                return;
            }
            if (checkIP(_tbIp.Text))
            {
                MessageBox.Show("IP주소의 양식이 올바르지 않습니다.", "경고", MessageBoxButtons.OK);
                return;
            }

            //정상 데이터일때 저장
            if (MessageBox.Show("저장하시겠습니까?", "저장", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                //완료 후 닫기
                _pApiEntity.Api_ip = _tbIp.Text;
                _pApiEntity.Api_port = _tbPort.Text;
                CreateXML(_pApiEntity);
                MessageBox.Show("저장되었습니다. 시스템을 재시작해주세요.", "저장완료", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private void CreateXML(ServerEntity ser)
        {
            // 생성할 XML 파일 경로와 이름, 인코딩 방식을 설정합니다.
            XmlTextWriter textWriter = new XmlTextWriter(Application.StartupPath.ToString() + @"\ApiSetting\" + "ApiSettingFile.xml", Encoding.UTF8);
            // 들여쓰기 설정
            textWriter.Formatting = System.Xml.Formatting.Indented;

            // 문서에 쓰기를 시작합니다.
            textWriter.WriteStartDocument();
            // 루트 설정
            textWriter.WriteStartElement("root");

            // 노드와 값 설정
            textWriter.WriteStartElement("api_ip");
            textWriter.WriteString(_pApiEntity.Api_ip);
            textWriter.WriteEndElement();

            textWriter.WriteStartElement("api_port");
            textWriter.WriteString(_pApiEntity.Api_port);
            textWriter.WriteEndElement();


            textWriter.WriteEndElement();

            textWriter.WriteEndDocument();
            textWriter.Close();

        }

    }
}
