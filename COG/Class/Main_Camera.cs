using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics; //Timer
using System.Runtime.InteropServices; //DllImport
using System.IO;
using System.Net.NetworkInformation;

using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.CNLSearch;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.LineMax;
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.Dimensioning;

//using Matrox.MatroxImagingLibrary;

namespace COG
{
    public class ClsImage
    {
        #region "Neon 카메라 버퍼생성"
        /// <summary>
        /// Neon 카메라 버퍼생성
        /// </summary>
        public class SafeMalloc : SafeBuffer
        {
            public SafeMalloc(int size)
                : base(true)
            {
                this.SetHandle(Marshal.AllocHGlobal(size));
                this.Initialize((ulong)size);
            }
            protected override bool ReleaseHandle()
            {
                Marshal.FreeHGlobal(this.handle);
                return true;
            }
            public static implicit operator IntPtr(SafeMalloc h)
            {
                return h.handle;
            }
        }
        #endregion

        #region "CogImage Type으로 변경"
        /// <summary>
        /// CogImage Type으로 변경
        /// </summary>
        /// <param name="imageData"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public ICogImage Convert8BitRawImageToCognexImage(byte[] imageData, int width, int height)
        {
            var rawSize = width * height;
            var buf = new SafeMalloc(rawSize);
            Marshal.Copy(imageData, 0, buf, rawSize);

            var cogRoot = new CogImage8Root();
            cogRoot.Initialize(width, height, buf, width, buf);
            var cogImage = new CogImage8Grey();
            cogImage.SetRoot(cogRoot);

            return cogImage;
        }

        public ICogImage ConvertColorImageToCognexImage(byte[] colorImageByte, int width, int height)
        {
            var rawSize = (width * height);
            var bufR = new SafeMalloc(rawSize);
            Marshal.Copy(colorImageByte, 0, bufR, rawSize);

            var bufG = new SafeMalloc(rawSize);
            Marshal.Copy(colorImageByte, rawSize, bufG, rawSize);

            var bufB = new SafeMalloc(rawSize);
            Marshal.Copy(colorImageByte, rawSize * 2, bufB, rawSize);
            var cogRootR = new CogImage8Root();
            cogRootR.Initialize(width, height, bufR, width, bufR);
            var cogRootG = new CogImage8Root();
            cogRootG.Initialize(width, height, bufG, width, bufG);
            var cogRootB = new CogImage8Root();
            cogRootB.Initialize(width, height, bufB, width, bufB);
            var cogImage = new CogImage24PlanarColor();
            cogImage.SetRoots(cogRootR, cogRootG, cogRootB);
            return cogImage;
        }
        #endregion
    }

    public partial class Main
    {
  //      public static MILFrameGrabber MilFrameGrabber { get; private set; } = new MILFrameGrabber();

        public static ClsImage _ClsImage = new ClsImage();
        //-----------------------------------------------------------------------------------------------------------------------------------

        public static int[] nMainLiveCamera = new int[DEFINE.CAM_MAX];
        public static void Refresh_Unit()
        {
            
            #region START 상태에서 명령이 실행중이지 않을때만 GRAB할때 사용 미사용중
            //-----------------------------------------------------------------------------------------------------------------------------------
            //             for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
            //             {
            //                 for (int jj = 0; jj < AlignUnit[i].m_AlignPatTagMax; jj++)
            //                 {
            //                     for (int j = 0; j < AlignUnit[i].m_AlignPatMax; j++)
            //                     {
            //                         for (int k = 0; k < DEFINE.CAM_MAX; k++)
            //                         {
            //                             if (AlignUnit[i].PAT[jj, j].m_CamNo == k)
            //                                nCamera[k] += PLCDataTag.RData[AlignUnit[i].ALIGN_UNIT_ADDR + DEFINE.PLC_CMD];
            //                         }
            //                     }
            //                 }
            //              }
            //-----------------------------------------------------------------------------------------------------------------------------------
            #endregion

            for (int k = 0; k < DEFINE.CAM_MAX; k++) //무조건 STOP일때는 촬상 하도록하였음.
            {
                nMainLiveCamera[k] = 1;
                //if (k == 0)
                //    nCamera[k] = 0;
            }
            RefreshImage_Live(nMainLiveCamera);
        }

