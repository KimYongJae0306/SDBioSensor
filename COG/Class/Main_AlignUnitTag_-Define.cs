using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics; //Timer
using System.Runtime.InteropServices; //DllImport
using System.IO;
using System.Collections.Generic;
using System.Drawing;

using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Dimensioning; //길이측정시
using Cognex.VisionPro.CNLSearch;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.LineMax;

namespace COG
{
    public partial class Main
    {
        public static bool BLOBINSPECTION_USE(int m_AlignNo)
        {
            bool TempReturn = false;

            if (Main.AlignUnit[m_AlignNo].m_AlignName == "CRD_PRE1"
             || Main.AlignUnit[m_AlignNo].m_AlignName == "CRD_PRE2"
             || Main.AlignUnit[m_AlignNo].m_AlignName == "CRD_PRE3"
             || Main.AlignUnit[m_AlignNo].m_AlignName == "CRD_PRE4"
             || Main.AlignUnit[m_AlignNo].m_AlignName == "LOW_INSPECT"
             || Main.AlignUnit[m_AlignNo].m_AlignName == "UPPER_INSPECT"
                
                
                )
            {
                if (Main.AlignUnit[m_AlignNo].m_PatTagNo >= 0) //2번 태크 부터 검사. 0,1번은 얼라인이여서 . 
                    TempReturn = true;
            }
            return TempReturn;
        }
        public static bool ALIGNINSPECTION_USE(int m_AlignNo)
        {
            bool TempReturn = false;
            if (Main.AlignUnit[m_AlignNo].m_AlignName == "AOI_INSPECTION" 
                || Main.AlignUnit[m_AlignNo].m_AlignName == "FOB_INSPECT"
                || Main.AlignUnit[m_AlignNo].m_AlignName == "FOF_INSPECTION1"
                || Main.AlignUnit[m_AlignNo].m_AlignName == "FOF_INSPECTION2"
                || Main.AlignUnit[m_AlignNo].m_AlignName == "FOP_INSPECTION1") TempReturn = true;
            return TempReturn;
        }

        public class AlignUnitTags
        {
            private AlignUnitTag[] Unit = new AlignUnitTag[Main.DEFINE.AlignUnit_Max];
            public AlignUnitTag this[int Index]
            {
                get
                {
                    return Unit[Index];
                }
                set
                {
                    Unit[Index] = value;
                }
            }
            public AlignUnitTag this[string Unit_Name]
            {
                get
                {
                    for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
                    {
                        if (Unit[i].m_AlignName == Unit_Name)
                        {
                            return Unit[i];
                        }
                    }
                    return null;
                }
                set
                {
                    for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
                    {
                        if (Unit[i].m_AlignName == Unit_Name)
                        {
                            Unit[i] = value;
                        }
                    }
                }
            }
        }

