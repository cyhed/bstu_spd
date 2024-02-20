using System;
using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace lab1
{
    class NTPServer
    {
        public int Port { get; private set; } = 0;
        public bool Lilistening { get; private set; } = false;
        public NTPServer(int port)
        {   
            Port = port;
        }
        public void StartListening()
        {
            Task.Run(async () =>
            {
                using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
                var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), this.Port);

                // начинаем прослушивание входящих сообщений
                udpSocket.Bind(localIP);
                Console.WriteLine("UDP-сервер запущен...");


                byte[] data = new byte[256]; // буфер для получаемых данных                                            
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0); //адрес, с которого пришли данные
                
                // получаем данные в массив data                
                Lilistening = true;
                while (Lilistening)
                {
                    var result = await udpSocket.ReceiveFromAsync(data, SocketFlags.None, remoteIp);  
                    PrintConsole(result,data);

                    var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);

                    DateTime timeReceiving = DateTime.Now;
                    var parsedDate = DateTime.Parse(message);

                    string answer = message + " // " + timeReceiving.ToString() + " // " + DateTime.Now.ToString();  
                    int bytesSend = Send(answer
                        , IPAddress.Parse(((IPEndPoint)result.RemoteEndPoint).Address.ToString())
                        , 8889);
                    
                    
                };
            }); 
        }

        public void PrintConsole(SocketReceiveFromResult result, byte[] data)
        {
            Console.WriteLine($"Получено сервером {result.ReceivedBytes} байт");
            Console.WriteLine($"Удаленный адрес коиента : {result.RemoteEndPoint}");
            var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);
            Console.WriteLine($"Сообщение от клента: {message}");     // выводим полученное сообщение
        }

        public void StopListening()
        {
            Lilistening = false;
        }
        private int Send(string message, IPAddress address, int port)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return Send(data, address, port);
        }
        private int Send(byte[] message, IPAddress address, int port)
        {
            using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            EndPoint remotePoint = new IPEndPoint(address, port);

            int bytes = udpSocket.SendTo(message, remotePoint);

            return bytes;
        }
    }
}