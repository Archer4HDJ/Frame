using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using UnityEngine;
/// <summary>
/// 用来使用socket与服务器连接
/// </summary>
public class NetWorkTool
{
    private  Socket mSocket;
    //private string  stringData = "";
    private  byte[] readData = new byte[1024 * 1024];

    public  bool isConnect = false;

    public  CallBack<byte[]> ReceiveCallBack = null;
    public  CallBack<bool> ConnetResultCallBack = null;
    public  string mIPaddress = "";
    public  int mPort = 0;
    public  IPEndPoint mIPEndPoint = null;
    private  object lockObj_IsConnectSuccess = new object();

    public NetWorkTool(string ipAdress, int port)
    {
        IPAddress ad = IPAddress.Parse(ipAdress);
        mIPEndPoint = new IPEndPoint(ad, port);
        mIPaddress = ipAdress;
        mPort = port;
    }
    //关闭连接
    public  void Close()
    {
        isConnect = false;
        if (mSocket != null)
        {
            mSocket.Close(0);
            mSocket = null;
        }
    }
    //请求数据服务连接线程
    public void Connect()
    {
        try
        {
            mSocket = new Socket(mIPEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mSocket.BeginConnect(mIPEndPoint, ConnectedCallback, mSocket);
            Debug.Log("连接服务器");
        }
        catch(Exception e)
        {
            if (ConnetResultCallBack != null)
                ConnetResultCallBack(false);
            Debug.LogError(e.ToString());
        }
    }
    private  void ConnectedCallback(IAsyncResult iar)
    {
        lock (lockObj_IsConnectSuccess)
        {
            Socket client = (Socket)iar.AsyncState;
            try
            {
                client.EndConnect(iar);
                isConnect = true;
                if (ConnetResultCallBack != null)
                    ConnetResultCallBack(true);
                StartReceive();             //开始
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                if (ConnetResultCallBack != null)
                    ConnetResultCallBack(false);
                isConnect = false;
            }
        }
    }

   private void StartReceive()
   {
        mSocket.BeginReceive(readData, 0, readData.Length, SocketFlags.None, new AsyncCallback(EndReceive), mSocket);
   }

   private void EndReceive(IAsyncResult iar) //接收数据
    {
        Socket remote = (Socket)iar.AsyncState;
        int recv = remote.EndReceive(iar);
        if (recv > 0)
        {
            byte[] copy = new byte[recv];
            for (int i = 0; i < recv; i++)
            {
                copy[i] = readData[i];
            }
            if (ReceiveCallBack != null)
            {
                ReceiveCallBack(copy);
            }
            //stringData = Encoding.UTF8.GetString(readData, 0, recv);
        }
        StartReceive();
    }

    //发送消息
    public bool SendMessage(byte[] bytes)
    {
        try
        {
            //byte[] bytes = Encoding.UTF8.GetBytes(str + "&");
            if (!isConnect)
                return false;
            int n = mSocket.Send(bytes);
            if (n < 1)
            {
                isConnect = false;
                return false;
            }
        }
        catch (Exception e)
        {
            isConnect = false;
            Debug.LogError(e);
            ConnetResultCallBack(false);
            return false;
        }
        return true;
    }
}

