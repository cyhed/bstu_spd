using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace lab3.Network
{
    class ClientObject 
    {
        protected internal string Id { get; } = Guid.NewGuid().ToString();
        Socket client;

        public ClientObject(Socket tcpClient)
        {
            client = tcpClient;            
        }

        public void Close()
        {
            this.client.Shutdown(SocketShutdown.Both);
            this.client.Close();
        }       
    }    
}
