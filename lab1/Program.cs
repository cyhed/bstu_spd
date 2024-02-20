using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

 

namespace lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            NTPServer server = new NTPServer(8888);    
            server.StartListening();            

            Console.ReadKey();






            UdpClient client = new UdpClient();
            client.StartListening();
            int bytesSend = 0;

            

            

            
            DateTime now = DateTime.Now;
            
            bytesSend = client.Send(now.ToString(), IPAddress.Parse("127.0.0.1"), 8888);
            Console.WriteLine($"Отправлено {bytesSend} байт");   

            Console.ReadKey();
        }

        
    }
}