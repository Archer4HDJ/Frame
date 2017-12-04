using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;

namespace HDJ.Framework.Tools
{
    public class FTPTool
    {
        #region 变量属性
        /// <summary>
        /// Ftp服务器ip
        /// </summary>
        public  string ftpServerIP = string.Empty;
        /// <summary>
        /// Ftp 指定用户名
        /// </summary>
        public  string ftpUserID = string.Empty;
        /// <summary>
        /// Ftp 指定用户密码
        /// </summary>
        public  string ftpPassword = string.Empty;

        #endregion
       public FTPTool(string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            this.ftpServerIP = ftpServerIP;
            this.ftpUserID = ftpUserID;
            this.ftpPassword = ftpPassword;
            
        }


        #region 从FTP服务器下载文件，指定本地路径和本地文件名
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="localPath">保存文件的目录</param>
        /// <param name="remotePath">ftp服务器上的要下载的文件路径，如："NewFolder/11/22/33/TestScrptObj.cs"</param>
        /// <param name="updateProgress">下载进度<总长度,当前下载></param>
        public bool DownLoadFile(string localPath,string remotePath, Action<int, int> updateProgress = null)
        {
            string uri = "ftp://" + ftpServerIP + "/" + remotePath;
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.Timeout = 4;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);//用户，密码
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;//向服务器发出下载请求命令
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                long cl = response.ContentLength;
                int bufferSize = 2048;
                int readCount;
                byte[] buffer = new byte[bufferSize];

                readCount = ftpStream.Read(buffer, 0, bufferSize);
                if (localPath.EndsWith("/"))
                    localPath.Remove(localPath.Length - 1);
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
               
                string path= localPath+"/"+   Path.GetFileName(remotePath);
                if (File.Exists(path))
                    File.Delete(path);

                    FileStream outputStream = new FileStream(path, FileMode.OpenOrCreate);

                //更新进度  
                if (updateProgress != null)
                {
                    updateProgress((int)ftpStream.Length, 0);//更新进度条   
                }

                int count = 0;
                while (readCount > 0)
                {
                    count += readCount;
                    outputStream.Write(buffer, 0, readCount);
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    //更新进度  
                    if (updateProgress != null)
                    {
                        updateProgress((int)ftpStream.Length, count);//更新进度条   
                    }
                }

                ftpStream.Close();
                outputStream.Close();
                response.Close();

