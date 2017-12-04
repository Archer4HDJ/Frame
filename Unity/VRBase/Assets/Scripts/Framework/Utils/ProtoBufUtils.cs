using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace HDJ.Framework.Utils
{
    public static class ProtoBufUtils
    {
        public static byte[] Serialize<T>(T t)
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中
                using (MemoryStream ms = new MemoryStream())
                {
                    //使用ProtoBuf工具的序列化方法
                    ProtoBuf.Serializer.Serialize<T>(ms, t);
                    //定义二级制数组，保存序列化后的结果
                    byte[] result = new byte[ms.Length];
                    //将流的位置设为0，起始点
                    ms.Position = 0;
                    //将流中的内容读取到二进制数组中
                    ms.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("序列化失败: " + ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// 将收到的消息反序列化成对象
        /// </summary>
        /// <returns>The serialize.</returns>
        /// <param name="msg">收到的消息.</param>
        public static T DeSerialize<T>(byte[] msg)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中
                    ms.Write(msg, 0, msg.Length);
                    //将流的位置归0
                    ms.Position = 0;
                    //使用工具反序列化对象
                    T result = ProtoBuf.Serializer.Deserialize<T>(ms);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.Log("反序列化失败: " + ex.ToString());
                return default(T);
            }
        }
    }
   
}


