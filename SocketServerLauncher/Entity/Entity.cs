using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketServerLauncher
{
    public class Entity
    {
        
        public class Sensor
        {
            public string MAC { get; set; }
            public string TimeStamp { get; set; }
            public double CH1 { get; set; } // pm2.5
            public double CH2 { get; set; }// pm10
            public double CH3 { get; set; }// pm0.5
            public double CH4 { get; set; }// pm1.0
            public double CH5 { get; set; }// 온도
            public double CH6 { get; set; }// 습도
        }
    }
}
