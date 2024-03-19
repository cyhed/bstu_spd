using lab3.Network.FTP;
using System.Net;

namespace lab3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            FileStreamSocketManager fileStreamSocketManager = new(IPAddress.Any);
            Console.WriteLine(fileStreamSocketManager.RealPath);
            Console.WriteLine(fileStreamSocketManager.HomeDir);
        }
    }
}