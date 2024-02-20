using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    internal class UdpClient
    {
        public bool Lilistening { get; private set; } = false;
        public UdpClient() {
           
        }
        public  int Send(string message, IPAddress address, int port)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            return Send(data, address,port);
        }
        public int Send(byte[] message, IPAddress address, int port)
        {
            using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            
            EndPoint remotePoint = new IPEndPoint(address, port);

            int bytes = udpSocket.SendTo(message, remotePoint);

            return bytes;
        }

        public void StartListening()
        {
            Task.Run(async () =>
            {
                using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.PacketInformation, true);
                var localIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"),8889);

                // начинаем прослушивание входящих сообщений
                udpSocket.Bind(localIP);
                Console.WriteLine("UDP-клиент слушает...");


                byte[] data = new byte[256]; // буфер для получаемых данных                                            
                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0); //адрес, с которого пришли данные

                // получаем данные в массив data                
                Lilistening = true;
                while (Lilistening)
                {
                    var result = await udpSocket.ReceiveFromAsync(data, SocketFlags.None, remoteIp);
                    PrintConsole(result, data);
                    DateTime timeReceiving = DateTime.Now;
                    var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);
                    

                    string[] subs = message.Split(" // ");

                    foreach (var sub in subs)
                    {
                        Console.WriteLine($"Substring: {sub}");
                    }
                    DateTime T1 = DateTime.Parse(subs[0]);
                    DateTime T2 = DateTime.Parse(subs[1]); 
                    DateTime T3 = DateTime.Parse(subs[2]);
                    DateTime T4 = timeReceiving;
                   
                    TimeSpan d = T4.Subtract(T1).Subtract(T3.Subtract(T2));
                    DateTime T = T3 + d / 2;
                    Console.WriteLine($"Задержка передачи пакетов d: {d}");
                    Console.WriteLine($"Новое время : {T}");
                    //T` = T4 + t = T3 + d / 2
                };
            });
        }
        public void PrintConsole(SocketReceiveFromResult result, byte[] data)
        {
            Console.WriteLine($"Получено от серввера {result.ReceivedBytes} байт");
            Console.WriteLine($"Удаленный адрес сервера: {result.RemoteEndPoint}");
            var message = Encoding.UTF8.GetString(data, 0, result.ReceivedBytes);
            Console.WriteLine($"Сообщение сервера: {message}");     // выводим полученное сообщение
        }
        public void StopListening()
        {
            Lilistening = false;
        }
    }
}
