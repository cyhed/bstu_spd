using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lab3.Network.FTP
{
    internal class ServerFTP: TCPListener
    {
        public ServerFTP(IPAddress ipAddress, int port = 21) : base(ipAddress, port)
        {
        }

        public override async Task ProcessClientAsync(ClientObject clientObject)
        {


            clientObject.Close();            
        }
    }
}
