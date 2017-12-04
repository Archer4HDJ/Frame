using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class StateObject
{
    // Client socket.     
    public Socket workSocket = null;
    // Size of receive buffer.     
    public const int BufferSize = 1024;
    // Receive buffer.     
    public byte[] buffer = new byte[BufferSize];
    // Received data string.     
    public Queue<byte[]> receiveData = new Queue<byte[]>();

    public StateObject(Socket workSocket)
    {
        this.workSocket = workSocket;
    }
}
public class ServerManager  {


        // 创建一个和客户端通信的套接字
        static Socket socketwatch = null;
        //定义一个集合，存储客户端信息
        static Dictionary<string, StateObject> clientConnectionItems = new Dictionary<string, StateObject> ();
    static Thread threadwatch;
        public static void StartListen(int port)
        {
            //定义一个套接字用于监听客户端发来的消息，包含三个参数（IP4寻址协议，流式连接，Tcp协议）  
            socketwatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //服务端发送信息需要一个IP地址和端口号  
            IPAddress address = IPAddress.Parse("127.0.0.1");
            //将IP地址和端口号绑定到网络节点point上  
            IPEndPoint point = new IPEndPoint(address, port);
        //此端口专门用来监听的  
        Debug.Log("开启监听。。。:"+ point.ToString());
        //监听绑定的网络节点  
        socketwatch.Bind(point);

            //将套接字的监听队列长度限制为20  
            socketwatch.Listen(20);

            //负责监听客户端的线程:创建一个监听线程  
             threadwatch = new Thread(watchconnecting);

            //将窗体线程设置为与后台同步，随着主线程结束而结束  
            threadwatch.IsBackground = true;

            //启动线程     
            threadwatch.Start();
        }
    public static void Close()
    {
        if (socketwatch != null)
        {
            socketwatch.Close();
            threadwatch.Abort();
            socketwatch = null;
            threadwatch = null;
            clientConnectionItems.Clear();
        }
    }

        //监听客户端发来的请求  
        static void watchconnecting()
        {
            Socket connection = null;

            //持续不断监听客户端发来的请求     
            while (true)
            {
                try
                {
                    connection = socketwatch.Accept();
                }
                catch (Exception ex)
                {
                //提示套接字监听异常     
                Debug.LogError(ex);
                    break;
                }

                //获取客户端的IP和端口号  
                IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

                //让客户显示"连接成功的"的信息  
                string sendmsg = "连接服务端成功！\r\n" + "本地IP:" + clientIP + "，本地端口" + clientPort.ToString();
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                connection.Send(arrSendMsg);

                //客户端网络结点号  
                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                //显示与客户端连接情况
               Debug.Log("成功与" + remoteEndPoint + "客户端建立连接！\t\n");
                //添加客户端信息  
                clientConnectionItems.Add(remoteEndPoint,new StateObject( connection));

                //创建一个通信线程      
                ParameterizedThreadStart pts = new ParameterizedThreadStart(recv);
                Thread thread = new Thread(pts);
                //设置为后台线程，随着主线程退出而退出 
                thread.IsBackground = true;
                //启动线程     
                thread.Start(connection);
            }
        }

        /// <summary>
        /// 接收客户端发来的信息，客户端套接字对象
        /// </summary>
        /// <param name="socketclientpara"></param>    
        static void recv(object socketclientpara)
        {
            Socket socketServer = socketclientpara as Socket;

            while (true)
            {
            StateObject so = clientConnectionItems[socketServer.RemoteEndPoint.ToString()];
                //创建一个内存缓冲区，其大小为1024*1024字节  即1M     
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                //将接收到的信息存入到内存缓冲区，并返回其字节数组的长度    
                try
                {
                    socketServer.Receive(so.buffer);
                so.receiveData.Enqueue(so.buffer);
              
                }
                catch (Exception ex)
                {
                    clientConnectionItems.Remove(socketServer.RemoteEndPoint.ToString());
                    //提示套接字监听异常  
                    Debug.Log("客户端" + socketServer.RemoteEndPoint + "已经中断连接" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
                    //关闭之前accept出来的和客户端进行通信的套接字 
                    socketServer.Close();
                    break;
                }
            }
        }


}
