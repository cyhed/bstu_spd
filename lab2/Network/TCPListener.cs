using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Transactions;

namespace lab2.Network
{
    abstract class TCPListener
    {

        private Socket tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public int port { get; private set; } 
        public IPAddress ipAddress { get; private set; }

        public bool Lilistening { get; private set; } = false;

        public TCPListener(IPAddress ipAddress, int port = 0)
        {
            this.port = port;
            this.ipAddress = ipAddress;
        }       

        public void StopListening()
        {
            Lilistening = false;
        }


        public async void StartListeningAsync()
        {

            try
            {
                IPEndPoint serverIPEndPoint = new IPEndPoint(ipAddress, port);
                tcpListener.Bind(serverIPEndPoint);
                tcpListener.Listen();    // запускаем сервер
                Console.WriteLine("Сервер запущен {0} . Ожидание подключений... ", serverIPEndPoint);

                this.Lilistening = true;
                while (Lilistening)
                {
                    // получаем подключение в виде TcpClient
                    var tcpClient = await tcpListener.AcceptAsync();
                    Console.WriteLine("Соединение с {0}", tcpClient.RemoteEndPoint);
                    // создаем новую задачу для обслуживания нового клиента
                    Task.Run(async () => await ProcessClientAsync(tcpClient));

                    // вместо задач можно использовать стандартный Thread
                    // new Thread(async ()=> await ProcessClientAsync(tcpClient)).Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }

        // обрабатываем клиент
        protected abstract  Task ProcessClientAsync(Socket tcpClient);
                
    }
}
