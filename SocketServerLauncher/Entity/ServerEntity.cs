using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerLauncher
{

    public class ServerEntity
    {
        private string name = ""; // 서버이름
        private string server = ""; //서버종류
        private string ip = ""; // ip주소
        private string port = ""; //포트번호
        private string sql = ""; // sql 형식
        private string sync = ""; // 동기화종류
        private string resource_code = ""; // 코드
        private string api_ip = ""; // api ip주소
        private string api_port = "";// api 포트번호
        public string Server
        {
            get
            {
                return server;
            }

            set
            {
                server = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        public string Port
        {
            get
            {
                return port;
            }

            set
            {
                port = value;
            }
        }

        public string Sql
        {
            get
            {
                return sql;
            }

            set
            {
                sql = value;
            }
        }

        public string Sync
        {
            get
            {
                return sync;
            }

            set
            {
                sync = value;
            }
        }

        public string Resource_code
        {
            get
            {
                return resource_code;
            }

            set
            {
                resource_code = value;
            }
        }

        public string Api_ip
        {
            get
            {
                return api_ip;
            }

            set
            {
                api_ip = value;
            }
        }

        public string Api_port
        {
            get
            {
                return api_port;
            }

            set
            {
                api_port = value;
            }
        }
    }
    public class ProviderEntity
    {
        private string resource_server;
        private decimal value;
        private string attr1;
        private string attr2;
        private string resource_code;
        public string Resource_server
        {
            get
            {
                return resource_server;
            }

            set
            {
                resource_server = value;
            }
        }

        public decimal Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public string Attr1
        {
            get
            {
                return attr1;
            }

            set
            {
                attr1 = value;
            }
        }

        public string Attr2
        {
            get
            {
                return attr2;
            }

            set
            {
                attr2 = value;
            }
        }

        public string Resource_code
        {
            get
            {
                return resource_code;
            }

            set
            {
                resource_code = value;
            }
        }
    }

}
