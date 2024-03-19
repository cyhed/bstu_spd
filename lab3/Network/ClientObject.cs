using lab2.Entities;
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

        public Socket client{get;private set;}
        public Socket data { get;private set;}
     
        
        public User activeUser { get; set; } = null;
        public ClientStatus status { get; set; }

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
