using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JAS.Interface.localtime;
using System.IO;
using System.Windows.Forms;
namespace COG
{
     public partial class Main
    {
        //-----------------------------------------------------Thread 관련-----------------------------------------------------------------
        private static bool threadFlag;
        private static bool bShowingPMMode = false;
        private static Form_PMMode FormPMMode = new Form_PMMode();

        private static List<Thread> AlignUnitThread = new List<Thread>();
        private static List<Thread> CAMThread = new List<Thread>();

        private static MTickTimer m_Timer = new MTickTimer();
        private static MTickTimer m_AliveTimer = new MTickTimer();
        public static MTickTimer TimerRunningCheck = new MTickTimer();
        public static bool bFirstStartRun = false;

        SetlocalTime.SYSTEMTIME _SetTime = new SetlocalTime.SYSTEMTIME();

        #region Thread Define
        private static Thread ThreadCAM0;
        private static Thread ThreadCAM1;

        private static Thread ThreadProcM;
        //private static Thread ThreadWatchCCLinkMachine;
        private static Thread ThreadPLCRead;
        private static Thread ThreadPLCAlive;

        private static Thread ThreadProc0;
        private static Thread ThreadProc1;
        private static Thread ThreadProc2;


        private static Thread ThreadProcDir;

        #endregion

        #region CAM관련 Thread
        public static void ThreadCAM_Initial_Start()
        {
            ThreadCAM0 = new Thread(new ThreadStart(ThreadCAM_0));
            ThreadCAM1 = new Thread(new ThreadStart(ThreadCAM_1));
            //ThreadCAM2 = new Thread(new ThreadStart(ThreadCAM_2));
            //ThreadCAM3 = new Thread(new ThreadStart(ThreadCAM_3));

            //ThreadCAM4 = new Thread(new ThreadStart(ThreadCAM_4));
            //ThreadCAM5 = new Thread(new ThreadStart(ThreadCAM_5));
            //ThreadCAM6 = new Thread(new ThreadStart(ThreadCAM_6));
            //ThreadCAM7 = new Thread(new ThreadStart(ThreadCAM_7));
            //ThreadCAM8 = new Thread(new ThreadStart(ThreadCAM_8));
            //ThreadCAM9 = new Thread(new ThreadStart(ThreadCAM_9));
            //ThreadCAM10 = new Thread(new ThreadStart(ThreadCAM_10));
            //ThreadCAM11 = new Thread(new ThreadStart(ThreadCAM_11));

            CAMThread.Add(ThreadCAM0);
            CAMThread.Add(ThreadCAM1);
            //CAMThread.Add(ThreadCAM2);
            //CAMThread.Add(ThreadCAM3);

            //CAMThread.Add(ThreadCAM4);
            //CAMThread.Add(ThreadCAM5);
            //CAMThread.Add(ThreadCAM6);
            //CAMThread.Add(ThreadCAM7);
            //CAMThread.Add(ThreadCAM8);
            //CAMThread.Add(ThreadCAM9);
            //CAMThread.Add(ThreadCAM10);
            //CAMThread.Add(ThreadCAM11);

            for (int i = 0; i < Main.DEFINE.CAM_MAX; i++)
            {
                CAMThread[i].SetApartmentState(ApartmentState.STA);
                CAMThread[i].Start();
            }
        }
        public static void ThreadCAM_Stop()
        {
            threadFlag = false;
            Thread.Sleep(500);
            int count = CAMThread.Count;
            for (int i = 0; i < count; i++)
            {
                if (CAMThread[i] != null)
                {
                    if (CAMThread[i].IsAlive) CAMThread[i].Abort();
                }

            }
        }
        private static void ThreadCAM_0()
        {
            while (threadFlag)
            {
                if (Main.vision.Grab_Flag_Start[0] == true)
                {
                    while (true)
                    {
                        if (ImageGrab(0)) break;
                    }
                }
                Thread.Sleep(50);
            }
        }
        private static void ThreadCAM_1()
        {
            while (threadFlag)
            {
                if (Main.vision.Grab_Flag_Start[1] == true)
                {
                    while (true)
                    {
                        if (ImageGrab(1)) break;
                    }
                }
                Thread.Sleep(50);
            }
        }
        #endregion

