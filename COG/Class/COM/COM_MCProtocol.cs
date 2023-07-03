using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;

namespace COG
{
    public class PLC
    {
  //      private UdpClient[] m_UdpClient;
        private TcpClient[] m_TcpClient;
        private IPAddress m_IPAddress;
        private IPEndPoint[] m_IPEndPoint = new IPEndPoint[2];
        private static AutoResetEvent[] m_AutoEventHandle = new AutoResetEvent[1];

        private bool PortOpenConfirm = false;
//         private long SRTime = 0L;
        private bool ThreadConfrim = false;
         private bool WaitDataFlag = false;


        private int ReturnLength = 0;
        private int[] ReceiveResult = new int[0x1000];
        private bool m_bReadPLCFlag;
        private System.Threading.Semaphore m_Semaphore;


  //      private bool ReturnResult = false;
        private int M_READ_ = 0;
        private int M_WRITE = 1;


        public PLC()
        {
            this.m_bReadPLCFlag = false;
            m_AutoEventHandle[0] = new AutoResetEvent(false);
            this.m_Semaphore = new System.Threading.Semaphore(0, 0x2710);
        }
        public void UClose()
        {
            this.WaitDataFlag = true;
            m_TcpClient[M_READ_].Close();
            m_TcpClient[M_WRITE].Close();
        }
        public bool Open(int _intReadLocalPort, int _intReadRemotePort, string _strRemoteIP, int _intReadRecTimeOut, int _intWriteLocalPort, int _intWriteRemotePort)
        {
            bool flag = false;
            m_IPAddress = IPAddress.Parse(_strRemoteIP);

            m_TcpClient = new TcpClient[2];
            m_IPEndPoint[M_READ_] = new IPEndPoint(m_IPAddress, _intReadRemotePort);
            m_IPEndPoint[M_WRITE] = new IPEndPoint(m_IPAddress, _intWriteRemotePort);
            m_TcpClient[M_READ_] = new TcpClient();
            m_TcpClient[M_WRITE] = new TcpClient();

            m_TcpClient[M_READ_].Connect(m_IPEndPoint[M_READ_]);
            m_TcpClient[M_WRITE].Connect(m_IPEndPoint[M_WRITE]);

       //     m_TcpClient[M_READ_].BeginConnect(m_IPEndPoint[M_READ_].Address, m_IPEndPoint[M_READ_].Port, null, null); // (IPAddress address, int port, AsyncCallback requestCallback, object state);
      //      m_TcpClient[M_WRITE].BeginConnect(m_IPEndPoint[M_WRITE].Address, m_IPEndPoint[M_WRITE].Port, null, null);


            //       m_TcpClient[M_READ_].Connect(m_IPAddress, _intReadLocalPort); //_intReadLocalPort
            //       m_TcpClient[M_WRITE].Connect(m_IPAddress, _intWriteLocalPort);



            m_TcpClient[M_READ_].Client.ReceiveTimeout = _intReadRecTimeOut;
            m_TcpClient[M_WRITE].Client.ReceiveTimeout = 10000;

            PortOpenConfirm = true;
            if (PortOpenConfirm) flag = true;
            return flag;
        }

        public bool ReadSocketConnect
        {

            get
            {
                bool _return = false;
                try
                {
                    //   return this.m_TcpClient[M_READ_].Client.Connected;
                      _return =  this.m_TcpClient[M_READ_].Client.Connected;

                //    if (!this.m_TcpClient[M_READ_].AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(this.connectionTimeout), false))





           //         this.m_TcpClient[M_READ_].Client.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.UpdateConnectContext);
                    _return = true;
                }
                catch (System.Exception ex)
                {
                    _return =  false;
                }

                return _return;
            }
        }

        public bool CheckConnected(Socket s)
        {
            bool flag = true;
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
                flag = s.IsBound;
            return flag;
        }

