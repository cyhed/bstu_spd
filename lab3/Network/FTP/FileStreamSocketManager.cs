using lab3.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lab3.Network.FTP
{
    internal class FileStreamSocketManager
    {
        public string RealPath{get;private set;}
        public string HomeDir { get; private set; } = @"\home";

        public FileStreamSocketManager(IPAddress ipAddress,  int port = 20) {
            RealPath = AppDomain.CurrentDomain.BaseDirectory;
            Port = port;
            IPAddress = ipAddress;
        }
        private Socket tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public int Port { get; private set; }
        public IPAddress IPAddress { get; private set; }

        public bool Lilistening { get; private set; } = false;

        List<ClientObject> clients = new List<ClientObject>(); // все подключения

        public void StopListening()
        {
            Lilistening = false;
        }


        public async void ListeningAsync()
        {

            try
            {
                IPEndPoint serverIPEndPoint = new IPEndPoint(IPAddress, Port);
                tcpListener.Bind(serverIPEndPoint);
                tcpListener.Listen();    // запускаем сервер

                Lilistening = true;
                while (Lilistening)
                {
                    // получаем подключение в виде TcpClient
                    var tcpClient = await tcpListener.AcceptAsync();
                    Console.WriteLine("Соединение с {0}", tcpClient.RemoteEndPoint);

                    ClientObject clientObject = new ClientObject(tcpClient);
                    clients.Add(clientObject);                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                this.Disconnect();
            }


        } 
        public void SendFileAsync(IPAddress ipAddress, string path)
        {
            if (!IsFileExists(path))
            {
                //вызов функции FTP для отправки сообщения ошибки передачи
                //550 Запрошенное действие не выполнено. Файл недоступен(например, файл не найден, нет доступа).

                return;
            }
            else {
                //вызов функции FTP 150. Состояние файла в порядке; собираюсь открыть соединение для передачи данных. 
            }

          /* var stream = client.GetStream();
            using var file = File.OpenRead(filename);
            var length = file.Length;
            byte[] lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(length));
            await stream.WriteAsync(lengthBytes);
            await file.CopyToAsync(stream);*/




        }
        private bool IsFileExists(string path){
            return File.Exists(RealPath + HomeDir + path);
        }
        public void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject? client = clients.FirstOrDefault(c => c.Id == id);
            // и удаляем его из списка подключений
            if (client != null) clients.Remove(client);
            client?.Close();
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            foreach (var client in clients)
            {
                client.Close(); //отключение клиента
            }
            tcpListener.Shutdown(SocketShutdown.Both); //остановка сервера
            tcpListener.Close();
        }
       
    }
}