        #region Main Thread & AlignUnit Thread & PLC Read Thread
        public static void Thread_Initial_Start()
        {
            threadFlag = true;

            ThreadProcM = new Thread(new ThreadStart(ThreadProc_MMM));
            ThreadPLCRead = new Thread(new ThreadStart(ThreadPLC_Read));
            ThreadPLCAlive = new Thread(new ThreadStart(ThreadPLC_Alive));

            ThreadProcM.SetApartmentState(ApartmentState.STA);
            ThreadPLCRead.SetApartmentState(ApartmentState.STA);
            ThreadPLCAlive.SetApartmentState(ApartmentState.STA);
            ThreadProcM.Start();
            ThreadPLCRead.Start();
            ThreadPLCAlive.Start();
            ThreadProcDir = new Thread(new ThreadStart(ThreadDIR_Delete));
            ThreadProcDir.SetApartmentState(ApartmentState.STA);
            ThreadProcDir.Start();



            ThreadProc0 = new Thread(new ThreadStart(ThreadProc_0));
            ThreadProc1 = new Thread(new ThreadStart(ThreadProc_1));
            ThreadProc2 = new Thread(new ThreadStart(ThreadProc_2));

            AlignUnitThread.Add(ThreadProc0);
            AlignUnitThread.Add(ThreadProc1);
            AlignUnitThread.Add(ThreadProc2);

            //2022 11 12 10분이상 비가동 시 조명 Off 스레드 동작위해 임의로 +1 추가
            for (int i = 0; i < Main.DEFINE.AlignUnit_Max + 1; i++)
            {
                AlignUnitThread[i].SetApartmentState(ApartmentState.STA);
                AlignUnitThread[i].Start();
            }
        }
        public static void Thread_Stop()
        {
            threadFlag = false;
            Thread.Sleep(500);
            if (ThreadProcM != null)
            {
                if (ThreadProcM.IsAlive) ThreadProcM.Abort();
            }

            if (ThreadPLCRead != null)
            {
                if (ThreadPLCRead.IsAlive) ThreadPLCRead.Abort();
            }
            if (ThreadProcDir != null)
            {
                if (ThreadProcDir.IsAlive) ThreadProcDir.Abort();
            }
            //-----------------------------------------------------------------
            for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
            {
                if (AlignUnitThread[i] != null)
                {
                    if (AlignUnitThread[i].IsAlive) AlignUnitThread[i].Abort();
                }
            }
            //-------------------------------------------------

            Thread.Sleep(500);
        }

        private static void ThreadProc_MMM()
        {

            while (threadFlag)
            {
                if (Status.MC_STATUS == DEFINE.MC_RUN)
                {
                    int nCmd, nstatus, nSkipCmd;
                    string LogMsg=" ";
                    nCmd = PLCDataTag.RData[DEFINE.PLC_CMD];
                    nSkipCmd = PLCDataTag.RData[DEFINE.PLC_VISION_SKIP_MODE];
                    nstatus = PLCDataTag.RData[DEFINE.VIS_STATUS];
                    int[] setValue = new int[1];
                    if (nstatus == 0 && nCmd != 0)
                    {
                        switch (nCmd)
                        {
                            case 6000:

                                SetlocalTime.SYSTEMTIME _SetTime = new SetlocalTime.SYSTEMTIME();
                                _SetTime = SetlocalTime.GetTime();
                                _SetTime.wYear      = (ushort)PLCDataTag.RData[Main.DEFINE.YEAR + 0];   //(ushort)
                                _SetTime.wMonth     = (ushort)PLCDataTag.RData[Main.DEFINE.YEAR + 1];   //(ushort)
                                _SetTime.wDay       = (ushort)PLCDataTag.RData[Main.DEFINE.YEAR + 2];   //(ushort)
                                _SetTime.wHour      = (ushort)PLCDataTag.RData[Main.DEFINE.YEAR + 3];   //(ushort)
                                _SetTime.wMinute    = (ushort)PLCDataTag.RData[Main.DEFINE.YEAR + 4];   //(ushort)
                                _SetTime.wSecond    = (ushort)PLCDataTag.RData[Main.DEFINE.YEAR + 5];   //(ushort)

//                                _SetTime.wHour = (ushort)(_SetTime.wHour % 24);
                                 int _ErrorCode = 0;
                                 if (SetlocalTime.SetLocalTime_(_SetTime, ref _ErrorCode))
                                 {
                                     LogMsg = "LocalTime Changed OK";
                                 }
                                 else
                                 {                              
                                     LogMsg = "LocalTime Changed NG" + " ,ErrorCode:"+_ErrorCode.ToString();
                                 }
                                Main.AlignUnit[0].LogdataDisplay(LogMsg, true);
                                Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.PLC_CMD, 0);
                                CmdCheck();
                                break;
                            case 8000:  //에러처리함
                                 if (ProjectLoad(PLCDataTag.RData[DEFINE.PLC_MODEL_CODE].ToString("000")))
                                 {                                
                                    Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.PLC_CMD, 0);
                                    Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.VIS_STATUS, nCmd);
                                    LogMsg = "MODEL: " + ProjectName + ProjectInfo + " LOAD OK";
                                 }
                                 else
                                 {
                                    Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.PLC_CMD, 0);
                                    Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.VIS_STATUS, -nCmd);
                                    LogMsg = "MODEL: " + PLCDataTag.RData[DEFINE.PLC_MODEL_CODE].ToString("000") + " LOAD NG";
                                    MessageBox.Show("Don't Model File , Please Make a ModelFile");
                                 }
                                 LogMsg = "<- " + LogMsg;
                                 Main.AlignUnit[0].LogdataDisplay(LogMsg, true);
                                 CmdCheck();
                                break;


