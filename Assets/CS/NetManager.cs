using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class NetManager
{
    static Socket socket;
    static byte[] readBuff = new byte[1024];
    public delegate void MsgListener(string str);

    //协议，用字典来写
    static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    static List<string> msgList = new List<string>();

    //为协议添加处理方法
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取本机的IP地址
    public static string GetDesc()
    {
        if (socket == null)
        {
            return "";
        }
        if (!socket.Connected)
        {
            return "";
        }
        return socket.LocalEndPoint.ToString();
    }

    //连接方法
    public static void Connect(string ip, int port)
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        if (!Message.CheckMessage(Application.dataPath + "/Swm.dat"))
        {
            return;
        }
        socket.Connect(ip, port);
        socket.BeginReceive(readBuff/*缓冲区*/, 0, 1024, 0, ReceiveCallBack/*回调函数*/, socket);
    }


    private static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            //收到消息的长度
            int count = socket.EndReceive(ar);
            //把二进制转换为字符串
            string recvStr = Encoding.Default.GetString(readBuff, 0, count);
            Debug.Log(recvStr);
            //将信息添加到协议列表中
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("接受失败");
        }
    }

    //发送消息，发送的消息是二进制，将二进制转化为字符串
    public static void Send(string sendStr)
    {
        if (socket == null)
        {
            return;
        }
        if (!socket.Connected)
        {
            return;
        }
        byte[] sendBytes = Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //刷新
    public static void Update()
    {
        //没有消息，return
        if (msgList.Count == 0)
        {
            return;
        }
        //读出协议列表的第一个元素
        string msgStr = msgList[0];
        //移除第一个元素
        msgList.RemoveAt(0);
        //协议分为两部分：协议名 | 协议体
        string[] split = msgStr.Split('|');
        string msgName = split[0];
        string msgArgs = split[1];
        
        if (listeners.ContainsKey(msgName))
        {
            listeners[msgName](msgArgs);
        }
    }
}