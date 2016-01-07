using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketConsoleServeren
{
    class Program
    {
        private static bool _running;
        private static TcpListener _listener;
        private static List<TcpClient> _clients = new List<TcpClient>();

        private static object _clientslock = new object();

        static void Main(string[] args)
        {
            // Opretter en IP


            // Sætter IP'en og en port til serveren og starter den
            _listener = new TcpListener(IPAddress.Any, 20160);

            _running = true;
            _listener.Start();

            // Hvis serverens ip er valid så udskriver den det
            Console.WriteLine("Started server on "+_listener.Server.LocalEndPoint.ToString());

            // Starter en server tråd så at den kan lytte til klienter men stadig skrive i konsollen eller lave andet arbejde
            Thread clientThread = new Thread(AcceptClient);
            // Sørger for at den lukker ned med main tråden
            clientThread.IsBackground = true;
            clientThread.Start();

            Console.ReadKey();

        }

        private static void AcceptClient()
        {
            while(_running)
            {
                // Lytter til klienten som er connected og tilføjer til listen
                TcpClient client = _listener.AcceptTcpClient();

                lock(_clientslock)
                {
                    _clients.Add(client);
                }

                // Tilføjer klienten til en tråd så at der kan være flere klienter på en gang
                Thread listenerThread = new Thread(ClientListener);
                // Sørger for det er en backgroundstråd så den lukker med main tråden 
                listenerThread.IsBackground = true;
                listenerThread.Start(client);
            }
        }

        private static void ClientListener(object incclient)
        { 
            // Converting the incoming client to a TcpClient
            TcpClient client = (TcpClient)incclient;

            StreamReader reader = new StreamReader(client.GetStream());
            StreamWriter writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;

            Console.WriteLine("User connected successfully");

            // Imens serveren er oppe så kør
            while(_running)
            {
                writer.WriteLine("Skriv \'dir\' eller \'subdir\' efterfulgt af den stig du vil se om eksitere");
                // Splitter beskeden op. commanden er det første i beskeden og resten er beskeden (message[1] -> message.Count() - 1)
                var message = reader.ReadLine().Split(' ').ToArray<string>();
                string command = message[0];

                Console.WriteLine(command);

                switch(command.ToUpper())
                {
                    case "DIR":
                        try
                        {
                            DirectoryInfo directory = new DirectoryInfo(message[1]);

                            if (directory.Exists)
                            {
                                writer.WriteLine("Mappen findes.\n Oprettelses dato for mappen: "+directory.CreationTime.ToString("MM-dd-yyyy"));
                            } else
                            {
                                writer.WriteLine("Mappen findes ikke");
                            }
                        }
                        catch (Exception)
                        {
                            writer.WriteLine("Der skete en fejl - prøv igen");
                        }
                        break;
                    case "SUBDIR":
                        try
                        {
                            DirectoryInfo directory = new DirectoryInfo(message[1]);

                            if(directory.Exists)
                            {
                                var subdirectories = directory.GetDirectories();

                                foreach(var dirinfo in subdirectories)
                                {
                                    writer.WriteLine(dirinfo.Name + " - " + dirinfo.CreationTime.ToString("MM-dd-yyy"));
                                }
                            } else
                            {
                                writer.WriteLine("Mappen findes ikke");
                            }
                        }
                        catch (Exception)
                        {
                            writer.WriteLine("Der skete en fejl - prøv igen");
                        }

                        break;
                    default:
                        // error
                        writer.WriteLine("Kommando ikke genkendt");
                        break;
                }
            }


        }
    }
}
