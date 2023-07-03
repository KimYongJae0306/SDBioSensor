using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
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
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.CNLSearch;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro.LineMax;
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.Dimensioning;



namespace COG
{
    public partial class Main
    {
        public partial class PatternTag
        {
            private AlignUnitTag AlignUnitTag_;
            public void PatAlloc(int CAM0, string PatternName)
            {
                m_CamNo = CAM0;
                m_PatternName = PatternName;
                m_DisNo = m_CamNo;
                m_AlignName = Main.AlignUnit[m_PatAlign_No].m_AlignName;
                if (DEFINE.OPEN_CAM) m_CamNo = 0;

                m_PatternName_Short = PatternName.Replace(Main.AlignUnit[m_PatAlign_No].m_AlignName, "");
            }
            public bool Search()
            {
                return Search_PATCNL(m_CamNo);
            }
            public bool Search_PATCNL(int CAM)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool Ret = false;

                CogSearchMaxResults tempResult = null;
                CogPMAlignResults tempGResult = null;

                CogIPOneImageTool mOneImageTool = new CogIPOneImageTool();

                SearchSubNum = 0;

                double nTempV2R_X = 0;
                double nTempV2R_Y = 0;
                try
                {
                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                PMAlignResult.m_PatternName = m_PatternName;
                                PMAlignResult.m_CamNo = m_CamNo;
                                PMAlignResult.m_ACCeptScore = m_ACCeptScore;
                                PMAlignResult.m_GACCeptScore = m_GACCeptScore;

                                PMAlignResult.m_Align_No = m_PatAlign_No;
                                PMAlignResult.m_PatTagNo = Main.AlignUnit[m_PatAlign_No].m_PatTagNo;
                                PMAlignResult.m_PatNo = m_PatNo;

                                seq++;
                                break;

                            case 1:
                                PMAlignResult.CNLSearchResult = null;
                                PMAlignResult.PMAlignResult = new CogPMAlignResults();
                                PMAlignResult.m_PMAlign = false;
                                PMAlignResult.m_Rotation = 0;
                                PMAlignResult.Score = 0;

                                seq++;
                                break;

                            case 2://SEARCH_MAX
                                if (Pattern[Main.DEFINE.MAIN] != null)
                                {
                                    if (TempImage == null)
                                    {
                                        return false;
                                    }
                                    //--------------------------------------------------------------------------------------
                                    Pattern[Main.DEFINE.MAIN].InputImage = TempImage;
                                    Pattern[Main.DEFINE.MAIN].RunParams.MaximumNumberToFind = 1;
                                    //--------------------------------------------------------------------------------------
//                                     if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE" && m_PatNo == Main.DEFINE.TAR_L)
//                                     {
//  //                                           Pattern[Main.DEFINE.MAIN].RunParams.ZoneUsePattern = true;
//                                             Pattern[Main.DEFINE.MAIN].RunParams.MaximumNumberToFind = Main.AlignUnit[m_PatAlign_No].m_Tray_Pocket_X * Main.AlignUnit[m_PatAlign_No].m_Tray_Pocket_Y;
//                                             if (Pattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold < Main.DEFINE.DEFAULT_ACCEPT_SCORE)
//                                                 Pattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = Main.DEFINE.DEFAULT_ACCEPT_SCORE;
//                                             else
//                                                 Pattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = m_ACCeptScore;
//                                         
//                                     }                             
                                    //---------------------------------------------------------------------------------------
                                    Pattern[Main.DEFINE.MAIN].Run();
                                    tempResult = Pattern[Main.DEFINE.MAIN].Results;
                                }
                                seq++;
                                break;

                            case 3:
                                if (tempResult != null && tempResult.Count >= 1)
                                {
                                    PMAlignResult.m_Rotation = tempResult[0].GetPose().Rotation;
                                    PMAlignResult.CNLSearchResult = tempResult;
                                    PMAlignResult.Score = tempResult[0].Score;
                                    if (tempResult[0].Score >= m_ACCeptScore && tempResult[0].Score  <= Main.DEFINE.m_ACCeptScore100)
                                    {
                                        seq = SEQ.OK_SEQ;
                                        break;
                                    }
                                }
                                seq++;
                                break;

                            case 4:
                                for (int i = 1; i < Main.DEFINE.SUBPATTERNMAX; i++)
                                {
                                    if (Pattern_USE[i] && Pattern[i].Pattern.Trained)
                                    {
                                        Pattern[i].InputImage = TempImage;
                                        Pattern[i].RunParams.MaximumNumberToFind = 1;
                                        //--------------------------------------------------------------------------------------
//                                         if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE" && m_PatNo == Main.DEFINE.TAR_L)
//                                         {
//                                             Pattern[i].RunParams.ZoneUsePattern = true;
//                                             Pattern[Main.DEFINE.MAIN].RunParams.MaximumNumberToFind = Main.AlignUnit[m_PatAlign_No].m_Tray_Pocket_X * Main.AlignUnit[m_PatAlign_No].m_Tray_Pocket_Y;
//                                             if (Pattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold < Main.DEFINE.DEFAULT_ACCEPT_SCORE)
//                                                 Pattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = Main.DEFINE.DEFAULT_ACCEPT_SCORE;
//                                             else
//                                                 Pattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = m_ACCeptScore;
// 
//                                         }    
                                        Pattern[i].Run();
                                        tempResult = Pattern[i].Results;
                                        if (tempResult != null && tempResult.Count >= 1)
                                        {
                                            PMAlignResult.m_Rotation = tempResult[0].GetPose().Rotation;
                                            PMAlignResult.CNLSearchResult = tempResult;

                                            if (tempResult[0].Score >= m_ACCeptScore && tempResult[0].Score <= Main.DEFINE.m_ACCeptScore100)
                                            {
                                                Ret = true;
                                                SearchSubNum = i;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (Ret)
                                    seq = SEQ.OK_SEQ;
                                else
                                {
                                    if (m_PMAlign_Use)
                                        seq = 5;
                                    else
                                        seq = SEQ.NG_SEQ; //NG

                                }
                                break;


                            case 5://PMALIGN START
                                if (GPattern[Main.DEFINE.MAIN] != null)
                                {
                                    GPattern[Main.DEFINE.MAIN].InputImage = TempImage;

                                    //--------------------------------------------------------------------------------------
//                                     if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE" && m_PatNo == Main.DEFINE.TAR_L)
//                                     {
//                                         if (GPattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold < Main.DEFINE.DEFAULT_ACCEPT_SCORE)
//                                             GPattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = Main.DEFINE.DEFAULT_ACCEPT_SCORE;
//                                         else
//                                             GPattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = m_GACCeptScore;
//                                      }    

                                    GPattern[Main.DEFINE.MAIN].Run();
                                    tempGResult = GPattern[Main.DEFINE.MAIN].Results;
                                }
                                seq++;
                                break;

                            case 6:
                                PMAlignResult.m_PMAlign = true;
                                if (tempGResult != null && tempGResult.Count >= 1)
                                {
                                    PMAlignResult.m_Rotation = tempGResult[0].GetPose().Rotation;
                                    PMAlignResult.PMAlignResult = tempGResult;
                                    if (tempGResult[0].Score >= m_GACCeptScore && tempGResult[0].Score <= Main.DEFINE.m_ACCeptScore100)
                                    {
                                        seq = SEQ.OK_SEQ + 1;
                                        break;
                                    }
                                }
                                seq++;
                                break;

                            case 7:
                                for (int i = 1; i < Main.DEFINE.SUBPATTERNMAX; i++)
                                {
                                    if (Pattern_USE[i] && GPattern[i].Pattern.Trained)
                                    {
                                        GPattern[i].InputImage = TempImage;
                                        GPattern[i].RunParams.ApproximateNumberToFind = 1;
                                        //--------------------------------------------------------------------------------------
//                                         if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE" && m_PatNo == Main.DEFINE.TAR_L)
//                                         {
//                                             if (GPattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold < Main.DEFINE.DEFAULT_ACCEPT_SCORE)
//                                                 GPattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = Main.DEFINE.DEFAULT_ACCEPT_SCORE;
//                                             else
//                                                 GPattern[Main.DEFINE.MAIN].RunParams.AcceptThreshold = m_GACCeptScore;
//                                         } 
                                        GPattern[i].Run();
                                        tempGResult = GPattern[i].Results;
                                        if (tempGResult != null && tempGResult.Count >= 1)
                                        {
                                            PMAlignResult.PMAlignResult = tempGResult;
                                            PMAlignResult.m_Rotation = tempGResult[0].GetPose().Rotation;
                                            if (tempGResult[0].Score >= m_GACCeptScore && tempGResult[0].Score <= Main.DEFINE.m_ACCeptScore100)
                                            {
                                                Ret = true;
                                                SearchSubNum = i;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (Ret)
                                    seq = SEQ.OK_SEQ + 1; //PM ALIGN OK
                                else
                                    seq = SEQ.NG_SEQ; //NG
                                break;

                            case SEQ.OK_SEQ: //--------------------------------------------------------------CNLSEARCH
                                double nTempTransX = 0, nTempTransY = 0;
                                nTempTransX = tempResult[0].GetPose().TranslationX;
                                nTempTransY = tempResult[0].GetPose().TranslationY;
                                PMAlignResult.Score = tempResult[0].Score;

                                #region
                                
//                                 if ((m_PatNo == Main.DEFINE.TAR_L && Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE"))
//                                 {
//                                     Main.AlignUnit[m_PatAlign_No].m_PatternPoints.Clear();
// 
//                                     if (tempResult != null && tempResult.Count >= 1)
//                                     {
//                                         List<DoublePoint> TempPoint = new List<DoublePoint>();
//                                         DoublePoint[] ResultPoint = new DoublePoint[tempResult.Count];
//                                        
// 
//                                         for (int i = 0; i < tempResult.Count; i++)
//                                         {
//                                             DoublePoint nTempPoint = new DoublePoint();
//                                             if (tempResult[i].Score >= m_ACCeptScore)
//                                             {
//                                                 nTempPoint.X = tempResult[i].GetPose().TranslationX;
//                                                 nTempPoint.Y = tempResult[i].GetPose().TranslationY;
//                                                 TempPoint.Add(nTempPoint);
//                                             }
// 
//                                         }
// 
//                                         //  var sortedPoints = TempPoint.OrderBy(p => p.X).ThenBy(p => p.Y); //추후 선택 하든지 . 
//                                         // var sortedPoints = TempPoint.OrderBy(p => p.Y).ToList();                   //오름 차순 Image 위상단 먼저
//                                         var sortedPoints = TempPoint.OrderByDescending(p => p.Y).ToList();            // 내림 차순 Image 하단 기준
//                                         int nCount = 0;
//                                         foreach (var p in sortedPoints)
//                                         {
//                                             ResultPoint[nCount] = new DoublePoint();
//                                             ResultPoint[nCount].X = p.X;
//                                             ResultPoint[nCount].Y = p.Y;
//                                             Main.AlignUnit[m_PatAlign_No].m_PatternPoints.Add(ResultPoint[nCount]);
//                                             nCount++;
//                                         }
// 
//                                         for (int j = 0; j < tempResult.Count; j++)
//                                         {
//                                             if (Main.AlignUnit[m_PatAlign_No].m_PatternPoints[0].X == tempResult[j].GetPose().TranslationX 
//                                                 && Main.AlignUnit[m_PatAlign_No].m_PatternPoints[0].Y == tempResult[j].GetPose().TranslationY)      // Searchmax만 PmAlign 추가해야될듯.
//                                             {
//                                                 Main.AlignUnit[m_PatAlign_No].m_PatternNumber = j;
//                                             }
//                                         }
//                                         nTempTransX = ResultPoint[0].X;
//                                         nTempTransY = ResultPoint[0].Y;
//                                         PMAlignResult.m_Rotation    = tempResult[Main.AlignUnit[m_PatAlign_No].m_PatternNumber].GetPose().Rotation;
//                                         PMAlignResult.Score         = tempResult[Main.AlignUnit[m_PatAlign_No].m_PatternNumber].Score;
//                                     }
//                                     Main.AlignUnit[m_PatAlign_No].m_Chip_SearchCount = Main.AlignUnit[m_PatAlign_No].m_PatternPoints.Count;
//                                 }
                                #endregion                     
    
                                PMAlignResult.m_Pixel[DEFINE.X] = Pixel[DEFINE.X] = nTempTransX;
                                PMAlignResult.m_Pixel[DEFINE.Y] = Pixel[DEFINE.Y] = nTempTransY;

                                V2R(ref nTempV2R_X, ref nTempV2R_Y);
                                //V2RPIXEL(ref Pixel_Center[Main.DEFINE.X], ref Pixel_Center[Main.DEFINE.Y]);
                                PMAlignResult.m_RobotPosX = m_RobotPosX = nTempV2R_X;
                                PMAlignResult.m_RobotPosY = m_RobotPosY = nTempV2R_Y;
                                Ret = true;
                                PMAlignResult.CNLSearchResult = tempResult;
                                if (Pattern[SearchSubNum].SearchRegion != null)
                                    PMAlignResult.SearchRegion = new CogRectangle(Pattern[SearchSubNum].SearchRegion as CogRectangle);
                                else
                                {
                                    Pattern[SearchSubNum].SearchRegion = new CogRectangle();
                                    (Pattern[SearchSubNum].SearchRegion as CogRectangle).SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[m_CamNo], Main.vision.IMAGE_CENTER_Y[m_CamNo], Main.vision.IMAGE_SIZE_X[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA, Main.vision.IMAGE_SIZE_Y[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA);
                                    PMAlignResult.SearchRegion = new CogRectangle(Pattern[SearchSubNum].SearchRegion as CogRectangle);
                                }
                                    seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.OK_SEQ + 1://------------------------------------------------------------PMALIGN
                                double nGTempTransX = 0, nGTempTransY = 0;
                                nGTempTransX = tempGResult[0].GetPose().TranslationX;
                                nGTempTransY = tempGResult[0].GetPose().TranslationY;
                                PMAlignResult.Score = tempGResult[0].Score;

                                #region

//                                 if ((m_PatNo == Main.DEFINE.TAR_L && Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE"))
//                                 {
//                                     Main.AlignUnit[m_PatAlign_No].m_PatternPoints.Clear();
// 
//                                     if (tempGResult != null && tempGResult.Count >= 1)
//                                     {
//                                         List<DoublePoint> TempPoint = new List<DoublePoint>();
//                                         DoublePoint[] ResultPoint = new DoublePoint[tempResult.Count];
// 
//                                         for (int i = 0; i < tempGResult.Count; i++)
//                                         {
// 
//                                             DoublePoint nTempPoint = new DoublePoint();
//                                             if (tempGResult[i].Score >= m_ACCeptScore)
//                                             {
//                                                 nTempPoint.X = tempGResult[i].GetPose().TranslationX;
//                                                 nTempPoint.Y = tempGResult[i].GetPose().TranslationY;
//                                                 TempPoint.Add(nTempPoint);
//                                             }
//                                         }
// 
//                                         //  var sortedPoints = TempPoint.OrderBy(p => p.X).ThenBy(p => p.Y); //추후 선택 하든지 . 
//                                         var sortedPoints = TempPoint.OrderBy(p => p.Y).ToList();
// 
//                                         int nCount = 0;
//                                         foreach (var p in sortedPoints)
//                                         {
//                                             ResultPoint[nCount] = new DoublePoint();
//                                             ResultPoint[nCount].X = p.X;
//                                             ResultPoint[nCount].Y = p.Y;
//                                             Main.AlignUnit[m_PatAlign_No].m_PatternPoints.Add(ResultPoint[nCount]);
//                                             nCount++;
//                                         }
// 
//                                         for (int j = 0; j < tempGResult.Count; j++)
//                                         {
//                                             if (Main.AlignUnit[m_PatAlign_No].m_PatternPoints[0].X == tempGResult[j].GetPose().TranslationX && Main.AlignUnit[m_PatAlign_No].m_PatternPoints[0].Y == tempGResult[j].GetPose().TranslationY)
//                                             {
//                                                 Main.AlignUnit[m_PatAlign_No].m_PatternNumber = j;
//                                             }
//                                         }
//                                         nGTempTransX = ResultPoint[0].X;
//                                         nGTempTransY = ResultPoint[0].Y;
//                                         PMAlignResult.m_Rotation = tempGResult[Main.AlignUnit[m_PatAlign_No].m_PatternNumber].GetPose().Rotation;
//                                         PMAlignResult.Score = tempGResult[Main.AlignUnit[m_PatAlign_No].m_PatternNumber].Score;
//                                     }
//                                 }
                                #endregion   

                                PMAlignResult.m_Pixel[DEFINE.X] = Pixel[DEFINE.X] = nGTempTransX;
                                PMAlignResult.m_Pixel[DEFINE.Y] = Pixel[DEFINE.Y] = nGTempTransY;
  
                                V2R(ref nTempV2R_X, ref nTempV2R_Y);
                                //V2RPIXEL(ref Pixel_Center[Main.DEFINE.X], ref Pixel_Center[Main.DEFINE.Y]);
                                PMAlignResult.m_RobotPosX = m_RobotPosX = nTempV2R_X;
                                PMAlignResult.m_RobotPosY = m_RobotPosY = nTempV2R_Y;
                                Ret = true;

                                PMAlignResult.PMAlignResult = tempGResult;
                                if (GPattern[SearchSubNum].SearchRegion != null)
                                    PMAlignResult.SearchRegion = new CogRectangle(GPattern[SearchSubNum].SearchRegion as CogRectangle);
                                else
                                {
                                    GPattern[SearchSubNum].SearchRegion = new CogRectangle();
                                    (GPattern[SearchSubNum].SearchRegion as CogRectangle).SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[m_CamNo], Main.vision.IMAGE_CENTER_Y[m_CamNo], Main.vision.IMAGE_SIZE_X[m_CamNo] - 30, Main.vision.IMAGE_SIZE_Y[m_CamNo] - 30);
                                    PMAlignResult.SearchRegion = new CogRectangle(GPattern[SearchSubNum].SearchRegion as CogRectangle);
                                }
                                seq = SEQ.COMPLET_SEQ;
                                break;


                            case SEQ.NG_SEQ:
                                PMAlignResult.m_Pixel[DEFINE.X] = Pixel[DEFINE.X] = 0;
                                PMAlignResult.m_Pixel[DEFINE.Y] = Pixel[DEFINE.Y] = 0;
                                PMAlignResult.m_RobotPosX = m_RobotPosX = 0;
                                PMAlignResult.m_RobotPosY = m_RobotPosY = 0;
                                PMAlignResult.m_Rotation = 0;

                                Ret = false;
                                if (PMAlignResult.m_PMAlign)
                                {
                                    if (GPattern[SearchSubNum].SearchRegion != null)
                                        PMAlignResult.SearchRegion = new CogRectangle(GPattern[SearchSubNum].SearchRegion as CogRectangle);
                                    else
                                    {
                                        (GPattern[SearchSubNum].SearchRegion as CogRectangle).SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[m_CamNo], Main.vision.IMAGE_CENTER_Y[m_CamNo], Main.vision.IMAGE_SIZE_X[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA, Main.vision.IMAGE_SIZE_Y[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA);
                                        PMAlignResult.SearchRegion = new CogRectangle(GPattern[SearchSubNum].SearchRegion as CogRectangle);
                                    }
                                }
                                else
                                {
                                    if (Pattern[SearchSubNum].SearchRegion != null)
                                        PMAlignResult.SearchRegion = new CogRectangle(Pattern[SearchSubNum].SearchRegion as CogRectangle);
                                    else
                                    {
                                        Pattern[SearchSubNum].SearchRegion = new CogRectangle();
                                        (Pattern[SearchSubNum].SearchRegion as CogRectangle).SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[m_CamNo], Main.vision.IMAGE_CENTER_Y[m_CamNo], Main.vision.IMAGE_SIZE_X[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA, Main.vision.IMAGE_SIZE_Y[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA);
                                        PMAlignResult.SearchRegion = new CogRectangle(Pattern[SearchSubNum].SearchRegion as CogRectangle);
                                    }
                                }
                                seq = SEQ.ERROR_SEQ;
                                break;


                            case SEQ.ERROR_SEQ:
                              
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.COMPLET_SEQ:
                                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                                PMAlignResult.ManuMathchFlag = false;
                                PMAlignResult.m_PMAlign_Use = m_PMAlign_Use;
                                ImageSaveResult = PMAlignResult.SearchResult = SearchResult = Ret;
                                LoopFlag = false;
                                break;
                        }

                    }
                }
                catch(System.Exception ex)
                {                              
                   MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                    Ret = false;
                }
                return Ret;
            }   

            public bool Search_Caliper()
            {
                return Search_Caliper(m_CamNo);
            }
            public bool Search_Caliper(int CAM)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool Ret = false;

                List<int> nCaliNum = new List<int>();
                List<CogCaliperResults> nCaliResultsList = new List<CogCaliperResults>();
                CogImage8Grey TempIMG = new CogImage8Grey();
                try
                {
                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                PMAlignResult.m_PatternName = m_PatternName;
                                PMAlignResult.m_CamNo = m_CamNo;
                                seq++;
                                break;

                            case 1:
                                seq++;
                                break;

                            case 2:
                                seq++;
                                break;

                            case 3:
                                seq++;
                                break;

                            case 4:
                                for (int ii = 0; ii < Main.DEFINE.CALIPER_MAX; ii++)
                                {
                                    if (CaliperPara[ii].m_UseCheck)
                                        Ret = true; //하나라도 있으면 True시키고 시작. 
                                }
                                seq++;
                                break;


                            case 5:
                                for (int i = 0; i < Main.DEFINE.CALIPER_MAX; i++)
                                {
                                    CaliResults[i].Pixel[DEFINE.X] = 0;
                                    CaliResults[i].Pixel[DEFINE.Y] = 0;
                                    CaliResults[i].m_UseCheck = CaliperPara[i].m_UseCheck;
                                    CaliResults[i].SearchResult = false;
                                    if (CaliperPara[i].m_UseCheck)
                                    {
                                        CaliperTools[i].InputImage = TempImage;

                                        if (Main.ALIGNINSPECTION_USE(m_PatAlign_No))
                                        //    if (Main.ALIGNINSPECTION_USE(m_PatAlign_No) && (Main.GetCaliperDirection(CaliperTools[i].Region.Rotation) == Main.DEFINE.TOP_TO_BOTTOM || Main.GetCaliperDirection(CaliperTools[i].Region.Rotation) == Main.DEFINE.BOTTOM_TO_TOP))
                                        {
                                            CaliperTools[i] = CaliperToolPairRun(CaliperTools[i], out CaliResults[i].m_PlusMinus);
                                        }
                                        else
                                        {
                                            if (BLOBINSPECTION_USE(m_PatAlign_No))
                                            {
                                                CaliperTools[i].RunParams.SingleEdgeScorers.Clear();
                                                CogCaliperScorerPositionNeg nItem = new CogCaliperScorerPositionNeg();
                                                CaliperTools[i].RunParams.SingleEdgeScorers.Add(nItem);
                                            }
                                            CaliperTools[i].Run();
                                        }
                                        CaliResults[i].CaliperTool = CaliperToolCopy(CaliperTools[i]);

                                        CaliResults[i].m_CALAlignNo = m_PatAlign_No;
                                        CaliResults[i].m_CALPatNo = m_PatNo;
                                        CaliResults[i].m_CALPatTagNo = Main.AlignUnit[m_PatAlign_No].m_PatTagNo;

                                        System.Array.Clear(CaliResults[i].PixelPosX, 0, CaliResults[i].PixelPosX.Length);
                                        System.Array.Clear(CaliResults[i].PixelPosY, 0, CaliResults[i].PixelPosY.Length);

                                        if (CaliperTools[i].Results == null || CaliperTools[i].Results.Count <= 0)
                                        {
                                            Ret = false;
                                            Caliper_SearchResult = CaliResults[i].SearchResult = false;
                                            Caliper_Only_SearchResult = false;
                                        }
                                        else
                                        {
                                            if (m_Cmd == 1095 || m_Cmd == 1096)
                                            {
                                                Caliper_Only_SearchResult = false;
                                                Caliper_SearchResult = CaliResults[i].SearchResult = true;
                                            }
                                            else if (m_Cmd == CMD.ALIGN_INSPECT_LEFT || m_Cmd == CMD.ALIGN_INSPECT_RIGHT || m_Cmd == CMD.DOPO_INSPECT_LEFT || m_Cmd == CMD.DOPO_INSPECT_RIGHT)
                                            {
                                                Caliper_SearchResult = CaliResults[i].SearchResult = true;
                                                Caliper_Only_SearchResult = true;
                                            }
                                            else
                                            {
                                                Caliper_SearchResult = false;
                                                Caliper_Only_SearchResult = true;
                                            }
//                                             CaliResults[i].SearchResult = true;
//                                             Caliper_SearchResult = false;

                                            switch (Main.GetCaliperDirection(Main.GetCaliperDirection(CaliperTools[i].Region.Rotation)))
                                            {
                                                case Main.DEFINE.X:
                                                    PixelCaliper[DEFINE.X] = CaliResults[i].Pixel[DEFINE.X] = (CaliperTools[i].Results[0].Edge0.PositionX + FixtureTrans.TranslationX);
                                                    CaliResults[i].PixelPosX[Main.DEFINE.FIRST_, Main.DEFINE.XPOS] = (CaliperTools[i].Results[0].Edge0.PositionX + FixtureTrans.TranslationX);
                                                    CaliResults[i].PixelPosY[Main.DEFINE.FIRST_, Main.DEFINE.YPOS] = (CaliperTools[i].Results[0].Edge0.PositionY + FixtureTrans.TranslationY);
                                                    if (Main.GetCaliperPairMode(CaliperTools[i].RunParams.EdgeMode))
                                                    {
                                                        CaliResults[i].PixelPosX[Main.DEFINE.SECOND, Main.DEFINE.XPOS] = (CaliperTools[i].Results[0].Edge1.PositionX + FixtureTrans.TranslationX);
                                                        CaliResults[i].PixelPosY[Main.DEFINE.SECOND, Main.DEFINE.YPOS] = (CaliperTools[i].Results[0].Edge1.PositionY + FixtureTrans.TranslationY);
                                                    }
                                                    break;

                                                case Main.DEFINE.Y:
                                                    PixelCaliper[DEFINE.Y] = CaliResults[i].Pixel[DEFINE.Y] = (CaliperTools[i].Results[0].Edge0.PositionY + FixtureTrans.TranslationY);
                                                    CaliResults[i].PixelPosX[Main.DEFINE.FIRST_, Main.DEFINE.XPOS] = (CaliperTools[i].Results[0].Edge0.PositionX + FixtureTrans.TranslationX);
                                                    CaliResults[i].PixelPosY[Main.DEFINE.FIRST_, Main.DEFINE.YPOS] = (CaliperTools[i].Results[0].Edge0.PositionY + FixtureTrans.TranslationY);
                                                    if (Main.GetCaliperPairMode(CaliperTools[i].RunParams.EdgeMode))
                                                    {
                                                        CaliResults[i].PixelPosX[Main.DEFINE.SECOND, Main.DEFINE.XPOS] = (CaliperTools[i].Results[0].Edge1.PositionX + FixtureTrans.TranslationX);
                                                        CaliResults[i].PixelPosY[Main.DEFINE.SECOND, Main.DEFINE.YPOS] = (CaliperTools[i].Results[0].Edge1.PositionY + FixtureTrans.TranslationY);
                                                    }
                                                    break;

                                            }
                                        }//if
                                    }

                                }
                                if (BLOBINSPECTION_USE(m_PatAlign_No) || m_AlignName == "ACF_CUT_1" || m_AlignName == "ACF_CUT_2")
                                {
                                    for (int i = 0; i < Main.DEFINE.CALIPER_MAX; i++)
                                    {
                                        if (CaliResults[i].SearchResult)
                                        {
                                            Ret = true;
                                        }
                                    }
                                }
                                if (Ret)
                                    seq = SEQ.OK_SEQ;
                                else
                                    seq = SEQ.NG_SEQ; //NG
                                break;

                            case SEQ.OK_SEQ:
                                Ret = true;
                                seq = SEQ.OK_SEQ + 1;
                                break;

                            case SEQ.OK_SEQ + 1:

                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.NG_SEQ:
                                Ret = false;
                                seq = SEQ.ERROR_SEQ;
                                break;

                            case SEQ.ERROR_SEQ:
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.COMPLET_SEQ:
                                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                                ImageSaveResult = Ret;
                                LoopFlag = false;
                                break;
                        }

                    }
                }
                catch
                {
                    Ret = false;
                }
                return Ret;
            }
            public bool Search_Line()
            {
                return Search_Line(m_CamNo);
            }
            public bool Search_Line(int CAM)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool Ret = true;

                CogImage8Grey TempIMG = new CogImage8Grey();
                CogIntersectLineLineTool LineLineTool = new CogIntersectLineLineTool();
                List<CogLine> TempLine = new List<CogLine>();
                try
                {
                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                PMAlignResult.m_PatternName = m_PatternName;
                                PMAlignResult.m_CamNo = m_CamNo;
                                PMAlignResult.m_Align_No = m_PatAlign_No;
                                seq++;
                                break;
                            case 1:
                                PixelFindLine[DEFINE.X] = 0;
                                PixelFindLine[DEFINE.Y] = 0;
                                FINDLineResults[0].CrossPointList.Clear();
                                seq++;
                                break;

                            case 2:
                                seq++;
                                break;

                            case 3:
                                seq++;
                                break;

                            case 4:
                                seq++;
                                break;

                            case 5:             //DATA 초기화
                                for (int i = 0; i < Main.DEFINE.FINDLINE_MAX; i++)
                                {
                                    FINDLineResults[i].Pixel[DEFINE.X] = 0;
                                    FINDLineResults[i].Pixel[DEFINE.Y] = 0;

                                    FINDLineResults[i].m_UseCheck = FINDLinePara[DEFINE.MAIN, i].m_UseCheck;
                                    FINDLineResults[i].SearchResult = false;

                                    if (FINDLinePara[DEFINE.MAIN, i].m_UseCheck)
                                    {
                                        // Find Line Position Check
                                        m_FindLinePositions |= FINDLinePara[DEFINE.MAIN, i].m_LinePosition;

                                        FINDLineTools[DEFINE.MAIN, i].InputImage = TempImage;

                                        FINDLineTools[DEFINE.MAIN, i].Run();
                                        FINDLineResults[i].FindLineTool = new CogFindLineTool(FINDLineTools[DEFINE.MAIN, i]);

                                        if (FINDLineTools[DEFINE.MAIN, i].Results != null && FINDLineTools[DEFINE.MAIN, i].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, i].Results.GetLine() != null)
                                        {
                                            FINDLineResults[i].SearchResult = true;
                                            FINDLineResults[i].Pixel[DEFINE.X] = FINDLineTools[DEFINE.MAIN, i].Results.GetLineSegment().MidpointX;
                                            FINDLineResults[i].Pixel[DEFINE.Y] = FINDLineTools[DEFINE.MAIN, i].Results.GetLineSegment().MidpointY;

                                            //                                                 DoublePoint Temp = new DoublePoint();
                                            //                                                 Temp.X = FINDLineResults[0].Pixel[DEFINE.X];
                                            //                                                 Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y];
                                            //                                                 FINDLineResults[0].CrossPointList.Add(Temp);

                                        }
                                        else
                                        {
                                            Ret = false;
                                        }
                                    }
                                }
                                seq++;
                                break;

                            case 6:
                                if (DEFINE.FINDLINE_MAX >= 2)
                                {
                                    if ((FINDLineTools[DEFINE.MAIN, 0].Results != null && FINDLineTools[DEFINE.MAIN, 0].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 0].Results.GetLine() != null)
                                        && (FINDLineTools[DEFINE.MAIN, 1].Results != null && FINDLineTools[DEFINE.MAIN, 1].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 1].Results.GetLine() != null))
                                    {
                                        LineLineTool = new CogIntersectLineLineTool();
                                        LineLineTool.InputImage = TempImage;
                                        LineLineTool.LineA = FINDLineTools[DEFINE.MAIN, 0].Results.GetLine();
                                        LineLineTool.LineB = FINDLineTools[DEFINE.MAIN, 1].Results.GetLine();
                                        LineLineTool.Run();
                                        if (LineLineTool.Intersects)
                                        {
                                            if (LineLineTool.X >= 0 && LineLineTool.X <= LineLineTool.InputImage.Width && LineLineTool.Y >= 0 && LineLineTool.Y <= LineLineTool.InputImage.Height)
                                            {
                                                Ret = true;
                                                DoublePoint Temp = new DoublePoint();
                                                Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = Pixel[DEFINE.X] = PixelFindLine[DEFINE.X] = (LineLineTool.X);
                                                Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = Pixel[DEFINE.Y] = PixelFindLine[DEFINE.Y] = (LineLineTool.Y);
                                                FINDLineResults[0].CrossPointList.Add(Temp);
                                            }
                                        }
                                    }
                                }

                                //CornerTools[0].InputImage = TempImage;

                                ///////////////////////////////
                                //CornerTools[0].RunParams.ExpectedLineSegmentA = FINDLineTools[0].RunParams.ExpectedLineSegment;
                                //CornerTools[0].RunParams.ExpectedLineSegmentB = FINDLineTools[1].RunParams.ExpectedLineSegment;

                                //CornerTools[0].RunParams.NumCalipers = FINDLineTools[0].RunParams.NumCalipers;
                                //CornerTools[0].RunParams.CaliperSearchLength = FINDLineTools[0].RunParams.CaliperSearchLength;
                                //CornerTools[0].RunParams.CaliperProjectionLength = FINDLineTools[0].RunParams.CaliperProjectionLength;
                                //CornerTools[0].RunParams.CaliperSearchDirection = FINDLineTools[0].RunParams.CaliperSearchDirection;
                                //CornerTools[0].RunParams.NumToIgnore = FINDLineTools[0].RunParams.NumToIgnore;
                                //CornerTools[0].RunParams.CaliperRunParams = FINDLineTools[0].RunParams.CaliperRunParams;

                                //CornerTools[0].LastRunRecordDiagEnable = CogFindCornerLastRunRecordDiagConstants.InputImageByReference;
                                //CornerTools[0].LastRunRecordEnable = CogFindCornerLastRunRecordConstants.BestFitLines | CogFindCornerLastRunRecordConstants.FoundCorner | CogFindCornerLastRunRecordConstants.ResultsIgnoredPoints |
                                //                                CogFindCornerLastRunRecordConstants.ResultsUsedPoints;
                                //CornerTools[0].Run();
                                ///////////////////////////////
                                //if (null != CornerTools[0].Result)
                                //{
                                //    CornerResults[0].SearchResult = true;
                                //    CornerResults[0].Pixel[DEFINE.X] = CornerTools[0].Result.CornerX;
                                //    CornerResults[0].Pixel[DEFINE.Y] = CornerTools[0].Result.CornerY;

                                //    DoublePoint Temp = new DoublePoint();
                                //    Temp.X = CornerResults[0].Pixel[DEFINE.X];
                                //    Temp.Y = CornerResults[0].Pixel[DEFINE.Y];
                                //    FINDLineResults[0].CrossPointList.Add(Temp);
                                //}


                                //                              
                                //                                 if(TempLine.Count >= 2)  //패턴단위로 에러 치지 말고 우선 교차점은 구하고 나서 교차점을 찾았나 못찼았냐에 대한 에러 설정 할 수 있도록 수정하기.. 패턴단위에서는 교차점 무조건 계산 해서 얼라인 유닛으로 넘어가도록 수정하기.
                                //                                 {
                                //                                     for (int i = 0; i < TempLine.Count; i++)
                                //                                     {
                                //                                         for (int j = 0; j < TempLine.Count; j++)
                                //                                         {
                                //                                             if (i < j) 
                                //                                             {
                                //                                                 LineLineTool.InputImage = TempImage;
                                //                                                 LineLineTool.LineA = TempLine[i];
                                //                                                 LineLineTool.LineB = TempLine[j];
                                //                                                 LineLineTool.Run();
                                //                                                 if (LineLineTool.Intersects)
                                //                                                 {
                                //                                                     if (LineLineTool.X >= 0 && LineLineTool.X <= LineLineTool.InputImage.Width && LineLineTool.Y >= 0 && LineLineTool.Y <= LineLineTool.InputImage.Height)
                                //                                                     {
                                //                                                              Ret = true;
                                //                                                              DoublePoint Temp = new DoublePoint();
                                //                                                              Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = Pixel[DEFINE.X] = PixelFindLine[DEFINE.X] = (LineLineTool.X);
                                //                                                              Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = Pixel[DEFINE.Y] = PixelFindLine[DEFINE.Y] = (LineLineTool.Y);
                                //                                                              FINDLineResults[0].CrossPointList.Add(Temp);
                                //                                                     }                                                  
                                //                                                  }
                                //                                               }
                                // 
                                //                                            }
                                //                                        }
                                //                                    }                        
                                seq++;
                                break;


                            case 7:
                                if (Ret)
                                    seq = SEQ.OK_SEQ;
                                else
                                    seq = SEQ.NG_SEQ; //NG
                                break;

                            case SEQ.OK_SEQ:
//                                 double nTempV2R_X = 0;
//                                 double nTempV2R_Y = 0;
//                                 V2R(ref nTempV2R_X, ref nTempV2R_Y);
//                                 PMAlignResult.m_RobotPosX = m_RobotPosX = nTempV2R_X;
//                                 PMAlignResult.m_RobotPosY = m_RobotPosY = nTempV2R_Y;
                                Ret = true;
                                SearchResult = true;
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.NG_SEQ:
                                Ret = false;
                                seq = SEQ.ERROR_SEQ;
                                break;

                            case SEQ.ERROR_SEQ:
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.COMPLET_SEQ:
                                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                                ImageSaveResult = Ret;
                                LoopFlag = false;
                                break;
                        }
                    }
                }
                catch
                {
                    Ret = false;
                }