                            case 9000:
                                for (int i = 0; i < DEFINE.AlignUnit_Max; i++)
                                {
                                    AlignUnit[i].ClearPlcCmd();
                                }
                                Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.PLC_CMD, 0);
                                CmdCheck();
                                break;   
                         }
            
                    }
                    if (nSkipCmd == 1)
                    {
                        //2023 0110 Vision Skip 표시 기능
                        //해당 번지(전면 : 27107, 후면 : 28107)에 1이 있을경우 Vision Skip 표시 기능
                        Main.bVisionSkip = true;
                    }
                    else if (nSkipCmd == 0)
                        Main.bVisionSkip = false;
                    if(DEFINE.OPEN_F)
                    {
                        nCmd = PLCDataTag.RData[DEFINE.PLC_CMD] = 0;
                        nstatus = PLCDataTag.RData[DEFINE.VIS_STATUS] = 0;
                    }
                }
                Thread.Sleep(50);
            }
        }
        private static void CmdCheck()
        {
            int seq = 0;
            bool LoopFlag = true;

            while (LoopFlag)
            {
                switch (seq)
                {
                    case 0:
                        m_Timer.StartTimer();
                        seq++;
                        break;

                    case 1:
                        if (m_Timer.GetElapsedTime() > DEFINE.CMD_CHECK_TIMEOUT)
                        {
                            seq = SEQ.COMPLET_SEQ;
                            break;
                        }
                        if (PLCDataTag.RData[DEFINE.PLC_CMD] != 0)
                            break;
                        else
                            seq = SEQ.COMPLET_SEQ;
                        break;

                    case SEQ.COMPLET_SEQ:
                        LoopFlag = false;
                        break;

                }

            }

        }
        private static void ThreadProc_0()
        {
            while (threadFlag)
            {
                if (Status.MC_STATUS == DEFINE.MC_RUN)
                {
                    try
                    {
                        AlignUnit[0].ExecuteCMD();
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(50);
            }
        }
        private static void ThreadProc_1()
        {
            while (threadFlag)
            {
                if (Status.MC_STATUS == DEFINE.MC_RUN)
                {
                    try
                    {
                        AlignUnit[1].ExecuteCMD();
                        //2번 스테이지
                       // AlignUnit[1].ReceiveCommand();
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(50);
            }
        }
        private static void ThreadProc_2()
        {
            while (threadFlag)
            {
                if (Status.MC_STATUS == DEFINE.MC_RUN)
                {
                    try
                    {
                        //AlignUnit[2].ExecuteCMD();
                        //AlignUnit[2].ReceiveCommand();
                        //2022 11 12 YSH 비동작 10분이상 소요 시 조명 Off 
                        if (bFirstStartRun && TimerRunningCheck.GetElapsedTime() > 600000)
                        {
                            for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
                            {
                                for (int j = 0; j < Main.DEFINE.Pattern_Max; j++)
                                {
                                    Main.AlignUnit[i].PAT[0, j].SetAllLightOFF();
                                }
                            }
                            TimerRunningCheck.StartTimer();
                            bFirstStartRun = false;
                        }
                    }
                    catch
                    {

                    }
                }
                Thread.Sleep(50);
            }
        }
     
        private static void ThreadPLC_Alive()
        {
            if (!Main.DEFINE.OPEN_F && !Main.DEFINE.OPEN_CAM)
            {
               // int[] setValue = new int[1];
                while (threadFlag)
                {              
                    //2022 05 09 YSH               
                    //setValue[0] = 1;
                    //Main.PLCsocket.WriteDevice_W((PLCDataTag.BASE_RW_ADDR + Main.DEFINE.PLC_ALIVE).ToString(), 1, setValue);
                    Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + Main.DEFINE.PLC_ALIVE, 1);
                    Thread.Sleep(1000);
                    Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + Main.DEFINE.PLC_ALIVE, 0);
                    Thread.Sleep(1000);
                }
            }
        }
        #endregion












    }//Main
}//COG