        public static AlignUnitTags AlignUnit = new AlignUnitTags();
        public static void AlignUnit_Initial()
        {
            #region ALIGN UNIT TAG 초기화

 //           AlignUnit = new AlignUnitTag[DEFINE.AlignUnit_Max];

            for (int i = 0; i < DEFINE.AlignUnit_Max; i++)
            {
                AlignUnit[i] = new AlignUnitTag();
                AlignUnit[i].m_AlignNo = i;
                AlignUnit[i].Aligndata = new DAlignData[DEFINE.PATTERNTAG_MAX];
                AlignUnit[i].PAT = new PatternTag[DEFINE.PATTERNTAG_MAX, 4];

                if (Main.DEFINE.NON_STANDARD)
                {
                    AlignUnit[i].m_DirX = -1;
                    AlignUnit[i].m_DirY = 1;
                    AlignUnit[i].m_DirT = -1;
                }
                else
                {
                    AlignUnit[i].m_DirX = 1;
                    AlignUnit[i].m_DirY = 1;
                    AlignUnit[i].m_DirT = 1;
                }

                if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                {
                    if (i % 2 == 0) // Cam 1, 3
                    {
                        AlignUnit[i].UNIT_RES_SIGNAL_ADDR = DEFINE.CCLINK_OUT_STAGE1_CAM1_SEARCH_COMP_X;
                        AlignUnit[i].UNIT_RES_DATA_ADDR = DEFINE.CCLINK_WW_CAMERA1_SEARCH1_X;
                    }
                    else
                    {
                        AlignUnit[i].UNIT_RES_SIGNAL_ADDR = DEFINE.CCLINK_OUT_STAGE1_CAM2_SEARCH_COMP_X;
                        AlignUnit[i].UNIT_RES_DATA_ADDR = DEFINE.CCLINK_WW_CAMERA2_SEARCH1_X;
                    }
                }
                else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                {
                    if (i == 0)
                    {
                        AlignUnit[i].UNIT_RES_SIGNAL_ADDR = DEFINE.CCLINK2_OUT_FIRST_ALIGN_SEARCH_COMP_X;
                        AlignUnit[i].UNIT_RES_DATA_ADDR = DEFINE.CCLINK2_WW_FIRST_ALIGN_SEARCH_X;
                    }
                    else if (i == 1)
                    {
                        AlignUnit[i].UNIT_RES_SIGNAL_ADDR = DEFINE.CCLINK2_OUT_PRE_ALIGN1_SEARCH_COMP_X;
                        AlignUnit[i].UNIT_RES_DATA_ADDR = DEFINE.CCLINK2_WW_PRE_ALIGN_SEARCH1_X;
                    }
                }

                AlignUnit[i].ALIGN_UNIT_ADDR = (10 + (i * 10)); //------------------
                for (int jj = 0; jj < DEFINE.PATTERNTAG_MAX; jj++)
                {
                    AlignUnit[i].Aligndata[jj] = new DAlignData();
                    AlignUnit[i].m_LogStageX[jj] = new List<double>();
                    AlignUnit[i].m_LogStageY[jj] = new List<double>();
                    AlignUnit[i].m_LogStageT[jj] = new List<double>();

                    AlignUnit[i].m_LogCenterX[jj] = new List<double>();
                    AlignUnit[i].m_LogCenterY[jj] = new List<double>();
                    AlignUnit[i].m_LogCenterT[jj] = new List<double>();

                    for (int j = 0; j < 4; j++)
                    {
                        AlignUnit[i].PAT[jj, j] = new PatternTag();
                        AlignUnit[i].PAT[jj, j].m_PatNo = j;
                        AlignUnit[i].PAT[jj, j].m_PatAlign_No = i;
                        for (int k = 0; k < DEFINE.SUBPATTERNMAX; k++)
                        {
                            AlignUnit[i].PAT[jj, j].Pattern[k] = new CogSearchMaxTool();
                            AlignUnit[i].PAT[jj, j].GPattern[k] = new CogPMAlignTool();
                        }
                        for (int k = 0; k < DEFINE.CALIPER_MAX; k++)
                        {
                            AlignUnit[i].PAT[jj, j].CaliperTools[k] = new CogCaliperTool();
                            AlignUnit[i].PAT[jj, j].CaliperPara[k] = new CaliperTagData();
                            for (int l = 0; l < DEFINE.M_TOOLMAXCOUNT; l++)
                            {
                                AlignUnit[i].PAT[jj, j].CaliperPara[k].m_TargetToCenter[l] = new DoublePoint();
                            }

                            AlignUnit[i].PAT[jj, j].CaliResults[k] = new CaliperResult();
                            AlignUnit[i].PAT[jj, j].CaliResults[k].m_Index = k;
                            AlignUnit[i].PAT[jj, j].CaliResults[k].m_AlignNum = i;
                            AlignUnit[i].PAT[jj, j].CaliResults[k].m_PatTagNum = jj;
                            AlignUnit[i].PAT[jj, j].CaliResults[k].m_PatNum = j;
                        }
                        for (int k = 0; k < DEFINE.BLOB_CNT_MAX; k++)
                        {
                            AlignUnit[i].PAT[jj, j].BlobTools[k] = new CogBlobTool();
                            AlignUnit[i].PAT[jj, j].BlobPara[k] = new BlobTagData();
                            for (int l = 0; l < DEFINE.M_TOOLMAXCOUNT; l++)
                            {
                                AlignUnit[i].PAT[jj, j].BlobPara[k].m_TargetToCenter[l] = new DoublePoint();
                            }

                            AlignUnit[i].PAT[jj, j].BlobResults[k] = new BlobResult();
                            AlignUnit[i].PAT[jj, j].BlobResults[k].m_Index = k;
                            AlignUnit[i].PAT[jj, j].BlobResults[k].m_AlignNum = i;
                            AlignUnit[i].PAT[jj, j].BlobResults[k].m_PatTagNum = jj;
                            AlignUnit[i].PAT[jj, j].BlobResults[k].m_PatNum = j;
                        }

                        for (int k = 0; k < DEFINE.FINDLINE_MAX; k++)
                        {
                            for (int kk = 0; kk < DEFINE.SUBLINE_MAX; kk++)
                            {
                                AlignUnit[i].PAT[jj, j].FINDLineTools[kk, k] = new CogFindLineTool();
                                AlignUnit[i].PAT[jj, j].FINDLinePara[kk, k] = new FindLineTagData();
                                AlignUnit[i].PAT[jj, j].FINDLinePara[kk, k].m_LinePosition = (ushort)(1 << k);    // 200716 JHKIM Line Index 부여
                                AlignUnit[i].PAT[jj, j].LineMaxTools[kk, k] = new CogLineMaxTool();
                                for (int l = 0; l < DEFINE.M_TOOLMAXCOUNT; l++)
                                {
                                    AlignUnit[i].PAT[jj, j].FINDLinePara[kk, k].m_TargetToCenter[l] = new DoublePoint();
                                }
                            }
                            AlignUnit[i].PAT[jj, j].FINDLineResults[k] = new FindLineResult();
                            AlignUnit[i].PAT[jj, j].FINDLineResults[k].m_Index = k;
                            AlignUnit[i].PAT[jj, j].FINDLineResults[k].m_AlignNum = i;
                            AlignUnit[i].PAT[jj, j].FINDLineResults[k].m_PatTagNum = jj;
                            AlignUnit[i].PAT[jj, j].FINDLineResults[k].m_PatNum = j;
                        }

                        //                         for (int k = 0; k < DEFINE.TRAY_POCKET_LIMIT; k++)
                        //                         {
                        //                             AlignUnit[i].PAT[jj, j].TRAYResults[k] = new PMResult();
                        // 
                        //                         }
                        for (int k = 0; k < DEFINE.CIRCLE_MAX; k++)
                        {
                            AlignUnit[i].PAT[jj, j].CircleTools[k] = new CogFindCircleTool();
                            AlignUnit[i].PAT[jj, j].CirclePara[k] = new FindCircleTagData();
                            AlignUnit[i].PAT[jj, j].CircleResults[k] = new CircleResult();
                            for (int l = 0; l < DEFINE.M_TOOLMAXCOUNT; l++)
                            {
                                AlignUnit[i].PAT[jj, j].CirclePara[k].m_TargetToCenter[l] = new DoublePoint();
                            }

                            AlignUnit[i].PAT[jj, j].CircleResults[k] = new CircleResult();
                            AlignUnit[i].PAT[jj, j].CircleResults[k].m_Index = k;
                            AlignUnit[i].PAT[jj, j].CircleResults[k].m_AlignNum = i;
                            AlignUnit[i].PAT[jj, j].CircleResults[k].m_PatTagNum = jj;
                            AlignUnit[i].PAT[jj, j].CircleResults[k].m_PatNum = j;

                        }

                        for (int k = 0; k < DEFINE.CORNER_MAX; k++)
                        {
                            AlignUnit[i].PAT[jj, j].CornerTools[k] = new CogFindCornerTool();
                            AlignUnit[i].PAT[jj, j].CornerResults[k] = new CornerResult();
                            for (int l = 0; l < DEFINE.M_TOOLMAXCOUNT; l++)
                            {
                                //AlignUnit[i].PAT[jj, j].FINDLinePara[k].m_TargetToCenter[l] = new DoublePoint();
                            }

                            AlignUnit[i].PAT[jj, j].CornerResults[k] = new CornerResult();
                            AlignUnit[i].PAT[jj, j].CornerResults[k].m_Index = k;
                            AlignUnit[i].PAT[jj, j].CornerResults[k].m_AlignNum = i;
                            AlignUnit[i].PAT[jj, j].CornerResults[k].m_PatTagNum = jj;
                            AlignUnit[i].PAT[jj, j].CornerResults[k].m_PatNum = j;
                        }


                    }//pattern
                }//PatternTag
            } //AlignUnit_Max
            
            #endregion
            AlignUnit_Initial_UNIT();
            #region
            for (int i = 0; i < DEFINE.AlignUnit_Max; i++)
            {
                item.Add(AlignUnit[i].m_AlignName, AlignUnit[i].m_AlignNo);
            }


            #endregion

        } //AlignUnit_Initial