                return Ret;
            }

            // 200716 BP
            public ushort SearchbyCommand(int nSelPad, ushort nCmd)
            {
                int seq = 0;
                bool LoopFlag = true;
                ushort Ret = 0;

                if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C))
                {
                    Ret |= (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C;
                    if (m_PatAlign_No == 0 || m_PatAlign_No == 1)
                    {
                        if ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.COMMAND5) == (ushort)AlignUnitTag.GRAB_CMD.COMMAND5)
                            Ret |= (ushort)AlignUnitTag.GRAB_CMD.COMMAND5;
                        if ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.COMMAND6) == (ushort)AlignUnitTag.GRAB_CMD.COMMAND6)
                            Ret |= (ushort)AlignUnitTag.GRAB_CMD.COMMAND6;
                    }
                }
                else if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" &&  ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R))
                {
                    Ret |= (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R;
                }

                if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S))
                {
                    Ret |= (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S;
                }

                CogImage8Grey TempIMG = new CogImage8Grey();
                CogIntersectLineLineTool[] LineLineTool = new CogIntersectLineLineTool[3];
                List<CogLine> TempLine = new List<CogLine>();
                try
                {
                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                PMAlignResult.m_PatternName = m_PatternName;
                                PMAlignResult.m_CamNo = m_CamNo;
                                PMAlignResult.m_Align_No = m_PatAlign_No;
                                m_FindLinePositions = 0;
                                seq++;
                                break;
                            case 1:
                                PixelFindLine[DEFINE.X] = 0;
                                PixelFindLine[DEFINE.Y] = 0;
                                Pixel[DEFINE.X] = 0;
                                Pixel[DEFINE.Y] = 0;

                                seq++;
                                break;

                            case 2:
                                seq++;
                                break;

                            case 3:
                                seq++;
                                break;

                            case 4:
                                seq++;
                                break;

                            case 5:             //DATA 초기화
                                for (int i = 0; i < Main.DEFINE.FINDLINE_MAX; i++)
                                {
                                    FINDLineResults[i].CrossPointList.Clear();
                                    FINDLineResults[i].m_bInterSearched = false;
                                    FINDLineResults[i].m_bInterResult = false;
                                    FINDLineResults[i].m_bLoadingLimitOver_X = false;
                                    FINDLineResults[i].m_bLoadingLimitOver_Y = false;
                                    FINDLineResults[i].m_bAngleLimit = false;
                                    FINDLineResults[i].m_FoundSubLineNum = -1;
                                    FINDLineResults[i].ManuMatchComplete = false;

                                    FINDLineResults[i].Pixel[DEFINE.X] = 0;
                                    FINDLineResults[i].Pixel[DEFINE.Y] = 0;

                                    FINDLineResults[i].m_UseCheck = FINDLinePara[DEFINE.MAIN, i].m_UseCheck;
                                    FINDLineResults[i].SearchResult = false;

                                    if (i == 0 && (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_X) != (ushort)AlignUnitTag.GRAB_CMD.SEARCH_X)
                                        continue;
                                    else if (i == 1 && (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_Y) != (ushort)AlignUnitTag.GRAB_CMD.SEARCH_Y)
                                        continue;
                                    else if (i == 2 && (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C) != (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C)
                                        continue;

                                    //FINDLineResults[i].Pixel[DEFINE.X] = 0;
                                    //FINDLineResults[i].Pixel[DEFINE.Y] = 0;   // 20201119 위로 옮김

                                    if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                    {
                                        // Vision에서 Inspection Size 구하기 위함
                                        if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_FINE_ALIGN_MODE))
                                        {
                                            InspectionPosRobot_X[i] = 0;    // Glass Line 좌표
                                            InspectionPosRobot_Y[i] = 0;
                                        }
                                    }

                                    //FINDLineResults[i].m_UseCheck = FINDLinePara[DEFINE.MAIN, i].m_UseCheck;
                                    //FINDLineResults[i].SearchResult = false;   // 20201119 위로 옮김
                                    FINDLineResults[i].m_UsedLineMax = false;
                                    FINDLineResults[i].m_LineMaxSelIdx = 0;

                                    if (FINDLinePara[DEFINE.MAIN, i].m_UseCheck)
                                    {
                                        // Find Line Position Check
                                        m_FindLinePositions |= FINDLinePara[DEFINE.MAIN, i].m_LinePosition;

                                        #region USELINEMAX
                                        if (m_UseLineMax)
                                        {
                                            int nLineIdx = 0;
                                            LineMaxTools[DEFINE.MAIN, i].InputImage = TempImage;
                                            FINDLineResults[i].m_UsedLineMax = true;

                                            if (i == 2) continue;   // Diag Position Correction, next step

                                            LineMaxTools[DEFINE.MAIN, i].Run();
                                            FINDLineResults[i].LineMaxTool = new CogLineMaxTool(LineMaxTools[DEFINE.MAIN, i]);

                                            if (LineMaxTools[DEFINE.MAIN, i].Results != null && LineMaxTools[DEFINE.MAIN, i].Results.Count > 0 && LineMaxTools[DEFINE.MAIN, i].Results[0].GetLine() != null)
                                            {
                                                GetLineMaxIndex(i, FINDLinePara[DEFINE.MAIN, i], LineMaxTools[DEFINE.MAIN, i], ref nLineIdx);
                                                FINDLineResults[i].m_LineMaxSelIdx = nLineIdx;
                                                FINDLineResults[i].m_FoundSubLineNum = 0;

                                                FINDLineResults[i].SearchResult = true;
                                                FINDLineResults[i].Pixel[DEFINE.X] = LineMaxTools[DEFINE.MAIN, i].Results[nLineIdx].GetLineSegment().MidpointX;
                                                FINDLineResults[i].Pixel[DEFINE.Y] = LineMaxTools[DEFINE.MAIN, i].Results[nLineIdx].GetLineSegment().MidpointY;

                                                if (i < 3) Ret |= (ushort)(1 << i);
                                            }
                                            else
                                            {
                                                //Ret = false;
                                            }
                                        }
                                        #endregion
                                        else
                                        {
                                            FINDLineTools[DEFINE.MAIN, i].InputImage = TempImage;

                                            if (i == 2) continue;   // Diag Position Correction

                                            FINDLineTools[DEFINE.MAIN, i].Run();
                                            FINDLineResults[i].FindLineTool = new CogFindLineTool(FINDLineTools[DEFINE.MAIN, i]);

                                            if (FINDLineTools[DEFINE.MAIN, i].Results != null && FINDLineTools[DEFINE.MAIN, i].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, i].Results.GetLine() != null)
                                            {
                                                FINDLineResults[i].m_FoundSubLineNum = 0;

                                                FINDLineResults[i].SearchResult = true;
                                                FINDLineResults[i].Pixel[DEFINE.X] = FINDLineTools[DEFINE.MAIN, i].Results.GetLineSegment().MidpointX;
                                                FINDLineResults[i].Pixel[DEFINE.Y] = FINDLineTools[DEFINE.MAIN, i].Results.GetLineSegment().MidpointY;

                                                if (i < 3) Ret |= (ushort)(1 << i);
                                            }
                                            else
                                            {
                                                //Ret = false;
                                            }
                                        }

                                        if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                        {
                                            // Vision에서 Inspection Size 구하기 위함
                                            if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_FINE_ALIGN_MODE))
                                            {
                                                InspectionPosRobot_X[i] = FINDLineResults[i].Pixel[DEFINE.X];    // Glass Line 좌표
                                                InspectionPosRobot_Y[i] = FINDLineResults[i].Pixel[DEFINE.Y];
                                            }
                                            else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                                            {
                                                if (FINDLineTools[DEFINE.MAIN, i].Results != null && FINDLineTools[DEFINE.MAIN, i].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, i].Results.GetLine() != null)
                                                    InspectionSizeRobot_X[i] = FINDLineTools[DEFINE.MAIN, i].Results.GetLineSegment().DistanceToPoint(InspectionPosRobot_X[i], InspectionPosRobot_Y[i]);    // Cutting POL Line Gap Size
                                                else
                                                    InspectionSizeRobot_X[i] = 0;
                                            }
                                        }
                                    }
                                }
                                seq++;
                                break;
                            case 6: // Sub Retry
                                if (FINDLineResults[0].SearchResult == false || FINDLineResults[1].SearchResult == false)
                                {
                                    for (int s = 1; s < Main.DEFINE.SUBLINE_MAX; s++)
                                    {
                                        //for (int i = 0; i < Main.DEFINE.FINDLINE_MAX; i++)
                                        for (int i = 0; i < 2; i++) // X, Y Only
                                        {
                                            if (FINDLineResults[i].SearchResult == true)
                                                continue;

                                            FINDLineResults[i].CrossPointList.Clear();
                                            FINDLineResults[i].m_bInterSearched = false;
                                            FINDLineResults[i].m_bInterResult = false;
                                            FINDLineResults[i].m_bLoadingLimitOver_X = false;
                                            FINDLineResults[i].m_bLoadingLimitOver_Y = false;
                                            FINDLineResults[i].m_bAngleLimit = false;

                                            FINDLineResults[i].Pixel[DEFINE.X] = 0;
                                            FINDLineResults[i].Pixel[DEFINE.Y] = 0;

                                            FINDLineResults[i].m_UseCheck = FINDLinePara[s, i].m_UseCheck;
                                            FINDLineResults[i].SearchResult = false;
                                            FINDLineResults[i].m_UsedLineMax = false;
                                            FINDLineResults[i].m_LineMaxSelIdx = 0;

                                            if (FINDLinePara[s, i].m_UseCheck)
                                            {
                                                // Find Line Position Check
                                                m_FindLinePositions |= FINDLinePara[s, i].m_LinePosition;

                                                #region USELINEMAX
                                                if (m_UseLineMax)
                                                {
                                                    int nLineIdx = 0;
                                                    LineMaxTools[s, i].InputImage = TempImage;
                                                    FINDLineResults[i].m_UsedLineMax = true;

                                                    LineMaxTools[s, i].Run();
                                                    FINDLineResults[i].LineMaxTool = new CogLineMaxTool(LineMaxTools[s, i]);

                                                    if (LineMaxTools[s, i].Results != null && LineMaxTools[s, i].Results.Count > 0 && LineMaxTools[s, i].Results[0].GetLine() != null)
                                                    {
                                                        GetLineMaxIndex(i, FINDLinePara[s, i], LineMaxTools[s, i], ref nLineIdx);
                                                        FINDLineResults[i].m_LineMaxSelIdx = nLineIdx;
                                                        FINDLineResults[i].m_FoundSubLineNum = s;

                                                        FINDLineResults[i].SearchResult = true;
                                                        FINDLineResults[i].Pixel[DEFINE.X] = LineMaxTools[s, i].Results[nLineIdx].GetLineSegment().MidpointX;
                                                        FINDLineResults[i].Pixel[DEFINE.Y] = LineMaxTools[s, i].Results[nLineIdx].GetLineSegment().MidpointY;

                                                        if (i < 3) Ret |= (ushort)(1 << i);
                                                    }
                                                    else
                                                    {
                                                        //Ret = false;
                                                    }
                                                }
                                                #endregion
                                                else
                                                {
                                                    FINDLineTools[s, i].InputImage = TempImage;

                                                    FINDLineTools[s, i].Run();
                                                    FINDLineResults[i].FindLineTool = new CogFindLineTool(FINDLineTools[s, i]);

                                                    if (FINDLineTools[s, i].Results != null && FINDLineTools[s, i].Results.Count > 0 && FINDLineTools[s, i].Results.GetLine() != null)
                                                    {
                                                        FINDLineResults[i].m_FoundSubLineNum = s;

                                                        FINDLineResults[i].SearchResult = true;
                                                        FINDLineResults[i].Pixel[DEFINE.X] = FINDLineTools[s, i].Results.GetLineSegment().MidpointX;
                                                        FINDLineResults[i].Pixel[DEFINE.Y] = FINDLineTools[s, i].Results.GetLineSegment().MidpointY;

                                                        if (i < 3) Ret |= (ushort)(1 << i);
                                                    }
                                                    else
                                                    {
                                                        //Ret = false;
                                                    }
                                                }

                                                if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                                {
                                                    // Vision에서 Inspection Size 구하기 위함
                                                    if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_FINE_ALIGN_MODE))
                                                    {
                                                        InspectionPosRobot_X[i] = FINDLineResults[i].Pixel[DEFINE.X];    // Glass Line 좌표
                                                        InspectionPosRobot_Y[i] = FINDLineResults[i].Pixel[DEFINE.Y];
                                                    }
                                                    else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                                                    {
                                                        if (FINDLineTools[s, i].Results != null && FINDLineTools[s, i].Results.Count > 0 && FINDLineTools[s, i].Results.GetLine() != null)
                                                            InspectionSizeRobot_X[i] = FINDLineTools[s, i].Results.GetLineSegment().DistanceToPoint(InspectionPosRobot_X[i], InspectionPosRobot_Y[i]);    // Cutting POL Line Gap Size
                                                        else
                                                            InspectionSizeRobot_X[i] = 0;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                seq++;
                                break;
                            case 7: // 교점
                                int nFoundXIdx = FINDLineResults[0].m_FoundSubLineNum;
                                int nFoundYIdx = FINDLineResults[1].m_FoundSubLineNum;
                                if (nFoundXIdx < 0 || nFoundYIdx < 0)
                                {
                                    seq++;
                                    break;
                                }

                                if (DEFINE.FINDLINE_MAX >= 3)
                                {
                                    #region USELINEMAX
                                    if (m_UseLineMax)
                                    {
                                        // 1. X, Y 교점 (모든 경우에 필요)
                                        if ((LineMaxTools[nFoundXIdx, 0].Results != null && LineMaxTools[nFoundXIdx, 0].Results.Count > 0 && LineMaxTools[nFoundXIdx, 0].Results[0].GetLine() != null)
                                            && (LineMaxTools[nFoundYIdx, 1].Results != null && LineMaxTools[nFoundYIdx, 1].Results.Count > 0 && LineMaxTools[nFoundYIdx, 1].Results[0].GetLine() != null))
                                        {
                                            int nLineIdx1 = 0, nLineIdx2 = 0;

                                            GetLineMaxIndex(0, FINDLinePara[nFoundXIdx, 0], LineMaxTools[nFoundXIdx, 0], ref nLineIdx1);
                                            GetLineMaxIndex(1, FINDLinePara[nFoundYIdx, 1], LineMaxTools[nFoundYIdx, 1], ref nLineIdx2);

                                            LineLineTool[0] = new CogIntersectLineLineTool();
                                            LineLineTool[0].InputImage = TempImage;
                                            LineLineTool[0].LineA = LineMaxTools[nFoundXIdx, 0].Results[nLineIdx1].GetLine();
                                            LineLineTool[0].LineB = LineMaxTools[nFoundYIdx, 1].Results[nLineIdx2].GetLine();
                                            LineLineTool[0].Run();
                                            if (LineLineTool[0].Intersects)
                                            {
                                                FINDLineResults[0].m_bInterSearched = true;

                                                if (LineLineTool[0].X >= 0 && LineLineTool[0].X <= LineLineTool[0].InputImage.Width && LineLineTool[0].Y >= 0 && LineLineTool[0].Y <= LineLineTool[0].InputImage.Height)
                                                {
                                                    FINDLineResults[0].RectangleAngle = LineLineTool[0].Angle * DEFINE.degree;

                                                    //Ret &= true;
                                                    if (Main.machine.m_bUseLoadingLimit && m_PatNo == DEFINE.CAM_SELECT_ALIGN)
                                                    {
                                                        double dTempX = 0, dTempY = 0;
                                                        V2R(LineLineTool[0].X, LineLineTool[0].Y, ref dTempX, ref dTempY);

                                                        int nLoadingX = (int)(Math.Abs(dTempX - m_dCustomCrossX) * 1000);
                                                        int nLoadingY = (int)(Math.Abs(dTempY - m_dCustomCrossY) * 1000);
                                                        if (nLoadingX <= machine.m_nLoadingLimitX_um && nLoadingY <= machine.m_nLoadingLimitY_um)
                                                        {
                                                            Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;
                                                            FINDLineResults[0].m_bInterResult = true;
                                                        }
                                                        else
                                                        {
                                                            if (nLoadingX > machine.m_nLoadingLimitX_um)
                                                            {
                                                                AlignUnit[m_PatAlign_No].LogdataDisplay("Loading Limit X Error : " + nLoadingX.ToString() + " > " + machine.m_nLoadingLimitX_um, true);
                                                                FINDLineResults[0].m_bLoadingLimitOver_X = true;
                                                            }
                                                            if (nLoadingY > machine.m_nLoadingLimitY_um)
                                                            {
                                                                AlignUnit[m_PatAlign_No].LogdataDisplay("Loading Limit Y Error : " + nLoadingY.ToString() + " > " + machine.m_nLoadingLimitY_um, true);
                                                                FINDLineResults[0].m_bLoadingLimitOver_Y = true;
                                                            }
                                                        }
                                                    }
                                                    else if (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] == DEFINE.NORMAL_RUN && Main.machine.m_bUseAlign1AngleLimit && m_PatNo == DEFINE.CAM_SELECT_1ST_ALIGN)
                                                    {
                                                        if ((90 - Math.Abs(FINDLineResults[0].RectangleAngle)) <= machine.m_f1stAlignCornerAngleLimit)
                                                        {
                                                            Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;
                                                            FINDLineResults[0].m_bInterResult = true;
                                                        }
                                                        else
                                                        {
                                                            AlignUnit[m_PatAlign_No].LogdataDisplay("Cross Angle Limit Error : " + FINDLineResults[0].RectangleAngle.ToString() + " > 90±" + machine.m_f1stAlignCornerAngleLimit.ToString("0.0"), true);
                                                            FINDLineResults[0].m_bAngleLimit = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;
                                                        FINDLineResults[0].m_bInterResult = true;
                                                    }

                                                    DoublePoint Temp = new DoublePoint();
                                                    Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = /*PixelFindLine[DEFINE.X] =*/ Pixel[DEFINE.X] = (LineLineTool[0].X);
                                                    Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = /*PixelFindLine[DEFINE.Y] =*/ Pixel[DEFINE.Y] = (LineLineTool[0].Y);
                                                    FINDLineResults[0].CrossPointList.Add(Temp);
                                                }

                                                // Diag Position Correction (Shape C)
                                                if ((DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" || (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S ) &&
                                                    (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C)
                                                {
                                                    if (FINDLinePara[DEFINE.MAIN, 2].m_UseCheck)
                                                    {
                                                        // Diag Position correction
                                                        {
                                                            (LineMaxTools[DEFINE.MAIN, 2].Region as CogRectangleAffine).CenterX = LineLineTool[DEFINE.MAIN].X + FINDLinePara[DEFINE.MAIN, 2].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].X;
                                                            (LineMaxTools[DEFINE.MAIN, 2].Region as CogRectangleAffine).CenterY = LineLineTool[DEFINE.MAIN].Y + FINDLinePara[DEFINE.MAIN, 2].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].Y;
                                                        }

                                                        LineMaxTools[DEFINE.MAIN, 2].Run();
                                                        FINDLineResults[2].LineMaxTool = new CogLineMaxTool(LineMaxTools[DEFINE.MAIN, 2]);

                                                        if (LineMaxTools[DEFINE.MAIN, 2].Results != null && LineMaxTools[DEFINE.MAIN, 2].Results.Count > 0 && LineMaxTools[DEFINE.MAIN, 2].Results[0].GetLine() != null)
                                                        {
                                                            int nLineIdx = 0;
                                                            GetLineMaxIndex(2, FINDLinePara[DEFINE.MAIN, 2], LineMaxTools[DEFINE.MAIN, 2], ref nLineIdx);
                                                            FINDLineResults[2].m_LineMaxSelIdx = nLineIdx;

                                                            FINDLineResults[2].SearchResult = true;
                                                            FINDLineResults[2].Pixel[DEFINE.X] = LineMaxTools[DEFINE.MAIN, 2].Results[nLineIdx].GetLineSegment().MidpointX;
                                                            FINDLineResults[2].Pixel[DEFINE.Y] = LineMaxTools[DEFINE.MAIN, 2].Results[nLineIdx].GetLineSegment().MidpointY;

                                                            if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                                                            {
                                                                DoublePoint Temp = new DoublePoint();
                                                                Temp.X2 = LineMaxTools[DEFINE.MAIN, 2].Results[nLineIdx].GetLineSegment().Rotation * DEFINE.degree;
                                                                Temp.X2 = (90 - Temp.X2) * (-1);

                                                                FINDLineResults[2].CrossPointList.Add(Temp);
                                                                FINDLineResults[2].RectangleAngle = Temp.X2;

                                                                if (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] == DEFINE.NORMAL_RUN && Main.machine.m_bUseAlign1AngleLimit && Math.Abs(Temp.X2) > machine.m_f1stAlignVerticalAngleLimit)
                                                                {
                                                                    AlignUnit[m_PatAlign_No].LogdataDisplay("Vertical Angle Limit Error : " + Temp.X2.ToString() + " > ±" + machine.m_f1stAlignVerticalAngleLimit.ToString("0.0"), true);
                                                                    FINDLineResults[2].m_bAngleLimit = true;
                                                                }
                                                                else
                                                                {
                                                                    Ret |= (ushort)(1 << 2);
                                                                }
                                                            }

                                                            if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                                            {
                                                                // Vision에서 Inspection Size 구하기 위함
                                                                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_FINE_ALIGN_MODE))
                                                                {
                                                                    InspectionPosRobot_X[2] = FINDLineResults[2].Pixel[DEFINE.X];    // Glass Line 좌표
                                                                    InspectionPosRobot_Y[2] = FINDLineResults[2].Pixel[DEFINE.Y];
                                                                }
                                                                else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                                                                {
                                                                    InspectionSizeRobot_X[2] = FINDLineTools[DEFINE.MAIN, 2].Results.GetLineSegment().DistanceToPoint(InspectionPosRobot_X[2], InspectionPosRobot_Y[2]);    // Cutting POL Line Gap Size
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Ret = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // No FINDLinePara[2].m_UseCheck error
                                                    }
                                                }
                                            }
                                        }

                                        // POL Edge
                                        if (DEFINE.FINDLINE_MAX >= 4)
                                        {
                                            if (FINDLinePara[DEFINE.MAIN, 3].m_UseCheck)
                                            {
                                                if ((LineMaxTools[DEFINE.MAIN, 3].Results != null && LineMaxTools[DEFINE.MAIN, 3].Results.Count > 0 && LineMaxTools[DEFINE.MAIN, 3].Results[0].GetLine() != null))
                                                {
                                                    FINDLineResults[3].SearchResult = true;
                                                    FINDLineResults[3].Pixel[DEFINE.X] = LineMaxTools[DEFINE.MAIN, 3].Results[0].GetLineSegment().MidpointX;
                                                    FINDLineResults[3].Pixel[DEFINE.Y] = LineMaxTools[DEFINE.MAIN, 3].Results[0].GetLineSegment().MidpointY;

                                                    DoublePoint Temp = new DoublePoint();
                                                    Temp.X = FINDLineResults[1].Pixel[DEFINE.X] - FINDLineResults[3].Pixel[DEFINE.X];
                                                    Temp.Y = FINDLineResults[1].Pixel[DEFINE.Y] - FINDLineResults[3].Pixel[DEFINE.Y];
                                                    FINDLineResults[3].CrossPointList.Add(Temp);
                                                }
                                            }
                                        }

                                        #region Shape C-Cut
                                            if ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S &&
                                                (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C)
                                        {
                                            // 2. X, Diag 교점
                                            if ((Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                                && (LineMaxTools[nFoundXIdx, 0].Results != null && LineMaxTools[nFoundXIdx, 0].Results.Count > 0 && LineMaxTools[nFoundXIdx, 0].Results[0].GetLine() != null)
                                                && (LineMaxTools[DEFINE.MAIN, 2].Results != null && LineMaxTools[DEFINE.MAIN, 2].Results.Count > 0 && LineMaxTools[DEFINE.MAIN, 2].Results[0].GetLine() != null))
                                            {
                                                int nLineIdx1 = 0, nLineIdx2 = 0;

                                                GetLineMaxIndex(0, FINDLinePara[nFoundXIdx, 0], LineMaxTools[nFoundXIdx, 0], ref nLineIdx1);
                                                GetLineMaxIndex(2, FINDLinePara[DEFINE.MAIN, 2], LineMaxTools[DEFINE.MAIN, 2], ref nLineIdx2);

                                                LineLineTool[1] = new CogIntersectLineLineTool();
                                                LineLineTool[1].InputImage = TempImage;
                                                LineLineTool[1].LineA = LineMaxTools[nFoundXIdx, 0].Results[nLineIdx1].GetLine();
                                                LineLineTool[1].LineB = LineMaxTools[DEFINE.MAIN, 2].Results[nLineIdx2].GetLine();
                                                LineLineTool[1].Run();
                                                if (LineLineTool[1].Intersects)
                                                {
                                                    FINDLineResults[1].m_bInterSearched = true;

                                                    if (LineLineTool[1].X >= 0 && LineLineTool[1].X <= LineLineTool[1].InputImage.Width && LineLineTool[1].Y >= 0 && LineLineTool[1].Y <= LineLineTool[1].InputImage.Height)
                                                    {
                                                        FINDLineResults[1].m_bInterResult = true;
                                                        //Ret &= true;
                                                        Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXD;

                                                        DoublePoint Temp = new DoublePoint();
                                                        Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = /*Pixel[DEFINE.X] = PixelFindLine[DEFINE.X] =*/ (LineLineTool[1].X);
                                                        Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = /*Pixel[DEFINE.Y] = PixelFindLine[DEFINE.Y] =*/ (LineLineTool[1].Y);
                                                        FINDLineResults[1].CrossPointList.Add(Temp);
                                                    }
                                                }
                                            }

                                            // 3. Y, Diag 교점
                                            if ((Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                                && (LineMaxTools[nFoundYIdx, 1].Results != null && LineMaxTools[nFoundYIdx, 1].Results.Count > 0 && LineMaxTools[nFoundYIdx, 1].Results[0].GetLine() != null)
                                                && (LineMaxTools[DEFINE.MAIN, 2].Results != null && LineMaxTools[DEFINE.MAIN, 2].Results.Count > 0 && LineMaxTools[DEFINE.MAIN, 2].Results[0].GetLine() != null))
                                            {
                                                int nLineIdx1 = 0, nLineIdx2 = 0;

                                                GetLineMaxIndex(1, FINDLinePara[nFoundYIdx, 1], LineMaxTools[nFoundYIdx, 1], ref nLineIdx1);
                                                GetLineMaxIndex(2, FINDLinePara[DEFINE.MAIN, 2], LineMaxTools[DEFINE.MAIN, 2], ref nLineIdx2);

                                                LineLineTool[2] = new CogIntersectLineLineTool();
                                                LineLineTool[2].InputImage = TempImage;
                                                LineLineTool[2].LineA = LineMaxTools[nFoundYIdx, 1].Results[nLineIdx1].GetLine();
                                                LineLineTool[2].LineB = LineMaxTools[DEFINE.MAIN, 2].Results[nLineIdx2].GetLine();
                                                LineLineTool[2].Run();
                                                if (LineLineTool[2].Intersects)
                                                {
                                                    FINDLineResults[2].m_bInterSearched = true;

                                                    if (LineLineTool[2].X >= 0 && LineLineTool[2].X <= LineLineTool[2].InputImage.Width && LineLineTool[2].Y >= 0 && LineLineTool[2].Y <= LineLineTool[2].InputImage.Height)
                                                    {
                                                        FINDLineResults[2].m_bInterResult = true;
                                                        //Ret &= true;
                                                        Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;

                                                        DoublePoint Temp = new DoublePoint();
                                                        Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = /*Pixel[DEFINE.X] = PixelFindLine[DEFINE.X] =*/ (LineLineTool[2].X);
                                                        Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = /*Pixel[DEFINE.Y] = PixelFindLine[DEFINE.Y] =*/ (LineLineTool[2].Y);
                                                        FINDLineResults[2].CrossPointList.Add(Temp);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    else
                                    {
                                        // 1. X, Y 교점 (모든 경우에 필요)
                                        if ((FINDLineTools[nFoundXIdx, 0].Results != null && FINDLineTools[nFoundXIdx, 0].Results.Count > 0 && FINDLineTools[nFoundXIdx, 0].Results.GetLine() != null)
                                            && (FINDLineTools[nFoundYIdx, 1].Results != null && FINDLineTools[nFoundYIdx, 1].Results.Count > 0 && FINDLineTools[nFoundYIdx, 1].Results.GetLine() != null))
                                        {
                                            LineLineTool[0] = new CogIntersectLineLineTool();
                                            LineLineTool[0].InputImage = TempImage;
                                            LineLineTool[0].LineA = FINDLineTools[nFoundXIdx, 0].Results.GetLine();
                                            LineLineTool[0].LineB = FINDLineTools[nFoundYIdx, 1].Results.GetLine();
                                            LineLineTool[0].Run();
                                            if (LineLineTool[0].Intersects)
                                            {
                                                FINDLineResults[0].m_bInterSearched = true;

                                                if (LineLineTool[0].X >= 0 && LineLineTool[0].X <= LineLineTool[0].InputImage.Width && LineLineTool[0].Y >= 0 && LineLineTool[0].Y <= LineLineTool[0].InputImage.Height)
                                                {
                                                    FINDLineResults[0].RectangleAngle = LineLineTool[0].Angle * DEFINE.degree;

                                                    //Ret &= true;
                                                    if (Main.machine.m_bUseLoadingLimit && m_PatNo == DEFINE.CAM_SELECT_ALIGN)
                                                    {
                                                        double dTempX = 0, dTempY = 0;
                                                        V2R(LineLineTool[0].X, LineLineTool[0].Y, ref dTempX, ref dTempY);

                                                        int nLoadingX = (int)(Math.Abs(dTempX - m_dCustomCrossX) * 1000);
                                                        int nLoadingY = (int)(Math.Abs(dTempY - m_dCustomCrossY) * 1000);
                                                        if (nLoadingX <= machine.m_nLoadingLimitX_um && nLoadingY <= machine.m_nLoadingLimitY_um)
                                                        {
                                                            Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;
                                                            FINDLineResults[0].m_bInterResult = true;
                                                        }
                                                        else
                                                        {
                                                            if (nLoadingX > machine.m_nLoadingLimitX_um)
                                                            {
                                                                AlignUnit[m_PatAlign_No].LogdataDisplay("Loading Limit X Error : " + nLoadingX.ToString() + " > " + machine.m_nLoadingLimitX_um, true);
                                                                FINDLineResults[0].m_bLoadingLimitOver_X = true;
                                                            }
                                                            if (nLoadingY > machine.m_nLoadingLimitY_um)
                                                            {
                                                                AlignUnit[m_PatAlign_No].LogdataDisplay("Loading Limit Y Error : " + nLoadingY.ToString() + " > " + machine.m_nLoadingLimitY_um, true);
                                                                FINDLineResults[0].m_bLoadingLimitOver_Y = true;
                                                            }
                                                        }
                                                    }
                                                    else if (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] == DEFINE.NORMAL_RUN
                                                        && Main.machine.m_bUseAlign1AngleLimit && m_PatNo == DEFINE.CAM_SELECT_1ST_ALIGN)
                                                    {
                                                        if ((90 - Math.Abs(FINDLineResults[0].RectangleAngle)) <= machine.m_f1stAlignCornerAngleLimit)
                                                        {
                                                            Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;
                                                            FINDLineResults[0].m_bInterResult = true;
                                                        }
                                                        else
                                                        {
                                                            AlignUnit[m_PatAlign_No].LogdataDisplay("Cross Angle Limit Error : " + FINDLineResults[0].RectangleAngle.ToString() + " > 90±" + machine.m_f1stAlignCornerAngleLimit.ToString("0.0"), true);
                                                            FINDLineResults[0].m_bAngleLimit = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXY;
                                                        FINDLineResults[0].m_bInterResult = true;
                                                    }

                                                    DoublePoint Temp = new DoublePoint();
                                                    Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = /*PixelFindLine[DEFINE.X] =*/Pixel[DEFINE.X] = (LineLineTool[0].X);
                                                    Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = /*PixelFindLine[DEFINE.Y] =*/Pixel[DEFINE.Y] = (LineLineTool[0].Y);
                                                    FINDLineResults[0].CrossPointList.Add(Temp);
                                                }

                                                // Diag Position Correction (Shape C)
                                                if ((DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" || (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) &&    // Normal cut 에서 대각선 찾는 것 방지
                                                    (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C)
                                                {
                                                    if (FINDLinePara[DEFINE.MAIN, 2].m_UseCheck)
                                                    {
                                                        // Position correction
                                                        {
                                                            FINDLineTools[DEFINE.MAIN, 2].RunParams.ExpectedLineSegment.StartX = LineLineTool[0].X + FINDLinePara[DEFINE.MAIN, 2].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].X;
                                                            FINDLineTools[DEFINE.MAIN, 2].RunParams.ExpectedLineSegment.StartY = LineLineTool[0].Y + FINDLinePara[DEFINE.MAIN, 2].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].Y;

                                                            FINDLineTools[DEFINE.MAIN, 2].RunParams.ExpectedLineSegment.EndX = LineLineTool[0].X + FINDLinePara[DEFINE.MAIN, 2].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].X2;
                                                            FINDLineTools[DEFINE.MAIN, 2].RunParams.ExpectedLineSegment.EndY = LineLineTool[0].Y + FINDLinePara[DEFINE.MAIN, 2].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].Y2;
                                                        }

                                                        FINDLineTools[DEFINE.MAIN, 2].Run();
                                                        FINDLineResults[2].FindLineTool = new CogFindLineTool(FINDLineTools[DEFINE.MAIN, 2]);

                                                        if (FINDLineTools[DEFINE.MAIN, 2].Results != null && FINDLineTools[DEFINE.MAIN, 2].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 2].Results.GetLine() != null)
                                                        {
                                                            FINDLineResults[2].SearchResult = true;
                                                            FINDLineResults[2].Pixel[DEFINE.X] = FINDLineTools[DEFINE.MAIN, 2].Results.GetLineSegment().MidpointX;
                                                            FINDLineResults[2].Pixel[DEFINE.Y] = FINDLineTools[DEFINE.MAIN, 2].Results.GetLineSegment().MidpointY;

                                                            if (DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                                                            {
                                                                DoublePoint Temp = new DoublePoint();
                                                                Temp.X2 = FINDLineTools[DEFINE.MAIN, 2].Results.GetLineSegment().Rotation * DEFINE.degree;
                                                                Temp.X2 = (90 - Temp.X2) * (-1);
                                                                FINDLineResults[2].CrossPointList.Add(Temp);
                                                                FINDLineResults[2].RectangleAngle = Temp.X2;

                                                                if (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] == DEFINE.NORMAL_RUN
                                                                    && Main.machine.m_bUseAlign1AngleLimit && Math.Abs(Temp.X2) > machine.m_f1stAlignVerticalAngleLimit)
                                                                {
                                                                    AlignUnit[m_PatAlign_No].LogdataDisplay("Vertical Angle Limit Error : " + Temp.X2.ToString() + " > ±" + machine.m_f1stAlignVerticalAngleLimit.ToString("0.0"), true);
                                                                    FINDLineResults[2].m_bAngleLimit = true;
                                                                }
                                                                else
                                                                {
                                                                    Ret |= (ushort)(1 << 2);
                                                                }
                                                            }
                                              
                                                            if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                                            {
                                                                // Vision에서 Inspection Size 구하기 위함
                                                                if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_FINE_ALIGN_MODE))
                                                                {
                                                                    InspectionPosRobot_X[2] = FINDLineResults[2].Pixel[DEFINE.X];    // Glass Line 좌표
                                                                    InspectionPosRobot_Y[2] = FINDLineResults[2].Pixel[DEFINE.Y];                                                                   
                                                                }
                                                                else if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                                                                {
                                                                    if (FINDLineTools[DEFINE.MAIN, 2].Results != null && FINDLineTools[DEFINE.MAIN, 2].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 2].Results.GetLine() != null)
                                                                        InspectionSizeRobot_X[2] = FINDLineTools[DEFINE.MAIN, 2].Results.GetLineSegment().DistanceToPoint(InspectionPosRobot_X[2], InspectionPosRobot_Y[2]);    // Cutting POL Line Gap Size
                                                                    else
                                                                        InspectionSizeRobot_X[2] = 0;
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //Ret = false;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        // No FINDLinePara[2].m_UseCheck error
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                        }


                                        // POL Edge
                                        if (DEFINE.FINDLINE_MAX >= 4)
                                        {
                                            if (FINDLinePara[DEFINE.MAIN, 3].m_UseCheck)
                                            {
                                                if ((FINDLineTools[DEFINE.MAIN, 3].Results != null && FINDLineTools[DEFINE.MAIN, 3].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 3].Results.GetLine() != null))
                                                {
                                                    FINDLineResults[3].SearchResult = true;
                                                    FINDLineResults[3].Pixel[DEFINE.X] = FINDLineTools[DEFINE.MAIN, 3].Results.GetLineSegment().MidpointX;
                                                    FINDLineResults[3].Pixel[DEFINE.Y] = FINDLineTools[DEFINE.MAIN, 3].Results.GetLineSegment().MidpointY;

                                                    DoublePoint Temp = new DoublePoint();
                                                    Temp.X = FINDLineResults[1].Pixel[DEFINE.X] - FINDLineResults[3].Pixel[DEFINE.X];
                                                    Temp.Y = FINDLineResults[1].Pixel[DEFINE.Y] - FINDLineResults[3].Pixel[DEFINE.Y];
                                                    FINDLineResults[3].CrossPointList.Add(Temp);
                                                }
                                            }
                                        }

                                        #region Shape C-Cut
                                        if ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S
                                            && (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_C)
                                        {
                                            // 2. X, Diag 교점
                                            if ((Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                                && (FINDLineTools[nFoundXIdx, 0].Results != null && FINDLineTools[nFoundXIdx, 0].Results.Count > 0 && FINDLineTools[nFoundXIdx, 0].Results.GetLine() != null)
                                                && (FINDLineTools[DEFINE.MAIN, 2].Results != null && FINDLineTools[DEFINE.MAIN, 2].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 2].Results.GetLine() != null))
                                            {
                                                LineLineTool[1] = new CogIntersectLineLineTool();
                                                LineLineTool[1].InputImage = TempImage;
                                                LineLineTool[1].LineA = FINDLineTools[nFoundXIdx, 0].Results.GetLine();
                                                LineLineTool[1].LineB = FINDLineTools[DEFINE.MAIN, 2].Results.GetLine();
                                                LineLineTool[1].Run();
                                                if (LineLineTool[1].Intersects)
                                                {
                                                    FINDLineResults[1].m_bInterSearched = true;

                                                    if (LineLineTool[1].X >= 0 && LineLineTool[1].X <= LineLineTool[1].InputImage.Width && LineLineTool[1].Y >= 0 && LineLineTool[1].Y <= LineLineTool[1].InputImage.Height)
                                                    {
                                                        FINDLineResults[1].m_bInterResult = true;
                                                        //Ret &= true;
                                                        Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossXD;

                                                        DoublePoint Temp = new DoublePoint();
                                                        Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = /*Pixel[DEFINE.X] = PixelFindLine[DEFINE.X] =*/ (LineLineTool[1].X);
                                                        Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = /*Pixel[DEFINE.Y] = PixelFindLine[DEFINE.Y] =*/ (LineLineTool[1].Y);
                                                        FINDLineResults[1].CrossPointList.Add(Temp);
                                                    }
                                                }
                                            }

                                            // 3. Y, Diag 교점
                                            if ((Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                                && (FINDLineTools[nFoundYIdx, 1].Results != null && FINDLineTools[nFoundYIdx, 1].Results.Count > 0 && FINDLineTools[nFoundYIdx, 1].Results.GetLine() != null)
                                                && (FINDLineTools[DEFINE.MAIN, 2].Results != null && FINDLineTools[DEFINE.MAIN, 2].Results.Count > 0 && FINDLineTools[DEFINE.MAIN, 2].Results.GetLine() != null))
                                            {
                                                LineLineTool[2] = new CogIntersectLineLineTool();
                                                LineLineTool[2].InputImage = TempImage;
                                                LineLineTool[2].LineA = FINDLineTools[nFoundYIdx, 1].Results.GetLine();
                                                LineLineTool[2].LineB = FINDLineTools[DEFINE.MAIN, 2].Results.GetLine();
                                                LineLineTool[2].Run();
                                                if (LineLineTool[2].Intersects)
                                                {
                                                    FINDLineResults[2].m_bInterSearched = true;

                                                    if (LineLineTool[2].X >= 0 && LineLineTool[2].X <= LineLineTool[2].InputImage.Width && LineLineTool[2].Y >= 0 && LineLineTool[2].Y <= LineLineTool[2].InputImage.Height)
                                                    {
                                                        FINDLineResults[2].m_bInterResult = true;
                                                        //Ret &= true;
                                                        Ret |= (ushort)AlignUnitTag.FindLineConstants.CrossYD;

                                                        DoublePoint Temp = new DoublePoint();
                                                        Temp.X = FINDLineResults[0].Pixel[DEFINE.X] = /*Pixel[DEFINE.X] = PixelFindLine[DEFINE.X] =*/ (LineLineTool[2].X);
                                                        Temp.Y = FINDLineResults[0].Pixel[DEFINE.Y] = /*Pixel[DEFINE.Y] = PixelFindLine[DEFINE.Y] =*/ (LineLineTool[2].Y);
                                                        FINDLineResults[2].CrossPointList.Add(Temp);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Shape R-Cut
                                    if (/*(nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_S
                                        &&*/ (nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R)
                                    {
                                        CircleResults[0].Pixel[DEFINE.X] = 0;
                                        CircleResults[0].Pixel[DEFINE.Y] = 0;
                                        CircleResults[0].R = 0;
                                        CircleResults[0].m_UseCheck = CirclePara[0].m_UseCheck;
                                        CircleResults[0].SearchResult = false;

                                        if (CirclePara[0].m_UseCheck)
                                        {
                                            CircleTools[0].InputImage = TempImage;

                                            CircleTools[0].RunParams.ExpectedCircularArc.CenterX = LineLineTool[0].X + CirclePara[0].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].X;
                                            CircleTools[0].RunParams.ExpectedCircularArc.CenterY = LineLineTool[0].Y + CirclePara[0].m_TargetToCenter[Main.DEFINE.M_FINDLINETOOL].Y;

                                            CircleTools[0].Run();

                                            CircleResults[0].CircleTool = CircleToolCopy(CircleTools[0]);

                                            if (CircleTools[0].Results == null || CircleTools[0].Results.Count <= 0)
                                            {
                                                //Ret = false;
                                                CircleResults[0].SearchResult = false;
                                            }
                                            else
                                            {
                                                Ret |= (ushort)AlignUnitTag.FindLineConstants.CircleR;

                                                CircleResults[0].SearchResult = true;

                                                CircleResults[0].Pixel[Main.DEFINE.XPOS] = CircleTools[0].Results.GetCircle().CenterX + FixtureTrans.TranslationX;
                                                CircleResults[0].Pixel[Main.DEFINE.YPOS] = CircleTools[0].Results.GetCircle().CenterY + FixtureTrans.TranslationY;
                                                CircleResults[0].R = CircleTools[0].Results.GetCircle().Radius;
                                            }//if
                                        }
                                    }
                                    #endregion
                                }

                                seq++;
                                break;


                            case 8:
                                if (nCmd == Ret)
                                    seq = SEQ.OK_SEQ;
                                else
                                    seq = SEQ.NG_SEQ; //NG
                                break;

                            case SEQ.OK_SEQ:
                                double nTempV2R_X = 0;
                                double nTempV2R_Y = 0;

                                if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                {
                                    // Vision에서 Inspection Size 구하기 위함
                                    if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                                    {
                                        for (int i = 0; i < 3; i++)
                                        {
                                            V2RScalar(InspectionSizeRobot_X[i], ref nTempV2R_X);

                                            InspectionSizeRobot_X[i] = nTempV2R_X;
                                        }
                                    }
                                }

                                for (int i = 0; i < FINDLineResults.Length; i++)
                                {
                                    if (FINDLineResults[i].CrossPointList.Count > 0)
                                    {
                                        if (i == 3) // POL Edge
                                        {
                                            V2RScalar(FINDLineResults[i].CrossPointList[0].X, ref nTempV2R_X);
                                            FINDLineResults[i].CrossPointList[0].X2 = nTempV2R_X;
                                        }
                                        else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                        {
                                            V2R(FINDLineResults[i].CrossPointList[0].X, FINDLineResults[i].CrossPointList[0].Y, ref nTempV2R_X, ref nTempV2R_Y);

                                            if (m_PatNo == DEFINE.CAM_SELECT_INSPECT)
                                            {
                                                nTempV2R_X += (m_CamOffsetX);
                                                nTempV2R_Y += (m_CamOffsetY);
                                            }

                                            FINDLineResults[i].CrossPointList[0].X2 = nTempV2R_X;
                                            FINDLineResults[i].CrossPointList[0].Y2 = nTempV2R_Y;

                                            if (i == 0 && m_PatNo == Main.DEFINE.CAM_SELECT_ALIGN)
                                            {
                                                m_RobotPosX = nTempV2R_X;
                                                m_RobotPosY = nTempV2R_Y;

                                                AlignUnit[m_PatAlign_No].m_StageX = nTempV2R_X;
                                                AlignUnit[m_PatAlign_No].m_StageY = nTempV2R_Y;
                                                AlignUnit[m_PatAlign_No].m_StageT = 0;
                                            }
                                        }
                                        else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                                        {
                                            if (i > 1) continue;

                                            V2RByCAL(FINDLineResults[i].CrossPointList[0].X, FINDLineResults[i].CrossPointList[0].Y, ref nTempV2R_X, ref nTempV2R_Y);

                                            FINDLineResults[i].CrossPointList[0].X2 = m_RobotPosX = nTempV2R_X;
                                            FINDLineResults[i].CrossPointList[0].Y2 = m_RobotPosY = nTempV2R_Y;
                                        }
                                    }
                                    else
                                    {
                                        DoublePoint Temp = new DoublePoint();
                                        Temp.X = 0;
                                        Temp.Y = 0;
                                        FINDLineResults[i].CrossPointList.Add(Temp);
                                    }
                                }

                                if (CircleResults.Length > 0 && ((nCmd & (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R) == (ushort)AlignUnitTag.GRAB_CMD.SEARCH_R))
                                {
                                    V2R(CircleResults[0].Pixel[Main.DEFINE.XPOS], CircleResults[0].Pixel[Main.DEFINE.YPOS], ref nTempV2R_X, ref nTempV2R_Y);
                                    CircleResults[0].m_RobotPosX = nTempV2R_X;
                                    CircleResults[0].m_RobotPosY = nTempV2R_Y;

                                    V2RScalar(CircleResults[0].R, ref CircleResults[0].m_RobotRadius);

                                    if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE))
                                    {
                                        InspectionSizeRobot_X[2] = CircleResults[0].m_RobotRadius;
                                    }
                                }
                                //                                 PMAlignResult.m_RobotPosX = m_RobotPosX = nTempV2R_X;
                                //                                 PMAlignResult.m_RobotPosY = m_RobotPosY = nTempV2R_Y;
                                //Ret = true;
                                SearchResult = true;
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.NG_SEQ:
                                double dTempV2R_X = 0;
                                double dTempV2R_Y = 0;

                                for (int i = 0; i < FINDLineResults.Length; i++)
                                {
                                    if (FINDLineResults[i].CrossPointList.Count <= 0)
                                    {
                                        DoublePoint Temp = new DoublePoint();
                                        Temp.X = 0;
                                        Temp.Y = 0;
                                        FINDLineResults[i].CrossPointList.Add(Temp);
                                    }
                                    else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                    {
                                        V2R(FINDLineResults[i].CrossPointList[0].X, FINDLineResults[i].CrossPointList[0].Y, ref dTempV2R_X, ref dTempV2R_Y);

                                        FINDLineResults[i].CrossPointList[0].X2 = m_RobotPosX = dTempV2R_X;
                                        FINDLineResults[i].CrossPointList[0].Y2 = m_RobotPosY = dTempV2R_Y;
                                    }
                                }
                                seq = SEQ.ERROR_SEQ;
                                break;

                            case SEQ.ERROR_SEQ:
                                AlignUnit[m_PatAlign_No].LogdataDisplay("FAILED to EXECUTE!", true);
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.COMPLET_SEQ:
                                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                                ImageSaveResult = (nCmd == Ret);
                                //AlignUnit[m_PatAlign_No].LogdataDisplay("Find lines : " + m_FindLinePositions.ToString("00"), true);
                                string strLog = "";

                                if (FINDLineResults.Length >= 2)
                                {
                                    AlignUnit[m_PatAlign_No].LogdataDisplay("H Line : " + FINDLineResults[0].m_FoundSubLineNum.ToString() + ", V Line : " + FINDLineResults[1].m_FoundSubLineNum.ToString(), true);
                                }

                                for (int i = 0; i < FINDLineResults.Length; i++)
                                {
                                    if (i > 2)  continue;

                                    for (int j = 0; j < FINDLineResults[i].CrossPointList.Count; j++)
                                    {
                                        strLog += "(" + FINDLineResults[i].CrossPointList[j].X.ToString("0.000") + "," + FINDLineResults[i].CrossPointList[j].Y.ToString("0.000") + ") ";
                                    }
                                }

                                AlignUnit[m_PatAlign_No].LogdataDisplay(strLog, true);

                                LoopFlag = false;
                                break;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    AlignUnit[m_PatAlign_No].LogdataDisplay(ex.Source + ex.Message + ex.StackTrace, true);

                    double dTempV2R_X = 0;
                    double dTempV2R_Y = 0;

                    for (int i = 0; i < FINDLineResults.Length; i++)
                    {
                        if (FINDLineResults[i].CrossPointList.Count <= 0)
                        {
                            DoublePoint Temp = new DoublePoint();
                            Temp.X = 0;
                            Temp.Y = 0;
                            FINDLineResults[i].CrossPointList.Add(Temp);
                        }
                        else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                        {
                            V2R(FINDLineResults[i].CrossPointList[0].X, FINDLineResults[i].CrossPointList[0].Y, ref dTempV2R_X, ref dTempV2R_Y);

                            FINDLineResults[i].CrossPointList[0].X2 = m_RobotPosX = dTempV2R_X;
                            FINDLineResults[i].CrossPointList[0].Y2 = m_RobotPosY = dTempV2R_Y;
                        }
                    }

                    AlignUnit[m_PatAlign_No].LogdataDisplay("(catch) FAILED to EXECUTE!", true);

                    if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                    ImageSaveResult = (nCmd == Ret);

                    return 0;
                }

                return Ret;
            }

            private List<DoublePoint> FINDLINE_4POINT_ARRAY(List<DoublePoint> CircleList)
            {
                ////0   1
                ////2   3   순서 변경.
                DoublePoint Temp = new DoublePoint();
                double[] CenterPoint = new double[2];
                CircleList = CircleList.OrderBy(p => p.Y).ToList();
                for (int i = 0; i < CircleList.Count; i++)
                {
                    CenterPoint[0] += CircleList[i].X;
                    CenterPoint[1] += CircleList[i].Y;
                }
                CenterPoint[0] = CenterPoint[0] / 4;
                CenterPoint[1] = CenterPoint[1] / 4;

                if (CircleList[0].X > CircleList[1].X)
                {
                    Temp = CircleList[0];
                    CircleList[0] = CircleList[1];
                    CircleList[1] = Temp;
                }
                if (CircleList[2].X > CircleList[3].X)
                {
                    Temp = CircleList[2];
                    CircleList[2] = CircleList[3];
                    CircleList[3] = Temp;
                }
                return CircleList;
            }
            public static DoublePoint RotationChange(DoublePoint CenterPoint, DoublePoint OriginPoint, double Angle)
            {
                DoublePoint ReturnCircle = new DoublePoint();
                ReturnCircle.X = ((OriginPoint.X - CenterPoint.X) * Math.Cos(Angle) - (OriginPoint.Y - CenterPoint.Y) * Math.Sin(Angle)) + CenterPoint.X;
                ReturnCircle.Y = ((OriginPoint.X - CenterPoint.X) * Math.Sin(Angle) + (OriginPoint.Y - CenterPoint.Y) * Math.Cos(Angle)) + CenterPoint.Y;
                return ReturnCircle;
            }
            private DoublePoint FINDLINE_Rectangle_AngleCalc(List<DoublePoint> CircleList, ref double RectangleAngle)
            {
                double[] dx = new double[4];
                double[] dy = new double[4];
                double[] nCAMFixPos_X = new double[4];
                double[] nCAMFixPos_Y = new double[4];
                double t1, t2, t3, t4;
                for (int i = 0; i < CircleList.Count; i++)
                {
                    nCAMFixPos_X[i] = CircleList[i].X;
                    nCAMFixPos_Y[i] = CircleList[i].Y;
                }
                // t1
                dx[0] = (nCAMFixPos_X[1] - nCAMFixPos_X[0]);
                dy[0] = nCAMFixPos_Y[1] - nCAMFixPos_Y[0];
                if (dx[0] != 0) t1 = Math.Atan(dy[0] / dx[0]); else t1 = 0;
                // t2
                dx[1] = (nCAMFixPos_X[3] - nCAMFixPos_X[2]);
                dy[1] = nCAMFixPos_Y[3] - nCAMFixPos_Y[2];
                if (dx[1] != 0) t2 = Math.Atan(dy[1] / dx[1]); else t2 = 0;
                // t3
                dx[2] = nCAMFixPos_X[2] - nCAMFixPos_X[0];
                dy[2] = (nCAMFixPos_Y[0] - nCAMFixPos_Y[2]);
                if (dx[2] != 0) t3 = Math.Atan(dx[2] / dy[2]); else t3 = 0;
                // t4
                dx[3] = nCAMFixPos_X[3] - nCAMFixPos_X[1];
                dy[3] = (nCAMFixPos_Y[1] - nCAMFixPos_Y[3]);
                if (dx[3] != 0) t4 = Math.Atan(dx[3] / dy[3]); else t4 = 0;

                double dt = (t1 + t2 + t3 + t4) / (4);

                RectangleAngle = dt * 180 / Math.PI;

                ///////////////////////////////////////  Center Point   ////////////////////////////////////////////////
                DoublePoint Center = new DoublePoint();
                Center.X = (nCAMFixPos_X[0] + nCAMFixPos_X[1] + nCAMFixPos_X[2] + nCAMFixPos_X[3]) / 4;
                Center.Y = (nCAMFixPos_Y[0] + nCAMFixPos_Y[1] + nCAMFixPos_Y[2] + nCAMFixPos_Y[3]) / 4;
                return Center;
            }

            public void GetLineMaxIndex(int nLineType, FindLineTagData FindLinePara, CogLineMaxTool LineMaxTool, ref int nLineIndex)
            {
                if (nLineType == 0 && LineMaxTool.Results.Count > 1)
                {
                    for (int h = 1; h < LineMaxTool.Results.Count; h++)
                    {
                        if (FindLinePara.m_LineMaxHCond == Main.DEFINE.LINEMAX_H_YMIN)
                        {
                            // Search Y Min Index
                            if (LineMaxTool.Results[h].GetLine().Y < LineMaxTool.Results[nLineIndex].GetLine().Y)
                            {
                                nLineIndex = h;
                            }
                        }
                        else if (FindLinePara.m_LineMaxHCond == Main.DEFINE.LINEMAX_H_YMAX)
                        {
                            // Search Y Max Index
                            if (LineMaxTool.Results[h].GetLine().Y > LineMaxTool.Results[nLineIndex].GetLine().Y)
                            {
                                nLineIndex = h;
                            }
                        }
                    }
                }
                // Horizontal Y Min Y Max
                else if (nLineType == 1 && LineMaxTool.Results.Count > 1)
                {
                    for (int v = 1; v < LineMaxTool.Results.Count; v++)
                    {
                        if (FindLinePara.m_LineMaxVCond == Main.DEFINE.LINEMAX_V_XMIN)
                        {
                            // Search X Min Index
                            if (LineMaxTool.Results[v].GetLine().X < LineMaxTool.Results[nLineIndex].GetLine().X)
                            {
                                nLineIndex = v;
                            }
                        }
                        else if (FindLinePara.m_LineMaxVCond == Main.DEFINE.LINEMAX_V_XMAX)
                        {
                            // Search X Max Index
                            if (LineMaxTool.Results[v].GetLine().X > LineMaxTool.Results[nLineIndex].GetLine().X)
                            {
                                nLineIndex = v;
                            }
                        }
                    }
                }
            }

            private CogCaliperTool CaliperToolCopy(CogCaliperTool nSourceTool)
            {
                CogCaliperTool nCopyTool = new CogCaliperTool();
                //Main.Mutex_lock_CaliperTool.WaitOne();
                try
                {
                    nCopyTool = new CogCaliperTool(nSourceTool);
                }
                catch
                {

                }
                finally
                {
                    //Main.Mutex_lock_CaliperTool.ReleaseMutex();
                }
                return nCopyTool;
            }
            public CogCaliperTool CaliperToolPairRun(CogCaliperTool nSourceTool, out int nPlusMinus)
            {
                int TempPlusMinus = 1; ;
                CogCaliperTool nCopyTool = new CogCaliperTool();
                //Main.Mutex_lock_CaliperTool.WaitOne();
                try
                {
                    nCopyTool = new CogCaliperTool(nSourceTool);
                    CogRectangleAffine nBackUpRect = new CogRectangleAffine(nCopyTool.Region);


                    if (Main.GetCaliperDirection(Main.GetCaliperDirection(nCopyTool.Region.Rotation)) == Main.DEFINE.Y)
                    { //-----------------------------------------------------------------------------------------+++++++++
                        nCopyTool.Region.SideXLength = nCopyTool.Region.SideXLength / 2;
                        nCopyTool.Region.CenterY = nBackUpRect.CenterY - nCopyTool.Region.SideXLength / 2; //+++++++++
                        nCopyTool.Run();
                        TempPlusMinus = -1;
                        //-----------------------------------------------------------------------------------------
                        if (nCopyTool.Results == null || nCopyTool.Results.Count <= 0)
                        {
                            nCopyTool.Region.CenterY = nBackUpRect.CenterY + nCopyTool.Region.SideXLength / 2; // --------
                            nCopyTool.Run();
                            TempPlusMinus = 1;
                        }
                    }
                    else
                    {
                        { //-----------------------------------------------------------------------------------------+++++++++
                            nCopyTool.Region.SideXLength = nCopyTool.Region.SideXLength / 2;
                            nCopyTool.Region.CenterX = nBackUpRect.CenterX - nCopyTool.Region.SideXLength / 2; //+++++++++
                            nCopyTool.Run();
                            TempPlusMinus = 1;
                            //-----------------------------------------------------------------------------------------
                            if (nCopyTool.Results == null || nCopyTool.Results.Count <= 0)
                            {
                                nCopyTool.Region.CenterX = nBackUpRect.CenterX + nCopyTool.Region.SideXLength / 2; // --------
                                nCopyTool.Run();
                                TempPlusMinus = -1;
                            }
                        }
                     }
                    //-----------------------------------------------------------------------------------------
                    nCopyTool.Region = new CogRectangleAffine(nBackUpRect);
                }
                catch
                {

                }
                finally
                {
                    //Main.Mutex_lock_CaliperTool.ReleaseMutex();
                }
                nPlusMinus = TempPlusMinus;
                return nCopyTool;
            }
      

            #region CIRCLE

            public bool Search_Circle()
            {
                return Search_Circle(m_CamNo);
            }
            public bool Search_Circle(int CAM)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool Ret = false;

                CogImage8Grey TempIMG = new CogImage8Grey();
                double nTempV2R_X = 0;
                double nTempV2R_Y = 0;
                try
                {
                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                PMAlignResult.m_PatternName = m_PatternName;
                                PMAlignResult.m_CamNo = m_CamNo;
                                seq++;
                                break;

                            case 1:
                                seq++;
                                break;

                            case 2:
                                seq++;
                                break;

                            case 3:
                                seq++;
                                break;

                            case 4:
                                for (int ii = 0; ii < Main.DEFINE.CIRCLE_MAX; ii++)
                                {
                                    if (CirclePara[ii].m_UseCheck)
                                        Ret = true;
                                }
                                seq++;
                                break;


                            case 5:
                                for (int i = 0; i < Main.DEFINE.CIRCLE_MAX; i++)
                                {
                                    CircleResults[i].Pixel[DEFINE.X] = 0;
                                    CircleResults[i].Pixel[DEFINE.Y] = 0;
                                    CircleResults[i].R = 0;
                                    CircleResults[i].m_UseCheck = CirclePara[i].m_UseCheck;
                                    if (CirclePara[i].m_UseCheck)
                                    {
                                        CircleTools[i].InputImage = TempImage;

                                        CircleTools[i].Run();

                                        CircleResults[i].CircleTool = CircleToolCopy(CircleTools[i]);

                                        

                                        if (CircleTools[i].Results == null || CircleTools[i].Results.Count <= 0)
                                        {
                                            Ret = false;
                                            CircleResults[i].SearchResult = false;
                                        }
                                        else
                                        {
                                            CircleResults[i].SearchResult = true;

                                            CircleResults[i].Pixel[Main.DEFINE.XPOS] = CircleTools[i].Results.GetCircle().CenterX + FixtureTrans.TranslationX;
                                            CircleResults[i].Pixel[Main.DEFINE.YPOS] = CircleTools[i].Results.GetCircle().CenterY + FixtureTrans.TranslationY;
                                            CircleResults[i].R = CircleTools[i].Results.GetCircle().Radius;

                                            V2R(CircleResults[i].Pixel[Main.DEFINE.XPOS] , CircleResults[i].Pixel[Main.DEFINE.YPOS] , ref nTempV2R_X, ref nTempV2R_Y);
                                            CircleResults[i].m_RobotPosX =  nTempV2R_X;
                                            CircleResults[i].m_RobotPosY =  nTempV2R_Y;

                                            V2R(CircleResults[i].R, CircleResults[i].R, ref nTempV2R_X, ref nTempV2R_Y);
                                            CircleResults[i].m_RobotRadius = (Math.Abs(nTempV2R_X) + Math.Abs(nTempV2R_Y)) / 2.0;   // TEMP
                                        }//if
                                    }

                                }
                                if (Ret)
                                    seq = SEQ.OK_SEQ;
                                else
                                    seq = SEQ.NG_SEQ; //NG
                                break;

                            case SEQ.OK_SEQ:
                                Ret = true;
                                seq = SEQ.OK_SEQ + 1;
                                break;

                            case SEQ.OK_SEQ + 1:

                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.NG_SEQ:
                                Ret = false;
                                seq = SEQ.ERROR_SEQ;
                                break;

                            case SEQ.ERROR_SEQ:
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.COMPLET_SEQ:
                                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                                ImageSaveResult = Ret;
                                LoopFlag = false;
                                break;
                        }

                    }
                }
                catch
                {
                    Ret = false;
                }
                return Ret;
            }
            public CogFindCircleTool CircleToolCopy(CogFindCircleTool nSourceTool)
            {
                CogFindCircleTool nCopyTool = new CogFindCircleTool();
               // Main.Mutex_lock_CircleTool.WaitOne();
                try
                {
                    nCopyTool = new CogFindCircleTool(nSourceTool);
                }
                catch
                {

                }
                finally
                {
                    //Main.Mutex_lock_CircleTool.ReleaseMutex();
                }
                return nCopyTool;
            }
            
            #endregion

            #region BLOB
            public bool BLOBSearch()
            {
                return Search_BLOB(m_CamNo);
            }
            public bool Search_BLOB(int CAM)
            {
                int seq = 0;
                bool LoopFlag = true;
                bool Ret = false;
                double tempTotalPixel = 0;

                List<int> nBlobNum = new List<int>();
                List<BlobTagData> nBlobResultsList = new List<BlobTagData>();

                try
                {
                    while (LoopFlag)
                    {
                        switch (seq)
                        {
                            case 0:
                                PMAlignResult.m_PatternName = m_PatternName;
                                PMAlignResult.m_CamNo = m_CamNo;
                                BlobScore = "";
                                seq++;
                                break;

                            case 1:
                                for (int i = 0; i < Main.DEFINE.BLOB_CNT_MAX; i++)
                                {
                                    if (BlobPara[i].m_UseCheck) Ret = true;
                                }
                                seq++;
                                break;

                            case 2:// Blob Tool para
                                for (int i = 0; i < Main.DEFINE.BLOB_CNT_MAX; i++)
                                {
                                    BlobResults[i].m_UseCheck = BlobPara[i].m_UseCheck;
                                    if (BlobPara[i].m_UseCheck)
                                    {
                                        BlobTools[i].InputImage = TempImage;

                                        if (Main.ALIGNINSPECTION_USE(m_PatAlign_No))
                                        {
                                            BlobTools[i] = BlobToolPairRun(BlobTools[i], i, out BlobResults[i].m_PlusMinus);
                                        }
                                        else
                                        {
                                            BlobTools[i].Run();
                                        }
                                        //-------------------------------DATA COPY FOR BLOB_DRAW---------------------------------------------
                                        BlobResults[i].SearchRegion = new CogRectangleAffine(BlobTools[i].Region as CogRectangleAffine);
//                                         BlobResults[i].SearchRegion.X = BlobResults[i].SearchRegion.X; //+ FixtureTrans.TranslationX;
//                                         BlobResults[i].SearchRegion.Y = BlobResults[i].SearchRegion.Y; //+ FixtureTrans.TranslationY;
                                        if (i > 0) BlobScore += " , ";
                                        try
                                        {
                                    //        double tempTotalPixel = 0;
                                            double TempBlobScore = 0;
                                            for (int j = 0; j < BlobTools[i].Results.GetBlobs().Count; j++)
                                            {
                                                if (BlobResults[i].m_UseCheck && (BlobTools[i].Results.GetBlobs()[j].Area != null))
                                                    tempTotalPixel += BlobTools[i].Results.GetBlobs()[j].Area;
                                                TempBlobScore = 100 - ((tempTotalPixel / BlobResults[i].SearchRegion.Area) * 100);
                                                if(j ==BlobTools[i].Results.GetBlobs().Count-1 ) BlobScore += TempBlobScore.ToString("0.0");
                                            }
                                        }
                                        catch
                                        { 

                                        }
                                        //-------------------------------------------------------------------------------------------------
                                        if (BlobTools[i].Results != null)
                                        {
                                            BlobResults[i].m_TrayBlobNo = i;
                                            BlobResults[i].BlobToolResult = new CogBlobResults(BlobTools[i].Results);
                                            BlobResults[i].Pixel[DEFINE.X] = FixtureTrans.TranslationX;
                                            BlobResults[i].Pixel[DEFINE.Y] = FixtureTrans.TranslationY;
                                          
                                            if (BlobTools[i].Results.GetBlobs().Count > 0)
                                            {
                                                if ((Main.BLOBINSPECTION_USE(m_PatAlign_No) && i < Main.DEFINE.BLOB_INSP_LIMIT_CNT) || Main.ALIGNINSPECTION_USE(m_PatAlign_No))
                                                {
                                                    BlobMinMax_Control(i);
                                                }
                                                if (!Main.BLOBINSPECTION_USE(m_PatAlign_No) || i >= Main.DEFINE.BLOB_INSP_LIMIT_CNT)
                                                    BlobResults[i].SearchResult = Ret = false;
                                            }
                                            else
                                            {
                                                BlobScore += "100";
                                                BlobResults[i].SearchResult = true;
                                            }
                                        }
                                        else
                                        {
                                            BlobResults[i].BlobToolResult = null;

                                            if (Main.ALIGNINSPECTION_USE(m_PatAlign_No))
                                            {
                                                BlobResults[i].SearchResult = true;
                                            }
                                            else
                                            {
                                                BlobResults[i].SearchResult = Ret = false;
                                            }
                                        }
                                    }
                                }
                                if (Main.BLOBINSPECTION_USE(m_PatAlign_No))
                                {
                                    Y_MAXGAP[0, 1] = Y_MAXGAP[1, 1] = Y_MAXGAP[2, 1] = Y_MAXGAP[3, 1] = 0;
                                    Blob_InspCalc_Control(m_Blob_InspCnt);
                                    Main.AlignUnit[m_PatAlign_No].WriteInspGap(Y_MAXGAP[0, 1], Y_MAXGAP[1, 1], Y_MAXGAP[2, 1], Y_MAXGAP[3, 1]);
                                }
                                if (Main.BLOBINSPECTION_USE(m_PatAlign_No))
                                {
                                    Ret = true;
                                }
                                if (Ret)
                                    seq = SEQ.OK_SEQ;
                                else
                                    seq = SEQ.NG_SEQ; //NG
                                break;

                            case SEQ.OK_SEQ:
                                seq = SEQ.COMPLET_SEQ;
                                break;

                            case SEQ.NG_SEQ:
                                seq = SEQ.ERROR_SEQ;
                                break;

                            case SEQ.ERROR_SEQ:
                                seq = SEQ.COMPLET_SEQ;
                                Ret = false;
                                break;

                            case SEQ.COMPLET_SEQ:
  
                                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use | AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use) Save_Image_Copy();
                                ImageSaveResult = Ret;
                                LoopFlag = false;
                                break;
                        }

                    }
                }
                catch
                {
                    Ret = false;
                }
                return Ret;
            }
            private CogBlobTool BlobToolCopy(CogBlobTool nSourceTool)
            {
                CogBlobTool nCopyTool = new CogBlobTool();
               // Main.Mutex_lock_BlobTool.WaitOne();
                try
                {
                    nCopyTool = new CogBlobTool(nSourceTool);
                }
                catch
                {

                }
                finally
                {
                   // Main.Mutex_lock_BlobTool.ReleaseMutex();
                }
                return nCopyTool;
            }
            public CogBlobTool BlobToolPairRun(CogBlobTool nSourceTool, int nDirection, out int nPlusMinus)
            {
                int TempPlusMinus = 1; ;
                CogBlobTool nCopyTool = new CogBlobTool();
               // Main.Mutex_lock_BlobTool.WaitOne();
                try
                {
                    nCopyTool = new CogBlobTool(nSourceTool);


                    CogRectangle nTempRect = new CogRectangle();
                    nTempRect.SetCenterWidthHeight(((CogRectangleAffine)nCopyTool.Region).CenterX,((CogRectangleAffine)nCopyTool.Region).CenterY,((CogRectangleAffine)nCopyTool.Region).SideXLength,((CogRectangleAffine)nCopyTool.Region).SideYLength);

                    CogRectangleAffine nBackUpRectAffine = new CogRectangleAffine((CogRectangleAffine)nCopyTool.Region);

                    CogRectangle nBackUpRect = new CogRectangle(nTempRect);
                    CogRectangle nSearchRect = new CogRectangle(nTempRect);
                   // CogRectangle nBackUpRect = new CogRectangle((CogRectangle)nCopyTool.Region);
                   // CogRectangle nSearchRect = new CogRectangle((CogRectangle)nCopyTool.Region);

                    if (nDirection == Main.DEFINE.HEIGHT)
                    { //-----------------------------------------------------------------------------------------+++++++++                        
                        nSearchRect.SetCenterWidthHeight(nBackUpRect.CenterX, nBackUpRect.CenterY - (nBackUpRect.Height / 4), nBackUpRect.Width, nBackUpRect.Height / 2);
                        nCopyTool.Region = new CogRectangle(nSearchRect);
                        nCopyTool.Run();
                        TempPlusMinus = -1;
                        //-----------------------------------------------------------------------------------------
                        if (nCopyTool.Results == null || nCopyTool.Results.GetBlobs().Count <= 0)
                        {
                            nSearchRect.SetCenterWidthHeight(nBackUpRect.CenterX, nBackUpRect.CenterY + (nBackUpRect.Height / 4), nBackUpRect.Width, nBackUpRect.Height / 2);
                            nCopyTool.Region = new CogRectangle(nSearchRect);
                            nCopyTool.Run();
                            TempPlusMinus = 1;
                        }
                    }
                    else //(nDirection == Main.DEFINE.WIDTH_)
                    {
                        { //-----------------------------------------------------------------------------------------+++++++++                             
                            nSearchRect.SetCenterWidthHeight(nBackUpRect.CenterX - (nBackUpRect.Width / 4), nBackUpRect.CenterY, nBackUpRect.Width / 2, nBackUpRect.Height);
                            nCopyTool.Region = new CogRectangle(nSearchRect);
                            nCopyTool.Run();
                            TempPlusMinus = 1;
                            //-----------------------------------------------------------------------------------------
                            if (nCopyTool.Results == null || nCopyTool.Results.GetBlobs().Count <= 0)
                            {
                                nSearchRect.SetCenterWidthHeight(nBackUpRect.CenterX + (nBackUpRect.Width / 4), nBackUpRect.CenterY, nBackUpRect.Width / 2, nBackUpRect.Height);
                                nCopyTool.Region = new CogRectangle(nSearchRect);
                                nCopyTool.Run();
                                TempPlusMinus = -1;
                            }
                        }
                    }
                    //-----------------------------------------------------------------------------------------
                    nCopyTool.Region = new CogRectangleAffine(nBackUpRectAffine);
                }
                catch
                {

                }
                finally
                {
                   // Main.Mutex_lock_BlobTool.ReleaseMutex();
                }
                nPlusMinus = TempPlusMinus;
                return nCopyTool;
            }
            public void BlobMinMax_Control(int i)
            {
                int BlobNo = 0;
                BlobNo = i;

                /////2/////
                //0//////1//
                /////3/////

                double[] VertexValue = new double[4];
                int[,] VertexArray = new int[4, 2];

                int POS_I = 0, POS_J = 1;
                int POS_X = 0, POS_Y = 1;
                int MIN_X = 0, MAX_X = 1, MIN_Y = 2, MAX_Y = 3;

                if (BlobTools[BlobNo].Results.GetBlobs().Count > 0)
                {
                    int GetBlobsCount = 1;
                    if (!Main.ALIGNINSPECTION_USE(m_PatAlign_No)) GetBlobsCount = BlobTools[BlobNo].Results.GetBlobs().Count;

                    for (i = 0; i < GetBlobsCount; i++) //BlobTools[BlobNo].Results.GetBlobs().Count
                    {
                        for (int j = 0; j < BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertices().Length / 2; j++)
                        {

                            if (i == 0 && j == 0)
                            {
                                VertexValue[MIN_X] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                VertexValue[MAX_X] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                VertexValue[MIN_Y] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                VertexValue[MAX_Y] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                            }
                            if (BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexX(j) < VertexValue[MIN_X])
                            {
                                VertexValue[MIN_X] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                VertexArray[MIN_X, POS_I] = i;
                                VertexArray[MIN_X, POS_J] = j;
                            }
                            if (BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexX(j) > VertexValue[MAX_X])
                            {
                                VertexValue[MAX_X] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                VertexArray[MAX_X, POS_I] = i;
                                VertexArray[MAX_X, POS_J] = j;
                            }

                            if (BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexY(j) < VertexValue[MIN_Y])
                            {
                                VertexValue[MIN_Y] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                VertexArray[MIN_Y, POS_I] = i;
                                VertexArray[MIN_Y, POS_J] = j;
                            }
                            if (BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexY(j) > VertexValue[MAX_Y])
                            {
                                VertexValue[MAX_Y] = BlobTools[BlobNo].Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                VertexArray[MAX_Y, POS_I] = i;
                                VertexArray[MAX_Y, POS_J] = j;
                            }
                        }
                    }

                    for (i = 0; i < 4; i++)
                    {
                        BlobResults[BlobNo].VertexResults[i, POS_X] = BlobTools[BlobNo].Results.GetBlobs()[VertexArray[i, POS_I]].GetBoundary().GetVertexX(VertexArray[i, POS_J]);
                        BlobResults[BlobNo].VertexResults[i, POS_Y] = BlobTools[BlobNo].Results.GetBlobs()[VertexArray[i, POS_I]].GetBoundary().GetVertexY(VertexArray[i, POS_J]);
                    }
                }
            }
            public void Blob_InspCalc_Control(int BlobInspCnt)
            {
                int POS_X = 0, POS_Y = 1;
                int MIN_Y = 2, MAX_Y = 3;
                int BlobCnt = 0, IncCnt = 0;

                BlobCnt = BlobInspCnt;
                IncCnt = 2;
                for (int i = 0; i < BlobCnt; i++)
                {
                    Y_MAXGAP[i, POS_X] = BlobResults[i * IncCnt].VertexResults[MIN_Y, POS_X];
                    Y_MAXGAP[i, POS_Y] = BlobResults[i * IncCnt + 1].VertexResults[MAX_Y, POS_Y] - BlobResults[i * IncCnt].VertexResults[MIN_Y, POS_Y];
                    Y_MAXGAP[i, POS_X] *= m_CalX[0];
                    Y_MAXGAP[i, POS_Y] *= m_CalY[0];
                }
            }
            #endregion

            public void DrawResult(CogRecordDisplay Display)
            {
                Display.Image = TempImage;
                Main.DisplayClear(Display);

                string[] nMessage = new string[2];
                nMessage[0] = "Mark Search Error";
                nMessage[1] = "";
                //DisplayFit(Display);
                if (!CaliperDraw && !BlobDraw && !SearchResult && !FINDLineDraw)
                {
                    DrawOverlayPMAlign(Display, PMAlignResult);
                }
                else
                {
                    if (!CaliperDraw && !BlobDraw && !FINDLineDraw)
                    {
                            DrawOverlayPMAlign(Display, PMAlignResult);
                    }
                    if (CaliperDraw)
                    {
                        #region CALIPER
                        if (Caliper_MarkUse)
                        {
                            if (SearchResult)
                            {
                                DrawOverlayPMAlign(Display, PMAlignResult);
                                DrawOverlayCaliper(Display, CaliResults);
                            }
                            else
                                DrawOverlayMessage(Display, nMessage);
                        }
                        else
                        {
                            DrawOverlayCaliper(Display, CaliResults);
                        }
                        CaliperDraw = false;
                        #endregion
                    }
                    if (BlobDraw)
                    {
                        #region BLOB
                        if (Blob_MarkUse)
                        {
                            if (SearchResult)
                            {
                                DrawOverlayPMAlign(Display, PMAlignResult);
                                DrawOverlayBlobTool(Display, BlobResults);
                            }
                            else
                            {
                                DrawOverlayMessage(Display, nMessage);
                            }
                        }
                        else
                        {
                            if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "FPC_TRAY1" || Main.AlignUnit[m_PatAlign_No].m_AlignName == "FPC_TRAY2")
                            {
                                if (nPocketNum < 0)
                                    DrawOverlayTrayBlobTool(Display, TRAYBlobResult);
                                else
                                {

                                    try
                                    {
                                        if (TRAYBlobResult != null && TRAYBlobResult.Count > nPocketNum)
                                        {
                                            DrawOverlayTrayBlobTool(Display, TRAYBlobResult[nPocketNum]);
                                        }
                                    }
                                    catch
                                    {

                                    }
                                }
                            }
                            else
                                DrawOverlayBlobTool(Display, BlobResults);                       
                        }
                        BlobDraw = false;
                        #endregion
                    }
                    if (FINDLineDraw)
                    {
                        #region FINDLine
                        if (FINDLine_MarkUse)
                        {
                            if (SearchResult)
                            {
                                DrawOverlayPMAlign(Display, PMAlignResult);
                                DrawOverlayFindLine(Display, FINDLineResults);
                            }
                            else
                            {
                                DrawOverlayMessage(Display, nMessage);
                            }
                        }
                        else
                        {
                            if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "FPC_TRAY1" || Main.AlignUnit[m_PatAlign_No].m_AlignName == "FPC_TRAY2")
                            {
                                if (nPocketNum < 0)
                                    DrawOverlayPMAlign(Display, TRAYResults);
                                else
                                {
                                    if (TRAYResults != null && TRAYResults.Count > nPocketNum)
                                    {
                                        DrawOverlayPMAlign(Display, TRAYResults[nPocketNum]);
                                    }
                                }

                            }
                            else
                            {
                                if ((Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2") && FINDLineResults[0].m_bInterSearched == false)
                                {
                                    nMessage[0] = "Line Search Error!";
                                    DrawOverlayMessage(Display, nMessage);
                                }
                                else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" && (FINDLineResults[0].m_bLoadingLimitOver_X == true || FINDLineResults[0].m_bLoadingLimitOver_Y == true))
                                {
                                    nMessage[0] = "";
                                    nMessage[1] = "Panel Loading Limit Error!";

                                    DrawOverlayMessage(Display, nMessage);
                                    DrawOverlayFindLine(Display, FINDLineResults);
                                }
                                else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.RUN_MODE] == DEFINE.NORMAL_RUN && (FINDLineResults[0].m_bAngleLimit == true || FINDLineResults[2].m_bAngleLimit == true))
                                {
                                    nMessage[0] = "NG -> X : " + FINDLineResults[0].CrossPointList[0].X2.ToString("0.000") + " Y : " + FINDLineResults[0].CrossPointList[0].Y2.ToString("0.000");
                                    nMessage[1] = " Corner : " + FINDLineResults[0].RectangleAngle.ToString("0.000") + " T : " + FINDLineResults[2].RectangleAngle.ToString("0.000");
                                    DrawOverlayMessage(Display, nMessage);
                                    DrawOverlayFindLine(Display, FINDLineResults);
                                }
                                else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" && FINDLineResults[0].m_bInterSearched == true)
                                {
                                    nMessage[0] = "OK -> X : " + FINDLineResults[0].CrossPointList[0].X2.ToString("0.000") + " Y : " + FINDLineResults[0].CrossPointList[0].Y2.ToString("0.000");
                                    nMessage[1] = "OK -> Corner : " + FINDLineResults[0].RectangleAngle.ToString("0.000") + " T : " + FINDLineResults[2].RectangleAngle.ToString("0.000");
                                    DrawOverlayMessage(Display, nMessage);
                                    DrawOverlayFindLine(Display, FINDLineResults);
                                }
                                else
                                    DrawOverlayFindLine(Display, FINDLineResults);
                            }
                        }
                        FINDLineDraw = false;
                        #endregion
                    }
                    if (CircleDraw)
                    {
                        #region CIRCLE
                        if (Circle_MarkUse)
                        {
                            if (SearchResult)
                            {
                                DrawOverlayPMAlign(Display, PMAlignResult);
                                DrawOverlayCircle(Display, CircleResults);
                            }
                            else
                            {
                                DrawOverlayMessage(Display, nMessage);
                            }
                        }
                        else
                        {
                            DrawOverlayCircle(Display, CircleResults);
                        }
                        CircleDraw = false;
                        #endregion
                    }
//                     if (Main.ALIGNINSPECTION_USE(m_PatAlign_No))
//                         DrawOverlayMessage(Display, Main.AlignUnit[m_PatAlign_No].m_Message, FixtureTrans.TranslationX, FixtureTrans.TranslationY);

                }
                //DisplayFit(Display);
                //    CrossLine(Display, m_CamNo);
                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use && ImageSaveResult) Save_Image("OK", Display);
                if (AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use && !ImageSaveResult) Save_Image("NG", Display);
            }
            public void DrawResultCalibration(CogRecordDisplay Display)
            {
                Display.Image = Main.vision.CogCamBuf[m_CamNo];
                DrawCalibration(Display, m_CamNo, Pixel[DEFINE.X], Pixel[DEFINE.Y]);
                if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use && ImageSaveResult) Save_Image("OK", Display);
                if (AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use && !ImageSaveResult) Save_Image("NG", Display);
            }
            public void DrawResultCalibration_Theta(CogRecordDisplay Display)
            {
                Display.Image = Main.vision.CogCamBuf[m_CamNo];
                if (FitCicleTool.Result != null)
                {
                    DrawCalibration_Theta(Display, FitCicleTool);
                    if (AlignUnit[m_PatAlign_No].m_GD_ImageSave_Use && ImageSaveResult) Save_Image("OK", Display);
                    if (AlignUnit[m_PatAlign_No].m_NG_ImageSave_Use && !ImageSaveResult) Save_Image("NG", Display);
                }
            }
            public void GetImage()
            {
                string LogMsg;
                if (Main.DEFINE.OPEN_F)
                {
                    TempImage = new CogImage8Grey(Main.vision.CogCamBuf[m_CamNo] as CogImage8Grey);
                    Save_Image_Copy();
                    return;
                }
                int seq = 0;
                bool LoopFlag = true;

                while (LoopFlag)
                {
                    switch (seq)
                    {
                        case 0:
                            Main.vision.Grab_Flag_End[m_CamNo] = false;
                            Main.vision.Grab_Flag_Start[m_CamNo] = true;
                            m_Timer.StartTimer();
                            GrabComplete = false;
                            seq++;
                            break;

                        case 1:
                            if (m_Timer.GetElapsedTime() > DEFINE.GRAB_TIMEOUT)
                            {
                                seq = SEQ.ERROR_SEQ;
                                break;
                            }
                            if (!Main.vision.Grab_Flag_End[m_CamNo] /*&& Main.vision.Grab_Flag_Start[m_CamNo]*/)
                                break;
                            seq++;
                            break;

                        case 2:
                            TempImage = new CogImage8Grey(Main.vision.CogCamBuf[m_CamNo] as CogImage8Grey);
                            Save_Image_Copy();
                           
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.ERROR_SEQ:
                            seq = SEQ.COMPLET_SEQ;
                            break;

                        case SEQ.COMPLET_SEQ:
//                            Main.vision.Grab_Flag_End[m_CamNo] = false;
                            LoopFlag = false;
                            GrabComplete = true;
                            break;
                    }
                    //	Thread.Sleep(10);
                }
            }

            #region _____________________________________________Light_Control_______________________________________________
            public void SetAllLightOFF()
            {
                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    SetLight(i, 0);
                }
            }
            public void SetAllLight(int ToolMode)
            {
                try
                {
                    for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                    {
                        SetLight(i, ToolMode, 0);
                    }
                }
                catch
                {

                }
            }
            public bool LightCompare(int LightType1, int LightType2)
            {
                bool ret = true;
                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    if (m_LightValue[i, LightType1] != m_LightValue[i, LightType2])
                        ret = false;
                }
                return ret;
            }
            public void SetLight(int LightNum, int ToolMode, int Null)
            {
                SetLightCtrl(m_LightCtrl[LightNum], m_LightCH[LightNum], m_LightValue[LightNum, ToolMode]);
            }
            public void SetLight(int LightNum, int m_Value)
            {
                SetLightCtrl(m_LightCtrl[LightNum], m_LightCH[LightNum], m_Value);
            }
            public void SetLightCtrl(int LightControl, int Channel, int Value)
            {
                SetLightCtrl_232(LightControl, Channel, Value);

            }
            public void SetLightCtrl_232(int LightControl, int Channel, int Value)
            {
                if (Main.DEFINE.OPEN_F || Main.DEFINE.OPEN_CAM)
                    return;
                try
                {
                    string m_sendData = "";
                    int mChannel;
                    if (LightControl < 0) return;
                    //if (LightCurrent[LightControl, Channel] == Value) return; // 한번 통신 등 오류나서 조명값 변경 안되면 그 뒤로 계속 return 됨
                    LightCurrent[LightControl, Channel] = Value;
                    mChannel = Channel + 1;

                    m_sendData = "[" + mChannel.ToString("D2") + Value.ToString("D3");
                    Light.Write(LightControl, m_sendData);
                }
                catch
                {

                }
            }
#endregion

#region NOT USE
            public void BlobGetVertexs(CogBlobTool BLOBTOOL, ref double[,] nVertexs)
            {
                int[,] VertexArray = new int[4, 2];
                double[] VertexValue = new double[4];
                double[,] VertexResults = new double[4, 2];

                int POS_I = 0;
                int POS_J = 1;

                int POS_X = 0;
                int POS_Y = 1;

                int MIN_X = 0;
                int MAX_X = 1;

                int MIN_Y = 2;
                int MAX_Y = 3;



                try
                {
                    if (BLOBTOOL.Results.GetBlobs().Count > 0)
                    {
                        for (int i = 0; i < BLOBTOOL.Results.GetBlobs().Count; i++)
                        {
                            for (int j = 0; j < BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertices().Length / 2; j++)
                            {
                                if (i == 0 && j == 0)
                                {
                                    VertexValue[MIN_X] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                    VertexValue[MAX_X] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                    VertexValue[MIN_Y] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                    VertexValue[MAX_Y] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                }
                                if (BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexX(j) < VertexValue[MIN_X])
                                {
                                    VertexValue[MIN_X] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                    VertexArray[MIN_X, POS_I] = i;
                                    VertexArray[MIN_X, POS_J] = j;
                                }
                                if (BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexX(j) > VertexValue[MAX_X])
                                {
                                    VertexValue[MAX_X] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexX(j);
                                    VertexArray[MAX_X, POS_I] = i;
                                    VertexArray[MAX_X, POS_J] = j;
                                }

                                if (BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexY(j) < VertexValue[MIN_Y])
                                {
                                    VertexValue[MIN_Y] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                    VertexArray[MIN_Y, POS_I] = i;
                                    VertexArray[MIN_Y, POS_J] = j;
                                }
                                if (BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexY(j) > VertexValue[MAX_Y])
                                {
                                    VertexValue[MAX_Y] = BLOBTOOL.Results.GetBlobs()[i].GetBoundary().GetVertexY(j);
                                    VertexArray[MAX_Y, POS_I] = i;
                                    VertexArray[MAX_Y, POS_J] = j;
                                }
                            }
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            VertexResults[i, POS_X] = BLOBTOOL.Results.GetBlobs()[VertexArray[i, POS_I]].GetBoundary().GetVertexX(VertexArray[i, POS_J]);
                            VertexResults[i, POS_Y] = BLOBTOOL.Results.GetBlobs()[VertexArray[i, POS_I]].GetBoundary().GetVertexY(VertexArray[i, POS_J]);
                        }
                    }
                }
                catch
                {

                }
                nVertexs = VertexResults;
            }
            private bool PMAlignTool(Type nPMToolBlock)
            {
                if (nPMToolBlock.Name.ToString() == "CogPMAlignTool")
                    return true;
                else
                    return false;
            }
            public void CrossLine(CogDisplay nDisplay, int m_CamNo)
            {
                CogLine mCogLine1 = new CogLine();
                CogLine mCogLine2 = new CogLine();
                mCogLine1.Color = CogColorConstants.Purple;
                mCogLine2.Color = CogColorConstants.Purple;
                mCogLine1.SetFromStartXYEndXY(0, (double)Main.vision.IMAGE_CENTER_Y[m_CamNo], (double)Main.vision.IMAGE_SIZE_X[m_CamNo], (double)Main.vision.IMAGE_CENTER_Y[m_CamNo]);
                nDisplay.StaticGraphics.Add(mCogLine1 as ICogGraphic, "Find MarkerPos");
                mCogLine2.SetFromStartXYEndXY((double)Main.vision.IMAGE_CENTER_X[m_CamNo], 0, (double)Main.vision.IMAGE_CENTER_X[m_CamNo], (double)Main.vision.IMAGE_SIZE_Y[m_CamNo]);
                nDisplay.StaticGraphics.Add(mCogLine2 as ICogGraphic, "Find MarkerPos");
            }
#endregion

        }// PatternTag

    }
}
