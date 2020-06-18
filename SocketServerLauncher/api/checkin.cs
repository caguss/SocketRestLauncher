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

namespace SocketServerLauncher
{
    class checkin
    {
        
        [RestResource]
        public class TestResource
        {
            [RestRoute(HttpMethod = HttpMethod.POST, PathInfo = "/checkin")]
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
                        Console.WriteLine("Message\r\n" + context.Request.Payload);
                    }
                }
                catch (Exception ex)
                {

                }

                string unixstr = ((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1,0,0,0))).TotalSeconds).ToString() ;
                
                context.Response.SendResponse("<xml><root><ack>ok</ack><timestamp>" + unixstr + "</timestamp><offset-ch1>0.6</offset-ch1><offset-ch2>1.3</offset-ch2><sample-mode>11</sample-mode></root></xml>");


                return context;

            }



        }

    }
}