        public partial class AlignUnitTag
        {

            public DAlignData[] Aligndata;
            public int m_AlignNo = 0;
            public int m_PatTagNo = 0;
            //            public int m_AlignType = 0;
            public int[] m_AlignType = new int[Main.DEFINE.PATTERNTAG_MAX];
            public int m_AlignPatTagMax = 0;
//            public int m_AlignPatMax = 0;
            public int[] m_AlignPatMax = new int[DEFINE.PATTERNTAG_MAX];
            public String m_AlignName = "";
            public int m_Cmd = 0;
            public PatternTag[,] PAT;   //[Inspeciton, Align Inspecton] 
            public List<Main.PatternTag.SDParameter> m_listSDParmet;
            public int m_errSts = -1;
            public int m_Point_Num = 0;

            public bool[] m_MOT_NOT_USE = new bool[4];

            public long m_Cal_MOVE_X, m_Cal_MOVE_Y, m_Cal_MOVE_T1, m_Cal_MOVE_T2;
            public int m_DirX = 1, m_DirY = 1, m_DirT = 1;
            public bool m_bCalibMode = false;

            public string m_TabNum;
            public string m_Cell_ID, m_Cell_ID_Temp;
            public string W_Cell_ID, Bkup_W_Cell_ID;
            public int CellNum;
            public bool DOPOGI_TYPE_2POINT = false;

