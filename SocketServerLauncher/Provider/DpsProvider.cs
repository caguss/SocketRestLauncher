using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using MySql.Data.MySqlClient;

using System.Data;
using System.Data.Common;

namespace SocketServerLauncher.Provider
{
    public class DpsProvider : Entity
    {
        #region ○ DB관련 기본 설정
        #region ○ DB커넥션
        public void DBConnection()
        {
            using (con = new MySqlConnection("Server=m.coever.co.kr;Port=3306;Database=coever_mes_hwt;Uid=dbmes;Pwd=dbmes1!"))
            {
                try//예외 처리
                {
                    con.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DB커넥션 실패 : 0x0000");
                    //("DB커넥션 연결 실패! \r\n오류코드 : 0x0000");
                }

            }
        }
        #endregion
        #region ○ getdatatable()
        public MySqlConnection con;
        public MySqlCommand cmd;
        private MySqlTransaction _pDbTransaction;


        private DbCommand GetDbCommand(string _pCommandText, IDataParameter[] pDataParameters)
        {

            cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = _pCommandText;
            cmd.CommandTimeout = 300;
            if (pDataParameters != null)
            {
                foreach (DbParameter pDbParameter in pDataParameters)
                {
                    cmd.Parameters.Add(pDbParameter);
                }
            }

            if (_pDbTransaction != null)
            {
                cmd.Transaction = _pDbTransaction;
            }


            return cmd;
        }

        public DataTable GetDataTable(CommandType pCommandType, string strCommandText, IDataParameter[] pDataParameters)
        {
            try
            {
                DbDataAdapter pDbDataAdapter = new MySqlDataAdapter();
                pDbDataAdapter.SelectCommand = GetDbCommand(strCommandText, pDataParameters);
                DataTable pDataTable = new DataTable();
                pDbDataAdapter.Fill(pDataTable);

                return pDataTable;
            }
            catch (Exception ex)
            {
                return new DataTable();
                Console.WriteLine("GetDataTable오류 !");
            }
        }

        #endregion
        #endregion

        #region ○ data_collection 범용 프로시저
        public bool USP_SvrSensor_I42(string v_resource_mst, string v_property_value, string v_condition_code, decimal v_collection_value, string v_resource_code)
        {
            DBConnection();
            cmd = new MySqlCommand();
            IDataParameter[] pDataParameters = null;

            pDataParameters = new IDataParameter[]
                        {
                            new MySqlParameter("@v_resource_mst", MySqlDbType.VarChar, 50),
                            new MySqlParameter("@v_property_value", MySqlDbType.VarChar, 50),
                            new MySqlParameter("@v_condition_code", MySqlDbType.VarChar, 50),
                            new MySqlParameter("@v_collection_value", MySqlDbType.Decimal),
                            new MySqlParameter("@v_resource_code", MySqlDbType.VarChar, 50),

                        };
            pDataParameters[0].Value = v_resource_mst;
            pDataParameters[1].Value = v_property_value;
            pDataParameters[2].Value = v_condition_code;
            pDataParameters[3].Value = v_collection_value;
            pDataParameters[4].Value = v_resource_code;

            DataTable pDataTable = GetDataTable(CommandType.StoredProcedure, "USP_SvrSensor_I42", pDataParameters);
            if (pDataTable.Rows[0]["err_msg"].ToString() == "OK")
            {
                //Console.WriteLine("프로시저 실행 완료");
                return true;

            }
            else if(pDataTable==null ||pDataTable.Rows.Count<=0)
            {
                //Console.WriteLine("프로시저 실행 실패");
                return false;
            }
            con.Close();

            return true;

        }
        #endregion
        public bool DQ_Data_Save(ProviderEntity _pProviderEntity)
        {
            DBConnection();

            bool pErrorYN = false; // 정상 저장 TRUE / 저장 오류 FALSE
                                   //int pCount = 0;
            IDataParameter[] pDataParameters = null;
            try
            {
                        pDataParameters = new IDataParameter[]
                        {
                                    new MySqlParameter("@v_resource_code", MySqlDbType.VarChar,50),
                                    new MySqlParameter("@v_property_value", MySqlDbType.VarChar,50),
                                    new MySqlParameter("@v_condition_code", MySqlDbType.VarChar,50),
                                    new MySqlParameter("@v_collection_value", MySqlDbType.Decimal),
                                    new MySqlParameter("@v_resource_mst", MySqlDbType.VarChar,50)
                        };

                pDataParameters[0].Value = _pProviderEntity.Resource_server.Trim();  // rsxx000x
                pDataParameters[1].Value = _pProviderEntity.Attr1;
                pDataParameters[2].Value = _pProviderEntity.Attr2;
                pDataParameters[3].Value = _pProviderEntity.Value;
                pDataParameters[4].Value = _pProviderEntity.Resource_code;  // ex ) RS0001

                //프로시저 생성
                DataTable pDataTable = GetDataTable(CommandType.StoredProcedure, "USP_SvrSensor_I41", pDataParameters);

                if (pDataTable.Rows[0][0].ToString() != "00")
                {
                    pErrorYN = true;
                }


            }
            catch (Exception pException)
            {
                pErrorYN = true;
                //CoFAS_DevExpressManager.ShowInformationMessage(pException.ToString());
            }

            return pErrorYN;
        }
        public void USP_ucGatheringCtl_TipClear_U10()
        {
            DataTable pDataTable;
            try
            {
                IDataParameter[] pDataParameters = null;

                        pDataParameters = new IDataParameter[]
                        {
                        };


                pDataTable = GetDataTable(CommandType.StoredProcedure, "USP_ucGatheringCtl_TipClear_U10", pDataParameters);

            }
            catch (Exception pException)
            {

            }

            return;
        }

    }
}