        public static void Refresh_Unit_Part(int[] nMainLiveFlag)
        {
            RefreshImage_Live(nMainLiveFlag);
        }

        public static void Refresh_Unit(Main.AlignUnitTag AlignUnit)
        {
            List<int> ListCamNum = new List<int>();
            int PatMax;
            int PatTagMax = AlignUnit.m_AlignPatTagMax;

            for (int i = 0; i < PatTagMax; i++)
            {
                PatMax = AlignUnit.m_AlignPatMax[i];
                for (int j = 0; j < PatMax; j++)
                {
                    if (!ListCamNum.Contains(AlignUnit.PAT[i, j].m_CamNo))
                    {
                        ListCamNum.Add(AlignUnit.PAT[i, j].m_CamNo);
                    }
                }
            }
            RefreshImage_UnitLive(ListCamNum);
        }
        public struct Vision
        {
            public ICogImage[] CogCamBuf; //카메라랑 연결 시킬 버퍼. 
            public CogImageFileTool[] CogImgTool; //Test할때 사용 이미지 가져오기위한.
            public CogToolBlock[] CogImageBlock; //카메라 관련 

            // private static Mutex[] Mutex_lock = new Mutex[Main.DEFINE.CAM_MAX];

            public int[] IMAGE_SIZE_X;
            public int[] IMAGE_SIZE_Y;
            public int[] IMAGE_CENTER_X;
            public int[] IMAGE_CENTER_Y;
            public int[] CUSTOM_CROSS_X;
            public int[] CUSTOM_CROSS_Y;
            public bool[] USE_CUSTOM_CROSS;

            //  public bool[] GrabRefresh;
            public bool[] GrabRefresh_LiveView;
            public bool[] Grab_Flag_Start;
            public bool[] Grab_Flag_End;
            //public Mutex[] Mutex_lock_Grab;
            public string[] CoordinateSpaceName; //DisplayStatusBar 에서 마우스 좌표 표시 할때 이미지의 회전에 따라서 제대루된 좌표값 뿌릴때. 사용함 "*\\#\\"기본 ,"*\\#\\@"-> Y 반전시

