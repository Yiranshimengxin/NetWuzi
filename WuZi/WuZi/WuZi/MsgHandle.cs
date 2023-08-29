using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WuZi
{
    internal class MsgHandle
    {
        public static void MsgEnter(ClientState c, string msgArgs)
        {
            c.player.pname = msgArgs;
            string sendStr = "Enter|" + c.socket.LocalEndPoint;
            Console.WriteLine("玩家" + msgArgs + "上线");
            Program.Send(c, sendStr);
        }

        public static void MsgGameStart(ClientState c, string msgArgs)
        {
            c.player.isPlayer = true;
            if (Program.clients.Count < 2)
            {
                return;
            }
            ClientState opp = Program.GetOpp(c);
            if (!opp.player.isPlayer)
            {
                return;
            }
            c.player.type = 3 - c.player.type;
            opp.player.type = 3 - c.player.type;
            foreach (ClientState item in Program.clients.Values)
            {
                ClientState other = Program.GetOpp(item);
                Program.Send(item, "GameStart|" + (int)item.player.type + "_" + other.player.pname);
            }
            ChessLogic.Init();
        }

        public static void MsgList(ClientState c, string msgArgs)
        {
            string sendStr = "List|";
            foreach (ClientState cs in Program.clients.Values)
            {
                sendStr += cs.socket.RemoteEndPoint.ToString();
            }
            Program.Send(c, sendStr);
        }

        public static void MsgPlay(ClientState c, string msgArgs)
        {
            string sendStr = "Play|" + msgArgs + "_" + (int)c.player.type;
            foreach (ClientState cs in Program.clients.Values)
            {
                Program.Send(cs, sendStr);
                Console.WriteLine("发送：" + sendStr);
            }
            Console.WriteLine("___________");
            ChessLogic.PlayChess(c, msgArgs);
        }
    }
}
