using Grapevine.Interfaces.Server;
using Grapevine.Server;
using Grapevine.Server.Attributes;
using Grapevine.Shared;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Json;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using MySql.Data.MySqlClient;

namespace SocketServerLauncher
{
    class datain
    {


        [RestResource]
        public class TestResource
        {
            [RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/datain")]
            public IHttpContext RepeatMe2(IHttpContext context)
            {
                Console.WriteLine("URL: {0}", context.Request.RawUrl);
                Console.WriteLine("Method: {0}", context.Request.HttpMethod);

                try
                {
                    foreach (string k in context.Request.QueryString)
                    {
                        Console.WriteLine("{0}: {1}", k, context.Request.QueryString[k]);
                    }

                    if (context.Request.HttpMethod.Equals(Grapevine.Shared.HttpMethod.POST))
                    {
                        Console.WriteLine("메세지 : \r\n" + context.Request.Payload);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("오류 : {0}", ex.Message);
                    context.Response.SendResponse("<xml><root><ack>error</ack></root></xml>");
                    return context;

                }


                try
                {
                    //Sensor sen = new Sensor();
                    string[] data = context.Request.Payload.Split('&');
                    //sen.MAC = data[0].Substring(4);

                    string strConn = "Server=m.coever.co.kr;Database=coever_mes_hwt;Uid=dbmes;Pwd=dbmes1!;";

                    using (MySqlConnection conn = new MySqlConnection(strConn))
                    {
                        conn.Open();
                        

                        
                        for (int i = 5; i < data.Count(); i++)
                        {

                            string senddata = data[i];
                            ////SENSORCOLLECTION 수집
                            //sen.TimeStamp = ConvertFromUnixTimestamp( Convert.ToDouble( senddata.Split('|')[0].Substring(5))).ToString();
                            //sen.CH1 = Convert.ToDouble( senddata.Split('|')[1]);
                            //sen.CH2 = Convert.ToDouble( senddata.Split('|')[2]);
                            //sen.CH3 = Convert.ToDouble( senddata.Split('|')[3]);
                            //sen.CH4 = Convert.ToDouble( senddata.Split('|')[4]);
                            //sen.CH5 = Convert.ToDouble( senddata.Split('|')[5]);
                            //sen.CH6 = Convert.ToDouble( senddata.Split('|')[6]);

                            //Sensor_I43(sen, conn);

                            ////DATACOLLECTION 수집
                            //int cnt = senddata.Split('|').Count() - 1;
                            //for (int j = 1; j < cnt; j++)
                            //{
                            //    sen.TimeStamp = senddata.Split('|')[0].Substring(5);

                            //    //파라미터
                            //    Sensor_I42(sen.MAC, "", "", Convert.ToDouble(senddata.Split('|')[j]), j.ToString(), conn);
                            //}

                        }
                    }

                    context.Response.SendResponse("<xml><root><ack>ok</ack></root></xml>");
                    return context;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("오류 : {0}",ex.Message);
                    context.Response.SendResponse("<xml><root><ack>error</ack></root></xml>");
                    return context;
                }




            }
            
            static DateTime ConvertFromUnixTimestamp(double timestamp)
            {
                DateTime origin = new DateTime(1970, 1, 1, 9, 0, 0, 0);
                return origin.AddSeconds(timestamp);

            }

           

            public static void Sensor_I42(string v_resource_mst, string v_property_value, string v_condition_code, double v_collection_value, string v_resource_code, MySqlConnection conn)
            {
                MySqlCommand cmd = new MySqlCommand();

                cmd.CommandText = "USP_SvrSensor_I42";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                cmd.Parameters.Add(new MySqlParameter("@v_resource_mst", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_property_value", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_condition_code", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_collection_value", MySqlDbType.VarChar, 50));
                cmd.Parameters.Add(new MySqlParameter("@v_resource_code", MySqlDbType.VarChar, 50));

                cmd.Parameters["@v_resource_mst"].Value = v_resource_mst;
                cmd.Parameters["@v_property_value"].Value = v_property_value;
                cmd.Parameters["@v_condition_code"].Value = v_condition_code;
                cmd.Parameters["@v_collection_value"].Value = v_collection_value;
                cmd.Parameters["@v_resource_code"].Value = v_resource_code;


                

                cmd.ExecuteNonQuery();

            }
        }

    }
}