            public string[] CamName;
        }
        public static Vision vision;
        public static void Vision_Initial()
        {
            vision.CogCamBuf = new ICogImage[DEFINE.CAM_MAX];
            vision.CogImgTool = new CogImageFileTool[DEFINE.CAM_MAX];
            vision.CogImageBlock = new CogToolBlock[DEFINE.CAM_MAX];

            vision.GrabRefresh_LiveView = new bool[DEFINE.CAM_MAX];
            vision.Grab_Flag_Start = new bool[DEFINE.CAM_MAX];
            vision.Grab_Flag_End = new bool[DEFINE.CAM_MAX];
            vision.IMAGE_SIZE_X = new int[DEFINE.CAM_MAX];
            vision.IMAGE_SIZE_Y = new int[DEFINE.CAM_MAX];
            vision.IMAGE_CENTER_X = new int[DEFINE.CAM_MAX];
            vision.IMAGE_CENTER_Y = new int[DEFINE.CAM_MAX];
            vision.CUSTOM_CROSS_X = new int[DEFINE.CAM_MAX];
            vision.CUSTOM_CROSS_Y = new int[DEFINE.CAM_MAX];
            vision.USE_CUSTOM_CROSS = new bool[DEFINE.CAM_MAX];

            //vision.Mutex_lock_Grab = new Mutex[Main.DEFINE.CAM_MAX];
            vision.CoordinateSpaceName = new string[Main.DEFINE.CAM_MAX];
            vision.CamName = new string[DEFINE.CAM_MAX];

            Form_ProgressBar formProgressBar1 = new Form_ProgressBar();
            formProgressBar1.nMaximum = Main.DEFINE.CAM_MAX;
            formProgressBar1.nMessage = "CAMERA";
            formProgressBar1.Show();

       //     if (Main.DEFINE.MIL_CAM_MAX > 0)
      //          MilFrameGrabber.Initialize(false);

            for (int i = 0; i < DEFINE.CAM_MAX; i++)
            {
                if (Main.DEFINE.OPEN_F)
                {
                    vision.CogImgTool[i] = new CogImageFileTool();
                    //vision.Mutex_lock_Grab[i] = new Mutex();

                    GetImageFile(vision.CogImgTool[i], Main.DEFINE.IMAGE_FILE);

                    vision.IMAGE_SIZE_X[i] = vision.CogImgTool[i].OutputImage.Width;
                    vision.IMAGE_SIZE_Y[i] = vision.CogImgTool[i].OutputImage.Height;
                    vision.IMAGE_CENTER_X[i] = vision.IMAGE_SIZE_X[i] / 2;
                    vision.IMAGE_CENTER_Y[i] = vision.IMAGE_SIZE_Y[i] / 2;
                    vision.USE_CUSTOM_CROSS[i] = false;
                    vision.CogCamBuf[i] = vision.CogImgTool[i].OutputImage as ICogImage;
                    vision.CamName[i] = i.ToString();
                }
                else
                {
                    if (i < DEFINE.MIL_CAM_MAX)
                    {
                        int systemNo = i / 2;   //TEMP

                        //if (MilFrameGrabber.OpenDigitizer(systemNo, i, DEFINE.DCFPath))
                        //{
                        //    vision.Mutex_lock_Grab[i] = new Mutex();
                        //    /*vision.CogImageBlock[i] = new CogToolBlock();

                        //    CogAcqFifoTool temptool = new CogAcqFifoTool();
                        //    CogIPOneImageTool temptool1 = new CogIPOneImageTool();
                        //    CogFixtureTool temptool2 = new CogFixtureTool();

                        //    temptool.Name = "CogAcqFifoTool1";
                        //    temptool1.Name = "CogIPOneImageTool1";
                        //    temptool2.Name = "CogFixtureTool1";

                        //    Main.vision.CogImageBlock[i].Tools.Add(temptool);
                        //    Main.vision.CogImageBlock[i].Tools.Add(temptool1);
                        //    Main.vision.CogImageBlock[i].Tools.Add(temptool2);

                        //    Main.vision.CogImageBlock[i].Tools["CogIPOneImageTool1"].DataBindings.Add("InputImage",
                        //    Main.vision.CogImageBlock[i].Tools["CogAcqFifoTool1"],
                        //    "OutputImage");

                        //    Main.vision.CogImageBlock[i].Tools["CogFixtureTool1"].DataBindings.Add("InputImage",
                        //    Main.vision.CogImageBlock[i].Tools["CogIPOneImageTool1"],
                        //    "OutputImage");

                        //    Cognex.VisionPro.CogImage8Grey nOutPutImage = new CogImage8Grey();
                        //    CogToolBlockTerminal nOutPut = new CogToolBlockTerminal("OutputImage", nOutPutImage);
                        //    Main.vision.CogImageBlock[i].Outputs.Add(nOutPut);

                        //    string desPath = "";
                        //    desPath = DataBindingPath(Main.vision.CogImageBlock[i].Outputs["OutputImage"]);
                        //    Main.vision.CogImageBlock[i].Outputs.DataBindings.Add(desPath, Main.vision.CogImageBlock[i].Tools["CogFixtureTool1"], "OutputImage");*/

                        //    vision.IMAGE_SIZE_X[i] = MilFrameGrabber.ImageWidth[i];
                        //    vision.IMAGE_SIZE_Y[i] = MilFrameGrabber.ImageHeight[i];
                        //    vision.IMAGE_CENTER_X[i] = vision.IMAGE_SIZE_X[i] / 2;
                        //    vision.IMAGE_CENTER_Y[i] = vision.IMAGE_SIZE_Y[i] / 2;
                        //    vision.USE_CUSTOM_CROSS[i] = false;
                        //    vision.CamName[i] = "ALIGN_" + i.ToString();

                        //    bool bRet = MilFrameGrabber.AllocBuffer_Area(i);
                        //    if (bRet == false)
                        //    {
                        //        MessageBox.Show("Failed to alloc MIL buffer!");
                        //    }

                        //    //vision.CogCamBuf[i] = MilFrameGrabber.
                        //    //Logger.Instance.Log(LEVEL.INFO, CLASS.SYSTEM, "Success MIL FrameGrabber Init");
                        //    //frameGrabberInit = true;
                        //}
                        //else
                        //{
                        //    //Logger.Instance.Log(LEVEL.ERROR, CLASS.SYSTEM, "Failed MIL FrameGrabber Init");
                        //    //frameGrabberInit = false;
                        //}
                    }
                    else
                    {
                        try
                        {
                            //int j = i - DEFINE.MIL_CAM_MAX;
                            GetCamSetValue(i);
                            vision.IMAGE_SIZE_X[i] = (vision.CogImageBlock[i].Outputs[0].Value as ICogImage).Width;
                            vision.IMAGE_SIZE_Y[i] = (vision.CogImageBlock[i].Outputs[0].Value as ICogImage).Height;
                            vision.IMAGE_CENTER_X[i] = vision.IMAGE_SIZE_X[i] / 2;
                            vision.IMAGE_CENTER_Y[i] = vision.IMAGE_SIZE_Y[i] / 2;
                            vision.USE_CUSTOM_CROSS[i] = false;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                        }
                    }
                }
                formProgressBar1.progressBar1.Value = (i + 1);
            }
            formProgressBar1.Dispose();
        }
        public static void Vision_Close()
        {
            try
            {
                CogFrameGrabbers frameGrabbers = new CogFrameGrabbers();
                foreach (ICogFrameGrabber fg in frameGrabbers)
                    fg.Disconnect(false);
                for (int i = 0; i < DEFINE.CAM_MAX; i++)
                {
                    if (!Main.DEFINE.OPEN_F)
                    {
                        if (i < DEFINE.MIL_CAM_MAX)
                        {
                //            MilFrameGrabber.Release(i);
                        }
                        else
                            vision.CogImageBlock[i].Dispose();
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
            }

        }

        private static void GetCamSetValue(int CamNo)
        {
            vision.CogImageBlock[CamNo] = new CogToolBlock();
            //vision.Mutex_lock_Grab[CamNo] = new Mutex();
            String filename = DEFINE.SYS_DATADIR + DEFINE.CAM_SETDIR + "CCD_" + CamNo + ".vpp";
            try
            {
                if (File.Exists(filename))
                {
                    Main.vision.CogImageBlock[CamNo] = CogSerializer.LoadObjectFromFile(filename) as CogToolBlock;
                }
                else
                {
                    #region Camera VPP initialize
                    CogFrameGrabbers frameGrabberALL = new CogFrameGrabbers();

                    CogAcqFifoTool temptool = new CogAcqFifoTool();
                    CogIPOneImageTool temptool1 = new CogIPOneImageTool();
                    CogFixtureTool temptool2 = new CogFixtureTool();

                    temptool.Name = "CogAcqFifoTool1";
                    temptool1.Name = "CogIPOneImageTool1";
                    temptool2.Name = "CogFixtureTool1";

                    int idx = CamNo - Main.DEFINE.MIL_CAM_MAX;
                    temptool.Operator = frameGrabberALL[idx].CreateAcqFifo("Generic GigEVision (Mono)", CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);

                    Main.vision.CogImageBlock[CamNo].Tools.Add(temptool);
                    Main.vision.CogImageBlock[CamNo].Tools.Add(temptool1);
                    Main.vision.CogImageBlock[CamNo].Tools.Add(temptool2);

                    Main.vision.CogImageBlock[CamNo].Tools["CogIPOneImageTool1"].DataBindings.Add("InputImage",
                    Main.vision.CogImageBlock[CamNo].Tools["CogAcqFifoTool1"],
                    "OutputImage");

                    Main.vision.CogImageBlock[CamNo].Tools["CogFixtureTool1"].DataBindings.Add("InputImage",
                    Main.vision.CogImageBlock[CamNo].Tools["CogIPOneImageTool1"],
                    "OutputImage");

                    Cognex.VisionPro.CogImage8Grey nOutPutImage = new CogImage8Grey();
                    CogToolBlockTerminal nOutPut = new CogToolBlockTerminal("OutputImage", nOutPutImage);
                    Main.vision.CogImageBlock[CamNo].Outputs.Add(nOutPut);

                    string desPath = "";
                    desPath = DataBindingPath(Main.vision.CogImageBlock[CamNo].Outputs["OutputImage"]);
                    Main.vision.CogImageBlock[CamNo].Outputs.DataBindings.Add(desPath, Main.vision.CogImageBlock[CamNo].Tools["CogFixtureTool1"], "OutputImage");
                    #endregion

                    #region 카메라 초기 셋팅
                    int nX, nY, nW, nH;
                    double Exposure, Brightness, Contrast;

                    CogAcqFifoTool nfifotool = new CogAcqFifoTool();
                    nfifotool = Main.vision.CogImageBlock[CamNo].Tools[0] as CogAcqFifoTool;
                    nfifotool.Operator.OwnedROIParams.GetROIXYWidthHeight(out nX, out nY, out nW, out nH);
                    if (nW == 2592)  // 가압
                    {
                        Exposure = 120;
                        Brightness = 0.1;
                        Contrast = 0.4;
                    }
                    else
                    {
                        Exposure = 60;
                        Brightness = 0.1;
                        Contrast = 0.4;
                    }
                    try { nfifotool.Operator.OwnedExposureParams.Exposure = Exposure; }         catch { }
                    try { nfifotool.Operator.OwnedBrightnessParams.Brightness = Brightness; }   catch { }
                    try { nfifotool.Operator.OwnedContrastParams.Contrast = Contrast; }         catch { }

                    #endregion

                    try
                    {
                        CogSerializer.SaveObjectToFile(Main.vision.CogImageBlock[CamNo], filename);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                    }
                }
                Main.vision.CogImageBlock[CamNo].Name = CamNo.ToString();
                Main.vision.CogImageBlock[CamNo].Ran += Main_Ran;
                Main.vision.CogImageBlock[CamNo].Run();
                Main.vision.CogCamBuf[CamNo] = Main.vision.CogImageBlock[CamNo].Outputs[0].Value as ICogImage;

                #region CAMERA NAME
                string CamToHostIP, CurLocalIP;

                vision.CamName[CamNo] = CamNo.ToString();

                CogAcqFifoTool fifotool = new CogAcqFifoTool();
                fifotool = Main.vision.CogImageBlock[CamNo].Tools[0] as CogAcqFifoTool;

                NetworkInterface[] network = NetworkInterface.GetAllNetworkInterfaces();
                CamToHostIP = fifotool.Operator.FrameGrabber.OwnedGigEAccess.HostIPAddress;

                for (int i = 0; i < network.Length; i++)
                {
                    if (network[i].NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        for (int j = 0; j < network[i].GetIPProperties().UnicastAddresses.Count; j++)
                        {
                            CurLocalIP = network[i].GetIPProperties().UnicastAddresses[j].Address.ToString();
                            if (CurLocalIP == CamToHostIP)
                            {
                                vision.CamName[CamNo] = network[i].Name;
                            }
                        }
                    }
                }                
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);

            }
        }
#if false
        public static bool ImageGrab_(int CamNo)
        {
            if (Main.DEFINE.OPEN_F)
            {
                Main.vision.Grab_Flag_Start[CamNo] = false;
                return true;
            }

            vision.Mutex_lock_Grab[CamNo].WaitOne();
            CogAcqFifoTool temptool = new CogAcqFifoTool();
            CogImage8Grey nTempIMG = new CogImage8Grey(Main.vision.IMAGE_SIZE_X[CamNo], Main.vision.IMAGE_SIZE_Y[CamNo]);
            try
            {
                if (CamNo < DEFINE.MIL_CAM_MAX)
                {
                    MIL.MdigGrab(MilFrameGrabber._MilDigitizer[CamNo], MilFrameGrabber._MilGrabImage[CamNo]);
                    UInt32 BuffSize = (UInt32)(vision.IMAGE_SIZE_X[CamNo] * vision.IMAGE_SIZE_Y[CamNo]);
                    byte[] Imagebuff = new byte[BuffSize];
                    MIL.MbufGet2d(MilFrameGrabber._MilGrabImage[CamNo], 0, 0, vision.IMAGE_SIZE_X[CamNo], vision.IMAGE_SIZE_Y[CamNo], Imagebuff);
                    nTempIMG = _ClsImage.Convert8BitRawImageToCognexImage(Imagebuff, vision.IMAGE_SIZE_X[CamNo], vision.IMAGE_SIZE_Y[CamNo]) as CogImage8Grey;
                }
                else
                {
                    temptool = (CogAcqFifoTool)Main.vision.CogImageBlock[CamNo].Tools["CogAcqFifoTool1"];
                    Main.vision.CogImageBlock[CamNo].Run();

                    if (Main.vision.CogImageBlock[CamNo].RunStatus.Result == CogToolResultConstants.Accept)
                    {
                        nTempIMG = Main.vision.CogImageBlock[CamNo].Outputs[0].Value as CogImage8Grey;
                    }
                }
            }
            catch (System.Exception ex)
            {
            }
            finally
            {
                Main.vision.CogCamBuf[CamNo] = nTempIMG;
                temptool.Operator.Flush();
                vision.Mutex_lock_Grab[CamNo].ReleaseMutex();
                Main.vision.Grab_Flag_End[CamNo] = true;
                Main.vision.Grab_Flag_Start[CamNo] = false;
                Main.vision.GrabRefresh_LiveView[CamNo] = true;
            }
            return true;
        }
#endif
        public static bool ImageGrab(int CamNo)
        {
            if (Main.DEFINE.OPEN_F || CamNo >= Main.DEFINE.CAM_MAX)
            {
                Main.vision.Grab_Flag_Start[CamNo] = false;
                return true;
            }

            //vision.Mutex_lock_Grab[CamNo].WaitOne();

            if (Main.vision.CogCamBuf[CamNo] == null || Main.vision.CogCamBuf[CamNo].Width != Main.vision.IMAGE_SIZE_X[CamNo] 
                || Main.vision.CogCamBuf[CamNo].Height != Main.vision.IMAGE_SIZE_Y[CamNo])
            {
                Main.vision.CogCamBuf[CamNo] = new CogImage8Grey(Main.vision.IMAGE_SIZE_X[CamNo], Main.vision.IMAGE_SIZE_Y[CamNo]);
            }

            int seq = 0;
            bool LoopFlag = true;
            bool nReSearch = false;
            while (LoopFlag)
            {
                switch (seq)
                {
                    case 0:
                        //                         vision.Mutex_lock_Grab[CamNo].WaitOne();
                        //                         nTempIMG = new CogImage8Grey(Main.vision.IMAGE_SIZE_X[CamNo], Main.vision.IMAGE_SIZE_Y[CamNo]);
                        seq++;
                        break;
                    case 1:
                        if (CamNo >= DEFINE.MIL_CAM_MAX)
                            Main.vision.CogImageBlock[CamNo].Run();
                        seq++;
                        break;
                    case 2:
                        try
                        {
                            if (CamNo < DEFINE.MIL_CAM_MAX)
                            {
                     //           byte[] Imagebuff = MilFrameGrabber.Grab(CamNo);
                   //             Main.vision.CogCamBuf[CamNo] = _ClsImage.Convert8BitRawImageToCognexImage(Imagebuff, vision.IMAGE_SIZE_X[CamNo], vision.IMAGE_SIZE_Y[CamNo]) as CogImage8Grey;

                                //CogImageFileBMP imagecontrol = new CogImageFileBMP();
                                //imagecontrol.Open("Test.bmp", CogImageFileModeConstants.Write);
                                //imagecontrol.Append(nTempIMG);
                                //imagecontrol.Close();
                                //Main.AlignUnit[0].LogdataDisplay("Grab : " + CamNo.ToString(), true);
                            }
                            else
                            {
                                if (Main.vision.CogImageBlock[CamNo].RunStatus.Result == CogToolResultConstants.Accept)
                                {
                                    Main.vision.CogCamBuf[CamNo] = Main.vision.CogImageBlock[CamNo].Outputs[0].Value as CogImage8Grey;
                                }
                                else
                                {
                                    if (!nReSearch)
                                    {
                                        nReSearch = true;
                                        Main.SearchDelay(100);
                                        seq = 1;
                                        break;
                                    }
                                    else
                                    {
                                        seq = SEQ.ERROR_SEQ;
                                        break;
                                    }
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            seq = SEQ.ERROR_SEQ;
                            break;
                        }
                        seq = SEQ.COMPLET_SEQ;
                        break;

                    case SEQ.ERROR_SEQ:

                        seq = SEQ.COMPLET_SEQ;
                        break;

                    case SEQ.COMPLET_SEQ:
                        //Main.vision.CogCamBuf[CamNo] = nTempIMG;
                        if (CamNo >= Main.DEFINE.MIL_CAM_MAX)
                            ((CogAcqFifoTool)Main.vision.CogImageBlock[CamNo].Tools["CogAcqFifoTool1"]).Operator.Flush();
                       // vision.Mutex_lock_Grab[CamNo].ReleaseMutex();
                        Main.vision.Grab_Flag_End[CamNo] = true;
                        Main.vision.Grab_Flag_Start[CamNo] = false;
                        Main.vision.GrabRefresh_LiveView[CamNo] = true;
                        LoopFlag = false;

                        if (CamNo < DEFINE.MIL_CAM_MAX)     // MIL의 경우
                            Thread.Sleep(100);  // ToolBox 에서 시간 조절이 안됨

                        break;
                }
            }
            return true;
        }

        static void Main_Ran(object sender, EventArgs e)
        {
            CogToolBlock ToolBlock = (CogToolBlock)sender;
            int CamNo = Convert.ToInt32(ToolBlock.Name);
   //         Main.vision.Grab_Flag_Start[CamNo] = false;
            if (Main.Status.MC_STATUS == Main.DEFINE.MC_STOP && Main.Status.MC_STATUS != Main.DEFINE.MC_CAMERAFORM)
            {
                //DisPlay Image 에 복사 하기 
            }
//             Main.vision.CogCamBuf[CamNo] = ToolBlock.Outputs[0].Value as CogImage8Grey;
//             Main.vision.Grab_Flag_End[CamNo] = true;
//             Main.vision.GrabRefresh_LiveView[CamNo] = true;
        }
        private static void RefreshImage_Live(int[] nCamera)
        {
            for (int i = 0; i < nCamera.Length; i++)
            {
                if (nCamera[i] > 0)
                {
                    Main.vision.Grab_Flag_Start[i] = true;
                }
            }

        }
        private static void RefreshImage_UnitLive(List<int> nCamera)
        {
            for (int i = 0; i < nCamera.Count; i++)
            {
                Main.vision.Grab_Flag_Start[nCamera[i]] = true;
            }
        }
        public static void GetImageFile(CogImageFileTool ImageFileTool, String FileName)
        {
            try
            {
                ImageFileTool.Operator.Open(FileName, CogImageFileModeConstants.Read);
                ImageFileTool.Run();
            }
            catch (System.Exception ex)
            {
                ImageFileTool.Operator.Open("D:\\SystemData\\QDIDB.idb", CogImageFileModeConstants.Read);
                ImageFileTool.Run();
            }
        }
        public static string DataBindingPath(Cognex.VisionPro.ToolBlock.CogToolBlockTerminal Terminal)
        {
            CogToolBlockTerminal inputTerminal;
            string sourcePath;

            try
            {
                inputTerminal = Terminal;
                sourcePath = "Item[\"" + inputTerminal.ID + "\"].Value.(" + inputTerminal.ValueType.FullName + ")";
                sourcePath = Cognex.VisionPro.Implementation.Internal.CogToolTerminals.RemoveExtraAssemblyInfoFromPath(sourcePath);
            }
            catch (System.Exception ex)
            {
                return sourcePath = null;
            }
            return sourcePath;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
    }
}
