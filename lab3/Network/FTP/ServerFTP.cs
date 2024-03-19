using lab2.data;
using lab2.Entities;
using lab3.data;
using lab3.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace lab3.Network.FTP
{
    internal class ServerFTP: TCPListener
    {
        IStorageUsers storageUsers;

        public ServerFTP(IPAddress ipAddress, IStorageUsers storageUsers, int port = 21) : base(ipAddress, port)
        {
            this.storageUsers = storageUsers;
        }

        public override async Task ProcessClientAsync(ClientObject clientObject)
        {
            GreetingTheClient(clientObject.client);

            clientObject.status = ClientStatus.Connected;
            // буфер для накопления входящих данных
            var response = new List<byte>();
            // буфер для считывания одного байта
            var bytesRead = new byte[1];
            try
            {
                while (true)
                {
                    // считываем данные до конечного символа
                    response = await ReadStream(clientObject.client);
                    var word = Encoding.UTF8.GetString(response.ToArray());
                    response.Clear();
                    //завершаем соединение
                    if (word == "QUIT")
                    {
                        clientObject.activeUser = null;
                        ExitClient(clientObject.client);
                        break;
                    }

                    //обработка статуса с разными статусами
                    
                    switch (clientObject.status)
                    {

                        case ClientStatus.Connected:
                            await ProcessClientIfConnected(clientObject, word);
                            break;

                        case ClientStatus.Qualified:
                            await ProcessClientIfQualified(clientObject, word);   
                            break;                        
                        default:
                            Console.WriteLine("Ошибка статуса клиента");
                            break;
                    }
                    
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            finally { 
                clientObject.Close(); 
            }                        
        }

        private async Task ProcessClientIfConnected(ClientObject clientObject, String? word)
        {
            if (clientObject.activeUser == null)
            {
                word = ComandParser.ParseParameter(word, "USER");
                if (storageUsers.FoundUserbyLogin(word))
                {
                    clientObject.activeUser = storageUsers.GetUserbyLogin(word);
                    await clientObject.client.SendAsync(Encoding.UTF8.GetBytes($"331 User name okay, need password. \n"), SocketFlags.None);                    
                }
                else
                {
                    await clientObject.client.SendAsync(Encoding.UTF8.GetBytes($"530 Not logged in. \n"), SocketFlags.None);                    
                }                
            }
            else
            {
                word = ComandParser.ParseParameter(word, "PASS");
                if (clientObject.activeUser.Password == word)
                {
                    await clientObject.client.SendAsync(Encoding.UTF8.GetBytes("230 user qualified\n"), SocketFlags.None);
                    clientObject.status = ClientStatus.Qualified;
                }
                else
                {
                    clientObject.activeUser = null;
                    await clientObject.client.SendAsync(Encoding.UTF8.GetBytes("530 Not logged in.\n"), SocketFlags.None);                    
                }
            }
        }

        private async Task ProcessClientIfQualified(ClientObject clientObject, String? word)
        {
            if (word == "PASV")
            {
                await clientObject.client.SendAsync(Encoding.UTF8.GetBytes($"227 Entering Passive Mode ({this.IPAddress},20)\n"), SocketFlags.None);
               
            }
            if (word == "NOOP") {
                await clientObject.client.SendAsync(Encoding.UTF8.GetBytes($"200\n"), SocketFlags.None);
               
            }
            if (word.IndexOf("RETR") == 0)
            {
                string path = ComandParser.ParseParameter(word, "RETR");             
            }
            if (word == "STOR")
            {
                string path = ComandParser.ParseParameter(word, "STOR");               
            }
        }


        private async Task<List<byte>> ReadStream(Socket client)
        {
            var bytesRead = new byte[1];
            var response = new List<byte>();
            while (true)
            {
                var count = await client.ReceiveAsync(bytesRead, SocketFlags.None);
                // смотрим, если считанный байт представляет конечный символ, выходим
                if (count == 0 || bytesRead[0] == '\n') break;
                // иначе добавляем в буфер
                response.Add(bytesRead[0]);
            }
            return response;            
        }       

        private async void GreetingTheClient(Socket tcpClient)
        {
            await tcpClient.SendAsync(Encoding.UTF8.GetBytes("220 FTP server ready.\n"), SocketFlags.None);
        }

        private async void ExitClient(Socket tcpClient)
        {
            await tcpClient.SendAsync(Encoding.UTF8.GetBytes("221 Goodbye.\n"), SocketFlags.None);
        }

    }
}
