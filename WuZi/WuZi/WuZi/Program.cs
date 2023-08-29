using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Media;
using System.Reflection;

namespace WuZi
{
    public enum ChessTurn { None, White, Black }

    //客户端类，记录每个客户端相关的信息
    class ClientState
    {
        public Socket socket;  //网络套接字
        public byte[] readBuff = new byte[1024];
        public Player player;
    }

    class Program
    {
        static Socket socket;
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipep = new IPEndPoint(ipAdr, 8888);  //ip地址
            socket.Bind(ipep);  //绑定
            socket.Listen(2);  //监听有多少个客户端连接服务器，目前为2个
            Console.WriteLine("服务器启动!");

            List<Socket> checkRead = new List<Socket>();
            while (true)
            {
                checkRead.Clear();
                checkRead.Add(socket);
                foreach (ClientState item in clients.Values)
                {
                    checkRead.Add(item.socket);
                }
                Socket.Select(checkRead, null, null, 1000);
                foreach (Socket item in checkRead)
                {
                    if (item == socket)
                    {
                        ReadListenfd(item);
                    }
                    else
                    {
                        ReadClient(item);
                    }
                }
            }
        }

        //处理客户端发送来的协议
        private static bool ReadClient(Socket item)
        {
            ClientState state = clients[item];
            int count = 0;  //记录
            try
            {
                //接受数据，同步接收
                count = item.Receive(state.readBuff);
            }
            catch (SocketException ex)
            {
                //执行断线方法
                MethodInfo mei = typeof(EventHandle).GetMethod("OnDisconnect");
                object[] obj = { state };
                mei.Invoke(null, obj);
                item.Close();
                clients.Remove(item);
                Console.WriteLine("Socker close");
                return false;
            }
            if (count == 0)
            {
                //断开通信的最后一条信息长度为0
                MethodInfo mei = typeof(EventHandle).GetMethod("OnDisconnect");
                object[] obj = { state };
                mei.Invoke(null, obj);
                item.Close();
                clients.Remove(item);
                Console.WriteLine("Socker close");
                return false;
            }
            string receStr = Encoding.Default.GetString(state.readBuff, 0, count);
            Console.WriteLine(receStr);
            string[] split = receStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            string funName = "Msg" + msgName;
            MethodInfo mi = typeof(MsgHandle).GetMethod(funName);
            object[] o = { state, msgArgs };
            mi.Invoke(null, o);
            return true;
        }

        private static void ReadListenfd(Socket item)
        {
            Console.WriteLine("连接成功！");
            Socket client = socket.Accept();
            ClientState state = new ClientState();
            state.socket = client;
            state.player = new Player(state.socket);
            clients.Add(client, state);
        }

        private static void AcceptCallBack(IAsyncResult ar)
        {
            Console.WriteLine("连接成功！");
        }

        public static void Send(ClientState state,string sendStr)
        {
            byte[] sengByte=Encoding.Default.GetBytes(sendStr);
            state.socket.Send(sengByte);
        }

        public static ClientState GetOpp(ClientState c)
        {
            foreach (ClientState item in clients.Values)
            {
                if (item != c)
                {
                    return item;
                }
            }
            return null;
        }
    }
}
