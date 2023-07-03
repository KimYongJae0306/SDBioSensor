using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;

//using Matrox.MatroxImagingLibrary;
using System.IO;

namespace CLineScan
{
#if false
    public class LineScan
    {
        private static int BUF_SIZE_MAX = 1;

        private MIL_ID MilSystem            = MIL.M_NULL;
        private MIL_ID MilApplication       = MIL.M_NULL;
        private MIL_ID MilDigitizer         = MIL.M_NULL;

        public  MIL_ID MilGrabImage         = MIL.M_NULL;
        public  MIL_ID MilBufImage          = MIL.M_NULL;
        public  MIL_ID[] MilScanBuffer  = new MIL_ID[BUF_SIZE_MAX];
         
        private MIL_ID BufAttributes = MIL.M_IMAGE + MIL.M_PROC;
        private MIL_ID BufDispAttributes = MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP;
        private MIL_ID BufGrabAttributes = MIL.M_IMAGE + MIL.M_PROC + MIL.M_GRAB + MIL.M_DISP;

        private int DevID = -1;
        private int GrabImageWidth = 0;
        private double GrabImageHeightDefault = 1000;
        private double GrabImageHeight = 0;
        private double ScanImageHeight = 0;

        private bool BoardUsable = false;
        public  bool ScanEndFlag = false;

        public bool Initialize(int iDevID, string DCFFileName, bool bBoardUsable)
        {
            bool bRet = false;
            DevID = iDevID;
            BoardUsable = bBoardUsable;

            do
            {
                if (0 == MilApplication) { if (MIL.M_NULL == MIL.MappAlloc(MIL.M_DEFAULT, ref MilApplication)) break; }
                MIL.MappControl(MIL.M_ERROR, MIL.M_PRINT_DISABLE);

                if (true == BoardUsable)
                {
                    MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0 + DevID, MIL.M_COMPLETE, ref MilSystem);
                    MIL.MdigAlloc(MilSystem, MIL.M_DEV0, DCFFileName, MIL.M_DEFAULT, ref MilDigitizer);
                    if (MIL.M_NULL == MilDigitizer) break;

                    GrabImageWidth  = (int)MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_X, MIL.M_NULL);
                    GrabImageHeight = (int)MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_Y, MIL.M_NULL);
                }

