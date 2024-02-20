using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using lab2.data;
using lab2.Entities;

namespace lab2.Network.SMTP
{

    class SMTPServer : TCPListener
    {
        IStorageLetters storageLetter;

        
        private List<string> comands = new List<string>() { 
            "HELO",                       
            "DATA",
            ".",
            "QUIT",
        };
        
        public SMTPServer(IStorageLetters storageLetter,IPAddress ipAddress, int port = 25) : base(ipAddress,port)
        {
            this.storageLetter = storageLetter;
        }       

        protected override async Task ProcessClientAsync(Socket tcpClient)
        {
            
            GreetingTheClient(tcpClient);

            ClientStatus status = ClientStatus.Connected;
            // буфер для накопления входящих данных
            var response = new List<byte>();
            // буфер для считывания одного байта
            var bytesRead = new byte[1];


            Letter currentLetter = new Letter();

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
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("221 SMTP closing connection\n"), SocketFlags.None);
                    break;
                }

                //обработка статуса с разными статусами
                ClientStatus receivedStatus;
                switch (status)
                {
                    
                    case ClientStatus.Connected:
                        
                        receivedStatus = ProcessClientIfConnected(tcpClient, word).Result;
                        status = receivedStatus == ClientStatus.Сonstant ? status : receivedStatus;  
                        
                        break;

                    case ClientStatus.Qualified:
                       
                        receivedStatus = ProcessClientIfQualified(tcpClient, word, currentLetter).Result;
                        status = receivedStatus == ClientStatus.Сonstant ? status : receivedStatus;

                        break;
                    case ClientStatus.Writer:                        
                        receivedStatus = ProcessClientIfWriter(tcpClient, word, currentLetter).Result;
                        status = receivedStatus == ClientStatus.Сonstant ? status : receivedStatus;

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
            tcpClient.SendAsync(Encoding.UTF8.GetBytes("220 SMTP is glad to see you!\n"),SocketFlags.None);
        }

        private async Task<ClientStatus> ProcessClientIfConnected(Socket tcpClient, String? word)
        {
            if (comands.Contains(word))
            {
                //обработка команд без параметров
                if (word == "HELO")
                {          
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("250 domain name should be qualified\n"), SocketFlags.None);
                    return ClientStatus.Qualified;
                }
                else
                {
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("503 Bad sequence of commands\n"), SocketFlags.None);
                    return ClientStatus.Сonstant;
                }
            }
            else
            {   
                await tcpClient.SendAsync(Encoding.UTF8.GetBytes("502 Command not implemented\n"), SocketFlags.None);
                return ClientStatus.Сonstant;
            }
        }
        private async Task<ClientStatus> ProcessClientIfQualified(Socket tcpClient, String? word, Letter currentLetter)
        {
            currentLetter = currentLetter == null ? new Letter() : currentLetter;

            if (comands.Contains(word))
            {
                //обработка команд без параметров
                if (word == "HELO") {
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("503 Bad sequence of commands\n"), SocketFlags.None);
                    return ClientStatus.Сonstant;
                }

                if (word == "DATA") {
                    currentLetter = new Letter();
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("354 Enter mail, end with \".\" on a line by itself\n"), SocketFlags.None);                    
                    return ClientStatus.Writer;
                }
                
                await tcpClient.SendAsync(Encoding.UTF8.GetBytes("502 Command not implemented\n"), SocketFlags.None);
                return ClientStatus.Сonstant;
            }
            else
            {
                if (word.IndexOf("MAIL FROM:") == 0)
                {
                    word = word.Remove(0, "MAIL FROM:".Length);
                    word = word.Trim().ToLower();                  

                    currentLetter.SetAuthor(word);

                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("250 sender accept\n"), SocketFlags.None);
                    return ClientStatus.Сonstant;
                }
                if (word.IndexOf("RCPT TO:") == 0)
                {

                    word = word.Remove(0, "RCPT TO:".Length);
                    word = word.Trim().ToLower();
                  
                    currentLetter.AddRecipient(word);
                    
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes("250 recipient accept\n"), SocketFlags.None);
                    return ClientStatus.Сonstant;
                }

                await tcpClient.SendAsync(Encoding.UTF8.GetBytes("502 Command not implemented\n"), SocketFlags.None);
                return ClientStatus.Сonstant;
            }
        }


        private async Task<ClientStatus> ProcessClientIfWriter(Socket tcpClient, String? word, Letter currentLetter)
        {           
           
            if (word == ".")
            {
                currentLetter.AddLineInText(word);
                storageLetter.AddLetter(currentLetter);
                
                currentLetter = null;

                await tcpClient.SendAsync(Encoding.UTF8.GetBytes("250 OK\n"), SocketFlags.None);
                return ClientStatus.Qualified;
            }
            else
            {
                
                currentLetter.AddLineInText(word);
                
                await tcpClient.SendAsync(Encoding.UTF8.GetBytes($"OK {word}\n"), SocketFlags.None);
                return ClientStatus.Сonstant;
            }
        }
        
        
    }
}
