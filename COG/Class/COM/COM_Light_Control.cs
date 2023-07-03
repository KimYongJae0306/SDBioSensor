using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace COG
{
    public partial class Main
    {
        public static int[,] LightCurrent = new int[8, 20];
        public static int[] WhiteRefCurrent = new int[DEFINE.Board_Max * 8];
        public static int[] BlackRefCurrent = new int[DEFINE.Board_Max * 8];

        public partial class PatternTag
        {
            public int[,] m_LightValue = new int[Main.DEFINE.Light_PatMaxCount,Main.DEFINE.Light_ToolMaxCount]; //[조명번호,서브패턴]


            public int[] m_LightCtrl = new int[Main.DEFINE.Light_PatMaxCount];
            public int[] m_LightCH = new int[Main.DEFINE.Light_PatMaxCount];
            public string[] m_Light_Name = new string[Main.DEFINE.Light_PatMaxCount];
        }
    }

    class Light
    {
        private static SerialPort[] LightController = new SerialPort[Main.DEFINE.Light_Control_Max];
        //    private static string resultMessage;
        private static Main.MTickTimer LightTimer = new Main.MTickTimer();


        public static void Port_Initial()
        {
            if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM) return;
            for (int i = 0; i < Main.DEFINE.Light_Control_Max; i++)
            {
                PortSetting(i, 9600);
                Port_Open(i);
            }
        }
        public static void Port_Close()
        {
            for (int i = 0; i < Main.DEFINE.Light_Control_Max; i++)
            {
                Port_Close(i);
            }
        }


        public static void Port_Refresh()
        {
            for (int i = 0; i < Main.DEFINE.Light_Control_Max; i++)
            {
                LightControl_ONOFF(i, Main.DEFINE.M_CONTROL_ON);
            }
        }

        public static void LightControl_ONOFF(int portName , string nType)
        {
            string m_sendData;
            //jyh
            for (int j = 0; j < 3; j++) // 1~8CH
            {
                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                {
                    byte[] commandCode = new byte[6];
                    int Channel = 0x01 << j;

                    commandCode[0] = Main.DEFINE.LVS_LIGHT_CMD_START;           //START
                    commandCode[1] = Main.DEFINE.LVS_LIGHT_CMD_WRITE;           //OP Code
                    commandCode[2] = 0x01;                                      //Data length
                    commandCode[3] = Main.DEFINE.LVS_LIGHT_CMD_RIGISTER_CSR;    //SET Channel Rigister 
                    commandCode[4] = Convert.ToByte(Channel);                         //channel bits
                    commandCode[5] = Main.DEFINE.LVS_LIGHT_CMD_END;             //END

                    Light.Write(portName, commandCode, 0, commandCode.Length);

                    commandCode[0] = Main.DEFINE.LVS_LIGHT_CMD_START;           //START
                    commandCode[1] = Main.DEFINE.LVS_LIGHT_CMD_WRITE;           //OP Code
                    commandCode[2] = 0x01;                                      //Data length
                    commandCode[3] = Main.DEFINE.LVS_LIGHT_CMD_RIGISTER_SVR;    //SET VALUE Rigister 
                    commandCode[5] = Main.DEFINE.LVS_LIGHT_CMD_END;             //END

                    if (nType == Main.DEFINE.M_CONTROL_ON)
                    {
                        commandCode[4] = Convert.ToByte(50);                     //value bits
                        Light.Write(portName, commandCode, 0, commandCode.Length);
                        Thread.Sleep(100);
                    }

                    commandCode[4] = Convert.ToByte(0);                         //value bits
                    Light.Write(portName, commandCode, 0, commandCode.Length);
                }
                else
                {
                    m_sendData = "]" + j.ToString("D2") + nType;
                    Write(portName, m_sendData);
                }
            }
        }

        private static void PortSetting(int portName, int baudRate)
        {
            int com;
            if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                com = portName + 2;
            else
                com = portName + 1;

            try
            {
                LightController[portName] = new SerialPort("COM" + com.ToString(), baudRate, Parity.None, 8, StopBits.One);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
            }
        }

        public static bool LightControl_ReadTest(int portName)
        {
            if (!LightController[portName].IsOpen)
                return false;

            int readCnt = 0;
            byte[] recvBuf = new byte[128];

            if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
            {
                byte[] commandCode = new byte[5];

                commandCode[0] = Main.DEFINE.LVS_LIGHT_CMD_START;           //START
                commandCode[1] = Main.DEFINE.LVS_LIGHT_CMD_READ;           //OP Code
                commandCode[2] = 0x01;                                      //Data length
                commandCode[3] = Main.DEFINE.LVS_LIGHT_CMD_RIGISTER_RTR;    //SET Channel Rigister 
                commandCode[4] = Main.DEFINE.LVS_LIGHT_CMD_END;             //END

                try
                {
                    Light.Write(portName, commandCode, 0, commandCode.Length);

                    LightTimer.StartTimer();
                    while (LightTimer.GetElapsedTime() < Main.DEFINE.LVS_LIGHT_RESP_TIMEOUT)
                    {
                        readCnt = LightController[portName].Read(recvBuf, 0, 128);
                        if (recvBuf[4] == Main.DEFINE.LVS_LIGHT_CMD_RTR_DATA || recvBuf[5] == Main.DEFINE.LVS_LIGHT_CMD_RTR_DATA)
                            return true;
                        else
                            return false;
                    }
                }
                catch (Exception e)
                {

                }
            }

            return false;
        }

        private static bool Port_Open(int portName)
        {
            string m_sendData;
            int com;
            com = portName + 1;
            try
            {
                if (LightController[portName].IsOpen)
                    LightController[portName].Close();
                else
                    LightController[portName].Open();
            }
            catch (System.Exception ex)
            {
                if (!LightController[portName].IsOpen)
                {
                    MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                    //                     resultMessage = "Light" + com.ToString() + " isn't Open.";
                    //                     MessageBox.Show(resultMessage);
                    return false;
                }
            }
            finally
            {
                LightControl_ONOFF(portName, Main.DEFINE.M_CONTROL_ON);
            }
            return true;
        }

        private static void Port_Close(int portName)
        {
            try
            {
                LightControl_ONOFF(portName, Main.DEFINE.M_CONTROL_OFF);
                LightController[portName].Close();
            }
            catch
            {
            }
        }

        public static void Write(int portName, string SendData, bool bRetry = false)
        {
            if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM) return;

            try
            {
                LightController[portName].Write(SendData);

                bool bRet = false;
                if (bRetry)
                {
                    int readCnt = 0;
                    byte[] recvBuf = new byte[128];
                    LightTimer.StartTimer();
                    while (LightTimer.GetElapsedTime() < Main.DEFINE.LVS_LIGHT_RESP_TIMEOUT)
                    {
                        readCnt = LightController[portName].Read(recvBuf, 0, 128);
                        if (recvBuf[0] == Main.DEFINE.LVS_LIGHT_CMD_ACK)
                            bRet = true;
                    }
                    if (!bRet)
                        LightController[portName].Write(SendData);
                }
                //string strLog = "#" + portName.ToString() + " >>";
                //strLog += SendData;
                //if (bRet) strLog += " (Retried)";
                //Save_SystemLog(strLog, Main.DEFINE.LIGHTCTRL);
            }
            catch
            {
            }
            finally
            {
            }
        }

        public static void Write(int portName, byte[] SendData, int offset, int length, bool bRetry = false)
        {
            if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM) return;

            try
            {
                LightController[portName].Write(SendData, offset, length);

                bool bRet = false;
                if (bRetry)
                {
                    int readCnt = 0;
                    byte[] recvBuf = new byte[128];
                    
                    LightTimer.StartTimer();
                    while (LightTimer.GetElapsedTime() < Main.DEFINE.LVS_LIGHT_RESP_TIMEOUT)
                    {
                        readCnt = LightController[portName].Read(recvBuf, 0, 128);
                        if (recvBuf[0] == Main.DEFINE.LVS_LIGHT_CMD_ACK)
                            bRet = true;
                    }
                    if (!bRet)
                        LightController[portName].Write(SendData, offset, length);
                }

                //string strLog = "#" + portName.ToString() + " >>";
                ////strLog += Encoding.UTF8.GetString(SendData);  // byte array to string
                //strLog += BitConverter.ToString(SendData).Replace("-", ""); // byte array to hexadecimal string
                //if (bRet) strLog += " (Retried)";
                //Save_SystemLog(strLog, Main.DEFINE.LIGHTCTRL);
            }
            catch
            {
            }
            finally
            {
            }
        }

        static object syncLock_LightLog = new object();
        private static void Save_SystemLog(string nMessage, string nType)
        {
            string nFolder;
            string nFileName = "";
            nFolder = Main.LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\";
            if (!Directory.Exists(Main.LogdataPath)) Directory.CreateDirectory(Main.LogdataPath);
            if (!Directory.Exists(nFolder)) Directory.CreateDirectory(nFolder);

            string Date;
            Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");

            lock (syncLock_LightLog)
            {
                try
                {
                    switch (nType)
                    {
                        case Main.DEFINE.LIGHTCTRL:
                            nFileName = "CommsLog.txt";
                            nMessage = Date + nMessage;
                            break;
                    }

                    StreamWriter SW = new StreamWriter(nFolder + nFileName, true, Encoding.Unicode);
                    SW.WriteLine(nMessage);
                    SW.Close();
                }
                catch
                {

                }
            }
        }

    }// Light

}
