using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Transactions;

namespace lab3.Network
{
    abstract class TCPListener
    {

        private Socket tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public int Port { get; private set; }
        public IPAddress IPAddress { get; private set; }

        public bool Lilistening { get; private set; } = false;

        List<ClientObject> clients = new List<ClientObject>(); // все подключения

        public TCPListener(IPAddress ipAddress, int port = 0)
        {
            Port = port;
            IPAddress = ipAddress;
        }

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
                    
                    // создаем новую задачу для обслуживания нового клиента
                    Task.Run(async () => { await ProcessClientAsync(clientObject); RemoveConnection(clientObject.Id); });

                    // вместо задач можно использовать стандартный Thread
                    // new Thread(async ()=> await ProcessClientAsync(tcpClient)).Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally {                 
                this.Disconnect();
            }
           

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
        public abstract Task ProcessClientAsync(ClientObject clientObject);
    }
}
