//using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows.Forms;
using Cognex.VisionPro;

namespace COG
{
    public partial class Main
    {
#if false
        public enum ScanStatus { SCAN_NONE, SCAN_READY, SCAN_START, SCANNING, SCAN_COMPLETE, PAGE_COMPLETE }
        public class MILFrameGrabber
        {
            #region Field
            private static int BUF_SIZE = 1;

            private MIL_ID _MilApplication = MIL.M_NULL;
            private MIL_ID[] _MilSystem = new MIL_ID[DEFINE.MIL_BOARD_MAX];
            public MIL_ID[] _MilDigitizer = new MIL_ID[DEFINE.MIL_CAM_MAX];
            private MIL_INT _SizeBand = 0;
            private MIL_INT _Type = 0;

            public MIL_ID[] _MilGrabImage = new MIL_ID[DEFINE.MIL_CAM_MAX];
            private MIL_ID[] _MilScanBuffer = new MIL_ID[BUF_SIZE];

            private MIL_ID _BufAttributes = MIL.M_IMAGE + MIL.M_PROC;
            private MIL_ID _BufDispAttributes = MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP;
            private MIL_ID _BufGrabAttributes = MIL.M_IMAGE + MIL.M_PROC + MIL.M_GRAB + MIL.M_DISP;

            //hook func alloc
            private MIL_DIG_HOOK_FUNCTION_PTR _ProcessingFunctionPtr = new MIL_DIG_HOOK_FUNCTION_PTR(ProcessingFunction);
            private HookDataStruct _UserHookData = new HookDataStruct();

            private bool _isSimualtionMode = false;
            private string previousMessage = string.Empty;
            #endregion

            #region Property
            public int DeviceID { get; set; } = -1;
            public int []ImageWidth { get; set; } = new int[DEFINE.MIL_CAM_MAX];
            public int []ImageHeight { get; set; } = new int[DEFINE.MIL_CAM_MAX];
            public int []ImageHeightDefault { get; private set; } = new int[DEFINE.MIL_CAM_MAX];

            public int [] COMPortNum { get; private set; } = new int[DEFINE.MIL_CAM_MAX];
            public double LineCount { get; set; } = 0;
            public bool IsInitialize { get; private set; }
            public string DcfFilePath { get; set; } = string.Empty;
            #endregion

            #region Event
            public event Action<int> ReceiveImage;
            private static event Action CheckHookDataState;
            #endregion


            public bool Initialize(bool isSimautionMode)
            {

                this._isSimualtionMode = isSimautionMode;
                try
                {
                    if (MIL.M_NULL == _MilApplication && _isSimualtionMode == false)
                    {
                        if (MIL.M_NULL == MIL.MappAlloc(MIL.M_DEFAULT, ref _MilApplication))
                            return false;
                    }

                    MIL.MappControl(MIL.M_ERROR, MIL.M_PRINT_DISABLE);

                    int nDeviceCnt = (int)MIL.MappInquire(MIL.M_INSTALLED_SYSTEM_COUNT);

                    if (nDeviceCnt < 1)
                        return false;

                    int nCompletCnt = 0;
                    for (int i = 0; i < nDeviceCnt; i++)
                    {
                        if (i >= DEFINE.MIL_BOARD_MAX) break;
                        _MilSystem[i] = MIL.M_NULL;

                        //시스템 할당 
                        if (MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEV0 + i, MIL.M_COMPLETE, ref _MilSystem[i]) != MIL.M_NULL)
                        {
                            nCompletCnt++;
                        }
                    }

                    if (nCompletCnt == nDeviceCnt)
                        return true;

                    return false;
                }
                catch(Exception e)
                {
                    MessageBox.Show("MIL Grabber Initialize Fail " +e.ToString());
                    return false;
                }
            }

