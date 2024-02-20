using lab2.data;
using lab2.Entities;
using lab2.Network.SMTP;
using lab2.Network.POP3;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace lab2
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            IStorageLetters storageLetter = new StorageLetters();
            StorageUsers storageUsers = new StorageUsers();
            User user = new User(1, "you@gmail", "1234");

            storageUsers.AddUser(user);

            SMTPServer server = new SMTPServer(storageLetter, IPAddress.Any, 25);
            server.StartListeningAsync();

            POP3Server popServer = new POP3Server(storageLetter, storageUsers, IPAddress.Any, 110);
            popServer.StartListeningAsync();

            Console.ReadLine();

            
            using var tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                await tcpClient.ConnectAsync("127.0.0.1", 25);


                var response = new List<byte>();
                // буфер для считывания одного байта
                var bytesRead = new byte[1];
                int nunMesg = 0;
                bool flag = true;
                while (flag)
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

                    Console.WriteLine($"Ответ : {word}");
                    response.Clear();
                    // буфер для входящих данных
                    String data = null;
                    switch (nunMesg)
                    {
                        case 0:
                            data ="HELO" ;
                            
                            break;

                        case 1:
                            data = "MAIL FROM: vlad@martovsciu.pms";                            
                            break;

                        case 2:
                            data = "RCPT TO: you@gmail";
                            break;
                        case 3:
                            data = "DATA";  
                            break;
                        case 4:
                            data = "text masg for you";
                            break;
                        case 5:
                            data = "text end mesg";
                            break;
                        case 6:
                            data = ".";
                            break;
                        case 7:
                            data = "QUIT";
                            break;
                        default:
                            flag = false;
                            break;
                    }
                    nunMesg++;

                    Console.WriteLine("==>" + data);
                    await tcpClient.SendAsync(Encoding.UTF8.GetBytes(data + '\n'), SocketFlags.None);  

                }
                tcpClient.Shutdown(SocketShutdown.Both);
                tcpClient.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            using var popClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                await popClient.ConnectAsync("127.0.0.1", 110);


                var response = new List<byte>();
                // буфер для считывания одного байта
                var bytesRead = new byte[1];
                int nunMesg = 0;
                bool flag = true;
                while (flag)
                {
                    // считываем данные до конечного символа
                    while (true)
                    {
                        var count = await popClient.ReceiveAsync(bytesRead, SocketFlags.None);
                        // смотрим, если считанный байт представляет конечный символ, выходим
                        if (count == 0 || bytesRead[0] == '\n') break;
                        // иначе добавляем в буфер
                        response.Add(bytesRead[0]);
                    }
                    var word = Encoding.UTF8.GetString(response.ToArray());

                    Console.WriteLine($"Ответ : {word}");
                    response.Clear();
                    // буфер для входящих данных
                    String data = "";
                    switch (nunMesg)
                    {
                        case 0:
                            data = "USER you@gmail";                           
                            break;

                        case 1:
                            data = "PASS 1234";                           
                            break;
                        case 2:
                            data = "STAT";                           
                            break;

                        case 3:
                            {
                                data = "RETR 1";
                                Console.WriteLine("==>" + data);
                                await popClient.SendAsync(Encoding.UTF8.GetBytes(data + '\n'), SocketFlags.None);
                                while (true)
                                {                               
                                    while (true)
                                    {
                                        var count = await popClient.ReceiveAsync(bytesRead, SocketFlags.None);
                                        // смотрим, если считанный байт представляет конечный символ, выходим
                                        if (count == 0 || bytesRead[0] == '\n') break;
                                        // иначе добавляем в буфер
                                        response.Add(bytesRead[0]);
                                    }
                                    var word1 = Encoding.UTF8.GetString(response.ToArray());

                                    Console.WriteLine($"Ответ : {word1}");
                                    response.Clear();
                                    if(word.Equals('.'))
                                        break;
                                }
                            }
                            
                            break;
                        case 4:
                            data = "QUIT";
                            break;
                        default:
                            flag = false;
                            break;
                    }
                    nunMesg++;

                    Console.WriteLine("==>" + data);
                    await popClient.SendAsync(Encoding.UTF8.GetBytes(data + '\n'), SocketFlags.None);

                }
                popClient.Shutdown(SocketShutdown.Both);
                popClient.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}