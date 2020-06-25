using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerLauncher
{
    public class ServerEntity
    {
        private string name;
        private string server;
        private string ip;
        private string port;
        private string sql;
        private string sync;
        private string resource_code;       

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
    }
}
