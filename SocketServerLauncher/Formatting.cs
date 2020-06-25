using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServerLauncher
{
    public class Formatting // formatting here
    {

        public int[] Format_PLC(byte data)
        {
            byte received = data;
            
            //string str = Convert.ToString(received, 2).PadLeft(8, '0');
            int[] _received = new int[8];
            for (int i = 0; i < 8; i++)
            {
                _received[7-i] = (received >>i )& 1;
            }


            return _received;
        }
 
    }
}
