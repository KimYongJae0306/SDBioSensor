using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using AsyncSocket;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Sockets;

namespace COG
{
    public partial class Main
    {
        private static ePLCControl MCClient_READ = new ePLCControl();
        private static ePLCControl MCClient_WRITE = new ePLCControl();
        private static ePLCControl.DeviceName deviceName = ePLCControl.DeviceName.R;
        public static DateTime m_dtLastWriteTime;

        public static int PLC_TimeOut
        {
            get
            {
                return MCClient_READ.TimeOut;
            }
            set
            {
                if(value < 500)
                {
                    MCClient_READ.TimeOut  = 500;
                    MCClient_WRITE.TimeOut = 500;
                }
                else
                {
                    MCClient_READ.TimeOut  = value;
                    MCClient_WRITE.TimeOut = value;
                }
            }
        }

        public static bool OpenDM(int _intReadLocalPort, int _intReadRemotePort, string _strRemoteIP, int _intReadRecTimeOut, int _intWriteLocalPort, int _intWriteRemotePort)
        {
            bool nRet = true;

            if (!Main.DEFINE.OPEN_F && !Main.DEFINE.OPEN_CAM)
            {
                try
                {
                    MCClient_READ.SetPLCProperties(_strRemoteIP, _intReadLocalPort, _intReadRecTimeOut);
                    int connecting = MCClient_READ.Open();
                    if (connecting != 0)
                    {
                        nRet = false;
                        MessageBox.Show("READ PORT OPEN ERROR:" + _intReadLocalPort.ToString());
                    }

                    MCClient_WRITE.SetPLCProperties(_strRemoteIP, _intWriteLocalPort, _intReadRecTimeOut);
                    connecting = MCClient_WRITE.Open();
                    if (connecting != 0)
                    {
                        nRet = false;
                        MessageBox.Show("WRITE PORT OPEN ERROR:" + _intWriteLocalPort.ToString());
                    }
                    m_dtLastWriteTime = DateTime.Now;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("PLC OPEN ERROR " + ex.ToString());
                    nRet = false;
                }
            }
            return nRet;
        }

        public static void CloseDM()
        {
            if (!Main.DEFINE.OPEN_F && !Main.DEFINE.OPEN_CAM)
            {
                try
                {
                    MCClient_READ.Close();
                    MCClient_WRITE.Close();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("PLC CLOSE ERROR " + ex.ToString());
                }
            }
        }

        public static void ReadDeviceBlock(string szDevice, int lSize, out int[] lplData)
        {
            int[] returnValue = new int[lSize];
            try
            {
                returnValue = MCClient_READ.ReadDeviceBlock(ePLCControl.SubCommand.Word, deviceName, PLCDataTag.BASE_RW_ADDR.ToString(), lSize);
            }
            catch (System.Exception ex)
            {
               MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                MCClient_READ.WriteLogFile("PLC READ DISCONNECT");
            }
            finally
            {
                lplData = returnValue;
            }
        }

        public static void ThreadPLC_Read()
        {
            if (!Main.DEFINE.OPEN_F && !Main.DEFINE.OPEN_CAM)
            {
                while (threadFlag)
                {
                    string DeviceName;
                    DeviceName = "D" + Convert.ToString(PLCDataTag.BASE_RW_ADDR).ToUpper();

                    ReadDeviceBlock(DeviceName, PLCDataTag.ReadSize, out PLCDataTag.BData);
                    try
                    {
                        if (PLCDataTag.BData.Length == PLCDataTag.ReadSize)
                        {
                            for (int i = 0; i < PLCDataTag.ReadSize; i++)
                                PLCDataTag.RData[i] = (Int16)PLCDataTag.BData[i];
                        }
                        else
                        {

                        }
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show("PLC READ DISCONNECT" + ex.Source + ex.Message + ex.StackTrace);
                    }
                    Thread.Sleep(50);
                }
            }
        }


        public static void WriteDevice(int szDevice, int lplData)
        {
            if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM)
                return;

            try
            {

                int[] Data = new int[1];
                Data[0] = lplData;
                MCClient_WRITE.WriteDeviceBlock(ePLCControl.SubCommand.Word, deviceName, szDevice.ToString(), Data);
                m_dtLastWriteTime = DateTime.Now;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
            }
            finally
            {
                //Thread.Sleep(50);
            }
        }

        public static void WriteDeviceBlock(int szDevice, int lSize, ref int[] lplData)
        {
            if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM)
                return;

