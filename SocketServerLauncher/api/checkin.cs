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
using System.Windows.Forms;

namespace SocketServerLauncher.api
{
    class checkin
    {
        
        [RestResource]
        public class TestResource
        {
            static ListBox logbox;
            public static CoFAS_Log _pCoFAS_Log;
            static public ListBox Logbox
            {
                get
                {
                    return logbox;
                }

                set
                {
                    logbox = value;
                }
            }

            [RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/checkin")]
            public IHttpContext RepeatMe2(IHttpContext context)
            {
                logbox.Invoke(new Action(delegate ()
                {
                    logbox.Items.Insert(0, string.Format("URL: {0}", context.Request.RawUrl));

                }));
                _pCoFAS_Log.WLog(string.Format("URL: {0}", context.Request.RawUrl));

                Console.WriteLine("URL: {0}", context.Request.RawUrl);
                Console.WriteLine("Method: {0}", context.Request.HttpMethod);

                try
                {
                    foreach (string k in context.Request.QueryString)
                    {
                        logbox.Invoke(new Action(delegate ()
                        {
                            logbox.Items.Insert(0, string.Format("{0}: {1}", k, context.Request.QueryString[k]));

                        }));
                        _pCoFAS_Log.WLog(string.Format("{0}: {1}", k, context.Request.QueryString[k]));

                        Console.WriteLine("{0}: {1}", k, context.Request.QueryString[k]);
                    }

                    if (context.Request.HttpMethod.Equals(Grapevine.Shared.HttpMethod.POST))
                    {
                        logbox.Invoke(new Action(delegate ()
                        {
                            logbox.Items.Insert(0, string.Format("Message : " + context.Request.Payload));

                        }));
                        _pCoFAS_Log.WLog(string.Format("Message : " + context.Request.Payload));
                    
                        Console.WriteLine("Message\r\n" + context.Request.Payload);
                    }
                }
                catch (Exception ex)
                {

                }

                string unixstr = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1,0,0,0))).TotalSeconds).ToString() ;
                
                context.Response.SendResponse("<xml><root><ack>ok</ack><timestamp>" + unixstr + "</timestamp><offset-ch1>0.6</offset-ch1><offset-ch2>1.3</offset-ch2><sample-mode>2</sample-mode></root></xml>");


                return context;

            }



        }

    }
}
