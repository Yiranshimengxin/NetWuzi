using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WuZi
{
    internal class Player
    {
        public Player(Socket s)
        {
            type = ChessTurn.Black;
            socket = s;
        }
        public string pname { set; get; }
        public bool isPlayer { set; get; }
        public ChessTurn type { set; get; }
        public int win { set; get; }
        public int lost { set; get; }
        public Socket  socket { set; get; }

    }
}