            /// <summary>
            /// MIL 프레임 그래버 초기화 함수 최초 한번 진행
            /// </summary>
            /// <param name="deviceID">MIL System ID</param>
            /// <param name="dcfFilePath">DCF File 경로</param>
            /// <param name="isSimautionMode">SimautionMode로 사용 여부</param>
            /// <returns></returns>
            public bool OpenDigitizer(int systemID, int deviceID, string dcfFilePath)
            {
                DeviceID = deviceID;
                if (DeviceID == -1)
                    return false;
                if (string.IsNullOrEmpty(dcfFilePath))
                    return false;

                #region MIL 관련 초기화
                if (false == _isSimualtionMode)//Simulation mode Disable
                {
                    //if (deviceID == 0)
                    //{
                    //    dcfFilePath = "D:\\Solb_mil9u5_XCL-S900_s4tap_c2tap_8bit_c.dcf";
                    //}
                    //else if (deviceID == 1)
                    //{
                    //    dcfFilePath = "D:\\solxcl_mil8_g60fv11cl_c_8bit_2tap.dcf";
                    //}
                    //else if (deviceID == 2)
                    //{
                    //    dcfFilePath = "D:\\STC-CMB120APCL-F_4096x3072_2Tap_Cont.dcf";
                    //}

                    //dcf 설정 가져오기
                    //MIL.MdigAlloc(_MilSystem[systemID], MIL.M_DEV0 + (deviceID%2), dcfFilePath, MIL.M_DEFAULT, ref _MilDigitizer[DeviceID]);

                    COMPortNum[deviceID] = (int)MIL.MsysInquire(_MilSystem[systemID], MIL.M_COM_PORT_NUMBER + MIL.M_UART_NB(deviceID%2), MIL.M_NULL);

                    //MIL.MdigControl(_MilDigitizer, MIL.M_, MIL.M_ENABLE);
                    if (MIL.M_NULL == _MilDigitizer[DeviceID])
                        return false;

                    //dcf 설정값 읽어오기
                    ImageWidth[deviceID] = (int)MIL.MdigInquire(_MilDigitizer[DeviceID], MIL.M_SIZE_X, MIL.M_NULL);
                    ImageHeight[deviceID] = (int)MIL.MdigInquire(_MilDigitizer[DeviceID], MIL.M_SIZE_Y, MIL.M_NULL);
                    ImageHeightDefault[deviceID] = (int)MIL.MdigInquire(_MilDigitizer[DeviceID], MIL.M_SIZE_Y, MIL.M_NULL);
                    _Type = (int)MIL.MdigInquire(_MilDigitizer[DeviceID], MIL.M_TYPE, MIL.M_NULL);
                    _SizeBand = (int)MIL.MdigInquire(_MilDigitizer[DeviceID], MIL.M_SIZE_BAND, MIL.M_NULL);
                }
                else if (true == _isSimualtionMode)//Simulation mode Enable
                {
                    MIL.MsysAlloc(MIL.M_SYSTEM_VGA, MIL.M_DEV0 + DeviceID, MIL.M_COMPLETE, ref _MilSystem[systemID]);
                }

                #endregion

                CheckHookDataState += MILFrameGrabber_CheckHookDataState;
                IsInitialize = true;
                return true;
            }

            public void Release(int nDevNo)
            {
                if (_isSimualtionMode)
                    return;

                for (int i = 0; i < _MilScanBuffer.Length; ++i)
                    if (_MilScanBuffer[i] != MIL.M_NULL) MIL.MbufFree(_MilScanBuffer[i]);

                int systemNo = nDevNo/2;

                if (MIL.M_NULL != _MilGrabImage[nDevNo]) { MIL.MbufFree(_MilGrabImage[nDevNo]); _MilGrabImage[nDevNo] = MIL.M_NULL; }
                if (MIL.M_NULL != _MilDigitizer[nDevNo]) { MIL.MdigFree(_MilDigitizer[nDevNo]); _MilDigitizer[nDevNo] = MIL.M_NULL; }
                if (MIL.M_NULL != _MilSystem[systemNo]) { MIL.MsysFree(_MilSystem[systemNo]); _MilSystem[systemNo] = MIL.M_NULL; }
                if (MIL.M_NULL != _MilApplication) { MIL.MappFree(_MilApplication); _MilApplication = MIL.M_NULL; }
            }

            private void MILFrameGrabber_CheckHookDataState()
            {
                switch (_UserHookData.Status)
                {
                    case ScanStatus.SCAN_NONE:
                        previousMessage = "Buffer Not Alloc.";
                        Debug.WriteLine(previousMessage);
                        break;
                    case ScanStatus.SCAN_READY:
                        previousMessage = $"Buffer alloc / ImageWidth : {ImageWidth[0]}/ImageHeight : {(int)ImageHeight[0]}";
                        Debug.WriteLine(previousMessage);
                        break;
                    case ScanStatus.SCAN_START:
                        previousMessage = "Scan Start";
                        Debug.WriteLine(previousMessage);
                        break;
                    case ScanStatus.SCANNING:
                        previousMessage = "Scaning........";
                        Debug.WriteLine(previousMessage);
                        break;
                    case ScanStatus.SCAN_COMPLETE:
                        previousMessage = "Scan Complete";
                        Debug.WriteLine(previousMessage);
                        //ExternalTriggerGrabStop();
                        ReceiveImage?.Invoke(-1);
                        break;
                    case ScanStatus.PAGE_COMPLETE:
                        previousMessage = "Page Complete";
                        Debug.WriteLine(previousMessage);
                        ReceiveImage?.Invoke(_UserHookData.ProcessedImageCount - 1);
                        _UserHookData.Status = ScanStatus.SCANNING;
                        if (_UserHookData.ProcessedImageCount == BUF_SIZE)
                            _UserHookData.Status = ScanStatus.SCAN_NONE;
                        break;
                    default:
                        break;
                }
            }

            #region AllocBuffer Gray

