﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace COG
{
    public partial class Main
    {
        private static FTPManager ftpManager = new FTPManager();

        public static bool FTPConnectToServer(string ip, string port, string userId, string pwd)
        {
            return ftpManager.ConnectToServer(ip, port, userId, pwd);
        }

        public static bool UpLoad(string folder, string filename)
        {
            return ftpManager.UpLoad(folder, filename);
        }
    }


    class FTPManager
    {
        public delegate void ExceptionEventHandler(string LocationID, Exception ex);
        public event ExceptionEventHandler ExceptionEvent;

        public Exception LastException = null;

        public bool IsConnected { get; set; }

        private string ipAddr = string.Empty;
        private string port = string.Empty;
        private string userId = string.Empty;
        private string pwd = string.Empty;

        public FTPManager()
        {

        }

        public bool ConnectToServer(string ip, string port, string userId, string pwd)
        {
            this.IsConnected = false;

            this.ipAddr = ip;
            this.port = port;
            this.userId = userId;
            this.pwd = pwd;

            string url = string.Format(@"FTP://{0}:{1}", this.ipAddr, this.port);

            try
            {
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
                ftpRequest.Credentials = new NetworkCredential(userId, pwd);
                ftpRequest.KeepAlive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                ftpRequest.UsePassive = false;

                using (ftpRequest.GetResponse())
                {

                }

                this.IsConnected = true;
            }
            catch(Exception ex)
            {
                this.LastException = ex;

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return false;
            }

            return true;
        } // ConnectToServer

        public bool UpLoad(string folder, string filename)
        {
            return upload(folder, filename);
        }
        
        private bool upload(string folder, string filename)
        {
            try
            {
                //makeDir(folder);

                FileInfo fileInf = new FileInfo(filename);

                folder = folder.Replace('\\', '/');
                filename = filename.Replace('\\', '/');

                string url = string.Format(@"FTP://{0}:{1}/{2}/{3}", this.ipAddr, this.port, folder, fileInf.Name);
                FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
                ftpRequest.Credentials = new NetworkCredential(userId, pwd);
                ftpRequest.KeepAlive = false;
                ftpRequest.UseBinary = false;
                ftpRequest.UsePassive = false;

                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                ftpRequest.ContentLength = fileInf.Length;

                int buffLength = 2048;
                byte[] buff = new byte[buffLength];
                int contentLen;

                using (FileStream fs = fileInf.OpenRead())
                {
                    using (Stream strm = ftpRequest.GetRequestStream())
                    {
                        contentLen = fs.Read(buff, 0, buffLength);

                        while (contentLen != 0)
                        {
                            strm.Write(buff, 0, contentLen);
                            contentLen = fs.Read(buff, 0, buffLength);
                        }
                    }

                    fs.Flush();
                    fs.Close();
                }

                if (buff != null)
                {
                    Array.Clear(buff, 0, buff.Length);
                    buff = null;
                }
            }
            catch(Exception ex)
            {
                this.LastException = ex;

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);

                return false;
            }

            return true;
        } // upload

        private void makeDir(string dirName)
        {
            string[] arrDir = dirName.Split('\\');
            string currentDir = string.Empty;

            try
            {
                foreach(string tmpFolder in arrDir)
                {
                    try
                    {
                        if (tmpFolder == string.Empty) continue;

                        currentDir += @"/" + tmpFolder;

                        string url = string.Format(@"FTP://{0}:{1}{2}", this.ipAddr, this.port, currentDir);
                        FtpWebRequest ftpRequest = (FtpWebRequest)WebRequest.Create(url);
                        ftpRequest.Credentials = new NetworkCredential(userId, pwd);

                        ftpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                        ftpRequest.KeepAlive = false;
                        ftpRequest.UsePassive = false;

                        FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                        response.Close();
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                this.LastException = ex;

                System.Reflection.MemberInfo info = System.Reflection.MethodInfo.GetCurrentMethod();
                string id = string.Format("{0}.{1}", info.ReflectedType.Name, info.Name);

                if (this.ExceptionEvent != null)
                    this.ExceptionEvent(id, ex);
            }
        }   // makeDir

        private void checkDir(string localFullPathFile)
        {
            FileInfo fInfo = new FileInfo(localFullPathFile);

            if (!fInfo.Exists)
            {
                DirectoryInfo dInfo = new DirectoryInfo(fInfo.DirectoryName);
                if (!dInfo.Exists)
                {
                    dInfo.Create();
                }
            }
        }
    }
}
