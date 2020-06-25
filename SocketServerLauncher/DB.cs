using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerLauncher
{
    class DB
    {
        private string ConnectionString;
        private MySqlConnection conn_MY;
        private SqlConnection conn_MS;
        private string strConn = "";
        ServerEntity ser;
        public DB(ServerEntity _ser)
        {
            ser = _ser;
        }
        public void Open()
        {

            switch (ser.Sql)
            {

                case "mysql":
                    conn_MY = new MySqlConnection(string.Format
                (
                   "Server={0};Database={1};UID={2};PWD={3}",
                   Properties.Resources.DB_IP,
                   Properties.Resources.DB_NM,
                   Properties.Resources.DB_ID,
                   Properties.Resources.DB_PW  // "dbmes1!"  0we11Passw0rd!@#dbmes
                ));
                    conn_MY.Open();
                    break;
                case "mssql":
                    conn_MS = new SqlConnection(string.Format
                (
                   "server = {0}; uid = {1}; pwd = {2}; database = {3 }",
                   Properties.Resources.DB_IP,
                   Properties.Resources.DB_ID,
                   Properties.Resources.DB_PW,// "dbmes1!"  0we11Passw0rd!@#dbmes
                   Properties.Resources.DB_NM
                   ));
                    conn_MS.Open();
                    break;
               
            }
        }



        public void Close()
        {

            switch (ser.Sql)
            {

                case "MYSQL":
                    
                    conn_MY.Close();
                    break;
                case "MSSQL":
                    conn_MS.Close();
                    break;

            }
        }
    }


}
