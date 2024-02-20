using lab2.data;
using lab2.Network.SMTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using lab2.Entities;

namespace lab2.Network.POP3
{
    class POP3Server : TCPListener
    {
        StorageUsers storageUsers;
        IStorageLetters storageLetter;
        User activeUser = null;

        public POP3Server(IStorageLetters storageLetter, StorageUsers storageUsers, IPAddress ipAddress, int port = 110) : base(ipAddress, port)
        {
            this.storageLetter = storageLetter;
            this.storageUsers = storageUsers;
        }

        protected override async Task ProcessClientAsync(Socket tcpClient)
        {
             GreetingTheClient(tcpClient);
             ClientStatus status = ClientStatus.Connected;
            // буфер для накопления входящих данных
            var response = new List<byte>();
            // буфер для считывания одного байта
            var bytesRead = new byte[1];

            while (true)
            {
                // считываем данные до конечного символа
                while (true)
                {
                    var count = await tcpClient.ReceiveAsync(bytesRead, SocketFlags.None);
                    // смотрим, если считанный байт представляет конечный символ, выходим
                    if (count == 0 || bytesRead[0] == '\n') break;
                    // иначе добавляем в буфер
                    response.Add(bytesRead[0]);
                }
                var word = Encoding.UTF8.GetString(response.ToArray());

                //завершаем соединение
                if (word == "QUIT")
                {
                    activeUser = null;
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("221 POP3 closing connection\n"), SocketFlags.None);
                    break;
                }

                //обработка статуса с разными статусами
                ClientStatus receivedStatus = ClientStatus.Сonstant;
                switch (status)
                {

                    case ClientStatus.Connected:

                        receivedStatus = ProcessClientIfConnected(tcpClient, word).Result;
                        status = receivedStatus == ClientStatus.Сonstant ? status : receivedStatus;

                        break;

                    case ClientStatus.Qualified:
                        receivedStatus = ProcessClientIfQualified(tcpClient, word).Result;
                        status = receivedStatus == ClientStatus.Сonstant ? status : receivedStatus;

                        break;
                    case ClientStatus.Writer:
                        break;
                    default:
                        Console.WriteLine("Ошибка статуса клиента");
                        break;
                }


                response.Clear();
            }
            tcpClient.Shutdown(SocketShutdown.Both);
            tcpClient.Close();
        }
        void GreetingTheClient(Socket tcpClient)
        {
            tcpClient.SendAsync(Encoding.UTF8.GetBytes("OK POP3 is glad to see you!\n"), SocketFlags.None);
        }
        private async Task<ClientStatus> ProcessClientIfConnected(Socket tcpClient, String? word)
        {
            if (activeUser == null)
            {
                word = ComandParser(word, "USER");              
                if (storageUsers.FoundUserbyEmail(word))
                {                    
                    activeUser = storageUsers.GetUserbyEmail(word);
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"+OK user found {word}\n"), SocketFlags.None);
                    return ClientStatus.Connected;
                }
                else {
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"-ERR  user not found {word}\n"), SocketFlags.None);
                    return ClientStatus.Connected;
                }
            }
            else
            {
                word = ComandParser(word, "PASS");             
                if (activeUser.Password == word)
                {
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("+OK user qualified\n"), SocketFlags.None);
                    return ClientStatus.Qualified;
                }
                else {
                    activeUser = null;
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("-ERR ivalid pass\n"), SocketFlags.None);
                    return ClientStatus.Connected;
                }
            }
        }

        private string ComandParser(string comand, string nameComand) {
            if (comand.IndexOf(nameComand) == 0)
            {
                comand = comand.Remove(0, nameComand.Length);
                comand = comand.Trim();
            }
            return comand;
        }

        private async Task<ClientStatus> ProcessClientIfQualified(Socket tcpClient, String? word) {
            List<Letter> leters = storageLetter.GetLetter(activeUser.Email);
            int count = leters.Count;
            if (word == "STAT") {
                await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"+OK {count} leters\n"), SocketFlags.None);
                return ClientStatus.Сonstant;
            }
            if (word.IndexOf("RETR") == 0) {
                word = ComandParser(word, "RETR");
                int numberLeter = Convert.ToInt32(word);
                if (count >= numberLeter)
                {
                    string text = leters[numberLeter - 1].GetText();
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"+OK\n"), SocketFlags.None);
                    string[] splitText = text.Split('\n');
                    foreach(string line in splitText[0..(splitText.Length-1)])
                    {
                        await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"{line}\n"), SocketFlags.None);
                    }
                    return ClientStatus.Сonstant;
                }
                else {
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"-ERR\n"), SocketFlags.None);
                    return ClientStatus.Сonstant;
                }

            }
            await tcpClient.SendAsync(Encoding.UTF8.GetBytes("-ERR\n"), SocketFlags.None);
            return ClientStatus.Сonstant;
        }
    }
}