            try
            {
                MCClient_WRITE.WriteDeviceBlock(ePLCControl.SubCommand.Word, deviceName, szDevice.ToString(), lplData);
                m_dtLastWriteTime = DateTime.Now;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
            }
            finally
            {
            }
        }


        public static void TwoWordtoDouble(int _highWord, int _lowWord, ref double _dbval)
        {
            int num = 0;
            num = (_highWord << 0x10) | _lowWord;
            _dbval = ((double)num) / 1000.0;
        }

        public static void WriteDeviceRandom_W(string[] szDevice, ref int[] lplData)
        {
            if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM)
                return;
            try
            {
                ePLCControl.DeviceName[] _DeviceName = new ePLCControl.DeviceName[szDevice.Length];
                for (int i = 0; i < szDevice.Length; i++)
                {
                    _DeviceName[i] = deviceName;
                }
                m_dtLastWriteTime = DateTime.Now;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
            }
            finally
            {
            }
        }
    }


    public class ePLCControl
    {
        private string sLogPath = "D:\\ePLCLog\\";
        private string IP = "";
        private int Port = 5002;
        private int NetworkNO = 0;
        private int PLCStationNO = 255;
        private int PCStationNO = 00;
        public int TimeOut = 3000;
        private byte[] ReceivedData = new byte[0];
        private byte[] SendBytes_BlockRead = new byte[21]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 12,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 1,
      (byte) 20,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 168,
      (byte) 192,
      (byte) 3
    };
        private byte[] SendBytes_BlockWrite = new byte[21]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 12,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 1,
      (byte) 20,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 168,
      (byte) 192,
      (byte) 3
    };
        private byte[] SendBytes_RandomRead = new byte[21]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 12,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 3,
      (byte) 4,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 168
    };
        private byte[] SendBytes_RandomWrite = new byte[23]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 16,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 2,
      (byte) 20,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 168,
      (byte) 0,
      (byte) 0
    };
        private byte[] SendBytes_MultiBlockRead = new byte[23]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 14,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 6,
      (byte) 4,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 168,
      (byte) 1,
      (byte) 0
    };
        private byte[] SendBytes_MultiBlockWrite = new byte[25]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 16,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 6,
      (byte) 20,
      (byte) 0,
      (byte) 0,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 168,
      (byte) 1,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };
        private byte[] SendBytes_ErrorLEDOff = new byte[15]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 6,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 23,
      (byte) 22,
      (byte) 0,
      (byte) 0
    };
        private byte[] SendBytes_Diagnostic = new byte[22]
    {
      (byte) 80,
      (byte) 0,
      (byte) 1,
      (byte) 1,
      byte.MaxValue,
      (byte) 3,
      (byte) 2,
      (byte) 16,
      (byte) 0,
      (byte) 16,
      (byte) 0,
      (byte) 25,
      (byte) 6,
      (byte) 0,
      (byte) 0,
      (byte) 5,
      (byte) 0,
      (byte) 65,
      (byte) 66,
      (byte) 67,
      (byte) 68,
      (byte) 69
    };
        private static int m_ID;
        private int m_SocketID;
        private static bool bLicensed;
        private AsyncSocketClient client;
        public bool bLogEnable;
        public bool IsConnected;
        private bool bReceived;

        public event EventHandler evhDisconnected;

        public ePLCControl()
        {
            this.m_SocketID = ePLCControl.m_ID;
            ++ePLCControl.m_ID;
            Directory.CreateDirectory(this.sLogPath);
        }

        public void WriteLogFile(string sData)
        {
            StreamWriter streamWriter = new StreamWriter(this.sLogPath + DateTime.Now.ToString("yyyyMMddHH") + ".txt", true);
            streamWriter.WriteLine(DateTime.Now.ToString() + "    " + sData);
            streamWriter.Close();
        }

        public void SetPLCProperties(
            string _IP,
            int _Port,
            int _TimeOut)
        {
            this.IP = _IP;
            this.Port = _Port;
            this.TimeOut = _TimeOut;
            this.SendBytes_BlockRead[2] = (byte)this.NetworkNO;
            this.SendBytes_BlockRead[3] = (byte)this.PLCStationNO;
            this.SendBytes_BlockRead[6] = (byte)this.PCStationNO;
            this.SendBytes_BlockWrite[2] = (byte)this.NetworkNO;
            this.SendBytes_BlockWrite[3] = (byte)this.PLCStationNO;
            this.SendBytes_BlockWrite[6] = (byte)this.PCStationNO;
            this.SendBytes_RandomRead[2] = (byte)this.NetworkNO;
            this.SendBytes_RandomRead[3] = (byte)this.PLCStationNO;
            this.SendBytes_RandomRead[6] = (byte)this.PCStationNO;
            this.SendBytes_RandomWrite[2] = (byte)this.NetworkNO;
            this.SendBytes_RandomWrite[3] = (byte)this.PLCStationNO;
            this.SendBytes_RandomWrite[6] = (byte)this.PCStationNO;
            this.SendBytes_MultiBlockRead[2] = (byte)this.NetworkNO;
            this.SendBytes_MultiBlockRead[3] = (byte)this.PLCStationNO;
            this.SendBytes_MultiBlockRead[6] = (byte)this.PCStationNO;
            this.SendBytes_MultiBlockWrite[2] = (byte)this.NetworkNO;
            this.SendBytes_MultiBlockWrite[3] = (byte)this.PLCStationNO;
            this.SendBytes_MultiBlockWrite[6] = (byte)this.PCStationNO;
        }

        public int Open()
        {
            int num1 = 0;
            if (this.IsConnected)
                return num1;
            this.client = new AsyncSocketClient(this.m_SocketID++);
            this.client.OnConnet -= new AsyncSocketConnectEventHandler(this.client_OnConnet);
            this.client.OnConnet += new AsyncSocketConnectEventHandler(this.client_OnConnet);
            this.client.Connect(this.IP, this.Port);
            int tickCount = Environment.TickCount;
            int num2;
            while (!this.IsConnected)
            {
                if (Environment.TickCount - tickCount > this.TimeOut)
                {
                    num2 = 3;
                    goto label_7;
                }
            }
            num2 = 0;
            label_7:
            return num2;
        }

        private void client_OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            this.client.OnReceive += new AsyncSocketReceiveEventHandler(this.client_OnReceive);
            this.client.OnError += new AsyncSocketErrorEventHandler(this.client_OnError);
            this.client.OnClose += new AsyncSocketCloseEventHandler(this.client_OnClose);
            this.client.Receive();
            this.IsConnected = true;
        }

        private void client_OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
        }

        private void client_OnError(object sender, AsyncSocketErrorEventArgs e)
        {
        }

        public void Close()
        {
            if (this.client != null)
                this.client.Close();
            this.IsConnected = false;
        }

        public void SetTimeOut(int _TimeOut)
        {
            this.TimeOut = _TimeOut;
        }

        private void client_OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            try
            {
                this.ReceivedData = new byte[e.ReceiveBytes - 11];
                Array.Copy((Array)e.ReceiveData, 11, (Array)this.ReceivedData, 0, e.ReceiveBytes - 11);
            }
            catch
            {
            }
            this.bReceived = true;
        }

        private void PLCCommunicationReset()
        {
            this.client.Send(this.SendBytes_ErrorLEDOff);
            this.bReceived = false;
            int tickCount = Environment.TickCount;
            while (!this.bReceived)
            {
                if (Environment.TickCount - tickCount > this.TimeOut)
                    return;
            }
            this.bReceived = false;
        }

        private bool IsConnected1()
        {
            try
            {
                return !(client.Connection.Poll(1, SelectMode.SelectRead) && client.Connection.Available == 0);
            }
            catch (SocketException) { return false; }
        }
        public bool IsConnected2()
        {
         return   client.Connection.Connected;
            bool flag = true;
            Socket s = client.Connection;
             if (s.Connected)
             {
                 try
                 {
                     if ((uint)new Ping().Send(((System.Net.IPEndPoint)s.RemoteEndPoint).Address).Status > 0U)
                         flag = false;
                 }
                 catch (PingException ex)
                 {
                     flag = false;
                 }
                 if (s.Poll(5000, SelectMode.SelectRead) && s.Available == 0)
                     flag = false;
             }
             else
                 flag = false;
          //      flag = s.IsBound;

            return flag;
        }

        public int[] ReadDeviceBlock(
          ePLCControl.SubCommand _Unit,
          ePLCControl.DeviceName _DeviceName,
          string _StartAddress,
          int _Length)
        {
            if (!IsConnected2())
            {
                this.WriteLogFile("Read DisConnect Port:" + Port.ToString());
                this.client.Close();

                if (Open() == 0)
                {
                    this.WriteLogFile("ReConnect OK");
                }
                else
                {
                    this.WriteLogFile("ReConnect NG");
                }
            }
            int[] array = new int[0];
            if (!this.IsConnected)
                return array;
            if (_Unit == ePLCControl.SubCommand.Bit)
            {
                if (_DeviceName == ePLCControl.DeviceName.D || _DeviceName == ePLCControl.DeviceName.R || (_DeviceName == ePLCControl.DeviceName.W || _DeviceName == ePLCControl.DeviceName.ZR))
                    throw new Exception("Word Device는 Bit단위 블락으로 읽을 수 없습니다.\r\n _Unit 또는 _DeviceName을 바꿔주시기 바랍니다.");
                if (_Length % 2 == 1)
                    throw new Exception("Bit Device의_Length는 항상짝수여야 합니다");
            }
            if (_Unit == ePLCControl.SubCommand.Bit)
            {
                this.SendBytes_BlockRead[13] = (byte)1;
                this.SendBytes_BlockRead[14] = (byte)0;
            }
            else if (_Unit == ePLCControl.SubCommand.Word)
            {
                this.SendBytes_BlockRead[13] = (byte)0;
                this.SendBytes_BlockRead[14] = (byte)0;
            }
            this.SendBytes_BlockRead[7] = (byte)12;
            this.SendBytes_BlockRead[8] = (byte)0;
            this.SendBytes_BlockRead[11] = (byte)1;
            this.SendBytes_BlockRead[12] = (byte)4;
            this.SendBytes_BlockRead[18] = (byte)_DeviceName;
            int num1 = 0;
            if (_Unit == ePLCControl.SubCommand.Word)
            {
                num1 = _Length / 960 + 1;
                if (_Length % 960 == 0)
                    --num1;
            }
            else if (_Unit == ePLCControl.SubCommand.Bit)
            {
                num1 = _Length / 7168 + 1;
                if (_Length % 7168 == 0)
                    --num1;
            }
            int num2 = num1;
            int index1 = 0;
            for (int index2 = 0; index2 < num2; ++index2)
            {
                switch (_DeviceName)
                {
                    case ePLCControl.DeviceName.M:
                    case ePLCControl.DeviceName.L:
                    case ePLCControl.DeviceName.D:
                    case ePLCControl.DeviceName.R:
                    case ePLCControl.DeviceName.ZR:
                        int num3 = 0;
                        switch (_Unit)
                        {
                            case ePLCControl.SubCommand.Bit:
                                num3 = Convert.ToInt32(_StartAddress) + index2 * 7168;
                                break;
                            case ePLCControl.SubCommand.Word:
                                num3 = _DeviceName == ePLCControl.DeviceName.L || _DeviceName == ePLCControl.DeviceName.M ? Convert.ToInt32(_StartAddress) + index2 * 15360 : Convert.ToInt32(_StartAddress) + index2 * 960;
                                break;
                        }
                        this.SendBytes_BlockRead[15] = (byte)(num3 % 256);
                        this.SendBytes_BlockRead[16] = (byte)(num3 / 256);
                        this.SendBytes_BlockRead[17] = (byte)(num3 / 65536);
                        break;
                    case ePLCControl.DeviceName.X:
                    case ePLCControl.DeviceName.Y:
                    case ePLCControl.DeviceName.B:
                    case ePLCControl.DeviceName.W:
                        int num4 = 0;
                        switch (_Unit)
                        {
                            case ePLCControl.SubCommand.Bit:
                                num4 = int.Parse(_StartAddress, NumberStyles.HexNumber) + index2 * 7168;
                                break;
                            case ePLCControl.SubCommand.Word:
                                num4 = _DeviceName != ePLCControl.DeviceName.W ? int.Parse(_StartAddress, NumberStyles.HexNumber) + index2 * 15360 : int.Parse(_StartAddress, NumberStyles.HexNumber) + index2 * 960;
                                break;
                        }
                        this.SendBytes_BlockRead[15] = (byte)(num4 % 256);
                        this.SendBytes_BlockRead[16] = (byte)(num4 / 256);
                        this.SendBytes_BlockRead[17] = (byte)(num4 / 65536);
                        break;
                }
                if (num1 == 1)
                {
                    switch (_Unit)
                    {
                        case ePLCControl.SubCommand.Bit:
                            this.SendBytes_BlockRead[19] = (byte)((_Length - 7168 * index2) % 256);
                            this.SendBytes_BlockRead[20] = (byte)((_Length - 7168 * index2) / 256);
                            break;
                        case ePLCControl.SubCommand.Word:
                            this.SendBytes_BlockRead[19] = (byte)((_Length - 960 * index2) % 256);
                            this.SendBytes_BlockRead[20] = (byte)((_Length - 960 * index2) / 256);
                            break;
                    }
                }
                else
                {
                    switch (_Unit)
                    {
                        case ePLCControl.SubCommand.Bit:
                            this.SendBytes_BlockRead[19] = (byte)0;
                            this.SendBytes_BlockRead[20] = (byte)28;
                            break;
                        case ePLCControl.SubCommand.Word:
                            this.SendBytes_BlockRead[19] = (byte)192;
                            this.SendBytes_BlockRead[20] = (byte)3;
                            break;
                    }
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return array;
                this.bReceived = false;
                this.client.Send(this.SendBytes_BlockRead);
                int tickCount = Environment.TickCount;

                int RetryCount = 0;
                int seq = 0;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                    {
                       // if(RetryCount <= 0)
                       // {
                       //     this.WriteLogFile("PLC Read TimeOut Retry Send:" + this.TimeOut.ToString());
                       //     tickCount = Environment.TickCount;
                       //     this.client.Send(this.SendBytes_BlockRead);
                       // }
                       // else
                        {
                            this.WriteLogFile("PLC Read TimeOut LimitTime:" + this.TimeOut.ToString());
                            this.Close();
                            this.evhDisconnected((object)this, (EventArgs)null);
                            return array;
                        }
                    }
                }
                switch (_Unit)
                {
                    case ePLCControl.SubCommand.Bit:
                        int newSize1 = array.Length + this.ReceivedData.Length * 2;
                        if (num1 == 1 && _Length % 2 == 1)
                            --newSize1;
                        int length = array.Length;
                        Array.Resize<int>(ref array, newSize1);
                        for (int index3 = 0; index3 < this.ReceivedData.Length; ++index3)
                        {
                            array[length] = (int)this.ReceivedData[index3] / 16;
                            ++length;
                            if (array.Length != length)
                            {
                                array[length] = (int)this.ReceivedData[index3] % 16;
                                ++length;
                            }
                        }
                        break;
                    case ePLCControl.SubCommand.Word:
                        int newSize2 = array.Length + this.ReceivedData.Length / 2;
                        Array.Resize<int>(ref array, newSize2);
                        int num5 = this.ReceivedData.Length / 2;
                        for (int index3 = 0; index3 < num5; ++index3)
                        {
                            array[index1] = (int)this.ReceivedData[index3 * 2 + 1] * 256 + (int)this.ReceivedData[index3 * 2];
                            ++index1;
                        }
                        break;
                }
                --num1;
            }
            this.bReceived = false;
            return array;
        }

        public int WriteDeviceBlock(
          ePLCControl.SubCommand _Unit,
          ePLCControl.DeviceName _DeviceName,
          string _StartAddress,
          int[] _Data)
        {
            if (!IsConnected2())
            {
                this.WriteLogFile("Write DisConnect Port:" + Port.ToString());
                this.client.Close();

                if (Open() == 0)
                {
                    this.WriteLogFile("ReConnect OK");
                }
                else
                {
                    this.WriteLogFile("ReConnect NG");
                }
            }
            if (!this.IsConnected)
                return 1;
            if (_Unit == ePLCControl.SubCommand.Bit)
            {
                if (_DeviceName == ePLCControl.DeviceName.D || _DeviceName == ePLCControl.DeviceName.R || (_DeviceName == ePLCControl.DeviceName.W || _DeviceName == ePLCControl.DeviceName.ZR))
                    throw new Exception("Word Device는 Bit단위 블락으로 쓸 수 없습니다.\r\n _Unit 또는 _DeviceName을 바꿔주시기 바랍니다.");
                if (_Data.Length % 2 == 1)
                    throw new Exception("Bit Device의 _Data길이 는 항상짝수여야 합니다");
            }
            Array.Resize<byte>(ref this.SendBytes_BlockWrite, 21);
            if (_Unit == ePLCControl.SubCommand.Bit)
            {
                this.SendBytes_BlockWrite[13] = (byte)1;
                this.SendBytes_BlockWrite[14] = (byte)0;
            }
            else if (_Unit == ePLCControl.SubCommand.Word)
            {
                this.SendBytes_BlockWrite[13] = (byte)0;
                this.SendBytes_BlockWrite[14] = (byte)0;
            }
            this.SendBytes_BlockWrite[11] = (byte)1;
            this.SendBytes_BlockWrite[12] = (byte)20;
            this.SendBytes_BlockWrite[18] = (byte)_DeviceName;
            int num1 = 0;
            if (_Unit == ePLCControl.SubCommand.Word)
            {
                num1 = _Data.Length / 960 + 1;
                if (_Data.Length % 960 == 0)
                    --num1;
            }
            else if (_Unit == ePLCControl.SubCommand.Bit)
            {
                num1 = _Data.Length / 7168 + 1;
                if (_Data.Length % 7168 == 0)
                    --num1;
            }
            int num2 = num1;
            int index1 = 0;
            for (int index2 = 0; index2 < num2; ++index2)
            {
                switch (_DeviceName)
                {
                    case ePLCControl.DeviceName.M:
                    case ePLCControl.DeviceName.L:
                    case ePLCControl.DeviceName.D:
                    case ePLCControl.DeviceName.R:
                    case ePLCControl.DeviceName.ZR:
                        int num3 = 0;
                        switch (_Unit)
                        {
                            case ePLCControl.SubCommand.Bit:
                                num3 = Convert.ToInt32(_StartAddress) + index2 * 7168;
                                break;
                            case ePLCControl.SubCommand.Word:
                                num3 = _DeviceName == ePLCControl.DeviceName.M || _DeviceName == ePLCControl.DeviceName.L ? Convert.ToInt32(_StartAddress) + index2 * 15360 : Convert.ToInt32(_StartAddress) + index2 * 960;
                                break;
                        }
                        this.SendBytes_BlockWrite[15] = (byte)(num3 % 256);
                        this.SendBytes_BlockWrite[16] = (byte)(num3 / 256);
                        this.SendBytes_BlockWrite[17] = (byte)(num3 / 65536);

                        this.SendBytes_BlockWrite[15] = (byte)(num3 & 0xFF);
                        this.SendBytes_BlockWrite[16] = (byte)((num3 >> 8 ) & 0xFF);
                        this.SendBytes_BlockWrite[17] = (byte)((num3 >> 16) & 0xFF);


                        break;
                    case ePLCControl.DeviceName.X:
                    case ePLCControl.DeviceName.Y:
                    case ePLCControl.DeviceName.B:
                    case ePLCControl.DeviceName.W:
                        int num4 = 0;
                        switch (_Unit)
                        {
                            case ePLCControl.SubCommand.Bit:
                                num4 = int.Parse(_StartAddress, NumberStyles.HexNumber) + index2 * 7168;
                                break;
                            case ePLCControl.SubCommand.Word:
                                num4 = _DeviceName != ePLCControl.DeviceName.W ? int.Parse(_StartAddress, NumberStyles.HexNumber) + index2 * 15360 : int.Parse(_StartAddress, NumberStyles.HexNumber) + index2 * 960;
                                break;
                        }
                        this.SendBytes_BlockWrite[15] = (byte)(num4 % 256);
                        this.SendBytes_BlockWrite[16] = (byte)(num4 / 256);
                        this.SendBytes_BlockWrite[17] = (byte)(num4 / 65536);

                        this.SendBytes_BlockWrite[15] = (byte)(num4 & 0xFF);
                        this.SendBytes_BlockWrite[16] = (byte)((num4 >> 8) & 0xFF);
                        this.SendBytes_BlockWrite[17] = (byte)((num4 >> 16) & 0xFF);
                        break;
                }
                if (num1 == 1)
                {
                    switch (_Unit)
                    {
                        case ePLCControl.SubCommand.Bit:
                            this.SendBytes_BlockWrite[19] = (byte)((_Data.Length - 7168 * index2) % 256);
                            this.SendBytes_BlockWrite[20] = (byte)((_Data.Length - 7168 * index2) / 256);
                            break;
                        case ePLCControl.SubCommand.Word:
                            this.SendBytes_BlockWrite[19] = (byte)((_Data.Length - 960 * index2) % 256);
                            this.SendBytes_BlockWrite[20] = (byte)((_Data.Length - 960 * index2) / 256);
                            this.SendBytes_BlockWrite[19] = (byte)(_Data.Length & 0xFF);
                            this.SendBytes_BlockWrite[20] = (byte)((_Data.Length >> 8) & 0xFF); 
                            break;
                    }
                }
                else
                {
                    switch (_Unit)
                    {
                        case ePLCControl.SubCommand.Bit:
                            this.SendBytes_BlockWrite[19] = (byte)0;
                            this.SendBytes_BlockWrite[20] = (byte)28;
                            break;
                        case ePLCControl.SubCommand.Word:
                            this.SendBytes_BlockWrite[19] = (byte)192;
                            this.SendBytes_BlockWrite[20] = (byte)3;
                            break;
                    }
                }
                int num5 = 0;
                if (_Unit == ePLCControl.SubCommand.Word)
                {
                    if (num1 == 1)
                    {
                        num5 = 12 + (_Data.Length - index2 * 960) * 2;
                        //this.SendBytes_BlockWrite[7] = (byte)(num5 % 256);
                        //this.SendBytes_BlockWrite[8] = (byte)(num5 / 256);

                        this.SendBytes_BlockWrite[7] = (byte)(num5 & 0xFF);
                        this.SendBytes_BlockWrite[8] = (byte)((num5 >> 8) & 0xFF);
                    }
                    else
                    {
                        num5 = 1932;
                        this.SendBytes_BlockWrite[7] = (byte)(num5 % 256);
                        this.SendBytes_BlockWrite[8] = (byte)(num5 / 256);
                    }
                }
                else if (_Unit == ePLCControl.SubCommand.Bit)
                {
                    if (num1 == 1)
                    {
                        num5 = 12 + (_Data.Length - index2 * 7168) / 2;
                        this.SendBytes_BlockWrite[7] = (byte)(num5 % 256);
                        this.SendBytes_BlockWrite[8] = (byte)(num5 / 256);
                    }
                    else
                    {
                        num5 = 3596;
                        this.SendBytes_BlockWrite[7] = (byte)(num5 % 256);
                        this.SendBytes_BlockWrite[8] = (byte)(num5 / 256);
                    }
                }
                Array.Resize<byte>(ref this.SendBytes_BlockWrite, 9 + num5);
                if (_Unit == ePLCControl.SubCommand.Word)
                {
                    int num6 = num1 != 1 ? 960 : (_Data.Length % 960 != 0 ? _Data.Length % 960 : 960);
                    for (int index3 = 0; index3 < num6; ++index3)
                    {
                       // this.SendBytes_BlockWrite[21 + index3 * 2] = (byte)(_Data[index1] % 256);
                       // this.SendBytes_BlockWrite[22 + index3 * 2] = (byte)(_Data[index1] / 256);

                        this.SendBytes_BlockWrite[21 + index3 * 2] = (byte)(_Data[index1]& 0xFF);       
                        this.SendBytes_BlockWrite[22 + index3 * 2] = (byte)((_Data[index1] >> 8) & 0xFF);
                        ++index1;
                    }
                }
                else if (_Unit == ePLCControl.SubCommand.Bit)
                {
                    int num6 = num1 != 1 ? 7168 : (index2 != 0 ? _Data.Length % 7168 : _Data.Length);
                    for (int index3 = 0; index3 < num6 / 2; ++index3)
                    {
                        this.SendBytes_BlockWrite[21 + index3] = (byte)(_Data[index1 * 2] * 16 + _Data[index1 * 2 + 1]);
                        ++index1;
                    }
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return 1;
                this.bReceived = false;
                this.client.Send(this.SendBytes_BlockWrite);
                int tickCount = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                        return 1;
                }
                --num1;
                this.bReceived = false;
            }
            return 0;
        }

        public int[] ReadDeviceRandom(ePLCControl.DeviceName[] _DeviceName, string[] _StartAddress)
        {
            int[] numArray = new int[_StartAddress.Length];
            if (!this.IsConnected)
                return numArray;
            if (_DeviceName.Length != _StartAddress.Length)
                throw new Exception("_DeviceName갯수와 _StartAddress의 갯수가 일치하지 않습니다.");
            this.SendBytes_RandomRead[11] = (byte)3;
            this.SendBytes_RandomRead[12] = (byte)4;
            this.SendBytes_RandomRead[13] = (byte)0;
            this.SendBytes_RandomRead[14] = (byte)0;
            int num1 = _StartAddress.Length / 192 + 1;
            if (_StartAddress.Length % 192 == 0)
                --num1;
            int num2 = num1;
            int index1 = 0;
            int index2 = 0;
            for (int index3 = 0; index3 < num2; ++index3)
            {
                int num3 = num1 != 1 ? 192 : _StartAddress.Length - 192 * index3;
                this.SendBytes_RandomRead[7] = (byte)((8 + num3 * 4) % 256);
                this.SendBytes_RandomRead[8] = (byte)((8 + num3 * 4) / 256);
                this.SendBytes_RandomRead[15] = (byte)num3;
                this.SendBytes_RandomRead[16] = (byte)0;
                Array.Resize<byte>(ref this.SendBytes_RandomRead, 17 + num3 * 4);
                int num4 = 17;
                for (int index4 = 0; index4 < num3; ++index4)
                {
                    byte num5;
                    byte num6;
                    byte num7;
                    if (_DeviceName[index1] == ePLCControl.DeviceName.D || _DeviceName[index1] == ePLCControl.DeviceName.M || (_DeviceName[index1] == ePLCControl.DeviceName.L || _DeviceName[index1] == ePLCControl.DeviceName.R) || _DeviceName[index1] == ePLCControl.DeviceName.ZR)
                    {
                        int int32 = Convert.ToInt32(_StartAddress[index1]);
                        num5 = (byte)(int32 % 256);
                        num6 = (byte)(int32 / 256);
                        num7 = (byte)(int32 / 65536);
                    }
                    else
                    {
                        if (_DeviceName[index1] != ePLCControl.DeviceName.X && _DeviceName[index1] != ePLCControl.DeviceName.Y && (_DeviceName[index1] != ePLCControl.DeviceName.B && _DeviceName[index1] != ePLCControl.DeviceName.W))
                            throw new Exception("잘못된 DeviceName이 존재합니다");
                        int num8 = int.Parse(_StartAddress[index1], NumberStyles.HexNumber);
                        num5 = (byte)(num8 % 256);
                        num6 = (byte)(num8 / 256);
                        num7 = (byte)(num8 / 65536);
                    }
                    byte[] sendBytesRandomRead1 = this.SendBytes_RandomRead;
                    int index5 = num4;
                    int num9 = index5 + 1;
                    int num10 = (int)num5;
                    sendBytesRandomRead1[index5] = (byte)num10;
                    byte[] sendBytesRandomRead2 = this.SendBytes_RandomRead;
                    int index6 = num9;
                    int num11 = index6 + 1;
                    int num12 = (int)num6;
                    sendBytesRandomRead2[index6] = (byte)num12;
                    byte[] sendBytesRandomRead3 = this.SendBytes_RandomRead;
                    int index7 = num11;
                    int num13 = index7 + 1;
                    int num14 = (int)num7;
                    sendBytesRandomRead3[index7] = (byte)num14;
                    byte[] sendBytesRandomRead4 = this.SendBytes_RandomRead;
                    int index8 = num13;
                    num4 = index8 + 1;
                    int num15 = (int)_DeviceName[index1];
                    sendBytesRandomRead4[index8] = (byte)num15;
                    ++index1;
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return numArray;
                this.bReceived = false;
                this.client.Send(this.SendBytes_RandomRead);
                int tickCount = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                        return numArray;
                }
                int num16 = this.ReceivedData.Length / 2;
                for (int index4 = 0; index4 < num16; ++index4)
                {
                    numArray[index2] = (int)this.ReceivedData[index4 * 2 + 1] * 256 + (int)this.ReceivedData[index4 * 2];
                    ++index2;
                }
                this.bReceived = false;
                --num1;
            }
            return numArray;
        }

        public int WriteDeviceRandom(
          ePLCControl.DeviceName[] _DeviceName,
          string[] _StartAddress,
          int[] _Data)
        {
            if (!this.IsConnected)
                return 1;
            if (_DeviceName.Length != _StartAddress.Length || _DeviceName.Length != _Data.Length)
                throw new Exception("_DeviceName,_StartAddress,_Data의 갯수가 일치해야 합니다.");
            this.SendBytes_RandomWrite[11] = (byte)2;
            this.SendBytes_RandomWrite[12] = (byte)20;
            this.SendBytes_RandomWrite[13] = (byte)0;
            this.SendBytes_RandomWrite[14] = (byte)0;
            int num1 = _StartAddress.Length / 160 + 1;
            if (_StartAddress.Length % 160 == 0)
                --num1;
            int num2 = num1;
            int index1 = 0;
            for (int index2 = 0; index2 < num2; ++index2)
            {
                int num3;
                if (num1 == 1)
                {
                    if (_StartAddress.Length % 160 == 0)
                    {
                        num3 = 160;
                        this.SendBytes_RandomWrite[15] = (byte)160;
                        this.SendBytes_RandomWrite[16] = (byte)0;
                    }
                    else
                    {
                        num3 = _StartAddress.Length % 160;
                        this.SendBytes_RandomWrite[15] = (byte)num3;
                        this.SendBytes_RandomWrite[16] = (byte)0;
                    }
                }
                else
                {
                    num3 = 160;
                    this.SendBytes_RandomWrite[15] = (byte)160;
                    this.SendBytes_RandomWrite[16] = (byte)0;
                }
                Array.Resize<byte>(ref this.SendBytes_RandomWrite, 17 + num3 * 6);
                int num4 = 17;
                for (int index3 = 0; index3 < num3; ++index3)
                {
                    byte num5 = 0;
                    byte num6 = 0;
                    byte num7 = 0;
                    if (_DeviceName[index1] == ePLCControl.DeviceName.D || _DeviceName[index1] == ePLCControl.DeviceName.M || (_DeviceName[index1] == ePLCControl.DeviceName.L || _DeviceName[index1] == ePLCControl.DeviceName.R) || _DeviceName[index1] == ePLCControl.DeviceName.ZR)
                    {
                        int int32 = Convert.ToInt32(_StartAddress[index1]);
                        num5 = (byte)(int32 % 256);
                        num6 = (byte)(int32 / 256);
                        num7 = (byte)(int32 / 65536);
                    }
                    else if (_DeviceName[index1] == ePLCControl.DeviceName.X || _DeviceName[index1] == ePLCControl.DeviceName.Y || (_DeviceName[index1] == ePLCControl.DeviceName.B || _DeviceName[index1] == ePLCControl.DeviceName.W))
                    {
                        int num8 = int.Parse(_StartAddress[index1], NumberStyles.HexNumber);
                        num5 = (byte)(num8 % 256);
                        num6 = (byte)(num8 / 256);
                        num7 = (byte)(num8 / 65536);
                    }
                    byte[] bytesRandomWrite1 = this.SendBytes_RandomWrite;
                    int index4 = num4;
                    int num9 = index4 + 1;
                    int num10 = (int)num5;
                    bytesRandomWrite1[index4] = (byte)num10;
                    byte[] bytesRandomWrite2 = this.SendBytes_RandomWrite;
                    int index5 = num9;
                    int num11 = index5 + 1;
                    int num12 = (int)num6;
                    bytesRandomWrite2[index5] = (byte)num12;
                    byte[] bytesRandomWrite3 = this.SendBytes_RandomWrite;
                    int index6 = num11;
                    int num13 = index6 + 1;
                    int num14 = (int)num7;
                    bytesRandomWrite3[index6] = (byte)num14;
                    byte[] bytesRandomWrite4 = this.SendBytes_RandomWrite;
                    int index7 = num13;
                    int num15 = index7 + 1;
                    int num16 = (int)_DeviceName[index1];
                    bytesRandomWrite4[index7] = (byte)num16;
                    byte[] bytesRandomWrite5 = this.SendBytes_RandomWrite;
                    int index8 = num15;
                    int num17 = index8 + 1;
                    int num18 = (int)(byte)(_Data[index1] % 256);
                    bytesRandomWrite5[index8] = (byte)num18;
                    byte[] bytesRandomWrite6 = this.SendBytes_RandomWrite;
                    int index9 = num17;
                    num4 = index9 + 1;
                    int num19 = (int)(byte)(_Data[index1] / 256);
                    bytesRandomWrite6[index9] = (byte)num19;
                    ++index1;
                }
                this.SendBytes_RandomWrite[7] = (byte)((8 + num3 * 6) % 256);
                this.SendBytes_RandomWrite[8] = (byte)((8 + num3 * 6) / 256);
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return 1;
                this.bReceived = false;
                this.client.Send(this.SendBytes_RandomWrite);
                int tickCount = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                        return 1;
                }
                --num1;
            }
            return 0;
        }

        public int[] ReadDeviceMultiBlock(
          ePLCControl.DeviceName_WORD[] _WordDeviceName,
          string[] _WordStartAddress,
          int[] _WordNumPoint,
          ePLCControl.DeviceName_BIT[] _BitDeviceName,
          string[] _BitStartAddress,
          int[] _BitNumPoint)
        {
            int[] numArray1 = new int[0];
            if (!this.IsConnected || _WordDeviceName.Length != _WordStartAddress.Length || (_WordDeviceName.Length != _WordNumPoint.Length || _BitDeviceName.Length != _BitStartAddress.Length) || (_BitDeviceName.Length != _BitNumPoint.Length || _WordStartAddress.Length + _BitStartAddress.Length > 120 || _WordDeviceName.Length + _BitDeviceName.Length > 960))
                return numArray1;
            Array.Resize<byte>(ref this.SendBytes_MultiBlockRead, 17 + (_WordStartAddress.Length + _BitStartAddress.Length) * 6);
            this.SendBytes_MultiBlockRead[7] = (byte)((8 + (_WordStartAddress.Length + _BitStartAddress.Length) * 6) % 256);
            this.SendBytes_MultiBlockRead[8] = (byte)((8 + (_WordStartAddress.Length + _BitStartAddress.Length) * 6) / 256);
            this.SendBytes_MultiBlockRead[11] = (byte)6;
            this.SendBytes_MultiBlockRead[12] = (byte)4;
            this.SendBytes_MultiBlockRead[15] = (byte)_WordStartAddress.Length;
            this.SendBytes_MultiBlockRead[16] = (byte)_BitStartAddress.Length;
            int num1 = 17;
            for (int index1 = 0; index1 < _WordStartAddress.Length; ++index1)
            {
                byte num2;
                byte num3;
                byte num4;
                if (_WordDeviceName[index1] == ePLCControl.DeviceName_WORD.D || _WordDeviceName[index1] == ePLCControl.DeviceName_WORD.R || _WordDeviceName[index1] == ePLCControl.DeviceName_WORD.ZR)
                {
                    int int32 = Convert.ToInt32(_WordStartAddress[index1]);
                    num2 = (byte)(int32 % 256);
                    num3 = (byte)(int32 / 256);
                    num4 = (byte)(int32 / 65536);
                }
                else
                {
                    if (_WordDeviceName[index1] != ePLCControl.DeviceName_WORD.W)
                        return numArray1;
                    int num5 = int.Parse(_WordStartAddress[index1], NumberStyles.HexNumber);
                    num2 = (byte)(num5 % 256);
                    num3 = (byte)(num5 / 256);
                    num4 = (byte)(num5 / 65536);
                }
                byte[] bytesMultiBlockRead1 = this.SendBytes_MultiBlockRead;
                int index2 = num1;
                int num6 = index2 + 1;
                int num7 = (int)num2;
                bytesMultiBlockRead1[index2] = (byte)num7;
                byte[] bytesMultiBlockRead2 = this.SendBytes_MultiBlockRead;
                int index3 = num6;
                int num8 = index3 + 1;
                int num9 = (int)num3;
                bytesMultiBlockRead2[index3] = (byte)num9;
                byte[] bytesMultiBlockRead3 = this.SendBytes_MultiBlockRead;
                int index4 = num8;
                int num10 = index4 + 1;
                int num11 = (int)num4;
                bytesMultiBlockRead3[index4] = (byte)num11;
                byte[] bytesMultiBlockRead4 = this.SendBytes_MultiBlockRead;
                int index5 = num10;
                int num12 = index5 + 1;
                int num13 = (int)_WordDeviceName[index1];
                bytesMultiBlockRead4[index5] = (byte)num13;
                byte[] bytesMultiBlockRead5 = this.SendBytes_MultiBlockRead;
                int index6 = num12;
                int num14 = index6 + 1;
                int num15 = (int)(byte)(_WordNumPoint[index1] % 256);
                bytesMultiBlockRead5[index6] = (byte)num15;
                byte[] bytesMultiBlockRead6 = this.SendBytes_MultiBlockRead;
                int index7 = num14;
                num1 = index7 + 1;
                int num16 = (int)(byte)(_WordNumPoint[index1] / 256);
                bytesMultiBlockRead6[index7] = (byte)num16;
            }
            for (int index1 = 0; index1 < _BitStartAddress.Length; ++index1)
            {
                byte num2;
                byte num3;
                byte num4;
                if (_BitDeviceName[index1] == ePLCControl.DeviceName_BIT.M || _BitDeviceName[index1] == ePLCControl.DeviceName_BIT.L)
                {
                    int int32 = Convert.ToInt32(_BitStartAddress[index1]);
                    num2 = (byte)(int32 % 256);
                    num3 = (byte)(int32 / 256);
                    num4 = (byte)(int32 / 65536);
                }
                else
                {
                    if (_BitDeviceName[index1] != ePLCControl.DeviceName_BIT.X && _BitDeviceName[index1] != ePLCControl.DeviceName_BIT.Y && _BitDeviceName[index1] != ePLCControl.DeviceName_BIT.B)
                        return numArray1;
                    int num5 = int.Parse(_BitStartAddress[index1], NumberStyles.HexNumber);
                    num2 = (byte)(num5 % 256);
                    num3 = (byte)(num5 / 256);
                    num4 = (byte)(num5 / 65536);
                }
                byte[] bytesMultiBlockRead1 = this.SendBytes_MultiBlockRead;
                int index2 = num1;
                int num6 = index2 + 1;
                int num7 = (int)num2;
                bytesMultiBlockRead1[index2] = (byte)num7;
                byte[] bytesMultiBlockRead2 = this.SendBytes_MultiBlockRead;
                int index3 = num6;
                int num8 = index3 + 1;
                int num9 = (int)num3;
                bytesMultiBlockRead2[index3] = (byte)num9;
                byte[] bytesMultiBlockRead3 = this.SendBytes_MultiBlockRead;
                int index4 = num8;
                int num10 = index4 + 1;
                int num11 = (int)num4;
                bytesMultiBlockRead3[index4] = (byte)num11;
                byte[] bytesMultiBlockRead4 = this.SendBytes_MultiBlockRead;
                int index5 = num10;
                int num12 = index5 + 1;
                int num13 = (int)_BitDeviceName[index1];
                bytesMultiBlockRead4[index5] = (byte)num13;
                byte[] bytesMultiBlockRead5 = this.SendBytes_MultiBlockRead;
                int index6 = num12;
                int num14 = index6 + 1;
                int num15 = (int)(byte)(_BitNumPoint[index1] % 256);
                bytesMultiBlockRead5[index6] = (byte)num15;
                byte[] bytesMultiBlockRead6 = this.SendBytes_MultiBlockRead;
                int index7 = num14;
                num1 = index7 + 1;
                int num16 = (int)(byte)(_BitNumPoint[index1] / 256);
                bytesMultiBlockRead6[index7] = (byte)num16;
            }
            if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                return numArray1;
            this.bReceived = false;
            this.client.Send(this.SendBytes_MultiBlockRead);
            int tickCount = Environment.TickCount;
            while (!this.bReceived)
            {
                if (Environment.TickCount - tickCount > this.TimeOut)
                    return numArray1;
            }
            int[] numArray2 = new int[this.ReceivedData.Length / 2];
            for (int index = 0; index < numArray2.Length; ++index)
                numArray2[index] = (int)this.ReceivedData[index * 2 + 1] * 256 + (int)this.ReceivedData[index * 2];
            return numArray2;
        }

        public int[] ReadDeviceMultiBlock_WORD(
          ePLCControl.DeviceName_WORD[] _DeviceName,
          string[] _StartAddress,
          int[] _NumPoint)
        {
            int[] numArray1 = new int[0];
            if (!this.IsConnected)
                return numArray1;
            if (_DeviceName.Length != _StartAddress.Length || _DeviceName.Length != _NumPoint.Length)
                throw new Exception("_DeviceName, _StartAddress, _NumPoint 의 갯수는 같아야 합니다.");
            bool flag = false;
            int length = 0;
            for (int index = 0; index < _NumPoint.Length; ++index)
            {
                if (_NumPoint[index] > 960)
                    flag = true;
                length += _NumPoint[index];
            }
            int[] numArray2 = new int[length];
            if (flag)
                throw new Exception("_WordNumPoint에 960을 초과하는 숫자가 존재합니다. ");
            int num1 = 0;
            int num2 = 0;
            int index1 = 0;
            List<int> intList1 = new List<int>();
            List<int> intList2 = new List<int>();
            for (int index2 = 0; index2 < _NumPoint.Length; ++index2)
            {
                ++num1;
                num2 += _NumPoint[index2];
                if (index2 == _NumPoint.Length - 1)
                {
                    if (num1 > 120 || num2 > 960)
                    {
                        intList1.Add(num1 - 1);
                        intList2.Add(num2 - _NumPoint[index2]);
                        intList1.Add(1);
                        intList2.Add(_NumPoint[index2]);
                        break;
                    }
                    intList1.Add(num1);
                    intList2.Add(num2);
                    break;
                }
                if (num1 > 120 || num2 > 960)
                {
                    intList1.Add(num1 - 1);
                    intList2.Add(num2 - _NumPoint[index2]);
                    num1 = 1;
                    num2 = _NumPoint[index2];
                }
            }
            this.SendBytes_MultiBlockRead[11] = (byte)6;
            this.SendBytes_MultiBlockRead[12] = (byte)4;
            int index3 = 0;
            for (int index2 = 0; index2 < intList1.Count; ++index2)
            {
                Array.Resize<byte>(ref this.SendBytes_MultiBlockRead, 17 + intList1[index2] * 6);
                this.SendBytes_MultiBlockRead[7] = (byte)((8 + intList1[index2] * 6) % 256);
                this.SendBytes_MultiBlockRead[8] = (byte)((8 + intList1[index2] * 6) / 256);
                this.SendBytes_MultiBlockRead[15] = (byte)intList1[index2];
                this.SendBytes_MultiBlockRead[16] = (byte)0;
                int num3 = 17;
                for (int index4 = 0; index4 < intList1[index2]; ++index4)
                {
                    byte num4 = 0;
                    byte num5 = 0;
                    byte num6 = 0;
                    if (_DeviceName[index3] == ePLCControl.DeviceName_WORD.D || _DeviceName[index3] == ePLCControl.DeviceName_WORD.R || _DeviceName[index3] == ePLCControl.DeviceName_WORD.ZR)
                    {
                        int int32 = Convert.ToInt32(_StartAddress[index3]);
                        num4 = (byte)(int32 % 256);
                        num5 = (byte)(int32 / 256);
                        num6 = (byte)(int32 / 65536);
                    }
                    else if (_DeviceName[index3] == ePLCControl.DeviceName_WORD.W)
                    {
                        int num7 = int.Parse(_StartAddress[index3], NumberStyles.HexNumber);
                        num4 = (byte)(num7 % 256);
                        num5 = (byte)(num7 / 256);
                        num6 = (byte)(num7 / 65536);
                    }
                    byte[] bytesMultiBlockRead1 = this.SendBytes_MultiBlockRead;
                    int index5 = num3;
                    int num8 = index5 + 1;
                    int num9 = (int)num4;
                    bytesMultiBlockRead1[index5] = (byte)num9;
                    byte[] bytesMultiBlockRead2 = this.SendBytes_MultiBlockRead;
                    int index6 = num8;
                    int num10 = index6 + 1;
                    int num11 = (int)num5;
                    bytesMultiBlockRead2[index6] = (byte)num11;
                    byte[] bytesMultiBlockRead3 = this.SendBytes_MultiBlockRead;
                    int index7 = num10;
                    int num12 = index7 + 1;
                    int num13 = (int)num6;
                    bytesMultiBlockRead3[index7] = (byte)num13;
                    byte[] bytesMultiBlockRead4 = this.SendBytes_MultiBlockRead;
                    int index8 = num12;
                    int num14 = index8 + 1;
                    int num15 = (int)_DeviceName[index3];
                    bytesMultiBlockRead4[index8] = (byte)num15;
                    byte[] bytesMultiBlockRead5 = this.SendBytes_MultiBlockRead;
                    int index9 = num14;
                    int num16 = index9 + 1;
                    int num17 = (int)(byte)(_NumPoint[index3] % 256);
                    bytesMultiBlockRead5[index9] = (byte)num17;
                    byte[] bytesMultiBlockRead6 = this.SendBytes_MultiBlockRead;
                    int index10 = num16;
                    num3 = index10 + 1;
                    int num18 = (int)(byte)(_NumPoint[index3] / 256);
                    bytesMultiBlockRead6[index10] = (byte)num18;
                    ++index3;
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return numArray2;
                this.bReceived = false;
                this.client.Send(this.SendBytes_MultiBlockRead);
                int tickCount = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                        return numArray2;
                }
                int num19 = this.ReceivedData.Length / 2;
                for (int index4 = 0; index4 < num19; ++index4)
                {
                    numArray2[index1] = (int)this.ReceivedData[index4 * 2 + 1] * 256 + (int)this.ReceivedData[index4 * 2];
                    ++index1;
                }
            }
            return numArray2;
        }

        public int[] ReadDeviceMultiBlock_BIT(
          ePLCControl.DeviceName_BIT[] _DeviceName,
          string[] _StartAddress,
          int[] _NumPoint)
        {
            int[] numArray1 = new int[0];
            if (!this.IsConnected)
                return numArray1;
            if (_DeviceName.Length != _StartAddress.Length || _DeviceName.Length != _NumPoint.Length)
                throw new Exception("_WordDeviceName, _WordStartAddress, _WordNumPoint 의 갯수는 같아야 합니다.");
            bool flag = false;
            int length = 0;
            for (int index = 0; index < _NumPoint.Length; ++index)
            {
                if (_NumPoint[index] > 960)
                    flag = true;
                length += _NumPoint[index];
            }
            int[] numArray2 = new int[length];
            if (flag)
                throw new Exception("_WordNumPoint에 960을 초과하는 숫자가 존재합니다. ");
            int num1 = 0;
            int num2 = 0;
            int index1 = 0;
            List<int> intList1 = new List<int>();
            List<int> intList2 = new List<int>();
            for (int index2 = 0; index2 < _NumPoint.Length; ++index2)
            {
                ++num1;
                num2 += _NumPoint[index2];
                if (index2 == _NumPoint.Length - 1)
                {
                    if (num1 > 120 || num2 > 960)
                    {
                        intList1.Add(num1 - 1);
                        intList2.Add(num2 - _NumPoint[index2]);
                        intList1.Add(1);
                        intList2.Add(_NumPoint[index2]);
                        break;
                    }
                    intList1.Add(num1);
                    intList2.Add(num2);
                    break;
                }
                if (num1 > 120 || num2 > 960)
                {
                    intList1.Add(num1 - 1);
                    intList2.Add(num2 - _NumPoint[index2]);
                    num1 = 1;
                    num2 = _NumPoint[index2];
                }
            }
            this.SendBytes_MultiBlockRead[11] = (byte)6;
            this.SendBytes_MultiBlockRead[12] = (byte)4;
            int index3 = 0;
            for (int index2 = 0; index2 < intList1.Count; ++index2)
            {
                Array.Resize<byte>(ref this.SendBytes_MultiBlockRead, 17 + intList1[index2] * 6);
                this.SendBytes_MultiBlockRead[7] = (byte)((8 + intList1[index2] * 6) % 256);
                this.SendBytes_MultiBlockRead[8] = (byte)((8 + intList1[index2] * 6) / 256);
                this.SendBytes_MultiBlockRead[15] = (byte)0;
                this.SendBytes_MultiBlockRead[16] = (byte)intList1[index2];
                int num3 = 17;
                for (int index4 = 0; index4 < intList1[index2]; ++index4)
                {
                    byte num4 = 0;
                    byte num5 = 0;
                    byte num6 = 0;
                    if (_DeviceName[index3] == ePLCControl.DeviceName_BIT.M || _DeviceName[index3] == ePLCControl.DeviceName_BIT.L)
                    {
                        int int32 = Convert.ToInt32(_StartAddress[index3]);
                        num4 = (byte)(int32 % 256);
                        num5 = (byte)(int32 / 256);
                        num6 = (byte)(int32 / 65536);
                    }
                    else if (_DeviceName[index3] == ePLCControl.DeviceName_BIT.X || _DeviceName[index3] == ePLCControl.DeviceName_BIT.Y || _DeviceName[index3] == ePLCControl.DeviceName_BIT.B)
                    {
                        int num7 = int.Parse(_StartAddress[index3], NumberStyles.HexNumber);
                        num4 = (byte)(num7 % 256);
                        num5 = (byte)(num7 / 256);
                        num6 = (byte)(num7 / 65536);
                    }
                    byte[] bytesMultiBlockRead1 = this.SendBytes_MultiBlockRead;
                    int index5 = num3;
                    int num8 = index5 + 1;
                    int num9 = (int)num4;
                    bytesMultiBlockRead1[index5] = (byte)num9;
                    byte[] bytesMultiBlockRead2 = this.SendBytes_MultiBlockRead;
                    int index6 = num8;
                    int num10 = index6 + 1;
                    int num11 = (int)num5;
                    bytesMultiBlockRead2[index6] = (byte)num11;
                    byte[] bytesMultiBlockRead3 = this.SendBytes_MultiBlockRead;
                    int index7 = num10;
                    int num12 = index7 + 1;
                    int num13 = (int)num6;
                    bytesMultiBlockRead3[index7] = (byte)num13;
                    byte[] bytesMultiBlockRead4 = this.SendBytes_MultiBlockRead;
                    int index8 = num12;
                    int num14 = index8 + 1;
                    int num15 = (int)_DeviceName[index3];
                    bytesMultiBlockRead4[index8] = (byte)num15;
                    byte[] bytesMultiBlockRead5 = this.SendBytes_MultiBlockRead;
                    int index9 = num14;
                    int num16 = index9 + 1;
                    int num17 = (int)(byte)(_NumPoint[index3] % 256);
                    bytesMultiBlockRead5[index9] = (byte)num17;
                    byte[] bytesMultiBlockRead6 = this.SendBytes_MultiBlockRead;
                    int index10 = num16;
                    num3 = index10 + 1;
                    int num18 = (int)(byte)(_NumPoint[index3] / 256);
                    bytesMultiBlockRead6[index10] = (byte)num18;
                    ++index3;
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return numArray2;
                this.bReceived = false;
                this.client.Send(this.SendBytes_MultiBlockRead);
                int tickCount = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                        return numArray2;
                }
                int num19 = this.ReceivedData.Length / 2;
                for (int index4 = 0; index4 < num19; ++index4)
                {
                    numArray2[index1] = (int)this.ReceivedData[index4 * 2 + 1] * 256 + (int)this.ReceivedData[index4 * 2];
                    ++index1;
                }
            }
            return numArray2;
        }

        public int WriteDeviceMultiBlock(
          ePLCControl.DeviceName_WORD[] _WordDeviceName,
          string[] _WordStartAddress,
          int[] _WordNumPoint,
          ePLCControl.DeviceName_BIT[] _BitDeviceName,
          string[] _BitStartAddress,
          int[] _BitNumPoint,
          int[] _Data)
        {
            if (!this.IsConnected)
                return 1;
            if (_WordDeviceName.Length != _WordStartAddress.Length || _WordDeviceName.Length != _WordNumPoint.Length)
                throw new Exception("_WordDeviceName, _WordStartAddress, _WordNumPoint의 길이가 같지 않습니다.");
            if (_BitDeviceName.Length != _BitStartAddress.Length || _BitDeviceName.Length != _BitNumPoint.Length)
                throw new Exception("_BitDeviceName, _BitStartAddress, _BitNumPoint의 길이가 같지 않습니다.");
            if ((_WordDeviceName.Length + _BitDeviceName.Length) * 4 + _Data.Length > 960)
                throw new Exception("다음 조건이 만족하지 않습니다:\r\n_WordDeviceName.Length + _BitDeviceName.Length) * 4 + _Data.Length <= 960 \r\n 960 >= 4 * (워드블락수 + 비트블락수) + 워드블락의 합계점수 + 비트블락의 함계점수");
            Array.Resize<byte>(ref this.SendBytes_MultiBlockWrite, 17 + (_WordStartAddress.Length + _BitStartAddress.Length) * 6 + _Data.Length * 2);
            this.SendBytes_MultiBlockWrite[7] = (byte)((8 + (_WordStartAddress.Length + _BitStartAddress.Length) * 6 + _Data.Length * 2) % 256);
            this.SendBytes_MultiBlockWrite[8] = (byte)((8 + (_WordStartAddress.Length + _BitStartAddress.Length) * 6 + _Data.Length * 2) / 256);
            this.SendBytes_MultiBlockWrite[11] = (byte)6;
            this.SendBytes_MultiBlockWrite[12] = (byte)20;
            this.SendBytes_MultiBlockWrite[15] = (byte)_WordStartAddress.Length;
            this.SendBytes_MultiBlockWrite[16] = (byte)_BitStartAddress.Length;
            int num1 = 17;
            int index1 = 0;
            for (int index2 = 0; index2 < _WordStartAddress.Length; ++index2)
            {
                byte num2;
                byte num3;
                byte num4;
                if (_WordDeviceName[index2] == ePLCControl.DeviceName_WORD.D || _WordDeviceName[index2] == ePLCControl.DeviceName_WORD.R || _WordDeviceName[index2] == ePLCControl.DeviceName_WORD.ZR)
                {
                    int int32 = Convert.ToInt32(_WordStartAddress[index2]);
                    num2 = (byte)(int32 % 256);
                    num3 = (byte)(int32 / 256);
                    num4 = (byte)(int32 / 65536);
                }
                else
                {
                    if (_WordDeviceName[index2] != ePLCControl.DeviceName_WORD.W)
                        return 2;
                    int num5 = int.Parse(_WordStartAddress[index2], NumberStyles.HexNumber);
                    num2 = (byte)(num5 % 256);
                    num3 = (byte)(num5 / 256);
                    num4 = (byte)(num5 / 65536);
                }
                byte[] bytesMultiBlockWrite1 = this.SendBytes_MultiBlockWrite;
                int index3 = num1;
                int num6 = index3 + 1;
                int num7 = (int)num2;
                bytesMultiBlockWrite1[index3] = (byte)num7;
                byte[] bytesMultiBlockWrite2 = this.SendBytes_MultiBlockWrite;
                int index4 = num6;
                int num8 = index4 + 1;
                int num9 = (int)num3;
                bytesMultiBlockWrite2[index4] = (byte)num9;
                byte[] bytesMultiBlockWrite3 = this.SendBytes_MultiBlockWrite;
                int index5 = num8;
                int num10 = index5 + 1;
                int num11 = (int)num4;
                bytesMultiBlockWrite3[index5] = (byte)num11;
                byte[] bytesMultiBlockWrite4 = this.SendBytes_MultiBlockWrite;
                int index6 = num10;
                int num12 = index6 + 1;
                int num13 = (int)_WordDeviceName[index2];
                bytesMultiBlockWrite4[index6] = (byte)num13;
                byte[] bytesMultiBlockWrite5 = this.SendBytes_MultiBlockWrite;
                int index7 = num12;
                int num14 = index7 + 1;
                int num15 = (int)(byte)(_WordNumPoint[index2] % 256);
                bytesMultiBlockWrite5[index7] = (byte)num15;
                byte[] bytesMultiBlockWrite6 = this.SendBytes_MultiBlockWrite;
                int index8 = num14;
                num1 = index8 + 1;
                int num16 = (int)(byte)(_WordNumPoint[index2] / 256);
                bytesMultiBlockWrite6[index8] = (byte)num16;
                for (int index9 = 0; index9 < _WordNumPoint[index2]; ++index9)
                {
                    byte[] bytesMultiBlockWrite7 = this.SendBytes_MultiBlockWrite;
                    int index10 = num1;
                    int num5 = index10 + 1;
                    int num17 = (int)(byte)(_Data[index1] % 256);
                    bytesMultiBlockWrite7[index10] = (byte)num17;
                    byte[] bytesMultiBlockWrite8 = this.SendBytes_MultiBlockWrite;
                    int index11 = num5;
                    num1 = index11 + 1;
                    int num18 = (int)(byte)(_Data[index1] / 256);
                    bytesMultiBlockWrite8[index11] = (byte)num18;
                    ++index1;
                }
            }
            for (int index2 = 0; index2 < _BitStartAddress.Length; ++index2)
            {
                byte num2 = 0;
                byte num3 = 0;
                byte num4 = 0;
                if (_BitDeviceName[index2] == ePLCControl.DeviceName_BIT.M || _BitDeviceName[index2] == ePLCControl.DeviceName_BIT.L)
                {
                    int int32 = Convert.ToInt32(_BitStartAddress[index2]);
                    num2 = (byte)(int32 % 256);
                    num3 = (byte)(int32 / 256);
                    num4 = (byte)(int32 / 65536);
                }
                else if (_BitDeviceName[index2] == ePLCControl.DeviceName_BIT.X || _BitDeviceName[index2] == ePLCControl.DeviceName_BIT.Y || _BitDeviceName[index2] == ePLCControl.DeviceName_BIT.B)
                {
                    int num5 = int.Parse(_BitStartAddress[index2], NumberStyles.HexNumber);
                    num2 = (byte)(num5 % 256);
                    num3 = (byte)(num5 / 256);
                    num4 = (byte)(num5 / 65536);
                }
                byte[] bytesMultiBlockWrite1 = this.SendBytes_MultiBlockWrite;
                int index3 = num1;
                int num6 = index3 + 1;
                int num7 = (int)num2;
                bytesMultiBlockWrite1[index3] = (byte)num7;
                byte[] bytesMultiBlockWrite2 = this.SendBytes_MultiBlockWrite;
                int index4 = num6;
                int num8 = index4 + 1;
                int num9 = (int)num3;
                bytesMultiBlockWrite2[index4] = (byte)num9;
                byte[] bytesMultiBlockWrite3 = this.SendBytes_MultiBlockWrite;
                int index5 = num8;
                int num10 = index5 + 1;
                int num11 = (int)num4;
                bytesMultiBlockWrite3[index5] = (byte)num11;
                byte[] bytesMultiBlockWrite4 = this.SendBytes_MultiBlockWrite;
                int index6 = num10;
                int num12 = index6 + 1;
                int num13 = (int)_BitDeviceName[index2];
                bytesMultiBlockWrite4[index6] = (byte)num13;
                byte[] bytesMultiBlockWrite5 = this.SendBytes_MultiBlockWrite;
                int index7 = num12;
                int num14 = index7 + 1;
                int num15 = (int)(byte)(_BitNumPoint[index2] % 256);
                bytesMultiBlockWrite5[index7] = (byte)num15;
                byte[] bytesMultiBlockWrite6 = this.SendBytes_MultiBlockWrite;
                int index8 = num14;
                num1 = index8 + 1;
                int num16 = (int)(byte)(_BitNumPoint[index2] / 256);
                bytesMultiBlockWrite6[index8] = (byte)num16;
                for (int index9 = 0; index9 < _BitNumPoint[index2]; ++index9)
                {
                    byte[] bytesMultiBlockWrite7 = this.SendBytes_MultiBlockWrite;
                    int index10 = num1;
                    int num5 = index10 + 1;
                    int num17 = (int)(byte)(_Data[index1] % 256);
                    bytesMultiBlockWrite7[index10] = (byte)num17;
                    byte[] bytesMultiBlockWrite8 = this.SendBytes_MultiBlockWrite;
                    int index11 = num5;
                    num1 = index11 + 1;
                    int num18 = (int)(byte)(_Data[index1] / 256);
                    bytesMultiBlockWrite8[index11] = (byte)num18;
                    ++index1;
                }
            }
            if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                return 1;
            this.bReceived = false;
            this.client.Send(this.SendBytes_MultiBlockWrite);
            int tickCount = Environment.TickCount;
            while (!this.bReceived)
            {
                if (Environment.TickCount - tickCount > this.TimeOut)
                    return 1;
            }
            return 0;
        }

        public int WriteDeviceMultiBlock_WORD(
          ePLCControl.DeviceName_WORD[] _DeviceName,
          string[] _StartAddress,
          int[] _NumPoint,
          int[] _Data)
        {
            int tickCount1 = Environment.TickCount;
            if (!this.IsConnected)
                return 1;
            if (_DeviceName.Length != _StartAddress.Length || _DeviceName.Length != _NumPoint.Length)
                throw new Exception("_DeviceName, _StartAddress, _NumPoint의 길이가 같지 않습니다.");
            int num1 = 0;
            int num2 = 0;
            List<int> intList1 = new List<int>();
            List<int> intList2 = new List<int>();
            for (int index = 0; index < _NumPoint.Length; ++index)
            {
                ++num1;
                num2 += _NumPoint[index];
                if (index == _NumPoint.Length - 1)
                {
                    if (num1 * 4 + num2 > 960)
                    {
                        intList1.Add(num1 - 1);
                        intList2.Add(num2 - _NumPoint[index]);
                        intList1.Add(1);
                        intList2.Add(_NumPoint[index]);
                        break;
                    }
                    intList1.Add(num1);
                    intList2.Add(num2);
                    break;
                }
                if (num1 * 4 + num2 > 960)
                {
                    intList1.Add(num1 - 1);
                    intList2.Add(num2 - _NumPoint[index]);
                    num1 = 1;
                    num2 = _NumPoint[index];
                }
            }
            this.SendBytes_MultiBlockWrite[11] = (byte)6;
            this.SendBytes_MultiBlockWrite[12] = (byte)20;
            int index1 = 0;
            int index2 = 0;
            for (int index3 = 0; index3 < intList1.Count; ++index3)
            {
                Array.Resize<byte>(ref this.SendBytes_MultiBlockWrite, 17 + intList1[index3] * 6 + intList2[index3] * 2);
                this.SendBytes_MultiBlockWrite[7] = (byte)((8 + intList1[index3] * 6 + intList2[index3] * 2) % 256);
                this.SendBytes_MultiBlockWrite[8] = (byte)((8 + intList1[index3] * 6 + intList2[index3] * 2) / 256);
                this.SendBytes_MultiBlockWrite[15] = (byte)intList1[index3];
                this.SendBytes_MultiBlockWrite[16] = (byte)0;
                int num3 = 17;
                for (int index4 = 0; index4 < intList1[index3]; ++index4)
                {
                    byte num4 = 0;
                    byte num5 = 0;
                    byte num6 = 0;
                    if (_DeviceName[index1] == ePLCControl.DeviceName_WORD.D || _DeviceName[index1] == ePLCControl.DeviceName_WORD.R || _DeviceName[index1] == ePLCControl.DeviceName_WORD.ZR)
                    {
                        int int32 = Convert.ToInt32(_StartAddress[index1]);
                        num4 = (byte)(int32 % 256);
                        num5 = (byte)(int32 / 256);
                        num6 = (byte)(int32 / 65536);
                    }
                    else if (_DeviceName[index1] == ePLCControl.DeviceName_WORD.W)
                    {
                        int num7 = int.Parse(_StartAddress[index1], NumberStyles.HexNumber);
                        num4 = (byte)(num7 % 256);
                        num5 = (byte)(num7 / 256);
                        num6 = (byte)(num7 / 65536);
                    }
                    byte[] bytesMultiBlockWrite1 = this.SendBytes_MultiBlockWrite;
                    int index5 = num3;
                    int num8 = index5 + 1;
                    int num9 = (int)num4;
                    bytesMultiBlockWrite1[index5] = (byte)num9;
                    byte[] bytesMultiBlockWrite2 = this.SendBytes_MultiBlockWrite;
                    int index6 = num8;
                    int num10 = index6 + 1;
                    int num11 = (int)num5;
                    bytesMultiBlockWrite2[index6] = (byte)num11;
                    byte[] bytesMultiBlockWrite3 = this.SendBytes_MultiBlockWrite;
                    int index7 = num10;
                    int num12 = index7 + 1;
                    int num13 = (int)num6;
                    bytesMultiBlockWrite3[index7] = (byte)num13;
                    byte[] bytesMultiBlockWrite4 = this.SendBytes_MultiBlockWrite;
                    int index8 = num12;
                    int num14 = index8 + 1;
                    int num15 = (int)_DeviceName[index1];
                    bytesMultiBlockWrite4[index8] = (byte)num15;
                    byte[] bytesMultiBlockWrite5 = this.SendBytes_MultiBlockWrite;
                    int index9 = num14;
                    int num16 = index9 + 1;
                    int num17 = (int)(byte)(_NumPoint[index1] % 256);
                    bytesMultiBlockWrite5[index9] = (byte)num17;
                    byte[] bytesMultiBlockWrite6 = this.SendBytes_MultiBlockWrite;
                    int index10 = num16;
                    num3 = index10 + 1;
                    int num18 = (int)(byte)(_NumPoint[index1] / 256);
                    bytesMultiBlockWrite6[index10] = (byte)num18;
                    for (int index11 = 0; index11 < _NumPoint[index1]; ++index11)
                    {
                        byte[] bytesMultiBlockWrite7 = this.SendBytes_MultiBlockWrite;
                        int index12 = num3;
                        int num7 = index12 + 1;
                        int num19 = (int)(byte)(_Data[index2] % 256);
                        bytesMultiBlockWrite7[index12] = (byte)num19;
                        byte[] bytesMultiBlockWrite8 = this.SendBytes_MultiBlockWrite;
                        int index13 = num7;
                        num3 = index13 + 1;
                        int num20 = (int)(byte)(_Data[index2] / 256);
                        bytesMultiBlockWrite8[index13] = (byte)num20;
                        ++index2;
                    }
                    ++index1;
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return 1;
                this.bReceived = false;
                this.client.Send(this.SendBytes_MultiBlockWrite);
                int tickCount2 = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount2 > this.TimeOut)
                        return 1;
                }
            }
