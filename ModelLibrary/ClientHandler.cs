using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary
{
    public class ClientHandler
    {
        public StreamReader Reader { get; set; }
        public StreamWriter Writer { get; set; }

        public ClientHandler(TcpClient listener)
        {
            Reader = new StreamReader(listener.GetStream());
            Writer = new StreamWriter(listener.GetStream());
        }
    }
}