                return true;
            }
            catch(Exception e)
            {
                Debug.Log("下载失败：" + uri);
                Debug.Log(e);
                return false;
            }
        }
        #endregion

        #region 上传文件到FTP服务器
        /// <summary>
        /// 上传文件到FTP服务器
        /// </summary>
        /// <param name="localFullPath">本地带有完整路径的文件名</param>
        /// <param name="updateProgress">报告进度的处理(第一个参数：总大小，第二个参数：当前进度)</param>
        /// <returns>是否下载成功</returns>
        public  bool FtpUploadFile(string localFullPathName,string remoteDirectory =null, Action<int, int> updateProgress = null)
        {
            FtpWebRequest reqFTP;
            Stream stream = null;
       
            FileStream fs = null;
            try
            {
                FileInfo finfo = new FileInfo(localFullPathName);
                if (ftpServerIP == null || ftpServerIP.Trim().Length == 0)
                {
                    throw new Exception("ftp上传目标服务器地址未设置！");
                }

                string rPath;
                if (!string.IsNullOrEmpty(remoteDirectory))
                {

                    if (remoteDirectory.EndsWith("/"))
                        remoteDirectory = remoteDirectory.Remove(remoteDirectory.Length - 1);

                    string[] tempPaths = remoteDirectory.Split(new char[] { '/'});

                    string temp ="";
                    for (int i = 0; i < tempPaths.Length; i++)
                    {
                        if (i != 0)
                            temp += "/";
                        temp += tempPaths[i];
                        Debug.Log("Temp :" + temp);
                     
                        {
                            MakeDir(temp);
                        }
                    }

                   

                    rPath = "ftp://" + ftpServerIP + "/" + remoteDirectory + "/" + finfo.Name;
                }
                else
                {
                    rPath = "ftp://" + ftpServerIP  + "/" + finfo.Name;
                }

                Uri uri = new Uri(rPath);
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.Timeout = 4;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);//用户，密码
                reqFTP.Method = WebRequestMethods.Ftp.UploadFile;//向服务器发出下载请求命令
                reqFTP.ContentLength = finfo.Length;//为request指定上传文件的大小
               // response = reqFTP.GetResponse() as FtpWebResponse;
                reqFTP.ContentLength = finfo.Length;
                int buffLength = 1024;
                byte[] buff = new byte[buffLength];
                int contentLen;
                fs = finfo.OpenRead();
                stream = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                int allbye = (int)finfo.Length;
                //更新进度  
                if (updateProgress != null)
                {
                    updateProgress((int)allbye, 0);//更新进度条   
                }
                int startbye = 0;
                while (contentLen != 0)
                {
                    startbye = contentLen + startbye;
                    stream.Write(buff, 0, contentLen);
                    //更新进度  
                    if (updateProgress != null)
                    {
                        updateProgress((int)allbye, (int)startbye);//更新进度条   
                    }
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                stream.Close();
                fs.Close();
                Debug.Log("上传成功： " + rPath);
                return true;

        }
            catch (Exception e)
            {
                Debug.LogError(e);
                return false;
                throw;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

       

        /// <summary>
        /// 去除空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private  string RemoveSpaces(string str)
        {
            string a = "";
            CharEnumerator CEnumerator = str.GetEnumerator();
            while (CEnumerator.MoveNext())
            {
                byte[] array = new byte[1];
                array = System.Text.Encoding.ASCII.GetBytes(CEnumerator.Current.ToString());
                int asciicode = (short)(array[0]);
                if (asciicode != 32)
                {
                    a += CEnumerator.Current.ToString();
                }
            }
            string sdate = System.DateTime.Now.Year.ToString() + System.DateTime.Now.Month.ToString() + System.DateTime.Now.Day.ToString() + System.DateTime.Now.Hour.ToString()
                + System.DateTime.Now.Minute.ToString() + System.DateTime.Now.Second.ToString() + System.DateTime.Now.Millisecond.ToString();
            return a.Split('.')[a.Split('.').Length - 2] + "." + a.Split('.')[a.Split('.').Length - 1];
        }
        /// <summary>
        /// 获取已上传文件大小
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="path">服务器文件路径</param>
        /// <returns></returns>
        public  long GetFileSize(string filename, string remoteFilepath)
        {
            long filesize = 0;
            try
            {
                FtpWebRequest reqFTP;
                FileInfo fi = new FileInfo(filename);
                string uri;
                if (remoteFilepath.Length == 0)
                {
                    uri = "ftp://" + ftpServerIP + "/" + fi.Name;
                }
                else
                {
                    uri = "ftp://" + ftpServerIP + "/" + remoteFilepath + "/" + fi.Name;
                }
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.KeepAlive = false;
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);//用户，密码
                reqFTP.Method = WebRequestMethods.Ftp.GetFileSize;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                filesize = response.ContentLength;
                return filesize;
            }
            catch
            {
                return 0;
            }
        }


        #endregion

        #region 上传文件夹  

        ///// <summary>  
        ///// 上传整个目录  
        ///// </summary>  
        ///// <param name="localDir">要上传的目录的上一级目录</param>  
        ///// <param name="ftpPath">FTP路径</param>  
        ///// <param name="dirName">要上传的目录名</param>  
        ///// <param name="ftpUser">FTP用户名（匿名为空）</param>  
        ///// <param name="ftpPassword">FTP登录密码（匿名为空）</param>  
        //public void UploadDirectory(string localDir, string ftpPath, string dirName)
        //{
        //    string dir = localDir + dirName + @"\"; //获取当前目录（父目录在目录名）  
        //    //检测本地目录是否存在  
        //    if (!Directory.Exists(dir))
        //    {
        //         Debug.LogError("本地目录：“" + dir + "” 不存在！<br/>");
        //        return;
        //    }
        //    //检测FTP的目录路径是否存在  
        //    if (!CheckDirectoryExist(ftpPath, dirName))
        //    {
        //        MakeDir(ftpPath, dirName);//不存在，则创建此文件夹  
        //    }
        //    List<List<string>> infos = GetDirDetails(dir); //获取当前目录下的所有文件和文件夹  

        //    //先上传文件  
        //    //Response.Write(dir + "下的文件数：" + infos[0].Count.ToString() + "<br/>");  
        //    for (int i = 0; i < infos[0].Count; i++)
        //    {
        //        Console.WriteLine(infos[0][i]);
        //        UpLoadFile(dir + infos[0][i], ftpPath + dirName + @"/" + infos[0][i]);
        //    }
        //    //再处理文件夹  
        //    //Response.Write(dir + "下的目录数：" + infos[1].Count.ToString() + "<br/>");  
        //    for (int i = 0; i < infos[1].Count; i++)
        //    {
        //        UploadDirectory(dir, ftpPath + dirName + @"/", infos[1][i]);
        //        //Response.Write("文件夹【" + dirName + "】上传成功！<br/>");  
        //    }
        //}




        /// <summary>  
        /// 创建文件夹    
        /// </summary>    
        /// <param name="ftpPath">FTP路径</param>    
        /// <param name="dirName">创建文件夹名称</param>    
        public void MakeDir(string remoteDirPath)
        {

            FtpWebRequest reqFTP;
            try
            {
                string uri = "ftp://" + ftpServerIP + "/" + remoteDirPath;

                reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);
                reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                reqFTP.UseBinary = true;
                reqFTP.Timeout = 4;
                reqFTP.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                ftpStream.Close();
                response.Close();
                Debug.Log("文件夹【" + uri + "】创建成功！<br/>");

            }

            catch (Exception ex)
            {
                //Debug.LogError("新建文件夹【" + remoteDirPath + "】时，发生错误：" + ex.Message);
            }
        }
#endregion
    }
}