        public bool WriteSocketConnect
        {
            get
            {
                bool _return = false;
                try
                {
                    //   return this.m_TcpClient[M_WRITE].Client.Connected;
                    _return = this.m_TcpClient[M_WRITE].Client.Connected;
                }
                catch (System.Exception ex)
                {
                    _return = false;
                }

                return _return;
            }
        }

        //---------------------------------------------------------------------------------------------------------------
        public int ReadDevice_W(string nAddress, int Length, ref int[] returnValue)
        {
            if (!CheckConnected(m_TcpClient[M_READ_].Client))
            {
                m_TcpClient[M_READ_].Close();
                m_TcpClient[M_READ_] = new TcpClient();
                //m_TcpClient[M_READ_].Connect(m_IPEndPoint[M_READ_]);      //shkang
            }


             int nSSize = 21; //nStatementSize

            int nSendCount = Length;

            int nSendTotalLength = nSSize;
            int nReqLength = 12;

            byte[] datagram = new byte[nSendTotalLength];
            //-------------------------nSendTotalLength는 이거 밑으로 갯수-------------------------------------
            datagram[0] = 0x50; //서브헤더
            datagram[1] = 0x00; //서브헤더
            datagram[2] = 0x00; //네트워크번호
            datagram[3] = 0xFF; //PLC 번호
            datagram[4] = 0xFF; //I/O 번호
            datagram[5] = 0x03; //요구 상대 모듈
            datagram[6] = 0x00; //요구 상대 모듈 국번호
            //---------------------------------------------------------------------------
            datagram[7]  = (byte)((nReqLength) & 0xff); //요구데이터 개수 L 
            datagram[8]  = (byte)((nReqLength >> 8) & 0xFF); //요구데이터 개수 H 
            //-------------------------nReqLength는 이거 밑으로 갯수-------------------------------------------
            datagram[9]  = 0x0A; //(CPU감시 타임 L) //10
            datagram[10] = 0x00; //(CPU감시 타임 H)
            datagram[11] = 0x01; //(커맨드 L)                //L
            datagram[12] = 0x04; //(커맨드 H) 0x14           //H
            datagram[13] = 0x00; //(서브커맨드 L)            //L
            datagram[14] = 0x00; //(서브커맨드 H)            //H

            datagram[15] = (byte)((Int32.Parse(nAddress.Remove(0, 1), GetNumberStyles(nAddress.Substring(0, 1)))) & 0xFF);   //디바이스 번지 L
            datagram[16] = (byte)((Int32.Parse(nAddress.Remove(0, 1), GetNumberStyles(nAddress.Substring(0, 1))) >> 8) & 0xFF);   //디바이스 번지 -
            datagram[17] = (byte)((Int32.Parse(nAddress.Remove(0, 1), GetNumberStyles(nAddress.Substring(0, 1))) >> 16) & 0xFF);   //디바이스 번지 H
            datagram[18] = (byte)(GetDeviceCode(nAddress.Substring(0, 1)));                                      //디바이스 코드
            datagram[19] = (byte)(Length & 0xFF);                            //쓰는갯수 L
            datagram[20] = (byte)((Length >> 8) & 0xFF);                     //쓰는갯수 H




            int num9 = 0;
            try
            {
                this.m_bReadPLCFlag = true;
                num9 = this.m_TcpClient[M_READ_].Client.Send(datagram, nSendTotalLength ,SocketFlags.None);


                if (this.m_bReadPLCFlag)
                {
                    //      IPEndPoint remoteEP = null;
                    int nReceiveStatement = 11;

                    byte[] buffer2 = new byte[nReceiveStatement + Length * 2];

                    int ReceiveCount = this.m_TcpClient[M_READ_].Client.Receive(buffer2);
                    int index = 0;

                    int[] numArray = new int[Length];
                    this.ReturnLength = 0;
                    
                    for (int i = 0; i < Length; i++)
                    {
                        numArray[i] = buffer2[nReceiveStatement + index + 1] << 8;
                        numArray[i] += (Int16)buffer2[nReceiveStatement + index];
                        index += 2;
                        this.ReturnLength = i + 1;
                    }
                    this.ReceiveResult = numArray;
                    returnValue = numArray;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                if (num9 <= 0)
                {
                    MessageBox.Show("전송오류");
                }
            }
            return this.ReturnLength;
        }
        public int WriteDevice_W(string nAddress, int Length, int[] SetValue)
        {
            if (m_TcpClient == null)
                return -1;
            if (!CheckConnected(m_TcpClient[M_WRITE].Client))
            {
                m_TcpClient[M_WRITE].Close();
                m_TcpClient[M_WRITE] = new TcpClient();
                m_TcpClient[M_WRITE].Connect(m_IPEndPoint[M_WRITE]);
            }

            int nDataArraySize = 2;
            int nSSize = 21; //nStatementSize


            int nSendCount = Length;
            int nSendTotalLength = nSSize + (nSendCount * nDataArraySize);
            int nReqLength = 12 + (nSendCount * nDataArraySize);


            byte[] datagram = new byte[nSendTotalLength];
            //-------------------------nSendTotalLength는 이거 밑으로 갯수-------------------------------------
            datagram[0] = 0x50; //서브헤더
            datagram[1] = 0x00; //서브헤더
            datagram[2] = 0x00; //네트워크번호
            datagram[3] = 0xFF; //PLC 번호
            datagram[4] = 0xFF; //I/O 번호
            datagram[5] = 0x03; //요구 상대 모듈
            datagram[6] = 0x00; //요구 상대 모듈 국번호
            //---------------------------------------------------------------------------
            datagram[7] = (byte)((nReqLength) & 0xff); //요구데이터 개수 L 
            datagram[8] = (byte)((nReqLength >> 8) & 0xFF); //요구데이터 개수 H 
            //-------------------------nReqLength는 이거 밑으로 갯수-------------------------------------------
            datagram[9] = 0x0A; //(CPU감시 타임 L) //10
            datagram[10] = 0x00; //(CPU감시 타임 H)

            datagram[11] = 0x01; //(커맨드 L)                //L
            datagram[12] = 0x14; //(커맨드 H) 0x14           //H
            datagram[13] = 0x00; //(서브커맨드 L)            //L
            datagram[14] = 0x00; //(서브커맨드 H)            //H

            datagram[15] = (byte)((Int32.Parse(nAddress.Remove(0, 1), GetNumberStyles(nAddress.Substring(0, 1)))) & 0xFF);   //디바이스 번지 L
            datagram[16] = (byte)((Int32.Parse(nAddress.Remove(0, 1), GetNumberStyles(nAddress.Substring(0, 1))) >> 8) & 0xFF);   //디바이스 번지 -
            datagram[17] = (byte)((Int32.Parse(nAddress.Remove(0, 1), GetNumberStyles(nAddress.Substring(0, 1))) >> 16) & 0xFF);   //디바이스 번지 H
            datagram[18] = (byte)(GetDeviceCode(nAddress.Substring(0, 1)));                                      //디바이스 코드
            datagram[19] = (byte)(Length & 0xFF);                            //쓰는갯수 L
            datagram[20] = (byte)((Length >> 8) & 0xFF);                     //쓰는갯수 H

            //----------------------------------------------------------------------------
            for (int i = 0; i < nSendCount; i++)
            {
                datagram[(nSSize + (i * nDataArraySize)) + 00] = (byte)(SetValue[i] & 0xFF);                            //데이터 L
                datagram[(nSSize + (i * nDataArraySize)) + 01] = (byte)((SetValue[i] >> 8) & 0xFF);                     //데이터 H
            }
            this.m_TcpClient[M_WRITE].Client.Send(datagram, nSendTotalLength, SocketFlags.None);
            int nReturn = 0;

            try
            {
                //                 IPEndPoint remoteEP = null;
                //                 byte[] buffer2 = new byte[Length];

     //           this.m_TcpClient[M_READ_].Client.Send(datagram, nSendTotalLength, SocketFlags.None);
                int nReceiveStatement = 11;
                byte[] buffer2 = new byte[nReceiveStatement/* + Length  * sizeof(short)*/];
               int ReceiveCount =  this.m_TcpClient[M_WRITE].Client.Receive(buffer2);
                nReturn = buffer2[10] << 8;
                nReturn += (Int16)buffer2[9];
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                nReturn = -1;
            }
            this.ThreadConfrim = true;
            return nReturn;
        }


#if false
        public int ReadDeviceRandom_W(string[] nAddress, int Length, ref int[] returnValue)
        {
            int nDataArraySize = 4;
            int nSSize = 17; //nStatementSize


            int nSendCount = Length;
            int nSendTotalLength = nSSize + (nSendCount * nDataArraySize);
            int nReqLength = 8 + (nSendCount * nDataArraySize);

            byte[] datagram = new byte[nSendTotalLength];
            //-------------------------nSendTotalLength는 이거 밑으로 갯수-------------------------------------
            datagram[0] = 0x50; //서브헤더
            datagram[1] = 0x00; //서브헤더
            datagram[2] = 0x00; //네트워크번호
            datagram[3] = 0xFF; //PLC 번호
            datagram[4] = 0xFF; //I/O 번호
            datagram[5] = 0x03; //요구 상대 모듈
            datagram[6] = 0x00; //요구 상대 모듈 국번호
            //---------------------------------------------------------------------------
            datagram[7] = (byte)((nReqLength) & 0xff); //요구데이터 개수 L 
            datagram[8] = (byte)((nReqLength >> 8) & 0xFF); //요구데이터 개수 H 
            //-------------------------nReqLength는 이거 밑으로 갯수-------------------------------------------
            datagram[9] = 0x0A; //(CPU감시 타임 L) //10
            datagram[10] = 0x00; //(CPU감시 타임 H)
            datagram[11] = 0x03; //(커맨드 L)                //L
            datagram[12] = 0x04; //(커맨드 H) 0x14           //H
            datagram[13] = 0x00; //(서브커맨드 L)            //L
            datagram[14] = 0x00; //(서브커맨드 H)            //H

            datagram[15] = (byte)nSendCount;           //워드액세스 개수
            datagram[16] = 0x00;                       //더블워드액세스 개수
            //----------------------------------------------------------------------------
            for (int i = 0; i < nSendCount; i++)
            {
                datagram[(nSSize + (i * nDataArraySize)) + 00] = (byte)((Int32.Parse(nAddress[i].Remove(0, 1), GetNumberStyles(nAddress[i].Substring(0, 1)))) & 0xFF);   //디바이스 번지 L
                datagram[(nSSize + (i * nDataArraySize)) + 01] = (byte)((Int32.Parse(nAddress[i].Remove(0, 1), GetNumberStyles(nAddress[i].Substring(0, 1))) >> 8) & 0xFF);   //디바이스 번지 -
                datagram[(nSSize + (i * nDataArraySize)) + 02] = (byte)((Int32.Parse(nAddress[i].Remove(0, 1), GetNumberStyles(nAddress[i].Substring(0, 1))) >> 16) & 0xFF);   //디바이스 번지 H

                datagram[(nSSize + (i * nDataArraySize)) + 03] = (byte)(GetDeviceCode(nAddress[i].Substring(0, 1)));                                      //디바이스 코드
            }
            int num9 = 0;
            try
            {
                this.m_bReadPLCFlag = true;
                num9 = this.m_UdpClient[M_READ_].Client.Send(datagram, nSendTotalLength , SocketFlags.None);


                if (this.m_bReadPLCFlag)
                {
                    IPEndPoint remoteEP = null;
                    byte[] buffer2 = new byte[Length];
                    this.m_UdpClient[M_READ_].Client.Receive(buffer2);
                    int index = 0;

                    int[] numArray = new int[Length];
                    this.ReturnLength = 0;
                    int nReceiveStatement = 11;
                    for (int i = 0; i < Length; i++)
                    {
                        numArray[i] = buffer2[nReceiveStatement + index + 1] << 8;
                        numArray[i] += (Int16)buffer2[nReceiveStatement + index];
                        index += 2;
                        this.ReturnLength = i + 1;
                    }
                    this.ReceiveResult = numArray;
                    returnValue = numArray;
                }
            }
            catch
            {
                if (num9 <= 0)
                {
                    MessageBox.Show("전송오류");
                }
            }
            return this.ReturnLength;
        }
        public int WriteDeviceRandom_W(string[] nAddress, int Length, int[] SetValue)
        {
           
            int nDataArraySize = 6;
            int nSSize = 17; //nStatementSize
       

            int nSendCount = Length;
            int nSendTotalLength = nSSize + (nSendCount * nDataArraySize);
            int nReqLength = 8 + (nSendCount * nDataArraySize);
            

            byte[] datagram = new byte[nSendTotalLength];
            //-------------------------nSendTotalLength는 이거 밑으로 갯수-------------------------------------
            datagram[0] = 0x50; //서브헤더
            datagram[1] = 0x00; //서브헤더
            datagram[2] = 0x00; //네트워크번호
            datagram[3] = 0xFF; //PLC 번호
            datagram[4] = 0xFF; //I/O 번호
            datagram[5] = 0x03; //요구 상대 모듈
            datagram[6] = 0x00; //요구 상대 모듈 국번호
            //---------------------------------------------------------------------------
            datagram[7] = (byte)((nReqLength )     & 0xff); //요구데이터 개수 L 
            datagram[8] = (byte)((nReqLength >> 8) & 0xFF); //요구데이터 개수 H 
            //-------------------------nReqLength는 이거 밑으로 갯수-------------------------------------------
            datagram[9]  = 0x0A; //(CPU감시 타임 L) //10
            datagram[10] = 0x00; //(CPU감시 타임 H)
            datagram[11] = 0x02; //(커맨드 L)                //L
            datagram[12] = 0x14; //(커맨드 H) 0x14           //H
            datagram[13] = 0x00; //(서브커맨드 L)            //L
            datagram[14] = 0x00; //(서브커맨드 H)            //H
            datagram[15] = (byte)nSendCount;           //워드액세스 개수
            datagram[16] = 0x00;                       //더블워드액세스 개수
            //----------------------------------------------------------------------------
            for (int i = 0; i < nSendCount; i++)
            {
                datagram[(nSSize + (i * nDataArraySize)) + 00] = (byte)((Int32.Parse(nAddress[i].Remove(0, 1), GetNumberStyles(nAddress[i].Substring(0, 1)) )) & 0xFF);   //디바이스 번지 L
                datagram[(nSSize + (i * nDataArraySize)) + 01] = (byte)((Int32.Parse(nAddress[i].Remove(0, 1), GetNumberStyles(nAddress[i].Substring(0, 1)) ) >> 8) & 0xFF);   //디바이스 번지 -
                datagram[(nSSize + (i * nDataArraySize)) + 02] = (byte)((Int32.Parse(nAddress[i].Remove(0, 1), GetNumberStyles(nAddress[i].Substring(0, 1)) ) >> 16) & 0xFF);   //디바이스 번지 H
                datagram[(nSSize + (i * nDataArraySize)) + 03] = (byte)(GetDeviceCode(nAddress[i].Substring(0, 1)));                                      //디바이스 코드
   
                datagram[(nSSize + (i * nDataArraySize)) + 04] = (byte)(SetValue[i] & 0xFF);                            //데이터 L
                datagram[(nSSize + (i * nDataArraySize)) + 05] = (byte)((SetValue[i] >> 8) & 0xFF);                     //데이터 H
            }
            this.m_UdpClient[M_WRITE].Client.Send(datagram, nSendTotalLength, SocketFlags.None  );
            int nReturn = 0;

            try
            {
                this.m_UdpClient[M_READ_].Client.Send(datagram, nSendTotalLength, SocketFlags.None);
                IPEndPoint remoteEP = null;
                byte[] buffer2 = new byte[Length];
                    
                this.m_UdpClient[M_READ_].Client.Receive(buffer2);
                nReturn = buffer2[10] << 8;
                nReturn += (Int16)buffer2[9];
            }
            catch
            {
                nReturn = -1;
            }
            this.ThreadConfrim = true;
            Thread.Sleep(1);
            return nReturn;
        }

#endif
        private byte GetDeviceCode(string DeviceCode)
        {
            byte nRetValue = 0;

            switch(DeviceCode)          
            {
                case "L":  nRetValue = 0x92; break;
                case "F":  nRetValue = 0x93; break;
                case "V":  nRetValue = 0x94; break;
                case "TS": nRetValue = 0xC1; break;
                case "TC": nRetValue = 0xC0; break;
                case "TN": nRetValue = 0xC2; break;
                case "SS": nRetValue = 0xC7; break;
                case "SC": nRetValue = 0xC6; break;
                case "SN": nRetValue = 0xC8; break;
                case "CS": nRetValue = 0xC4; break;
                case "CC": nRetValue = 0xC4; break;
                case "CN": nRetValue = 0xC5; break;
                case "S":  nRetValue = 0x98; break;
                case "DX": nRetValue = 0xA2; break;
                case "DY": nRetValue = 0xA3; break;
                case "Z":  nRetValue = 0xCC; break;
                case "R":  nRetValue = 0xAF; break;
                case "ZR": nRetValue = 0xB0; break;
                //-----------------------------------------------MELSECNET
                case "SM": nRetValue = 0x91; break;
                case "SD": nRetValue = 0xA9; break;
                case "X":  nRetValue = 0x9C; break;
                case "Y":  nRetValue = 0x9D; break;
                case "M":  nRetValue = 0x90; break;
                case "B":  nRetValue = 0xA0; break;
                case "D":  nRetValue = 0xA8; break;
                case "W":  nRetValue = 0xB4; break;
                case "SB": nRetValue = 0xA1; break;
                case "SW": nRetValue = 0xB5; break;
                //------------------------------------------------------------
                case "null":
                    nRetValue = 0xA8; //"D"
                    break;
            }
            return nRetValue;
        }
        private System.Globalization.NumberStyles GetNumberStyles(string DeviceCode)
        {
            System.Globalization.NumberStyles nRetValue = System.Globalization.NumberStyles.AllowDecimalPoint;

            switch (DeviceCode)
            {
                case "L": 
                case "F": 
                case "V": 
                case "TS":
                case "TC":
                case "TN":
                case "SS":
                case "SC":
                case "SN":
                case "CS":
                case "CC":
                case "CN":
                case "S": 
                case "Z": 
                case "R": 
                case "M": 
                case "D": 
                case "SM":
                case "SD":
                    nRetValue = System.Globalization.NumberStyles.AllowDecimalPoint;
                    break;      
                //-----------------------------------------------MELSECNET
                case "DX":
                case "DY":
                case "SB":
                case "SW":
                case "B": 
                case "W": 
                case "X": 
                case "Y": 
                case "ZR":
                    nRetValue = System.Globalization.NumberStyles.HexNumber;
                    break;

                //------------------------------------------------------------
                case "null":
                    nRetValue = System.Globalization.NumberStyles.AllowDecimalPoint;
                    break;
            }
            return nRetValue;
        }
        //---------------------------------------------------------------------------------------------------------------
    }

}
