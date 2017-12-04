using HDJ.Framework.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetWorkManager  {
    public  CallBack<byte,byte[]> NetReceiveMessageCallBack;
    public  CallBack ConnectSuccess;

    private  bool isOpenNetWork = true;

    public  bool IsOpenNetWork
    {
        get
        {
            return isOpenNetWork;
        }

        set
        {
            isOpenNetWork = value;

            if (!isOpenNetWork)
            {
                netTool.Close();
            }
        }
    }

    public  bool IsConnected
    {
        get
        {
            return netTool.isConnect;
        }
    }
    private NetWorkTool netTool;

    // Use this for initialization
    public NetWorkManager(string ipAdress, int port)
    {
        netTool = new NetWorkTool(ipAdress, port);
        netTool.ConnetResultCallBack = ConnetResultCallBack;
        netTool.ReceiveCallBack = ReceiveMessage;
        Debug.Log("NetWorkManager");
        //netTool.Connect();
    }
    const float connectTime = 1.5f;
     bool isConnecting = false;

     float tempTime=0f;
	// Update is called once per frame
	public  void Update () {
        if (!IsOpenNetWork) return;

        if (isConnecting) return;
        if (IsConnected) return;
        if (tempTime <= 0)
        {
            tempTime = connectTime;
            netTool.Connect();
            isConnecting = true;
        }
        else
        {
            tempTime -= Time.unscaledDeltaTime;
        }

    }


    private  void ConnetResultCallBack(bool res)
    {
        isConnecting = false;
        if (res)
        {
            Debug.Log("连接成功");
            if (ConnectSuccess != null)
                ConnectSuccess();
        }
        else
        {
            Debug.Log("连接失败");
        }
    }

    public bool SendMessage<T>(byte opCode, T content)
    {
        bool res = false;

        byte[] bs = ProtoBufUtils.Serialize(content);
        byte[] buff = new byte[bs.Length + 1];
        buff[0] = opCode;
        for (int i = 0; i < bs.Length; i++)
        {
            buff[i + 1] = bs[i];
        }
        netTool.SendMessage(buff);

        return res;
    }

    private  void ReceiveMessage(byte[] mes)
    {
        byte opCode = mes[0];
        byte[] buff = new byte[mes.Length - 1];
        for (int i = 0; i < buff.Length; i++)
        {
            buff[i] = mes[i + 1];
        }
        if (NetReceiveMessageCallBack != null)
        {
            NetReceiveMessageCallBack(opCode, buff);
        }
    }
}