            public int m_ManualMatchPatTagNum = 0;
            public int m_ManualMatchPatNum = 0;
            public bool m_ManualMatchFlag = false;
            public bool m_ManualMatchRunning = false;
            public bool m_ManualMatchResult = false;
            public int m_ManualMatchLineType = 0;
            public ushort m_ManualResult = 0;

            public bool m_NgImage_MonitorFlag = false;
            public bool m_Blob_NG_View_Use = false;


            public bool m_GD_ImageSave_Use = false;
            public bool m_NG_ImageSave_Use = false;

            public int m_AlignDelay = 0;

            public bool m_LengthCheck_Use = false;
            public double[] m_Length_Standard = new double[4];
            public double m_Length_Tolerance = new double();  //TOLERANCE

            public double m_OBJ_Standard_Length;
            public double m_TAR_Standard_Length;

            //          public int DrawAll_Pat;
            public int[] DrawAll_Pat = new int[Main.DEFINE.PATTERNTAG_MAX];

            public int m_RepeatLimit;
            public long m_AxisCamX1, m_AxisCamX2;
            public double m_AxisX, m_AxisY, m_AxisT;

            //          public long m_AxisX1, m_AxisX2, m_AxisY1, m_AxisY2; //X1 , X2 , Y1 

            public long m_CAL_MotorPos; //도포기 -> Trans , 합착기 ->Out PnP

            public long m_Stage_U, m_Stage_V, m_Stage_W;
            public double m_StageX, m_StageY, m_StageT;
            public long m_StageX1, m_StageX2, m_StageY1, m_StageY2;

            public long m_CRD_Pitch_Y;

            public double m_OffsetX, m_OffsetY, m_OffsetT;

            public double m_OffsetX2, m_OffsetY2, m_OffsetT2;

            public double m_PBD_ReAlignY;

            public int m_STAGE_NUM;

            public int PBD_OBJ_L_SCORE, PBD_OBJ_R_SCORE, PBD_TAR_L_SCORE , PBD_TAR_R_SCORE;

            public List<double>[] m_LogStageX = new List<double>[Main.DEFINE.PATTERNTAG_MAX];
            public List<double>[] m_LogStageY = new List<double>[Main.DEFINE.PATTERNTAG_MAX];
            public List<double>[] m_LogStageT = new List<double>[Main.DEFINE.PATTERNTAG_MAX];

            public List<double>[] m_LogCenterX = new List<double>[Main.DEFINE.PATTERNTAG_MAX];
            public List<double>[] m_LogCenterY = new List<double>[Main.DEFINE.PATTERNTAG_MAX];
            public List<double>[] m_LogCenterT = new List<double>[Main.DEFINE.PATTERNTAG_MAX];


            public double[] m_CenterX = new double[Main.DEFINE.PATTERNTAG_MAX];
            public double[] m_CenterY = new double[Main.DEFINE.PATTERNTAG_MAX];

            public double[] m_CalMotoPosX = new double[Main.DEFINE.PATTERNTAG_MAX];
            public double[] m_CalMotoPosY = new double[Main.DEFINE.PATTERNTAG_MAX];

            public double[] m_CalDisplayCX = new double[Main.DEFINE.PATTERNTAG_MAX];
            public double[] m_CalDisplayCY = new double[Main.DEFINE.PATTERNTAG_MAX];

            public double[] m_CALC_Center_X = new double[2];
            public double[] m_CALC_Center_Y = new double[2];
            public double[] m_CALC_Center_T = new double[2];  // 4 카메라의 중심 위치 + 찾은 위치 의 중심

            public double[] m_CAMFixPosCenter_X = new double[2];
            public double[] m_CAMFixPosCenter_Y = new double[2];          // 4 카메라의 중심 위치

            public double m_Current_T; // 현재 UVW Theta 값

