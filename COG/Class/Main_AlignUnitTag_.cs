//#define SD_BIO_VENT
//#define SD_BIO_PATH

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics; //Timer
using System.Runtime.InteropServices; //DllImport
using System.IO;



using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Implementation.Internal;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.CNLSearch;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.Dimensioning;
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.LineMax;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV;

namespace COG
{
    public partial class Main
    {
        public enum enumROIType { FindLine = 0, FindCircle = 1 }
        public enum enumResultType { InspectionOK = 0, MarkSearchNG = 1, CrossLineNG = 2, InspectionNG = 3 }

        public static Stopwatch TestSW = new Stopwatch();
        public partial class AlignUnitTag
        {
            public ManualResetEvent InspectionDoneEvent = new ManualResetEvent(true);
            public bool m_bThetaNGFlag;
            public bool m_bMarkNGFlag;
            public bool m_bFilmNGFlag;
            public bool m_bThetaMarkNGFlag;

            private int m_DistIgnoreCnt = 0;
            public void PatAlloc(int AlignType, string AlignName, int PatternTagMax, int CAM0, int CAM1)
            {
                m_AlignName = AlignName;
                if (PatternTagMax > Main.DEFINE.PATTERNTAG_MAX) m_AlignPatTagMax = Main.DEFINE.PATTERNTAG_MAX;
                else m_AlignPatTagMax = PatternTagMax;

                for (int i = 0; i < PatternTagMax; i++)
                {
                    m_AlignPatMax[i] = 1;
                    m_AlignType[i] = AlignType;

                    // PAT[i, 0].PatAlloc(CAM0, AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName0);
                    // PAT[i, 1].PatAlloc(CAM1, AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName1);
                    PAT[i, 0].PatAlloc(CAM0, m_AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName0);
                    PAT[i, 1].PatAlloc(CAM1, m_AlignName + "ALIGN INSPECTION");
                }
            }

            public void PatAlloc(int AlignType, string AlignName, int PatternTagMax, int CAM0, int CAM1, int CAM2)
            {
                string nName_0, nName_1, nName_2;

                m_AlignName = AlignName;
                if (PatternTagMax > Main.DEFINE.PATTERNTAG_MAX) m_AlignPatTagMax = Main.DEFINE.PATTERNTAG_MAX;
                else m_AlignPatTagMax = PatternTagMax;

                for (int i = 0; i < PatternTagMax; i++)
                {
                    nName_0 = DEFINE.OBJName + DEFINE.PatName0; nName_1 = DEFINE.OBJName + DEFINE.PatName1;
                    nName_2 = DEFINE.TARName + DEFINE.PatName0;

                    if ((DEFINE.PROGRAM_TYPE == "TFOF_PC1") && m_AlignName == "LOAD_ALIGN")
                    {
                        nName_2 = "_PICKUP_CENTER";
                    }

                    m_AlignPatMax[i] = 3;
                    m_AlignType[i] = AlignType;
                    PAT[i, 0].PatAlloc(CAM0, AlignName + "_" + i.ToString() + nName_0);
                    PAT[i, 1].PatAlloc(CAM1, AlignName + "_" + i.ToString() + nName_1);
                    PAT[i, 2].PatAlloc(CAM2, AlignName + "_" + i.ToString() + nName_2);
                }
            }

            public void PatAlloc(int AlignType, string AlignName, int PatternTagMax, int CAM0, int CAM1, int CAM2, int CAM3)
            {
                m_AlignName = AlignName;
                if (PatternTagMax > Main.DEFINE.PATTERNTAG_MAX) m_AlignPatTagMax = Main.DEFINE.PATTERNTAG_MAX;
                else m_AlignPatTagMax = PatternTagMax;

                //                m_AlignType = AlignType;
                for (int i = 0; i < PatternTagMax; i++)
                {
                    m_AlignPatMax[i] = 4;
                    m_AlignType[i] = AlignType;
                    PAT[i, 0].PatAlloc(CAM0, AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName0);
                    PAT[i, 1].PatAlloc(CAM1, AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName1);
                    PAT[i, 2].PatAlloc(CAM2, AlignName + "_" + i.ToString() + DEFINE.TARName + DEFINE.PatName0);
                    PAT[i, 3].PatAlloc(CAM3, AlignName + "_" + i.ToString() + DEFINE.TARName + DEFINE.PatName1);
                }
            }
            public void PatAlloc(int[] AlignType, string AlignName, int PatternTagMax, int[] CAM0, int[] CAM1, int[] CAM2, int[] CAM3)
            {
                m_AlignName = AlignName;
                if (PatternTagMax > Main.DEFINE.PATTERNTAG_MAX) m_AlignPatTagMax = Main.DEFINE.PATTERNTAG_MAX;
                else m_AlignPatTagMax = PatternTagMax;

                for (int i = 0; i < PatternTagMax; i++)
                {
                    m_AlignPatMax[i] = 4;
                    m_AlignType[i] = AlignType[i];
                    PAT[i, 0].PatAlloc(CAM0[i], AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName0); //PANEL
                    PAT[i, 1].PatAlloc(CAM1[i], AlignName + "_" + i.ToString() + DEFINE.OBJName + DEFINE.PatName1);
                    PAT[i, 2].PatAlloc(CAM2[i], AlignName + "_" + i.ToString() + DEFINE.TARName + DEFINE.PatName0);
                    PAT[i, 3].PatAlloc(CAM3[i], AlignName + "_" + i.ToString() + DEFINE.TARName + DEFINE.PatName1);
                }
            }
            public void DisplayAlloc(int PatternTagNum, int[] DisplayNum)
            {
                for (int i = 0; i < DisplayNum.Length; i++)
                {
                    PAT[PatternTagNum, i].m_DisNo = DisplayNum[i];
                }
            }
            public void DisplayAlloc(int[,] DisplayNum)
            {
                for (int i = 0; i < m_AlignPatTagMax; i++)
                {
                    for (int j = 0; j < DisplayNum.Length / m_AlignPatTagMax; j++)
                    {
                        PAT[i, j].m_DisNo = DisplayNum[i, j];
                    }
                }
            }
            public void DisplayAlloc(int[] DisplayNum)
            {
                for (int i = 0; i < Main.DEFINE.PATTERNTAG_MAX; i++)
                {
                    for (int j = 0; j < DisplayNum.Length; j++)
                    {
                        PAT[i, j].m_DisNo = DisplayNum[j];
                    }
                }
            }
            public void MatrixNotUse()
            {
                for (int i = 0; i < m_AlignPatTagMax; i++)
                {
                    for (int j = 0; j < m_AlignPatMax[i]; j++)
                    {
                        PAT[i, j].CALMATRIX_NOTUSE = true;
                    }
                }
            }
            public void MatrixNotUse(bool MOT_X, bool MOT_Y, bool MOT_T1, bool MOT_T2) //사용 USE 미사용 NOTUSE
            {
                m_MOT_NOT_USE[Main.DEFINE.X] = !MOT_X;
                m_MOT_NOT_USE[Main.DEFINE.Y] = !MOT_Y;
                m_MOT_NOT_USE[Main.DEFINE.T] = !MOT_T1;
                m_MOT_NOT_USE[Main.DEFINE.T2] = !MOT_T2;

                for (int i = 0; i < m_AlignPatTagMax; i++)
                {
                    for (int j = 0; j < m_AlignPatMax[i]; j++)
                    {
                        PAT[i, j].CALMATRIX_NOTUSE = true;
                    }
                }
            }

            public void CALMOTORNotUse(bool MOT_X, bool MOT_Y, bool MOT_T1, bool MOT_T2) //사용 USE 미사용 NOTUSE
            {
                m_MOT_NOT_USE[Main.DEFINE.X] = !MOT_X;
                m_MOT_NOT_USE[Main.DEFINE.Y] = !MOT_Y;
                m_MOT_NOT_USE[Main.DEFINE.T] = !MOT_T1;
                m_MOT_NOT_USE[Main.DEFINE.T2] = !MOT_T2;
            }

            private void GetImage(List<int> nPatNum)
            {

                int seq = 0;
                bool LoopFlag = true;


                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:

                            seq++;
                            break;

                        case 1:
                            if (Main.DEFINE.OPEN_F)
                            {
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].TempImage = new CogImage8Grey(Main.vision.CogCamBuf[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] as CogImage8Grey);
                                }
                                return;
                            }
                            seq++;
                            break;

                        case 2:

                            seq++;
                            break;

                        case 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                Main.vision.Grab_Flag_End[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] = false;
                                Main.vision.Grab_Flag_Start[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] = true;
                            }
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case 4:
                            if (m_Timer.GetElapsedTime() > DEFINE.GRAB_TIMEOUT)
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            bool GrabEnd = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!Main.vision.Grab_Flag_End[PAT[m_PatTagNo, nPatNum[i]].m_CamNo])
                                    GrabEnd = false;
                            }
                            if (!GrabEnd)
                                break;
                            seq++;
                            break;

                        case 5:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].TempImage = new CogImage8Grey(Main.vision.CogCamBuf[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] as CogImage8Grey);
                            }
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.ERROR_SEQ:
                            //                             for (int i = 0; i < nPatNum.Count; i++)
                            //                             {
                            //                                 Main.vision.Grab_Flag_End[PAT[m_PatTagNo,nPatNum[i]].m_CamNo] = true;
                            //                                 Main.vision.Grab_Flag_Start[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] = false;
                            //                             }
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ:
                            //                             for (int i = 0; i < nPatNum.Count; i++)
                            //                             {
                            //                                 Main.vision.Grab_Flag_End[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] = false;
                            //                             }
                            LoopFlag = false;
                            break;
                    }
                    //	Thread.Sleep(10);
                }
            }

            private int SetPatTagCmd(int nCmd)
            {
                int TempnCmd = nCmd;
                TempnCmd = Convert.ToInt16("1" + nCmd.ToString().Substring(1, nCmd.ToString().Length - 1));
                m_PatTagNo = (int)(nCmd / 1000) - 1;
                if (m_PatTagNo >= m_AlignPatTagMax)
                    TempnCmd = 0;

                return TempnCmd;
            }

            private int GetPatTagCmd(int nCmd)
            {
                int tempCmd = 0;
                int tempMod = 0;
                tempCmd = (int)(nCmd / 1000) * ((m_PatTagNo + 1) * 1000);
                Math.DivRem(nCmd, 1000, out tempMod);
                tempCmd += tempMod;

                return tempCmd;
            }

            public int CheckCameraSelect()
            {
                int nCamSel = -1;

                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_FINE_ALIGN_MODE))
                    nCamSel = DEFINE.CAM_SELECT_ALIGN;
                else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                    nCamSel = DEFINE.CAM_SELECT_INSPECT;

                return nCamSel;
            }

            public int CheckPadSelect()
            {
                int nJobSel = -1;

                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_PAD_CAM_SELECT))// UNIT 0, 1
                {
                    if (m_AlignNo >= 2) return nJobSel;

                    nJobSel = DEFINE.JOB_PANEL_PAD;
                }
                else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_ROUND_CAM_SELECT))// UNIT 2, 3
                {
                    if (m_AlignNo < 2) return nJobSel;

                    nJobSel = DEFINE.JOB_PANEL_ROUND;
                }

                return nJobSel;
            }

            public bool CheckGrabCommand()
            {
                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_PAD_CAM_SELECT))  // UNIT 0, 1
                {
                    if (m_AlignNo >= 2) return false;

                    if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_X) == false
                        && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_Y) == false
                        && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_X) == false
                        && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_Y) == false)
                    {
                        LogdataDisplay("CMD << GRAB_REQ) No X, Y Search Bit Error!", true);

                        return false;
                    }
                    else
                        return true;
                }
                else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_ROUND_CAM_SELECT))   // UNIT 2, 3
                {
                    if (m_AlignNo < 2) return false;

                    if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_X) == false
                        && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_Y) == false
                        && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_X) == false
                        && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_Y) == false)
                    {
                        LogdataDisplay("CMD << GRAB_REQ) No X, Y Search Bit Error!", true);

                        return false;
                    }
                    else
                        return true;
                }

                return false;
            }

            public void ExecuteCMD()
            {
                if (PAT[m_PatTagNo, 0].InspComplete)
                    return;

                InspectionDoneEvent.WaitOne();
                int status1 = 0, cmd;
                if (DEFINE.OPEN_F || DEFINE.OPEN_CAM)
                {
                    status1 = 0;
                }
                else
                {
                    m_Cmd = PLCDataTag.BData[ALIGN_UNIT_ADDR + DEFINE.PLC_STAGE1_CMD];

                    //m_Cmd = PLCDataTag.RData[];
                    status1 = PLCDataTag.BData[ALIGN_UNIT_ADDR + DEFINE.VIS_STATUS];
                    m_errSts = -1;
                    //        Light_Change();

                }


                if (status1 == 0 && m_Cmd > 0 && m_Cmd != 9000)
                {
                   
                    InspectionDoneEvent.Reset();
                    m_Timer_index.StartTimer();
                    m_Cmd = SetPatTagCmd(m_Cmd);
                    Log_Cmd(m_Cmd);

                    //CellDataRead();
                    //GetPosition();

                    cmd = m_Cmd;
                    switch (cmd)
                    {
                        #region SD BIO Inspection
                        case CMD.REQ_INSPECTION:
                            TimerRunningCheck.StartTimer();
                            bFirstStartRun = true;
                            //shkang_s      설정한 카운트만큼 Retry
                            if (!Inspection())
                            {                             
                                if (Main.machine.m_bRetryUse == true)   //Retry Use Check
                                {
                                    for (int i = 0; i < Main.machine.m_RetryCount; i++)
                                    {
                                        //Retry Log
                                        string LogMsg = " ";
                                        LogdataDisplay(LogMsg, true);
                                        LogMsg = string.Format("Inspection Retry {0:D}/{1:D}",i + 1,Main.machine.m_RetryCount);
                                        LogdataDisplay(LogMsg, true);

                                        if (!Inspection())
                                        {
                                            
                                            if (i == Main.machine.m_RetryCount - 1)
                                            {
                                                cmd = Math.Abs(cmd) * (-1); // 강제 OK로 해논거.. OK NG 판정하려면 * -1 하면됨 cyh
                                                break;
                                            }
                                        }
                                        else
                                            break;
                                    }
                                }
                                else   //Retry Not Use
                                {
                                    cmd = Math.Abs(cmd) * (-1); // 강제 OK로 해논거.. OK NG 판정하려면 * -1 하면됨 cyh
                                }
                            }

                            PAT[m_PatTagNo, 0].InspComplete = true;
                            //InspectionDoneEvent.Set();
                            //shkang_e         
                            break;
                        #endregion
                        #region PATTERN SEARCH SEQ
                        case CMD.OBJ_SEARCH_LEFT:
                            break;
                        case CMD.REEL_ALIGN:
                            if (!PATSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.OBJ_SEARCH_RIGHT:
                            if (!PATSearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.OBJ_SEARCH_ALL:
                            if (!PATSearch(DEFINE.OBJ_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.TAR_SEARCH_LEFT:
                            if (!PATSearch(DEFINE.TAR_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.TAR_SEARCH_RIGHT:
                            if (!PATSearch(DEFINE.TAR_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.TAR_SEARCH_ALL:
                            if (!PATSearch(DEFINE.TAR_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.LEFT_SEARCH_ALL:
                            if (!PATSearch(DEFINE.LEFT_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.RIGHT_SEARCH_ALL:
                            if (!PATSearch(DEFINE.RIGHT_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.OBJTAR_SEARCH_ALL:
                            if (!PATSearch(DEFINE.OBJTAR_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.GRAB_ALL:
                            if (!PATSearch(DEFINE.OBJ_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.GRAB_LEFT:
                            if (!PATSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.GRAB_RIGHT:
                            if (!PATSearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        #endregion

                        #region ALIGN SEQ_________
                        case CMD.ALIGN_OBJECT:          //1031
                            if (!AlignCenter(DEFINE.OBJ_ALL, true, 0)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_OBJECT_REALIGN:  //1032
                            if (!AlignCenter(DEFINE.OBJ_ALL, true, 1)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_TARGET:          //1035
                            if (!AlignCenter(DEFINE.TAR_ALL, true, 0)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_TARGET_REALIGN:  //1036
                            if (!AlignCenter(DEFINE.TAR_ALL, true, 1)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_OBJTAR_ALL:          //1037
                            if (!AlignCenter(DEFINE.OBJTAR_ALL, true, 0)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_OBJTAR_ALL_REALIGN:  //1038
                            if (!AlignCenter(DEFINE.OBJTAR_ALL, true, 1)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ALIGN_TRAY_SEARCH:     //1085
                            if (!PocketSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_TRAY_TRIGER:     //1040
                            if (!PocketTirger(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.BLOB_TRAY_SEARCH:     //1049
                            if (!PocketBLOBSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.BLOB_TRAY_TRIGER_PICKER:     //1050
                            if (!PocketBLOBTirger(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ALIGN_CENTER:          //1033
                            if (m_AlignName == "LOAD_ALIGN")
                            {
                                if (!AlignCenter(DEFINE.OBJTAR_ALL, false, 0)) cmd = Math.Abs(cmd) * -1;
                            }
                            else
                            {
                                if (!AlignCenter(DEFINE.OBJ_ALL, false, 0)) cmd = Math.Abs(cmd) * -1;
                            }
                            break;
                        case CMD.ALIGN_CENTER_REALIGN:  //1034
                            if (!AlignCenter(DEFINE.OBJ_ALL, false, 1)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.CRD_ALIGN_LEFT://1071
                        case CMD.ALIGN_1CAM_2SHOT_LEFT: //1043
                            if (!Align1Cam2Shot(DEFINE.OBJ_L, false)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CRD_ALIGN_RIGHT://1072
                        case CMD.ALIGN_1CAM_2SHOT_RIGHT://1044
                            if (!Align1Cam2Shot(DEFINE.OBJ_R, false)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ALIGN_1CAM_4SHOT_LEFT: //1053
                            if (!Align1Cam2Shot(DEFINE.OBJ_L, true)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_1CAM_4SHOT_RIGHT://1054
                            if (!Align1Cam2Shot(DEFINE.OBJ_R, true)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_1CAM_4SHOT_TAR_LEFT: //1055
                            if (!Align1Cam2Shot(DEFINE.TAR_L, true)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_1CAM_4SHOT_TAR_RIGHT://1056
                            if (!Align1Cam2Shot(DEFINE.TAR_R, true)) cmd = Math.Abs(cmd) * -1;
                            break;


                        case CMD.CALIPER_ALIGN_1CAM_2SHOT_LEFT: //1167
                            if (!Align1Cam2Shot_Caliper(DEFINE.OBJ_L, false)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CALIPER_ALIGN_1CAM_2SHOT_RIGHT: //1168
                            if (!Align1Cam2Shot_Caliper(DEFINE.OBJ_R, false)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CALIPER_ALIGN_CENTER:          //1169
                            if (!Align1Cam2Shot_Caliper(DEFINE.OBJ_ALL, false)) cmd = Math.Abs(cmd) * -1;
                            break;
                        #endregion

                        #region CALIPER_BLOB_CIRCLE_INSPECTION

                        case CMD.ACF_BLOB_LEFT:
                            if (!BLOBSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.DOPO_INSPECT_LEFT:
                            if (!BLOBSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            PAT[m_PatTagNo, Main.DEFINE.OBJ_L].SetAllLightOFF();// 이부분은 확인 해봐야됨. 
                            break;
                        case CMD.ACF_BLOB_RIGHT:
                            if (!BLOBSearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.DOPO_INSPECT_RIGHT:
                            if (!BLOBSearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            PAT[m_PatTagNo, Main.DEFINE.OBJ_R].SetAllLightOFF();// 이부분은 확인 해봐야됨. 
                            break;

                        case CMD.ACF_BLOB_OBJ_ALL:
                            if (!BLOBSearch(DEFINE.OBJ_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ACF_BLOB_TAR_LEFT:
                            if (!BLOBSearch(DEFINE.TAR_L)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ACF_BLOB_TAR_RIGHT:
                            if (!BLOBSearch(DEFINE.TAR_R)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ACF_BLOB_TAR_ALL:
                            if (!BLOBSearch(DEFINE.TAR_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.CALIPER_SEARCH_LEFT:
                        case CMD.CALIPER_SEARCH_LEFT_ACFCUT:
                            if (!CALISearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CALIPER_SEARCH_RIGHT:
                        case CMD.CALIPER_SEARCH_RIGHT_ACFCUT:
                            if (!CALISearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CALIPER_OBJ_SEARCH_ALL:
                            if (!CALISearch(DEFINE.OBJ_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.FINDLINE_OBJ_LEFT:
                            if (!LINESearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.FINDLINE_OBJ_RIGHT:
                            if (!LINESearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;



                        case CMD.ALIGN_INSPECT_LEFT:
                            if (!AlignInspection(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.ALIGN_INSPECT_RIGHT:
                            if (!AlignInspection(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.OBJ_CAL_POS_LEFT:
                            if (!CalPosBlob(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.OBJ_CAL_POS_RIGHT:
                            if (!CalPosBlob(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;

                        #region CIRCLE
                        case CMD.CIRCLE_OBJ_SEARCH_LEFT:
                            if (!CircleSearch(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CIRCLE_OBJ_SEARCH_RIGHT:
                            if (!CircleSearch(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CIRCLE_OBJ_SEARCH_ALL:
                            if (!CircleSearch(DEFINE.OBJ_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.CIRCLE_TAR_SEARCH_LEFT:
                            if (!CircleSearch(DEFINE.TAR_L)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CIRCLE_TAR_SEARCH_RIGHT:
                            if (!CircleSearch(DEFINE.TAR_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        case CMD.CIRCLE_TAR_SEARCH_ALL:
                            if (!CircleSearch(DEFINE.TAR_ALL)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ALIGN_CIRCLE_INSPECT_LEFT:
                            if (!CircleInspection(DEFINE.OBJ_L)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.ALIGN_CIRCLE_INSPECT_RIGHT:
                            if (!CircleInspection(DEFINE.OBJ_R)) cmd = Math.Abs(cmd) * -1;
                            break;
                        #endregion

                        #endregion

                        #region CALIBRATION
                        case CMD.OBJ_CALRIBRATION_LEFT:
                            if (cmd > 0) if (!Calibration(DEFINE.OBJ_L, DEFINE.CAL_XY)) cmd = Math.Abs(cmd) * -1;
                            if (Common.PBD_TYPE != DEFINE.REAR_STAGE)
                                if (cmd > 0) if (!Calibration(DEFINE.OBJ_L, DEFINE.CAL_T)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.OBJ_CALRIBRATION_RIGHT:
                            if (cmd > 0) if (!Calibration(DEFINE.OBJ_R, DEFINE.CAL_XY)) cmd = Math.Abs(cmd) * -1;
                            if (Main.DEFINE.NON_STANDARD)
                            {
                                if (m_AlignType[m_PatTagNo] == DEFINE.M_1CAM2SHOT)
                                {
                                    if (cmd > 0) if (!Calibration(DEFINE.OBJ_R, DEFINE.CAL_T)) cmd = Math.Abs(cmd) * -1;
                                }
                            }
                            break;

                        case CMD.TAR_CALRIBRATION_LEFT:
                            if (cmd > 0) if (!Calibration(DEFINE.TAR_L, DEFINE.CAL_XY)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.TAR_CALRIBRATION_RIGHT:
                            if (cmd > 0) if (!Calibration(DEFINE.TAR_R, DEFINE.CAL_XY)) cmd = Math.Abs(cmd) * -1;
                            break;

                        case CMD.HEAD_CALRIBRATION:
                            if (cmd > 0) if (!Calibration(DEFINE.OBJ_L, DEFINE.CAL_T)) cmd = Math.Abs(cmd) * -1;
                            break;
                        #endregion

                        default:
                            break;
                    }

                    if (cmd != 0)
                    {
                        if (cmd < 0 && m_errSts > -1)
                        {
                            Log_Error();
                            //    cmd_Error(ref cmd);     //Limit Error 리턴값 변경.
                        }
                        if ((cmd > 1020 && cmd < 1090) || cmd == CMD.ALIGN_TRAY_SEARCH)
                        {
                            SetAlignPosEnd(cmd);
                        }
                        else if (cmd != 9000)
                        {
                            //Console.WriteLine("Write Result");
                            WriteStsEnd(cmd);
                            //Console.WriteLine("Write Done");
                        }
                        if (cmd < 0 && m_errSts > -1)
                        {
                            m_errSts = -1;
                        }

                    }
                    if (cmd != 0)
                        CmdCheck();

                    m_Cmd = cmd = 0;
                }
                InspectionDoneEvent.Set();
            } //ExecuteCMD 

            private ushort GetGrabCommand()
            {
                string strLog = "";
                ushort bRet = 0;

                if (m_AlignNo % 2 == 0 && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_X)) // m_AlignNo % 2 == 0 -> CAM 0 2 4 6 => CAM1 Command
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_X;    // 0b00000001
                    strLog += "X_";
                }

                if (m_AlignNo % 2 == 0 && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_Y))
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_Y;    // 0b00000010
                    strLog += "Y_";
                }

                if (m_AlignNo % 2 == 1 && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_X)) // m_AlignNo % 2 == 1 -> CAM 1 3 5 7 => CAM2 Command
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_X;    // 0b00000001
                    strLog += "X_";
                }

                if (m_AlignNo % 2 == 1 && Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_Y))
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_Y;    // 0b00000010
                    strLog += "Y_";
                }

                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_C_CUT_MODE))
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_C;    // 0b00000100
                    strLog += "C_";

                    if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_SHAPE_CUT_MODE))
                    {
                        bRet |= (ushort)GRAB_CMD.COMMAND5;    // 0b00010000
                        bRet |= (ushort)GRAB_CMD.COMMAND6;    // 0b00100000
                    }
                }

                if (((bRet & (ushort)GRAB_CMD.SEARCH_X) == (ushort)GRAB_CMD.SEARCH_X) &&
                    ((bRet & (ushort)GRAB_CMD.SEARCH_Y) == (ushort)GRAB_CMD.SEARCH_Y))
                    bRet |= (ushort)GRAB_CMD.COMMAND4;    // 0b00001000

                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_R_CUT_MODE))
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_R;    // 0b01000000
                    strLog += "R_";
                }

                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_SHAPE_CUT_MODE))
                {
                    bRet |= (ushort)GRAB_CMD.SEARCH_S;    // 0b10000000
                    strLog += "S_";
                }

                LogdataDisplay(strLog + "COMMAND", true);

                strLog = Convert.ToString(bRet, 2);

                if (strLog.Length < 8)
                    strLog = strLog.PadLeft(8 - strLog.Length, '0');

                LogdataDisplay("***** Grab Command : 0b" + strLog, true);

                return bRet;
            }

            public bool ExecuteGrab(int nCamType, int nSelPad)
            {
                string strLog = "";
                if (nCamType != DEFINE.CAM_SELECT_ALIGN && nCamType != DEFINE.CAM_SELECT_INSPECT)
                    return false;

                bool bRet = false;
                ushort nRet = 0;
                ushort nInspRet = 0;
                ushort cmd = GetGrabCommand();

                if (nCamType == DEFINE.CAM_SELECT_ALIGN && m_AlignNo == 0)
                {
                    Main.INSP_RESULT.Clear();
                    LogdataDisplay("INSP_RESULT_CLEAR", false);
                }

                m_lProcTime = m_TimerInOut.GetElapsedTime();

                // For Calibration
                if (m_bCalibMode == true)
                    bRet = PATSearch(nCamType);
                else
                    nRet = ExecuteSearch(nCamType, nSelPad, cmd);

                if (nCamType == DEFINE.CAM_SELECT_ALIGN)    // ALIGN 모드일 때 INSPECTION도 시행(Glass Edge 추출)
                {
                    nInspRet = ExecuteSearch(DEFINE.CAM_SELECT_INSPECT, nSelPad, cmd);
                    nRet &= nInspRet;   // Inspection NG도 공통 NG로 취하기 위해
                }

                m_lProcTime = m_TimerInOut.GetElapsedTime() - m_lProcTime;

                if (nCamType == DEFINE.CAM_SELECT_INSPECT)
                {
                    if (m_AlignNo == 0)
                    {
                        Main.INSP_RESULT.m_dPoint[0] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[0];
                        Main.INSP_RESULT.m_bUnitComp[m_AlignNo] = true;

                        //WaitInspectionComplete(nSelPad);
                    }
                    else if (m_AlignNo == 1)
                    {
                        Main.INSP_RESULT.m_dPoint[5] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[0];
                        Main.INSP_RESULT.m_bUnitComp[m_AlignNo] = true;

                        //WaitInspectionComplete(nSelPad);
                    }
                    else if (m_AlignNo == 2)
                    {
                        Main.INSP_RESULT.m_dPoint[1] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[0];
                        Main.INSP_RESULT.m_dPoint[2] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[1];
                        Main.INSP_RESULT.m_dPoint[6] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[2];
                        Main.INSP_RESULT.m_bUnitComp[m_AlignNo] = true;

                        //WaitInspectionComplete(nSelPad);
                    }
                    else if (m_AlignNo == 3)
                    {
                        Main.INSP_RESULT.m_dPoint[4] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[0];
                        Main.INSP_RESULT.m_dPoint[3] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[1];
                        Main.INSP_RESULT.m_dPoint[7] = PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[2];
                        Main.INSP_RESULT.m_bUnitComp[m_AlignNo] = true;

                        //WaitInspectionComplete(nSelPad);
                    }
                }

                SetResultData(nCamType, cmd, nRet);

                //Thread.Sleep(100);

                if (nCamType == DEFINE.CAM_SELECT_INSPECT)
                    WaitInspectionComplete(nSelPad);

                if (nCamType == DEFINE.CAM_SELECT_ALIGN)
                    strLog = "ALIGN_";
                else if (nCamType == DEFINE.CAM_SELECT_INSPECT)
                    strLog = "INSPECTION_";

                if (m_AlignNo == 0)
                {
                    m_Timer.StartTimer();
                    while (m_Timer.GetElapsedTime() <= DEFINE.COMP_WAIT_TIMEOUT)
                    {
                        if (Main.CCLink_IsBit((ushort)(AlignUnit[1].UNIT_RES_SIGNAL_ADDR + DEFINE.CL_X))
                            && Main.CCLink_IsBit((ushort)(AlignUnit[1].UNIT_RES_SIGNAL_ADDR + DEFINE.CL_Y)))
                        {
                            strLog += "CAM1_2_GRAB_COMPLETE";
                            LogdataDisplay(strLog, true);

                            Main.CCLink_CommandHandshake(DEFINE.CCLINK_OUT_STAGE1_CAM_GRAB_COMP, DEFINE.CCLINK_IN_STAGE1_CAM_GRAB_REQ, DEFINE.CMD_HANDSHAKE_TIMEOUT, m_AlignNo);

                            break;
                        }
                        else
                            Thread.Sleep(10);
                    }

                    if (m_Timer.GetElapsedTime() > DEFINE.COMP_WAIT_TIMEOUT)
                    {
                        strLog += "CAM1_2_GRAB_COMP_TIMEOUT";
                        LogdataDisplay(strLog, true);
                    }
                }
                else if (m_AlignNo == 2)
                {
                    m_Timer.StartTimer();
                    while (m_Timer.GetElapsedTime() <= DEFINE.COMP_WAIT_TIMEOUT)
                    {
                        if (Main.CCLink_IsBit((ushort)(AlignUnit[3].UNIT_RES_SIGNAL_ADDR + DEFINE.CL_X))
                            && Main.CCLink_IsBit((ushort)(AlignUnit[3].UNIT_RES_SIGNAL_ADDR + DEFINE.CL_Y)))
                        {
                            strLog += "CAM3_4_GRAB_COMPLETE";
                            LogdataDisplay(strLog, true);

                            Main.CCLink_CommandHandshake(DEFINE.CCLINK_OUT_STAGE1_CAM_GRAB_COMP, DEFINE.CCLINK_IN_STAGE1_CAM_GRAB_REQ, DEFINE.CMD_HANDSHAKE_TIMEOUT, m_AlignNo);

                            if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && nCamType == Main.DEFINE.CAM_SELECT_ALIGN)
                            {
                                long panelSizeX = (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + DEFINE.PANEL_SIZE_X + 1] << 16) | (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + DEFINE.PANEL_SIZE_X] & 0x0000ffff);
                                m_StageT = Math.Atan2(m_StageX - AlignUnit[3].m_StageX, /*m_StageY - AlignUnit[3].m_StageY + */(panelSizeX / 1000)) * DEFINE.degree * (-1);
                                InputAligndata(m_StageX, m_StageY, m_StageT);
                                AlignUnit[3].InputAligndata(AlignUnit[3].m_StageX, AlignUnit[3].m_StageY, m_StageT);
                            }

                            break;
                        }
                        else
                            Thread.Sleep(10);
                    }

                    if (m_Timer.GetElapsedTime() > DEFINE.COMP_WAIT_TIMEOUT)
                    {
                        strLog += "CAM3_4_GRAB_COMP_TIMEOUT";
                        LogdataDisplay(strLog, true);
                    }
                }

                m_lInOutTime = m_TimerInOut.GetElapsedTime();
                m_OutTime = m_TimerInOut.timeEnd;

                ResetResultData();

                m_bDisplayStatus = true;

                string temps = "IN - OUT : " + m_lInOutTime.ToString() + " ms, Processing : " + m_lProcTime.ToString() + " ms";
                LogdataDisplay(temps, true);

                if (nCamType == DEFINE.CAM_SELECT_INSPECT)
                {
                    if (m_AlignNo == 1)
                    {
                        temps = "";
                        for (int i = 0; i < DEFINE.MeasurePoint_Max; i++)
                        {
                            temps += Main.INSP_RESULT.m_dPoint[i].ToString("0.000");
                            if (i < DEFINE.MeasurePoint_Max - 1)
                                temps += ",";
                        }

                        WriteCSVLogFile(temps, nCamType);
                        WriteRCSLogFile();
                        WriteTrackOutFile();
                        LogdataDisplayData(temps);
                    }
                }
                else
                {
                    temps = PAT[m_PatTagNo, nCamType].m_RobotPosX.ToString("0.000") + "," + PAT[m_PatTagNo, nCamType].m_RobotPosY.ToString("0.000");
                    if (nCamType == DEFINE.CAM_SELECT_ALIGN && nSelPad == DEFINE.JOB_PANEL_PAD && PAT[m_PatTagNo, nCamType].FINDLineResults.Length > 3 && PAT[m_PatTagNo, nCamType].FINDLineResults[3].CrossPointList.Count > 0)
                        temps += "," + PAT[m_PatTagNo, nCamType].FINDLineResults[3].CrossPointList[0].X2.ToString("0.000");

                    WriteCSVLogFile(temps, nCamType);
                    LogdataDisplayData(temps);
                }

                MainGriddataDisplay();
                PatternPixelRobotSave();

                return bRet;
            }

            private void SetResultData(int nCamType, ushort cmd, ushort ret, bool bForceGrab = false)
            {
                ushort nAddr;

                if (m_bCalibMode == true)
                {
                    //if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                    {
                        if (nCamType == DEFINE.CAM_SELECT_ALIGN)
                        {
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.m_RobotPosX * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.m_RobotPosY * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), 0);
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.Score * 1000));
                        }
                        else
                        {
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.m_RobotPosX * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.m_RobotPosY * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), 0);
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.Score * 1000));
                        }
                    }
                }
                else
                {
                    if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && nCamType == DEFINE.CAM_SELECT_ALIGN)
                    {
                        if ((cmd & (ushort)GRAB_CMD.SEARCH_C) == (ushort)GRAB_CMD.SEARCH_C)     // C cut
                        {
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].Y2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[3].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);

                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_X), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[1].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_Y), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[1].CrossPointList[0].Y2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_R_T), 0);
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_SCORE), 0);

                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_X), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[2].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_Y), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[2].CrossPointList[0].Y2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_R_T), 0);
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_SCORE), 0);
                        }
                        else if ((cmd & (ushort)GRAB_CMD.SEARCH_R) == (ushort)GRAB_CMD.SEARCH_R)    // R cut
                        {
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].Y2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[3].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);

                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_X), (int)(PAT[m_PatTagNo, nCamType].CircleResults[0].m_RobotPosX * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_Y), (int)(PAT[m_PatTagNo, nCamType].CircleResults[0].m_RobotPosY * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_R_T), (int)(PAT[m_PatTagNo, nCamType].CircleResults[0].m_RobotRadius * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_SCORE), 0);
                        }
                        else    // Normal cut
                        {
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].X2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].Y2 * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), 0);
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);
                        }
                    }
                    else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && nCamType == DEFINE.CAM_SELECT_INSPECT)   // Inspection Size
                    {
                        {
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[1] * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[0] * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), (int)(PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[2] * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);

                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_X), (int)(PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[1] * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_Y), (int)(PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[0] * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_R_T), (int)(PAT[m_PatTagNo, DEFINE.CAM_SELECT_INSPECT].InspectionSizeRobot_X[2] * 1000));
                            Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_SCORE), 0);
                        }
                    }
                    else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && m_AlignNo == 0 && nCamType == DEFINE.CAM_SELECT_1ST_ALIGN)
                    {
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].X2 * 1000));   // X-Y 교점 X
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[0].CrossPointList[0].Y2 * 1000));   // X-Y 교점 Y
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), (int)(PAT[m_PatTagNo, nCamType].FINDLineResults[2].CrossPointList[0].X2 * 1000)); // 기울기
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);
                    }
                    else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && m_AlignNo == 1 && nCamType == DEFINE.CAM_SELECT_PRE_ALIGN_RIGHT)
                    {
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.m_RobotPosX * 1000));    // Left Mark X
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.m_RobotPosY * 1000));    // Left Mark Y
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), 0);
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.Score * 1000));

                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_X), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.m_RobotPosX * 1000));    // Right Mark X
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_Y), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.m_RobotPosY * 1000));    // Right Mark Y
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_R_T), 0);
                        Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_SCORE), (int)(PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.Score * 1000));
                    }
                }

                Thread.Sleep(Main.machine.m_nCCLinkCommDelay_ms);

                SetResultSignal(nCamType, cmd, ret, bForceGrab);
            }

            private void SetResultSignal(int nCamType, ushort cmd, ushort ret, bool bForceGrab = false)
            {
                string strLog = "***** Set Result (0b" + Convert.ToString(ret, 2) + ") ";

                #region PC1
                if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && !bForceGrab)
                {
                    if (((cmd & (ushort)GRAB_CMD.SEARCH_S) == (ushort)GRAB_CMD.SEARCH_S) &&
                        ((cmd & (ushort)GRAB_CMD.SEARCH_C) == (ushort)GRAB_CMD.SEARCH_C
                        || (cmd & (ushort)GRAB_CMD.SEARCH_R) == (ushort)GRAB_CMD.SEARCH_R))
                    {
                        Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.OK_C_R), true);
                    }

                    if ((cmd & (ushort)GRAB_CMD.SEARCH_X) == (ushort)GRAB_CMD.SEARCH_X
                     || ((cmd & (ushort)AlignUnitTag.GRAB_CMD.PATMATCH) == (ushort)AlignUnitTag.GRAB_CMD.PATMATCH))
                    {
                        if (nCamType == DEFINE.CAM_SELECT_ALIGN && Main.machine.m_bUseLoadingLimit && PAT[m_PatTagNo, nCamType].FINDLineResults[0].m_bLoadingLimitOver_X == true)
                        {
                            //Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.NG_X), true);
                            Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.LOAD_NG), true);
                            strLog += "X_LOAD_NG ";
                        }
                        else
                        {
                            if ((ret & (ushort)GRAB_CMD.SEARCH_X) == (ushort)GRAB_CMD.SEARCH_X
                                || PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.SearchResult == true)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.OK_X), true);
                                strLog += "X_OK ";
                            }
                            else
                            {
                                //Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.NG_X), true);
                                strLog += "X_NG ";
                            }
                        }
                        Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_X), true);
                        strLog += "COMPLETE ";
                    }

                    if ((cmd & (ushort)GRAB_CMD.SEARCH_Y) == (ushort)GRAB_CMD.SEARCH_Y
                        || ((cmd & (ushort)AlignUnitTag.GRAB_CMD.PATMATCH) == (ushort)AlignUnitTag.GRAB_CMD.PATMATCH))
                    {
                        if (nCamType == DEFINE.CAM_SELECT_ALIGN && Main.machine.m_bUseLoadingLimit && PAT[m_PatTagNo, nCamType].FINDLineResults[0].m_bLoadingLimitOver_Y == true)
                        {
                            //Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.NG_Y), true);
                            Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.LOAD_NG), true);
                            strLog += "Y_LOAD_NG ";
                        }
                        else
                        {
                            if ((ret & (ushort)GRAB_CMD.SEARCH_Y) == (ushort)GRAB_CMD.SEARCH_Y
                                || PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.SearchResult == true)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.OK_Y), true);
                                strLog += "Y_OK ";
                            }
                            else
                            {
                                //Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.NG_Y), true);
                                strLog += "Y_NG ";
                            }
                        }
                        Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_Y), true);
                        strLog += "COMPLETE";
                    }

                    string strRetryLog = "";

                    if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_X)) == false)
                    {
                        strRetryLog = "CAM GRAB X RETRY ";
                        m_TimerCommCheck.StartTimer();
                        while (m_TimerCommCheck.GetElapsedTime() <= 500)
                        {
                            Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_X), true);
                            if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_X)) == true)
                                break;
                            else
                                Thread.Sleep(10);

                            strRetryLog += ".";
                        }

                        LogdataDisplay(strRetryLog, true);

                        if (m_TimerCommCheck.GetElapsedTime() > 500)
                            LogdataDisplay("CAM GRAB X COMPLETE TIMEOUT", true);
                    }

                    if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_Y)) == false)
                    {
                        strRetryLog = "CAM GRAB Y RETRY ";
                        m_TimerCommCheck.StartTimer();
                        while (m_TimerCommCheck.GetElapsedTime() <= 500)
                        {
                            Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_Y), true);
                            if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CL_Y)) == true)
                                break;
                            else
                                Thread.Sleep(10);

                            strRetryLog += ".";
                        }

                        LogdataDisplay(strRetryLog, true);

                        if (m_TimerCommCheck.GetElapsedTime() > 500)
                            LogdataDisplay("CAM GRAB Y COMPLETE TIMEOUT", true);
                    }
                }
                #endregion
                #region PC2
                if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && !bForceGrab)
                {
                    if ((cmd & (ushort)GRAB_CMD.SEARCH_X) == (ushort)GRAB_CMD.SEARCH_X
                     || ((cmd & (ushort)AlignUnitTag.GRAB_CMD.PATMATCH) == (ushort)AlignUnitTag.GRAB_CMD.PATMATCH))
                    {
                        if (nCamType == DEFINE.CAM_SELECT_1ST_ALIGN && Main.machine.m_bUseAlign1AngleLimit)
                        {
                            if (PAT[m_PatTagNo, nCamType].FINDLineResults[0].m_bAngleLimit == true)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.CROSS_NG), true);
                                strLog += "CROSS_NG ";
                            }
                            if (PAT[m_PatTagNo, nCamType].FINDLineResults[2].m_bAngleLimit == true)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.VERTI_NG), true);
                                strLog += "VERTICAL_NG ";
                            }
                        }

                        {
                            if ((ret & (ushort)GRAB_CMD.SEARCH_X) == (ushort)GRAB_CMD.SEARCH_X
                                || PAT[m_PatTagNo, Main.DEFINE.OBJ_L].PMAlignResult.SearchResult == true)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.OK_1ST), true);
                                strLog += "1_OK ";
                            }
                            else
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.NG_1ST), true);
                                strLog += "1_NG ";
                            }
                        }
                        Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_1ST), true);
                        strLog += "COMPLETE ";
                    }

                    if (m_AlignNo == 1 &&
                        ((cmd & (ushort)GRAB_CMD.SEARCH_Y) == (ushort)GRAB_CMD.SEARCH_Y
                        || ((cmd & (ushort)AlignUnitTag.GRAB_CMD.PATMATCH) == (ushort)AlignUnitTag.GRAB_CMD.PATMATCH)))
                    {
                        if ((ret & (ushort)GRAB_CMD.SEARCH_Y) == (ushort)GRAB_CMD.SEARCH_Y
                            || PAT[m_PatTagNo, Main.DEFINE.OBJ_R].PMAlignResult.SearchResult == true)
                        {
                            Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.OK_2ND), true);
                            strLog += "2_OK ";
                        }
                        else
                        {
                            Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.NG_2ND), true);
                            strLog += "2_NG ";
                        }

                        Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_2ND), true);
                        strLog += "COMPLETE";
                    }

                    string strRetryLog = "";

                    if (m_AlignNo == 1)
                    {
                        if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_1ST)) == false)
                        {
                            strRetryLog = "CAM GRAB1 RETRY ";
                            m_TimerCommCheck.StartTimer();
                            while (m_TimerCommCheck.GetElapsedTime() <= 500)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_1ST), true);
                                if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_1ST)) == true)
                                    break;
                                else
                                    Thread.Sleep(10);

                                strRetryLog += ".";
                            }

                            LogdataDisplay(strRetryLog, true);

                            if (m_TimerCommCheck.GetElapsedTime() > 500)
                                LogdataDisplay("CAM GRAB1 COMPLETE TIMEOUT", true);
                        }

                        if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_2ND)) == false)
                        {
                            strRetryLog = "CAM GRAB2 RETRY ";
                            m_TimerCommCheck.StartTimer();
                            while (m_TimerCommCheck.GetElapsedTime() <= 500)
                            {
                                Main.CCLink_PutBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_2ND), true);
                                if (Main.CCLink_IsBit((ushort)(UNIT_RES_SIGNAL_ADDR + DEFINE.COMP_2ND)) == true)
                                    break;
                                else
                                    Thread.Sleep(10);

                                strRetryLog += ".";
                            }

                            LogdataDisplay(strRetryLog, true);

                            if (m_TimerCommCheck.GetElapsedTime() > 500)
                                LogdataDisplay("CAM GRAB2 COMPLETE TIMEOUT", true);
                        }
                    }
                }
                #endregion

                LogdataDisplay(strLog, true);
            }

            public void ResetResultData()
            {
                if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                {
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_X, DEFINE.CCLINK_OUT_STAGE1_CAM1_SEARCH_COMP_X, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_Y, DEFINE.CCLINK_OUT_STAGE1_CAM1_SEARCH_COMP_Y, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_X, DEFINE.CCLINK_OUT_STAGE1_CAM2_SEARCH_COMP_X, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_Y, DEFINE.CCLINK_OUT_STAGE1_CAM2_SEARCH_COMP_Y, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);

                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM1_RESULT_OK_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM1_RESULT_OK_Y, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM1_RESULT_OK_C_R, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM1_PANEL_LIMIT, false);

                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM2_RESULT_OK_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM2_RESULT_OK_Y, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM2_RESULT_OK_C_R, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK_OUT_STAGE1_CAM2_PANEL_LIMIT, false);

                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);

                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_X), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_Y), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_R_T), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_SCORE), 0);

                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_X), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_Y), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_R_T), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH3_SCORE), 0);
                }
                else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                {
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK2_IN_FIRST_ALIGN_SEARCH_REQ_X, DEFINE.CCLINK2_OUT_FIRST_ALIGN_SEARCH_COMP_X, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK2_IN_PRE_ALIGN1_SEARCH_REQ_X, DEFINE.CCLINK2_OUT_PRE_ALIGN1_SEARCH_COMP_X, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);
                    Main.CCLink_OffCheckOffHandshake(DEFINE.CCLINK2_IN_PRE_ALIGN2_SEARCH_REQ_X, DEFINE.CCLINK2_OUT_PRE_ALIGN2_SEARCH_COMP_X, DEFINE.NORMAL_HANDSHAKE_TIMEOUT, m_AlignNo);

                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_FIRST_ALIGN_RESULT_OK_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_FIRST_ALIGN_RESULT_NG_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_FIRST_ALIGN_CROSS_ANGLE_NG, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_FIRST_ALIGN_VERTICAL_ANGLE_NG, false);

                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_PRE_ALIGN1_RESULT_OK_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_PRE_ALIGN1_RESULT_NG_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_PRE_ALIGN2_RESULT_OK_X, false);
                    Main.CCLink_PutBit(DEFINE.CCLINK2_OUT_PRE_ALIGN2_RESULT_NG_X, false);

                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_X), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_Y), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_R_T), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH1_SCORE), 0);

                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_X), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_Y), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_R_T), 0);
                    Main.CCLink_WriteDWord((ushort)(UNIT_RES_DATA_ADDR + DEFINE.SEARCH2_SCORE), 0);
                }
            }

            private bool WaitInspectionComplete(int nSelPad)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool bAllComp = true;
                bool bRet = false;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case 1:
                            if (m_Timer.GetElapsedTime() > DEFINE.INSECTION_WAIT_TIMEOUT)
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }

                            if (nSelPad == DEFINE.JOB_PANEL_PAD)
                            {
                                bAllComp &= Main.INSP_RESULT.m_bUnitComp[0];
                                bAllComp &= Main.INSP_RESULT.m_bUnitComp[1];
                            }
                            else
                            {
                                bAllComp &= Main.INSP_RESULT.m_bUnitComp[2];
                                bAllComp &= Main.INSP_RESULT.m_bUnitComp[3];
                            }

                            if (bAllComp == false)
                                break;
                            else
                                seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.ERROR_SEQ:
                            bRet = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            bRet = true;
                            LoopFlag = false;
                            break;
                    }
                }

                return bRet;
            }

            public bool LightUseCheck(int nType)
            {
                bool bRet = false;
                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    if (PAT[m_PatTagNo, nType].m_LightCtrl[i] >= 0)
                        bRet = true;
                }
                return bRet;
            }

            private bool Align1Cam2Shot(int search, bool TargetUse)
            {
                bool bRet = false;
                int seq;
                bool LoopFlag;
                seq = 0;
                LoopFlag = true;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            if (!PATSearch(search))
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq++;
                            break;

                        case 1:
                            seq++;
                            break;

                        case 2:
                            if (TargetUse == true || m_AlignName == "COF_CUTTING_ALIGN1" || m_AlignName == "COF_CUTTING_ALIGN2")
                                Processing(0, TargetUse, true, false);
                            else
                                Processing(search + 1, TargetUse, true, false);
                            seq++;
                            break;

                        case 3:
                            seq++;
                            break;

                        case 4:
                            if (!LengthCheck(DEFINE.OBJ_ALL))
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq = SEQ.COMPLET_SEQ;
                            break;


                        case SEQ.ERROR_SEQ:
                            bRet = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            InputAligndata(m_StageX, m_StageY, m_StageT);
                            bRet = true;
                            LoopFlag = false;
                            break;
                    }
                }

                return bRet;
            }
            private void LengthCalculation(int nType)
            {
                double dx = 0, dy = 0, dt = 0, t1 = 0, t2 = 0;

                double Target_Mark_Dist = 0, Object_Mark_Dist = 0;
                int CalStageNo = m_PatTagNo;

                if (m_AlignType[m_PatTagNo] == DEFINE.M_1CAM1SHOT || m_AlignType[m_PatTagNo] == DEFINE.M_1CAM2SHOT)
                {
                    m_CamDistX1 = m_CamDistX2 = 0;
                }
                else
                {
                    m_CamDistX2 = m_TAR_Distance;
                    m_CamDistX1 = m_OBJ_Distance;
                }
                //---------------------- Target ----------------------
                dx = m_MotStagePosX[DEFINE.TAR_L] - m_MotStagePosX[DEFINE.TAR_R] + PAT[m_PatTagNo, DEFINE.TAR_R].m_RobotPosX - PAT[m_PatTagNo, DEFINE.TAR_L].m_RobotPosX + m_CamDistX2;
                dy = m_MotStagePosY[DEFINE.TAR_L] - m_MotStagePosY[DEFINE.TAR_R] + PAT[m_PatTagNo, DEFINE.TAR_R].m_RobotPosY - PAT[m_PatTagNo, DEFINE.TAR_L].m_RobotPosY;
                if (dx != 0)
                    t1 = Math.Atan(dy / dx);
                else
                    t1 = 0;

                if (nType == DEFINE.TAR_ALL || nType == DEFINE.OBJTAR_ALL)
                    Target_Mark_Dist = m_TAR_Mea_Dis = Math.Round(Math.Sqrt(dy * dy + dx * dx));

                //---------------------- Object ----------------------
                dx = m_MotStagePosX[DEFINE.OBJ_L] - m_MotStagePosX[DEFINE.OBJ_R] + PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosX - PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosX + m_CamDistX1;
                dy = m_MotStagePosY[DEFINE.OBJ_L] - m_MotStagePosY[DEFINE.OBJ_R] + PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosY - PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosY;

                if (dx != 0)
                    t2 = Math.Atan(dy / dx);
                else t2 = 0;

                if (nType == DEFINE.OBJ_ALL || nType == DEFINE.OBJTAR_ALL)
                    Object_Mark_Dist = m_OBJ_Mea_Dis = Math.Round(Math.Sqrt(dy * dy + dx * dx));

                long Degree;
                dt = (t1 - t2);
                Degree = (long)((dt * 180.0 / DEFINE.PI) * 1000.0);
            }

            private bool LengthCheck(int nType)
            {
                bool[] nReturn = new bool[2];
                nReturn[0] = true;
                nReturn[1] = true;

                bool nRet = new bool();
                double nDistanceRet = new double();
                nRet = true;
                if (m_LengthCheck_Use)
                {
                    LengthCalculation(nType);
                    if (nType == DEFINE.OBJ_ALL || nType == DEFINE.OBJTAR_ALL)
                    {
                        nReturn[0] = LengthCheck(m_OBJ_Mea_Dis, m_OBJ_Standard_Length, m_Length_Tolerance, ref nDistanceRet);
                        m_OBJ_Mea_Dis = nDistanceRet;
                    }
                    if (nType == DEFINE.TAR_ALL || nType == DEFINE.OBJTAR_ALL)
                    {
                        nReturn[1] = LengthCheck(m_TAR_Mea_Dis, m_TAR_Standard_Length, m_Length_Tolerance, ref nDistanceRet);
                        m_TAR_Mea_Dis = nDistanceRet;
                    }
                    if (!nReturn[0] && !nReturn[1])
                    {
                        m_errSts = ERR.LENTH + DEFINE.OBJTAR_ALL;
                    }
                    else if (!nReturn[0])
                        m_errSts = ERR.LENTH + DEFINE.OBJ_ALL;
                    else if (!nReturn[1])
                        m_errSts = ERR.LENTH + DEFINE.TAR_ALL;

                    if (!nReturn[0] || !nReturn[1])
                    {
                        nRet = false;
                    }
                }
                return nRet;
            }
            private bool Align1Cam2Shot_Caliper(int search, bool TargetUse)
            {
                bool bRet = false;
                int seq;
                bool LoopFlag;
                seq = 0;
                LoopFlag = true;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            if (!CALISearch(search))
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq++;
                            break;

                        case 1:
                            CalierToRobot();
                            seq++;
                            break;

                        case 2:
                            if (TargetUse || m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2")
                                Processing(0, TargetUse, true, false);
                            else
                                Processing(search + 1, TargetUse, true, false);
                            seq++;
                            break;

                        case 3:
                            seq++;
                            break;

                        case 4:
                            if (!LengthCheck(DEFINE.OBJ_ALL))
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq = SEQ.COMPLET_SEQ;
                            break;


                        case SEQ.ERROR_SEQ:
                            bRet = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            if (m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2")
                            {
                                long[] nACFLength = new long[1];
                                nACFLength[0] = (long)m_OBJ_Mea_Dis;
                                InputTABdata(DEFINE.OBJ_L, m_OBJ_Mea_Dis);
                                WriteTABLength(nACFLength);
                            }
                            InputAligndata(m_StageX, m_StageY, m_StageT);
                            bRet = true;
                            LoopFlag = false;
                            break;
                    }
                }

                return bRet;
            }

            private void CalierToRobot()
            {
                double nTempV2R_X = 0;
                double nTempV2R_Y = 0;

                for (int i = 0; i < 2; i++)
                {
                    nTempV2R_X = 0;
                    nTempV2R_Y = 0;

                    for (int j = 0; j < Main.DEFINE.CALIPER_MAX; j++)
                    {
                        if (PAT[m_PatTagNo, i].CaliperPara[j].m_UseCheck)
                        {
                            switch (Main.GetCaliperDirection(Main.GetCaliperDirection(PAT[m_PatTagNo, i].CaliperTools[j].Region.Rotation)))
                            {
                                case Main.DEFINE.X:
                                    PAT[m_PatTagNo, i].Pixel[DEFINE.X] = PAT[m_PatTagNo, i].CaliResults[j].Pixel[DEFINE.X];
                                    break;

                                case Main.DEFINE.Y:
                                    PAT[m_PatTagNo, i].Pixel[DEFINE.Y] = PAT[m_PatTagNo, i].CaliResults[j].Pixel[DEFINE.Y];
                                    break;
                            }
                        }
                    }
                    PAT[m_PatTagNo, i].V2R(ref nTempV2R_X, ref nTempV2R_Y);
                    PAT[m_PatTagNo, i].PMAlignResult.m_RobotPosX = PAT[m_PatTagNo, i].m_RobotPosX = nTempV2R_X;
                    PAT[m_PatTagNo, i].PMAlignResult.m_RobotPosY = PAT[m_PatTagNo, i].m_RobotPosY = nTempV2R_Y;
                }
            }
            private bool AlignCenter(int SearchType, bool TargetUse, int ReAlign)  // 0: 1 count,   1: retry			
            {
                bool bRet = true;

                int seq;
                bool LoopFlag;
                seq = 0;
                int m_RepeatCount = 0;
                bool Research = false;
                LoopFlag = true;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0: // OBJECT
                            if (!Research)
                            {
                                if (!PATSearch(SearchType))
                                {
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                                if (m_AlignName == "PBD1" || m_AlignName == "PBD2")
                                {
                                    Processing(0, false, false, false);
                                    InputAligndata(m_StageX, m_StageY, m_StageT, true);
                                }
                            }
                            if (ReAlign == 0)
                            {
                                Processing(0, TargetUse, true, false); //Offset		
                            }
                            else
                            {
                                Processing(0, TargetUse, false, false);
                            }
                            seq++;
                            break;

                        case 1:
                            if (ReAlign == 1)
                                seq++;
                            else
                                seq = SEQ.COMPLET_SEQ;
                            break;

                        case 2:
                            RetryMove();
                            CmdCheck_(ALIGN_UNIT_ADDR + Main.DEFINE.PLC_MOVE_END);
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case 3:
                            if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT)
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            if (RetryDone())
                            {
                                seq++;
                            }
                            break;

                        case 4:
                            if (SearchType == Main.DEFINE.TAR_ALL)
                            {
                                SearchType = Main.DEFINE.OBJ_ALL; //Object 먼저 촬상후 Target 찾으면서 Align 하고, ReAlign시에는 Object가 움직이기 때문에 OBJECT 촬상 해야됨. 
                            }

                            if (!PATSearch(SearchType))
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq++;
                            break;

                        case 5:
                            //----------------------------------------------------------------------------------------------  
                            if (Processing(0, TargetUse, false, true))
                            {
                                Processing(0, TargetUse, true, false);    // mode 1, target=false, offset=true
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }

                            seq++;
                            break;

                        case 6:
                            if (m_RepeatCount >= m_RepeatLimit)
                            {
                                m_errSts = ERR.ALIGN_REPEAT_LIMIT;  //Retry Limit
                                seq = SEQ.ERROR_SEQ;
                                break; ;
                            }
                            Research = true;
                            m_RepeatCount++;
                            seq = 0;
                            break;

                        case SEQ.ERROR_SEQ:
                            bRet = false;
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ:
                            if ((m_AlignName == "PBD1" || m_AlignName == "PBD2" || m_AlignName == "PBD_FOF1" || m_AlignName == "PBD_FOF2") && bRet == true)
                            {
                                WriteObjTarLength();
                                long[] nPBD_DIS = new long[2];
                                nPBD_DIS[0] = (long)m_OBJ_Mea_Dis;
                                nPBD_DIS[1] = (long)m_TAR_Mea_Dis;
                                InputTABdata(DEFINE.OBJ_L, nPBD_DIS[0]);
                                InputTABdata(DEFINE.OBJ_R, nPBD_DIS[1]);

                            }
                            if (bRet) InputAligndata(m_StageX, m_StageY, m_StageT);
                            LoopFlag = false;
                            break;
                    }
                }

                return bRet;
            }
            private bool Processing(int AlignMode, bool TargetFlag, bool OffsetFlag, bool VerifyFlag) // 0 :양측 1:왼쪽 2: 오른쪽 
            {
                double dx = 0, dy = 0, dt = 0, t1 = 0, t2 = 0;
                double Cx = 0, Cy = 0, Px = 0, Py = 0;
                double ImagePosX = 0, ImagePosY = 0;

                double alignX = 0, alignY = 0;

                double Target_Mark_Dist = 0, Object_Mark_Dist = 0;
                int CalStageNo = m_PatTagNo;

                double OffsetT;
                double CenterOffset;


                if (m_AlignType[m_PatTagNo] == DEFINE.M_1CAM1SHOT || m_AlignType[m_PatTagNo] == DEFINE.M_1CAM2SHOT)
                {
                    m_CamDistX1 = m_CamDistX2 = 0;
                }
                else
                {
                    m_CamDistX2 = m_TAR_Distance;
                    m_CamDistX1 = m_OBJ_Distance;
                }
                //---------------------- Target ----------------------
                dx = m_MotStagePosX[DEFINE.TAR_L] - m_MotStagePosX[DEFINE.TAR_R] + PAT[m_PatTagNo, DEFINE.TAR_R].m_RobotPosX - PAT[m_PatTagNo, DEFINE.TAR_L].m_RobotPosX + m_CamDistX2;
                dy = m_MotStagePosY[DEFINE.TAR_L] - m_MotStagePosY[DEFINE.TAR_R] + PAT[m_PatTagNo, DEFINE.TAR_R].m_RobotPosY - PAT[m_PatTagNo, DEFINE.TAR_L].m_RobotPosY + m_CamDistY2;


                //                     case DEFINE.M_1CAM2SHOT:
                // //                         t1 = -PAT[m_PatTagNo , 0].CAMCCDTHETA[0, DEFINE.XPOS] * DEFINE.PI / 180;
                // //                         double Theta_T1 = (long)((t1 * 180.0 / DEFINE.PI) * 1000.0);
                //                     break;
                if (dx != 0)
                {
                    t1 = Math.Atan(dy / dx);
                }
                else
                    t1 = 0;
                if (!TargetFlag) t1 = 0;  // target이 없을시

                Target_Mark_Dist = m_TAR_Mea_Dis = Math.Sqrt(dy * dy + dx * dx);
                //---------------------- Object ----------------------
                dx = m_MotStagePosX[DEFINE.OBJ_L] - m_MotStagePosX[DEFINE.OBJ_R] + PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosX - PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosX + m_CamDistX1;
                dy = m_MotStagePosY[DEFINE.OBJ_L] - m_MotStagePosY[DEFINE.OBJ_R] + PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosY - PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosY + m_CamDistY1;

                if (dx != 0)
                {

                    t2 = Math.Atan(dy / dx);

                }
                else t2 = 0;

                Object_Mark_Dist = m_OBJ_Mea_Dis = Math.Sqrt(dy * dy + dx * dx);
                CenterOffset = Object_Mark_Dist;

                long Degree;
                dt = (t1 - t2);
                Degree = (long)((dt * 180.0 / DEFINE.PI) * 1000.0);

                if (OffsetFlag)
                {
                    OffsetT = (double)(m_OffsetT / 1000.0 * DEFINE.PI / 180.0);
                    dt = dt + OffsetT;
                }

                double tempX = 0;
                double tempY = 0;

                if (m_AlignName == "IC_PRE_ALIGN" || m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2" || m_AlignName == "REEL_ALIGN_1" || m_AlignName == "REEL_ALIGN_2" || m_AlignName == "REEL_ALIGN_3" || m_AlignName == "REEL_ALIGN_4" || m_AlignName == "ART_PROBE")
                {
                    dt = 0;
                }

                if (m_AlignName == "FPC_CLEANER1" || m_AlignName == "FPC_CLEANER2" || m_AlignName == "FPC_CLEANER3" || m_AlignName == "FPC_CLEANER4")
                {
                    dt = 0;
                }
                if (m_AlignName == "COF_CUTTING_ALIGN")
                {
                    dt = 0;
                }
                //         dt = 0;
                // P : 회전변환대상  C: 중심점
                switch (AlignMode)
                {
                    case 0:  // center mode
                        tempX = (PAT[m_PatTagNo, 0].m_RobotPosX + PAT[m_PatTagNo, 1].m_RobotPosX) / 2;
                        tempY = (PAT[m_PatTagNo, 0].m_RobotPosY + PAT[m_PatTagNo, 1].m_RobotPosY) / 2;

                        Px = m_MotStagePosX[DEFINE.OBJ_L] - (m_CamDistX1 / 2 + tempX);
                        Py = m_MotStagePosY[DEFINE.OBJ_L] - (tempY);
                        // if (TargetAlignCMD(m_Cmd))
                        if (TargetFlag)
                        {   // Target Align
                            ImagePosX = (PAT[m_PatTagNo, 0].m_RobotPosX + PAT[m_PatTagNo, 1].m_RobotPosX - PAT[m_PatTagNo, 2].m_RobotPosX - PAT[m_PatTagNo, 3].m_RobotPosX) / 2;
                            ImagePosY = (PAT[m_PatTagNo, 0].m_RobotPosY + PAT[m_PatTagNo, 1].m_RobotPosY - PAT[m_PatTagNo, 2].m_RobotPosY - PAT[m_PatTagNo, 3].m_RobotPosY) / 2;
                        }
                        else
                        {
                            ImagePosX = (PAT[m_PatTagNo, 0].m_RobotPosX + PAT[m_PatTagNo, 1].m_RobotPosX) / 2;
                            ImagePosY = (PAT[m_PatTagNo, 0].m_RobotPosY + PAT[m_PatTagNo, 1].m_RobotPosY) / 2;
                        }
                        Cx = m_CenterX[CalStageNo];
                        Cy = m_CenterY[CalStageNo];
                        break;

                    case 1: // left mode
                        tempX = PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosX;
                        tempY = PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosY;
                        Px = m_MotStagePosX[DEFINE.OBJ_L] - tempX;
                        Py = m_MotStagePosY[DEFINE.OBJ_L] - tempY;
                        ImagePosX = PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosX;
                        ImagePosY = PAT[m_PatTagNo, DEFINE.OBJ_L].m_RobotPosY;
                        Cx = m_CenterX[CalStageNo];
                        Cy = m_CenterY[CalStageNo];
                        break;

                    case 2: // right mode
                        tempX = PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosX;
                        tempY = PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosY;
                        Px = m_MotStagePosX[DEFINE.OBJ_R] - tempX;
                        Py = m_MotStagePosY[DEFINE.OBJ_R] - tempY;
                        ImagePosX = PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosX;
                        ImagePosY = PAT[m_PatTagNo, DEFINE.OBJ_R].m_RobotPosY;
                        Cx = m_CenterX[CalStageNo];
                        Cy = m_CenterY[CalStageNo];
                        break;
                }

                ////////////////////////////////////////////////////
                // x2 = (x-cx) * cos (t) - (y - cy) * sin (t) + cx
                // y2 = (y-cy) * cos (t) + (x - cx) * sin (t) + cy
                ////////////////////////////////////////////////////

                alignX = (Px - Cx) * Math.Cos(dt) - (Py - Cy) * Math.Sin(dt) + Cx - ImagePosX;
                alignY = (Py - Cy) * Math.Cos(dt) + (Px - Cx) * Math.Sin(dt) + Cy - ImagePosY;


                if (AlignMode == 0)
                {
                    alignX = alignX + (m_CamDistX1 / 2) + tempX;
                    alignY = alignY + tempY;
                }
                if (AlignMode != 0)
                {
                    alignX = alignX + tempX;
                    alignY = alignY + tempY;
                }

                //                 double nDFx = 0;
                //                 double nDFy = 0;
                // 
                //                 nDFx = (Px - Cx) * Math.Cos(dt) - (Py - Cy) * Math.Sin(dt) + Cx + (m_CamDistX1 / 2) - m_AxisX;
                //                 nDFy = (Py - Cy) * Math.Cos(dt) + (Px - Cx) * Math.Sin(dt) + Cy - m_AxisY;


                if (OffsetFlag)
                {
                    alignX = alignX + m_OffsetX * Math.Cos(t1) - m_OffsetY * Math.Sin(t1);
                    alignY = alignY + m_OffsetY * Math.Cos(t1) + m_OffsetX * Math.Sin(t1);
                }

                m_StageX = (alignX - m_AxisX) * m_DirX;
                m_StageY = (alignY - m_AxisY) * m_DirY;
                m_StageT = ((dt * 180.0 / DEFINE.PI) * 1000.0) * m_DirT;

                if (m_AlignName == "FPC_CLEANER1" || m_AlignName == "FPC_CLEANER2" || m_AlignName == "FPC_CLEANER3" || m_AlignName == "FPC_CLEANER4")
                {
                    m_StageX = -ImagePosX + m_OffsetX;
                    m_StageY = -ImagePosY + m_OffsetY;
                    m_StageT = ((dt * 180.0 / DEFINE.PI) * 1000.0) * m_DirT;
                }

                if (m_AlignName == "CHIP_PRE")
                {
                    m_StageY *= -1; //X는하부 픽업은 툴
                }
                //                 if (Main.DEFINE.PROGRAM_TYPE == "FOB_PC4" && m_AlignName == "AI_PRE")
                //                 {
                //                     m_StageX = 0;
                //                 }
                if (m_AlignName == "PBD1" || m_AlignName == "PBD2" || m_AlignName == "PBD_FOF1" || m_AlignName == "PBD_FOF2")
                {
                    if (m_AlignType[m_PatTagNo] == DEFINE.M_2CAM1SHOT && (m_Cmd == CMD.ALIGN_OBJECT_REALIGN || m_Cmd == CMD.ALIGN_OBJTAR_ALL_REALIGN || m_Cmd == CMD.ALIGN_TARGET_REALIGN))
                    {
                        m_StageY += (long)m_PBD_ReAlignY;
                    }
                    if (Common.HEAD_MODE == DEFINE.T_MOVE)
                    {
                        m_StageX *= -1;
                        m_StageY *= -1;
                    }


                    if ((Main.DEFINE.PROGRAM_TYPE == "TFOG_PC2" && m_AlignName == "PBD1") || ((Main.DEFINE.PROGRAM_TYPE == "FOF_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOF_PC4") && (m_AlignName == "PBD1" || m_AlignName == "PBD2")))
                    {
                        m_StageY *= -1;
                    }
                }

                #region COF PRE
                if (m_AlignName == "COF_PRE")
                {

                    #region  COF THETA 가 보정시
#if false
                    double TAixs_R = 85000;
                    double TIP_TAixs_R = 18000;

                    double CenterToMark1 = 0;
                    double CenterToMark2 = 0;

                    double MarkHalf = 0;

//                      m_OBJ_Distance = 39456;
//                      dt = 1 * Main.DEFINE.radian;

                    MarkHalf = m_OBJ_Distance / 2;

                    CenterToMark1 = Math.Sqrt(TAixs_R * TAixs_R + MarkHalf * MarkHalf);
                    CenterToMark2 = Math.Sqrt(TIP_TAixs_R * TIP_TAixs_R + MarkHalf * MarkHalf);

                    double X1 = 0;
                    double Y1 = 0;
                    double X2 = 0;
                    double Y2 = 0;
                    double SUM_X = 0;
                    double SUM_Y = 0;

                    double TAixs_T = 0;
                    double TIP_TAixs_T = 0;
                    double Total_T     = 0;

                    TAixs_T = dt;
                    TIP_TAixs_T = TAixs_T * 2;

                    X1 = (CenterToMark1 * Math.Cos(Math.Acos(MarkHalf / CenterToMark1)) - CenterToMark1 * Math.Cos(Math.Acos(MarkHalf / CenterToMark1) + TAixs_T));
                    Y1 = (CenterToMark1 * Math.Sin(Math.Acos(MarkHalf / CenterToMark1)) - CenterToMark1 * Math.Sin(Math.Acos(MarkHalf / CenterToMark1) + TAixs_T));

                    X2 = (CenterToMark2 * Math.Cos(Math.Acos(MarkHalf / CenterToMark2)) - CenterToMark2 * Math.Cos(Math.Acos(MarkHalf / CenterToMark2) + TIP_TAixs_T));
                    Y2 = (CenterToMark2 * Math.Sin(Math.Acos(MarkHalf / CenterToMark2)) - CenterToMark2 * Math.Sin(Math.Acos(MarkHalf / CenterToMark2) + TIP_TAixs_T));

                    Total_T = Math.Tanh((TAixs_R * Math.Sin(TAixs_T) + TIP_TAixs_R * Math.Sin(TIP_TAixs_T)) / (TAixs_R * Math.Cos(TAixs_T) + TIP_TAixs_R * Math.Cos(TIP_TAixs_T)));

                    SUM_X = X1 + X2;
                    SUM_Y = Y1 + Y2;

                    if (OffsetFlag)
                    {
                        ImagePosX = ImagePosX + m_OffsetX * Math.Cos(t1) - m_OffsetY * Math.Sin(t1);
                        ImagePosY = ImagePosY + m_OffsetY * Math.Cos(t1) + m_OffsetX * Math.Sin(t1);
                    }
                    m_StageX = (long)(SUM_X - ImagePosX);
                    m_StageY = (long)(SUM_Y - ImagePosY);
                    m_StageT = (long)((Total_T * 180.0 / DEFINE.PI) * 1000.0) * m_DirT;
#endif
                    #endregion

                    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                    #region HEAD 보정
#if true // HEAD에서 Theta 보정시
                    if (OffsetFlag)
                    {
                        ImagePosX = ImagePosX + m_OffsetX * Math.Cos(t1) - m_OffsetY * Math.Sin(t1);
                        ImagePosY = ImagePosY + m_OffsetY * Math.Cos(t1) + m_OffsetX * Math.Sin(t1);
                    }
                    m_StageX = (long)(-ImagePosX);
                    m_StageY = (long)(-ImagePosY);
                    m_StageT = (long)((dt * 180.0 / DEFINE.PI) * 1000.0) * m_DirT;
                    m_StageT = m_StageT * -1; // HEAD 보정시 마이너스
#endif
                    #endregion
                }

                #endregion

                //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                m_StageX = Truncate(m_StageX);
                m_StageY = Truncate(m_StageY);
                m_StageT = Truncate(m_StageT);
                //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                if (VerifyFlag)
                {
                    if ((Math.Abs(m_StageX) > m_Standard[DEFINE.X] && m_Standard[DEFINE.X] != 0) ||
                        (Math.Abs(m_StageY) > m_Standard[DEFINE.Y] && m_Standard[DEFINE.Y] != 0) ||
                        (Math.Abs(m_StageT / 1000) > m_Standard[DEFINE.T] && m_Standard[DEFINE.T] != 0))
                    {
                        return false;
                    }
                    else
                    {
                        string LogMsg = "";
                        //-----------------------------------------------------------------------------------------------
                        LogMsg = "SPEC OK(" + m_StageX.ToString("0.0") + ", " + m_StageY.ToString("0.0") + ", " + (m_StageT).ToString("0.0") + ")";
                        LogdataDisplay(LogMsg, true);
                        return true;
                    }
                }
                //---------------------------------------------------------------------------------------------------------------------------------------------------------------------
                return true;
            }

            private bool Inspection()
            {
                bool bRes = true;
                double dMoveX, dMoveY;
                string LogMsg = " ";
                m_bMarkNGFlag = false;
                PAT[m_PatTagNo, 0].ResultType = enumResultType.InspectionOK;
                PAT[m_PatTagNo, 0].GrabComplete = true;
                PAT[m_PatTagNo, 0].bResult = true;
                PAT[m_PatTagNo, 1].bResult = true;
                PAT[m_PatTagNo, 0].resultGraphics.Clear();
                PAT[m_PatTagNo, 0].resultGraphics = new CogGraphicInteractiveCollection();
                PAT[m_PatTagNo, 0].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                PAT[m_PatTagNo, 0].GetImage();
                LogMsg = "Image Grab Complete";
                LogdataDisplay(LogMsg, true);

                bRes = PAT[m_PatTagNo, 0].Search();
                LogMsg = "Mark Search Complete";
                LogdataDisplay(LogMsg, true);

                if (bRes == false) //Mark Searh NG
                {
                    LogMsg = "Mark Search NG";
                    LogdataDisplay(LogMsg, true);
                    m_bMarkNGFlag = true;
                    PAT[m_PatTagNo, 0].ResultType = enumResultType.MarkSearchNG;
                    PAT[m_PatTagNo, 0].bResult = false;
                    PAT[m_PatTagNo, 1].bResult = false;
                    PAT[m_PatTagNo, 0].FixtureImage = PAT[m_PatTagNo, 0].TempImage;
                    PAT[m_PatTagNo, 0].resultDipGraphics = new CogGraphicInteractiveCollection();
                    PAT[m_PatTagNo, 0].resultDipGraphics = PAT[m_PatTagNo, 0].resultGraphics;
                }
                else //Mark Search OK
                {
                    if (bRes == true)
                    {
                        LogMsg = "Mark Search OK";
                        LogdataDisplay(LogMsg, true);
                        dMoveX = PAT[m_PatTagNo, 0].Pattern[0].Pattern.Origin.TranslationX - PAT[m_PatTagNo, 0].PMAlignResult.m_Pixel[0];
                        dMoveY = PAT[m_PatTagNo, 0].Pattern[0].Pattern.Origin.TranslationY - PAT[m_PatTagNo, 0].PMAlignResult.m_Pixel[1];
                        //shkang_s 2023.06.09 자재 로딩 위치 Log (마크 Origin 좌표)
                        LogMsg = string.Format("AmpModule Load Position, X : {0:F3}(mm) , Y: {1:F3}(mm)", PAT[m_PatTagNo, 0].Pattern[0].Results[0].GetPose().TranslationX * 13.36 / 1000, PAT[m_PatTagNo, 0].Pattern[0].Results[0].GetPose().TranslationY * 13.36 / 1000);
                        LogdataDisplay(LogMsg, true);

                        PAT[m_PatTagNo, 0].bResult = GaloInspection(dMoveX, dMoveY); // bRes로 X,Y값 보

                        PAT[m_PatTagNo, 0].resultDipGraphics = new CogGraphicInteractiveCollection();
                        PAT[m_PatTagNo, 0].resultDipGraphics = PAT[m_PatTagNo, 0].resultGraphics;
                    }
                    else
                    {
                        PAT[m_PatTagNo, 0].ResultType = enumResultType.InspectionNG;
                        LogMsg = "Inspection NG";
                        LogdataDisplay(LogMsg, true);
                        PAT[m_PatTagNo, 0].bResult = false;
                        PAT[m_PatTagNo, 1].bResult = false;
                        PAT[m_PatTagNo, 0].FixtureImage = PAT[m_PatTagNo, 0].TempImage;
                        PAT[m_PatTagNo, 0].resultDipGraphics = new CogGraphicInteractiveCollection();
                        PAT[m_PatTagNo, 0].resultDipGraphics = PAT[m_PatTagNo, 0].resultGraphics;

                    }
                }

                if (PAT[m_PatTagNo, 0].bResult == true && PAT[m_PatTagNo, 1].bResult == true)
                {
                    bRes = true;
                    LogMsg = "Inspection OK";
                    LogdataDisplay(LogMsg, true);
                    CogGraphicLabel LabelText = new CogGraphicLabel();
                    LabelText.Font = new Font(Main.DEFINE.FontStyle, 20, FontStyle.Bold);
                    LabelText.Color = CogColorConstants.Green;
                    LabelText.Text = "OK";
                    if (PAT[m_PatTagNo, 0].FitCicleTool == null)
                    {
                        if (PAT[m_PatTagNo, 0].m_bFInealignFlag == true)
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 3100;
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                LabelText.X = 2000;
                                LabelText.Y = 3000;
                            }
                        }
                        else  //Not Check
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                LabelText.X = 2000;
                                LabelText.Y = 1000;
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                LabelText.X = 0;
                                LabelText.Y = 900;
                            }
                        }
                    }
                    else  //Auto Overlay
                    {
                        if (PAT[m_PatTagNo, 0].m_bFInealignFlag == true)
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                if(Main.ProjectInfo =="_1WELL_VENT")
                                {
                                    LabelText.X = 1500;
                                    LabelText.Y = 3100;
                                }
                                else
                                {
                                    LabelText.X = 500;
                                    LabelText.Y = 3100;
                                }
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                if (Main.ProjectInfo == "_1WELL_PATH")
                                {
                                    LabelText.X = 1000;
                                    LabelText.Y = 3000;
                                }
                                else
                                {
                                    LabelText.X = 2000;
                                    LabelText.Y = 3000;
                                }
                            }
                        }
                        else  //Not Check
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                LabelText.X = 2000;
                                LabelText.Y = 1000;
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                LabelText.X = 0;
                                LabelText.Y = 900;
                            }
                        }
                    }
                    if (PAT[m_PatTagNo, 0].resultGraphics == null)
                        PAT[m_PatTagNo, 0].resultGraphics = new CogGraphicInteractiveCollection();
                    PAT[m_PatTagNo, 0].resultGraphics.Add(LabelText);
                }
                else
                {
                    PAT[m_PatTagNo, 0].ResultType = enumResultType.InspectionNG;
                    bRes = false;
                    LogMsg = "Inspection NG";
                    LogdataDisplay(LogMsg, true);
                    CogGraphicLabel LabelText = new CogGraphicLabel();
                    CogGraphicLabel LabelNGText = new CogGraphicLabel();
                    string strTemp;
                    LabelText.Font = new Font(Main.DEFINE.FontStyle, 20, FontStyle.Bold);
                    LabelText.Color = CogColorConstants.Red;
                    LabelText.Text = "NG";
                    LabelNGText.Text = "";
                    LabelNGText.Font = new Font(Main.DEFINE.FontStyle, 10, FontStyle.Bold);
                    LabelNGText.Color = CogColorConstants.Red;
                    if(m_bMarkNGFlag)
                    {
                        strTemp = string.Format("Mark NG");
                        LabelNGText.Text = strTemp;
                    }
                    if(m_bFilmNGFlag)
                    {
                        strTemp = string.Format("Film Align NG");
                        LabelNGText.Text = strTemp;
                    }
                    if (m_bThetaNGFlag)
                    {
                        strTemp = string.Format("Theta NG : {0:F2}°, SPEC : ±{1:F2}°", PAT[m_PatTagNo, 0].m_FinealignGapT * 180 / Math.PI, PAT[m_PatTagNo, 0].m_FinealignThetaSpec);
                        LabelNGText.Text = strTemp;
                    }
                    if (PAT[m_PatTagNo, 0].FitCicleTool == null)
                    {
                        if (PAT[m_PatTagNo, 0].m_bFInealignFlag == true)
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                LabelText.X = 2500;
                                LabelText.Y = 3600;
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                LabelText.X = 2000;
                                LabelText.Y = 3000;
                            }
                        }
                        else
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                LabelText.X = 2000;
                                LabelText.Y = 1000;
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                LabelText.X = 0;
                                LabelText.Y = 900;
                            }
                        }
                    }
                    else   //Auto Overlay
                    {
                        if (PAT[m_PatTagNo, 0].m_bFInealignFlag == true)
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                if (Main.ProjectInfo == "_1WELL_VENT")
                                {
                                    LabelText.X = 1500;
                                    LabelText.Y = 3100;
                                    if (m_bThetaNGFlag || m_bMarkNGFlag || m_bFilmNGFlag || m_bThetaMarkNGFlag)
                                    {
                                        LabelText.X = 2500;
                                        LabelText.Y = 3600;
                                        LabelNGText.X = 2000;
                                        LabelNGText.Y = 3800;
                                    }
                                }
                                else
                                {
                                    LabelText.X = 500;
                                    LabelText.Y = 3100;
                                    if (m_bThetaNGFlag || m_bMarkNGFlag || m_bFilmNGFlag || m_bThetaMarkNGFlag)  
                                    {
                                        LabelText.X = 2500;
                                        LabelText.Y = 3600;
                                        LabelNGText.X = 2000;
                                        LabelNGText.Y = 3800;
                                    }
                                }
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                if (Main.ProjectInfo == "_1WELL_PATH")
                                {
                                    LabelText.X = 1000;
                                    LabelText.Y = 3000;
                                }
                                else
                                {
                                    LabelText.X = 2000;
                                    LabelText.Y = 3000;
                                    if (m_bThetaNGFlag || m_bMarkNGFlag || m_bFilmNGFlag || m_bThetaMarkNGFlag)  
                                    {
                                        LabelText.X = 2500;
                                        LabelText.Y = 3600;
                                        LabelNGText.X = 2000;
                                        LabelNGText.Y = 3800;
                                    }
                                }
                            }
                        }
                        else //FineAlign 기능 사용X 시 OVERLAY
                        {
                            if (Main.DEFINE.UNIT_TYPE == "VENT")
                            {
                                LabelText.X = 2000;
                                LabelText.Y = 1000;
                            }
                            else if (Main.DEFINE.UNIT_TYPE == "PATH")
                            {
                                LabelText.X = 0;
                                LabelText.Y = 900;
                            }
                        }
                    }
                    if (PAT[m_PatTagNo, 0].resultGraphics == null)
                        PAT[m_PatTagNo, 0].resultGraphics = new CogGraphicInteractiveCollection();
                    PAT[m_PatTagNo, 0].resultGraphics.Add(LabelText);
                    if (m_bThetaNGFlag || m_bMarkNGFlag || m_bFilmNGFlag || m_bThetaMarkNGFlag)  
                        PAT[m_PatTagNo, 0].resultGraphics.Add(LabelNGText);

                }

                return bRes;
            }

            public bool GaloOppositeInspection(int nROI, int toolType, object tool, CogImage8Grey cogImage, out double[] ResultData, out int NonCaliperCnt, ref CogGraphicInteractiveCollection GraphicData)
            {
                NonCaliperCnt = 0;
                if (toolType == (int)enumROIType.FindLine)
                {
                    CogFindLineTool m_LineTool = tool as CogFindLineTool;
                    bool MoveTypeY = false;
                    bool bRes = true;

                    int[] nCaliperCount = new int[2];
                    CogFindLineTool[] SingleFindLine = new CogFindLineTool[2];
                    PointF[,] RawSearchData = new PointF[2, 100];
                    ResultData = new double[100];
                    CogDistancePointPointTool DistanceData = new CogDistancePointPointTool();
                    double startPosX = m_LineTool.RunParams.ExpectedLineSegment.StartX;
                    double startPosY = m_LineTool.RunParams.ExpectedLineSegment.StartY;
                    double EndPosX = m_LineTool.RunParams.ExpectedLineSegment.EndX;
                    double EndPosY = m_LineTool.RunParams.ExpectedLineSegment.EndY;
                    double MovePos1, MovePos2;
                    double Move = m_LineTool.RunParams.CaliperSearchLength / 2;
                    double diretion = m_LineTool.RunParams.CaliperSearchDirection;

                    double OriginalLength = m_LineTool.RunParams.CaliperSearchLength;
                    double Cal_StartX = 0;
                    double Cal_StartY = 0;
                    double Cal_EndX = 0;
                    double Cal_EndY = 0;
                    double searchDirection = m_LineTool.RunParams.CaliperSearchDirection;
                    CogCaliperPolarityConstants edgePolarity = m_LineTool.RunParams.CaliperRunParams.Edge0Polarity;

                    double noneEdge_Threshold = 0;
                    int noeEdge_FilterSize = 0;

                    try
                    {
                        //1Well 일때 동작
                        if (!PAT[m_PatTagNo, 0].m_bFInealignFlag)
                        {
                            if (Math.Abs(EndPosY - startPosY) < 20)
                            {
                                MoveTypeY = true;
                                if (startPosX > EndPosX)
                                {
                                    diretion *= -1;
                                }
                            }
                            else
                            {
                                MoveTypeY = false;
                                if (startPosY > EndPosY)
                                {
                                    diretion *= -1;
                                }

                            }
                        }

                        #region FindLine Search
                        bool isTwiceFixture = false;
                        CogFixtureTool mCogFixtureTool2 = new CogFixtureTool();

                        for (int i = 0; i < 2; i++) //Left, Right 의미
                        {
                            SingleFindLine[i] = new CogFindLineTool();
                            SingleFindLine[i] = m_LineTool;
                            noneEdge_Threshold = SingleFindLine[i].RunParams.CaliperRunParams.ContrastThreshold;
                            noeEdge_FilterSize = SingleFindLine[i].RunParams.CaliperRunParams.FilterHalfSizeInPixels;

                            if (i == 1)
                            {
                                //하나의 FindLineTool방향만 정반대로 돌려서 Search진행
                                double dist = SingleFindLine[i].RunParams.CaliperSearchDirection;
                                SingleFindLine[i].RunParams.CaliperSearchDirection *= (-1);
                                SingleFindLine[i].RunParams.CaliperRunParams.Edge0Polarity = SingleFindLine[i].RunParams.CaliperRunParams.Edge1Polarity;


                                if (PAT[m_PatTagNo, 0].m_bFInealignFlag)
                                {
                                    double Calrotation = PAT[m_PatTagNo, 0].m_FinealignGapT - SingleFindLine[i].RunParams.ExpectedLineSegment.Rotation;

                                    Main.AlignUnit[m_AlignNo].Position_Calculate(SingleFindLine[i].RunParams.ExpectedLineSegment.StartX, SingleFindLine[i].RunParams.ExpectedLineSegment.StartY,
                                            m_LineTool.RunParams.CaliperSearchLength / 2, Calrotation, out Cal_StartX, out Cal_StartY);

                                    Main.AlignUnit[m_AlignNo].Position_Calculate(SingleFindLine[i].RunParams.ExpectedLineSegment.EndX, SingleFindLine[i].RunParams.ExpectedLineSegment.EndY,
                                            m_LineTool.RunParams.CaliperSearchLength / 2, Calrotation, out Cal_EndX, out Cal_EndY);

                                    SingleFindLine[i].RunParams.ExpectedLineSegment.StartX = Cal_StartX;
                                    SingleFindLine[i].RunParams.ExpectedLineSegment.EndX = Cal_EndX;
                                    SingleFindLine[i].RunParams.ExpectedLineSegment.StartY = Cal_StartY;
                                    SingleFindLine[i].RunParams.ExpectedLineSegment.EndY = Cal_EndY;
                                }
                                else
                                {
                                    if (!MoveTypeY)
                                    {
                                        if (diretion < 0)
                                        {
                                            MovePos1 = SingleFindLine[i].RunParams.ExpectedLineSegment.StartX + Move;
                                            MovePos2 = SingleFindLine[i].RunParams.ExpectedLineSegment.EndX + Move;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.StartX = MovePos1;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.EndX = MovePos2;
                                        }
                                        else
                                        {
                                            MovePos1 = SingleFindLine[i].RunParams.ExpectedLineSegment.StartX - Move;
                                            MovePos2 = SingleFindLine[i].RunParams.ExpectedLineSegment.EndX - Move;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.StartX = MovePos1;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.EndX = MovePos2;
                                        }
                                    }
                                    else
                                    {
                                        if (diretion < 0)
                                        {

                                            MovePos1 = SingleFindLine[i].RunParams.ExpectedLineSegment.StartY - Move;
                                            MovePos2 = SingleFindLine[i].RunParams.ExpectedLineSegment.EndY - Move;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.StartY = MovePos1;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.EndY = MovePos2;
                                        }
                                        else
                                        {
                                            MovePos1 = SingleFindLine[i].RunParams.ExpectedLineSegment.StartY + Move;
                                            MovePos2 = SingleFindLine[i].RunParams.ExpectedLineSegment.EndY + Move;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.StartY = MovePos1;
                                            SingleFindLine[i].RunParams.ExpectedLineSegment.EndY = MovePos2;
                                        }
                                    }
                                }
                            }

                            SingleFindLine[i].RunParams.CaliperRunParams.EdgeMode = CogCaliperEdgeModeConstants.SingleEdge;
                            SingleFindLine[i].LastRunRecordDiagEnable = CogFindLineLastRunRecordDiagConstants.None;

                            if (PAT[m_PatTagNo, 0].m_InspParameter[nROI].bThresholdUse == true && i == 1)
                            {
                                // Crop 처리
                                var transform = Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].TempFixtureTrans;
                                var cropResult = GetCropImage(cogImage, SingleFindLine[i], transform, out CogRectangle cropRect);

                                EdgeAlgorithm edgeAlgorithm = new EdgeAlgorithm();
                                edgeAlgorithm.Threshold = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iThreshold;

                                var image = cropResult.Item1 as CogImage8Grey;
                                image.CoordinateSpaceTree = new CogCoordinateSpaceTree();
                                image.SelectedSpaceName = "@";

                                edgeAlgorithm.IgnoreSize = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iIgnoreSize;
                                Mat convertImage = edgeAlgorithm.Inspect(image, ref SingleFindLine[i], cropResult.Item2, transform, cropRect);

                                double lengthX = Math.Abs(SingleFindLine[i].RunParams.ExpectedLineSegment.StartX - SingleFindLine[i].RunParams.ExpectedLineSegment.EndX);
                                double lengthY = Math.Abs(SingleFindLine[i].RunParams.ExpectedLineSegment.StartY - SingleFindLine[i].RunParams.ExpectedLineSegment.EndY);

                                int searchedValue = -1;
                                List<Point> boundRectPointList = new List<Point>();

                                if (lengthX > lengthY) // 가로
                                {
                                    double startX = SingleFindLine[i].RunParams.ExpectedLineSegment.StartX;
                                    double startY = SingleFindLine[i].RunParams.ExpectedLineSegment.StartY;
                                    double endX = SingleFindLine[i].RunParams.ExpectedLineSegment.EndX;
                                    double endY = SingleFindLine[i].RunParams.ExpectedLineSegment.EndY;
                                    transform.MapPoint(startX, startY, out double orgStartX, out double orgStartY);
                                    transform.MapPoint(endX, endY, out double orgEndX, out double orgEndY);

                                    transform.MapPoint(cropRect.X, cropRect.Y, out double mappingStartX, out double mappingStartY);

                                    if (orgStartX > orgEndX) // 화살표 방향 아래에서 위
                                    {
                                        int topCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iTopCutPixel;
                                        int bottomCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iBottomCutPixel;

                                        var minPosY = edgeAlgorithm.GetVerticalMinEdgeTopPosY(convertImage, topCutPixel, bottomCutPixel);
                                        if (minPosY.Count > 0)
                                        {
                                            searchedValue = minPosY.Min();
                                            int maskX = (int)mappingStartX;
                                            int maskY = searchedValue + (int)mappingStartY;

                                            Rectangle rect = new Rectangle((int)mappingStartX, 0, convertImage.Width, maskY);

                                            int maskWidth = convertImage.Width;
                                            int maskHeight = rect.Height;

                                            boundRectPointList.Add(new Point(maskX, maskY));
                                            boundRectPointList.Add(new Point(maskX, maskY - convertImage.Height));
                                            boundRectPointList.Add(new Point(maskX + maskWidth, maskY - convertImage.Height));
                                            boundRectPointList.Add(new Point(maskX + maskWidth, maskY));
                                        }
                                    }
                                    else// 화살표 방향 위에서 아래
                                    {
                                        int topCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iTopCutPixel;
                                        int bottomCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iBottomCutPixel;

                                        var edgePointList = edgeAlgorithm.GetVerticalEdgeBottomPos(convertImage, topCutPixel, bottomCutPixel);
                                        if (edgePointList.Count > 0)
                                        {
                                            var target = edgePointList.OrderByDescending(edgePoint => edgePoint.PointY);
                                            var minEdge = target.Last();
                                            var maxEdge = target.First();

                                            int leftTopY = (int)mappingStartY;
                                            int rightTopY = (int)mappingStartY;

                                            int leftTopTempY = minEdge.PointY > maxEdge.PointY ? maxEdge.PointY : minEdge.PointY;
                                            int rightTopTempY = minEdge.PointY > maxEdge.PointY ? minEdge.PointY : maxEdge.PointY;

                                            leftTopY += leftTopTempY;
                                            rightTopY += rightTopTempY;

                                            searchedValue = 1;

                                            int maskX = (int)mappingStartX;
                                            int maskY = (int)mappingStartY; // Y 좌표 설정

                                            boundRectPointList.Add(new Point(maskX, leftTopY));
                                            boundRectPointList.Add(new Point(maskX + convertImage.Width, rightTopY));
                                            boundRectPointList.Add(new Point(maskX + convertImage.Width, rightTopY + convertImage.Height));
                                            boundRectPointList.Add(new Point(maskX, leftTopY + convertImage.Height));
                                        }
                                    }
                                }
                                else
                                {
                                    double startX = SingleFindLine[i].RunParams.ExpectedLineSegment.StartX;
                                    double startY = SingleFindLine[i].RunParams.ExpectedLineSegment.StartY;
                                    double endX = SingleFindLine[i].RunParams.ExpectedLineSegment.EndX;
                                    double endY = SingleFindLine[i].RunParams.ExpectedLineSegment.EndY;
                                    transform.MapPoint(startX, startY, out double orgStartX, out double orgStartY);
                                    transform.MapPoint(endX, endY, out double orgEndX, out double orgEndY);

                                    transform.MapPoint(cropRect.X, cropRect.Y, out double mappingStartX, out double mappingStartY);
                                    if (orgStartX > orgEndX) // 화살표 방향 오른쪽에서 왼쪽
                                    {
                                        int topCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iTopCutPixel;
                                        int bottomCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iBottomCutPixel;

                                        searchedValue = edgeAlgorithm.GetHorizontalMinEdgePosY(convertImage, topCutPixel, bottomCutPixel);
                                        if (searchedValue >= 0)
                                        {
                                            // 마스크를 그릴 영역의 X, Y 좌표 계산
                                            int maskX = searchedValue + (int)mappingStartX; // X 좌표 설정
                                            int maskY = (int)mappingStartY; // Y 좌표 설정

                                            Rectangle rect = new Rectangle((int)mappingStartX, 0, convertImage.Width, maskY);

                                            // 마스크를 그릴 영역의 너비와 높이 계산
                                            int maskWidth = convertImage.Width; // 너비 설정
                                            int maskHeight = convertImage.Height; // 높이 설정

                                            boundRectPointList.Add(new Point(maskX, maskY));
                                            boundRectPointList.Add(new Point(maskX - convertImage.Width, maskY));
                                            boundRectPointList.Add(new Point(maskX - convertImage.Width, maskY + maskHeight));
                                            boundRectPointList.Add(new Point(maskX, maskY + maskHeight));
                                        }
                                    }
                                    else // 화살표 방향 왼쪽에서 오른쪽
                                    {
                                        int topCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iTopCutPixel;
                                        int bottomCutPixel = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iBottomCutPixel;

                                        var edgePointList = edgeAlgorithm.GetHorizontalEdgePos(convertImage, topCutPixel, bottomCutPixel);
                                        if (edgePointList.Count > 0)
                                        {
                                            var target = edgePointList.OrderByDescending(edgePoint => edgePoint.PointX);
                                            var minEdge = target.Last();
                                            var maxEdge = target.First();

                                            int leftTopTempX = minEdge.PointY > maxEdge.PointY ? maxEdge.PointX : minEdge.PointX;
                                            int leftBottomTempX = minEdge.PointY > maxEdge.PointY ? minEdge.PointX : maxEdge.PointX;

                                            int leftTopX = (int)mappingStartX;
                                            int leftBottomX = (int)mappingStartX;

                                            leftTopX += leftTopTempX;
                                            leftBottomX += leftBottomTempX;

                                            searchedValue = 1;

                                            //searchedValue = min;
                                            int maskX = (int)mappingStartX;
                                            int maskY = (int)mappingStartY; // Y 좌표 설정

                                            boundRectPointList.Add(new Point(leftTopX, maskY));
                                            boundRectPointList.Add(new Point(leftTopX + convertImage.Width, maskY));
                                            boundRectPointList.Add(new Point(leftBottomX + convertImage.Width, maskY + convertImage.Height));
                                            boundRectPointList.Add(new Point(leftBottomX, maskY + convertImage.Height));

                                        }
                                    }
                                    //   

                                }

                                if (searchedValue >= 0)
                                {
                                    int MaskingValue = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iMaskingValue; // UI 에 빼야함
                                    MCvScalar maskingColor = new MCvScalar(MaskingValue);

                                    Mat matImage = edgeAlgorithm.GetConvertMatImage(cogImage.CopyBase(CogImageCopyModeConstants.CopyPixels) as CogImage8Grey);
                                    CvInvoke.FillPoly(matImage, new VectorOfPoint(boundRectPointList.ToArray()), maskingColor);

                                    var filterImage = edgeAlgorithm.GetConvertCogImage(matImage);

                                    SingleFindLine[i].RunParams.CaliperRunParams.ContrastThreshold = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iEdgeCaliperThreshold;
                                    SingleFindLine[i].RunParams.CaliperRunParams.FilterHalfSizeInPixels = PAT[m_PatTagNo, 0].m_InspParameter[nROI].iEdgeCaliperFilterSize;
                                    SingleFindLine[i].InputImage = (CogImage8Grey)filterImage;
                                }
                                else
                                {
                                    // Edge 못찾은 경우
                                    SingleFindLine[i].RunParams.CaliperRunParams.ContrastThreshold = noneEdge_Threshold;
                                    SingleFindLine[i].RunParams.CaliperRunParams.FilterHalfSizeInPixels = noeEdge_FilterSize;
                                    SingleFindLine[i].InputImage = cogImage;
                                }

                                if (cogImage.SelectedSpaceName == "@\\Fixture\\Fixture")
                                    isTwiceFixture = true;

                                if (searchedValue >= 0)
                                {
                                    mCogFixtureTool2.InputImage = SingleFindLine[i].InputImage;
                                    mCogFixtureTool2.RunParams.UnfixturedFromFixturedTransform = Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].TempFixtureTrans;
                                    mCogFixtureTool2.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
                                    mCogFixtureTool2.Run();

                                    SingleFindLine[i].InputImage = (CogImage8Grey)mCogFixtureTool2.OutputImage;
                                }
                                else
                                    isTwiceFixture = true;
                            }
                            else
                            {
                                SingleFindLine[i].InputImage = cogImage;
                            }
                            SingleFindLine[i].Run();
                            if (SingleFindLine[i].Results == null)
                            {
                                m_LineTool.RunParams.CaliperRunParams.ContrastThreshold = noneEdge_Threshold;
                                m_LineTool.RunParams.CaliperRunParams.FilterHalfSizeInPixels = noeEdge_FilterSize;
                                m_LineTool.RunParams.CaliperSearchDirection = searchDirection;
                                m_LineTool.RunParams.CaliperRunParams.Edge0Polarity = edgePolarity;
                                m_LineTool.RunParams.ExpectedLineSegment.StartX = startPosX;
                                m_LineTool.RunParams.ExpectedLineSegment.StartY = startPosY;
                                m_LineTool.RunParams.ExpectedLineSegment.EndX = EndPosX;
                                m_LineTool.RunParams.ExpectedLineSegment.EndY = EndPosY;
                                continue;
                            }
                            //Search OK
                            if (SingleFindLine[i].Results != null)
                                {
                                    if( SingleFindLine[i].Results.Count > 0)
                                    {
                                        ResultData = new double[SingleFindLine[i].Results.Count];
                                        for (int j = 0; j < SingleFindLine[i].Results.Count; j++)
                                        {
                                            if (isTwiceFixture)
                                            {
                                                var graphic = SingleFindLine[i].Results[j].CreateResultGraphics(CogFindLineResultGraphicConstants.CaliperEdge /*| CogFindLineResultGraphicConstants.CaliperRegion*/);

                                                foreach (var item in graphic.Shapes)
                                                {
                                                    if (item is CogLineSegment line)
                                                    {
                                                        cogImage.GetTransform("@", cogImage.SelectedSpaceName).MapPoint(line.StartX, line.StartY, out double mX, out double mY);
                                                        mCogFixtureTool2.RunParams.UnfixturedFromFixturedTransform.MapPoint(line.StartX, line.StartY, out double mappingStartX, out double mappingStartY);
                                                        line.StartX = mappingStartX;
                                                        line.StartY = mappingStartY;

                                                        mCogFixtureTool2.RunParams.UnfixturedFromFixturedTransform.MapPoint(line.EndX, line.EndY, out double mappingEndX, out double mappingEndY);
                                                        line.EndX = mappingEndX;
                                                        line.EndY = mappingEndY;
                                                    }
                                                }

                                                GraphicData.Add(graphic);
                                            }
                                            else
                                            {
                                                var graphic = SingleFindLine[i].Results[j].CreateResultGraphics(CogFindLineResultGraphicConstants.CaliperEdge /*| CogFindLineResultGraphicConstants.CaliperRegion*/);
                                                GraphicData.Add(graphic);
                                            }
                                      
                                        if (SingleFindLine[i].Results[j].CaliperResults.Count == 1)
                                            {
                                                RawSearchData[i, j].X = (float)SingleFindLine[i].Results[j].CaliperResults[0].Edge0.PositionX;
                                                RawSearchData[i, j].Y = (float)SingleFindLine[i].Results[j].CaliperResults[0].Edge0.PositionY;
                                            }
                                            else
                                            {
                                                RawSearchData[i, j].X = 0;
                                                RawSearchData[i, j].Y = 0;
                                                NonCaliperCnt++;
                                            }
                                        }
                                    }
                                }
                                //Search NG
                                else
                                {
                                    bRes = false;
                                }
                            }
                        #endregion
                        
                        #region Result Data Calculate

                        if (SingleFindLine[0].Results != null)
                        {
                            for (int i = 0; i < SingleFindLine[0].Results.Count; i++)
                            {
                                //두 점 사이의 거리 
                                ResultData[i] = (Math.Sqrt((Math.Pow(RawSearchData[0, i].X - RawSearchData[1, i].X, 2) +
                                Math.Pow(RawSearchData[0, i].Y - RawSearchData[1, i].Y, 2)))) * 13.36 / 1000;   //단위변경 px->mm
                            }
                        }
                       
                        SingleFindLine[1].RunParams.CaliperSearchDirection *= (-1);
                        #endregion
                        m_LineTool.RunParams.CaliperRunParams.ContrastThreshold = noneEdge_Threshold;
                        m_LineTool.RunParams.CaliperRunParams.FilterHalfSizeInPixels = noeEdge_FilterSize;
                        m_LineTool.RunParams.CaliperSearchDirection = searchDirection;
                        m_LineTool.RunParams.CaliperRunParams.Edge0Polarity = edgePolarity;
                        m_LineTool.RunParams.ExpectedLineSegment.StartX = startPosX;
                        m_LineTool.RunParams.ExpectedLineSegment.StartY = startPosY;
                        m_LineTool.RunParams.ExpectedLineSegment.EndX = EndPosX;
                        m_LineTool.RunParams.ExpectedLineSegment.EndY = EndPosY;
                        return bRes;
                    }
                    catch (Exception err)
                    {
                        m_LineTool.RunParams.CaliperRunParams.ContrastThreshold = noneEdge_Threshold;
                        m_LineTool.RunParams.CaliperRunParams.FilterHalfSizeInPixels = noeEdge_FilterSize;
                        m_LineTool.RunParams.CaliperSearchDirection = searchDirection;
                        m_LineTool.RunParams.CaliperRunParams.Edge0Polarity = edgePolarity;
                        m_LineTool.RunParams.ExpectedLineSegment.StartX = startPosX;
                        m_LineTool.RunParams.ExpectedLineSegment.StartY = startPosY;
                        m_LineTool.RunParams.ExpectedLineSegment.EndX = EndPosX;
                        m_LineTool.RunParams.ExpectedLineSegment.EndY = EndPosY;

                        string LogMsg;
                        LogMsg = "Inspeciton Excetion NG"; LogMsg += "Error : " + err.ToString();
                        LogdataDisplay(LogMsg, true);
                        LogdataDisplay(nROI.ToString(), true);
                        ResultData = new double[] { };
                        NonCaliperCnt = 0;
                        GraphicData = new CogGraphicInteractiveCollection();
                        return false;
                    }

                }
                else   //Circle Tool
                {
                    try
                    {
                        CogFindCircleTool m_CircleTool = tool as CogFindCircleTool;
                        bool bRes = true;
                        int nCaliperCount;
                        CogFindCircleTool[] SingleCircleLine = new CogFindCircleTool[2];
                        PointF[,] RawSearchData = new PointF[2, 100];
                        ResultData = new double[100];
                        CogDistancePointPointTool DistanceData = new CogDistancePointPointTool();

                        #region FindLine Search
                        m_CircleTool.RunParams.CaliperRunParams.EdgeMode = CogCaliperEdgeModeConstants.Pair;
                        m_CircleTool.InputImage = cogImage;
                        m_CircleTool.Run();

                        if (m_CircleTool.Results != null)
                        {
                            nCaliperCount = m_CircleTool.Results.Count;
                            NonCaliperCnt = 0;
                        }
                        else
                        {
                            NonCaliperCnt = 0;
                            return false;
                        }
                        //Search OK
                        if (m_CircleTool.Results != null || m_CircleTool.Results.Count > 0)
                        {
                            ResultData = new double[m_CircleTool.Results.Count];
                            for (int j = 0; j < m_CircleTool.Results.Count; j++)
                            {

                                GraphicData.Add(m_CircleTool.Results[j].CreateResultGraphics(CogFindCircleResultGraphicConstants.CaliperEdge));
                                if (m_CircleTool.Results[j].CaliperResults.Count >= 1)
                                {
                                    RawSearchData[0, j].X = (float)m_CircleTool.Results[j].CaliperResults[0].Edge0.PositionX;
                                    RawSearchData[0, j].Y = (float)m_CircleTool.Results[j].CaliperResults[0].Edge0.PositionY;
                                    RawSearchData[1, j].X = (float)m_CircleTool.Results[j].CaliperResults[0].Edge1.PositionX;
                                    RawSearchData[1, j].Y = (float)m_CircleTool.Results[j].CaliperResults[0].Edge1.PositionY;
                                }
                                else
                                {
                                    RawSearchData[0, j].X = 0;
                                    RawSearchData[0, j].Y = 0;
                                    RawSearchData[1, j].X = 0;
                                    RawSearchData[1, j].Y = 0;
                                    NonCaliperCnt++;
                                }
                            }
                        }
                        //Search NG
                        else
                        {
                            bRes = false;
                        }

                        #endregion

                        #region Result Data Calculate
                        for (int i = 0; i < m_CircleTool.Results.Count; i++)
                        {
                            //두 점 사이의 거리 
                            ResultData[i] = (Math.Sqrt((Math.Pow(RawSearchData[0, i].X - RawSearchData[1, i].X, 2) +
                            Math.Pow(RawSearchData[0, i].Y - RawSearchData[1, i].Y, 2)))) * 13.36 / 1000;   //단위변경 px ->mm
                        }

                        #endregion

                        return bRes;
                    }
                    catch (Exception err)
                    {
                        string LogMsg;
                        LogMsg = "Inspeciton Excetion NG"; LogMsg += "Error : " + err.ToString();
                        LogdataDisplay(LogMsg, true);
                        LogdataDisplay(nROI.ToString(), true);
                        ResultData = new double[] { };
                        NonCaliperCnt = 0;
                        GraphicData = new CogGraphicInteractiveCollection();
                        return false;
                    }
                }
            }

            public Tuple<CogImage8Grey, EdgeDirection> GetCropImage(CogImage8Grey cogImage, CogFindLineTool tool, CogTransform2DLinear transform, out CogRectangle cropRect)
            {
                cropRect = new CogRectangle();
                EdgeDirection direction = EdgeDirection.Top;

                double MinLineDegreeStand = 1.396;
                double MaxLineDegreeStand = 1.745;

                //1.가로, 세로 확인                     
                if (Math.Abs(tool.RunParams.ExpectedLineSegment.Rotation) > MinLineDegreeStand &&
                   Math.Abs(tool.RunParams.ExpectedLineSegment.Rotation) < MaxLineDegreeStand)
                {
                    direction = EdgeDirection.Left;
                    //2.세로인 경우, 사분면 중 어디에 위치해 있는지 확인
                    if (tool.RunParams.ExpectedLineSegment.StartY < 0) //음수 1,4분면
                    {
                        //3.Start Y, End Y 중 어떤게 상단에 위치해 있는지 확인
                        if (tool.RunParams.ExpectedLineSegment.StartY <
                            tool.RunParams.ExpectedLineSegment.EndY)
                        {
                            //Start Y가 상단에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.StartX - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.StartY;
                            cropRect.Width = tool.RunParams.CaliperSearchLength;
                            cropRect.Height = tool.RunParams.ExpectedLineSegment.Length;
                        }
                        else
                        {
                            //End Y가 상단에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.EndX - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.EndY;
                            cropRect.Width = tool.RunParams.CaliperSearchLength;
                            cropRect.Height = tool.RunParams.ExpectedLineSegment.Length;
                        }
                    }
                    else //양수 2,3분면
                    {
                        //3.Start Y, End Y 중 어떤게 상단에 위치해 있는지 확인
                        if (tool.RunParams.ExpectedLineSegment.StartY <
                            tool.RunParams.ExpectedLineSegment.EndY)
                        {
                            //Start Y가 상단에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.StartX - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.StartY;
                            cropRect.Width = tool.RunParams.CaliperSearchLength;
                            cropRect.Height = tool.RunParams.ExpectedLineSegment.Length;
                        }
                        else
                        {
                            //End Y가 상단에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.EndX - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.EndY;
                            cropRect.Width = tool.RunParams.CaliperSearchLength;
                            cropRect.Height = tool.RunParams.ExpectedLineSegment.Length;
                        }
                    }
                }
                else
                {
                    direction = EdgeDirection.Top;
                    //2.가로인 경우, 사분면 중 어디에 위치해 있는지 확인
                    if (tool.RunParams.ExpectedLineSegment.StartX < 0) //음수 3,4분면
                    {
                        //3.Start X, End X 중 어떤게 좌측에 위치해 있는지 확인
                        if (tool.RunParams.ExpectedLineSegment.StartX <
                           tool.RunParams.ExpectedLineSegment.EndX)
                        {
                            //Start X가  좌측에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.StartX;
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.StartY - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Width = tool.RunParams.ExpectedLineSegment.Length;
                            cropRect.Height = tool.RunParams.CaliperSearchLength;
                        }
                        else
                        {
                            //End X가 좌측에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.EndX;
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.EndY - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Width = tool.RunParams.ExpectedLineSegment.Length;
                            cropRect.Height = tool.RunParams.CaliperSearchLength;
                        }

                    }
                    else //양수 1,2분면
                    {
                        //3.Start X, End X 중 어떤게 좌측에 위치해 있는지 확인
                        if (tool.RunParams.ExpectedLineSegment.StartX <
                           tool.RunParams.ExpectedLineSegment.EndX)
                        {
                            //Start X가 좌측에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.StartX;
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.StartY - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Width = tool.RunParams.ExpectedLineSegment.Length;
                            cropRect.Height = tool.RunParams.CaliperSearchLength;
                        }
                        else
                        {
                            //End X가 좌측에 있음
                            cropRect.X = tool.RunParams.ExpectedLineSegment.EndX;
                            cropRect.Y = tool.RunParams.ExpectedLineSegment.EndY - (tool.RunParams.CaliperSearchLength / 2);
                            cropRect.Width = tool.RunParams.ExpectedLineSegment.Length;
                            cropRect.Height = tool.RunParams.CaliperSearchLength;
                        }
                    }

                }

                EdgeAlgorithm edge = new EdgeAlgorithm();
                Mat mat = edge.GetConvertMatImage(cogImage);

                transform.MapPoint(cropRect.X, cropRect.Y, out double cropX, out double cropY);
                Mat cropMat = null;
      
                Rectangle rectFromMat = new Rectangle();
                rectFromMat.X = (int)cropX;
                rectFromMat.Y = (int)cropY;
                rectFromMat.Width = (int)cropRect.Width;
                rectFromMat.Height = (int)cropRect.Height;

                cropMat = edge.CropRoi(mat, rectFromMat);

                mat.Dispose();

                return new Tuple<CogImage8Grey, EdgeDirection>(edge.GetConvertCogImage(cropMat), direction);
            }

            private bool GaloInspection(double MoveX, double MoveY)
            {
                int nRoi = 0, jCaliperIndex = 0;
                enumROIType m_ROYTpe = new enumROIType();
                try
                {
                    bool bRes = true;
                    bool[] bROIRes;
                    double[] dDistance;
                    CogGraphicLabel[] Label;

                    bROIRes = new bool[PAT[m_PatTagNo, 0].m_InspParameter.Count];
                    dDistance = new double[PAT[m_PatTagNo, 0].m_InspParameter.Count];
                    int IDistgnore = 0;
                    double dSpec = 0.0;
             
                    m_bThetaNGFlag = false;
                    m_bFilmNGFlag = false;
                    m_bThetaMarkNGFlag = false;
                    if (FinalTracking(MoveX, MoveY) == false)
                    {
                        PAT[m_PatTagNo, 0].ResultType = enumResultType.CrossLineNG;
                        PAT[m_PatTagNo, 0].FixtureImage = PAT[m_PatTagNo, 0].TempImage;
                        return false;
                    }

                    for (int i = 0; i < PAT[m_PatTagNo, 0].m_InspParameter.Count; i++)
                    {
                        if (i == 0)
                        {
                            for (int iHitogramROI = 0; iHitogramROI < PAT[m_PatTagNo, 0].m_InspParameter[i].iHistogramROICnt; iHitogramROI++)
                            {
                                double RectCenterX, RectCenterY;
                                CogHistogramTool InspHistogramTool = PAT[m_PatTagNo, 0].m_InspParameter[i].m_CogHistogramTool[iHitogramROI];
                                CogRectangleAffine Rect = (CogRectangleAffine)InspHistogramTool.Region;
                                RectCenterX = Rect.CenterX;
                                RectCenterY = Rect.CenterY;

                                InspHistogramTool.InputImage = PAT[m_PatTagNo, 0].FixtureImage;
                                CogRectangleAffine ResultRect = (CogRectangleAffine)InspHistogramTool.Region;
                                InspHistogramTool.Run();
                                if (InspHistogramTool.Result.Mean > PAT[m_PatTagNo, 0].m_InspParameter[i].iHistogramSpec[iHitogramROI])
                                {
                                    CogGraphicLabel ResulteLabel = new CogGraphicLabel();

                                    double dx, dy;
                                    ResultRect.Color = CogColorConstants.Red;
                                    ResultRect.Interactive = false;
                                    ResulteLabel.Font = new Font(Main.DEFINE.FontStyle, 15);
                                    ResulteLabel.Color = CogColorConstants.Red;
                                    ResulteLabel.Text = InspHistogramTool.Result.Mean.ToString("F3");
                                    dx = RectCenterX;
                                    dy = RectCenterY;
                                    ResulteLabel.X = dx;
                                    ResulteLabel.Y = dy;
                                    PAT[m_PatTagNo, 0].resultGraphics.Add(ResulteLabel);
                                    PAT[m_PatTagNo, 0].resultGraphics.Add(ResultRect);
                                    string LogMsg;
                                    LogMsg = string.Format("Histogram Inspection Spec NG ROI:{0:D}", iHitogramROI + 1); // 실제로 Mark를 못찾는지 확인하는 Log 뿌려줌 - cyh
                                    LogdataDisplay(LogMsg, true);
                                    bRes = false;
                                }
                            }
                        }
                        nRoi = i;
                        m_ROYTpe = (enumROIType)PAT[m_PatTagNo, 0].m_InspParameter[i].m_enumROIType;
                        IDistgnore = PAT[m_PatTagNo, 0].m_InspParameter[i].IDistgnore;
                        dSpec = PAT[m_PatTagNo, 0].m_InspParameter[i].dSpecDistance;
                        m_DistIgnoreCnt = 0;
                        if (m_ROYTpe == enumROIType.FindLine)
                        {
                            double[] Result;

                            CogFindLineTool InpFindLine = new CogFindLineTool();
                            InpFindLine = PAT[m_PatTagNo, 0].m_InspParameter[i].m_FindLineTool;

                            int ignore = 0;
                            bROIRes[i] = GaloOppositeInspection(i, (int)enumROIType.FindLine, InpFindLine, PAT[m_PatTagNo, 0].FixtureImage, out Result, out ignore, ref PAT[m_PatTagNo, 0].resultGraphics);
                            if (bROIRes[i] == true)
                                bROIRes[i] = InspResultData(Result, PAT[m_PatTagNo, 0].m_InspParameter[i].dSpecDistance, PAT[m_PatTagNo, 0].m_InspParameter[i].dSpecDistanceMax, PAT[m_PatTagNo, 0].m_InspParameter[i].IDistgnore,
                                    ignore);
                           
                            if (bROIRes[i] == false)
                            {
                                bRes = false;
                                string LogMsg;
                                LogMsg = string.Format("Inspection NG ROI:{0:D}", i); // 실제로 Mark를 못찾는지 확인하는 Log 뿌려줌 - cyh
                                LogdataDisplay(LogMsg, true);
                                CogRectangleAffine CogNGRectAffine = new CogRectangleAffine();
                                double dCenterX = InpFindLine.RunParams.ExpectedLineSegment.MidpointX;
                                double dCenterY = InpFindLine.RunParams.ExpectedLineSegment.MidpointY;
                                double dAngle = InpFindLine.RunParams.ExpectedLineSegment.Rotation;
                                double dLenth = InpFindLine.RunParams.ExpectedLineSegment.Length;
                                CogNGRectAffine.Color = CogColorConstants.Red;
                                CogNGRectAffine.CenterX = dCenterX;
                                CogNGRectAffine.CenterY = dCenterY;
                                CogNGRectAffine.SideXLength = dLenth;
                                CogNGRectAffine.SideYLength = 100;
                                CogNGRectAffine.Rotation = dAngle;
                                PAT[m_PatTagNo, 0].resultGraphics.Add(CogNGRectAffine);
                                continue;
                            }

                        }
                        else
                        {
                            double[] Result;

                            CogFindCircleTool InspFindCircle = new CogFindCircleTool();
                            InspFindCircle = PAT[m_PatTagNo, 0].m_InspParameter[i].m_FindCircleTool;
                            int ignorecnt = 0;
                            bROIRes[i] = GaloOppositeInspection(i, (int)enumROIType.FindCircle, InspFindCircle, PAT[m_PatTagNo, 0].FixtureImage, out Result, out ignorecnt, ref PAT[m_PatTagNo, 0].resultGraphics);

                            if (bROIRes[i] == true)
                                bROIRes[i] = InspResultData(Result, PAT[m_PatTagNo, 0].m_InspParameter[i].dSpecDistance, PAT[m_PatTagNo, 0].m_InspParameter[i].dSpecDistanceMax, PAT[m_PatTagNo, 0].m_InspParameter[i].IDistgnore,
                                     ignorecnt);


                            if (bROIRes[i] == false)
                            {
                                bROIRes[i] = false;
                                bRes = false;
                                string LogMsg;
                                LogMsg = string.Format("Inspection NG ROI:{0:D}", i); // 실제로 Mark를 못찾는지 확인하는 Log 뿌려줌 - cyh
                                LogdataDisplay(LogMsg, true);
                                double dStartX = InspFindCircle.RunParams.ExpectedCircularArc.StartX;
                                double dStartY = InspFindCircle.RunParams.ExpectedCircularArc.StartY;
                                double dEndX = InspFindCircle.RunParams.ExpectedCircularArc.EndX;
                                double dEndY = InspFindCircle.RunParams.ExpectedCircularArc.EndY;
                                CogFindLineTool cogTempLine = new CogFindLineTool();
                                cogTempLine.RunParams.ExpectedLineSegment.StartX = dStartX;
                                cogTempLine.RunParams.ExpectedLineSegment.StartY = dStartY;
                                cogTempLine.RunParams.ExpectedLineSegment.EndX = dEndX;
                                cogTempLine.RunParams.ExpectedLineSegment.EndY = dEndY;

                                CogRectangleAffine CogNGRectAffine = new CogRectangleAffine();
                                CogNGRectAffine.Color = CogColorConstants.Red;
                                CogNGRectAffine.CenterX = cogTempLine.RunParams.ExpectedLineSegment.MidpointX;
                                CogNGRectAffine.CenterY = cogTempLine.RunParams.ExpectedLineSegment.MidpointY;
                                CogNGRectAffine.SideXLength = cogTempLine.RunParams.ExpectedLineSegment.Length;
                                CogNGRectAffine.SideYLength = 100;
                                CogNGRectAffine.Rotation = cogTempLine.RunParams.ExpectedLineSegment.Rotation;
                                PAT[m_PatTagNo, 0].resultGraphics.Add(CogNGRectAffine);
                                //InspFindCircle.RunParams.ExpectedCircularArc.CenterX = TenmpCenterX;
                                //InspFindCircle.RunParams.ExpectedCircularArc.CenterY = TenmpCenterY; //data가 누적되서 
                                continue;
                            }
                        }
                    };

                    //PAT[m_PatTagNo, 0].bResult = bRes;
                    PAT[m_PatTagNo, 0].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                    return bRes;
                }
                catch (Exception err)
                {
                    PAT[m_PatTagNo, 0].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                    string LogMsg;
                    LogMsg = "Inspeciton Excetion NG Type:" + m_ROYTpe.ToString() + " " + "ROI No:" + nRoi.ToString() + "CaliperIndex:" + jCaliperIndex.ToString();
                    LogdataDisplay(LogMsg, true);
                    LogMsg = "Inspeciton Excetion NG"; LogMsg += "Error : " + err.ToString();
                    LogdataDisplay(LogMsg, true);


                    return false;
                }
            }

            public bool ROIFinealign(CogSearchMaxTool[,] FinealignMark, CogImage8Grey cogImage, out double GapX, out double GapY, out double GapT, ref CogGraphicInteractiveCollection ResultGraphic)
            {
                bool bLeftMarkRet = false;
                bool bRightMarkRet = false;
                bool bTotalResult = false;
                bool nRetSearch_CNL = false;
                double dOriginTheta;
                double dCurrentTheta;
                double dCalX = 0;
                double dCalY = 0;
                double dCalT = 0;
                PointF[] SearchPosition = new PointF[Main.DEFINE.Pattern_Max];
                PointF[] OriginPosition = new PointF[Main.DEFINE.Pattern_Max];
                //ResultGraphic = new CogGraphicInteractiveCollection();
                CogGraphicLabel LabelText = new CogGraphicLabel();

                LabelText.Font = new Font(Main.DEFINE.FontStyle, 15, FontStyle.Bold);
                LabelText.Color = CogColorConstants.Red;

                for (int k = 0; k < Main.DEFINE.PATTERNTAG_MAX; k++)
                {
                    #region Left,Right Mark Search
                    for (int i = 0; i < Main.DEFINE.Pattern_Max; i++)
                    {
                        SearchPosition[i] = new PointF();
                        OriginPosition[i] = new PointF();
                        //Get Main Mark Origin Data 
                        OriginPosition[i].X = (float)FinealignMark[i, 0].Pattern.Origin.TranslationX;
                        OriginPosition[i].Y = (float)FinealignMark[i, 0].Pattern.Origin.TranslationY;

                        for (int j = 0; j < Main.DEFINE.SUBPATTERNMAX; j++)
                        {
                            FinealignMark[i, j].InputImage = cogImage;
                            FinealignMark[i, j].Run();

                            nRetSearch_CNL = false;
                            if (FinealignMark[i, j].Results != null)
                            {
                                if (FinealignMark[i, j].Results.Count >= 1)
                                    nRetSearch_CNL = true;

                            }
                            else
                            {
                                continue;
                            }

                            if (nRetSearch_CNL)
                            {
                                //SearchPosition[i].X = (float)FinealignMark[i, j].Results[0].GetPose().TranslationX;
                                //SearchPosition[i].Y = (float)FinealignMark[i, j].Results[0].GetPose().TranslationY;

                                //ResultGraphic.Add(FinealignMark[i, j].Results[0].CreateResultGraphics(Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.MatchRegion | Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.Origin));

                                if (FinealignMark[i, j].Results[0].Score < PAT[k, 0].m_FinealignMarkScore)
                                {
                                    //if (i == 0)//Left
                                    //{
                                    //    SearchPosition[i].X = 0;
                                    //    SearchPosition[i].Y = 0;
                                    //    if (Main.DEFINE.UNIT_TYPE == "VENT")
                                    //    {
                                    //        LabelText.X = 2000;
                                    //        LabelText.Y = 1000;
                                    //    }
                                    //    else if (Main.DEFINE.UNIT_TYPE == "PATH")
                                    //    {
                                    //        LabelText.X = 1800;
                                    //        LabelText.Y = 200;
                                    //    }
                                    //    LabelText.Text = "[Mark Score]Finealign Left Mark Search Fail";
                                    //    ResultGraphic.Add(LabelText);

                                    //}
                                    //else//Right
                                    //{
                                    //    SearchPosition[i].X = 0;
                                    //    SearchPosition[i].Y = 0;
                                    //    if (Main.DEFINE.UNIT_TYPE == "VENT")
                                    //    {
                                    //        LabelText.X = 2000;
                                    //        LabelText.Y = 1000;
                                    //    }
                                    //    else if (Main.DEFINE.UNIT_TYPE == "PATH")
                                    //    {
                                    //        LabelText.X = 1800;
                                    //        LabelText.Y = 400;
                                    //    }
                                    //    LabelText.Text = "[Mark Score]Finealign Right Mark Search Fail";
                                    //    ResultGraphic.Add(LabelText);
                                    //}
                                    continue;
                                }
                                else
                                {
                                    if (i == 0)
                                        bLeftMarkRet = true;
                                    else
                                        bRightMarkRet = true;

                                    SearchPosition[i].X = (float)FinealignMark[i, j].Results[0].GetPose().TranslationX;
                                    SearchPosition[i].Y = (float)FinealignMark[i, j].Results[0].GetPose().TranslationY;

                                    ResultGraphic.Add(FinealignMark[i, j].Results[0].CreateResultGraphics(Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.MatchRegion | Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.Origin));
                                    break;
                                }

                            }
                        }
                    }
                    #endregion
                }
                #region Calculate
                if(bLeftMarkRet == true && bRightMarkRet == true)//Left, Right Mark Search OK
                {
                    dCalX = SearchPosition[0].X;
                    dCalY = SearchPosition[0].Y;
                    //Theta
                    dOriginTheta = Math.Atan2((OriginPosition[1].Y - OriginPosition[0].Y), (OriginPosition[1].X - OriginPosition[0].X));
                    dCurrentTheta = Math.Atan2((SearchPosition[1].Y - SearchPosition[0].Y), (SearchPosition[1].X - SearchPosition[0].X));

                    dCalT = dCurrentTheta - dOriginTheta;
                    bTotalResult = true;
                }
    
                else //Mark Search Fail시 결과값 0으로 초기화
                {
                    dCalX = 0;
                    dCalY = 0;
                    dCalT = 0;
                    string LogMsg;
                    if (bLeftMarkRet == false)//Left
                    {
                        if (Main.DEFINE.UNIT_TYPE == "VENT")
                        {
                            if (Main.ProjectInfo == "_1WELL_VENT")
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                            else  //3,4Well
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                        }
                        else if (Main.DEFINE.UNIT_TYPE == "PATH")
                        {
                            if (Main.ProjectInfo == "_1WELL_PATH")
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                            else
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                        }
                        LabelText.Text = "Up Mark(Theta) Search Fail";
                        m_bThetaMarkNGFlag = true;
                        LogMsg = "Theta Align NG : Up Mark Search Fail";
                        LogdataDisplay(LogMsg, true);
                        ResultGraphic.Add(LabelText);

                    }
                    if (bRightMarkRet == false)//Right
                    {
                        if (Main.DEFINE.UNIT_TYPE == "VENT")
                        {
                            if (Main.ProjectInfo == "_1WELL_VENT")
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                            else
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                        }
                        else if (Main.DEFINE.UNIT_TYPE == "PATH")
                        {
                            if (Main.ProjectInfo == "_1WELL_PATH")
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                            else
                            {
                                LabelText.X = 1500;
                                LabelText.Y = 150;
                            }
                        }
                        LabelText.Text = "Down Mark(Theta) Search Fail";
                        m_bThetaMarkNGFlag = true;
                        LogMsg = "Theta Align NG : Down Mark Search Fail";
                        LogdataDisplay(LogMsg, true);
                        ResultGraphic.Add(LabelText);
                    }

                    bTotalResult = false;

                }
                #endregion


                GapX = dCalX;
                GapY = dCalY;
                GapT = dCalT;
                return bTotalResult;
            }

            public void Position_Calculate(double CurX, double CurY, double CurLength, double radian, out double ResultX, out double ResultY)
            {
                ResultX = CurX + (CurLength * Math.Sin(radian));
                ResultY = CurY + (CurLength * Math.Cos(radian));
            }

            private bool FinalTracking(double TranslationX, double TranslationY)
            {
                //매개변수로 이미 트래킹값 X Y가 들어가 있음.
                bool bRes = true;
                double dGapx = new double();
                double dGapy = new double();
                double dGapT = new double();
                double dGapT_degree = new double();
                //ROIFinealign 사용 시 기존 FixureImage 미사용(Film Size 측정용도만 사용함)
                if (PAT[m_PatTagNo, 0].m_bFInealignFlag)
                {
                    bRes = TrackingROI(TranslationX, TranslationY);
                    if (!bRes)
                    {
                        return bRes;
                    }
                    bRes = ROIFinealign(PAT[m_PatTagNo, 0].m_FinealignMark, PAT[m_PatTagNo, 0].TempImage, out dGapx, out dGapy, out dGapT, ref PAT[m_PatTagNo, 0].resultGraphics);
                    if (!bRes)
                    {
                        return bRes;
                    }
                    //2023 0228 YSH Finealign Spec 부분 추후 필요 시 수정 요망
                    dGapT_degree = dGapT * 180 / Math.PI;
                    if (-PAT[m_PatTagNo, 0].m_FinealignThetaSpec < dGapT_degree && dGapT_degree < PAT[m_PatTagNo, 0].m_FinealignThetaSpec)//Spec Check
                    {
                        CogFixtureTool mCogFixtureTool = new CogFixtureTool();
                        mCogFixtureTool.InputImage = PAT[m_PatTagNo, 0].TempImage;
                        CogTransform2DLinear TempData = new CogTransform2DLinear(); 
                        TempData.TranslationX = dGapx;
                        TempData.TranslationY = dGapy;
                        TempData.Rotation = dGapT;
                        PAT[m_PatTagNo, 0].m_FinealignGapT = dGapT;
                        //Console.WriteLine(TempData.TranslationX.ToString() + "    " + TempData.TranslationY);
                        Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].TempFixtureTrans = TempData;
                        mCogFixtureTool.RunParams.UnfixturedFromFixturedTransform = TempData;
                        mCogFixtureTool.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
                        mCogFixtureTool.Run();
                        PAT[m_PatTagNo, 0].FixtureImage = (CogImage8Grey)mCogFixtureTool.OutputImage;
                        string LogMsg = string.Format("Theta Align OK : {0:F2}(°) / Spec : ±{1:F2}(°)", dGapT_degree, PAT[m_PatTagNo, 0].m_FinealignThetaSpec);
                        LogdataDisplay(LogMsg, true);
                    }
                    else
                    {
                        PAT[m_PatTagNo, 0].m_FinealignGapT = dGapT;
                        string LogMsg = string.Format("Theta Align NG : {0:F2}(°) / Spec : ±{1:F2}(°)", dGapT_degree, PAT[m_PatTagNo, 0].m_FinealignThetaSpec);
                        LogdataDisplay(LogMsg, true);
                        m_bThetaNGFlag = true;
                        bRes = false;
                    }
                }
                else//ROIFinealign 미사용 시 BondingAlign 사용
                {
                    bRes = BondingAlignInspectionTest(TranslationX, TranslationY, out dGapx, out dGapy);
                    CogFixtureTool mCogFixtureTool = new CogFixtureTool();
                    mCogFixtureTool.InputImage = PAT[m_PatTagNo, 0].FixtureImage;
                    CogTransform2DLinear TempData = new CogTransform2DLinear();
                    if (Main.DEFINE.UNIT_TYPE == "VENT")
                    {
                        TempData.TranslationX = dGapx;
                        TempData.TranslationY = dGapy;
                    }
                    if (Main.DEFINE.UNIT_TYPE == "PATH")
                    {
                        TempData.TranslationX = -dGapx;
                        TempData.TranslationY = dGapy;
                    }

                    mCogFixtureTool.RunParams.UnfixturedFromFixturedTransform = TempData;
                    mCogFixtureTool.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
                    mCogFixtureTool.Run();
                    PAT[m_PatTagNo, 0].FixtureImage = (CogImage8Grey)mCogFixtureTool.OutputImage;
                }
                return bRes;
            }
            private bool BondingAlignInspectionTest(double TranslationX, double TranslationY, out double dFinalTrackingX, out double dFinalTrackingY)
            {
                double dPixelResolution = 13.36;
                bool bRes = true;
                CogGraphicLabel LabelTest = new CogGraphicLabel();
                LabelTest.Font = new Font(Main.DEFINE.FontStyle, 15, FontStyle.Bold);

                dFinalTrackingX = new double();
                dFinalTrackingY = new double();


                CogRectangleAffine Rect = new CogRectangleAffine();
                // CogRectangleAffine Rect = (CogRectangleAffine)m_CogHistogramTool[m_HistoROI].Region;
                //double CenterX = Rect.CenterX;
                //double CenterY = Rect.CenterY;

                // if (_useROITracking)
                //{
                bRes = TrackingROI(TranslationX, TranslationY);
                if (bRes == false)
                {
                    bRes = false;
                    return bRes;
                }
                // m_CogHistogramTool[m_HistoROI].InputImage = PT_Display01.Image;
                //Rect = (CogRectangleAffine)m_CogHistogramTool[m_HistoROI].Region;

                // }
                //else
                //Rect = (CogRectangleAffine)m_CogHistogramTool[m_HistoROI].Region;

                for (int i = 0; i < 4; i++)
                {

                    Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].m_BondingAlignLine[i].InputImage = PAT[m_PatTagNo, 0].FixtureImage;

                    Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].m_BondingAlignLine[i].Run();

                    if (Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].m_BondingAlignLine[i].Results != null && Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].m_BondingAlignLine[i].Results.Count > 0)
                    {

                        CogCompositeShape Graph = Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, 0].m_BondingAlignLine[i].Results[0].CreateResultGraphics(CogCaliperResultGraphicConstants.Edges);
                        Graph.Color = CogColorConstants.Orange;
                        PAT[m_PatTagNo, 0].resultGraphics.Add(Graph);
                    }
                    else
                    {
                        bRes = false;
                        dFinalTrackingX = 0;
                        dFinalTrackingY = 0;
                        return bRes;
                    }
                }

                //Calculation Bonding Align X,Y
                double dBonding_AlignX = 0;
                double dBonding_AlignY = 0;

                dBonding_AlignX = Math.Abs(PAT[m_PatTagNo, 0].m_BondingAlignLine[1].Results[0].Edge0.PositionX - PAT[m_PatTagNo, 0].m_BondingAlignLine[0].Results[0].Edge0.PositionX);
                dBonding_AlignY = Math.Abs(PAT[m_PatTagNo, 0].m_BondingAlignLine[3].Results[0].Edge0.PositionY - PAT[m_PatTagNo, 0].m_BondingAlignLine[2].Results[0].Edge0.PositionY);

                dBonding_AlignX = dBonding_AlignX * dPixelResolution / 1000;
                dBonding_AlignY = dBonding_AlignY * dPixelResolution / 1000;

                dBonding_AlignX = PAT[m_PatTagNo, 0].m_dOriginDistanceX - dBonding_AlignX;
                dBonding_AlignY = PAT[m_PatTagNo, 0].m_dOriginDistanceY - dBonding_AlignY;
                dFinalTrackingX = dBonding_AlignX / dPixelResolution * 1000;
                dFinalTrackingY = dBonding_AlignY / dPixelResolution * 1000;

                Console.WriteLine(dBonding_AlignX);
                Console.WriteLine(dBonding_AlignY);

                double dCheckDistX, dCheckDistY;
                dCheckDistX = Math.Abs(dFinalTrackingX);
                dCheckDistY = Math.Abs(dFinalTrackingY);
                dCheckDistX = dCheckDistX * dPixelResolution / 1000;
                dCheckDistY = dCheckDistY * dPixelResolution / 1000;


                //Overlay 추가
                if (PAT[m_PatTagNo, 0].m_dDistanceSpecX > dCheckDistX && PAT[m_PatTagNo, 0].m_dDistanceSpecY > dCheckDistY)   // OK
                {
                    ///ksh overlay 위치변경
                    LabelTest.X = 100;
                    LabelTest.Y = 100;
                    LabelTest.Text = string.Format("OK  X: {0:F3} Y: {1:F3}", dCheckDistX, dCheckDistY);
                    LabelTest.Color = CogColorConstants.Green;
                    // LabelTest.Text = string.Format("Left Position X:{0:F3}, Y:{1:F3}", dCrossX[i], dCrossY[i]);
                }
                else   // NG
                {
                    bRes = false;
                    if (PAT[m_PatTagNo, 0].m_dDistanceSpecX < dCheckDistX)
                    {

                        LabelTest.X = 700;
                        LabelTest.Y = 2900;
                        LabelTest.Text = string.Format("Align NG  X: {0:F2}", dCheckDistX);
                        LabelTest.Color = CogColorConstants.Red;
                        //CogRectangle resultRect = new CogRectangle();
                        //resultRect.X = PAT[m_PatTagNo, 0].m_BondingAlignLine[0].Results[0].Edge0.PositionX;
                        //resultRect.Y = PAT[m_PatTagNo, 0].m_BondingAlignLine[0].Results[0].Edge0.PositionY - 150;
                        //resultRect.Width = Math.Abs(PAT[m_PatTagNo, 0].m_BondingAlignLine[1].Results[0].Edge0.PositionX - PAT[m_PatTagNo, 0].m_BondingAlignLine[0].Results[0].Edge0.PositionX);
                        //resultRect.Height = 300;
                        //resultRect.Color = CogColorConstants.Red;
                        //PAT[m_PatTagNo, 0].resultGraphics.Add(resultRect);
                        string LogMsg = string.Format("Bonding Align X NG - Spec: {0:F3}, X: {1:F3}", PAT[m_PatTagNo, 0].m_dDistanceSpecX, dCheckDistX);
                        LogdataDisplay(LogMsg, true);
                    }
                    if (PAT[m_PatTagNo, 0].m_dDistanceSpecY < dCheckDistY)
                    {
                        LabelTest.X = 1000;
                        LabelTest.Y = 3100;
                        LabelTest.Text = string.Format("Align NG  Y: {0:F2}", dCheckDistY);
                        LabelTest.Color = CogColorConstants.Red;
                        //CogRectangle resultRect = new CogRectangle();
                        //resultRect.X = PAT[m_PatTagNo, 0].m_BondingAlignLine[0].Results[0].Edge0.PositionX;
                        //resultRect.Y = PAT[m_PatTagNo, 0].m_BondingAlignLine[0].Results[0].Edge0.PositionY;
                        //resultRect.Width = 100;
                        //resultRect.Height = Math.Abs(PAT[m_PatTagNo, 0].m_BondingAlignLine[3].Results[0].Edge0.PositionY - PAT[m_PatTagNo, 0].m_BondingAlignLine[2].Results[0].Edge0.PositionY);
                        //resultRect.Color = CogColorConstants.Red;
                        //PAT[m_PatTagNo, 0].resultGraphics.Add(resultRect);
                        string LogMsg = string.Format("Bonding Align Y NG - Spec: {0:F3}, Y: {1:F3}", PAT[m_PatTagNo, 0].m_dDistanceSpecY, dCheckDistY);
                        LogdataDisplay(LogMsg, true);
                    }


                    //LabelTest.Text = string.Format("Left Position X:{0:F3}, Y:{1:F3}", dCrossX[i], dCrossY[i]);

                    PAT[m_PatTagNo, 0].resultGraphics.Add(LabelTest);

                }



                return bRes;
            }
            private bool TrackingROI(double dTransX, double dTransY)
            {
                double ROIX = 0, ROIY = 0, RotT = 0;
                bool Res = false;

                try
                {
                    if (LineTrakingROI(dTransX, dTransY, ref ROIX, ref ROIY, ref RotT))
                    {
                        Res = true;
                        if (!PAT[m_PatTagNo, 0].m_bFInealignFlag)
                        {
                            //double dTrackingX = 0, dTrackingY = 0, dROT = 0, dFinalTrackingX = 0, dFinalTrackingY = 0;
                            //TrackingROICalculate(dCrossX, dCrossY, out dTrackingX, out dTrackingY, out dROT);
                            CogFixtureTool _CogFixtureTool = new CogFixtureTool();
                            _CogFixtureTool.InputImage = PAT[m_PatTagNo, 0].TempImage;
                            CogTransform2DLinear TempData = new CogTransform2DLinear();
                            TempData.TranslationX = PAT[m_PatTagNo, 0].PMAlignResult.m_Pixel[0];//현재 찾은 마크 좌표X
                            TempData.TranslationY = PAT[m_PatTagNo, 0].PMAlignResult.m_Pixel[1];//현재 찾은 마크 좌표Y
                            TempData.Rotation = RotT;
                            _CogFixtureTool.RunParams.UnfixturedFromFixturedTransform = TempData;
                            _CogFixtureTool.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
                            _CogFixtureTool.Run();
                            PAT[m_PatTagNo, 0].FixtureImage = (CogImage8Grey)_CogFixtureTool.OutputImage;
                            // BondingAlignInspectionTest(dTrackingX, dTrackingY, dROT, out dFinalTrackingX, out dFinalTrackingY);     //shkang
                        }

                    }
                    else
                    {
                        Res = false;
                    }
                }
                catch
                {
                    Res = false;
                    return Res;
                }
                return Res;
            }
            private void TrackingROICalculate(double[] dCrossX, double[] dCrossY, out double TrackingX, out double TrackingY, out double RotT)
            {
                double dx, dy, dTeachT, dRotT;
                dx = (dCrossX[1] + dCrossX[0]) / 2 - (PAT[m_PatTagNo, 0].RightOrigin[0] + PAT[m_PatTagNo, 0].LeftOrigin[0]) / 2;
                dy = (dCrossY[1] + dCrossY[0]) / 2 - (PAT[m_PatTagNo, 0].RightOrigin[1] + PAT[m_PatTagNo, 0].LeftOrigin[1]) / 2;
                double[] pntCenter = new double[2] { 0, 0 };
                double dRotDx = dCrossX[1] - dCrossX[0];
                double dRotDy = dCrossY[1] - dCrossY[0];
                dRotT = Math.Atan2(dRotDy, dRotDx);
                if (dRotT > 180.0) dRotT -= 360.0;

                dTeachT = Math.Atan2(PAT[m_PatTagNo, 0].RightOrigin[1] - PAT[m_PatTagNo, 0].LeftOrigin[1], PAT[m_PatTagNo, 0].RightOrigin[0] - PAT[m_PatTagNo, 0].LeftOrigin[0]);
                if (dTeachT > 180.0) dTeachT -= 360.0;

                dRotT -= dTeachT;
                RotT = dRotT;
                pntCenter[0] = (dCrossX[1] + dCrossX[0]) / 2;
                pntCenter[1] = (dCrossY[1] + dCrossY[0]) / 2;
                double[] dTaget = new double[2];
                dTaget[0] = (dCrossX[1] + dCrossX[0]) / 2;
                dTaget[1] = (dCrossY[1] + dCrossY[0]) / 2;
                //RotationTransform(pntCenter, dx, dy, RotT, ref dTaget);
                TrackingX = dTaget[0];
                TrackingY = dTaget[1];
            }
            private bool LineTrakingROI(double dTransX, double dTransY, ref double ROIX, ref double ROIY, ref double RotT)
            {
                bool Res = false;
                try
                {
                    //CogGraphicInteractiveCollection resultGraphics = new CogGraphicInteractiveCollection();
                    double dInspectionDistanceX = 0;
                    double[] dx = new double[4];
                    double[] dy = new double[4];
                    double[] dCrossX = new double[2];
                    double[] dCrossY = new double[2];
                    double[] dAngle = new double[4];
                    bool bSearchRes = PAT[m_PatTagNo, 0].Search();
                    if (bSearchRes == true)
                    {
                        double TranslationX = dTransX;
                        double TranslationY = dTransY;
                        CogIntersectLineLineTool[] CrossPoint = new CogIntersectLineLineTool[2];
                        CogLine[] Line = new CogLine[4];
                        for (int i = 0; i < 4; i++)
                        {
                            if (i < 2)
                            {
                                CrossPoint[i] = new CogIntersectLineLineTool();
                                CrossPoint[i].InputImage = PAT[m_PatTagNo, 0].TempImage;
                            }
                            Line[i] = new CogLine();
                            PAT[m_PatTagNo, 0].m_TrackingLine[i].InputImage = PAT[m_PatTagNo, 0].TempImage;
                            double TempStartA_X =  PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX;
                            double TempStartA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY;
                            double TempEndA_X =  PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX;
                            double TempEndA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY;


                            double StartA_X =  PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX - TranslationX;
                            double StartA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY - TranslationY;
                            double EndA_X =  PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX - TranslationX;
                            double EndA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY - TranslationY;

                            PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX = StartA_X;
                            PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY = StartA_Y;
                            PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX = EndA_X;
                            PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY = EndA_Y;

                            PAT[m_PatTagNo, 0].m_TrackingLine[i].Run();

                            if (PAT[m_PatTagNo, 0].m_TrackingLine[i].Results != null)
                            {
                                Line[i] = PAT[m_PatTagNo, 0].m_TrackingLine[i].Results.GetLine();
                                //shkang_
                                if (Line[i] == null)
                                    Line[i] = new CogLine();

                                if (i < 2)
                                    Line[i].Color = CogColorConstants.Blue;
                                else
                                    Line[i].Color = CogColorConstants.Orange;
                                PAT[m_PatTagNo, 0].resultGraphics.Add(Line[i]);
                            }

                             PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX = TempStartA_X;
                             PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY = TempStartA_Y;
                             PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX = TempEndA_X;
                            PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY = TempEndA_Y;
                        }
                        //shkang_s 
                        //필름 밀림 융착 검사 (자재 X거리 검출)
                        CogGraphicLabel LabelTest = new CogGraphicLabel();
                        LabelTest.Font = new Font(Main.DEFINE.FontStyle, 20, FontStyle.Bold);
                        LabelTest.Color = CogColorConstants.Green;
                        double dPixelResolution = 13.36;
                        dInspectionDistanceX = Line[3].X - Line[1].X;   //X 거리 검출
                        dInspectionDistanceX = dInspectionDistanceX * dPixelResolution / 1000;
                        if (PAT[m_PatTagNo, 0].m_dObjectDistanceX + PAT[m_PatTagNo, 0].m_dObjectDistanceSpecX <= dInspectionDistanceX) //NG - Film NG
                        {
                            m_bFilmNGFlag = true;
                            LabelTest.X = 1000;
                            LabelTest.Y = 180;
                            LabelTest.Color = CogColorConstants.Red;
                            LabelTest.Text = string.Format("Film NG, X:{0:F3}", dInspectionDistanceX);
                            string LogMsg = string.Format("Film Align NG, X : {0:F2}(mm) / Spec : {1:F2} + {2:F2}", dInspectionDistanceX, PAT[m_PatTagNo, 0].m_dObjectDistanceX, PAT[m_PatTagNo, 0].m_dObjectDistanceSpecX);
                            LogdataDisplay(LogMsg, true);
                            return false;

                        }
                        else   //OK - Film OK
                        {
                            LabelTest.X = 1000;
                            LabelTest.Y = 180;
                            LabelTest.Color = CogColorConstants.Green;
                            LabelTest.Text = string.Format("Film OK, X:{0:F3}", dInspectionDistanceX);
                            string LogMsg = string.Format("Film Align OK, X : {0:F2}(mm) / Spec : {1:F2} + {2:F2}", dInspectionDistanceX, PAT[m_PatTagNo, 0].m_dObjectDistanceX, PAT[m_PatTagNo, 0].m_dObjectDistanceSpecX);
                            LogdataDisplay(LogMsg, true);
                        }
                        //resultGraphics.Add(LabelTest);
                        //shkang_e
                        CrossPoint[0].LineA = Line[0];
                        CrossPoint[0].LineB = Line[1];
                        CrossPoint[1].LineA = Line[2];
                        CrossPoint[1].LineB = Line[3];
                        for (int i = 0; i < 2; i++)
                        {
                            CogGraphicLabel ThetaLabelTest = new CogGraphicLabel();
                            ThetaLabelTest.Font = new Font(Main.DEFINE.FontStyle, 15, FontStyle.Bold);
                            ThetaLabelTest.Color = CogColorConstants.Green;
                            if (Line[0] == null || Line[1] == null) return false;
                            if (Line[2] == null || Line[3] == null) return false;
                            CrossPoint[i].Run();
                            if (CrossPoint[i] != null)
                            {
                                dCrossX[i] = CrossPoint[i].X;
                                dCrossY[i] = CrossPoint[i].Y;
                                dAngle[i] = CrossPoint[i].Angle;
                                CogPointMarker PointMark = new CogPointMarker();
                                PointMark.Color = CogColorConstants.Green;
                                PointMark.SizeInScreenPixels = 50;
                                PointMark.X = CrossPoint[i].X;
                                PointMark.Y = CrossPoint[i].Y;
                                PointMark.Rotation = dAngle[i];
                                //PAT[m_PatTagNo, 0].resultGraphics.Add(PointMark);
                                if (i == 0)
                                {
                                    ThetaLabelTest.X = 350;
                                    ThetaLabelTest.Y = 100;
                                    ThetaLabelTest.Text = string.Format("Left Position X:{0:F3}, Y:{1:F3}", dCrossX[i], dCrossY[i]);
                                }
                                else
                                {
                                    ThetaLabelTest.X = 350;
                                    ThetaLabelTest.Y = 250;
                                    ThetaLabelTest.Text = string.Format("Right Position X:{0:F3}, Y:{1:F3}", dCrossX[i], dCrossY[i]);
                                }
                                //PAT[m_PatTagNo, 0].resultGraphics.Add(ThetaLabelTest);
                            }
                        }
                        //PT_Display01.InteractiveGraphics.AddList(resultGraphics, "RESULT", false);
                    }
                    double TrackingX, TrackingY, dRotT;
                    TrackingROICalculate(dCrossX, dCrossY, out TrackingX, out TrackingY, out dRotT);
                    RotT = dRotT;
                    ROIX = TrackingX;
                    ROIY = TrackingY;
                    Res = true;
                }
                catch
                {
                    Res = false;
                    return Res;
                }


                return Res;
                //bool bRes = false;
                //try
                //{
                //    CogLine[] Line = new CogLine[4];
                //    CogIntersectLineLineTool[] CrossPoint = new CogIntersectLineLineTool[2];
                //    for (int i = 0; i < 4; i++)
                //    {
                //        CogFindLineTool _LineTool = new CogFindLineTool();
                //        _LineTool = PAT[m_PatTagNo, 0].m_TrackingLine[i];
                //        if (i < 2)
                //        {
                //            CrossPoint[i] = new CogIntersectLineLineTool();
                //            CrossPoint[i].InputImage = PAT[m_PatTagNo, 0].TempImage;
                //        }
                //        Line[i] = new CogLine();
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].InputImage = PAT[m_PatTagNo, 0].TempImage;
                //        double TempStartA_X = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX;
                //        double TempStartA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY;
                //        double TempEndA_X = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX;
                //        double TempEndA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY;

                //        double StartA_X = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX - dTransX;
                //        double StartA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY - dTransY;
                //        double EndA_X = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX - dTransX;
                //        double EndA_Y = PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY - dTransY;

                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX = StartA_X;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY = StartA_Y;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX = EndA_X;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY = EndA_Y;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].Run();

                //        if (PAT[m_PatTagNo, 0].m_TrackingLine[i].Results.GetLine() != null)
                //        {
                //            Line[i] = PAT[m_PatTagNo, 0].m_TrackingLine[i].Results.GetLine();
                //            if (i < 2)
                //                Line[i].Color = CogColorConstants.Blue;
                //            else
                //                Line[i].Color = CogColorConstants.Orange;
                //            PAT[m_PatTagNo, 0].resultGraphics.Add(Line[i]);
                //        }
                //        else
                //        {
                //            bRes = false;
                //            return bRes;
                //        }
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartX = TempStartA_X;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.StartY = TempStartA_Y;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndX = TempEndA_X;
                //        PAT[m_PatTagNo, 0].m_TrackingLine[i].RunParams.ExpectedLineSegment.EndY = TempEndA_Y;
                //    }
                //    CrossPoint[0].LineA = Line[0];
                //    CrossPoint[0].LineB = Line[1];
                //    CrossPoint[1].LineA = Line[2];
                //    CrossPoint[1].LineB = Line[3];
                //    for (int i = 0; i < 2; i++)
                //    {
                //        CogGraphicLabel LabelTest = new CogGraphicLabel();
                //        LabelTest.Font = new Font(Main.DEFINE.FontStyle, 20, FontStyle.Bold);
                //        LabelTest.Color = CogColorConstants.Green;

                //        CrossPoint[i].Run();
                //        if (CrossPoint[i] != null)
                //        {
                //            dCrossX[i] = CrossPoint[i].X;
                //            dCrossY[i] = CrossPoint[i].Y;
                //            if (i == 0)
                //            {
                //                LabelTest.X = 100;
                //                LabelTest.Y = 100;
                //                LabelTest.Text = string.Format("Left Position X:{0:F3}, Y:{1:F3}", dCrossX[i], dCrossY[i]);
                //            }
                //            else
                //            {
                //                LabelTest.X = 100;
                //                LabelTest.Y = 200;
                //                LabelTest.Text = string.Format("Right Position X:{0:F3}, Y:{1:F3}", dCrossX[i], dCrossY[i]);
                //            }
                //            //shkang
                //            //PAT[m_PatTagNo, 0].resultGraphics.Add(LabelTest);
                //        }
                //    }
                //    bRes = true;
                //}
                //catch
                //{
                //    bRes = false;
                //    return bRes;
                //}
                //return bRes;
            }
            
            private bool InspResultData(double[] Dist, double SpecMin, double SpecMax, int SpecIgnore, int CurrentIgnor) // cyh - 매뉴얼시 데이터 나오는곳
            {
                bool Res = true;
                CurrentIgnor = 0;
                m_DistIgnoreCnt = CurrentIgnor;
                for (int i = 0; i < Dist.Length; i++)
                {
                    if (Dist[i] > SpecMin && Dist[i] < SpecMax)
                    {
                    }
                    else
                    {
                        m_DistIgnoreCnt++;
                        if (SpecIgnore < m_DistIgnoreCnt)
                        {
                            Res = false;
                            return Res;
                        }
                        else
                            continue;
                    }
                }

                return Res;
            }
            private bool PATSearch(int nType)
            {
                bool ret = false;
                bool[] nRet = new bool[2];
                double[] nDistanceRet = new double[2];
                List<int> nPatNum = new List<int>();
                AddPattern(nType, ref nPatNum);
#region OPEN_TRUE
                if (Main.DEFINE.OPEN_F)
                {
                    //                     for (int i = 0; i < nPatNum.Count; i++)
                    //                     {
                    //                         vision.Mutex_lock_Grab[PAT[m_PatTagNo, nPatNum[i]].m_CamNo].WaitOne();
                    //                         try
                    //                         {
                    //                             Main.vision.CogImgTool[PAT[m_PatTagNo, nPatNum[i]].m_CamNo].Run();
                    //                             vision.CogCamBuf[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] = vision.CogImgTool[PAT[m_PatTagNo, nPatNum[i]].m_CamNo].OutputImage as ICogImage;
                    //                         }
                    //                         catch
                    //                         {
                    // 
                    //                         }
                    //                         finally
                    //                         {
                    //                             vision.Mutex_lock_Grab[PAT[m_PatTagNo, nPatNum[i]].m_CamNo].ReleaseMutex();
                    //                         }
                    //                     }
                }
#endregion

                ret = PATSearch(nPatNum);
                if ((nType == DEFINE.OBJ_ALL || nType == DEFINE.TAR_ALL || nType == DEFINE.OBJTAR_ALL) && ret)
                {
                    ret = LengthCheck(nType);
                }
                if (ret && (m_AlignName == "COF_CUTTING_ALIGN") && machine.LengthCheck_Onf && (nType == DEFINE.OBJ_ALL || nType == DEFINE.OBJ_L || nType == DEFINE.OBJ_R))
                {
                    List<int> nPatNum1 = new List<int>();
                    nPatNum1.Add(DEFINE.OBJ_L);
                    TABLengthCheck(nPatNum);
                }
                if (ret && (m_AlignName == "REEL_ALIGN_1" || m_AlignName == "REEL_ALIGN_2" || m_AlignName == "REEL_ALIGN_3" || m_AlignName == "REEL_ALIGN_4" || m_AlignName == "ART_PROBE"))
                {
                    ReelAlign(nPatNum);
                }


                if (ret && (m_AlignName == "PBD1" || m_AlignName == "PBD_FOF1" || m_AlignName == "PBD2" || m_AlignName == "PBD_FOF2"))
                {
                    bool nCenterAlign_Flag = false;
                    string nStageMode = "";

                    if ((m_AlignName == "PBD1" || m_AlignName == "PBD2") && (nType == DEFINE.TAR_ALL || nType == DEFINE.OBJTAR_ALL))  //stage가 타겟일 경우
                    {
                        nCenterAlign_Flag = true;
                        nStageMode = "TARGET";
                    }
                    //                     if ((m_AlignName == "PBD_FOF1" || m_AlignName == "PBD_FOF2") && (nType == DEFINE.OBJ_ALL || nType == DEFINE.OBJTAR_ALL))    //Stage가 오브젝트일경우
                    //                     {
                    //                         nCenterAlign_Flag = true;
                    //                         nStageMode = "OBJECT";
                    //                     }
                    if (nCenterAlign_Flag) StageCenterAlign(m_AlignName, nStageMode); //Stage Center Align
                }

                //                 if ((m_AlignName == "AI_PRE") && m_Cmd != CMD.ALIGN_1CAM_2SHOT_LEFT && m_Cmd != CMD.ALIGN_1CAM_2SHOT_RIGHT)
                //                 {
                //                     m_StageX = 0;
                //                     m_StageY = (long)PAT[m_PatTagNo, nType].m_RobotPosY * -1;
                //                     /*m_StageT = 0;*/
                //                 }

                if (!m_DrawNotUse) DrawSet(nPatNum, nType);

                return ret;
            }
            private void ReelAlign(List<int> nPatNum)
            {
                for (int i = 0; i < nPatNum.Count; i++)
                {
                    m_StageX = (long)PAT[m_PatTagNo, nPatNum[i]].m_RobotPosX * -1;
                    m_StageY = (long)PAT[m_PatTagNo, nPatNum[i]].m_RobotPosY * -1;
                    m_StageT = 0;
                }
            }
            private bool TABLengthCheck(List<int> nPatNum)
            {

                int seq = 0;
                bool cmd = true;
                bool LoopFlag = true;
                bool[] nRet = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];
                string LogMsg = "";

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_CALIPER);

                                if (!nLightCompare[nPatNum[i]])
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                            }
                            seq++;
                            break;
                        case 1:
                            if (nPatNum.Count > 1)
                            {
                                bool nLight = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!nLightCompare[nPatNum[i]]) nLight = false;
                                }
                                if (!nLight)
                                {
                                    SearchDelay();
                                    GetImage(nPatNum);
                                }
                            }
                            else
                            {
                                if (!nLightCompare[nPatNum[0]])
                                {
                                    SearchDelay();
                                    PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                }
                            }
                            seq++;
                            break;

                        case 2:
                            bool mUseCheck = false;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    for (int ii = 0; ii < Main.DEFINE.CALIPER_MAX; ii++)
                                    {
                                        (PAT[m_PatTagNo, nPatNum[i]].CaliperTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].CaliperPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                        (PAT[m_PatTagNo, nPatNum[i]].CaliperTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].CaliperPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;
                                    }
                                }
                                nRet[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_Caliper();

                                if (nRet[nPatNum[i]])
                                    PAT[m_PatTagNo, nPatNum[i]].CaliperDraw = true;
                                else
                                    cmd = false;
                                for (int j = 0; j < Main.DEFINE.CALIPER_MAX; j++)
                                {
                                    if (PAT[m_PatTagNo, nPatNum[i]].CaliperPara[j].m_UseCheck)
                                        mUseCheck = true;
                                }
                                if (!mUseCheck)
                                {
                                    cmd = false;
                                    nRet[nPatNum[i]] = false;
                                }
                            }
                            seq++;
                            break;

                        case 3:
                            double[,] tempData = new double[DEFINE.Pattern_Max, 2];
                            double[,] tempDataMark = new double[DEFINE.Pattern_Max, 2];
                            long[] tempYLength = new long[nPatNum.Count];
                            long[] tempYLength_ = new long[2];

                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].m_Length[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].m_Length[DEFINE.Y] = 0;
                                if (nRet[nPatNum[i]])
                                {

                                    PAT[m_PatTagNo, nPatNum[i]].V2R(PAT[m_PatTagNo, nPatNum[i]].PixelCaliper[DEFINE.X], PAT[m_PatTagNo, nPatNum[i]].PixelCaliper[DEFINE.Y], ref tempData[nPatNum[i], DEFINE.X], ref tempData[nPatNum[i], DEFINE.Y]);
                                    if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                        PAT[m_PatTagNo, nPatNum[i]].V2R(PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X], PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y], ref tempDataMark[nPatNum[i], DEFINE.X], ref tempDataMark[nPatNum[i], DEFINE.Y]);
                                    else
                                        PAT[m_PatTagNo, nPatNum[i]].V2R(Main.vision.IMAGE_CENTER_X[PAT[m_PatTagNo, nPatNum[i]].m_CamNo], Main.vision.IMAGE_CENTER_Y[PAT[m_PatTagNo, nPatNum[i]].m_CamNo], ref tempDataMark[nPatNum[i], DEFINE.X], ref tempDataMark[nPatNum[i], DEFINE.Y]);

                                    PAT[m_PatTagNo, nPatNum[i]].m_Length[DEFINE.X] = Math.Abs(tempDataMark[nPatNum[i], DEFINE.X] - tempData[nPatNum[i], DEFINE.X]);
                                    PAT[m_PatTagNo, nPatNum[i]].m_Length[DEFINE.Y] = Math.Abs(tempDataMark[nPatNum[i], DEFINE.Y] - tempData[nPatNum[i], DEFINE.Y]);
                                    tempYLength[i] = (long)PAT[m_PatTagNo, nPatNum[i]].m_Length[DEFINE.Y];
                                    InputTABdata(nPatNum[i], tempYLength[i]);
                                    if (nPatNum[i] == 0 || nPatNum[i] == 2)
                                        LogMsg += "LEFT___YLENGTH: ";
                                    else
                                        LogMsg += "RIGHT_YLENGTH: ";

                                    LogMsg += tempYLength[i].ToString("00");
                                    LogMsg += "  ";
                                    tempYLength_[nPatNum[i]] = tempYLength[i];

                                }
                                else
                                {
                                    tempYLength[i] = 0;
                                }

                            }
                            if (LogMsg != "") LogdataDisplay(LogMsg, true);
                            WriteTABLength(tempYLength_);
                            //LOG SAVE 
                            seq++;
                            break;

                        case 4:

                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                }
                            }
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private bool PATSearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[8];
                bool nResearch = false;

                try
                {

                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                seq++;
                                break;

                            case 1:
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                }
                                if (nPatNum.Count == 1 && (nPatNum[0] == DEFINE.OBJ_R || nPatNum[0] == DEFINE.OBJ_L))
                                {
                                    if (!LightUseCheck(DEFINE.OBJ_R))
                                    {
                                        PAT[m_PatTagNo, DEFINE.OBJ_L].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    }
                                    if (!LightUseCheck(DEFINE.OBJ_L))
                                    {
                                        PAT[m_PatTagNo, DEFINE.OBJ_R].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    }
                                }
                                seq = SEQ.GRAB_SEQ;
                                break;



                            case SEQ.GRAB_SEQ:
                                SearchDelay();
                                if (!m_GrabNotUse)
                                {
                                    if (nPatNum.Count == 1)
                                    {
                                        PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                    }
                                    else
                                    {
                                        GetImage(nPatNum);
                                    }
                                }
                                m_GrabNotUse = false;
                                if (m_Cmd == 1018 || m_Cmd == 1019 || m_Cmd == 1020)
                                {
                                    if (m_GD_ImageSave_Use | m_NG_ImageSave_Use)
                                    {
                                        for (int i = 0; i < nPatNum.Count; i++)
                                        {
                                            PAT[m_PatTagNo, nPatNum[i]].Save_ORGImage("GRAB CONFIRM");
                                        }

                                    }
                                    seq = SEQ.COMPLET_SEQ;

                                }
                                else
                                {
                                    seq = SEQ.SEARCH_SEQ;
                                }
                                break;



                            case SEQ.SEARCH_SEQ:
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search();
                                }
                                seq = 5;
                                break;

                            case 5:
                                bool nRet = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!ret[nPatNum[i]])
                                        nRet = false;
                                }
                                if (nRet)
                                {
                                    seq = SEQ.COMPLET_SEQ;
                                }
                                else
                                {
                                    seq = SEQ.RESEARCH_SEQ;
                                }
                                break;

                            case SEQ.RESEARCH_SEQ:
                                if (!nResearch)
                                {
                                    nResearch = true;
                                    seq = SEQ.GRAB_SEQ;
                                }
                                else
                                {
                                    for (int i = 0; i < nPatNum.Count; i++)
                                        if (!ret[nPatNum[i]]) PAT[m_PatTagNo, nPatNum[i]].SearchResult = false; //Manual Set 때문에 false

                                    if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                                    {
                                        seq = SEQ.ERROR_SEQ;
                                    }
                                    else
                                        seq = SEQ.MANUAL_SEQ;
                                }
                                break;

                            case SEQ.MANUAL_SEQ:

                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (PAT[m_PatTagNo, nPatNum[i]].m_Manu_Match_Use)
                                    {
                                        if (!ret[nPatNum[i]]) ret[nPatNum[i]] = Manual_Matching(nPatNum[i]);
                                    }
                                }
                                bool nnRet = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!ret[nPatNum[i]])
                                        nnRet = false;
                                }
                                if (nnRet)
                                    seq = SEQ.COMPLET_SEQ;
                                else
                                    seq = SEQ.ERROR_SEQ;
                                break;

                            case SEQ.ERROR_SEQ:
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];
                                }
                                if (m_Cmd == CMD.ALIGN_OBJTAR_ALL || m_Cmd == CMD.ALIGN_OBJTAR_ALL_REALIGN) //OBJ , TAR 통합서치 일때 어떤게 에러났냐에 따라서 명령어 구분 해서 리턴 줄려구.. 
                                {
                                    if (!ret[Main.DEFINE.OBJ_L])
                                    {
                                        m_errSts = Main.DEFINE.OBJ_L;
                                    }
                                    if (!ret[Main.DEFINE.OBJ_R])
                                    {
                                        m_errSts = Main.DEFINE.OBJ_R;
                                    }
                                    if (!ret[Main.DEFINE.OBJ_L] && !ret[Main.DEFINE.OBJ_R])
                                    {
                                        m_errSts = Main.DEFINE.OBJ_ALL;
                                    }
                                    if (!ret[Main.DEFINE.TAR_L] && !ret[Main.DEFINE.TAR_R] && ret[Main.DEFINE.OBJ_L] && ret[Main.DEFINE.OBJ_R])
                                    {
                                        m_errSts = Main.DEFINE.TAR_ALL;
                                    }
                                }
                                cmd = false;
                                seq = SEQ.COMPLET_SEQ + 1;
                                break;

                            case SEQ.COMPLET_SEQ:
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    m_MotStagePosX[nPatNum[i]] = m_AxisX;
                                    m_MotStagePosY[nPatNum[i]] = m_AxisY;
                                    m_MotStagePosT[nPatNum[i]] = m_AxisT;
                                }

                                cmd = true;
                                seq = SEQ.COMPLET_SEQ + 1;
                                break;
                            case SEQ.COMPLET_SEQ + 1:
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    nMsgMarks += PAT[m_PatTagNo, nPatNum[i]].m_PatternName_Short + " -> MARK SCORE:, " + (PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.Score * 100).ToString("0.00") + " ,";
                                }
                                if (nPatNum.Count > 0) PatternScoreResultSave(nMsgMarks);
                                LoopFlag = false;
                                break;
                        }
                    }
                }
                catch
                {

                }
                return cmd;
            }
            private bool CALISearch(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                ret = CALISearch(nPatNum);
                if ((m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2" || M_ACFCUT_MODE) && m_Cmd != CMD.CALIPER_ALIGN_1CAM_2SHOT_LEFT && m_Cmd != CMD.CALIPER_ALIGN_1CAM_2SHOT_RIGHT)
                {
                    CalierToRobot();
                    m_StageX = (long)-PAT[m_PatTagNo, nType].m_RobotPosX;
                    m_StageY = (long)-PAT[m_PatTagNo, nType].m_RobotPosY;
                    m_StageT = 0;
                }
                DrawSet(nPatNum, nType);
                return ret;
            }
            private bool CALISearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;

                                Mark_ret[nPatNum[i]] = true;
                                PAT[m_PatTagNo, nPatNum[i]].m_Cmd = m_Cmd;

                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    nMarkUse = true;
                                }
                                else
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                            }
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:
                            SearchDelay();
                            if (nPatNum.Count == 1)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            }
                            else
                            {
                                GetImage(nPatNum);
                            }
                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_PATCNL(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                    nMsgMarks += PAT[m_PatTagNo, nPatNum[i]].m_PatternName_Short + " -> MARK SCORE:, " + (PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.Score * 100).ToString("0.00") + " ,";
                                }
                                if (!Mark_ret[nPatNum[i]])
                                {
                                    nRet = false;
                                    PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                }
                            }
                            if (nPatNum.Count > 0 && nMarkUse) PatternScoreResultSave(nMsgMarks);
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 2:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_CALIPER);

                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                                }
                            }

                            if (nPatNum.Count == 1)
                            {
                                if (!nLightCompare[nPatNum[0]])
                                {
                                    SearchDelay();
                                    PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                }
                            }
                            else
                            {
                                bool nLight = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!nLightCompare[nPatNum[i]]) nLight = false;
                                }
                                if (!nLight)
                                {
                                    SearchDelay();
                                    GetImage(nPatNum);
                                }
                            }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int ii = 0; ii < Main.DEFINE.CALIPER_MAX; ii++)
                                    {
                                        (PAT[m_PatTagNo, nPatNum[i]].CaliperTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].CaliperPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                        (PAT[m_PatTagNo, nPatNum[i]].CaliperTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].CaliperPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;
                                    }
                                }
                            }
                            //                             for (int i = 0; i < nPatNum.Count; i++)
                            //                             {
                            //                                 for (int j = 0; j < Main.DEFINE.EXTENSION; j++)
                            //                                 {
                            //                                     double CenterX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + this.PAT[this.m_PatTagNo, nPatNum[i]].InspectionPosRobot_X[j] / Main.AlignUnit[this.m_AlignNo].PAT[this.m_PatTagNo, nPatNum[i]].m_CalX[0];
                            //                                     double CenterY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + this.PAT[this.m_PatTagNo, nPatNum[i]].InspectionPosRobot_Y[j] / Main.AlignUnit[this.m_AlignNo].PAT[this.m_PatTagNo, nPatNum[i]].m_CalY[0];
                            //                                     double SizeX = this.PAT[this.m_PatTagNo, nPatNum[i]].InspectionSizeRobot_X[j] / Main.AlignUnit[this.m_AlignNo].PAT[this.m_PatTagNo, nPatNum[i]].m_CalX[0];
                            //                                     double SizeY = this.PAT[this.m_PatTagNo, nPatNum[i]].InspectionSizeRobot_Y[j] / Main.AlignUnit[this.m_AlignNo].PAT[this.m_PatTagNo, nPatNum[i]].m_CalY[0];
                            // 
                            //                                     if (j == 0)
                            //                                         this.PAT[this.m_PatTagNo, nPatNum[i]].CaliperTools[j].Region.SetCenterLengthsRotationSkew(CenterX, CenterY, SizeX, SizeY, Main.SetCaliperDirection(j), 0);
                            //                                     else
                            //                                         this.PAT[this.m_PatTagNo, nPatNum[i]].CaliperTools[j].Region.SetCenterLengthsRotationSkew(CenterX, CenterY, SizeY, SizeX, Main.SetCaliperDirection(j), 0);
                            //                                 }
                            //                             }


                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (Mark_ret[nPatNum[i]])
                                    ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_Caliper();
                            }
                            seq = 5;
                            break;

                        case 5:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!ret[nPatNum[i]] || !Mark_ret[nPatNum[i]])
                                    nRet = false;
                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                            }
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].CaliperDraw = true;
                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                m_MotStagePosX[nPatNum[i]] = m_AxisX;
                                m_MotStagePosY[nPatNum[i]] = m_AxisY;
                                m_MotStagePosT[nPatNum[i]] = m_AxisT;
                            }
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private bool LINESearch(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                ret = LINESearch(nPatNum);
                DrawSet(nPatNum, nType);

                return ret;
            }

            private bool LINESearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_FixturePixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_FixturePixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;

                                Mark_ret[nPatNum[i]] = true;
                                if (PAT[m_PatTagNo, nPatNum[i]].FINDLine_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    nMarkUse = true;
                                }
                            }
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:
                            SearchDelay();
                            if (nPatNum.Count == 1)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            }
                            else
                            {
                                GetImage(nPatNum);
                            }
                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_PATCNL(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                }
                                if (!Mark_ret[nPatNum[i]])
                                    nRet = false;
                            }
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;
                        case SEQ.GRAB_SEQ + 2:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_LINE);

                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].FINDLine_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_LINE);
                                }
                            }

                            if (nPatNum.Count == 1)
                            {
                                if (!nLightCompare[nPatNum[0]])
                                {
                                    SearchDelay();
                                    PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                }
                            }
                            else
                            {
                                bool nLight = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!nLightCompare[nPatNum[i]]) nLight = false;
                                }
                                if (!nLight)
                                {
                                    SearchDelay();
                                    GetImage(nPatNum);
                                }
                            }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int j = 0; j < Main.DEFINE.SUBLINE_MAX; j++)
                                    {
                                        for (int ii = 0; ii < Main.DEFINE.CALIPER_MAX; ii++)
                                        {
                                            PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, ii].RunParams.ExpectedLineSegment.StartX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].FINDLinePara[j, ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                            PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, ii].RunParams.ExpectedLineSegment.StartY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].FINDLinePara[j, ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;

                                            PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, ii].RunParams.ExpectedLineSegment.EndX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].FINDLinePara[j, ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X2;
                                            PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, ii].RunParams.ExpectedLineSegment.EndY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].FINDLinePara[j, ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y2;
                                        }

                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.ExpectedLineSegmentA = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.ExpectedLineSegment;
                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.ExpectedLineSegmentB = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 1].RunParams.ExpectedLineSegment;

                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.NumCalipers = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.NumCalipers;
                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.CaliperSearchLength = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.CaliperSearchLength;
                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.CaliperProjectionLength = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.CaliperProjectionLength;
                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.CaliperSearchDirection = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.CaliperSearchDirection;
                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.NumToIgnore = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.NumToIgnore;
                                        PAT[m_PatTagNo, nPatNum[i]].CornerTools[0].RunParams.CaliperRunParams = PAT[m_PatTagNo, nPatNum[i]].FINDLineTools[j, 0].RunParams.CaliperRunParams;
                                    }
                                }

                            }
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (Mark_ret[nPatNum[i]])
                                    ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_Line();
                            }
                            seq = 5;
                            break;

                        case 5:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!ret[nPatNum[i]] || !Mark_ret[nPatNum[i]])
                                    nRet = false;
                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].FINDLine_MarkUse)
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                            }
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].FINDLineDraw = true;
                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];
                            }

                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                m_MotStagePosX[nPatNum[i]] = m_AxisX;
                                m_MotStagePosY[nPatNum[i]] = m_AxisY;
                                m_MotStagePosT[nPatNum[i]] = m_AxisT;

                            }
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }

                return cmd;
            }

            // BP
            public ushort ExecuteSearch(int nType, int nSelPad, ushort nCmd)
            {
                ushort ret = 0;
                List<int> nPatNum = new List<int>();
                nPatNum.Add(nType);

                //AddPattern(nType, ref nPatNum);
                ret = ExecuteSearch(nPatNum, nSelPad, nCmd);
                DrawSet(nPatNum, nType);

                return ret;
            }


            // BP
            private ushort ExecuteSearch(List<int> nPatNum, int nSelPad, ushort nCmd)
            {
                int seq = 0;
                int iPatNum = nPatNum[0];

                bool LoopFlag = true;
                ushort ret = 0;
                bool Mark_ret = false;
                bool nLightCompare = false;

                bool nRet = true;
                bool nMarkUse = false;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            PAT[m_PatTagNo, iPatNum].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                            PAT[m_PatTagNo, iPatNum].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                            PAT[m_PatTagNo, iPatNum].PMAlignResult.m_FixturePixel[DEFINE.X] = 0;
                            PAT[m_PatTagNo, iPatNum].PMAlignResult.m_FixturePixel[DEFINE.Y] = 0;
                            PAT[m_PatTagNo, iPatNum].SearchResult = false;
                            PAT[m_PatTagNo, iPatNum].FixtureTrans.TranslationX = 0;
                            PAT[m_PatTagNo, iPatNum].FixtureTrans.TranslationY = 0;

                            Mark_ret = true;
                            if (((nCmd & (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE) == (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE)
                                && /*PAT[m_PatTagNo, iPatNum].FINDLine_MarkUse || */PAT[m_PatTagNo, iPatNum].Caliper_MarkUse)
                            {
                                PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                PAT[m_PatTagNo, iPatNum].m_bCompLight = true;
                                nMarkUse = true;
                            }
                            else if ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE) == (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE)
                            {
                                if (m_AlignNo == 0 || m_AlignNo == 2)   // Race condition 발생. 0, 2번 Unit에서만 조명을 제어. 그 다음 Unit 조명까지 순차적으로 제어
                                {
                                    PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                                    if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                    {
                                        Main.AlignUnit[m_AlignNo + 1].PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                                        Main.AlignUnit[m_AlignNo + 1].PAT[m_PatTagNo, iPatNum].m_bCompLight = true;
                                    }
                                }
                            }
                            else
                            {
                                if (iPatNum == DEFINE.CAM_SELECT_INSPECT)
                                {
                                    seq = SEQ.GRAB_SEQ;
                                    break;
                                }

                                if (m_AlignNo == 0 || m_AlignNo == 2)   // Race condition 발생. 0, 2번 Unit에서만 조명을 제어. 그 다음 Unit 조명까지 순차적으로 제어
                                {
                                    PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_LINE);
                                    if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                    {
                                        Main.AlignUnit[m_AlignNo + 1].PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_LINE);
                                        Main.AlignUnit[m_AlignNo + 1].PAT[m_PatTagNo, iPatNum].m_bCompLight = true;
                                    }
                                }
                            }
                            seq = SEQ.GRAB_SEQ - 1;
                            break;
                        case SEQ.GRAB_SEQ - 1:
                            if (m_AlignNo == 1 || m_AlignNo == 3)
                            {
                                if (PAT[m_PatTagNo, iPatNum].m_bCompLight)
                                {
                                    seq = SEQ.GRAB_SEQ;
                                    break;
                                }
                            }
                            else
                                seq = SEQ.GRAB_SEQ;

                            break;
                        case SEQ.GRAB_SEQ:
                            if (iPatNum != DEFINE.CAM_SELECT_INSPECT)
                                SearchDelay();

                            PAT[m_PatTagNo, iPatNum].m_bCompLight = false;

                            PAT[m_PatTagNo, iPatNum].GetImage();

                            if (m_GD_ImageSave_Use | m_NG_ImageSave_Use)
                            {
                                PAT[m_PatTagNo, iPatNum].Save_ORGImage("GRAB CONFIRM");
                            }

                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else if ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE) == (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE)
                                seq = SEQ.GRAB_SEQ + 3;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            if (PAT[m_PatTagNo, iPatNum].Caliper_MarkUse)
                            {
                                Mark_ret = PAT[m_PatTagNo, iPatNum].Search_PATCNL(PAT[m_PatTagNo, iPatNum].m_CamNo);
                            }
                            if (!Mark_ret)
                                nRet = false;
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                                LogdataDisplay("AlignUnit#" + (m_AlignNo + 1).ToString() + " MARK SEARCH FAIL!", true);
                            }
                            break;
                        case SEQ.GRAB_SEQ + 2:
                            nLightCompare = PAT[m_PatTagNo, iPatNum].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_CALIPER);

                            if (!nLightCompare && PAT[m_PatTagNo, iPatNum].Caliper_MarkUse)
                            {
                                PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                            }

                            if (!nLightCompare)
                            {
                                SearchDelay();
                                PAT[m_PatTagNo, iPatNum].GetImage();
                            }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:  // Check Beam Size
                            bool bToTalRet = true;
                            if (((nCmd & (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE) == (ushort)AlignUnitTag.GRAB_CMD.BEAMSIZE)
                                    /*&& PAT[m_PatTagNo, iPatNum].Caliper_MarkUse && Mark_ret*/)
                            {
                                for (int ii = 0; ii < Main.DEFINE.CALIPER_MAX; ii++)
                                {
                                    if (PAT[m_PatTagNo, iPatNum].Caliper_MarkUse && Mark_ret)
                                    {
                                        (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, iPatNum].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, iPatNum].CaliperPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                        (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, iPatNum].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, iPatNum].CaliperPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;
                                    }

                                    PAT[m_PatTagNo, iPatNum].CaliResults[ii].Pixel[DEFINE.X] = 0;
                                    PAT[m_PatTagNo, iPatNum].CaliResults[ii].Pixel[DEFINE.Y] = 0;
                                    PAT[m_PatTagNo, iPatNum].CaliResults[ii].m_UseCheck = PAT[m_PatTagNo, iPatNum].CaliperPara[ii].m_UseCheck;
                                    PAT[m_PatTagNo, iPatNum].CaliResults[ii].SearchResult = false;

                                    if (PAT[m_PatTagNo, iPatNum].CaliResults[ii].m_UseCheck)
                                    {
                                        PAT[m_PatTagNo, iPatNum].CaliperTools[ii].InputImage = PAT[m_PatTagNo, iPatNum].TempImage;

                                        PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Run();
                                        PAT[m_PatTagNo, iPatNum].CaliResults[ii].CaliperTool = new Cognex.VisionPro.Caliper.CogCaliperTool(PAT[m_PatTagNo, iPatNum].CaliperTools[ii]);

                                        if (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results == null || PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results.Count < 0)
                                        {
                                            PAT[m_PatTagNo, iPatNum].CaliResults[ii].SearchResult = false;
                                            bToTalRet = false;
                                            //seq = SEQ.ERROR_SEQ;
                                            PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosX[Main.DEFINE.FIRST_, Main.DEFINE.XPOS] = 0;
                                            PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosY[Main.DEFINE.FIRST_, Main.DEFINE.YPOS] = 0;
                                            if (Main.GetCaliperPairMode(PAT[m_PatTagNo, iPatNum].CaliperTools[ii].RunParams.EdgeMode))
                                            {
                                                PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosX[Main.DEFINE.SECOND, Main.DEFINE.XPOS] = 0;
                                                PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosY[Main.DEFINE.SECOND, Main.DEFINE.YPOS] = 0;
                                            }

                                            PAT[m_PatTagNo, iPatNum].V2RScalar(PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results[0].Width, ref PAT[m_PatTagNo, iPatNum].CaliResults[ii].RobotWidth);
                                        }
                                        else
                                        {
                                            PAT[m_PatTagNo, iPatNum].CaliResults[ii].SearchResult = true;

                                            PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosX[Main.DEFINE.FIRST_, Main.DEFINE.XPOS] = (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results[0].Edge0.PositionX + PAT[m_PatTagNo, iPatNum].FixtureTrans.TranslationX);
                                            PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosY[Main.DEFINE.FIRST_, Main.DEFINE.YPOS] = (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results[0].Edge0.PositionY + PAT[m_PatTagNo, iPatNum].FixtureTrans.TranslationY);
                                            if (Main.GetCaliperPairMode(PAT[m_PatTagNo, iPatNum].CaliperTools[ii].RunParams.EdgeMode))
                                            {
                                                PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosX[Main.DEFINE.SECOND, Main.DEFINE.XPOS] = (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results[0].Edge1.PositionX + PAT[m_PatTagNo, iPatNum].FixtureTrans.TranslationX);
                                                PAT[m_PatTagNo, iPatNum].CaliResults[ii].PixelPosY[Main.DEFINE.SECOND, Main.DEFINE.YPOS] = (PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results[0].Edge1.PositionY + PAT[m_PatTagNo, iPatNum].FixtureTrans.TranslationY);
                                            }

                                            PAT[m_PatTagNo, iPatNum].V2RScalar(PAT[m_PatTagNo, iPatNum].CaliperTools[ii].Results[0].Width, ref PAT[m_PatTagNo, iPatNum].CaliResults[ii].RobotWidth);
                                        }
                                    }
                                }

                                PAT[m_PatTagNo, iPatNum].CaliperDraw = true;
                                if (bToTalRet)
                                {
                                    ret = nCmd;
                                    seq = SEQ.COMPLET_SEQ;
                                    break;
                                }
                                else
                                {
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                            }
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            if (Mark_ret)
                                ret = PAT[m_PatTagNo, iPatNum].SearchbyCommand(nSelPad, nCmd);

                            seq = SEQ.SEARCH_SEQ + 1;
                            break;

                        case SEQ.SEARCH_SEQ + 1:
                            nRet = true;

                            if (nCmd != ret || !Mark_ret)
                                nRet = false;

                            if (!nLightCompare && PAT[m_PatTagNo, iPatNum].FINDLine_MarkUse)
                                PAT[m_PatTagNo, iPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CNL);

                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }

                            PAT[m_PatTagNo, iPatNum].FINDLineDraw = true;

                            if (((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R))
                                PAT[m_PatTagNo, iPatNum].CircleDraw = true;

                            if (nCmd != ret) m_errSts = iPatNum;

                            break;

                        case SEQ.ERROR_SEQ:
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            m_MotStagePosX[iPatNum] = m_AxisX;
                            m_MotStagePosY[iPatNum] = m_AxisY;
                            m_MotStagePosT[iPatNum] = m_AxisT;

                            if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                            {
                                if (m_AlignNo == 0)
                                {
                                    m_StageX = PAT[m_PatTagNo, iPatNum].FINDLineResults[0].CrossPointList[0].X2;
                                    m_StageY = PAT[m_PatTagNo, iPatNum].FINDLineResults[0].CrossPointList[0].Y2;
                                    m_StageT = PAT[m_PatTagNo, iPatNum].FINDLineResults[2].CrossPointList[0].X2;
                                }
                                else
                                {
                                    m_StageX = PAT[m_PatTagNo, iPatNum].PMAlignResult.m_RobotPosX;
                                    m_StageY = PAT[m_PatTagNo, iPatNum].PMAlignResult.m_RobotPosY;
                                    m_StageT = 0;
                                }

                                InputAligndata(m_StageX, m_StageY, m_StageT);
                                //m_StageX = 
                            }

                            if (iPatNum == DEFINE.CAM_SELECT_INSPECT)
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    m_InspecResult[i] = PAT[m_PatTagNo, iPatNum].InspectionSizeRobot_X[i];
                                }
                            }

                            LoopFlag = false;
                            break;
                    }
                }

                return ret;
            }
            // 200717 BP Cutting Inspection

            private bool PocketBLOBSearch(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                ret = PocketBLOBSearch(nPatNum);
                DrawSet(nPatNum, nType);
                return ret;
            }
            private bool PocketBLOBSearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;
                int nTempPatNum = 0;
                int nCount_OK = 0;


                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:

                            for (int i = 0; i < nPatNum.Count; i++)
                            {

                                PAT[m_PatTagNo, nPatNum[i]].nPocketNum = -1;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;

                                m_TrayResultData.Clear();
                                Mark_ret[nPatNum[i]] = true;

                                if (nPatNum.Count == 1 && (nPatNum[0] == DEFINE.OBJ_R || nPatNum[0] == DEFINE.OBJ_L))
                                {
                                    nTempPatNum = nPatNum[i];

                                    if (!LightUseCheck(DEFINE.OBJ_R))
                                    {
                                        nTempPatNum = DEFINE.OBJ_L;
                                    }
                                    if (!LightUseCheck(DEFINE.OBJ_L))
                                    {
                                        nTempPatNum = DEFINE.OBJ_R;
                                    }
                                }
                                else
                                {
                                    nTempPatNum = nPatNum[i];
                                }

                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    nMarkUse = true;
                                }
                                else if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                {
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                                    nMarkUse = true;
                                }
                                else
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                            }
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:
                            SearchDelay();
                            if (nPatNum.Count == 1)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            }
                            else
                            {
                                GetImage(nPatNum);
                            }

                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_PATCNL(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                }
                                else if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_Caliper(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                }
                                if (!Mark_ret[nPatNum[i]])
                                    nRet = false;
                            }
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 2:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {

                                if (nPatNum.Count == 1 && (nPatNum[0] == DEFINE.OBJ_R || nPatNum[0] == DEFINE.OBJ_L))
                                {
                                    nTempPatNum = nPatNum[i];

                                    if (!LightUseCheck(DEFINE.OBJ_R))
                                    {
                                        nTempPatNum = DEFINE.OBJ_L;
                                    }
                                    if (!LightUseCheck(DEFINE.OBJ_L))
                                    {
                                        nTempPatNum = DEFINE.OBJ_R;
                                    }
                                }
                                else
                                {
                                    nTempPatNum = nPatNum[i];
                                }

                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nTempPatNum].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_BLOB);
                                    if (!nLightCompare[nPatNum[i]])
                                    {
                                        PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                                    }
                                }
                                else if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                {
                                    nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nTempPatNum].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_BLOB);
                                    if (!nLightCompare[nPatNum[i]])
                                    {
                                        PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                                    }
                                }
                            }

                            if (nPatNum.Count == 1)
                            {
                                if (!nLightCompare[nPatNum[0]])
                                {
                                    SearchDelay();
                                    PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                }

                            }
                            else
                            {
                                bool nLight = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!nLightCompare[nPatNum[i]]) nLight = false;
                                }
                                if (!nLight)
                                {
                                    SearchDelay();
                                    GetImage(nPatNum);
                                }
                            }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int ii = 0; ii < Main.DEFINE.BLOB_CNT_MAX; ii++)
                                    {
                                        (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                        (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;
                                    }

                                }
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int ii = 0; ii < Main.DEFINE.BLOB_CNT_MAX; ii++)
                                    {
                                        if (PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_UseCheck)
                                        {
                                            (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PixelCaliper[Main.DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CALIPERTOOL].X;
                                            (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PixelCaliper[Main.DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CALIPERTOOL].Y;
                                        }
                                    }
                                }
                            }
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            try
                            {


                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    Mark_ret[nPatNum[i]] = false;
                                    CogBlobTool tempBlobTool = new CogBlobTool();
                                    CogRectangleAffine BlobSearchRegion = new CogRectangleAffine();
                                    List<CogRectangleAffine> BlobTrainRegion = new List<CogRectangleAffine>();

                                    List<CogRectangleAffine> nBlobSearchRegion = new List<CogRectangleAffine>(); //BLOB 설정영역  
                                    List<CogPolygon> ResultBoundary = new List<CogPolygon>();   // BLOB 찾은결과 경계 영역

                                    double[] BlobArea = new double[Main.DEFINE.BLOB_CNT_MAX];
                                    double Score = new double();

                                    List<string> nMessageList = new List<string>();
                                    string[] nMessage = new string[2];
                                    List<CogColorConstants> nColorList = new List<CogColorConstants>();

                                    List<double> nPosXs = new List<double>();
                                    List<double> nPosYs = new List<double>();

                                    bool tempSearchResult = false, tempUseCheck = false;
                                    int PoketNum = 0;

                                    bool[] Blob_ret = new bool[m_Tray_Pocket_X * m_Tray_Pocket_Y];
                                    //                                    BlobTrainRegion.Clear();
                                    for (int j = 0; j < Main.DEFINE.BLOB_CNT_MAX; j++)
                                    {
                                        if (PAT[m_PatTagNo, nPatNum[i]].BlobPara[j].m_UseCheck)
                                        {
                                            BlobTrainRegion.Add(new CogRectangleAffine(PAT[m_PatTagNo, nPatNum[i]].BlobTools[j].Region as CogRectangleAffine));
                                        }
                                    }



                                    PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult.Clear();

                                    PAT[m_PatTagNo, nPatNum[i]].BlobTools[0].InputImage = PAT[m_PatTagNo, nPatNum[i]].TempImage;

                                    for (int nY = 0; nY < m_Tray_Pocket_Y; nY++)
                                    {
                                        for (int nX = 0; nX < m_Tray_Pocket_X; nX++)
                                        {
                                            PoketNum = (nY * m_Tray_Pocket_X) + nX;


                                            PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult.Add(new TrayBlobResult());


                                            for (int j = 0; j < 1; j++)
                                            {
                                                PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob = new BlobResult();

                                                PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.m_Index = PoketNum;
                                                PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.m_AlignNum = m_AlignNo;
                                                PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.m_AlignName = m_AlignName;
                                                PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.m_PatTagNum = m_PatTagNo;
                                                PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.m_PatNum = nPatNum[i];
                                                //---------------------------------------------------------------------------------------------------------------------------------
                                                Main.DoublePoint FirstPoint = new Main.DoublePoint();
                                                FirstPoint.X = PAT[m_PatTagNo, nPatNum[i]].TRAY_GUIDE_DISX + (nX * PAT[m_PatTagNo, nPatNum[i]].TRAY_PITCH_DISX);
                                                FirstPoint.Y = PAT[m_PatTagNo, nPatNum[i]].TRAY_GUIDE_DISY + (nY * PAT[m_PatTagNo, nPatNum[i]].TRAY_PITCH_DISY);

                                                BlobArea[j] = new double();
                                                if (PAT[m_PatTagNo, nPatNum[i]].BlobPara[j].m_UseCheck)
                                                {
                                                    tempSearchResult = false; tempUseCheck = true;
                                                    tempBlobTool = new CogBlobTool(PAT[m_PatTagNo, nPatNum[i]].BlobTools[j]);

                                                    (tempBlobTool.Region as CogRectangleAffine).CenterX = FirstPoint.X;
                                                    (tempBlobTool.Region as CogRectangleAffine).CenterY = FirstPoint.Y;



                                                    tempBlobTool.Run();
                                                    if (tempBlobTool.Results != null)
                                                    {
                                                        if (tempBlobTool.Results.GetBlobs().Count <= 0)
                                                        {


                                                            Score = 100 - ((BlobArea[j] / BlobSearchRegion.Area) * 100);
                                                            //      Mark_ret[nPatNum[i]] = false;
                                                            PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.SearchResult = true;
                                                        }
                                                        else
                                                        {
                                                            int GetBlobsCount = 1;
                                                            GetBlobsCount = tempBlobTool.Results.GetBlobs().Count;
                                                            for (int k = 0; k < GetBlobsCount; k++) //PMBlobResults[i].BlobToolResult.GetBlobs().Count
                                                            {
                                                                ResultBoundary.Add(tempBlobTool.Results.GetBlobs()[k].GetBoundary());  //--BLOB RESULTS BOUNDARY
                                                                BlobArea[i] += tempBlobTool.Results.GetBlobs()[k].Area;
                                                            }
                                                            PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.SearchResult = false;
                                                            tempSearchResult = true;
                                                            Score = 100;
                                                            Mark_ret[nPatNum[i]] = true;
                                                            nCount_OK++;
                                                        }

                                                        PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].SearchBlobResult = tempSearchResult;
                                                        PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.m_UseCheck = tempUseCheck;
                                                        PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.SearchRegion = new CogRectangleAffine(tempBlobTool.Region as CogRectangleAffine);
                                                        PAT[m_PatTagNo, nPatNum[i]].TRAYBlobResult[PoketNum].TrayBlob.BlobToolResult = new CogBlobResults(tempBlobTool.Results);

                                                    }
                                                }
                                            }
                                        }
                                    }

                                    if (m_GD_ImageSave_Use | m_NG_ImageSave_Use) PAT[m_PatTagNo, nPatNum[i]].Save_Image_Copy();
                                    PAT[m_PatTagNo, nPatNum[i]].ImageSaveResult = Mark_ret[nPatNum[i]];


                                }
                            }
                            catch
                            {

                            }
                            seq = 5;
                            break;

                        case 5:
                            TrayResultData nTempResults;
                            string LogMsg;
                            for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                            {
                                nTempResults = new TrayResultData();

                                nTempResults.SearchResult = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYBlobResult[i].SearchBlobResult;

                                m_TrayResultData.Add(nTempResults);

                                LogMsg = "Index:" + (i + 1).ToString("00") + "," + m_TrayResultData[i].SearchResult.ToString();

                                LogdataDisplay(LogMsg, true);
                            }
                            seq++;
                            break;
                        case 6:

                            if (Main.DEFINE.PROGRAM_TYPE == "FOF_PC6")
                            {
                                int[] nDataBuf = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10]; //nDataBuf Size는 전송 속도 체크후 판단 
                                int nAddress;
                                List<long> nData = new List<long>();

                                for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                                {
                                    nData.Clear();
                                    nData.Add((long)m_TrayResultData[i].AlignData.X);
                                    nData.Add((long)m_TrayResultData[i].AlignData.Y);
                                    nData.Add((long)m_TrayResultData[i].AlignData.T);
                                    nData.Add((long)Convert.ToInt16(m_TrayResultData[i].SearchResult));
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                    nDataBuf[0 + (10 * i)] = (short)((nData[0] & 0x0000ffff));
                                    nDataBuf[1 + (10 * i)] = (short)((nData[0] & 0xffff0000) >> 16); // X DATA
                                    nDataBuf[2 + (10 * i)] = (short)((nData[1] & 0x0000ffff));
                                    nDataBuf[3 + (10 * i)] = (short)((nData[1] & 0xffff0000) >> 16); // Y DATA
                                    nDataBuf[4 + (10 * i)] = (short)((nData[2] & 0x0000ffff));
                                    nDataBuf[5 + (10 * i)] = (short)((nData[2] & 0xffff0000) >> 16); // T DATA
                                    nDataBuf[6 + (10 * i)] = (short)((nData[3]));                    // Search OK | NG

                                    nDataBuf[7 + (10 * i)] = 0;
                                    nDataBuf[8 + (10 * i)] = 0;
                                    nDataBuf[9 + (10 * i)] = 0;
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                }

                                nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_FOF;
                                if (Main.AlignUnit[m_AlignNo].m_AlignName == "FPC_TRAY2")
                                    nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_FOF + 1000;

                                //2022 05 09 YSH
                                //Main.WriteDeviceBlock(nAddress, nDataBuf.Length, ref nDataBuf);

                                int[] setValue = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10];
                                setValue = nDataBuf;
                                Main.PLCsocket.WriteDevice_W(nAddress.ToString(), setValue.Length, setValue);

                                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);
                            }

                            seq++;
                            break;
                        case 7:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!Mark_ret[nPatNum[i]])
                                    nRet = false;
                            }
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                    PAT[m_PatTagNo, nPatNum[i]].CaliperDraw = true;

                                PAT[m_PatTagNo, nPatNum[i]].BlobDraw = true;

                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];


                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            if (m_Blob_NG_View_Use) this.m_NgImage_MonitorFlag = true;
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private bool PocketBLOBTirger(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                ret = PocketBLOBTirger(nPatNum);
                DrawSet(nPatNum, nType);

                return ret;
            }
            private bool PocketBLOBTirger(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;

                bool nRet = true;
                string LogMsg = "";
                int nGetPocketNum = 0, nPickerNum = 0;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            int GetPocket_CountX = 1;
                            int GetPocket_CountY = 1;

                            GetPocket_CountX = (short)(PLCDataTag.RData[Main.AlignUnit[m_AlignNo].ALIGN_UNIT_ADDR + Main.DEFINE.GET_TRAY_COUNT_X]);
                            GetPocket_CountY = (short)(PLCDataTag.RData[Main.AlignUnit[m_AlignNo].ALIGN_UNIT_ADDR + Main.DEFINE.GET_TRAY_COUNT_Y]);

                            if (GetPocket_CountX == 0 || GetPocket_CountX < 0 || GetPocket_CountX > m_Tray_Pocket_X) GetPocket_CountX = 1;
                            if (GetPocket_CountY == 0 || GetPocket_CountY < 0 || GetPocket_CountY > m_Tray_Pocket_Y) GetPocket_CountY = 1;
                            nGetPocketNum = ((GetPocket_CountY - 1) * m_Tray_Pocket_X) + GetPocket_CountX - 1;


                            if (nGetPocketNum >= m_TrayResultData.Count)
                            {
                                nRet = false;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            nRet = m_TrayResultData[nGetPocketNum].SearchResult;

                            if (nRet)
                                seq++;
                            else
                                seq = 3;
                            break;

                        case 1:
                            LogMsg = "Index:" + (nGetPocketNum + 1).ToString("00") + "," + m_TrayResultData[nGetPocketNum].SearchResult.ToString();

                            LogdataDisplay(LogMsg, true);
                            //----------------------------------------------------------------------------------------------------------------------------------------------------------------- 
                            seq++;
                            break;

                        case 2:
                            seq++;
                            break;

                        case 3:
                            seq++;
                            break;

                        case 4:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].nPocketNum = nGetPocketNum;
                            }
                            seq++;
                            break;

                        case 5:
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].BlobDraw = true;
                                if (m_GD_ImageSave_Use | m_NG_ImageSave_Use) PAT[m_PatTagNo, nPatNum[i]].Save_Image_Copy();
                                PAT[m_PatTagNo, nPatNum[i]].ImageSaveResult = nRet;
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private bool PocketSearch(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);

                if (!TrayBlobMode)
                    ret = PocketSearch(nPatNum);
                else
                    ret = PocketBLOBSearch(nPatNum);
                DrawSet(nPatNum, nType);

                return ret;
            }
            private bool PocketSearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[m_Tray_Pocket_X * m_Tray_Pocket_Y];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;

                int nCount_OK = 0;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].nPocketNum = -1;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;

                                PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                nMarkUse = true;
                            }
                            m_TrayResultData.Clear();
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:
                            SearchDelay();
                            if (nPatNum.Count == 1)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            }
                            else
                            {
                                GetImage(nPatNum);
                            }
                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            //                             for (int i = 0; i < nPatNum.Count; i++)
                            //                             {
                            //                                nRet =  PAT[m_PatTagNo, nPatNum[i]].Search_Line(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                            //                             }

                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.ERROR_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 2:
                            //                             for (int i = 0; i < nPatNum.Count; i++)
                            //                             {
                            //                                 nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].LightCompare(Main.DEFINE.M_LIGHT_LINE, Main.DEFINE.M_LIGHT_CNL);
                            //                                 if (!nLightCompare[nPatNum[i]])
                            //                                 {
                            //                                     PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                            //                                 }
                            //                             }

                            //                             if (nPatNum.Count == 1)
                            //                             {
                            //                                 if (!nLightCompare[nPatNum[0]])
                            //                                 {
                            //                                     SearchDelay();
                            //                                     PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            //                                 }
                            // 
                            //                             }
                            //                             else
                            //                             {
                            //                                 bool nLight = true;
                            //                                 for (int i = 0; i < nPatNum.Count; i++)
                            //                                 {
                            //                                     if (!nLightCompare[nPatNum[i]]) nLight = false;
                            //                                 }
                            //                                 if (!nLight)
                            //                                 {
                            //                                     SearchDelay();
                            //                                     GetImage(nPatNum);
                            //                                 }
                            //                             }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            CogRectangle nBackUp_SearchRegion = new CogRectangle();
                            int PoketNum = 0;

                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Pattern[Main.DEFINE.MAIN].SearchRegion == null)
                                {
                                    nBackUp_SearchRegion.SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[PAT[m_PatTagNo, nPatNum[i]].m_CamNo], Main.vision.IMAGE_CENTER_Y[PAT[m_PatTagNo, nPatNum[i]].m_CamNo], Main.vision.IMAGE_SIZE_X[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA, Main.vision.IMAGE_SIZE_Y[PAT[m_PatTagNo, nPatNum[i]].m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA);
                                    PAT[m_PatTagNo, nPatNum[i]].Pattern[Main.DEFINE.MAIN].SearchRegion = new CogRectangle(nBackUp_SearchRegion);
                                }
                                nBackUp_SearchRegion = new CogRectangle(PAT[m_PatTagNo, nPatNum[i]].Pattern[Main.DEFINE.MAIN].SearchRegion as CogRectangle);

                                //   PAT[m_PatTagNo, nPatNum[i]].TRAYResults = new PMResult[m_Tray_Pocket_X * m_Tray_Pocket_Y];


                                PAT[m_PatTagNo, nPatNum[i]].TRAYResults.Clear();




                                for (int nY = 0; nY < m_Tray_Pocket_Y; nY++)
                                {
                                    for (int nX = 0; nX < m_Tray_Pocket_X; nX++)
                                    {
                                        PoketNum = (nY * m_Tray_Pocket_X) + nX;

                                        //---------------------------------------------------------------------------------------------------------------------------------
                                        Main.DoublePoint FirstPoint = new Main.DoublePoint();
                                        FirstPoint.X = PAT[m_PatTagNo, nPatNum[i]].TRAY_GUIDE_DISX + (nX * PAT[m_PatTagNo, nPatNum[i]].TRAY_PITCH_DISX);
                                        FirstPoint.Y = PAT[m_PatTagNo, nPatNum[i]].TRAY_GUIDE_DISY + (nY * PAT[m_PatTagNo, nPatNum[i]].TRAY_PITCH_DISY);
                                        PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[0].DrawPocketPoint2[PoketNum] = new DoublePoint();
                                        PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[0].DrawPocketPoint2[PoketNum] = FirstPoint;
                                        //---------------------------------------------------------------------------------------------------------------------------------
                                        for (int SubTrain = 0; SubTrain < Main.DEFINE.SUBPATTERNMAX; SubTrain++)
                                        {

                                            try
                                            {
                                                if (PAT[m_PatTagNo, nPatNum[i]].Pattern[SubTrain].Pattern.Trained) //if (PAT[m_PatTagNo, nPatNum[i]].Pattern[SubTrain].SearchRegion != null)
                                                {
                                                    if (PAT[m_PatTagNo, nPatNum[i]].Pattern[SubTrain].SearchRegion == null)
                                                    {
                                                        PAT[m_PatTagNo, nPatNum[i]].Pattern[SubTrain].SearchRegion = new CogRectangle();
                                                    }

                                                    (PAT[m_PatTagNo, nPatNum[i]].Pattern[SubTrain].SearchRegion as CogRectangle).SetCenterWidthHeight
                                                        (PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[Main.DEFINE.MAIN].DrawPocketPoint2[PoketNum].X,
                                                         PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[Main.DEFINE.MAIN].DrawPocketPoint2[PoketNum].Y,
                                                         (PAT[m_PatTagNo, nPatNum[i]].Pattern[Main.DEFINE.MAIN].SearchRegion as CogRectangle).Width,
                                                         (PAT[m_PatTagNo, nPatNum[i]].Pattern[Main.DEFINE.MAIN].SearchRegion as CogRectangle).Height);
                                                }
                                                if (PAT[m_PatTagNo, nPatNum[i]].GPattern[SubTrain].Pattern.Trained)  //if (PAT[m_PatTagNo, nPatNum[i]].GPattern[SubTrain].SearchRegion != null) 
                                                {
                                                    if (PAT[m_PatTagNo, nPatNum[i]].GPattern[SubTrain].SearchRegion == null)
                                                    {
                                                        PAT[m_PatTagNo, nPatNum[i]].GPattern[SubTrain].SearchRegion = new CogRectangle();
                                                    }

                                                    (PAT[m_PatTagNo, nPatNum[i]].GPattern[SubTrain].SearchRegion as CogRectangle).SetCenterWidthHeight
                                                    (PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[Main.DEFINE.MAIN].DrawPocketPoint2[PoketNum].X,
                                                    PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[Main.DEFINE.MAIN].DrawPocketPoint2[PoketNum].Y,
                                                    (PAT[m_PatTagNo, nPatNum[i]].GPattern[Main.DEFINE.MAIN].SearchRegion as CogRectangle).Width,
                                                    (PAT[m_PatTagNo, nPatNum[i]].GPattern[Main.DEFINE.MAIN].SearchRegion as CogRectangle).Height);
                                                }
                                            }
                                            catch (System.Exception ex)
                                            {

                                            }

                                        }
                                        //---------------------------------------------------------------------------------------------------------------------------------
                                        if (PAT[m_PatTagNo, nPatNum[i]].Search())
                                        {
                                            nCount_OK++;
                                            Mark_ret[PoketNum] = true;
                                        }
                                        //    PAT[m_PatTagNo, nPatNum[i]].TRAYResults[PoketNum] = new PMResult();

                                        PAT[m_PatTagNo, nPatNum[i]].TRAYResults.Add(new PMResult());
                                        Main.PMResultCopy(PAT[m_PatTagNo, nPatNum[i]].TRAYResults[PoketNum], PAT[m_PatTagNo, nPatNum[i]].PMAlignResult);
                                        PAT[m_PatTagNo, nPatNum[i]].TRAYResults[PoketNum].m_TrayPocketPos.X = PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[Main.DEFINE.MAIN].DrawPocketPoint2[PoketNum].X;
                                        PAT[m_PatTagNo, nPatNum[i]].TRAYResults[PoketNum].m_TrayPocketPos.Y = PAT[m_PatTagNo, nPatNum[i]].FINDLineResults[Main.DEFINE.MAIN].DrawPocketPoint2[PoketNum].Y;
                                    }
                                }
                                PAT[m_PatTagNo, nPatNum[i]].m_TrayCount_OK = nCount_OK;
                                PAT[m_PatTagNo, nPatNum[i]].Pattern[Main.DEFINE.MAIN].SearchRegion = new CogRectangle(nBackUp_SearchRegion);
                                PAT[m_PatTagNo, nPatNum[i]].GPattern[Main.DEFINE.MAIN].SearchRegion = new CogRectangle(nBackUp_SearchRegion);
                            }
                            if (nCount_OK <= 0) nRet = false;
                            seq = 5;
                            break;




                        case 5:
                            TrayResultData nTempResults;
                            string LogMsg;
                            for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                            {
                                nTempResults = new TrayResultData();

                                nTempResults.RobotPoint.X = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYResults[i].m_RobotPosX;
                                nTempResults.RobotPoint.Y = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYResults[i].m_RobotPosY;
                                nTempResults.RobotPoint.T = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYResults[i].m_Rotation;

                                nTempResults.PixelPoint.X = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYResults[i].m_Pixel[Main.DEFINE.X];
                                nTempResults.PixelPoint.Y = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYResults[i].m_Pixel[Main.DEFINE.Y];

                                nTempResults.SearchResult = PAT[m_PatTagNo, Main.DEFINE.OBJ_L].TRAYResults[i].SearchResult;

                                m_TrayResultData.Add(nTempResults);
                            }


                            seq++;
                            break;

                        case 6:
                            //---------------------------------------------------------------OFFSET 계산하는부분 -----------------------------------------------------------------------------
                            double[] OffsetX = new double[2];
                            double[] OffsetY = new double[2];
                            double[] OffsetT = new double[2];

                            OffsetX[0] = m_OffsetX;
                            OffsetX[1] = m_OffsetX2;

                            OffsetY[0] = m_OffsetY;
                            OffsetY[1] = m_OffsetY2;

                            OffsetT[0] = (double)(m_OffsetT / 1000.0 * DEFINE.PI / 180.0);
                            OffsetT[1] = (double)(m_OffsetT2 / 1000.0 * DEFINE.PI / 180.0);

                            double dt = 0;
                            double Cx = 0, Cy = 0, Px = 0, Py = 0;
                            double ImagePosX = 0, ImagePosY = 0;
                            double alignX = 0, alignY = 0;

                            int[] nPitch_X = new int[2];
                            int[] nPitch_Y = new int[2];

                            nPitch_X[0] = machine.m_Fpcpicker1_Dis_X;
                            nPitch_Y[0] = machine.m_Fpcpicker1_Dis_Y;

                            nPitch_X[1] = machine.m_Fpcpicker2_Dis_X;
                            nPitch_Y[1] = machine.m_Fpcpicker2_Dis_Y;

                            double nRotateGap_X1, nRotateGap_Y1;
                            int nPickerNum = 0;
                            for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                            {
                                nPickerNum = i % 2;
                                if (m_TrayResultData[i].SearchResult)
                                {
                                    dt = m_TrayResultData[i].RobotPoint.T;

                                    dt = dt + OffsetT[nPickerNum];

                                    nRotateGap_X1 = nRotateGap_Y1 = 0;
                                    Px = m_MotStagePosX[DEFINE.OBJ_L];
                                    Py = m_MotStagePosY[DEFINE.OBJ_L];

                                    ImagePosX = m_TrayResultData[i].RobotPoint.X;
                                    ImagePosY = m_TrayResultData[i].RobotPoint.Y;

                                    Cx = m_CenterX[m_PatTagNo];  //화면Center에서 회전중심까지 X
                                    Cy = m_CenterY[m_PatTagNo];  //화면Center에서 회전중심까지 Y

                                    alignX = (Cx - ImagePosX) * -1; //Align 값은 회전중심에서 마크좌표까지 거리 더하기
                                    alignY = (Cy - ImagePosY) * -1; //Align 값은 회전중심에서 마크좌표까지 거리 더하기

                                    alignX = alignX + (nPitch_X[nPickerNum] * -1); //회전중심에서 Picker 까지 거리 X
                                    alignY = alignY + (nPitch_Y[nPickerNum] * -1); //회전중심에서 Picker 까지 거리 Y

                                    nRotateGap_X1 = (nPitch_X[nPickerNum] * Math.Cos(dt) - nPitch_Y[nPickerNum] * Math.Sin(dt)); //Theta에 대한 회전별량 ( 회전중심에서 Picker까지의 거리값의 회전별량)
                                    nRotateGap_Y1 = (nPitch_Y[nPickerNum] * Math.Cos(dt) + nPitch_X[nPickerNum] * Math.Sin(dt)); //Theta에 대한 회전별량 ( 회전중심에서 Picker까지의 거리값의 회전별량)

                                    nRotateGap_X1 = nPitch_X[nPickerNum] - nRotateGap_X1;
                                    nRotateGap_Y1 = nPitch_Y[nPickerNum] - nRotateGap_Y1;

                                    alignX += nRotateGap_X1 + OffsetX[nPickerNum];                              //Offset X 
                                    alignY += nRotateGap_Y1 + OffsetY[nPickerNum];                              //Offset Y

                                    m_TrayResultData[i].AlignData.X = alignX;
                                    m_TrayResultData[i].AlignData.Y = alignY;

                                    m_TrayResultData[i].AlignData.X = m_TrayResultData[i].RobotPoint.X;
                                    m_TrayResultData[i].AlignData.Y = m_TrayResultData[i].RobotPoint.Y;
                                    m_TrayResultData[i].AlignData.T = 0;// (dt * Main.DEFINE.degree * 1000);
                                }
                                else
                                {
                                    m_TrayResultData[i].AlignData.X = 0;
                                    m_TrayResultData[i].AlignData.Y = 0;
                                    m_TrayResultData[i].AlignData.T = 0;
                                }
                                LogMsg = "Index:" + (i + 1).ToString("00") + "," + m_TrayResultData[i].SearchResult.ToString() + " ,Align(" + ((long)m_TrayResultData[i].AlignData.X).ToString() + ", " + ((long)m_TrayResultData[i].AlignData.Y).ToString() + ", " + ((long)m_TrayResultData[i].AlignData.T).ToString() + ")";

                                LogdataDisplay(LogMsg, true);
                            }
                            //----------------------------------------------------------------------------------------------------------------------------------------------------------------- 
                            seq++;
                            break;

                        case 7:
                            int GetPocket_CountX = 1;
                            int GetPocket_CountY = 1;
                            int nGetPocketNum = 0;

                            GetPocket_CountX = (short)(PLCDataTag.RData[Main.AlignUnit[m_AlignNo].ALIGN_UNIT_ADDR + Main.DEFINE.GET_TRAY_COUNT_X]);
                            GetPocket_CountY = (short)(PLCDataTag.RData[Main.AlignUnit[m_AlignNo].ALIGN_UNIT_ADDR + Main.DEFINE.GET_TRAY_COUNT_Y]);

                            if (GetPocket_CountX == 0 || GetPocket_CountX < 0 || GetPocket_CountX > m_Tray_Pocket_X) GetPocket_CountX = 1;
                            if (GetPocket_CountY == 0 || GetPocket_CountY < 0 || GetPocket_CountY > m_Tray_Pocket_Y) GetPocket_CountY = 1;
                            nGetPocketNum = ((GetPocket_CountY - 1) * m_Tray_Pocket_X) + GetPocket_CountX;

                            m_StageX = (long)m_TrayResultData[nGetPocketNum - 1].AlignData.X;
                            m_StageY = (long)m_TrayResultData[nGetPocketNum - 1].AlignData.Y;
                            m_StageT = (long)m_TrayResultData[nGetPocketNum - 1].AlignData.T;
                            seq++;
                            break;



                        case 8:

                            seq++;
                            break;

                        case 9:

                            seq++;
                            break;

                        case 10:
                            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------                      
                            if (Main.DEFINE.PROGRAM_TYPE == "FOF_PC6" && Main.AlignUnit[m_AlignNo].m_AlignName == "FPC_TRAY1")
                            {
                                int[] nDataBuf = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10]; //nDataBuf Size는 전송 속도 체크후 판단 
                                int nAddress;
                                List<long> nData = new List<long>();

                                for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                                {
                                    nData.Clear();
                                    nData.Add((long)m_TrayResultData[i].AlignData.X);
                                    nData.Add((long)m_TrayResultData[i].AlignData.Y);
                                    nData.Add((long)m_TrayResultData[i].AlignData.T);
                                    nData.Add((long)Convert.ToInt16(m_TrayResultData[i].SearchResult));
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                    nDataBuf[0 + (10 * i)] = (short)((nData[0] & 0x0000ffff));
                                    nDataBuf[1 + (10 * i)] = (short)((nData[0] & 0xffff0000) >> 16); // X DATA
                                    nDataBuf[2 + (10 * i)] = (short)((nData[1] & 0x0000ffff));
                                    nDataBuf[3 + (10 * i)] = (short)((nData[1] & 0xffff0000) >> 16); // Y DATA
                                    nDataBuf[4 + (10 * i)] = (short)((nData[2] & 0x0000ffff));
                                    nDataBuf[5 + (10 * i)] = (short)((nData[2] & 0xffff0000) >> 16); // T DATA
                                    nDataBuf[6 + (10 * i)] = (short)((nData[3]));                    // Search OK | NG

                                    nDataBuf[7 + (10 * i)] = 0;
                                    nDataBuf[8 + (10 * i)] = 0;
                                    nDataBuf[9 + (10 * i)] = 0;
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                }

                                nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_FOF;
                                //YSH 임시 주석, 필요 시 추가
                                //Main.WriteDeviceBlock(nAddress, nDataBuf.Length, ref nDataBuf);
                                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);
                            }
                            else if (Main.DEFINE.PROGRAM_TYPE == "FOF_PC6" && Main.AlignUnit[m_AlignNo].m_AlignName == "FPC_TRAY2")
                            {
                                int[] nDataBuf = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10]; //nDataBuf Size는 전송 속도 체크후 판단 
                                int nAddress;
                                List<long> nData = new List<long>();

                                for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                                {
                                    nData.Clear();
                                    nData.Add((long)m_TrayResultData[i].AlignData.X);
                                    nData.Add((long)m_TrayResultData[i].AlignData.Y);
                                    nData.Add((long)m_TrayResultData[i].AlignData.T);
                                    nData.Add((long)Convert.ToInt16(m_TrayResultData[i].SearchResult));
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                    nDataBuf[0 + (10 * i)] = (short)((nData[0] & 0x0000ffff));
                                    nDataBuf[1 + (10 * i)] = (short)((nData[0] & 0xffff0000) >> 16); // X DATA
                                    nDataBuf[2 + (10 * i)] = (short)((nData[1] & 0x0000ffff));
                                    nDataBuf[3 + (10 * i)] = (short)((nData[1] & 0xffff0000) >> 16); // Y DATA
                                    nDataBuf[4 + (10 * i)] = (short)((nData[2] & 0x0000ffff));
                                    nDataBuf[5 + (10 * i)] = (short)((nData[2] & 0xffff0000) >> 16); // T DATA
                                    nDataBuf[6 + (10 * i)] = (short)((nData[3]));                    // Search OK | NG

                                    nDataBuf[7 + (10 * i)] = 0;
                                    nDataBuf[8 + (10 * i)] = 0;
                                    nDataBuf[9 + (10 * i)] = 0;
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                }

                                nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_FOF + 1000;
                                //YSH 임의주석, 필요 시 추가
                                //Main.WriteDeviceBlock(nAddress, nDataBuf.Length, ref nDataBuf);
                                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);
                            }
                            else if (Main.DEFINE.PROGRAM_TYPE == "TFOG_PC2" && Main.AlignUnit[m_AlignNo].m_AlignName == "FPC_TRAY1")
                            {
                                int[] nDataBuf = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10]; //nDataBuf Size는 전송 속도 체크후 판단 
                                int nAddress;
                                List<long> nData = new List<long>();

                                for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                                {
                                    nData.Clear();
                                    nData.Add((long)m_TrayResultData[i].AlignData.X);
                                    nData.Add((long)m_TrayResultData[i].AlignData.Y);
                                    nData.Add((long)m_TrayResultData[i].AlignData.T);
                                    nData.Add((long)Convert.ToInt16(m_TrayResultData[i].SearchResult));
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                    nDataBuf[0 + (10 * i)] = (short)((nData[0] & 0x0000ffff));
                                    nDataBuf[1 + (10 * i)] = (short)((nData[0] & 0xffff0000) >> 16); // X DATA
                                    nDataBuf[2 + (10 * i)] = (short)((nData[1] & 0x0000ffff));
                                    nDataBuf[3 + (10 * i)] = (short)((nData[1] & 0xffff0000) >> 16); // Y DATA
                                    nDataBuf[4 + (10 * i)] = (short)((nData[2] & 0x0000ffff));
                                    nDataBuf[5 + (10 * i)] = (short)((nData[2] & 0xffff0000) >> 16); // T DATA
                                    nDataBuf[6 + (10 * i)] = (short)((nData[3]));                    // Search OK | NG

                                    nDataBuf[7 + (10 * i)] = 0;
                                    nDataBuf[8 + (10 * i)] = 0;
                                    nDataBuf[9 + (10 * i)] = 0;
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                }

                                nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_TFOG;
                                //YSH 임의주석, 필요 시 추가
                                //Main.WriteDeviceBlock(nAddress, nDataBuf.Length, ref nDataBuf);
                                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);
                            }
                            else if (Main.DEFINE.PROGRAM_TYPE == "FOB_PC5" && Main.AlignUnit[m_AlignNo].m_AlignName == "FPC_TRAY1")
                            {
                                int[] nDataBuf = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10]; //nDataBuf Size는 전송 속도 체크후 판단 
                                int nAddress;
                                List<long> nData = new List<long>();

                                for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                                {
                                    nData.Clear();
                                    nData.Add((long)m_TrayResultData[i].AlignData.X);
                                    nData.Add((long)m_TrayResultData[i].AlignData.Y);
                                    nData.Add((long)m_TrayResultData[i].AlignData.T);
                                    nData.Add((long)Convert.ToInt16(m_TrayResultData[i].SearchResult));
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                    nDataBuf[0 + (10 * i)] = (short)((nData[0] & 0x0000ffff));
                                    nDataBuf[1 + (10 * i)] = (short)((nData[0] & 0xffff0000) >> 16); // X DATA
                                    nDataBuf[2 + (10 * i)] = (short)((nData[1] & 0x0000ffff));
                                    nDataBuf[3 + (10 * i)] = (short)((nData[1] & 0xffff0000) >> 16); // Y DATA
                                    nDataBuf[4 + (10 * i)] = (short)((nData[2] & 0x0000ffff));
                                    nDataBuf[5 + (10 * i)] = (short)((nData[2] & 0xffff0000) >> 16); // T DATA
                                    nDataBuf[6 + (10 * i)] = (short)((nData[3]));                    // Search OK | NG

                                    nDataBuf[7 + (10 * i)] = 0;
                                    nDataBuf[8 + (10 * i)] = 0;
                                    nDataBuf[9 + (10 * i)] = 0;
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                }

                                nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_FOB;
                                //YSH 임의주석, 필요 시 추가
                                //Main.WriteDeviceBlock(nAddress, nDataBuf.Length, ref nDataBuf);
                                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);
                            }
                            else if (Main.DEFINE.PROGRAM_TYPE == "FOB_PC6" && Main.AlignUnit[m_AlignNo].m_AlignName == "FPC_TRAY2")
                            {
                                int[] nDataBuf = new int[m_Tray_Pocket_X * m_Tray_Pocket_Y * 10]; //nDataBuf Size는 전송 속도 체크후 판단 
                                int nAddress;
                                List<long> nData = new List<long>();

                                for (int i = 0; i < m_Tray_Pocket_X * m_Tray_Pocket_Y; i++)
                                {
                                    nData.Clear();
                                    nData.Add((long)m_TrayResultData[i].AlignData.X);
                                    nData.Add((long)m_TrayResultData[i].AlignData.Y);
                                    nData.Add((long)m_TrayResultData[i].AlignData.T);
                                    nData.Add((long)Convert.ToInt16(m_TrayResultData[i].SearchResult));
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                    nDataBuf[0 + (10 * i)] = (short)((nData[0] & 0x0000ffff));
                                    nDataBuf[1 + (10 * i)] = (short)((nData[0] & 0xffff0000) >> 16); // X DATA
                                    nDataBuf[2 + (10 * i)] = (short)((nData[1] & 0x0000ffff));
                                    nDataBuf[3 + (10 * i)] = (short)((nData[1] & 0xffff0000) >> 16); // Y DATA
                                    nDataBuf[4 + (10 * i)] = (short)((nData[2] & 0x0000ffff));
                                    nDataBuf[5 + (10 * i)] = (short)((nData[2] & 0xffff0000) >> 16); // T DATA
                                    nDataBuf[6 + (10 * i)] = (short)((nData[3]));                    // Search OK | NG

                                    nDataBuf[7 + (10 * i)] = 0;
                                    nDataBuf[8 + (10 * i)] = 0;
                                    nDataBuf[9 + (10 * i)] = 0;
                                    //-----------------------------------------------------------------------------------------------------------------------------------
                                }

                                nAddress = Main.DEFINE.VIS_FPCTRAY_ALIGN_DATA_FOB + 1000;
                                //YSH 임의주석, 필요 시 추가
                                //Main.WriteDeviceBlock(nAddress, nDataBuf.Length, ref nDataBuf);
                                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);
                            }
                            //-----------------------------------------------------------------------------------------------------------------------------------------------------------------
                            seq++;
                            break;

                        case 11:
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].FINDLineDraw = true;
                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }

            private bool PocketTirger(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                if (!TrayBlobMode)
                    ret = PocketTirger(nPatNum);
                else
                    ret = PocketBLOBTirger(nPatNum);
                DrawSet(nPatNum, nType);

                return ret;
            }
            private bool PocketTirger(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;

                bool nRet = true;
                int nGetPocketNum = 0;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            string LogMsg;

                            int GetPocket_CountX = 1;
                            int GetPocket_CountY = 1;
                            GetPocket_CountX = (short)(PLCDataTag.RData[Main.AlignUnit[m_AlignNo].ALIGN_UNIT_ADDR + Main.DEFINE.GET_TRAY_COUNT_X]);
                            GetPocket_CountY = (short)(PLCDataTag.RData[Main.AlignUnit[m_AlignNo].ALIGN_UNIT_ADDR + Main.DEFINE.GET_TRAY_COUNT_Y]);

                            if (GetPocket_CountX == 0 || GetPocket_CountX < 0 || GetPocket_CountX > m_Tray_Pocket_X) GetPocket_CountX = 1;
                            if (GetPocket_CountY == 0 || GetPocket_CountY < 0 || GetPocket_CountY > m_Tray_Pocket_Y) GetPocket_CountY = 1;
                            nGetPocketNum = ((GetPocket_CountY - 1) * m_Tray_Pocket_X) + GetPocket_CountX - 1;

                            if (nGetPocketNum >= m_TrayResultData.Count)
                            {
                                nRet = false;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            nRet = m_TrayResultData[nGetPocketNum].SearchResult;

                            m_StageX = (long)m_TrayResultData[nGetPocketNum].AlignData.X;
                            m_StageY = (long)m_TrayResultData[nGetPocketNum].AlignData.Y;
                            m_StageT = (long)m_TrayResultData[nGetPocketNum].AlignData.T;

                            LogMsg = "Index:" + (nGetPocketNum).ToString("00") + "," + m_TrayResultData[nGetPocketNum].SearchResult.ToString() +
                                     " ,Align(" + ((long)m_TrayResultData[nGetPocketNum].AlignData.X).ToString() + ", " + ((long)m_TrayResultData[nGetPocketNum].AlignData.Y).ToString() + ", " + ((long)m_TrayResultData[nGetPocketNum].AlignData.T).ToString() + ")";

                            seq++;
                            break;

                        case 1:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].nPocketNum = nGetPocketNum;
                            }
                            seq++;
                            break;

                        case 2:
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].FINDLineDraw = true;
                                if (m_GD_ImageSave_Use | m_NG_ImageSave_Use) PAT[m_PatTagNo, nPatNum[i]].Save_Image_Copy();
                                PAT[m_PatTagNo, nPatNum[i]].ImageSaveResult = nRet;
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }


            private bool CircleSearch(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                ret = CircleSearch(nPatNum);
                DrawSet(nPatNum, nType);
                return ret;
            }
            private bool CircleSearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            if (Main.DEFINE.OPEN_F)
                            {
                                //                                  Main.vision.CogImgTool[PAT[m_PatTagNo, nPatNum[0]].m_CamNo].Run();
                                //                                   vision.CogCamBuf[PAT[m_PatTagNo, nPatNum[0]].m_CamNo] = vision.CogImgTool[PAT[m_PatTagNo, nPatNum[0]].m_CamNo].OutputImage as ICogImage;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;

                                Mark_ret[nPatNum[i]] = true;

                                if (PAT[m_PatTagNo, nPatNum[i]].Circle_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    nMarkUse = true;
                                }
                                else
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CIRCLE);
                            }
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:
                            SearchDelay();
                            if (nPatNum.Count == 1)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            }
                            else
                            {
                                GetImage(nPatNum);
                            }
                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Circle_MarkUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_PATCNL(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                }
                                if (!Mark_ret[nPatNum[i]])
                                    nRet = false;
                            }
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 2:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_CIRCLE);

                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Circle_MarkUse)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CIRCLE);
                                }
                            }

                            if (nPatNum.Count == 1)
                            {
                                if (!nLightCompare[nPatNum[0]])
                                {
                                    SearchDelay();
                                    PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                }
                            }
                            else
                            {
                                bool nLight = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!nLightCompare[nPatNum[i]]) nLight = false;
                                }
                                if (!nLight)
                                {
                                    SearchDelay();
                                    GetImage(nPatNum);
                                }
                            }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Circle_MarkUse && Mark_ret[nPatNum[i]])
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTool.InputImage = PAT[m_PatTagNo, nPatNum[i]].TempImage;
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X];
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y];
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTool.RunParams.UnfixturedFromFixturedTransform = PAT[m_PatTagNo, nPatNum[i]].FixtureTrans;
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTool.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
                                    PAT[m_PatTagNo, nPatNum[i]].FixtureTool.Run();
                                    PAT[m_PatTagNo, nPatNum[i]].TempImage = PAT[m_PatTagNo, nPatNum[i]].FixtureTool.OutputImage as CogImage8Grey;
                                }
                            }
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (Mark_ret[nPatNum[i]])
                                    ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_Circle();
                            }
                            seq = 5;
                            break;

                        case 5:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!ret[nPatNum[i]] || !Mark_ret[nPatNum[i]])
                                    nRet = false;
                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Circle_MarkUse)
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                            }
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].CircleDraw = true;
                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                m_MotStagePosX[nPatNum[i]] = m_AxisX;
                                m_MotStagePosY[nPatNum[i]] = m_AxisY;
                                m_MotStagePosT[nPatNum[i]] = m_AxisT;
                            }
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private bool BLOBSearch(int nType)
            {
                bool ret = false;
                List<int> nPatNum = new List<int>();

                AddPattern(nType, ref nPatNum);
                ret = BLOBSearch(nPatNum);
                DrawSet(nPatNum, nType);
                return ret;
            }
            private bool BLOBSearch(List<int> nPatNum)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;
                int nTempPatNum = 0;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:

                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] = 0;
                                PAT[m_PatTagNo, nPatNum[i]].SearchResult = false;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationX = 0;
                                PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.TranslationY = 0;

                                Mark_ret[nPatNum[i]] = true;

                                if (nPatNum.Count == 1 && (nPatNum[0] == DEFINE.OBJ_R || nPatNum[0] == DEFINE.OBJ_L))
                                {
                                    nTempPatNum = nPatNum[i];

                                    if (!LightUseCheck(DEFINE.OBJ_R))
                                    {
                                        nTempPatNum = DEFINE.OBJ_L;
                                    }
                                    if (!LightUseCheck(DEFINE.OBJ_L))
                                    {
                                        nTempPatNum = DEFINE.OBJ_R;
                                    }
                                }
                                else
                                {
                                    nTempPatNum = nPatNum[i];
                                }

                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                    nMarkUse = true;
                                }
                                else if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                {
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                                    nMarkUse = true;
                                }
                                else
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                            }
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:
                            SearchDelay();
                            if (nPatNum.Count == 1)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].GetImage();
                            }
                            else
                            {
                                GetImage(nPatNum);
                            }

                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_PATCNL(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                }
                                else if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                {
                                    Mark_ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].Search_Caliper(PAT[m_PatTagNo, nPatNum[i]].m_CamNo);
                                }
                                if (!Mark_ret[nPatNum[i]])
                                    nRet = false;
                            }
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 2:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {

                                if (nPatNum.Count == 1 && (nPatNum[0] == DEFINE.OBJ_R || nPatNum[0] == DEFINE.OBJ_L))
                                {
                                    nTempPatNum = nPatNum[i];

                                    if (!LightUseCheck(DEFINE.OBJ_R))
                                    {
                                        nTempPatNum = DEFINE.OBJ_L;
                                    }
                                    if (!LightUseCheck(DEFINE.OBJ_L))
                                    {
                                        nTempPatNum = DEFINE.OBJ_R;
                                    }
                                }
                                else
                                {
                                    nTempPatNum = nPatNum[i];
                                }

                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nTempPatNum].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_BLOB);
                                    if (!nLightCompare[nPatNum[i]])
                                    {
                                        PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                                    }
                                }
                                else if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                {
                                    nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nTempPatNum].LightCompare(Main.DEFINE.M_LIGHT_CNL, Main.DEFINE.M_LIGHT_BLOB);
                                    if (!nLightCompare[nPatNum[i]])
                                    {
                                        PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                                    }
                                }
                            }

                            if (nPatNum.Count == 1)
                            {
                                if (!nLightCompare[nPatNum[0]])
                                {
                                    SearchDelay();
                                    PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                }

                            }
                            else
                            {
                                bool nLight = true;
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    if (!nLightCompare[nPatNum[i]]) nLight = false;
                                }
                                if (!nLight)
                                {
                                    SearchDelay();
                                    GetImage(nPatNum);
                                }
                            }
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int ii = 0; ii < Main.DEFINE.BLOB_CNT_MAX; ii++)
                                    {
                                        (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                        (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;

                                        //                                     if (PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.SearchResult)
                                        //                                         PAT[m_PatTagNo, nPatNum[i]].FixtureTrans.Rotation = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Rotation;
                                    }

                                }
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int ii = 0; ii < Main.DEFINE.BLOB_CNT_MAX; ii++)
                                    {
                                        if (PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_UseCheck)
                                        {
                                            (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PixelCaliper[Main.DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CALIPERTOOL].X;
                                            (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PixelCaliper[Main.DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CALIPERTOOL].Y;
                                        }
                                    }
                                }
                            }
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (Mark_ret[nPatNum[i]])
                                    ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].BLOBSearch();
                                if (!Main.BLOBINSPECTION_USE(m_AlignNo))
                                    nMsgMarks += PAT[m_PatTagNo, nPatNum[i]].m_PatternName_Short + " -> BLOB SCORE:, " + PAT[m_PatTagNo, nPatNum[i]].BlobScore + " ,";
                            }

                            seq = 5;
                            break;

                        case 5:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!ret[nPatNum[i]] || !Mark_ret[nPatNum[i]])
                                    nRet = false;

                                if (nPatNum.Count == 1 && (nPatNum[0] == DEFINE.OBJ_R || nPatNum[0] == DEFINE.OBJ_L))
                                {
                                    nTempPatNum = nPatNum[i];

                                    if (!LightUseCheck(DEFINE.OBJ_R))
                                    {
                                        nTempPatNum = DEFINE.OBJ_L;
                                    }
                                    if (!LightUseCheck(DEFINE.OBJ_L))
                                    {
                                        nTempPatNum = DEFINE.OBJ_R;
                                    }
                                }
                                else
                                {
                                    nTempPatNum = nPatNum[i];
                                }

                                if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                                else if (!nLightCompare[nPatNum[i]] && PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                    PAT[m_PatTagNo, nTempPatNum].SetAllLight(Main.DEFINE.M_LIGHT_CALIPER);
                                else
                                {
                                }
                            }
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_CaliperUse)
                                    PAT[m_PatTagNo, nPatNum[i]].CaliperDraw = true;

                                PAT[m_PatTagNo, nPatNum[i]].BlobDraw = true;

                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];


                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            if (m_Blob_NG_View_Use) this.m_NgImage_MonitorFlag = true;
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private bool BLOBSearch(List<int> nPatNum, string BLOB)
            {
                int seq = 0;
                bool cmd = false;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];
                bool[] Mark_ret = new bool[DEFINE.Pattern_Max];
                bool[] nLightCompare = new bool[DEFINE.Pattern_Max];

                bool nRet = true;
                bool nMarkUse = false;
                int nTempPatNum = 0;
                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {

                                Mark_ret[nPatNum[i]] = true;

                                nLightCompare[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].LightCompare(Main.DEFINE.M_LIGHT_CALIPER, Main.DEFINE.M_LIGHT_BLOB);
                                if (!nLightCompare[nPatNum[i]])
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].SetAllLight(Main.DEFINE.M_LIGHT_BLOB);
                                    SearchDelay();
                                    if (nPatNum.Count == 1)
                                    {
                                        PAT[m_PatTagNo, nPatNum[0]].GetImage();
                                    }
                                    else
                                    {
                                        GetImage(nPatNum);
                                    }
                                }
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse)
                                {
                                    nMarkUse = true;
                                }
                                else
                                {

                                }
                            }
                            seq = SEQ.GRAB_SEQ;
                            break;

                        case SEQ.GRAB_SEQ:

                            if (nMarkUse)
                                seq = SEQ.GRAB_SEQ + 1;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 1:
                            nRet = true;
                            if (nRet)
                                seq = SEQ.GRAB_SEQ + 2;
                            else
                                seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.GRAB_SEQ + 2:
                            seq = SEQ.GRAB_SEQ + 3;
                            break;

                        case SEQ.GRAB_SEQ + 3:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Blob_MarkUse && Mark_ret[nPatNum[i]])
                                {
                                    for (int ii = 0; ii < 2; ii++)
                                    {
                                        (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterX = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.X] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].X;
                                        (PAT[m_PatTagNo, nPatNum[i]].BlobTools[ii].Region as CogRectangleAffine).CenterY = PAT[m_PatTagNo, nPatNum[i]].PMAlignResult.m_Pixel[DEFINE.Y] + PAT[m_PatTagNo, nPatNum[i]].BlobPara[ii].m_TargetToCenter[Main.DEFINE.M_CNLSEARCHTOOL].Y;
                                    }
                                }
                            }
                            seq = SEQ.SEARCH_SEQ;
                            break;

                        case SEQ.SEARCH_SEQ:
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (Mark_ret[nPatNum[i]])
                                    ret[nPatNum[i]] = PAT[m_PatTagNo, nPatNum[i]].BLOBSearch();
                            }
                            seq = 5;
                            break;

                        case 5:
                            nRet = true;
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (!ret[nPatNum[i]] || !Mark_ret[nPatNum[i]])
                                    nRet = false;
                            }
                            if (nRet)
                            {
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                seq = SEQ.ERROR_SEQ;
                            }
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                PAT[m_PatTagNo, nPatNum[i]].BlobDraw = true;
                                if (!ret[nPatNum[i]]) m_errSts = nPatNum[i];
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            cmd = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            cmd = true;
                            LoopFlag = false;
                            break;
                    }
                }
                return cmd;
            }
            private void SearchDelay()
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
                            if (m_Timer.GetElapsedTime() >= m_AlignDelay)
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }
                            if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT)
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }
                            break;

                        case SEQ.ERROR_SEQ:
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            LoopFlag = false;
                            break;
                    }

                }

            }
            private void StageCenterAlign(string _AlignName, string nStageMode)
            {
                int nPatNumL, nPatNumR;
                int temp_TagNum;

                string nUnitAdd = "PBD_STAGE1";
                if (m_AlignName == "PBD2")
                    nUnitAdd = "PBD_STAGE2";


                if (nStageMode == "OBJECT")     //FOF 일때
                {
                    nPatNumL = DEFINE.OBJ_L;
                    nPatNumR = DEFINE.OBJ_R;

                    temp_TagNum = 1;
                }
                else
                {       //FOP / COF 일때
                    nPatNumL = DEFINE.TAR_L;
                    nPatNumR = DEFINE.TAR_R;
                    temp_TagNum = m_PatTagNo;
                }


                AlignUnit[nUnitAdd].m_Cell_ID = m_Cell_ID;
                AlignUnit[nUnitAdd].m_PatTagNo = temp_TagNum;

                AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_L].Pixel[DEFINE.X] = PAT[m_PatTagNo, nPatNumL].Pixel[DEFINE.X];
                AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_L].Pixel[DEFINE.Y] = PAT[m_PatTagNo, nPatNumL].Pixel[DEFINE.Y];

                AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_R].Pixel[DEFINE.X] = PAT[m_PatTagNo, nPatNumR].Pixel[DEFINE.X];
                AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_R].Pixel[DEFINE.Y] = PAT[m_PatTagNo, nPatNumR].Pixel[DEFINE.Y];

                AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_L].V2R(ref AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_L].m_RobotPosX, ref AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_L].m_RobotPosY);
                AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_R].V2R(ref AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_R].m_RobotPosX, ref AlignUnit[nUnitAdd].PAT[temp_TagNum, DEFINE.OBJ_R].m_RobotPosY);

                AlignUnit[nUnitAdd].m_Timer_index.StartTimer();


                string LogMsg = LogMsg = "Cell_ID_" + AlignUnit[nUnitAdd].m_PatTagNo.ToString() + "<-" + AlignUnit[nUnitAdd].m_Cell_ID;
                AlignUnit[nUnitAdd].LogdataDisplay(LogMsg, true);

                AlignUnit[nUnitAdd].GetPosition();

                AlignUnit[nUnitAdd].m_MotStagePosX[DEFINE.OBJ_R] = AlignUnit[nUnitAdd].m_MotStagePosX[DEFINE.OBJ_L] = AlignUnit[nUnitAdd].m_AxisX;
                AlignUnit[nUnitAdd].m_MotStagePosY[DEFINE.OBJ_R] = AlignUnit[nUnitAdd].m_MotStagePosY[DEFINE.OBJ_L] = AlignUnit[nUnitAdd].m_AxisY;
                AlignUnit[nUnitAdd].m_MotStagePosT[DEFINE.OBJ_R] = AlignUnit[nUnitAdd].m_MotStagePosT[DEFINE.OBJ_L] = AlignUnit[nUnitAdd].m_AxisT;


                AlignUnit[nUnitAdd].Processing(0, false, true, false);
                AlignUnit[nUnitAdd].SetAlignPosEnd(temp_TagNum);
                AlignUnit[nUnitAdd].InputAligndata(AlignUnit[nUnitAdd].m_StageX, AlignUnit[nUnitAdd].m_StageY, AlignUnit[nUnitAdd].m_StageT);

            }

            public void AddPattern(int nType, ref List<int> nPatNum)
            {
                if (nType == DEFINE.OBJ_ALL)
                {
                    nPatNum.Add(DEFINE.OBJ_L);
                    nPatNum.Add(DEFINE.OBJ_R);
                }
                else if (nType == DEFINE.TAR_ALL)
                {
                    nPatNum.Add(DEFINE.TAR_L);
                    nPatNum.Add(DEFINE.TAR_R);
                }
                else if (nType == DEFINE.LEFT_ALL)
                {
                    nPatNum.Add(DEFINE.OBJ_L);
                    nPatNum.Add(DEFINE.TAR_L);
                }
                else if (nType == DEFINE.RIGHT_ALL)
                {
                    nPatNum.Add(DEFINE.OBJ_R);
                    nPatNum.Add(DEFINE.TAR_R);
                }
                else if (nType == DEFINE.OBJTAR_ALL)
                {
                    if (m_AlignName == "LOAD_ALIGN")
                    {
                        nPatNum.Add(DEFINE.OBJ_L);
                        nPatNum.Add(DEFINE.OBJ_R);
                        nPatNum.Add(DEFINE.TAR_L);
                    }
                    else
                    {
                        nPatNum.Add(DEFINE.OBJ_L);
                        nPatNum.Add(DEFINE.OBJ_R);
                        nPatNum.Add(DEFINE.TAR_L);
                        nPatNum.Add(DEFINE.TAR_R);
                    }
                }
                else
                {
                    nPatNum.Add(nType);
                }
            }
            private bool AlignInspection(int nType)
            {

                int seq = 0;
                bool ret = true;
                bool LoopFlag = true;

                List<int> nPatNum = new List<int>();
                bool[] CALI_ret = new bool[Main.DEFINE.CALIPER_MAX];
                bool[] BLOB_ret = new bool[Main.DEFINE.BLOB_CNT_MAX];
                bool[] Rusult_ret = new bool[2];

                double[,] EdgePos = new double[2, 2];
                double[,] TempPos = new double[2, 2];
                double[,] nLength = new double[2, 2];
                string[] nMessage = new string[2];
                string LogMsg = "";
                bool nMarkUseRet = new bool();
                nMarkUseRet = true;

                double[] nLengthResult = new double[2];

                //                 int m_FBDToolNum = 0;
                //                 string m_FBDTool = "";


                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:

                            //--------------------------------------------------------------------------------------------------------------------------------
                            m_StageX = 0;
                            m_StageY = 0;
                            m_StageT = 0;
                            m_Message.Clear();
                            //--------------------------------------------------------------------------------------------------------------------------------

                            if (Main.DEFINE.OPEN_F) //&& !machine.Inspection_Onf
                            {
                                Main.vision.CogImgTool[PAT[m_PatTagNo, nType].m_CamNo].Run();
                                vision.CogCamBuf[PAT[m_PatTagNo, nType].m_CamNo] = vision.CogImgTool[PAT[m_PatTagNo, nType].m_CamNo].OutputImage as ICogImage;
                            }
                            nPatNum.Add(nType);
                            // 
                            //                             m_FBDToolNum = (PLCDataTag.RData[ALIGN_UNIT_ADDR + +Main.DEFINE.FBD_TOOL_NUM]);
                            //                             m_FBDTool = m_FBDToolNum.ToString() + " FBD_TOOL ";

                            nMessage[DEFINE.WIDTH_] += "Width : SPEC:" + m_Standard[DEFINE.WIDTH_].ToString("0") + " ,";
                            nMessage[DEFINE.HEIGHT] += "Height: SPEC:" + m_Standard[DEFINE.HEIGHT].ToString("0") + " ,";
                            seq++;
                            break;

                        case 1: //1차 CALIPER 검사 
                            CALISearch(nPatNum);
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)
                                {
                                    if (!PAT[m_PatTagNo, nPatNum[i]].SearchResult)
                                    {
                                        nMarkUseRet = false;
                                    }
                                }
                            }
                            if (!nMarkUseRet)
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            for (int i = 0; i < Main.DEFINE.CALIPER_MAX; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[0]].CaliperPara[i].m_UseCheck)
                                {
                                    CALI_ret[i] = PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].SearchResult;
                                    if (!CALI_ret[i]) ret = false;
                                }

                                if (CALI_ret[i])
                                {
                                    PAT[m_PatTagNo, nPatNum[0]].V2R(PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].PixelPosX[DEFINE.FIRST_, DEFINE.XPOS], PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].PixelPosY[DEFINE.FIRST_, DEFINE.YPOS]
                                        , ref TempPos[DEFINE.FIRST_, DEFINE.XPOS], ref TempPos[DEFINE.FIRST_, DEFINE.YPOS]);
                                    PAT[m_PatTagNo, nPatNum[0]].V2R(PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].PixelPosX[DEFINE.SECOND, DEFINE.XPOS], PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].PixelPosY[DEFINE.SECOND, DEFINE.YPOS]
                                        , ref TempPos[DEFINE.SECOND, DEFINE.XPOS], ref TempPos[DEFINE.SECOND, DEFINE.YPOS]);

                                    EdgePos[DEFINE.FIRST_, i] = TempPos[DEFINE.FIRST_, i];
                                    EdgePos[DEFINE.SECOND, i] = TempPos[DEFINE.SECOND, i];
                                }
                                if (!CALI_ret[i]) nMessage[i] += "Edge Search NG";

                                nLengthResult[i] = nLength[DEFINE.EDGE, i] = (long)Math.Abs(EdgePos[DEFINE.SECOND, i] - EdgePos[DEFINE.FIRST_, i]);

                                if ((nLength[DEFINE.EDGE, i] > m_Standard[i] || !CALI_ret[i] || nLength[DEFINE.EDGE, i] == 0)/* && m_Standard[i] != 0*/)
                                {
                                    Rusult_ret[i] = false;
                                    if (nLength[DEFINE.EDGE, i] != 0) nMessage[i] += "Edge Length NG";
                                }
                                else
                                {
                                    Rusult_ret[i] = true;
                                    nMessage[i] += "Edge Length OK";
                                }
                                nLengthResult[i] = nLength[DEFINE.EDGE, i] *= PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].m_PlusMinus;
                                nMessage[i] += " ,L:" + nLength[DEFINE.EDGE, i].ToString("0.0") + " , ";
                            }

                            seq++;
                            break;


                        case 2: //2차 BLOB 검사 

                            if (true)
                            {
                                if (!Rusult_ret[DEFINE.WIDTH_] || !Rusult_ret[DEFINE.HEIGHT])
                                {
                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (!Rusult_ret[i])
                                        {
                                            PAT[m_PatTagNo, nPatNum[0]].BlobPara[i].m_UseCheck = true;
                                        }
                                        else if (Rusult_ret[i])
                                        {
                                            PAT[m_PatTagNo, nPatNum[0]].BlobPara[i].m_UseCheck = false;
                                        }
                                    }

                                    BLOBSearch(nPatNum, "ALIGN_INSPECTION");

                                    for (int i = 0; i < 2; i++)
                                    {
                                        if (PAT[m_PatTagNo, nPatNum[0]].BlobPara[i].m_UseCheck)
                                        {
                                            BLOB_ret[i] = PAT[m_PatTagNo, nPatNum[0]].BlobResults[i].SearchResult;
                                        }

                                        if (!Rusult_ret[i])
                                        {
                                            if (!BLOB_ret[i])
                                            {
                                                PAT[m_PatTagNo, nPatNum[0]].V2R(PAT[m_PatTagNo, nPatNum[0]].BlobResults[i].VertexResults[(i * 2), DEFINE.XPOS], PAT[m_PatTagNo
                                                , nPatNum[0]].BlobResults[i].VertexResults[(i * 2), DEFINE.YPOS]
                                                , ref TempPos[DEFINE.FIRST_, DEFINE.XPOS], ref TempPos[DEFINE.FIRST_, DEFINE.YPOS]);

                                                PAT[m_PatTagNo, nPatNum[0]].V2R(PAT[m_PatTagNo, nPatNum[0]].BlobResults[i].VertexResults[(i * 2) + 1, DEFINE.XPOS], PAT[m_PatTagNo
                                                , nPatNum[0]].BlobResults[i].VertexResults[(i * 2) + 1, DEFINE.YPOS]
                                                , ref TempPos[DEFINE.SECOND, DEFINE.XPOS], ref TempPos[DEFINE.SECOND, DEFINE.YPOS]);

                                                EdgePos[DEFINE.FIRST_, i] = TempPos[DEFINE.FIRST_, i];
                                                EdgePos[DEFINE.SECOND, i] = TempPos[DEFINE.SECOND, i];

                                                nLengthResult[i] = nLength[DEFINE.BLOB, i] = (long)Math.Abs(EdgePos[DEFINE.SECOND, i] - EdgePos[DEFINE.FIRST_, i]);
                                                if ((nLength[DEFINE.BLOB, i] > m_Standard[i] || nLength[DEFINE.BLOB, i] == 0) && m_Standard[i] != 0)
                                                {
                                                    Rusult_ret[i] = false;
                                                    nMessage[i] += "Blob Length NG";
                                                }
                                                else
                                                {
                                                    Rusult_ret[i] = true;
                                                    nMessage[i] += "Blob Length OK";
                                                }
                                            }
                                            else
                                            {
                                                Rusult_ret[i] = true;
                                                nLengthResult[i] = nLength[DEFINE.BLOB, i] = 0;
                                                nMessage[i] += "Blob OK";
                                            }
                                            nLengthResult[i] = nLength[DEFINE.BLOB, i] *= PAT[m_PatTagNo, nPatNum[0]].BlobResults[i].m_PlusMinus;
                                            nMessage[i] = nMessage[i] + " ,L: " + nLength[DEFINE.BLOB, i].ToString("0.0");
                                        }

                                    }
                                }
                            }
                            //                             m_Message[DEFINE.WIDTH_] = nMessage[DEFINE.WIDTH_];
                            //                             m_Message[DEFINE.HEIGHT] = nMessage[DEFINE.HEIGHT];

                            m_Message.Add(nMessage[DEFINE.WIDTH_]);
                            m_Message.Add(nMessage[DEFINE.HEIGHT]);
                            seq++;
                            break;

                        case 3:
                            //                             for (int i = 0; i < nMessage.Length; i++)
                            //                             {
                            //                                 LogMsg = nMessage[i];
                            //                                 LogdataDisplay(nMessage[i], true);
                            //                             }
                            seq++;
                            break;

                        case 4:
                            if (Rusult_ret[DEFINE.WIDTH_] && Rusult_ret[DEFINE.HEIGHT])
                            {
                                ret = true;
                                seq = SEQ.COMPLET_SEQ;
                            }
                            else
                            {
                                if (!Rusult_ret[DEFINE.WIDTH_] && !Rusult_ret[DEFINE.HEIGHT])
                                {
                                    m_errSts = ERR.INSPEC + DEFINE.OBJ_ALL;
                                }
                                else if (!Rusult_ret[DEFINE.WIDTH_])
                                {
                                    m_errSts = ERR.INSPEC + DEFINE.WIDTH_;
                                }
                                else if (!Rusult_ret[DEFINE.HEIGHT])
                                {
                                    m_errSts = ERR.INSPEC + DEFINE.HEIGHT;
                                }
                                ret = false;
                                seq = SEQ.ERROR_SEQ;
                            }
                            m_AOI_Length = nLength;
                            PAT[m_PatTagNo, nPatNum[0]].m_Length[DEFINE.X] = nLengthResult[DEFINE.WIDTH_];
                            PAT[m_PatTagNo, nPatNum[0]].m_Length[DEFINE.Y] = nLengthResult[DEFINE.HEIGHT];

                            if (nType == Main.DEFINE.OBJ_L)
                            {
                                string nTempMSG = "";
                                double[] TempAlign = new double[2];
                                m_AOI_ExtensionX = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.X] - PAT[m_PatTagNo, Main.DEFINE.OBJ_L].m_Length[DEFINE.X];

                                TempAlign[DEFINE.X] = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.X] + PAT[m_PatTagNo, Main.DEFINE.OBJ_L].m_Length[DEFINE.X];
                                TempAlign[DEFINE.Y] = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.Y] + PAT[m_PatTagNo, Main.DEFINE.OBJ_L].m_Length[DEFINE.Y];
                                TempAlign[DEFINE.X] = (TempAlign[DEFINE.X] / 2) * -1;
                                TempAlign[DEFINE.Y] = (TempAlign[DEFINE.Y] / 2) * -1;

                                nTempMSG = "Extension :" + m_AOI_ExtensionX.ToString();

                                nTempMSG = "Extension :" + m_AOI_ExtensionX.ToString() + " , CX:" + TempAlign[DEFINE.X].ToString() + " ,CY:" + TempAlign[DEFINE.Y].ToString();

                                if (Math.Abs(m_AOI_ExtensionX) > m_Standard[Main.DEFINE.EXTENSION] && m_Standard[Main.DEFINE.EXTENSION] != 0)
                                {
                                    ret = false;
                                    seq = SEQ.ERROR_SEQ;
                                    m_errSts = ERR.INSPEC + DEFINE.EXTENSION;
                                    nTempMSG = "Extension : SPEC:" + m_Standard[DEFINE.EXTENSION].ToString("0") + " ," + "NG ,L:" + m_AOI_ExtensionX.ToString();
                                }
                                else
                                {
                                    if (m_Standard[Main.DEFINE.EXTENSION] != 0)
                                        nTempMSG = "Extension : SPEC:" + m_Standard[DEFINE.EXTENSION].ToString("0") + " ," + "OK ,L:" + m_AOI_ExtensionX.ToString();
                                }
                                m_Message.Add(nTempMSG);



                                LogdataDisplay_Length("L " + nTempMSG, true, true);
                                InputTABdata(Main.DEFINE.OBJ_L, m_AOI_ExtensionX);
                            }

                            if (nType == Main.DEFINE.OBJ_R)
                            {
                                string nTempMSG = "";
                                double[] TempAlign = new double[2];
                                m_AOI_ExtensionX = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.X] - PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.X];

                                TempAlign[DEFINE.X] = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.X] + PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.X];
                                TempAlign[DEFINE.Y] = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.Y] + PAT[m_PatTagNo, Main.DEFINE.OBJ_R].m_Length[DEFINE.Y];
                                TempAlign[DEFINE.X] = (TempAlign[DEFINE.X] / 2) * -1;
                                TempAlign[DEFINE.Y] = (TempAlign[DEFINE.Y] / 2) * -1;

                                //   nTempMSG = "Extension :" + m_AOI_ExtensionX.ToString();
                                //   nTempMSG = "Extension :" + m_AOI_ExtensionX.ToString() + " , CX:" + TempAlign[DEFINE.X].ToString() + " , CY:" + TempAlign[DEFINE.Y].ToString();

                                if (Math.Abs(m_AOI_ExtensionX) > m_Standard[Main.DEFINE.EXTENSION] && m_Standard[Main.DEFINE.EXTENSION] != 0)
                                {
                                    ret = false;
                                    seq = SEQ.ERROR_SEQ;
                                    m_errSts = ERR.INSPEC + DEFINE.EXTENSION;
                                    nTempMSG = "Extension : SPEC:" + m_Standard[DEFINE.EXTENSION].ToString("0") + " ," + "NG ,L:" + m_AOI_ExtensionX.ToString();
                                }
                                else
                                {
                                    if (m_Standard[Main.DEFINE.EXTENSION] != 0)
                                        nTempMSG = "Extension : SPEC:" + m_Standard[DEFINE.EXTENSION].ToString("0") + " ," + "OK ,L:" + m_AOI_ExtensionX.ToString();
                                }
                                m_Message.Add(nTempMSG);
                                //     LogdataDisplay_Length("R " +nTempMSG, true,true);
                                InputTABdata(Main.DEFINE.OBJ_L, m_AOI_ExtensionX);
                            }
                            else
                            {
                                m_AOI_ExtensionX = 0;
                            }

                            for (int i = 0; i < m_Message.Count; i++)
                            {
                                LogMsg = m_Message[i];
                                LogdataDisplay(m_Message[i], true);
                            }

                            string m_PatName = "";
                            if (nType == Main.DEFINE.OBJ_L) m_PatName = "_L";
                            if (nType == Main.DEFINE.OBJ_R) m_PatName = "_R";
                            InputAligndata(m_Cell_ID + m_PatName, m_AOI_Length, nLengthResult, nMessage, ret);
                            m_StageX = (long)nLengthResult[DEFINE.WIDTH_];
                            m_StageY = (long)nLengthResult[DEFINE.HEIGHT];
                            m_StageT = (long)m_AOI_ExtensionX;


                            break;

                        case SEQ.ERROR_SEQ:
                            ret = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            ret = true;
                            LoopFlag = false;
                            break;
                    }
                }

                DrawSet(nPatNum, nType);
                return ret;
            }
            private bool CalPosBlob(int nType)
            {
                int seq = 0;    //시퀀스의 동작 순서를 정하기 위한 변수, 처음에 0으로 초기화하여 0번부터 시작할 수 있게 한다.
                bool ret = false; // 리턴인듯...
                bool LoopFlag = true; //시퀀스의 전반적인 동작을 위한(반복적으로 동작시키기 위한) 부울값

                List<int> nPatNum = new List<int>(); //배열 생성
                bool[] CALI_ret = new bool[Main.DEFINE.CALIPER_MAX];    // 캘리퍼 갯수만큼 배열 생성
                bool[] BLOB_ret = new bool[Main.DEFINE.BLOB_CNT_MAX];   // 블랍 갯수만큼 배열 생성
                bool[] Rusult_ret = new bool[2];                        // 결과(?) 저장용 배열 생성

                double[] EdgePos = new double[2];
                double[,] TempPos = new double[2, 2];
                double[,] nLength = new double[2, 2];
                string[] nMessage = new string[2];

                bool nMarkUseRet = new bool();
                nMarkUseRet = true;

                double[] nLengthResult = new double[2];


                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:

                            //--------------------------------------------------------------------------------------------------------------------------------
                            //--------------------------------------------------------------------------------------------------------------------------------

                            if (Main.DEFINE.OPEN_F) // 파일 오픈 모드
                            {
                                Main.vision.CogImgTool[PAT[m_PatTagNo, nType].m_CamNo].Run();   // 이미지 불러오는 곳
                                vision.CogCamBuf[PAT[m_PatTagNo, nType].m_CamNo] = vision.CogImgTool[PAT[m_PatTagNo, nType].m_CamNo].OutputImage as ICogImage;  // 불러온 이미지를 버퍼에 저장
                            }
                            if (m_Cmd != CMD.OBJ_CAL_POS_LEFT && m_Cmd == CMD.OBJ_CAL_POS_RIGHT)
                                AddPattern(nType, ref nPatNum);
                            else
                                nPatNum.Add(nType);
                            seq++;
                            break;

                        case 1: //1차 CALIPER 검사 
                            //if (CALISearch(nPatNum))
                            //{
                            CALISearch(nPatNum);
                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[i]].Caliper_MarkUse)    // 캘리퍼가 마크 기준으로 움직이는지 확인
                                {
                                    if (!PAT[m_PatTagNo, nPatNum[i]].SearchResult)  // 마크 기준 결과
                                    {
                                        nMarkUseRet = false;
                                    }
                                }
                            }
                            if (!nMarkUseRet)
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            for (int i = 0; i < Main.DEFINE.CALIPER_MAX; i++)
                            {
                                if (PAT[m_PatTagNo, nPatNum[0]].CaliperPara[i].m_UseCheck)
                                {
                                    CALI_ret[i] = PAT[m_PatTagNo, nPatNum[0]].CaliResults[i].SearchResult;
                                    if (CALI_ret[i]) ret = true;
                                }
                                if (CALI_ret[i])
                                {
                                    PAT[m_PatTagNo, nPatNum[0]].Caliper_MarkPos[0] = PAT[m_PatTagNo, nPatNum[0]].CaliResults[0].Pixel[Main.DEFINE.X];
                                    PAT[m_PatTagNo, nPatNum[0]].Caliper_MarkPos[1] = PAT[m_PatTagNo, nPatNum[0]].CaliResults[1].Pixel[Main.DEFINE.Y];
                                }
                            }
                            //                             }
                            //                             else
                            //                             {
                            //                                 seq = SEQ.ERROR_SEQ;
                            //                             }
                            seq++;
                            break;
                        case 2:
                            if ((m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2"))
                            {
                                if (m_Cmd != CMD.OBJ_CAL_POS_LEFT && m_Cmd == CMD.OBJ_CAL_POS_RIGHT)
                                {
                                    CalierToRobot();
                                    m_StageX = (long)PAT[m_PatTagNo, nType].m_RobotPosX;
                                    m_StageY = (long)PAT[m_PatTagNo, nType].m_RobotPosY;
                                    m_StageT = 0;
                                }
                                else if (m_Cmd == CMD.OBJ_CAL_POS_LEFT && m_Cmd != CMD.OBJ_CAL_POS_RIGHT)
                                {
                                    CalierToRobot();
                                    Processing(0, false, true, false);
                                }
                            }
                            seq++;
                            break;
                        case 3:
                            if (m_Cmd == CMD.OBJ_CAL_POS_LEFT && m_Cmd != CMD.OBJ_CAL_POS_RIGHT)
                            {
                                if (!LengthCheck(DEFINE.OBJ_ALL))
                                {
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                                if (m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2")
                                {
                                    long[] nACFLength = new long[1];
                                    nACFLength[0] = (long)m_OBJ_Mea_Dis;
                                    InputTABdata(DEFINE.OBJ_L, m_OBJ_Mea_Dis);
                                    WriteTABLength(nACFLength);
                                }
                                InputAligndata(m_StageX, m_StageY, m_StageT);
                            }
                            seq++;
                            break;

                        case 4: //2차 BLOB 검사 
                            if (ret)
                            {
                                for (int i = 0; i < Main.DEFINE.BLOB_CNT_MAX; i++)
                                {
                                    if (PAT[m_PatTagNo, nPatNum[0]].BlobPara[i].m_UseCheck)
                                    {


                                    }
                                }

                                if (!BLOBSearch(nPatNum)) ret = false;
                            }
                            if (!ret)
                                seq = SEQ.ERROR_SEQ;
                            else
                                seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.ERROR_SEQ:
                            ret = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            ret = true;
                            LoopFlag = false;
                            break;
                    }
                }

                DrawSet(nPatNum, nType);
                return ret;
            }
            private bool CircleInspection(int nType)
            {

                int seq = 0;
                bool ret = true;
                bool LoopFlag = true;
                List<int> nPatNum = new List<int>();

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            if (Main.DEFINE.OPEN_F)
                            {
                                Main.vision.CogImgTool[PAT[m_PatTagNo, nType].m_CamNo].Run();
                                vision.CogCamBuf[PAT[m_PatTagNo, nType].m_CamNo] = vision.CogImgTool[PAT[m_PatTagNo, nType].m_CamNo].OutputImage as ICogImage;
                            }
                            nPatNum.Add(nType);
                            seq++;
                            break;

                        case 1:
                            if (!CircleSearch(nType))
                            {
                                ret = false;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq++;
                            break;


                        case 2: //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            double dx = 0, dy = 0, t2 = 0, dt = 0, t1 = 0;
                            double TempRobotPosX = 0;
                            double TempRobotPosY = 0;

                            m_CamDistX1 = m_OBJ_Distance;
                            m_CamDistY1 = m_TAR_Distance;

                            dx = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.VCM].m_RobotPosX - PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.VCM].m_RobotPosX + m_CamDistX1;
                            dy = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.VCM].m_RobotPosY - PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.VCM].m_RobotPosY + m_CamDistY1;
                            t1 = Math.Atan(dy / dx);

                            dx = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.FPC].m_RobotPosX - PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.FPC].m_RobotPosX + m_CamDistX1;
                            dy = PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.FPC].m_RobotPosY - PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.FPC].m_RobotPosY + m_CamDistY1;
                            t2 = Math.Atan(dy / dx);

                            TempRobotPosX = ((PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.VCM].m_RobotPosX + PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.VCM].m_RobotPosX) -
                                            (PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.FPC].m_RobotPosX + PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.FPC].m_RobotPosX)) / 2;

                            TempRobotPosY = ((PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.VCM].m_RobotPosY + PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.VCM].m_RobotPosY) -
                                            (PAT[m_PatTagNo, Main.DEFINE.OBJ_L].CircleResults[Main.DEFINE.FPC].m_RobotPosY + PAT[m_PatTagNo, Main.DEFINE.OBJ_R].CircleResults[Main.DEFINE.FPC].m_RobotPosY)) / 2;

                            dt = (t1 - t2);

                            m_StageX = (long)TempRobotPosX;
                            m_StageY = (long)TempRobotPosY;
                            m_StageT = (long)((dt * 180.0 / DEFINE.PI) * 1000.0 * m_DirT);
                            seq++;
                            break;

                        case 3:
                            InputAligndata(m_StageX, m_StageY, m_StageT);
                            seq++;
                            break;

                        case 4:
                            ret = true;
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.ERROR_SEQ:
                            ret = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            ret = true;
                            LoopFlag = false;
                            break;
                    }
                }

                DrawSet(nPatNum, nType);
                return ret;
            }
            private void CalPixelData(int nPatNum1, double[,] nPixelPos_1)
            {

                double[] nPixel_X_Gap = new double[2];
                double[] nPixel_Y_Gap = new double[2];

                string LogMsg, CalName;

                CalName = "";
                if (nPatNum1 > 3) CalName = "W";
                else CalName = "P";

                LogMsg = "";
                nPixel_X_Gap[0] = nPixel_X_Gap[1] = nPixel_Y_Gap[0] = nPixel_Y_Gap[1] = 0;
                for (int i = 0; i < DEFINE.CAL_POINT; i++)
                {
                    LogMsg = CalName + "_PIXEL_XL_" + i.ToString("0: ") + "," + nPixelPos_1[i, DEFINE.XPOS].ToString("0.00") + ", " + "PIXEL_YL_" + i.ToString("0.00") + ", " + nPixelPos_1[i, DEFINE.YPOS].ToString("0.00");
                    LogdataDisplay(LogMsg, true);
                }

                nPixel_X_Gap[0] += nPixelPos_1[1, 0] - nPixelPos_1[0, 0];
                nPixel_X_Gap[0] += nPixelPos_1[4, 0] - nPixelPos_1[3, 0];
                nPixel_X_Gap[0] += nPixelPos_1[7, 0] - nPixelPos_1[6, 0];
                nPixel_X_Gap[0] = nPixel_X_Gap[0] / 3;

                nPixel_X_Gap[1] += nPixelPos_1[2, 0] - nPixelPos_1[1, 0];
                nPixel_X_Gap[1] += nPixelPos_1[5, 0] - nPixelPos_1[4, 0];
                nPixel_X_Gap[1] += nPixelPos_1[8, 0] - nPixelPos_1[7, 0];
                nPixel_X_Gap[1] = nPixel_X_Gap[1] / 3;

                nPixel_Y_Gap[0] += nPixelPos_1[3, 1] - nPixelPos_1[0, 1];
                nPixel_Y_Gap[0] += nPixelPos_1[4, 1] - nPixelPos_1[1, 1];
                nPixel_Y_Gap[0] += nPixelPos_1[5, 1] - nPixelPos_1[2, 1];
                nPixel_Y_Gap[0] = nPixel_Y_Gap[0] / 3;

                nPixel_Y_Gap[1] += nPixelPos_1[6, 1] - nPixelPos_1[3, 1];
                nPixel_Y_Gap[1] += nPixelPos_1[7, 1] - nPixelPos_1[4, 1];
                nPixel_Y_Gap[1] += nPixelPos_1[8, 1] - nPixelPos_1[5, 1];
                nPixel_Y_Gap[1] = nPixel_Y_Gap[1] / 3;

                LogMsg = "";
                LogMsg = CalName + "_PIXEL_XL_1_GAP" + "," + nPixel_X_Gap[0].ToString("0.00") + ", " + "PIXEL_XL__2_GAP" + ", " + nPixel_X_Gap[1].ToString("0.00") + ", " + "Resolution X:," + PAT[m_PatTagNo, nPatNum1].m_CalX[0].ToString("0.0000");
                LogdataDisplay(LogMsg, true);
                LogMsg = "";
                LogMsg = CalName + "_PIXEL_YL_1_GAP" + "," + nPixel_Y_Gap[0].ToString("0.00") + ", " + "PIXEL_YL__2_GAP" + ", " + nPixel_Y_Gap[1].ToString("0.00") + ", " + "Resolution Y:," + PAT[m_PatTagNo, nPatNum1].m_CalY[0].ToString("0.0000");
                LogdataDisplay(LogMsg, true);
            }
#region LAMI
            private bool InspectCheck(int nSearchPos) //nSearchType -> Front Rear 0,2
            {
                if (!machine.Inspection_Onf) return true;

                string LogMsg = "";
                int seq = 0;
                bool LoopFlag = true;
                bool[] nRet = new bool[4];
                bool ret = true;

                double[] m_TempGap = new double[8];
                double[] m_ResultGap = new double[8];

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            for (int i = 0; i < 2; i++)
                            {
                                m_TempGap[i * 2 + DEFINE.WIDTH_] = (Math.Abs(PAT[m_PatTagNo, DEFINE.PANEL_ + i + nSearchPos].m_RobotPosX - PAT[m_PatTagNo, DEFINE.WINDOW + i + nSearchPos].m_RobotPosX)) * 1000;  // spec 값이 um 이고 robot 은 mm 단위.
                                m_TempGap[i * 2 + DEFINE.HEIGHT] = (Math.Abs(PAT[m_PatTagNo, DEFINE.PANEL_ + i + nSearchPos].m_RobotPosY - PAT[m_PatTagNo, DEFINE.WINDOW + i + nSearchPos].m_RobotPosY)) * 1000;
                            }
                            seq++;
                            break;

                        case 1:
                            for (int i = 0; i < 2; i++)
                            {
                                m_ResultGap[i * 2 + DEFINE.WIDTH_] = Math.Abs(m_TempGap[i * 2 + DEFINE.WIDTH_] - m_InspectionSpec[i * 2 + DEFINE.WIDTH_ + nSearchPos * 2]);
                                m_ResultGap[i * 2 + DEFINE.HEIGHT] = Math.Abs(m_TempGap[i * 2 + DEFINE.HEIGHT] - m_InspectionSpec[i * 2 + DEFINE.HEIGHT + nSearchPos * 2]);

                                if (m_ResultGap[i * 2 + DEFINE.WIDTH_] > m_Inspec_Tolerance[DEFINE.WIDTH_]) { nRet[i * 2 + DEFINE.WIDTH_] = false; } else { nRet[i * 2 + DEFINE.WIDTH_] = true; }
                                if (m_ResultGap[i * 2 + DEFINE.HEIGHT] > m_Inspec_Tolerance[DEFINE.HEIGHT]) { nRet[i * 2 + DEFINE.HEIGHT] = false; } else { nRet[i * 2 + DEFINE.HEIGHT] = true; }
                            }
                            seq++;
                            break;

                        case 2:
                            for (int i = 0; i < 2; i++)
                            {
                                m_InspecResult[(nSearchPos + i) * 2 + DEFINE.WIDTH_] = m_TempGap[i * 2 + DEFINE.WIDTH_];
                                m_InspecResult[(nSearchPos + i) * 2 + DEFINE.HEIGHT] = m_TempGap[i * 2 + DEFINE.HEIGHT];
                            }
                            seq++;
                            break;

                        case 3:
                            LogMsg = "";
                            LogMsg = LogMsg + "P1:" + nRet[DEFINE.LCHECK_L1].ToString() + ", " + "P2:" + nRet[DEFINE.LCHECK_L2].ToString() + ", " + "P3:" + nRet[DEFINE.LCHECK_L3].ToString() + ", " + "P4:" + nRet[DEFINE.LCHECK_L4].ToString();
                            LogdataDisplay(LogMsg, false);
                            seq++;
                            break;

                        case 4:
                            LogMsg = "";
                            LogMsg = LogMsg + "PLX:" + m_TempGap[DEFINE.LCHECK_L1].ToString("000") + ", " + "PLY:" + m_TempGap[DEFINE.LCHECK_L2].ToString("000") + ", " +
                                              "PRX:" + m_TempGap[DEFINE.LCHECK_L3].ToString("000") + ", " + "PRY:" + m_TempGap[DEFINE.LCHECK_L4].ToString("000");
                            LogdataDisplay(LogMsg, false);
                            if (m_AlignNo > 2)
                            {
                                //    InspecGridDataDisplay(m_TempGap, nSearchPos);

                                if (nSearchPos == DEFINE.REAR_)
                                {
                                    //                                     LogMsg = "";
                                    //                                     LogMsg = P_Cell_ID + "\t" + W_Cell_ID + "\t" 
                                    //                                              + m_InspecResult[DEFINE.FRONT_L__Width].ToString("000") + "\t" + m_InspecResult[DEFINE.FRONT_L_Height].ToString("000") + "\t"
                                    //                                              + m_InspecResult[DEFINE.FRONT_R__Width].ToString("000") + "\t" + m_InspecResult[DEFINE.FRONT_R_Height].ToString("000") + "\t"
                                    //                                              + m_InspecResult[DEFINE.REAR_L__Width].ToString("000") + "\t" + m_InspecResult[DEFINE.REAR_L_Height].ToString("000") + "\t"
                                    //                                              + m_InspecResult[DEFINE.REAR_R__Width].ToString("000") + "\t" + m_InspecResult[DEFINE.REAR_R_Height].ToString("000");
                                    //                                     LogDataSave(LogMsg, true ,DEFINE.INSPECTION);
                                }
                            }
                            seq++;
                            break;

                        case 5:
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.ERROR_SEQ:
                            ret = false;
                            LoopFlag = false;
                            break;

                        case SEQ.COMPLET_SEQ:
                            ret = true;
                            LoopFlag = false;
                            break;
                    }

                }
                return ret;
            }


            private bool nLengthCheck(double nDistence_X, double nDistence_Y, double nStandard_Distance, double nTolerance, ref double nDistanceRet)
            {
                bool ret = false;

                double nDistance = Math.Sqrt(Math.Pow(nDistence_X, 2) + Math.Pow(nDistence_Y, 2));

                if (Math.Abs(nDistance - nStandard_Distance) <= nTolerance / 1000)
                    ret = true;

                nDistanceRet = nDistance;
                return ret;
            }
            private bool LengthCheck(double nMeasured_Distance, double nStandard_Distance, double nTolerance, ref double nDistanceRet)
            {
                bool ret = false;

                string LogMsg = "";

                long nGapLength = 0;

                double nDistance = nMeasured_Distance;

                nGapLength = (long)Math.Abs(nDistance - nStandard_Distance);
                if (nGapLength <= nTolerance)
                    ret = true;
                if (nTolerance == 0)
                {
                    ret = true;
                }
                nDistanceRet = nDistance;
                LogMsg = "LENGTH RESULT: " + ret.ToString().ToUpper() + ", " + "Standard DISTANCE: " + nStandard_Distance.ToString("0") + ", Measured DISTANCE: " + nDistanceRet.ToString("0") + ", " + "GAP_SPEC: " + nTolerance.ToString() + ", " + "GAP: " + nGapLength.ToString();
                LogdataDisplay(LogMsg, false);

                return ret;
            }
#endregion
            private void DrawSet(List<int> nPatNum, int nType)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool[] ret = new bool[DEFINE.Pattern_Max];

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            bool nRet = true;

                            for (int i = 0; i < nPatNum.Count; i++)
                            {
                                if (nType == Main.DEFINE.OBJTAR_ALL && (m_AlignType[m_PatTagNo] == Main.DEFINE.M_2CAM2SHOT)) //|| m_AlignType == Main.DEFINE.M_2CAM1SHOT
                                {
                                    nRet = true;
                                }
                                else
                                {
                                    if (PAT[m_PatTagNo, nPatNum[i]].m_DisNo != PAT[m_PatTagNo, nPatNum[nPatNum.Count - 1]].m_DisNo)
                                        nRet = false;
                                }
                            }

                            if (nPatNum.Count > 1 && nRet)
                            {
                                DrawAll_Pat[m_PatTagNo] = nType;
                            }
                            else
                            {
                                for (int i = 0; i < nPatNum.Count; i++)
                                {
                                    PAT[m_PatTagNo, nPatNum[i]].m_DrawPat = 1;
                                }
                            }
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ:
                            LoopFlag = false;
                            break;
                    }
                }
            }
            private void DrawSet_CAL(int SearchMode)
            {
                PAT[m_PatTagNo, SearchMode].m_DrawPat_CAL = 1;
            }

            private void GetMovePosition(ref double[,] Robot)
            {
                double PitchX = 0;
                double PitchY = 0;

                PitchX = m_Cal_MOVE_X;
                PitchY = m_Cal_MOVE_Y;

                //       if (DEFINE.PROGRAM_TYPE == "FPC_TRAY") PitchY = m_Cal_MOVE_Y * -1;   // 카메라가 움직이니깐 좌표 반대로.

                if (PAT[m_PatTagNo, 0].CALMATRIX_NOTUSE)
                {
                    if (m_AlignName == "LOAD_ALIGN")
                    {
                        Robot[0, DEFINE.XPOS] = -PitchX;
                        Robot[0, DEFINE.YPOS] = 0;

                        Robot[1, DEFINE.XPOS] = PitchX;
                        Robot[1, DEFINE.YPOS] = 0;

                        Robot[2, DEFINE.XPOS] = 0;
                        Robot[2, DEFINE.YPOS] = PitchY;

                        Robot[3, DEFINE.XPOS] = 0;
                        Robot[3, DEFINE.YPOS] = -PitchY;
                    }
                    else if (m_AlignName == "FBD_1" || m_AlignName == "FBD_2" || m_AlignName == "FBD_3" || m_AlignName == "FBD_4"
                        || m_AlignName == "CHIP_PRE" || m_AlignName == "REEL_ALIGN_1" || m_AlignName == "REEL_ALIGN_2")
                    {
                        Robot[0, DEFINE.XPOS] = 0;
                        Robot[0, DEFINE.YPOS] = PitchY;

                        Robot[1, DEFINE.XPOS] = 0;
                        Robot[1, DEFINE.YPOS] = -PitchY * 2;
                    }
                    else
                    {
                        Robot[0, DEFINE.XPOS] = -PitchX;
                        Robot[0, DEFINE.YPOS] = 0;

                        Robot[1, DEFINE.XPOS] = PitchX * 2;
                        Robot[1, DEFINE.YPOS] = 0;

                        Robot[2, DEFINE.XPOS] = 0;
                        Robot[2, DEFINE.YPOS] = PitchY;

                        Robot[3, DEFINE.XPOS] = 0;
                        Robot[3, DEFINE.YPOS] = -PitchY * 2;
                    }
                    return;
                }
                if (Main.DEFINE.NON_STANDARD) PitchX = PitchX * -1;

                if (m_AlignName == "LOAD_ALIGN" && DEFINE.PROGRAM_TYPE == "TFOF_PC1")
                {
                    for (int i = 0; i < Robot.GetLength(0); i++)
                    {
                        switch (i)
                        {
                            case 0:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 1:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 2:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 3:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 4:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 5:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 6:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 7:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 8:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;
                        }
                    }
                }
                else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                {
                    for (int i = 0; i < Robot.GetLength(0); i++)
                    {
                        switch (i)
                        {
                            case 0:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 1:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 2:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 3:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 4:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 5:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 6:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 7:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 8:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;
                        }
                    }
                }
                else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                {
                    for (int i = 0; i < Robot.GetLength(0); i++)
                    {
                        switch (i)
                        {
                            case 0:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 1:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 2:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 3:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 4:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 5:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 6:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 7:
                                Robot[i, DEFINE.XPOS] = 0;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 8:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < Robot.GetLength(0); i++)
                    {
                        switch (i)
                        {
                            case 0:
                                Robot[i, DEFINE.XPOS] = -PitchX;
                                Robot[i, DEFINE.YPOS] = PitchY;
                                break;

                            case 1:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 2:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 3:
                                Robot[i, DEFINE.XPOS] = -PitchX * 2;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 4:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 5:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 6:
                                Robot[i, DEFINE.XPOS] = -PitchX * 2;
                                Robot[i, DEFINE.YPOS] = -PitchY;
                                break;

                            case 7:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;

                            case 8:
                                Robot[i, DEFINE.XPOS] = PitchX;
                                Robot[i, DEFINE.YPOS] = 0;
                                break;
                        }
                    }
                }
            }
            private void GetRobotPosition(ref double[,] Robot)
            {
                double PitchX = 0;
                double PitchY = 0;

                PitchX = m_Cal_MOVE_X / 1000.0;
                PitchY = m_Cal_MOVE_Y / 1000.0;


                for (int i = 0; i < 9; i++)
                {
                    //-------------------X-------------------------
                    if (i % 3 == 0)
                    {
                        Robot[i, DEFINE.XPOS] = -PitchX;
                    }
                    else if (i % 3 == 1)
                    {
                        Robot[i, DEFINE.XPOS] = 0;
                    }
                    else if (i % 3 == 2)
                    {
                        Robot[i, DEFINE.XPOS] = PitchX;
                    }
                    //-------------------Y-------------------------
                    if (i < 3)
                    {
                        Robot[i, DEFINE.YPOS] = PitchY;
                    }
                    else if (i < 6)
                    {
                        Robot[i, DEFINE.YPOS] = 0;
                    }
                    else if (i < 9)
                    {
                        Robot[i, DEFINE.YPOS] = -PitchY;
                    }
                }
            }

            private bool Calibration(int nPatNum_, int CalMode)
            {
                bool ret = false;
                bool roop = true;

                int seq = 0;
                int nSleep = 1000;

                int nCalMovePos = 0;

                List<int> nPatNum = new List<int>();
                nPatNum.Add(nPatNum_);

                //-----------------X,Y-----------------------------------
                double[,] nPixelPos_1 = new double[DEFINE.CAL_POINT, 2];
                double[,] nPixelPos_2 = new double[DEFINE.CAL_POINT, 2];
                double[,] nRotbotPos = new double[DEFINE.CAL_POINT, 2];
                double[,] nRotMovePos = new double[DEFINE.CAL_POINT, 2];
                double nTemX = 0;
                double nTemY = 0;
                //------------------Theta---------------------------------
                double[] nRotMoveTheta = new double[DEFINE.CAL_POINT_THETA];
                //                 double[] nV2RobotPos1_X = new double[DEFINE.CAL_POINT_THETA];
                //                 double[] nV2RobotPos1_Y = new double[DEFINE.CAL_POINT_THETA];
                //                 double[] nV2RobotPos2_X = new double[DEFINE.CAL_POINT_THETA];
                //                 double[] nV2RobotPos2_Y = new double[DEFINE.CAL_POINT_THETA];
                //                 double[,] nPixelPosTheta_1 = new double[DEFINE.CAL_POINT_THETA, 2];
                //                 double[,] nPixelPosTheta_2 = new double[DEFINE.CAL_POINT_THETA, 2];
                //-----------------------------------------------------
                //------------------Theta CAL 3---------------------------------
                double[] nRotMoveThetaCAL3 = new double[DEFINE.CAL_POINT_THETA * 2];
                double[] nPixelPosT_Y = new double[DEFINE.CAL_POINT_THETA * 2];
                double[] nPixelPosT_X = new double[DEFINE.CAL_POINT_THETA * 2];
                int ThetaDIR = -1;
                //-----------------------ThetaCalUse----------------------
                double x1 = 0, x2 = 0, y1 = 0, y2 = 0, theta = 0, rep = 1;
                double alignX = 0, alignY = 0;
                double dx = 0, dy = 0, dt = 0;
                double TargetImgX = 0, TargetImgY = 0, ObjectImgX = 0, ObjectImgY = 0;
                int CalNo = m_PatTagNo;

                int Pos_L, Pos_R, Pos_T, Pos_B;
                int nCAL_POINT;
                long nBackUp_T1 = 0;

                if (PAT[m_PatTagNo, nPatNum[0]].CALMATRIX_NOTUSE)  // 모터 X,Y 중 하나만 있을때 계산 되는 위치.
                {
                    if (m_AlignName == "FBD_1" || m_AlignName == "FBD_2" || m_AlignName == "FBD_3" || m_AlignName == "FBD_4"
                        || m_AlignName == "CHIP_PRE" || m_AlignName == "REEL_ALIGN_1" || m_AlignName == "REEL_ALIGN_2")
                    {
                        Pos_L = 0; Pos_R = 1;
                        Pos_T = 0; Pos_B = 1;
                        nCAL_POINT = 2;
                    }
                    else
                    {
                        Pos_L = 0; Pos_R = 1;
                        Pos_T = 2; Pos_B = 3;
                        nCAL_POINT = 4;
                    }
                }
                else
                {
                    Pos_L = 3; Pos_R = 5;
                    Pos_T = 1; Pos_B = 7;
                    nCAL_POINT = DEFINE.CAL_POINT;
                }
                //-----------------------------------------------------
                while (roop)
                {
                    switch (seq)
                    {
                        case SEQ.INIT_SEQ:
                            Thread.Sleep(nSleep);
                            m_Current_T = 0;
                            PAT[m_PatTagNo, nPatNum[0]].SetAllLight(Main.DEFINE.M_LIGHT_CNL);
                            if (CalMode == Main.DEFINE.CAL_XY)
                                seq = SEQ.X_CAL_SEQ;
                            if (CalMode == Main.DEFINE.CAL_T)
                            {
                                seq = SEQ.T1_CAL_SEQ;
                            }
                            break;

#region XY_9 POINT CAL
                        case SEQ.X_CAL_SEQ:
                            if ((Main.DEFINE.PROGRAM_TYPE == "FOP_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOP_PC4") && (m_AlignName == "PBD_FOF1" || m_AlignName == "PBD_FOF2"))
                            {
                                seq = SEQ.COMPLET_SEQ; //Only HEAD Theta Cal
                                break;
                            }
                            if (m_Cal_MOVE_X == 0 && m_Cal_MOVE_Y == 0)
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }
                            nCalMovePos = 0;
                            GetRobotPosition(ref nRotbotPos);
                            GetMovePosition(ref nRotMovePos);
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 1:

                            seq++;
                            break;


                        case SEQ.X_CAL_SEQ + 2:
                            if (Main.DEFINE.PROGRAM_TYPE != "QD_LPA_PC1" && DEFINE.PROGRAM_TYPE != "QD_LPA_PC2")
                            {
                                MotionMove((long)nRotMovePos[nCalMovePos, DEFINE.XPOS], (long)nRotMovePos[nCalMovePos, DEFINE.YPOS], 0);

                                Thread.Sleep(nSleep);
                                m_Timer.StartTimer();
                            }
                            else
                            {
                                if (!MotionMove((long)nRotMovePos[nCalMovePos, DEFINE.XPOS], (long)nRotMovePos[nCalMovePos, DEFINE.YPOS], 0))
                                {
                                    m_errSts = ERR.PLC_MOVE_END;
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                                Thread.Sleep(nSleep);
                            }
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 3:
                            if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                            {
                                if (!MotionDone())  // 안에서 타임아웃 체크 함
                                {
                                    m_errSts = ERR.PLC_MOVE_END;
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                                Thread.Sleep(nSleep);
                            }
                            else
                            {
                                if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT_CAL)
                                {
                                    m_errSts = ERR.PLC_MOVE_END;
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                                if (!MotionDone()) break;
                                Thread.Sleep(nSleep);
                            }
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 4:
                            GetImage(nPatNum);
                            Thread.Sleep(200);
                            GetPosition();
                            if (!PAT[m_PatTagNo, nPatNum[0]].Search())
                            {
                                if (!PAT[m_PatTagNo, nPatNum[0]].Search()) m_errSts = nPatNum[0];
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            DrawSet_CAL(nPatNum[0]);
                            //--------------------------------------------------------------------------------------
                            nPixelPos_1[nCalMovePos, DEFINE.XPOS] = PAT[m_PatTagNo, nPatNum[0]].Pixel[DEFINE.X];
                            nPixelPos_1[nCalMovePos, DEFINE.YPOS] = PAT[m_PatTagNo, nPatNum[0]].Pixel[DEFINE.Y];
                            //--------------------------------------------------------------------------------------
                            MainGriddataDisplay(nPatNum[0]);
                            if (Main.DEFINE.PROGRAM_TYPE != "QD_LPA_PC1" && DEFINE.PROGRAM_TYPE != "QD_LPA_PC2")
                                nCalMovePos++;
                            Thread.Sleep(100);
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 5:
                            if (Main.DEFINE.PROGRAM_TYPE != "QD_LPA_PC1" && DEFINE.PROGRAM_TYPE != "QD_LPA_PC2")
                            {
                                seq++;
                                break;
                            }

                            if (!MotionMove((long)(nRotMovePos[nCalMovePos, DEFINE.XPOS] * (-1)), (long)(nRotMovePos[nCalMovePos, DEFINE.YPOS] * (-1)), 0))
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            Thread.Sleep(nSleep);

                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 6:
                            if (Main.DEFINE.PROGRAM_TYPE != "QD_LPA_PC1" && DEFINE.PROGRAM_TYPE != "QD_LPA_PC2")
                            {
                                seq++;
                                break;
                            }

                            if (!MotionDone())  // 안에서 타임아웃 체크 함
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            Thread.Sleep(nSleep);
                            nCalMovePos++;
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 7:
                            if (nCalMovePos < nCAL_POINT)   // if(0 ~ 8 < 9-1) 
                            {
                                seq = SEQ.X_CAL_SEQ + 1; //Retry
                            }
                            else
                            {
                                if (PAT[m_PatTagNo, nPatNum[0]].CALMATRIX_NOTUSE)
                                    seq = SEQ.X_CAL_SEQ + 10; // Go Matrix
                                else
                                    seq = SEQ.X_CAL_SEQ + 8; // Go Matrix
                            }
                            break;

                        case SEQ.X_CAL_SEQ + 8: //Matrix Start
                            double[,] nTempOffset = new double[1, 2];
                            int CenterPOS = (DEFINE.CAL_POINT - 1) / 2;

                            nTempOffset[0, DEFINE.XPOS] = Main.vision.IMAGE_CENTER_X[PAT[m_PatTagNo, nPatNum[0]].m_CamNo] - nPixelPos_1[CenterPOS, DEFINE.XPOS];
                            nTempOffset[0, DEFINE.YPOS] = Main.vision.IMAGE_CENTER_Y[PAT[m_PatTagNo, nPatNum[0]].m_CamNo] - nPixelPos_1[CenterPOS, DEFINE.YPOS];

                            for (int i = 0; i < DEFINE.CAL_POINT; i++)
                            {
                                nPixelPos_1[i, DEFINE.XPOS] += nTempOffset[0, DEFINE.XPOS];
                                nPixelPos_1[i, DEFINE.YPOS] += nTempOffset[0, DEFINE.YPOS];
                            }

                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 9: //Matrix Start
                            if (Calibration_Matrix(nPixelPos_1, nRotbotPos, DEFINE.CAL_POINT, ref PAT[m_PatTagNo, nPatNum[0]].CALMATRIX) < 0 && !DEFINE.OPEN_F)
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 10:
                            if (m_Cal_MOVE_X == 0)
                            {
                                seq++;
                                break;
                            }
                            //------------------------X AXIS CAMERA CCD THETA-----------------------------------
                            nTemX = (nPixelPos_1[Pos_R, DEFINE.XPOS] - nPixelPos_1[Pos_L, DEFINE.XPOS]);
                            nTemY = (nPixelPos_1[Pos_L, DEFINE.YPOS] - nPixelPos_1[Pos_R, DEFINE.YPOS]);        // Y CCD Theta와 부호 맞추기 위해서.Pos_L - Pos_R

                            PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.XPOS] = Math.Atan(nTemY / nTemX);
                            PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.XPOS] *= DEFINE.degree;
                            if (PAT[m_PatTagNo, nPatNum[0]].CALMATRIX_NOTUSE)
                                PAT[m_PatTagNo, nPatNum[0]].m_CalX[0] = (m_Cal_MOVE_X * 2) / Math.Sqrt(nTemX * nTemX + nTemY * nTemY);
                            else
                                PAT[m_PatTagNo, nPatNum[0]].m_CalX[0] = (m_Cal_MOVE_X * (Math.Sqrt(nCAL_POINT) - 1)) / Math.Sqrt(nTemX * nTemX + nTemY * nTemY);
                            //----------------------------------------------------------------------------------
                            if (PAT[m_PatTagNo, nPatNum[0]].CALMATRIX_NOTUSE)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].m_CalY[0] = PAT[m_PatTagNo, nPatNum[0]].m_CalX[0];
                                PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.XPOS];
                            }
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 11:
                            if (m_Cal_MOVE_Y == 0)
                            {
                                seq++;
                                break;
                            }
                            //------------------------Y AXIS CAMERA CCD THETA-----------------------------------
                            nTemY = (nPixelPos_1[Pos_B, DEFINE.YPOS] - nPixelPos_1[Pos_T, DEFINE.YPOS]);
                            nTemX = (nPixelPos_1[Pos_B, DEFINE.XPOS] - nPixelPos_1[Pos_T, DEFINE.XPOS]);

                            PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.YPOS] = Math.Atan(nTemX / nTemY);
                            PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.YPOS] *= DEFINE.degree;

                            if (PAT[m_PatTagNo, nPatNum[0]].CALMATRIX_NOTUSE)
                                PAT[m_PatTagNo, nPatNum[0]].m_CalY[0] = (m_Cal_MOVE_Y * 2) / Math.Sqrt(nTemX * nTemX + nTemY * nTemY);
                            else
                                PAT[m_PatTagNo, nPatNum[0]].m_CalY[0] = (m_Cal_MOVE_Y * (Math.Sqrt(nCAL_POINT) - 1)) / Math.Sqrt(nTemX * nTemX + nTemY * nTemY);
                            //----------------------------------------------------------------------------------
                            if (PAT[m_PatTagNo, nPatNum[0]].CALMATRIX_NOTUSE)
                            {
                                PAT[m_PatTagNo, nPatNum[0]].m_CalX[0] = PAT[m_PatTagNo, nPatNum[0]].m_CalY[0];
                                PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.YPOS];
                            }
                            if (m_AlignName == "NACHI_ROBOT")
                            {
                                seq = SEQ.X_CAL_SEQ + 14;
                                break;
                            }
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 12:
                            if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                            {
                                seq++;
                                break;
                            }
                            MotionMove((long)nRotMovePos[0, DEFINE.XPOS], (long)nRotMovePos[0, DEFINE.YPOS], 0);
                            Thread.Sleep(nSleep);
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 13:
                            if (Main.DEFINE.PROGRAM_TYPE != "QD_LPA_PC1" && Main.DEFINE.PROGRAM_TYPE != "QD_LPA_PC2")
                            {
                                if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT_CAL)
                                {
                                    m_errSts = ERR.PLC_MOVE_END;
                                    seq = SEQ.ERROR_SEQ;
                                    break;
                                }
                                if (!MotionDone()) break;
                            }
                            GetImage(nPatNum);
                            Thread.Sleep(200);
                            GetPosition();
                            if (!PAT[m_PatTagNo, nPatNum[0]].Search())
                            {
                                if (!PAT[m_PatTagNo, nPatNum[0]].Search()) m_errSts = nPatNum[0];
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            DrawSet_CAL(nPatNum[0]);
                            MainGriddataDisplay(nPatNum[0]);
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 14:
                            seq++;
                            break;

                        case SEQ.X_CAL_SEQ + 15:
#region CAL TEST
                            if (Main.DEFINE.OPEN_F)
                            {
                                int temp = 2;
#region
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[0] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[1] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[2] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[3] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[4] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[5] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[6] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[7] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CALMATRIX[8] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].m_CalX[0] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].m_CalX[1] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].m_CalY[0] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].m_CalY[1] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.XPOS] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
                                PAT[m_PatTagNo, nPatNum[0]].CAMCCDTHETA[0, DEFINE.YPOS] = m_AlignNo * 10 + m_PatTagNo + nPatNum[0] + temp;
#endregion
                            }
#endregion
                            seq = SEQ.COMPLET_XY;
                            break;
#endregion

#region THETA_CALIBRATION
#region T1_CAL
                        case SEQ.T1_CAL_SEQ:
                            if (m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2" /*|| (m_AlignName == "FPC_ALIGN" && m_PatTagNo > 0)*/)
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }
                            if (m_Cal_MOVE_T1 == 0)
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }
                            Thread.Sleep(nSleep);
                            GetImage(nPatNum);
                            seq++;
                            break;

                        case SEQ.T1_CAL_SEQ + 1:
                            Thread.Sleep(200);
                            if (!PAT[m_PatTagNo, nPatNum[0]].Search())
                            {
                                if (!PAT[m_PatTagNo, nPatNum[0]].Search()) m_errSts = nPatNum[0];
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            DrawSet_CAL(nPatNum[0]);
                            x1 = m_AxisX - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX;
                            y1 = m_AxisY - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY;

                            m_CalMotoPosX[m_PatTagNo] = m_AxisX;
                            m_CalMotoPosY[m_PatTagNo] = m_AxisY;

                            string LogMsg0 = "T1 전:" + "Robot(" + PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX.ToString("0.00") + " , " + PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY.ToString("0.00") +
                                             "m_Axis(" + m_AxisX.ToString() + " , " + m_AxisY.ToString() + ", X1:" + x1.ToString() + " ,Y1:" + y1.ToString();
                            LogdataDisplay(LogMsg0, true);
                            seq++;
                            break;


                        case SEQ.T1_CAL_SEQ + 2:
                            MotionMove(0, 0, m_Cal_MOVE_T1 * m_DirT);
                            Thread.Sleep(nSleep);
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case SEQ.T1_CAL_SEQ + 3:
                            if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT_CAL)
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            if (!MotionDone()) break;
                            Thread.Sleep(nSleep);
                            seq++;
                            break;

                        case SEQ.T1_CAL_SEQ + 4:
                            GetImage(nPatNum);
                            Thread.Sleep(200);
                            if (!PAT[m_PatTagNo, nPatNum[0]].Search())
                            {
                                if (!PAT[m_PatTagNo, nPatNum[0]].Search()) m_errSts = nPatNum[0];

                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            DrawSet_CAL(nPatNum[0]);
                            //-------------------------------------------------------------------------------------------------------
                            x2 = m_AxisX - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX;
                            y2 = m_AxisY - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY;
                            //두점의 x1,x2 지나는 호의 중심점 구하
                            /////////////////////////////////////////////////////////
                            // cx = x1 - 0.5*((x1-x2)+(y1-y2)*sin(t)/(cos(t)-1))
                            // cy = y1 - 0.5*((y1-y2)-(x1-x2)*sin(t)/(cos(t)-1))
                            // theta = ((double)cdt / 1000.0) * PI / (double)180.0;
                            /////////////////////////////////////////////////////////
                            theta = ((double)m_Cal_MOVE_T1 / 1000.0) * Main.DEFINE.PI / 180;  // 라디안으로 변환


                            GetCenter(x1, y1, x2, y2, theta, ref m_CenterX[CalNo], ref m_CenterY[CalNo]);

                            string LogMsg11 = "T1 회전후:" + "Robot(" + PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX.ToString("0.00") + " , " + PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY.ToString("0.00") +
                            "m_Axis(" + m_AxisX.ToString() + " , " + m_AxisY.ToString() + ", X2:" + x2.ToString() + " ,Y2:" + y2.ToString();
                            LogdataDisplay(LogMsg11, true);

                            MainGriddataDisplay(nPatNum[0]);
                            Thread.Sleep(nSleep);
                            seq++;
                            break;

                        case SEQ.T1_CAL_SEQ + 5:
                            if (m_AlignName == "NACHI_ROBOT")
                                MotionMove(0, 0, 0);
                            else
                                MotionMove(0, 0, m_Cal_MOVE_T1 * m_DirT * -1);

                            Thread.Sleep(nSleep);
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case SEQ.T1_CAL_SEQ + 6:
                            if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT_CAL)
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            if (!MotionDone()) break;
                            Thread.Sleep(nSleep);
                            seq++;
                            break;
                        case SEQ.T1_CAL_SEQ + 7:
                            seq = SEQ.COMPLET_SEQ;
                            break;

#endregion

#region T2_CAL
                        case SEQ.T2_CAL_SEQ:
                            if (m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2")
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }
                            if (m_Cal_MOVE_T2 == 0)
                            {
                                seq = SEQ.COMPLET_SEQ;
                                break;
                            }

                            dt = m_Cal_MOVE_T1 * rep;
                            if (dt > m_Cal_MOVE_T2)
                            {
                                dt = m_Cal_MOVE_T2;
                            }
                            theta = ((double)dt / 1000.0) * Main.DEFINE.PI / 180;  // 라디안으로 변환
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 1:
                            Thread.Sleep(nSleep);
                            GetImage(nPatNum);
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 2:
                            Thread.Sleep(200);
                            if (!PAT[m_PatTagNo, nPatNum[0]].Search())
                            {
                                if (!PAT[m_PatTagNo, nPatNum[0]].Search()) m_errSts = nPatNum[0];
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            DrawSet_CAL(nPatNum[0]);

                            ObjectImgX = PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX;
                            ObjectImgY = PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY;

                            x1 = m_AxisX - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX;
                            y1 = m_AxisY - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY;
                            ////////////////////////////////////////////////////
                            // x2 = (x-cx) * cos (t) - (y - cy) * sin (t) + cx
                            // y2 = (y-cy) * cos (t) + (x - cx) * sin (t) + cy
                            ////////////////////////////////////////////////////			

                            alignX = (m_AxisX - m_CenterX[CalNo]) * Math.Cos(theta) - (m_AxisY - m_CenterY[CalNo]) * Math.Sin(theta) + m_CenterX[CalNo];
                            alignY = (m_AxisY - m_CenterY[CalNo]) * Math.Cos(theta) + (m_AxisX - m_CenterX[CalNo]) * Math.Sin(theta) + m_CenterY[CalNo];
                            dx = alignX - m_AxisX;
                            dy = alignY - m_AxisY;
                            seq++;
                            break;


                        case SEQ.T2_CAL_SEQ + 3:
                            MotionMove((long)dx * m_DirX, (long)dy * m_DirY, (long)dt * m_DirT); // 보정이동
                            Thread.Sleep(nSleep);
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 4:
                            if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT_CAL)
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            if (!MotionDone()) break;
                            Thread.Sleep(nSleep);
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 5:
                            GetImage(nPatNum);
                            Thread.Sleep(200);
                            if (!PAT[m_PatTagNo, nPatNum[0]].Search())
                            {
                                if (!PAT[m_PatTagNo, nPatNum[0]].Search()) m_errSts = nPatNum[0];

                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            DrawSet_CAL(nPatNum[0]);
                            //-------------------------------------------------------------------------------------------------------
                            TargetImgX = PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX;
                            TargetImgY = PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY;

                            x2 = alignX - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosX;
                            y2 = alignY - PAT[m_PatTagNo, nPatNum[0]].m_RobotPosY;

                            /////////////////////////////////////////////////////////
                            // cx = x1 - 0.5*((x1-x2)+(y1-y2)*sin(t)/(cos(t)-1))
                            // cy = y1 - 0.5*((y1-y2)-(x1-x2)*sin(t)/(cos(t)-1))
                            /////////////////////////////////////////////////////////

                            GetCenter(x1, y1, x2, y2, theta, ref m_CenterX[CalNo], ref m_CenterY[CalNo]);

                            //-------------------------------------------------------------------------------------------------------
                            //-------------------------------------------------------------------------------------------------------
                            MainGriddataDisplay(nPatNum[0]);
                            Thread.Sleep(nSleep);
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 6:
                            MotionMove((long)dx * m_DirX * -1, (long)dy * m_DirY * -1, (long)dt * m_DirT * -1); // 원상복구
                            Thread.Sleep(nSleep);
                            m_Timer.StartTimer();
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 7:
                            if (m_Timer.GetElapsedTime() > DEFINE.MOVE_TIMEOUT_CAL)
                            {
                                m_errSts = ERR.PLC_MOVE_END;
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            if (!MotionDone()) break;
                            Thread.Sleep(nSleep);
                            seq++;
                            break;

                        case SEQ.T2_CAL_SEQ + 8:

                            string LogMsg;
                            double TempGapX = 0, TempGapY = 0;

                            TempGapX = Math.Abs(ObjectImgX - TargetImgX);
                            TempGapY = Math.Abs(ObjectImgY - TargetImgY);

                            LogMsg = "T2_Gap" + " X:" + TempGapX.ToString("0.00") + "um" + " Y:" + TempGapY.ToString("0.00") + "um" + " Count" + rep.ToString();
                            LogdataDisplay(LogMsg, true);

                            if (TempGapX <= 3 && TempGapY <= 3 && rep > 3)
                            {

                            }
                            else
                            {
                                if (rep <= 9) // MAX = 9
                                {
                                    rep++;
                                    seq = SEQ.T2_CAL_SEQ;
                                    break;
                                }
                            }
                            seq = SEQ.COMPLET_SEQ;
                            break;
#endregion

#endregion

                        case SEQ.ERROR_SEQ: // 에러 시퀀스			
                            if (m_AlignName == "LOAD_ALIGN1")
                            {
                                for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                                {
                                    PAT[m_PatTagNo, nPatNum[0]].Pattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * 5);
                                    PAT[m_PatTagNo, nPatNum[0]].Pattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * 5);

                                    PAT[m_PatTagNo, nPatNum[0]].GPattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * 5);
                                    PAT[m_PatTagNo, nPatNum[0]].GPattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * 5);
                                }
                            }
                            DrawSet_CAL(nPatNum[0]);
                            PAT[m_PatTagNo, nPatNum[0]].Save();
                            m_Current_T = 0;
                            roop = false;
                            ret = false;
                            m_bCalibMode = false;
                            break;

                        case SEQ.COMPLET_XY: // XY완료 시퀀스
                            CalPixelData(nPatNum[0], nPixelPos_1);
                            PAT[m_PatTagNo, nPatNum[0]].Save();
                            Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PAT");        //"PAT","CENTER","PATTAG","UNIT"
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ: // 완료 시퀀스
                            m_CalDisplayCX[m_PatTagNo] = m_CenterX[m_PatTagNo] - m_CalMotoPosX[m_PatTagNo];
                            m_CalDisplayCY[m_PatTagNo] = m_CenterY[m_PatTagNo] - m_CalMotoPosY[m_PatTagNo];

                            if (Main.DEFINE.PROGRAM_TYPE == "OLB_PC3")
                            {
                                //                                 if (m_AlignName == "PBD_STAGE1" || m_AlignName == "PBD_STAGE2")
                                //                                 {
                                //                                     if (nPatNum[0] == DEFINE.OBJ_L || nPatNum[0] == DEFINE.OBJ_R)
                                //                                     {
                                //                                         Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "UNIT");
                                // //                                         Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PAT");
                                // //                                         Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "CENTER");
                                //                                         //Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PATTAG");
                                //                                     }
                                //                                 }

                                if (m_AlignName == "PBD1" || m_AlignName == "PBD2")
                                {
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PAT");
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "CENTER");
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "UNIT");
                                }
                            }


                            if (m_AlignName == "CRD_PRE1" || m_AlignName == "CRD_PRE2" || m_AlignName == "CRD_PRE3" || m_AlignName == "CRD_PRE4" || m_AlignName == "THICKNESS_PRE" || m_AlignName == "LOW_INSPECT" || m_AlignName == "UPPER_INSPECT")
                            {
#region
                                if (nPatNum[0] == DEFINE.OBJ_L || nPatNum[0] == DEFINE.OBJ_R)
                                {
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "CENTER");
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PATTAG");
                                }
#endregion
                            }

                            if ((Main.DEFINE.PROGRAM_TYPE == "FOF_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOF_PC4"))
                            {
                                if ((m_AlignName == "PBD1" || m_AlignName == " PBD2"))    //FOP MODE 일때 DATA 복사
                                {
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PAT");
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "CENTER");
                                    Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "PATTAG");
                                    //                                     if(CalMode == Main.DEFINE.CAL_T) Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "CENTER");
                                    //                                     if (m_PatTagNo == Main.DEFINE.FOF_TAG_MODE) Calibration_XYT_DataCopy(m_AlignName, m_PatTagNo, nPatNum[0], "UNIT"); //Target Calibraiton값 복사.. 
                                }
                            }

                            if (Main.DEFINE.PROGRAM_TYPE == "TFOG_PC2" && m_AlignName == "PBD_STAGE1")
                            {
#region
                                if (nPatNum[0] == DEFINE.OBJ_L || nPatNum[0] == DEFINE.OBJ_R)// PBD_Stage 유닛별 Object -> PBD 유닛별 Target,Object 복사 
                                {
                                    string Dest_Unit = "PBD1";
                                    int Dest_TAG, SourceTAG;
                                    Dest_TAG = SourceTAG = m_PatTagNo;
                                    int Dest_Pat = nPatNum[0] + 2;
                                    int Source = nPatNum[0];

                                    Array.Copy(PAT[SourceTAG, Source].CALMATRIX, Main.AlignUnit[Dest_Unit].PAT[Dest_TAG, Dest_Pat].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                                    Main.AlignUnit[Dest_Unit].PAT[Dest_TAG, Dest_Pat].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                                    Main.AlignUnit[Dest_Unit].PAT[Dest_TAG, Dest_Pat].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                                    Main.AlignUnit[Dest_Unit].PAT[Dest_TAG, Dest_Pat].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                                    Main.AlignUnit[Dest_Unit].PAT[Dest_TAG, Dest_Pat].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];
                                    Main.AlignUnit[Dest_Unit].PAT[Dest_TAG, Dest_Pat].Save();
                                }
#endregion
                            }
                            Main.CenterXYSave(m_AlignNo);
                            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                            seq++;
                            break;

                        case SEQ.COMPLET_SEQ + 1: // 완료 시퀀스

                            if (m_AlignName == "LOAD_ALIGN1")
                            {
                                for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                                {
                                    PAT[m_PatTagNo, nPatNum[0]].Pattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * 5);
                                    PAT[m_PatTagNo, nPatNum[0]].Pattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * 5);

                                    PAT[m_PatTagNo, nPatNum[0]].GPattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * 5);
                                    PAT[m_PatTagNo, nPatNum[0]].GPattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * 5);
                                }
                            }

                            // Cal Unit 위치 저장
                            m_Current_T = 0;
                            roop = false;
                            ret = true;
                            m_bCalibMode = false;
                            break;

                    }
                }


                return ret;
            }
            private bool Calibration_XYT_DataCopy(String SourceUnit, int SourceTAG, int Source, string CopyType)
            {
                bool Ret = false;
                int copyNum = 0;
                String DestinationUnit = "";
                int DestinationTAG = 0, Destination = 0;
                DestinationTAG = SourceTAG = m_PatTagNo;

                if (CopyType == "PAT")
                {
                    if (m_AlignType[m_PatTagNo] == DEFINE.M_1CAM1SHOT || m_AlignType[m_PatTagNo] == DEFINE.M_1CAM2SHOT || m_AlignType[m_PatTagNo] == DEFINE.M_1CAM4SHOT)
                    {
                        if (Source == DEFINE.OBJ_L || Source == DEFINE.TAR_L) Destination = Source + 1;  //Left 캘할때 Right에 복사
                        else Destination = Source - 1;  // Right 캘할때 Left에 복사

                        if (m_AlignName == "LOAD_ALIGN") Destination = Source + 2;

                        Array.Copy(PAT[SourceTAG, Source].CALMATRIX, PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                        PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                        PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                        PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                        PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];

                        PAT[DestinationTAG, Source].Save();
                        PAT[DestinationTAG, Destination].Save();
                    }
                    if ((Main.DEFINE.PROGRAM_TYPE == "OLB_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOF_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOF_PC4") && (m_AlignName == "PBD1" || m_AlignName == "PBD2"))
                    {
                        if (Source == DEFINE.OBJ_L || Source == DEFINE.OBJ_R) Destination = Source + 2;
                        else Destination = Source - 2;

                        Array.Copy(PAT[SourceTAG, Source].CALMATRIX, PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                        PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                        PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                        PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                        PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];

                        PAT[DestinationTAG, Source].Save();
                        PAT[DestinationTAG, Destination].Save();
                    }
                }
                else if (CopyType == "CENTER")
                {
                    for (int i = 1; i < m_AlignPatTagMax; i++)//Head XYT 얼라인시 1번태그 캘값을 태그 숫자만큼 복사.
                    {
                        DestinationTAG = i;
                        m_CenterX[DestinationTAG] = m_CenterX[SourceTAG];
                        m_CenterY[DestinationTAG] = m_CenterY[SourceTAG];
                        m_CalMotoPosX[DestinationTAG] = m_CalMotoPosX[SourceTAG];
                        m_CalMotoPosY[DestinationTAG] = m_CalMotoPosY[SourceTAG];
                        m_CalDisplayCX[DestinationTAG] = m_CalDisplayCX[SourceTAG];
                        m_CalDisplayCY[DestinationTAG] = m_CalDisplayCY[SourceTAG];
                    }
                }
                else if (CopyType == "PATTAG")
                {
                    for (int i = 1; i < m_AlignPatTagMax; i++)//Head XYT 얼라인시 1번태그 캘값을 태그 숫자만큼 복사.
                    {
                        DestinationTAG = i;
                        for (int j = 0; j < Main.DEFINE.Pattern_Max; j++)
                        {
                            Destination = Source = j;
                            Array.Copy(PAT[SourceTAG, Source].CALMATRIX, PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                            PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                            PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                            PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                            PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];

                            PAT[DestinationTAG, Destination].Save();
                        }
                    }
                }
                else if (CopyType == "UNIT")
                {
                    // PBD_Stage 유닛별 Object -> PBD 유닛별 Target,Object 복사( 0 -> 0,2)
                    //                     if ((Main.DEFINE.PROGRAM_TYPE == "FOF_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOF_PC4"))
                    //                     {
                    //                         if (Source == DEFINE.OBJ_L || Source == DEFINE.OBJ_R)// PBD_Stage 유닛별 Object -> PBD 유닛별 Target,Object 복사 
                    //                         {
                    //                             #region
                    //                             if (m_AlignName == "PBD_STAGE1" || m_AlignName == "PBD_STAGE2")
                    //                             {                      
                    //                                 if (m_AlignName == "PBD_STAGE1")DestinationUnit = "PBD1";
                    //                                 if (m_AlignName == "PBD_STAGE2")DestinationUnit = "PBD2"; 
                    //                                 for (int i = 0; i < 2; i++)
                    //                                 {                            
                    //                                     DestinationTAG = SourceTAG;
                    //                                     Destination = Source;
                    //                                     Array.Copy(PAT[SourceTAG, Source].CALMATRIX, Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].Save();
                    //                                 }
                    //                                 for (int i = 0; i < 2; i++)
                    //                                 {
                    //                                     DestinationTAG = SourceTAG;
                    //                                     Destination = Source + (i * 2);
                    //                                     Array.Copy(PAT[SourceTAG, Source].CALMATRIX, Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];
                    //                                     Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].Save();
                    //                                 }
                    //                             }
                    //                                 #endregion
                    //                         }
                    //                         if (Source == DEFINE.TAR_L || Source == DEFINE.TAR_R && (SourceTAG == Main.DEFINE.FOF_TAG_MODE)) //
                    //                         {
                    //                             #region
                    //                             if (m_AlignName == "PBD1" && m_AlignName == "PBD2") 
                    //                             {
                    //                                 DestinationTAG = SourceTAG;
                    //                                 Destination = Source - 2;
                    //                                 Array.Copy(PAT[SourceTAG, Source].CALMATRIX, PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                    //                                 PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                    //                                 PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                    //                                 PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                    //                                 PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];
                    //                                 PAT[DestinationTAG, Destination].Save();
                    //                             }
                    //                             #endregion
                    //                         }
                    // 
                    //                     }
                    //                     else if (Main.DEFINE.PROGRAM_TYPE == "OLB_PC3")
                    //                     {
                    //                         if (Source == DEFINE.TAR_L || Source == DEFINE.TAR_R)// PBD_Stage 유닛별 Object -> PBD 유닛별 Target,Object 복사 
                    //                         {
                    // 
                    //                             if (m_AlignName == "PBD1" || m_AlignName == "PBD2")
                    //                             {
                    // 
                    //                                 DestinationUnit = "PBD_STAGE1"; DestinationTAG = SourceTAG;
                    //                                 Destination = Source;
                    //                                 Array.Copy(PAT[SourceTAG, Source].CALMATRIX, Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CALMATRIX, PAT[SourceTAG, Source].CALMATRIX.Length);
                    //                                 Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].m_CalX[0] = PAT[SourceTAG, Source].m_CalX[0];
                    //                                 Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].m_CalY[0] = PAT[SourceTAG, Source].m_CalY[0];
                    //                                 Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.XPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.XPOS];
                    //                                 Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].CAMCCDTHETA[0, DEFINE.YPOS] = PAT[SourceTAG, Source].CAMCCDTHETA[0, DEFINE.YPOS];
                    // 
                    //                                 Main.AlignUnit[DestinationUnit].PAT[DestinationTAG, Destination].Save();
                    // 
                    //                                 if (Source == DEFINE.TAR_L)
                    //                                 {
                    //                                     AlignUnit[DestinationUnit].m_CenterX[DestinationTAG] = AlignUnit[m_AlignNo].m_CenterX[SourceTAG];
                    //                                     AlignUnit[DestinationUnit].m_CenterY[DestinationTAG] = AlignUnit[m_AlignNo].m_CenterY[SourceTAG];
                    // 
                    //                                     Main.CenterXYSave(Destination);
                    //                                 }
                    // 
                    //                                 
                    //                             }
                    // 
                    //                         }
                    //                     }


                }

                return Ret;
            }
            private void CmdCheck_(int nAddress)
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
                            if (PLCDataTag.RData[nAddress] != 0)
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
            private void CmdCheck()
            {
                int seq = 0;
                bool LoopFlag = true;
                string prevValue = "";
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
                            Int16 value = PLCDataTag.RData[ALIGN_UNIT_ADDR + DEFINE.PLC_CMD];
                            if (prevValue != value.ToString())
                            {
                                //Console.WriteLine("C : " + value.ToString());
                                //prevValue = value.ToString();
                            }
                            if (value != 0)
                            {
                                //Console.WriteLine("Re : " + value.ToString());
                                break;
                            }
                            else
                            {
                                //Console.WriteLine("2111111");
                                seq = SEQ.COMPLET_SEQ;
                            }
                            break;

                        case SEQ.COMPLET_SEQ:
                            long nTime = 0;
                            nTime = m_Timer.GetElapsedTime();
                            LoopFlag = false;
                            break;

                    }

                }

            }
            public void WriteObjTarLength()
            {
                string LogMsg = "Stage:" + (m_PatTagNo + 1).ToString("00") + " , " + m_Cell_ID + " (";
                LogMsg = LogMsg + "ObjDis:" + ((long)m_OBJ_Mea_Dis).ToString("0") + "," + " TarDis:" + ((long)m_TAR_Mea_Dis).ToString("0");

                int[] nnDataBuf = new int[4];
                //-----------------------------------------------------------------------------------------------
                List<long> nMOT = new List<long>();

                nMOT.Add((long)m_OBJ_Mea_Dis);
                nMOT.Add((long)m_TAR_Mea_Dis);

                nnDataBuf[0] = (short)(nMOT[0] & 0x0000ffff);           //m_OBJ_Mea_Dis
                nnDataBuf[1] = (short)((nMOT[0] & 0xffff0000) >> 16);   //m_OBJ_Mea_Dis

                nnDataBuf[2] = (short)(nMOT[1] & 0x0000ffff);           //m_TAR_Mea_Dis
                nnDataBuf[3] = (short)((nMOT[1] & 0xffff0000) >> 16);   //m_TAR_Mea_Dis

                //-----------------------------------------------------------------------------------------------              
                int nAddress = PLCDataTag.BASE_RW_ADDR + Main.DEFINE.LENGTH_OBJTAR;
                //2022 05 09 YSH
                //Main.WriteDeviceBlock(nAddress, 4, ref nnDataBuf);                
                int[] setValue = new int[4];
                setValue = nnDataBuf;
                Main.PLCsocket.WriteDevice_W(nAddress.ToString(), setValue.Length, setValue);

                System.Array.Clear(nnDataBuf, 0, 3);

                //-----------------------------------------------------------------------------------------------
                LogMsg += " )";
                LogdataDisplay_Length(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
            }
            public void WriteTABLength(long[] Length)
            {
                // string LogMsg = "TAB_LENGTH(";
                string LogMsg = (m_PatTagNo + 1).ToString("00") + "_TAB_LENGTH(";
                if (m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2")
                    LogMsg = "ACF_LENGTH(";
                int[] nnDataBuf = new int[4];
                //-----------------------------------------------------------------------------------------------
                List<long> nMOT = new List<long>();
                for (int i = 0; i < Length.Length; i++)
                {
                    nMOT.Add((long)Length[i]);
                    nnDataBuf[(i * 2)] = (short)(nMOT[i] & 0x0000ffff);
                    nnDataBuf[(i * 2) + 1] = (short)((nMOT[i] & 0xffff0000) >> 16);
                }
                //-----------------------------------------------------------------------------------------------              
                int nAddress = PLCDataTag.BASE_RW_ADDR + Main.DEFINE.LENGTH_ADD;
                //2022 05 09 YSH
                //Main.WriteDeviceBlock(nAddress, 4, ref nnDataBuf);
                int[] setValue = new int[4];
                setValue = nnDataBuf;
                Main.PLCsocket.WriteDevice_W(nAddress.ToString(), setValue.Length, setValue);

                System.Array.Clear(nnDataBuf, 0, 3);
                //-----------------------------------------------------------------------------------------------
                for (int i = 0; i < Length.Length; i++)
                {
                    LogMsg += nMOT[i].ToString("00");
                    if (i == 0)
                    {
                        LogMsg += ",";
                    }
                }
                LogMsg += ")";
                LogdataDisplay_Length(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
            }
            public void WriteInspGap(double Gap0, double Gap1, double Gap2, double Gap3)
            {
                string LogMsg;
                //-----------------------------------------------------------------------------------------------
                List<long> nMOT = new List<long>();
                nMOT.Add((long)Gap0);
                nMOT.Add((long)Gap1);
                nMOT.Add((long)Gap2);
                nMOT.Add((long)Gap3);

                //-----------------------------------------------------------------------------------------------
                int[] nnDataBuf = new int[10];
                int nAddress;

                nnDataBuf[0] = (short)(nMOT[0] & 0x0000ffff);
                nnDataBuf[1] = (short)((nMOT[0] & 0xffff0000) >> 16);
                nnDataBuf[2] = (short)(nMOT[1] & 0x0000ffff);
                nnDataBuf[3] = (short)((nMOT[1] & 0xffff0000) >> 16);
                nnDataBuf[4] = (short)(nMOT[2] & 0x0000ffff);
                nnDataBuf[5] = (short)((nMOT[2] & 0xffff0000) >> 16);

                nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.VIS_ALIGN_X;
                //YSH 임시 주석, 필요 시 추가
                //Main.WriteDeviceBlock(nAddress, 6, ref nnDataBuf);
                System.Array.Clear(nnDataBuf, 0, 5);

                nnDataBuf[0] = (short)(nMOT[3] & 0x0000ffff);
                nnDataBuf[1] = (short)((nMOT[3] & 0xffff0000) >> 16);

                nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + 8;
                //YSH 임시 주석, 필요 시 추가
                //Main.WriteDeviceBlock(nAddress, 2, ref nnDataBuf);
                System.Array.Clear(nnDataBuf, 0, 1);

                //-----------------------------------------------------------------------------------------------
                LogMsg = "YGAP_LENGTH(" + nMOT[0].ToString() + "," + nMOT[1].ToString() + "," + nMOT[2].ToString() + "," + nMOT[3].ToString() + ")";

                string nCmdMSG = "";
                if (m_Cmd == CMD.DOPO_INSPECT_LEFT)
                    nCmdMSG = "LEFT";
                else
                    nCmdMSG = "RIGHT";
                LogdataDisplay_Length(nCmdMSG + "," + LogMsg, true, true);
                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
            }
            public void ClearPlcCmd()
            {
                //2022 05 09 YSH
                WriteDevice(PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + DEFINE.PLC_CMD, 0);
                //int[] setValue = new int[1];
                //setValue[0] = 0;
                //Main.PLCsocket.WriteDevice_W((PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR).ToString(), 1, setValue);

                //-----------------------------------------------------------------------------------------------
                //                string Date;
                string LogMsg;
                //                 Date = DateTime.Now.ToString("[MM:dd:HH:mm:ss:fff]");
                //                 LogMsg = Date;
                LogMsg = "<- RESET CMD"; // +m_AlignNo.ToString();
                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
            }
            public void WriteStsEnd(int iCmd)
            {
                //-----------------------------------------------------------------------------------------------
                string LogMsg;
                short nDataBuf;
                int nAddress;

                //if (iCmd == -CMD.ALIGN_OBJTAR_ALL || iCmd == -CMD.ALIGN_OBJTAR_ALL_REALIGN)
                //{
                //    if (m_errSts == Main.DEFINE.TAR_L || m_errSts == Main.DEFINE.TAR_R || m_errSts == Main.DEFINE.TAR_ALL)
                //    {
                //        iCmd = -CMD.TAR_SEARCH_ALL; // -1003 Target Error 시에는. 
                //    }
                //}
          
                iCmd = GetPatTagCmd(iCmd);

                nDataBuf = (short)iCmd;
                nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.VIS_STATUS;
                //2022 05 09 YSH
                //Console.WriteLine("Send Result " + nAddress.ToString());
                Main.WriteDevice(nAddress, nDataBuf);

                //-----------------------------------------------------------------------------------------------               
                string tactTime = m_Timer_index.GetElapsedTime().ToString("000");
                LogMsg = "T/Time:" + tactTime + "(ms)";
                LogMsg = LogMsg + " cmd:" + iCmd.ToString();
                string nResult = "";
                if (iCmd < 0)
                    nResult = " CMD_ERROR";
                else
                    nResult = " CMD____OK";
                LogMsg += nResult;

#region T_FOG_LogData
                LogdataDisplayData(nResult);
                LogdataDisplay(nMsgMarks, true);
#endregion

                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
                MainGriddataDisplay();
                PatternPixelRobotSave();
            }
            public void SetAlignPosEnd(int iCmd)
            {
                string LogMsg;
                // int Result_sts = 1;
                List<long> nMOT = new List<long>();

                nMOT.Add(Convert_DtoL(m_StageX));
                nMOT.Add(Convert_DtoL(m_StageY));

                if (m_AlignName == "PBD1" || m_AlignName == "PBD2")
                {
                    nMOT.Add((long)(m_StageT));
                }
                else
                {
                    nMOT.Add(Convert_DtoL(m_StageT));
                }

                //-----------------------------------------------------------------------------------------------            
                iCmd = GetPatTagCmd(iCmd);
                //-----------------------------------------------------------------------------------------------
                int[] nDataBuf = new int[10];
                int nAddress;
                int nSize = 10;
                nDataBuf[0] = (short)(nMOT[0] & 0x0000ffff);
                nDataBuf[1] = (short)((nMOT[0] & 0xffff0000) >> 16);

                nDataBuf[2] = (short)(nMOT[1] & 0x0000ffff);
                nDataBuf[3] = (short)((nMOT[1] & 0xffff0000) >> 16);

                nDataBuf[4] = (short)(nMOT[2] & 0x0000ffff);
                nDataBuf[5] = (short)((nMOT[2] & 0xffff0000) >> 16);
                nDataBuf[6] = (short)iCmd;    //VIS_STATUS
                nDataBuf[7] = 0;    //VIS_MOVE_REQ

                int[] ScoreDataBuf = new int[10];
                int Score_Address;
                int ASize = 10;
                List<long> nMOT1 = new List<long>();
                nMOT1.Add((int)(PAT[m_PatTagNo, 0].PMAlignResult.m_ACCeptScore * 100));
                nMOT1.Add((int)(PAT[m_PatTagNo, 1].PMAlignResult.m_ACCeptScore * 100));

                nMOT1.Add((int)(PAT[m_PatTagNo, 0].PMAlignResult.Score * 100));
                nMOT1.Add((int)(PAT[m_PatTagNo, 1].PMAlignResult.Score * 100));

                nMOT1.Add((int)(PAT[m_PatTagNo, 2].PMAlignResult.m_ACCeptScore * 100));
                nMOT1.Add((int)(PAT[m_PatTagNo, 3].PMAlignResult.m_ACCeptScore * 100));

                nMOT1.Add((int)(PAT[m_PatTagNo, 2].PMAlignResult.Score * 100));
                nMOT1.Add((int)(PAT[m_PatTagNo, 3].PMAlignResult.Score * 100));


                ScoreDataBuf[0] = (short)(nMOT1[0] & 0x0000ffff);
                ScoreDataBuf[1] = (short)(nMOT1[1] & 0x0000ffff);
                ScoreDataBuf[2] = (short)(nMOT1[2] & 0x0000ffff);
                ScoreDataBuf[3] = (short)(nMOT1[3] & 0x0000ffff);
                ScoreDataBuf[4] = (short)(nMOT1[4] & 0x0000ffff);
                ScoreDataBuf[5] = (short)(nMOT1[5] & 0x0000ffff);
                ScoreDataBuf[6] = (short)(nMOT1[6] & 0x0000ffff);
                ScoreDataBuf[7] = (short)(nMOT1[7] & 0x0000ffff);

                Score_Address = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.SCORE;
                //YSH 임의주석, 필요 시 추가
                //Main.WriteDeviceBlock(Score_Address, ASize, ref ScoreDataBuf);
                System.Array.Clear(ScoreDataBuf, 0, ScoreDataBuf.Length);

                nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.VIS_ALIGN_X;
                //YSH 임의주석, 필요 시 추가
                //Main.WriteDeviceBlock(nAddress, nSize, ref nDataBuf);
                System.Array.Clear(nDataBuf, 0, nDataBuf.Length);

                LogMsg = "Align(" + m_StageX.ToString("0.0") + ", " + m_StageY.ToString("0.0") + ", " + m_StageT.ToString("0.0") + ")";
                LogMsg = LogMsg + " T/Time:" + m_Timer_index.GetElapsedTime().ToString("000");
                LogMsg = LogMsg + " cmd:" + iCmd.ToString() + "," + " ObjDis:" + m_OBJ_Mea_Dis.ToString("0") + "," + " TarDis:" + m_TAR_Mea_Dis.ToString("0");

                if ((m_AlignName == "PBD1" || m_AlignName == "PBD2") && machine.MAP_Function_Onf)
                {
                    nDataBuf[0] = (int)m_MAP_ResultData[Main.DEFINE.X];
                    nDataBuf[1] = (int)m_MAP_ResultData[Main.DEFINE.Y];
                    nAddress = PLCDataTag.BASE_RW_ADDR + Main.DEFINE.MAP_DATA_PBD;
                    //YSH 임의주석, 필요 시 추가
                    //Main.WriteDeviceBlock(nAddress, 2, ref nDataBuf);

                    System.Array.Clear(nDataBuf, 0, 1);
                    LogMsg = LogMsg + " MAP(X:" + m_MAP_ResultData[Main.DEFINE.X].ToString() + ", " + " Y:" + m_MAP_ResultData[Main.DEFINE.Y].ToString("0") + ")";
                }
                string nResult = "";
                if (iCmd < 0)
                    nResult = " CMD_ERROR";
                else
                    nResult = " CMD____OK";
                LogMsg += nResult;

#region T_FOG_LogData
                LogdataDisplayData(nResult);
                if (nMsgMarks != "") LogdataDisplay(nMsgMarks, true);
#endregion

                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
                MainGriddataDisplay();
                PatternPixelRobotSave();
            }
            public void InputAligndata(string CellID, double[,] nLength_, double[] nLength_Result, string[] nMessage_, bool Result_sts)
            {
                string LogMsg = "";
                try
                {
                    Aligndata[m_PatTagNo].NUM.Add(1);
                    Aligndata[m_PatTagNo].CELLID.Add(CellID);
                    Aligndata[m_PatTagNo].nLength.Add(nLength_);

                    Aligndata[m_PatTagNo].nLengthWidth.Add(nLength_Result[DEFINE.WIDTH_]);
                    Aligndata[m_PatTagNo].nLengthHeight.Add(nLength_Result[DEFINE.HEIGHT]);

                    Aligndata[m_PatTagNo].m_Message.Add(nMessage_[DEFINE.WIDTH_] + nMessage_[DEFINE.HEIGHT]);
                    Aligndata[m_PatTagNo].Result_sts.Add(Result_sts);

                    if (Aligndata[m_PatTagNo].NUM.Count > Main.DEFINE.ALIGNINSPECDATA_MAX)
                    {
                        Aligndata[m_PatTagNo].RemoveAt();
                    }

                    LogMsg = CellID + "\t"
                    + Result_sts.ToString() + "," + "\t"
                    + nLength_[DEFINE.EDGE, DEFINE.WIDTH_].ToString("000.0") + "\t" + nLength_[DEFINE.EDGE, DEFINE.HEIGHT].ToString("000.0") + "\t"
                    + nLength_[DEFINE.BLOB, DEFINE.WIDTH_].ToString("000.0") + "\t" + nLength_[DEFINE.BLOB, DEFINE.HEIGHT].ToString("000.0") + "\t"
                    + nMessage_[DEFINE.WIDTH_].ToString() + "\t" + nMessage_[DEFINE.HEIGHT].ToString();

                    InspecDataSave(LogMsg, true);
                }
                catch (System.Exception ex)
                {

                }

            }
            public void InputAligndata(double m_AlignX, double m_AlignY, double m_AlignT, bool CenterData = false)
            {
                try
                {
                    if (!CenterData)
                    {
                        m_LogStageX[m_PatTagNo].Add(m_AlignX);
                        m_LogStageY[m_PatTagNo].Add(m_AlignY);
                        m_LogStageT[m_PatTagNo].Add(m_AlignT);

                        if (m_LogStageX[m_PatTagNo].Count > Main.DEFINE.ALIGNDATA_MAX) m_LogStageX[m_PatTagNo].RemoveAt(0);
                        if (m_LogStageY[m_PatTagNo].Count > Main.DEFINE.ALIGNDATA_MAX) m_LogStageY[m_PatTagNo].RemoveAt(0);
                        if (m_LogStageT[m_PatTagNo].Count > Main.DEFINE.ALIGNDATA_MAX) m_LogStageT[m_PatTagNo].RemoveAt(0);
                    }
                    else
                    {
                        m_LogCenterX[m_PatTagNo].Add(m_AlignX);
                        m_LogCenterY[m_PatTagNo].Add(m_AlignY);
                        m_LogCenterT[m_PatTagNo].Add(m_AlignT);

                        if (m_LogCenterX[m_PatTagNo].Count > Main.DEFINE.ALIGNDATA_MAX) m_LogCenterX[m_PatTagNo].RemoveAt(0);
                        if (m_LogCenterY[m_PatTagNo].Count > Main.DEFINE.ALIGNDATA_MAX) m_LogCenterY[m_PatTagNo].RemoveAt(0);
                        if (m_LogCenterT[m_PatTagNo].Count > Main.DEFINE.ALIGNDATA_MAX) m_LogCenterT[m_PatTagNo].RemoveAt(0);
                    }
                }
                catch (System.Exception ex)
                {

                }

            }
            public void InputTABdata(int m_PatNo, double m_LengthY)
            {
                try
                {
                    //     int nTempTagNum = 0;
                    int nTempTagNum = m_PatTagNo; //나중에 풀기. . 

                    PAT[nTempTagNum, m_PatNo].m_LogLengthY.Add(m_LengthY);
                    if (PAT[nTempTagNum, m_PatNo].m_LogLengthY.Count > Main.DEFINE.ALIGNDATA_MAX)
                        PAT[nTempTagNum, m_PatNo].m_LogLengthY.RemoveAt(0);
                }
                catch (System.Exception ex)
                {

                }

            }
            public void Aligndata_RemoveAt(int nPatTagNo)
            {
                try
                {
                    m_LogStageX[nPatTagNo].Clear();
                    m_LogStageY[nPatTagNo].Clear();
                    m_LogStageT[nPatTagNo].Clear();

                    m_LogCenterX[nPatTagNo].Clear();
                    m_LogCenterY[nPatTagNo].Clear();
                    m_LogCenterT[nPatTagNo].Clear();
                }
                catch (System.Exception ex)
                {

                }

            }

            object syncCSVLock = new object();

            public void WriteCSVLogFile(string strData, int nType, bool bForceGrab = false)
            {
                bool bIsEmpty = false;
                string strFolder = "", strLog = "";
                string strFileName = "";
                strFolder = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\CSV\\";

                if (!Directory.Exists(LogdataPath)) Directory.CreateDirectory(LogdataPath);
                if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);

                if (m_AlignName == "")
                    strFileName = m_AlignNo.ToString();

                lock (syncCSVLock)
                {
                    try
                    {
                        if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && nType == DEFINE.CAM_SELECT_ALIGN)
                        {
                            strFileName += "_ALIGN.csv";

                            if (!File.Exists(strFolder + m_AlignName + strFileName))
                                bIsEmpty = true;

                            using (m_SWriterAlign = new StreamWriter(strFolder + m_AlignName + strFileName, true, Encoding.UTF8))
                            {
                                if (bIsEmpty)
                                {
                                    m_SWriterAlign.WriteLine("Time,CellID,X,Y,POL,MANUAL");
                                }

                                strLog = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff],");
                                strLog += m_Cell_ID + ",";
                                strLog += strData;
                                strLog += (bForceGrab ? "M" : "");

                                m_SWriterAlign.WriteLine(strLog);
                            }
                        }
                        else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && nType == DEFINE.CAM_SELECT_INSPECT)
                        {
                            strFileName += "INSPECTION_DATA.csv";

                            //if (!File.Exists(strFolder + m_AlignName + strFileName))
                            if (!File.Exists(strFolder + strFileName))
                                bIsEmpty = true;

                            //using (m_SWriterInsp = new StreamWriter(strFolder + m_AlignName + strFileName, true, Encoding.Unicode))
                            using (m_SWriterInsp = new StreamWriter(strFolder + strFileName, true, Encoding.UTF8))
                            {
                                if (bIsEmpty)
                                {
                                    //m_SWriterInsp.WriteLine("Time,CellID,X,Y");
                                    m_SWriterInsp.WriteLine("Time,CellID,P1,P2,P3,P4,P5,P6,R1(C1),R2(C2),MANUAL");
                                }

                                strLog = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff],");
                                strLog += m_Cell_ID + ",";
                                strLog += strData;
                                strLog += (bForceGrab ? "M" : "");

                                m_SWriterInsp.WriteLine(strLog);
                            }
                        }
                        else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && nType == DEFINE.CAM_SELECT_1ST_ALIGN)
                        {
                            strFileName += "_1ST_ALIGN.csv";

                            if (!File.Exists(strFolder + m_AlignName + strFileName))
                                bIsEmpty = true;

                            using (m_SWriterAlign = new StreamWriter(strFolder + m_AlignName + strFileName, true, Encoding.UTF8))
                            {
                                if (bIsEmpty)
                                {
                                    m_SWriterAlign.WriteLine("Time,CellID,X,Y,Corner,Angle");
                                }

                                strLog = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff],");
                                strLog += m_Cell_ID + ",";
                                strLog += strData;
                                m_SWriterAlign.WriteLine(strLog);
                            }
                        }
                        else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && nType == DEFINE.CAM_SELECT_PRE_ALIGN_RIGHT)
                        {
                            strFileName += "_PRE_ALIGN.csv";

                            if (!File.Exists(strFolder + m_AlignName + strFileName))
                                bIsEmpty = true;

                            using (m_SWriterInsp = new StreamWriter(strFolder + m_AlignName + strFileName, true, Encoding.UTF8))
                            {
                                if (bIsEmpty)
                                {
                                    //m_SWriterInsp.WriteLine("Time,CellID,X,Y");
                                    m_SWriterInsp.WriteLine("Time,CellID,LX,LY,RX,RY");
                                }

                                strLog = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff],");
                                strLog += m_Cell_ID + ",";
                                strLog += strData;
                                m_SWriterInsp.WriteLine(strLog);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        LogdataDisplay(e.Message, true);
                    }
                }
            }

            public void WriteRCSLogFile()
            {
                bool bIsEmpty = false;
                string strFolder = "", strLog = "";
                string strFileName = "";
                strFolder = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\RCS\\";

                if (!Directory.Exists(LogdataPath)) Directory.CreateDirectory(LogdataPath);
                if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);

                if (m_AlignName == "")
                    strFileName = m_AlignNo.ToString();

                lock (syncCSVLock)
                {
                    try
                    {

                        {
                            strFileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + m_Cell_ID + ".csv";
                            //if (!File.Exists(strFolder + m_AlignName + strFileName))
                            if (!File.Exists(strFolder + strFileName))
                                bIsEmpty = true;

                            //using (m_SWriterInsp = new StreamWriter(strFolder + m_AlignName + strFileName, true, Encoding.Unicode))
                            using (m_SWriterInsp = new StreamWriter(strFolder + strFileName, true, Encoding.UTF8))
                            {
                                if (bIsEmpty)
                                {
                                    //m_SWriterInsp.WriteLine("Time,CellID,X,Y");
                                    m_SWriterInsp.WriteLine("ITEM,CELITEM,MODULETYPE,PROD_TYPE,MODULEID,DEVICEID,STEPID,H_CELLID,E_CELLID,P_CELLID,OPERID,C_COUNT,C_PPID,C_GRADE,C_CODE,R_C_GRADE,CELL_IMAGE,L_TIME,U_TIME,S_TIME,E_TIME,UNIT,C_R_CUT_SET,CUT_MODE");
                                    m_SWriterInsp.WriteLine("ITEM,MESITEM,MES_ID,CELLID,MES_NO,COORD_X1,COORD_Y1,COORD_X2,COORD_Y2,GATENO_01,DATANO_01,GATENO_02,DATANO_02,IMAGENAME,P_GRADE,P_CODE,P_R_GRADE,ALIGN_DATA");
                                }

                                strLog = "DATA,CELDATA,1AMCA,*,";
                                strLog = strLog + Main.machine.m_strModuleID + ",";
                                strLog = strLog + ProjectInfo + ",";
                                strLog += "*,";
                                strLog = strLog + m_Cell_ID + ",";
                                strLog = strLog + m_Cell_ID + ",";
                                strLog += "*,*,*,*,*,*,*,*,*,*,";
                                strLog = strLog + AlignUnit[2].m_InTime.ToString("yyyyMMddHHmmss") + ",";
                                strLog = strLog + AlignUnit[0].m_OutTime.ToString("yyyyMMddHHmmss") + ",";
                                strLog += "um,";
                                //  if (Main.CCLink.GetDWord(DEFINE.CCLINK_WW_PANEL_CORNER_OFFSET) > 0)
                                //      strLog = strLog + Main.CCLink.GetDWord(DEFINE.CCLINK_WW_PANEL_CORNER_OFFSET) / 1000;
                                //  else
                                //      strLog += "0,";

                                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_C_CUT_MODE))
                                    strLog += "1";
                                else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_R_CUT_MODE))
                                    strLog += "2";
                                else
                                    strLog += "E";
                                m_SWriterInsp.WriteLine(strLog);

                                for (int i = 0; i < DEFINE.MeasurePoint_Max; i++)
                                {
                                    strLog = "";
                                    strLog += "DATA,MESDATA,LC_MEASURE,";
                                    strLog = strLog + m_Cell_ID + "," + (i + 1).ToString() + ",";
                                    strLog += "*,*,*,*,*,*,*,*,";
                                    strLog = strLog + AlignUnit[GetAlignNumber(i)].PAT[0, 1].m_strImageLogName + ",";
                                    //if (GetAlignNumber(i) == 0)
                                    //    strLog = strLog + "10_00_52_193_8HKJJH0P0A_SCANNER HEAD CAM1_0_CAMERA__INSP_OV.jpg,";
                                    //else if (GetAlignNumber(i) == 1)
                                    //    strLog = strLog + "10_00_52_053_8HKJJH0P0A_ALIGN INSP CAM2_0_CAMERA__INSP_OV.jpg,";
                                    //else if (GetAlignNumber(i) == 2)
                                    //    strLog = strLog + "10_00_50_270_8HKJJH0P0A_ALIGN INSP CAM3_0_CAMERA__INSP_OV.jpg,";
                                    //else if (GetAlignNumber(i) == 3)
                                    //    strLog = strLog + "10_00_50_131_8HKJJH0P0A_ALIGN INSP CAM4_0_CAMERA__INSP_OV.jpg,";

                                    if (AlignUnit[GetAlignNumber(i)].PAT[0, 1].SearchResult)
                                        strLog += "OK,";
                                    else
                                        strLog += "NG,";
                                    strLog += "*,*,";
                                    int nUnitum = (int)(Main.INSP_RESULT.m_dPoint[i] * 1000);
                                    strLog = strLog + nUnitum.ToString() + ",";

                                    m_SWriterInsp.WriteLine(strLog);
                                }
                            }
                        }

                    }
                    catch (IOException e)
                    {
                        LogdataDisplay(e.Message, true);
                    }
                }

                if (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] != DEFINE.NORMAL_RUN) return;

                if (!Main.ftpManager.UpLoad(@"_rdpdat01/CAF_FI", strFolder + strFileName))
                {
                    LogdataDisplay("FTP UpLoad fail", true);
                }
            }

            public void WriteTrackOutFile()
            {
                bool bIsEmpty = false;
                string strFolder = "", strLog = "";
                string strFileName = "";
                strFolder = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\RCS\\";
                if (!Directory.Exists(LogdataPath)) Directory.CreateDirectory(LogdataPath);
                if (!Directory.Exists(strFolder)) Directory.CreateDirectory(strFolder);

                if (m_AlignName == "")
                    strFileName = m_AlignNo.ToString();

                lock (syncCSVLock)
                {
                    try
                    {

                        {
                            strFileName = "TRACKOUT_" + m_Cell_ID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
                            //if (!File.Exists(strFolder + m_AlignName + strFileName))
                            if (!File.Exists(strFolder + strFileName))
                                bIsEmpty = true;

                            //using (m_SWriterInsp = new StreamWriter(strFolder + m_AlignName + strFileName, true, Encoding.Unicode))
                            using (m_SWriterInsp = new StreamWriter(strFolder + strFileName, true, Encoding.UTF8))
                            {
                                strLog = "MODULE_ID=" + Main.machine.m_strModuleID;
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "PRODUCT_ID=" + ProjectInfo;
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "STEP_ID=*";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "CELL_ID=" + m_Cell_ID;
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "RESULT=";
                                if (true)
                                    strLog += "OK";
                                else
                                    strLog += "NG";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "RDP_TYPE=NETWORK";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "REASON_CODE=*";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "RDP_PATH=\\\\12.96.142.43\\_rdpdat01\\CAF_FI";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "TIME=" + AlignUnit[0].m_OutTime.ToString("yyyy-MM-dd HH:mm:ss");
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "ICS_LEVEL=1";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "ICS_TIMEOUT=1";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "ICS_JUDGEMENT=*";
                                m_SWriterInsp.WriteLine(strLog);

                                strLog = "ICS_OPERATOR_ID=*";
                                m_SWriterInsp.WriteLine(strLog);
                            }
                        }
                    }
                    catch (IOException e)
                    {
                        LogdataDisplay(e.Message, true);
                    }
                }

                if (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] != DEFINE.NORMAL_RUN) return;

                if (!Main.ftpManager.UpLoad(@"_trackout/MOD_TRIGGER/CAF_FI", strFolder + strFileName))
                {
                    LogdataDisplay("FTP UpLoad fail", true);
                }
            }


            public void InspecDataSave(string nMessage, bool nTimeDisPlay)
            {
                string Date;
                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff]");
                if (nTimeDisPlay)
                    nMessage = Date + "\t" + nMessage;
                if (machine.LogMsg_Onf) Save_Command(nMessage, DEFINE.INSPECTION);
            }
            public void LogdataDisplay(string nMessage, bool nTimeDisPlay)
            {
                string Date;
                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");
                if (nTimeDisPlay)
                    nMessage = Date + nMessage;
                if (machine.LogMsg_Onf)
                    Save_Command(nMessage, DEFINE.CMD);
                m_LogString.Enqueue(nMessage);
            }
            public void LogdataDisplayData(string nMessage)
            {
                string Date;
                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");
                nMessage = " CELL ID:" + m_PatTagNo.ToString() + "<-" + m_Cell_ID + " , " + Date + " , " + nMsgMarks + nMessage;
                if (machine.LogMsg_Onf)
                    Save_Command(nMessage, DEFINE.DATA);
            }
            public void PatternScoreResultSave(string nMessage)
            {
                string Date;
                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");
                nMessage = " CELL ID:" + m_PatTagNo.ToString() + "<-" + m_Cell_ID + " , " + Date + " , " + nMessage;
                if (machine.LogMsg_Onf) Save_Command(nMessage, DEFINE.MARKSCORE);
            }
            public void LogdataDisplay_Length(string nMessage, bool nTimeDisPlay, bool nCellIDUse = false)
            {
                string Date;
                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");
                if (!nTimeDisPlay) Date = "";

                nMessage = Date + nMessage;


                if (nCellIDUse)
                    nMessage = " CELL ID:" + m_PatTagNo.ToString() + "<-" + m_Cell_ID + " , " + nMessage;

                else
                    nMessage = nMessage;

                if (machine.LogMsg_Onf) Save_Command(nMessage, DEFINE.TABLENGTH);
                m_LogStringLength.Enqueue(nMessage);
            }
            public void LogDataSave(string nMessage, bool nTimeDisPlay, string Filetype)
            {
                string Date;
                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff]");
                if (nTimeDisPlay)
                    nMessage = Date + "\t" + nMessage;
                if (machine.LogMsg_Onf) Save_Command(nMessage, Filetype);
            }
            private void MainGriddataDisplay(int nPatNo)
            {
                string[][] nMessage = new string[1][];


                nMessage[0] = new string[] { PAT[m_PatTagNo,nPatNo].m_PatternName, PAT[m_PatTagNo,nPatNo].Pixel[DEFINE.X].ToString("0.000"), PAT[m_PatTagNo,nPatNo].Pixel[DEFINE.Y].ToString("0.000"), PAT[m_PatTagNo,nPatNo].m_RobotPosX.ToString("0.00"), PAT[m_PatTagNo,nPatNo].m_RobotPosY.ToString("0.00"),
                                                    m_StageX.ToString(), m_StageY.ToString(), m_StageT.ToString()};

                lock(m_MainGridString)
                    m_MainGridString.Enqueue(nMessage[0]);

                PatternPixelRobotSave(nPatNo);
            }
            private void PatternPixelRobotSave(int nPatNo)
            {
                if (!machine.LogMsg_Onf) return;

                string Date;
                string nSavemessage = "";

                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");

                nSavemessage += "\n" + "PixelX_" + nPatNo.ToString() + ": " + PAT[m_PatTagNo, nPatNo].Pixel[DEFINE.X].ToString("0.000") + ", " + "PixelY_" + nPatNo.ToString() + ": " + PAT[m_PatTagNo, nPatNo].Pixel[DEFINE.Y].ToString("0.000") + ", " +
                                "RobotPosX_" + nPatNo.ToString() + ": " + PAT[m_PatTagNo, nPatNo].m_RobotPosX.ToString("0.000") + ", " + "RobotPosY_" + nPatNo.ToString() + ": " + PAT[m_PatTagNo, nPatNo].m_RobotPosY.ToString("0.000") + ", ";
                nSavemessage = Date + nSavemessage;
                if (machine.LogMsg_Onf) Save_Command(nSavemessage, DEFINE.PIXEL);
            }
            private void MainGriddataDisplay()
            {
                string[][] nMessage = new string[1][];
                for (int i = 0; i < m_AlignPatMax[m_PatTagNo]; i++)
                {
                    nMessage[0] = new string[] { PAT[m_PatTagNo,i].m_PatternName, PAT[m_PatTagNo,i].Pixel[DEFINE.X].ToString("0.000"), PAT[m_PatTagNo,i].Pixel[DEFINE.Y].ToString("0.000"), PAT[m_PatTagNo,i].m_RobotPosX.ToString("0.000"), PAT[m_PatTagNo,i].m_RobotPosY.ToString("0.000"),
                                                     m_StageX.ToString("0.000"), m_StageY.ToString("0.000"), m_StageT.ToString("0.000")};
                    lock(m_MainGridString)
                        m_MainGridString.Enqueue(nMessage[0]);
                }
            }
            private void PatternPixelRobotSave()
            {
                if (!machine.LogMsg_Onf)
                    return;

                string Date;
                string nSavemessage = "";

                Date = DateTime.Now.ToString("[MM_dd HH:mm:ss:fff] ");

                for (int i = 0; i < m_AlignPatMax[m_PatTagNo]; i++)
                {
                    nSavemessage += "\n" + "PixelX_" + i.ToString() + ": " + PAT[m_PatTagNo, i].Pixel[DEFINE.X].ToString("0.000") + ", " + "PixelY_" + i.ToString() + ": " + PAT[m_PatTagNo, i].Pixel[DEFINE.Y].ToString("0.000") + ", " +
                                    "RobotPosX_" + i.ToString() + ": " + PAT[m_PatTagNo, i].m_RobotPosX.ToString("0.000") + ", " + "RobotPosY_" + i.ToString() + ": " + PAT[m_PatTagNo, i].m_RobotPosY.ToString("0.000") + ", ";
                }
                nSavemessage = Date + " " + m_Cell_ID + nSavemessage;
                if (machine.LogMsg_Onf)
                    Save_Command(nSavemessage, DEFINE.PIXEL);
            }

            object syncLock = new object();
            private void Save_Command(string nMessage, string nType)
            {
                string nFolder;
                string nFileName = "";
                nFolder = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\";

                if (!Directory.Exists(LogdataPath))
                    Directory.CreateDirectory(LogdataPath);
                if (!Directory.Exists(nFolder))
                    Directory.CreateDirectory(nFolder);

                lock (syncLock)
                {
                    try
                    {
                        switch (nType)
                        {
                            case DEFINE.CMD:
                                nFileName = "_Command.txt";
                                break;
                            case DEFINE.INSPECTION:
                                //    nFileName = "_" + m_PatTagNo.ToString() + "_GapResult.csv";
                                nFileName = "_" + m_PatTagNo.ToString() + "_GapResult.txt";
                                break;
                            case DEFINE.PIXEL:
                                nFileName = "_PIXEL_ROBOT.txt";
                                break;
                            case DEFINE.ALIGN:
                                nFileName = "_AlignResult.txt";
                                break;
                            case DEFINE.ERROR:
                                nFileName = "_ErrorList.txt";
                                break;
                            case DEFINE.TABLENGTH:
                                nFileName = "_Length.txt";
                                break;
                            case DEFINE.DATA:
                                nFileName = "_Data.csv";
                                break;
                            case DEFINE.MARKSCORE:
                                nFileName = "_MarkScore.txt";
                                break;
                        }

                        StreamWriter SW = new StreamWriter(nFolder + m_AlignName + nFileName, true, Encoding.UTF8);
                        SW.WriteLine(nMessage);
                        SW.Close();
                    }
                    catch
                    {

                    }
                }
            }
            private void RetryMove()
            {
                MotionMove(m_StageX, m_StageY, m_StageT);
            }
            private bool RetryDone()
            {

                if (!MotionDone()) return false;
                GetPosition();
                return true;
            }
            public bool MotionMove(double dx, double dy, double dt)
            {

                //-----------------------------------------------------------------------------------------------
                string LogMsg;
                int[] nDataBuf = new int[8];
                int nAddress;
                long m_dx, m_dy, m_dt;

                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                {
                    if (Main.CCLink_IsBit(Main.DEFINE.CCLINK_IN_PC_MOTOR_BUSY))
                    {
                        LogdataDisplay("MOVE_REQ Fail! MOTOR BUSY!", true);

                        return false;
                    }
                    else
                    {
                        Main.CCLink_WriteDWord(Main.DEFINE.CCLINK_WW_CAMERA_CALIB_STAGE_MOVE_DIST_X, (int)dx);
                        Main.CCLink_WriteDWord(Main.DEFINE.CCLINK_WW_CAMERA_CALIB_STAGE_MOVE_DIST_Y, (int)dy);

                        //     Main.CCLink.PutBit(Main.DEFINE.CCLINK_OUT_STAGE1_MOVE_REQ, true);
                    }
                }
                else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                {
                    if (Main.CCLink_IsBit(DEFINE.CCLINK2_IN_PRE_ALIGN1_CALIB_REQ))
                    {
                        Main.CCLink_WriteDWord(Main.DEFINE.CCLINK2_WW_PRE_ALIGN_SEARCH1_X, (int)dx);
                        Main.CCLink_WriteDWord(Main.DEFINE.CCLINK2_WW_PRE_ALIGN_SEARCH1_Y, (int)dy);
                    }
                    else if (Main.CCLink_IsBit(DEFINE.CCLINK2_IN_PRE_ALIGN2_CALIB_REQ))
                    {
                        Main.CCLink_WriteDWord(Main.DEFINE.CCLINK2_WW_PRE_ALIGN_SEARCH2_X, (int)dx);
                        Main.CCLink_WriteDWord(Main.DEFINE.CCLINK2_WW_PRE_ALIGN_SEARCH2_Y, (int)dy);
                    }
                    //        Main.CCLink.PutBit(Main.DEFINE.CCLINK2_OUT_PRE_ALIGN_STAGE_MOVE_REQ, true);

                }
                else
                {
                    m_dx = Convert_DtoL(dx);
                    m_dy = Convert_DtoL(dy);
                    //                 if (m_AlignName == "IC_PRE_ALIGN") m_dt = Theta_Link(dt);
                    //                 else 
                    if (m_AlignName == "PBD1" || m_AlignName == "PBD2")
                    {
                        m_dt = (long)dt;
                    }
                    else
                    {
                        m_dt = Convert_DtoL(dt);
                    }

                    nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.PLC_MOVE_END;
                    //YSH 임의주석, 필요 시 추가
                    //Main.WriteDevice(nAddress, 0);

                    nDataBuf[0] = (short)(m_dx & 0x0000ffff);
                    nDataBuf[1] = (short)((m_dx & 0xffff0000) >> 16);
                    nDataBuf[2] = (short)(m_dy & 0x0000ffff);
                    nDataBuf[3] = (short)((m_dy & 0xffff0000) >> 16);
                    nDataBuf[4] = (short)(m_dt & 0x0000ffff);
                    nDataBuf[5] = (short)((m_dt & 0xffff0000) >> 16);

                    nDataBuf[6] = 0;
                    nDataBuf[7] = 5;   //VIS_MOVE_REQ


                    nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.VIS_ALIGN_X;
                    //YSH 임의주석, 필요 시 추가
                    //Main.WriteDeviceBlock(nAddress, 8, ref nDataBuf);
                    System.Array.Clear(nDataBuf, 0, 7);
                }
                //-----------------------------------------------------------------------------------------------
                LogMsg = "MOVE_REQ(" + dx.ToString("0.0") + ", " + dy.ToString("0.0") + ", " + dt.ToString("0.0") + ")";
                //         LogMsg = LogMsg + " Time:" + m_Timer_index.GetEllapseTime().ToString("000");
                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------

                return true;
            }
            private bool MotionDone()
            {
                string LogMsg;
                if (!Main.DEFINE.OPEN_F)
                {
                    //if (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_MOVE_END] != 5)
                    if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                    {
                        if (Main.CCLink_OnCheckOffHandshake(DEFINE.CCLINK_IN_STAGE1_MOVE_COMP, DEFINE.CCLINK_OUT_STAGE1_MOVE_REQ, DEFINE.MOVE_TIMEOUT, m_AlignNo) == false)
                        {
                            return false;
                        }
                    }
                    else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                    {
                        if (Main.CCLink_OnCheckOffHandshake(DEFINE.CCLINK2_IN_PRE_ALIGN_STAGE_MOVE_COMP, DEFINE.CCLINK2_OUT_PRE_ALIGN_STAGE_MOVE_REQ, DEFINE.MOVE_TIMEOUT, m_AlignNo) == false)
                        {
                            return false;
                        }
                    }
                }
                //-----------------------------------------------------------------------------------------------
                LogMsg = "Move OK ";
                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
                return true;
            }
            public void GetPosition()
            {
                if (m_Cmd == 1160 || m_Cmd == 1061 || m_Cmd == 1150 || m_Cmd == 1110 || m_Cmd == 1120 || m_Cmd == 1261 || m_Cmd == 1262) return;

                string LogMsg = "GET-> ";
                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                {
                    //     m_AxisX = Main.CCLink.GetDWord(DEFINE.CCLINK_WW_STAGE_X_CUR_POS);
                    //      m_AxisY = Main.CCLink.GetDWord(DEFINE.CCLINK_WW_STAGE_Y_CUR_POS);
                    m_AxisT = 0;// Main.CCLink.GetDWord(DEFINE.CCLINK_WW_4028);

                    LogMsg = LogMsg + "X_Y : " + m_AxisX.ToString() + "," + m_AxisY.ToString();// + "," + m_AxisT.ToString() + " ";
                    LogdataDisplay(LogMsg, true);
                }
                else
                {
                    //MOTORPOSITION
                    m_AxisX = (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_AXIS_X + 1] << 16) | (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_AXIS_X] & 0x0000ffff);
                    m_AxisY = (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_AXIS_Y + 1] << 16) | (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_AXIS_Y] & 0x0000ffff);
                    m_AxisT = (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_AXIS_T + 1] << 16) | (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_AXIS_T] & 0x0000ffff);

                    m_AxisX = m_AxisX * m_DirX;
                    m_AxisY = m_AxisY * m_DirY;
                    m_AxisT = m_AxisT * m_DirT;

                    m_TAR_Distance = (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_TAR_MARK_LENTH + 1] << 16) | (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_TAR_MARK_LENTH] & 0x0000ffff);
                    m_OBJ_Distance = (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_OBJ_MARK_LENTH + 1] << 16) | (PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_OBJ_MARK_LENTH] & 0x0000ffff);

                    if ((m_AlignName == "PBD1" || m_AlignName == "PBD2") && (Main.DEFINE.PROGRAM_TYPE == "FOF_PC3" || Main.DEFINE.PROGRAM_TYPE == "FOF_PC4" || Main.DEFINE.PROGRAM_TYPE == "TFOG_PC2"))
                    {
                        m_AxisY = 0;
                    }

                    if (nMotResol > 1) // Motor Resolution 0.1um 단위
                    {
                        m_AxisX /= (double)nMotResol;
                        m_AxisY /= (double)nMotResol;
                        //      m_AxisT /= (double)nMotResol; //천안은 Theta
                        m_TAR_Distance /= (double)nMotResol;
                        m_OBJ_Distance /= (double)nMotResol;
                    }

                    if ((Main.DEFINE.PROGRAM_TYPE == "OLB_PC3" || Main.DEFINE.PROGRAM_TYPE == "OLB_PC4") && (m_AlignName == "PBD1" || m_AlignName == "PBD2"))
                    {
                        m_TAR_Distance = m_OBJ_Distance;
                    }

                    m_OffsetX = (short)(PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_OFFSET_X]);
                    m_OffsetY = (short)(PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_OFFSET_Y]);
                    m_OffsetT = (short)(PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PLC_OFFSET_T]);



                    //MARK TO MARK LENGTH
                    LogMsg = LogMsg + "X_Y_T:" + m_AxisX.ToString() + "," + m_AxisY.ToString() + "," + m_AxisT.ToString() + " ";
                    LogMsg = LogMsg + "OFFSET:" + m_OffsetX.ToString("") + "," + m_OffsetY.ToString("") + "," + m_OffsetT.ToString("") + " ";
                    LogMsg = LogMsg + "OBJ_DIS:" + m_OBJ_Distance.ToString() + "," + "TAR_DIS: " + m_TAR_Distance.ToString();

                    m_OffsetX = m_OffsetX * m_DirX;
                    m_OffsetY = m_OffsetY * m_DirY;
                    m_OffsetT = m_OffsetT * m_DirT;

                    if ((Main.DEFINE.PROGRAM_TYPE == "OLB_PC3" || Main.DEFINE.PROGRAM_TYPE == "OLB_PC4") && (m_AlignName == "PBD1" || m_AlignName == "PBD2"))
                    {
                        m_PBD_ReAlignY = (short)(PLCDataTag.RData[ALIGN_UNIT_ADDR + Main.DEFINE.PBD_REALIGN_DIS_Y]);
                        LogMsg = LogMsg + " ReAlign_Y_DIS:" + m_PBD_ReAlignY.ToString();
                    }
                    LogdataDisplay(LogMsg, true);

                    if (m_AlignName == "LOAD_ALIGN")
                    {
                        m_AxisX = 0;
                        m_AxisY = 0;
                    }



                    if ((Main.DEFINE.PROGRAM_TYPE == "OLB_PC1" && m_AlignName == "PLASMA_PRE") || (Main.DEFINE.PROGRAM_TYPE == "FOB_PC4" && m_AlignName == "ART_PRE"))
                        m_AxisX = m_AxisX + (m_AxisT * -1);
                    //왼쪽 기준 카메라에 X축이 있어서. 위치가 바뀜에 따라서 좌표를 연동 해야되서.
                    // m_AxisT -> Camera의 축을 연동하고, 부호를 - 로 곱하고 난뒤에 기존 m_AxisX 축과 플러스 한다. 
                }
            }
            private void Log_Cmd(int nCmd)
            {
                int nBackCmd;
                nBackCmd = nCmd;
                string LogMsg = " ";
                try
                {
                    switch (Math.Abs(nCmd))
                    {
                        case CMD.OBJ_SEARCH_LEFT:
                        case CMD.GRAB_LEFT:
                            LogMsg = "OBJECT_LEFT_SEARCH";
                            break;
                        case CMD.OBJ_SEARCH_RIGHT:
                        case CMD.GRAB_RIGHT:
                            LogMsg = "OBJECT_RIGHT_SEARCH";
                            break;
                        case CMD.OBJ_SEARCH_ALL:
                            LogMsg = "OBJECT_ALL_SEARCH";
                            break;

                        case CMD.TAR_SEARCH_LEFT:
                            LogMsg = "TARGET_LEFT_SEARCH";
                            break;
                        case CMD.TAR_SEARCH_RIGHT:
                            LogMsg = "TARGET_RIGHT_SEARCH";
                            break;
                        case CMD.TAR_SEARCH_ALL:
                            LogMsg = "TARGET_ALL_SEARCH";
                            break;

                        case CMD.LEFT_SEARCH_ALL:
                            LogMsg = "LEFT_ALL_SEARCH";
                            break;
                        case CMD.RIGHT_SEARCH_ALL:
                            LogMsg = "RIGHT_ALL_SEARCH";
                            break;

                        case CMD.OBJTAR_SEARCH_ALL:
                            LogMsg = "OBJTAR_ALL_SEARCH";
                            break;

                        case CMD.ALIGN_CENTER:
                            LogMsg = "CENTER_ALIGN";
                            break;
                        case CMD.ALIGN_CENTER_REALIGN:
                            LogMsg = "CENTER_REALIGN_";
                            break;
                        case CMD.ALIGN_OBJECT:
                            LogMsg = "ALIGN";
                            break;

                        case CMD.ALIGN_TARGET_REALIGN:
                        case CMD.ALIGN_OBJECT_REALIGN:
                            LogMsg = "REALIGN_";
                            break;

                        case CMD.ALIGN_TARGET:
                            LogMsg = "TAR ALIGN";
                            break;

                        case CMD.ALIGN_OBJTAR_ALL:
                            LogMsg = "OBJTAR ALIGN";
                            break;
                        case CMD.ALIGN_OBJTAR_ALL_REALIGN:
                            LogMsg = "OBJTAR REALIGN_";
                            break;

                        case CMD.ALIGN_1CAM_2SHOT_LEFT:
                            LogMsg = "1CAM_2SHOT_LEFT_ALIGN";
                            break;
                        case CMD.ALIGN_1CAM_2SHOT_RIGHT:
                            LogMsg = "1CAM_2SHOT_RIGHT_ALIGN";
                            break;

                        case CMD.ALIGN_1CAM_4SHOT_LEFT:
                            LogMsg = "1CAM_4SHOT_LEFT_ALIGN";
                            break;
                        case CMD.ALIGN_1CAM_4SHOT_RIGHT:
                            LogMsg = "1CAM_4SHOT_RIGHT_ALIGN";
                            break;

                        case CMD.REEL_ALIGN:
                            LogMsg = "REEL_ALIGN";
                            break;

                        case CMD.CRD_ALIGN_LEFT:
                        case CMD.CRD_ALIGN_RIGHT:
                            LogMsg = "CRD_ALIGN";
                            break;

                        case CMD.ACF_BLOB_LEFT:
                            LogMsg = "BLOB_LEFT_SEARCH";
                            break;
                        case CMD.ACF_BLOB_RIGHT:
                            LogMsg = "BLOB_RIGHT_SEARCH";
                            break;
                        case CMD.ACF_BLOB_OBJ_ALL:
                            LogMsg = "BLOB_ALL_SEARCH";
                            break;

                        case CMD.ACF_BLOB_TAR_LEFT:
                            LogMsg = "BLOB_TAR_LEFT";
                            break;
                        case CMD.ACF_BLOB_TAR_RIGHT:
                            LogMsg = "BLOB_TAR_RIGHT";
                            break;
                        case CMD.ACF_BLOB_TAR_ALL:
                            LogMsg = "BLOB_TAR_ALL";
                            break;


                        case CMD.ALIGN_INSPECT_LEFT:
                            LogMsg = "ALIGN_INSPECTION_LEFT";
                            break;
                        case CMD.ALIGN_INSPECT_RIGHT:
                            LogMsg = "ALIGN_INSPECTION_RIGHT";
                            break;

                        case CMD.CALIPER_SEARCH_LEFT:
                        case CMD.CALIPER_SEARCH_LEFT_ACFCUT:
                            LogMsg = "OBJECT_LEFT_CALIPER_SEARCH";
                            break;
                        case CMD.CALIPER_SEARCH_RIGHT:
                        case CMD.CALIPER_SEARCH_RIGHT_ACFCUT:
                            LogMsg = "OBJECT_RIGHT_CALIPER_SEARCH";
                            break;

                        case CMD.CALIPER_OBJ_SEARCH_ALL:
                            LogMsg = "OBJECT_ALL_CALIPER_SEARCH";
                            break;
                        case CMD.CIRCLE_OBJ_SEARCH_LEFT:
                            LogMsg = "OBJECT_LEFT_CIRCLE_SEARCH";
                            break;
                        case CMD.CIRCLE_OBJ_SEARCH_RIGHT:
                            LogMsg = "OBJECT_RIGHT_CIRCLE_SEARCH";
                            break;

                        case CMD.CIRCLE_OBJ_SEARCH_ALL:
                            LogMsg = "OBJECT_ALL_CIRCLE_SEARCH";
                            break;

                        case CMD.CIRCLE_TAR_SEARCH_LEFT:
                            LogMsg = "TARGET_LEFT_CIRCLE_SEARCH";
                            break;
                        case CMD.CIRCLE_TAR_SEARCH_RIGHT:
                            LogMsg = "TARGET_RIGHT_CIRCLE_SEARCH";
                            break;

                        case CMD.CIRCLE_TAR_SEARCH_ALL:
                            LogMsg = "TARGET_ALL_CIRCLE_SEARCH";
                            break;

                        case CMD.CALIPER_ALIGN_1CAM_2SHOT_LEFT:
                            LogMsg = "1CAM_2SHOT_LEFT_ALIGN_CALIPER";
                            break;
                        case CMD.CALIPER_ALIGN_1CAM_2SHOT_RIGHT:
                            LogMsg = "1CAM_2SHOT_RIGHT_ALIGN_CALIPER";
                            break;


                        case CMD.ALIGN_TRAY_SEARCH:
                            LogMsg = "TRAY_SEARCH";
                            break;

                        case CMD.ALIGN_TRAY_TRIGER:
                            LogMsg = "TRAY_TRIGER";
                            break;

                        case CMD.BLOB_TRAY_SEARCH:
                            LogMsg = "TRAY_BlobSEARCH";
                            break;
                        case CMD.BLOB_TRAY_TRIGER_PICKER:
                            LogMsg = "TRAY_BlobPICKER";
                            break;

                        case CMD.ALIGN_CIRCLE_INSPECT_LEFT:
                            LogMsg = "CIRCLE_INSPECTION_LEFT";
                            break;
                        case CMD.ALIGN_CIRCLE_INSPECT_RIGHT:
                            LogMsg = "CIRCLE_INSPECTION_RIGHT";
                            break;


                        case CMD.CALIPER_ALIGN_CENTER:
                            LogMsg = "ALIGN_CENTER_CALIPER";
                            break;

                        case CMD.DOPO_INSPECT_LEFT:
                            LogMsg = "DOPO_INSPEC_LEFT_SEARCH";
                            break;
                        case CMD.DOPO_INSPECT_RIGHT:
                            LogMsg = "DOPO_INSPEC_RIGHT_SEARCH";
                            break;

                        case CMD.FINDLINE_OBJ_LEFT:
                            LogMsg = "FINDLINE_LEFT_SEARCH";
                            break;

                        case CMD.FINDLINE_OBJ_RIGHT:
                            LogMsg = "FINDLINE_RIGHT_SEARCH";
                            break;

                        case CMD.OBJ_CALRIBRATION_LEFT:
                        case CMD.OBJ_CALRIBRATION_RIGHT:
                        case CMD.TAR_CALRIBRATION_LEFT:
                        case CMD.TAR_CALRIBRATION_RIGHT:
                        case CMD.HEAD_CALRIBRATION:
                            LogMsg = "CALRIBRATION";
                            break;
                        case CMD.OBJ_CAL_POS_LEFT:
                            LogMsg = "CAL_POS_LEFT";
                            break;
                        case CMD.OBJ_CAL_POS_RIGHT:
                            LogMsg = "CAL_POS_RIGHT";
                            break;

                        case CMD.REQ_INSPECTION: //CYH
                            LogMsg = "WELDING_INSPECTION";
                            break;

                        default: LogMsg = "NOTHING?"; break;
                    }
                    short nDataBuf;
                    int nAddress;

                    nDataBuf = (short)0;
                    nAddress = PLCDataTag.BASE_RW_ADDR + ALIGN_UNIT_ADDR + Main.DEFINE.PLC_CMD;

                    //2022 05 09 YSH
                    //Main.WriteDevice(nAddress, nDataBuf);

                    int[] setValue = new int[1];
                    setValue[0] = nDataBuf;
                    Main.PLCsocket.WriteDevice_W(nAddress.ToString(), 1, setValue);
                }
                catch
                {

                }

                //-----------------------------------------------------------------------------------------------
                nMsgMarks = "";
                LogMsg = "<-" + LogMsg + "_START";
                if (nBackCmd < 0) LogMsg = LogMsg + "_ERROR";
                LogdataDisplay(LogMsg, true);
                //-----------------------------------------------------------------------------------------------
            }
            private void Log_Error()
            {
                string LogMsg = " ";
                int nErrorType = m_errSts;

                if (Main.ALIGNINSPECTION_USE(m_AlignNo))
                {
                    switch (m_PatTagNo)
                    {
                        case 0:
                            LogMsg = "STAGE1:";
                            break;
                            //                         case 1:
                            //                             LogMsg = "STAGE2:";
                            //                             break;
                            //                         case 2:
                            //                             LogMsg = "STAGE3:";
                            //                             break;
                            //                         case 3:
                            //                             LogMsg = "STAGE4:";
                            //                             break;
                    }
                }

                switch (nErrorType)
                {
                    case DEFINE.OBJ_L:
                    case DEFINE.OBJ_R:
                    case DEFINE.TAR_L:
                    case DEFINE.TAR_R:
                        LogMsg = Main.AlignUnit[m_AlignNo].PAT[m_PatTagNo, nErrorType].m_PatternName + "_SEARCH_ERROR";
                        break;

                    case DEFINE.OBJ_ALL:
                        LogMsg = "OBJ_ALL_SEARCH_ERROR";
                        break;

                    case DEFINE.TAR_ALL:
                        LogMsg = "TAR_ALL_SEARCH_ERROR";
                        break;

                    case DEFINE.OBJTAR_ALL:
                        LogMsg = "OBJTAR_SEARCH_ERROR";
                        break;

                    case ERR.LENTH + DEFINE.OBJ_ALL:
                        LogMsg = "OBJECT_LENGTH_ERROR";
                        break;

                    case ERR.LENTH + DEFINE.TAR_ALL:
                        LogMsg = "TARGET_LENGTH_ERROR";
                        break;

                    case ERR.LENTH + DEFINE.OBJTAR_ALL:
                        LogMsg = "OBJECT TARGET_LENGTH_ERROR";
                        break;

                    case ERR.INSPEC + DEFINE.OBJ_ALL:
                        LogMsg += "WIDTH_HEIGHT_ERROR";
                        break;

                    case ERR.INSPEC + DEFINE.WIDTH_:
                        LogMsg += "WIDTH_ERROR";
                        break;

                    case ERR.INSPEC + DEFINE.HEIGHT:
                        LogMsg += "HEIGHT_ERROR";
                        break;

                    case ERR.INSPEC + DEFINE.EXTENSION:
                        LogMsg += "EXTENSION_X_ERROR";
                        break;

                    case ERR.PLC_MOVE_END:
                        LogMsg = "PLC_MOVE_END_ERROR";
                        break;
                    case ERR.MOTER_LIMIT:
                        LogMsg = "MOTOR_MOVE_LIMIT_ERROR";
                        break;

                    case ERR.ALIGN_REPEAT_LIMIT:
                        LogMsg = "REALIGN_REPEAT_LIMIT_ERROR";
                        break;

                    default:
                        LogMsg = "??";
                        break;
                }
                //-----------------------------------------------------------------------------------------------
                LogMsg = m_Cell_ID + " <-" + LogMsg;
                //-----------------------------------------------------------------------------------------------
                LogdataDisplay(LogMsg, true);
                LogDataSave(LogMsg, true, DEFINE.ERROR);
            }
            private bool Manual_Matching(int nPatNum)
            {
                bool nResult = false;
                int seq = 0;
                bool LoopFlag = true;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            m_ManualMatchFlag = true;
                            m_ManualMatchPatNum = nPatNum;
                            m_ManualMatchPatTagNum = m_PatTagNo;
                            seq++;
                            break;

                        case 1:
                            if (m_ManualMatchFlag) break;
                            nResult = m_ManualMatchResult;
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ:
                            LoopFlag = false;
                            break;

                    }
                }
                return nResult;
            }
            public long Convert_DtoL(double nData)
            {
                long nlongData = 0;

                if (nMotResol == 1) nData = Math.Round(nData); // 1 마이크로 제어시 , 반올림하기.

                nlongData = (long)(nData * nMotResol);       // AlignUnit 별 배율 조정.

                return nlongData;
            }
            public double Truncate(double nData) //소수점 절삭할때.
            {
                double nlongData = 0;
                nlongData = Math.Truncate(nData * 10) / 10;

                return nlongData;
            }
            public long Theta_Link(double dt)
            {
                double Motor_Circle_Radius, Link_Circle_Radius;
                double nDt, ResultT;
                long nlongData;

                Link_Circle_Radius = 53000;		// Stage 쪽 53000um
                Motor_Circle_Radius = 13000;	// Motor 쪽 13000um

                nDt = dt * DEFINE.radian / 1000.0;
                ResultT = Math.Atan((Link_Circle_Radius * Math.Sin(nDt)) / (Motor_Circle_Radius + Link_Circle_Radius * (1 - Math.Cos(nDt))));
                ResultT = ResultT * DEFINE.degree * 1000.0;

                nlongData = Convert_DtoL(ResultT);

                return nlongData;
            }
            public void DrawResultALL(CogRecordDisplay Display, int nType)
            {
                List<PMResult> tempPMResult = new List<PMResult>();
                Main.DisplayClear(Display);

                List<int> nPatNo = new List<int>();
                try
                {
                    switch (nType)
                    {
                        case Main.DEFINE.OBJ_ALL:
                            nPatNo.Add(Main.DEFINE.OBJ_L);
                            nPatNo.Add(Main.DEFINE.OBJ_R);
                            break;

                        case Main.DEFINE.TAR_ALL:
                            nPatNo.Add(Main.DEFINE.TAR_L);
                            nPatNo.Add(Main.DEFINE.TAR_R);
                            break;

                        case Main.DEFINE.LEFT_ALL:
                            nPatNo.Add(Main.DEFINE.OBJ_L);
                            nPatNo.Add(Main.DEFINE.TAR_L);
                            break;

                        case Main.DEFINE.RIGHT_ALL:
                            nPatNo.Add(Main.DEFINE.OBJ_R);
                            nPatNo.Add(Main.DEFINE.TAR_R);
                            break;

                        case Main.DEFINE.OBJTAR_ALL:
                            nPatNo.Add(Main.DEFINE.OBJ_L);
                            nPatNo.Add(Main.DEFINE.OBJ_R);
                            nPatNo.Add(Main.DEFINE.TAR_L);
                            nPatNo.Add(Main.DEFINE.TAR_R);
                            break;

                        case Main.DEFINE.CHIPPAT_ALL:
                            nPatNo.Add(Main.DEFINE.OBJ_L);
                            nPatNo.Add(Main.DEFINE.OBJ_R);
                            nPatNo.Add(Main.DEFINE.TAR_L);
                            break;

                    }

                    Display.Image = PAT[m_PatTagNo, nPatNo[0]].TempImage;

                    for (int i = 0; i < nPatNo.Count; i++)
                        tempPMResult.Add(PAT[m_PatTagNo, nPatNo[i]].PMAlignResult);

                    DrawOverlayPMAlign(Display, tempPMResult);

                    if (m_GD_ImageSave_Use && PAT[m_PatTagNo, nPatNo[0]].ImageSaveResult) PAT[m_PatTagNo, nPatNo[0]].Save_Image("OK", Display);
                    if (m_NG_ImageSave_Use && !PAT[m_PatTagNo, nPatNo[0]].ImageSaveResult) PAT[m_PatTagNo, nPatNo[0]].Save_Image("NG", Display);
                }
                catch
                {

                }
            }

            [Flags]
            public enum FindLineConstants
            {
                None = -1,

                LineX = 1 << 0,
                LineY = 1 << 1,
                LineDiag = 1 << 2,
                LinePOL = 1 << 3,
                CrossXY = 1 << 4,
                CrossXD = 1 << 5,
                CrossYD = 1 << 6,
                CircleR = 1 << 7,
                ShapeCut = 1 << 8,
                BeamSize = 1 << 9,
            }

            [Flags]
            public enum GRAB_CMD
            {
                NONE = 0,

                SEARCH_X = 1 << 0,
                SEARCH_Y = 1 << 1,
                SEARCH_C = 1 << 2,
                COMMAND3 = 1 << 3,
                COMMAND4 = 1 << 4,
                COMMAND5 = 1 << 5,
                COMMAND6 = 1 << 6,
                SEARCH_R = 1 << 7,
                SEARCH_S = 1 << 8,
                BEAMSIZE = 1 << 9,
                PATMATCH = 1 << 10,
            }

            public int GetAlignNumber(int nPointNo)
            {
                int nANo = 0;

                switch (nPointNo)
                {
                    case 0:
                        nANo = 0;
                        break;
                    case 1:
                    case 2:
                    case 6:
                        nANo = 2;
                        break;
                    case 3:
                    case 4:
                    case 7:
                        nANo = 3;
                        break;
                    case 5:
                        nANo = 1;
                        break;
                }

                return nANo;
            }

        }// AlignUnitTag
    }
}