                else if (false == BoardUsable)
                {
                    MIL.MsysAlloc(MIL.M_SYSTEM_VGA, MIL.M_DEV0 + DevID, MIL.M_COMPLETE, ref MilSystem);
                }
            } while (false);

            return bRet;
        }

        public void DeInitialize()
        {
            int MilGrabBuffListSize = BUF_SIZE_MAX;
            while (MilGrabBuffListSize > 0) { MIL.MbufFree(MilScanBuffer[--MilGrabBuffListSize]); MilScanBuffer[MilGrabBuffListSize] = MIL.M_NULL; }
            if (MIL.M_NULL != MilBufImage)  { MIL.MbufFree(MilBufImage); MilBufImage = MIL.M_NULL; }
            if (MIL.M_NULL != MilGrabImage) { MIL.MbufFree(MilGrabImage); MilGrabImage = MIL.M_NULL; }
            if (MIL.M_NULL != MilDigitizer) { MIL.MdigFree(MilDigitizer); MilDigitizer = MIL.M_NULL; }
            if (MIL.M_NULL != MilSystem)      { MIL.MsysFree(MilSystem); MilSystem = MIL.M_NULL; }
            if (MIL.M_NULL != MilApplication) { MIL.MappFree(MilApplication); MilApplication = MIL.M_NULL; }
        }

        public bool AllocScanBuffer(int ImageWidth, double ImageHeight)
        {
            bool bRet = true;

            if (true == BoardUsable)
            {
                ScanImageHeight = ImageHeight;
                BUF_SIZE_MAX = (int)(ScanImageHeight / GrabImageHeightDefault);

                if (MilScanBuffer.Length <= 1) //최초일경우
                    MilScanBuffer = new MIL_ID[BUF_SIZE_MAX];
                else
                {
                    for (int i = 0; i < MilScanBuffer.Length; ++i)
                        if (MilScanBuffer[i] != MIL.M_NULL) MIL.MbufFree(MilScanBuffer[i]);
                    MilScanBuffer = new MIL_ID[BUF_SIZE_MAX];
                }

                if (MilGrabImage != MIL.M_NULL) MIL.MbufFree(MilGrabImage);
                MIL.MbufAlloc2d(MilSystem, GrabImageWidth, (MIL_INT)(GrabImageHeightDefault * BUF_SIZE_MAX), 8, BufGrabAttributes, ref MilGrabImage);
                for (int i = 0; i < BUF_SIZE_MAX; ++i)
                {
                    MIL.MbufChild2d(MilGrabImage, 0, (MIL_INT)(i * GrabImageHeightDefault), GrabImageWidth, (MIL_INT)GrabImageHeightDefault, ref MilScanBuffer[i]);
                    if (MIL.M_NULL != MilScanBuffer[i])
                    {
                        MIL.MbufClear(MilScanBuffer[i], 0x00);
                    }
                    else { bRet = false; break; }
                }

                GrabImageHeight = ImageHeight;
            }

            else if (false == BoardUsable)
            {
                GrabImageWidth = ImageWidth;
                GrabImageHeight = ImageHeight;
            }
            return bRet;
        }

        public byte[] Get2DBuffer()
        {
            UInt32 BuffSize = (UInt32)(GrabImageWidth * GrabImageHeight);
            byte[] pImagebuff = new byte[BuffSize];
            MIL.MbufGet2d(GetGrabImage(), 0, 0, GrabImageWidth, (MIL_INT)GrabImageHeight, pImagebuff);
            return pImagebuff;
        }

        #region Get grab image width height and GrabImage
        public int GetGrabImageWidth()
        {
            return GrabImageWidth;
        }
        public double GetGrabImageHeight()
        {
            return GrabImageHeight;
        }

        public MIL_ID GetGrabImage()
        {
            return MilGrabImage;
        }

        //public void SaveImage(MIL_ID milImg)
        //{
        //    string strFilePath = "D:\\Vision Process Image\\TestImage.bmp";
        //    MIL.MbufSave(strFilePath, milImg);
        //}
        #endregion

        #region Line Scan Function
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")] 
        public static extern void CopyMemory(IntPtr pDest, IntPtr pSrc, int length);
        
        public class HookDataStruct
        {
            public MIL_ID MilImageDisp;
            public int ProcessedImageCount;
        };

        static MIL_INT ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
        {
            Console.Write("callback0 start.\n");
            MIL_ID ModifiedBufferId = MIL.M_NULL;

            if (!IntPtr.Zero.Equals(HookDataPtr))
            {
                GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);          // get the handle to the DigHookUserData object back from the IntPtr
                HookDataStruct UserData = hUserData.Target as HookDataStruct;   // get a reference to the DigHookUserData object
                MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);
                UserData.ProcessedImageCount++;                                 // Print and draw the frame count.
                Console.Write("frame : #{0}.\n", UserData.ProcessedImageCount);
            }
            return 0;
        }

        public void Scan()
        {
            ScanEndFlag = false;
            HookDataStruct UserHookData = new HookDataStruct();
            GCHandle hUserData = GCHandle.Alloc(UserHookData);
            MIL_DIG_HOOK_FUNCTION_PTR ProcessingFunctionPtr;
            ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(ProcessingFunction);
            UserHookData.ProcessedImageCount = 0;

            MIL.MdigProcess(MilDigitizer, MilScanBuffer, BUF_SIZE_MAX, MIL.M_START, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));

            MIL.MdigProcess(MilDigitizer, MilScanBuffer, BUF_SIZE_MAX, MIL.M_STOP + MIL.M_WAIT, MIL.M_DEFAULT, ProcessingFunctionPtr, GCHandle.ToIntPtr(hUserData));
            ScanEndFlag = true;
        }
        #endregion

        public void SaveImage(string SerialNumber) //(DateTime DateTime, string RecipeName, ref string strFileNamePath)
        {
            UInt32 BuffSize = (UInt32)(GrabImageWidth * GrabImageHeight);
            byte[] pImageBuff = new byte[BuffSize];

            DateTime dateTime = DateTime.Now;

            //경로 변경
            string strDefaultPath = "D:\\Vision Inspection Data\\LeadFrameInspection";
            string strPath = String.Format("{0}\\VisionInspectionImages", strDefaultPath);
            //     if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            //     strPath = String.Format("{0}\\VisionInspectionImages\\{1}", strDefaultPath);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\OriginImages", strDefaultPath);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\OriginImages\\{1:D4}", strDefaultPath, dateTime.Year);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\OriginImages\\{1:D4}\\{2:D2}", strDefaultPath, dateTime.Year, dateTime.Month);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\OriginImages\\{1:D4}\\{2:D2}\\{3:D2}", strDefaultPath, dateTime.Year, dateTime.Month, dateTime.Day);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);

            //string strFileNamePath;
            //strFileNamePath = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}-", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            //strFileNamePath = strPath + "\\" + strFileNamePath + SerialNumber + ".bmp";

            string strFileName, strFileNamePath;
            strFileName = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2} {6}.bmp", dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, SerialNumber);
            strFileNamePath = strPath + "\\" + strFileName;

            //((MainForm)mObjForm).objResultManager.strImageName = strFileName;
            //((MainForm)mObjForm).objResultManager.strInspDate = String.Format("{0:D4}-{1:D2}-{2:D2}", DateTime.Year, DateTime.Month, DateTime.Day);
            //((MainForm)mObjForm).objResultManager.strInspTime = String.Format("{0:0#}:{1:0#}:{2:0#}:{3:0#}", DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond);

            try
            {
                MIL.MbufGetColor2d(MilGrabImage, MIL.M_PLANAR, MIL.M_ALL_BANDS, 0, 0, GrabImageWidth, (MIL_INT)GrabImageHeight, pImageBuff);
            }

            catch (System.Exception)
            {
                //Trace.Write("-----MbufGet info : " + ex.Message + "\n");
            }
            MIL.MbufPutColor2d(MilGrabImage, MIL.M_PLANAR, MIL.M_ALL_BANDS, 0, 0, GrabImageWidth, (MIL_INT)GrabImageHeight, pImageBuff);
            MIL.MbufExport(strFileNamePath, MIL.M_BMP, MilGrabImage);
        }

        public void SaveImage(DateTime DateTime, string RecipeName, ref string strFileNamePath)
        {
            UInt32 BuffSize = (UInt32)(GrabImageWidth * GrabImageHeight);
            byte[] pImageBuff = new byte[BuffSize];

            //경로 변경
            string strDefaultPath = "D:\\Vision Inspection Data\\LeadFrameInspection";
            string strPath = String.Format("{0}\\VisionInspectionImages", strDefaultPath);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\{1}", strDefaultPath, RecipeName);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\{1}\\OriginImages", strDefaultPath, RecipeName);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\{1}\\OriginImages\\{2:D4}", strDefaultPath, RecipeName, DateTime.Year);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\{1}\\OriginImages\\{2:D4}\\{3:D2}", strDefaultPath, RecipeName, DateTime.Year, DateTime.Month);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);
            strPath = String.Format("{0}\\VisionInspectionImages\\{1}\\OriginImages\\{2:D4}\\{3:D2}\\{4:D2}", strDefaultPath, RecipeName, DateTime.Year, DateTime.Month, DateTime.Day);
            if (false == Directory.Exists(strPath)) Directory.CreateDirectory(strPath);

            string strFileName;
            strFileName = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}{6:D3}.bmp", DateTime.Year, DateTime.Month, DateTime.Day, DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond);
            strFileNamePath = strPath + "\\" + strFileName;

            //((MainForm)mObjForm).objResultManager.strImageName = strFileName;
            //((MainForm)mObjForm).objResultManager.strInspDate = String.Format("{0:D4}-{1:D2}-{2:D2}", DateTime.Year, DateTime.Month, DateTime.Day);
            //((MainForm)mObjForm).objResultManager.strInspTime = String.Format("{0:0#}:{1:0#}:{2:0#}:{3:0#}", DateTime.Hour, DateTime.Minute, DateTime.Second, DateTime.Millisecond);

            try
            {
                MIL.MbufGetColor2d(MilGrabImage, MIL.M_PLANAR, MIL.M_ALL_BANDS, 0, 0, GrabImageWidth, (MIL_INT)GrabImageHeight, pImageBuff);
            }

            catch (System.Exception)
            {
                //Trace.Write("-----MbufGet info : " + ex.Message + "\n");
            }
            MIL.MbufPutColor2d(MilGrabImage, MIL.M_PLANAR, MIL.M_ALL_BANDS, 0, 0, GrabImageWidth, (MIL_INT)GrabImageHeight, pImageBuff);
            MIL.MbufExport(strFileNamePath, MIL.M_BMP, MilGrabImage);
        }
    }
#endif
}