            public bool AllocBuffer_Area(int nDevNo)
            {
                bool result = false;


                if (IsInitialize == false)
                {
                    return result;
                }

                if (CheckCamera(nDevNo) == false)
                {
                    return result;
                }

                if (_UserHookData.Status == ScanStatus.SCANNING)
                {
                    return result;
                }

                if (_MilGrabImage[nDevNo] != MIL.M_NULL)
                    MIL.MbufFree(_MilGrabImage[nDevNo]);

                ImageHeight = ImageHeightDefault;
                MIL.MbufAlloc2d(_MilSystem[nDevNo/2], ImageWidth[nDevNo], ImageHeightDefault[nDevNo], 8, MIL.M_IMAGE + MIL.M_GRAB + MIL.M_PROC, ref _MilGrabImage[nDevNo]);

                result = true;
                return result;
            }
            #endregion

            #region Live

            /// <summary>
            /// Live Start
            /// </summary>
            public void ContinuousGrabStart(int nDevNo)
            {
                if (_MilGrabImage[nDevNo] != MIL.M_NULL)
                {
                    MIL.MdigGrabContinuous(_MilDigitizer[nDevNo], _MilGrabImage[nDevNo]);
                }
            }
            /// <summary>
            /// Live Stop
            /// </summary>
            public void ContinuousGrabStop(int nDevNo)
            {
                if (_MilGrabImage[nDevNo] != MIL.M_NULL)
                {
                    MIL.MdigHalt(_MilDigitizer[nDevNo]);
                }
            }
            #endregion

            #region Grab

            /// <summary>
            /// One Shot Grab
            /// </summary>
            /// <returns></returns>
            public byte[] Grab(int nDevNo)
            {
                MIL.MdigGrab(_MilDigitizer[nDevNo], _MilGrabImage[nDevNo]);
                UInt32 BuffSize = (UInt32)(ImageWidth[nDevNo] * ImageHeight[nDevNo]);
                byte[] Imagebuff = new byte[BuffSize];
                MIL.MbufGet2d(_MilGrabImage[nDevNo], 0, 0, ImageWidth[nDevNo], (MIL_INT)ImageHeightDefault[nDevNo], Imagebuff);
                return Imagebuff;
            }
            #endregion

            #region Get Image Byte

            /// <summary>
            /// Get Buffer Full Image 
            /// </summary>
            /// <returns>image byte</returns>
            public byte[] GetImageBytes(int nDevNo)
            {
                if (IsInitialize && ImageWidth[nDevNo] != 0 && ImageHeight[nDevNo] != 0)
                {
                    UInt32 buffSize = (UInt32)(ImageWidth[nDevNo] * ImageHeight[nDevNo]);
                    byte[] imageBuffer = new byte[buffSize];
                    MIL.MbufGet2d(_MilGrabImage[nDevNo], 0, 0, ImageWidth[nDevNo], (MIL_INT)ImageHeight[nDevNo], imageBuffer);
                    return imageBuffer;
                }
                return null;
            }
            #endregion

            public bool CheckCamera(int nDevNo)
            {
                MIL_ID result = MIL.M_NO;
                MIL.MdigInquire(_MilDigitizer[nDevNo], MIL.M_CAMERA_PRESENT, ref result);
                if (result == MIL.M_YES)
                    return true;
                else
                    return false;
            }

            static MIL_INT ProcessingFunction(MIL_INT HookType, MIL_ID HookId, IntPtr HookDataPtr)
            {
                MIL_ID ModifiedBufferId = MIL.M_NULL;

                if (!IntPtr.Zero.Equals(HookDataPtr))
                {
                    GCHandle hUserData = GCHandle.FromIntPtr(HookDataPtr);          // get the handle to the DigHookUserData object back from the IntPtr
                    HookDataStruct UserData = hUserData.Target as HookDataStruct;   // get a reference to the DigHookUserData object
                    MIL.MdigGetHookInfo(HookId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);

                    UserData.ProcessedImageCount++;
                    if (UserData.RemainLineCount >= UserData.PageUnit)
                    {
                        UserData.RemainLineCount -= UserData.PageUnit;
                        UserData.Status = ScanStatus.PAGE_COMPLETE;
                        CheckHookDataState?.Invoke();
                    }
                    else
                    {
                        UserData.RemainLineCount -= UserData.RemainLineCount;
                        UserData.Status = ScanStatus.SCAN_COMPLETE;
                        CheckHookDataState?.Invoke();
                    }
                    Debug.WriteLine($"frame : #{UserData.ProcessedImageCount}, RamainLine : #{UserData.RemainLineCount}");

                }
                return 0;
            }


            public void SimualtionModeStart()
            {
                _isSimualtionMode = true;
            }
            public void SimualtionModeStop()
            {
                _isSimualtionMode = false;
            }
        }

        public class HookDataStruct
        {
            public MIL_ID MilImageDisp;
            public int ProcessedImageCount;
            public int RemainLineCount;
            public int PageUnit;
            public ScanStatus Status;
        };

#endif
    }
}
