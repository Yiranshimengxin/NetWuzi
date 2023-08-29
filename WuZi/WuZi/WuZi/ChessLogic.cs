using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WuZi
{
    internal class Point
    {
        public int x, y;
        public ChessTurn ct { get; set; }

        public Point(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }

    internal class ChessLogic
    {
        public static Point[,] points = new Point[15, 15];  //做一个15*15的二维数组
        public static void Init()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    points[i, j] = new Point(i, j);
                }
            }
        }

        static bool Check(Point p, ChessTurn ct, int offsetX, int offsetY)
        {
            if ((p.x + offsetX * 4 > 14)/*横向延伸是否超过棋盘*/ || (p.y + offsetY * 4 > 14)/*纵向延伸是否超过棋盘*/ || (p.x + offsetX * 4 < 0))
            {
                return false;
            }
            for (int i = 1; i < 5; i++)
            {
                if (points[p.x + offsetX * i, p.y + offsetY * i].ct != ct)
                {
                    return false;
                }
            }
            return true;
        }

        static void CheckWin(ClientState c)
        {
            //遍历棋盘
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    //如果为空，没有棋子，就跳过
                    if (points[i, j].ct != c.player.type)
                    {
                        continue;
                    }
                    //横向判断
                    if (Check(points[i, j], c.player.type, 1, 0))
                    {
                        Result(c);
                        return;
                    }
                    //纵向判断
                    if (Check(points[i, j], c.player.type, 0, 1))
                    {
                        Result(c);
                        return;
                    }
                    //斜向判断
                    if (Check(points[i, j], c.player.type, 1, 1))
                    {
                        Result(c);
                        return;
                    }
                    if (Check(points[i, j], c.player.type, -1, 1))
                    {
                        Result(c);
                        return;
                    }
                }
            }
        }

        //服务器记录棋盘状态
        public static void PlayChess(ClientState cs, string str)
        {
            string[] split = str.Split('_');
            int i = int.Parse(split[0]);
            int j = int.Parse(split[1]);
            points[i, j].ct = cs.player.type;
            CheckWin(cs);
        }

        static void Result(ClientState c)
        {
            c.player.win++;
            ClientState other = Program.GetOpp(c);
            other.player.lost++;
            c.player.isPlayer = false;
            other.player.isPlayer = false;
            SendResult(c.player.type);
        }

        static void SendResult(ChessTurn ct)
        {
            string s = "Result|" + (int)ct;
            foreach (ClientState cs in Program.clients.Values)
            {
                ClientState other = Program.GetOpp(cs);
                string str = s + "_" + cs.player.win + "_" + cs.player.lost + "_" + other.player.win + "_" + other.player.lost;
                Program.Send(cs, str);
            }
        }
    }
}
