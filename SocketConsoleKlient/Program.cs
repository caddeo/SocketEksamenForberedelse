using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketConsoleKlient
{
    class Program
    {
        private static StreamWriter writer;
        private static bool _running;

        static void Main(string[] args)
        {
            TcpClient server = new TcpClient("localhost", 20160);

            _running = true;

            Console.WriteLine("Connected to the server");

            writer = new StreamWriter(server.GetStream());
            writer.AutoFlush = true;

            Thread listen = new Thread(Listen);
            listen.IsBackground = true;
            listen.Start(server);

            while(_running)
            {
                writer.WriteLine(Console.ReadLine());
            }

        }

        private static void Listen(object listento)
        {
            TcpClient server = (TcpClient) listento;
            StreamReader reader = new StreamReader(server.GetStream());

            while (_running)
            {
                string message = reader.ReadLine();
                Console.WriteLine(message);
            }
        }
    }
}
