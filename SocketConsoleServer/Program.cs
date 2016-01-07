using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModelLibrary;

namespace SocketConsoleServeren
{
    class Program
    {
        private static bool _running;
        private static TcpListener _listener;
        private static List<ClientHandler> _clients = new List<ClientHandler>();

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

                ClientHandler clienthandler = new ClientHandler(client);

                lock(_clientslock)
                {
                    _clients.Add(clienthandler);
                }

                // Tilføjer klienten til en tråd så at der kan være flere klienter på en gang
                Thread listenerThread = new Thread(ClientListener);
                // Sørger for det er en backgroundstråd så den lukker med main tråden 
                listenerThread.IsBackground = true;
                listenerThread.Start(clienthandler);
            }
        }

        private static void ClientListener(object incclient)
        { 
            // Converting the incoming client to a TcpClient
            ClientHandler client = (ClientHandler)incclient;
            client.Writer.AutoFlush = true;

            Console.WriteLine("User connected successfully");

            // Imens serveren er oppe så kør
            while(_running)
            {
                client.Writer.WriteLine("Skriv \'dir\' eller \'subdir\' efterfulgt af den stig du vil se om eksitere");
                // Splitter beskeden op. commanden er det første i beskeden og resten er beskeden (message[1] -> message.Count() - 1)
                var message = client.Reader.ReadLine().Split(' ').ToArray<string>();
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
                                client.Writer.WriteLine("Mappen findes.\n Oprettelses dato for mappen: "+directory.CreationTime.ToString("MM-dd-yyyy"));
                            } else
                            {
                                client.Writer.WriteLine("Mappen findes ikke");
                            }
                        }
                        catch (Exception)
                        {
                            client.Writer.WriteLine("Der skete en fejl - prøv igen");
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
                                    client.Writer.WriteLine(" >"+dirinfo.Name + " - " + dirinfo.CreationTime.ToString("MM-dd-yyy"));
                                }
                            } else
                            {
                                client.Writer.WriteLine("Mappen findes ikke");
                            }
                        }
                        catch (Exception)
                        {
                            client.Writer.WriteLine("Der skete en fejl - prøv igen");
                        }

                        break;
                    default:
                        // error
                        client.Writer.WriteLine("Kommando ikke genkendt");
                        break;
                }
            }


        }
    }
}