            public double[] m_Standard = new double[3];

            public double m_GapAmountX;
            public double m_GapAmountY;
            public double m_GapAmountT;

            public int nMotResol = 1;

            public double m_StandardMark_T = 0;

            public long[] m_MAP_ResultData = new long[2];


            public double[] m_MotStagePosX = new double[4];
            public double[] m_MotStagePosY = new double[4];
            public double[] m_MotStagePosT = new double[4];

            public double[] m_CALMotStagePosX = new double[4];
            public double[] m_CALMotStagePosY = new double[4];

            public double m_CamDistX1, m_CamDistX2;
            public double m_CamDistY1, m_CamDistY2;

            public long m_CamOffsetX, m_CamOffsetY;   // BP, Align - Inspection Camera Center Distance Offset in um

            public double m_OBJ_Distance, m_TAR_Distance;
            public double m_OBJ_Mea_Dis, m_TAR_Mea_Dis; // 



            public double[,] m_CALC_DistX = new double[4, 4];
            public double[,] m_CALC_DistY = new double[4, 4];

            public double[] m_InspectionSpec = new double[8];
            public double[] m_Inspec_Tolerance = new double[2]; //TOLERANCE 0:Width 1:Height
            public double[] m_InspecResult = new double[8];
            // QD OLED LPA INSPECTION
            // m_InspecResult[0] = Pad 및 Round에서 Glass-POL Cutting Horizontal Line Gap (Y-Y)
            // m_InspecResult[1] = Pad에서는 POL 부착 공차 / Round에서는 Glass-POL Cutting Vertical Line Gap (X-X)
            // m_InspecResult[2] = Pad 무시 / Round에서 C-Cut Gap 혹은 R-Cut Gap
            // m_InspecResult[3] ~ [7] 무시

            public int ALIGN_UNIT_ADDR;
            public int PLC_CIM_BASE_ADDR;
            public ushort UNIT_RES_DATA_ADDR;
            public ushort UNIT_RES_SIGNAL_ADDR;

            public Queue<string> m_LogString = new Queue<string>();
            public Queue<string[]> m_MainGridString = new Queue<string[]>();
            public int nSearchPosition = 0;     // Inspection Grid 위치 넘기기 변수

            public Queue<string> m_LogStringLength = new Queue<string>();

            private MTickTimer m_Timer = new MTickTimer();
            private MTickTimer m_Timer_index = new MTickTimer();
            private MTickTimer m_TimerInOut = new MTickTimer();
            private MTickTimer m_TimerCommCheck = new MTickTimer();

            private DateTime m_InTime = new DateTime();
            private DateTime m_OutTime = new DateTime();

            public long m_lProcTime = 0;
            public long m_lInOutTime = 0;
            
            public int m_Change_Dis = 0;        // MainTab 유닛별 체인지 상태 변수

            public double[] m_Line_Length = new double[8];
            public double[] m_DistanceRet = new double[8];
            //-----------------------ALIGN INSPECTION-------------------------------
            public double[,] m_AOI_Length = new double[2, 2];
            //            public string[] m_Message = new string[2];
            public List<string> m_Message = new List<string>();
            public double m_AOI_ExtensionX = new double();

            public List<DoublePoint> m_PatternPoints = new List<DoublePoint>();
            public int m_PatternNumber = 0;
            public bool m_GrabNotUse = false;
            public bool m_DrawNotUse = false;

            public int m_Tray_Pocket_X = 1;
            public int m_Tray_Pocket_Y = 1;

            public int m_Chip_SearchCount = 0;


            public bool TrayBlobMode = false;

            public List<TrayResultData> m_TrayResultData = new List<TrayResultData>();
            public bool FOFCenterMode = false;
            //---------------------------------------------------------------------------
            public bool m_RobotAlign = false;
            public bool CRD_Mode = false;
            public bool M_ReversAxis_X = false; //CRD Cameara X Revers

            public bool ThetaAlignNotuse = false;

            public bool M_ACFCUT_MODE= false;
			string nMsgMarks = "";

            public bool m_UnitBusy = false;
            public bool m_bDisplayStatus = false;

            private FileStream m_FStreamAlign;
            private StreamWriter m_SWriterAlign;

            private FileStream m_FStreamInsp;
            private StreamWriter m_SWriterInsp;
        }//AlighUnit
        public class TrayResultData
        {
            public DoublePoint PixelPoint = new DoublePoint();
            public DoublePoint RobotPoint = new DoublePoint();
            public DoublePoint AlignData = new DoublePoint();

            public bool SearchResult;
        }
    } //Main







} // COG