//            this.WriteLogFile(((double)((Environment.TickCount - tickCount1) / 1000)).ToString());
            return 0;
        }

        public int WriteDeviceMultiBlock_BIT(
          ePLCControl.DeviceName_BIT[] _DeviceName,
          string[] _StartAddress,
          int[] _NumPoint,
          int[] _Data)
        {
            if (!this.IsConnected)
                return 1;
            if (_DeviceName.Length != _StartAddress.Length || _DeviceName.Length != _NumPoint.Length)
                throw new Exception("_DeviceName, _StartAddress, _NumPoint의 길이가 같지 않습니다.");
            int num1 = 0;
            int num2 = 0;
            List<int> intList1 = new List<int>();
            List<int> intList2 = new List<int>();
            for (int index = 0; index < _NumPoint.Length; ++index)
            {
                ++num1;
                num2 += _NumPoint[index];
                if (index == _NumPoint.Length - 1)
                {
                    if (num1 * 4 + num2 > 960)
                    {
                        intList1.Add(num1 - 1);
                        intList2.Add(num2 - _NumPoint[index]);
                        intList1.Add(1);
                        intList2.Add(_NumPoint[index]);
                        break;
                    }
                    intList1.Add(num1);
                    intList2.Add(num2);
                    break;
                }
                if (num1 * 4 + num2 > 960)
                {
                    intList1.Add(num1 - 1);
                    intList2.Add(num2 - _NumPoint[index]);
                    num1 = 1;
                    num2 = _NumPoint[index];
                }
            }
            this.SendBytes_MultiBlockWrite[11] = (byte)6;
            this.SendBytes_MultiBlockWrite[12] = (byte)20;
            int index1 = 0;
            int index2 = 0;
            for (int index3 = 0; index3 < intList1.Count; ++index3)
            {
                Array.Resize<byte>(ref this.SendBytes_MultiBlockWrite, 17 + intList1[index3] * 6 + intList2[index3] * 2);
                this.SendBytes_MultiBlockWrite[7] = (byte)((8 + intList1[index3] * 6 + intList2[index3] * 2) % 256);
                this.SendBytes_MultiBlockWrite[8] = (byte)((8 + intList1[index3] * 6 + intList2[index3] * 2) / 256);
                this.SendBytes_MultiBlockWrite[15] = (byte)0;
                this.SendBytes_MultiBlockWrite[16] = (byte)intList1[index3];
                int num3 = 17;
                for (int index4 = 0; index4 < intList1[index3]; ++index4)
                {
                    byte num4 = 0;
                    byte num5 = 0;
                    byte num6 = 0;
                    if (_DeviceName[index1] == ePLCControl.DeviceName_BIT.M || _DeviceName[index1] == ePLCControl.DeviceName_BIT.L)
                    {
                        int int32 = Convert.ToInt32(_StartAddress[index1]);
                        num4 = (byte)(int32 % 256);
                        num5 = (byte)(int32 / 256);
                        num6 = (byte)(int32 / 65536);
                    }
                    else if (_DeviceName[index1] == ePLCControl.DeviceName_BIT.X || _DeviceName[index1] == ePLCControl.DeviceName_BIT.Y || _DeviceName[index1] == ePLCControl.DeviceName_BIT.B)
                    {
                        int num7 = int.Parse(_StartAddress[index1], NumberStyles.HexNumber);
                        num4 = (byte)(num7 % 256);
                        num5 = (byte)(num7 / 256);
                        num6 = (byte)(num7 / 65536);
                    }
                    byte[] bytesMultiBlockWrite1 = this.SendBytes_MultiBlockWrite;
                    int index5 = num3;
                    int num8 = index5 + 1;
                    int num9 = (int)num4;
                    bytesMultiBlockWrite1[index5] = (byte)num9;
                    byte[] bytesMultiBlockWrite2 = this.SendBytes_MultiBlockWrite;
                    int index6 = num8;
                    int num10 = index6 + 1;
                    int num11 = (int)num5;
                    bytesMultiBlockWrite2[index6] = (byte)num11;
                    byte[] bytesMultiBlockWrite3 = this.SendBytes_MultiBlockWrite;
                    int index7 = num10;
                    int num12 = index7 + 1;
                    int num13 = (int)num6;
                    bytesMultiBlockWrite3[index7] = (byte)num13;
                    byte[] bytesMultiBlockWrite4 = this.SendBytes_MultiBlockWrite;
                    int index8 = num12;
                    int num14 = index8 + 1;
                    int num15 = (int)_DeviceName[index1];
                    bytesMultiBlockWrite4[index8] = (byte)num15;
                    byte[] bytesMultiBlockWrite5 = this.SendBytes_MultiBlockWrite;
                    int index9 = num14;
                    int num16 = index9 + 1;
                    int num17 = (int)(byte)(_NumPoint[index1] % 256);
                    bytesMultiBlockWrite5[index9] = (byte)num17;
                    byte[] bytesMultiBlockWrite6 = this.SendBytes_MultiBlockWrite;
                    int index10 = num16;
                    num3 = index10 + 1;
                    int num18 = (int)(byte)(_NumPoint[index1] / 256);
                    bytesMultiBlockWrite6[index10] = (byte)num18;
                    for (int index11 = 0; index11 < _NumPoint[index1]; ++index11)
                    {
                        byte[] bytesMultiBlockWrite7 = this.SendBytes_MultiBlockWrite;
                        int index12 = num3;
                        int num7 = index12 + 1;
                        int num19 = (int)(byte)(_Data[index2] % 256);
                        bytesMultiBlockWrite7[index12] = (byte)num19;
                        byte[] bytesMultiBlockWrite8 = this.SendBytes_MultiBlockWrite;
                        int index13 = num7;
                        num3 = index13 + 1;
                        int num20 = (int)(byte)(_Data[index2] / 256);
                        bytesMultiBlockWrite8[index13] = (byte)num20;
                        ++index2;
                    }
                    ++index1;
                }
                if (this.client == null || this.client.Connection == null || !this.client.Connection.Connected)
                    return 1;
                this.bReceived = false;
                this.client.Send(this.SendBytes_MultiBlockWrite);
                int tickCount = Environment.TickCount;
                while (!this.bReceived)
                {
                    if (Environment.TickCount - tickCount > this.TimeOut)
                        return 1;
                }
            }
            return 0;
        }

        private string OneWordToString(int nValue)
        {
            char ch1 = (char)(nValue >> 8 & (int)byte.MaxValue);
            if (ch1 == char.MinValue)
                ch1 = ' ';
            string str = ch1.ToString();
            char ch2 = (char)(nValue & (int)byte.MaxValue);
            if (ch2 == char.MinValue)
                ch2 = ' ';
            return str + ch2.ToString();
        }

        private string OneWordToString(int nValue, bool _Reverse)
        {
            char ch1 = (char)(nValue >> 8 & (int)byte.MaxValue);
            if (ch1 == char.MinValue)
                ch1 = ' ';
            string str = ch1.ToString();
            char ch2 = (char)(nValue & (int)byte.MaxValue);
            if (ch2 == char.MinValue)
                ch2 = ' ';
            return !_Reverse ? str + ch2.ToString() : ch2.ToString() + str;
        }

        public string WordsToString(int[] nValue)
        {
            string str = "";
            for (int index = 0; index < nValue.Length; ++index)
                str += this.OneWordToString(nValue[index]);
            return str.TrimEnd();
        }

        public string WordsToString(int[] nValue, bool _UseRemoveSpace)
        {
            string str = "";
            for (int index = 0; index < nValue.Length; ++index)
                str += this.OneWordToString(nValue[index]);
            if (_UseRemoveSpace)
                str = str.TrimEnd();
            return str;
        }

        public string WordsToString(int[] nValue, bool _UseRemoveSpace, bool _Reverse)
        {
            string str = "";
            if (_Reverse)
            {
                for (int index = 0; index < nValue.Length; ++index)
                    str += this.OneWordToString(nValue[index], true);
            }
            else
            {
                for (int index = 0; index < nValue.Length; ++index)
                    str += this.OneWordToString(nValue[index]);
            }
            if (_UseRemoveSpace)
                str = str.TrimEnd();
            return str;
        }

        public int[] StringToWords(string sVal)
        {
            int length = sVal.Length % 2 != 0 ? sVal.Length / 2 + 1 : sVal.Length / 2;
            int[] numArray = new int[length];
            for (int index = 0; index < length; ++index)
                numArray[index] = 0;
            int index1 = 0;
            for (int startIndex = 0; startIndex < sVal.Length; startIndex += 2)
            {
                string str;
                try
                {
                    str = sVal.Substring(startIndex, 2);
                }
                catch
                {
                    str = sVal.Substring(startIndex, 1);
                }
                int num1 = str.Length / 2 + str.Length % 2;
                int num2 = 0;
                char[] chArray1 = new char[1];
                char[] chArray2 = new char[1];
                if (str.Length == 1)
                    str += " ";
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    if (str.Length <= index2 * 2)
                        chArray1[0] = ' ';
                    else
                        chArray1 = str.Substring(index2 * 2 + 1, 1).ToCharArray();
                    if (str.Length <= index2 * 2 + 1)
                        chArray2[0] = ' ';
                    else
                        chArray2 = str.Substring(index2 * 2, 1).ToCharArray();
                    num2 = (int)chArray2[0] << 8 | (int)chArray1[0];
                }
                numArray[index1] = num2;
                ++index1;
            }
            return numArray;
        }

        public int[] StringToWords(string sVal, bool _Reverse)
        {
            int length = sVal.Length % 2 != 0 ? sVal.Length / 2 + 1 : sVal.Length / 2;
            int[] numArray = new int[length];
            for (int index = 0; index < length; ++index)
                numArray[index] = 0;
            int index1 = 0;
            for (int startIndex = 0; startIndex < sVal.Length; startIndex += 2)
            {
                string str;
                try
                {
                    str = sVal.Substring(startIndex, 2);
                }
                catch
                {
                    str = sVal.Substring(startIndex, 1);
                }
                int num1 = str.Length / 2 + str.Length % 2;
                int num2 = 0;
                char[] chArray1 = new char[1];
                char[] chArray2 = new char[1];
                if (str.Length == 1)
                    str += " ";
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    if (str.Length <= index2 * 2)
                        chArray1[0] = ' ';
                    else
                        chArray1 = str.Substring(index2 * 2 + 1, 1).ToCharArray();
                    if (str.Length <= index2 * 2 + 1)
                        chArray2[0] = ' ';
                    else
                        chArray2 = str.Substring(index2 * 2, 1).ToCharArray();
                    num2 = !_Reverse ? (int)chArray2[0] << 8 | (int)chArray1[0] : (int)chArray1[0] << 8 | (int)chArray2[0];
                }
                numArray[index1] = num2;
                ++index1;
            }
            return numArray;
        }

        public string WordToBit(int _Value)
        {
            string str = "";
            if (_Value == 0)
                str = "0";
            for (; _Value != 0; _Value /= 2)
                str = _Value % 2 != 0 ? str + "1" : str + "0";
            while (str.Length != 16)
                str += "0";
            return str;
        }

        public int BitToWord(string sValue)
        {
            int num = 0;
            for (int startIndex = 0; startIndex < sValue.Length; ++startIndex)
                num += Convert.ToInt32(sValue.Substring(startIndex, 1)) * (int)Math.Pow(2.0, (double)startIndex);
            return num;
        }

        public int[] ReadString(
          ePLCControl.DeviceName _DeviceName,
          string _StartAddress,
          int _Length,
          bool _RemoveSpace)
        {
            return this.ReadDeviceBlock(ePLCControl.SubCommand.Word, _DeviceName, _StartAddress, _Length);
        }

        public void WriteString(ePLCControl.DeviceName _DeviceName, string _StartAddress, string _sVal)
        {
            int[] words = this.StringToWords(_sVal);
            this.WriteDeviceBlock(ePLCControl.SubCommand.Word, _DeviceName, _StartAddress, words);
        }

        public uint[] WordsToDoubleWords(int[] _nArr)
        {
            int length;
            if (_nArr.Length % 2 == 1)
            {
                length = _nArr.Length / 2 + 1;
                Array.Resize<int>(ref _nArr, _nArr.Length + 1);
            }
            else
                length = _nArr.Length / 2;
            uint[] numArray1 = new uint[length];
            int num1 = 0;
            for (int index1 = 0; index1 < length; ++index1)
            {
                int[] numArray2 = _nArr;
                int index2 = num1;
                int num2 = index2 + 1;
                int num3 = (int)ushort.MaxValue & numArray2[index2];
                int[] numArray3 = _nArr;
                int index3 = num2;
                num1 = index3 + 1;
                int num4 = numArray3[index3] << 16;
                numArray1[index1] = (uint)(num3 + num4);
            }
            return numArray1;
        }

        public int[] DoubleWordsToWords(uint[] _nArr)
        {
            int[] numArray1 = new int[_nArr.Length * 2];
            int num1 = 0;
            for (int index1 = 0; index1 < _nArr.Length; ++index1)
            {
                int[] numArray2 = numArray1;
                int index2 = num1;
                int num2 = index2 + 1;
                int num3 = (int)ushort.MaxValue & (int)_nArr[index1];
                numArray2[index2] = num3;
                int[] numArray3 = numArray1;
                int index3 = num2;
                num1 = index3 + 1;
                int num4 = (int)(_nArr[index1] >> 16);
                numArray3[index3] = num4;
            }
            return numArray1;
        }

        public enum DeviceName : byte
        {
            M = 144, // 0x90
            L = 146, // 0x92
            X = 156, // 0x9C
            Y = 157, // 0x9D
            B = 160, // 0xA0
            D = 168, // 0xA8
            R = 175, // 0xAF
            ZR = 176, // 0xB0
            W = 180, // 0xB4
        }

        public enum DeviceName_WORD : byte
        {
            D = 168, // 0xA8
            R = 175, // 0xAF
            ZR = 176, // 0xB0
            W = 180, // 0xB4
        }

        public enum DeviceName_BIT : byte
        {
            M = 144, // 0x90
            L = 146, // 0x92
            X = 156, // 0x9C
            Y = 157, // 0x9D
            B = 160, // 0xA0
        }

        private enum Command : byte
        {
            BlockRead_L = 1,
            BlockWrite_L = 1,
            RandomWrite_L = 2,
            RandomRead_L = 3,
            BlockRead_H = 4,
            MultiBlockRead_H = 4,
            RandomRead_H = 4,
            MultiBlockRead_L = 6,
            MultiBlockWrite_L = 6,
            BlockWrite_H = 20, // 0x14
            MultiBlockWrite_H = 20, // 0x14
            RandomWrite_H = 20, // 0x14
        }

        public enum SubCommand
        {
            Bit,
            Word,
        }
    }
}
