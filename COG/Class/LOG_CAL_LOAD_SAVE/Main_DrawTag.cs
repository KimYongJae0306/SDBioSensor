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
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.CNLSearch;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.Dimensioning;
using Cognex.VisionPro.LineMax;

namespace COG
{
    public partial class Main
    {
        delegate void dGrabRefresh(Cognex.VisionPro.Display.CogDisplay CogDisplay);
        public static void DisplayFit(Cognex.VisionPro.Display.CogDisplay CogDisplay)
        {

            if (CogDisplay.InvokeRequired)
            {
                dGrabRefresh call = new dGrabRefresh(DisplayFit);
                CogDisplay.Invoke(call, CogDisplay);
            }
            else
            {
                CogDisplay.Fit(true);
                CogDisplay.AutoFitWithGraphics = true;
                CogDisplay.Fit(false);
               
                
//                 switch (CogDisplay.Margin.All)
//                 {
//                     case Main.DEFINE.M_CENTER:
// 
//                         break;
//                     case Main.DEFINE.M_LEFT:
//                         CogDisplay.PanX = -(CogDisplay.Size.Width / CogDisplay.Zoom) - CogDisplay.PanXMin;
//                         CogDisplay.PanY = 0;
//                         break;
//                     case Main.DEFINE.M_RIGHT:
//                         CogDisplay.PanX = (CogDisplay.Size.Width / CogDisplay.Zoom) - CogDisplay.PanXMax;
//                         CogDisplay.PanY = 0;
//                         break;
//                     case Main.DEFINE.M_TOP:
//                         CogDisplay.PanX = 0;
//                         CogDisplay.PanY = -(CogDisplay.Size.Height / CogDisplay.Zoom) - CogDisplay.PanYMin;
//                         break;
//                     case Main.DEFINE.M_BOTTOM:
//                         CogDisplay.PanX = 0;
//                         CogDisplay.PanY = (CogDisplay.Size.Height / CogDisplay.Zoom) - CogDisplay.PanYMax;
//                         break;
//                 }

            }
        }
        public static void DisplayClear(Cognex.VisionPro.Display.CogDisplay CogDisplay)
        {
            if (CogDisplay.InvokeRequired)
            {
                dGrabRefresh call = new dGrabRefresh(DisplayClear);
                CogDisplay.Invoke(call, CogDisplay);
            }
            else
            {
                CogDisplay.StaticGraphics.Clear();
                CogDisplay.InteractiveGraphics.Clear();
            }
        }
        public static void DisplayRefresh(Cognex.VisionPro.Display.CogDisplay CogDisplay)
        {
            CogDisplay.DrawingEnabled = false;
            CogDisplay.Fit(true);
            CogDisplay.DrawingEnabled = true;
            
        }


        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Mark Overlay관련


        public static void DrawOverlayManual_Match(CogRecordDisplay Display, PMResult PMResult0)
        {
     //       DisplayClear(Display);
            CogPointMarker MarkPoint = new CogPointMarker();
            MarkPoint.GraphicDOFEnable = CogPointMarkerDOFConstants.All;
            MarkPoint.SizeInScreenPixels = 80; //화면에 표시 되는 + 모양 크기 . 
            MarkPoint.X = PMResult0.m_Pixel[DEFINE.X];
            MarkPoint.Y = PMResult0.m_Pixel[DEFINE.Y];
            Display.InteractiveGraphics.Add(MarkPoint as ICogGraphicInteractive, "Find MarkerPos", false);

        }
        public static void DrawOverlayPMAlign(CogRecordDisplay Display, PMResult PMResult0)
        {
            List<PMResult> PMAlignResult_List = new List<PMResult>();
            PMAlignResult_List.Add(PMResult0);
            DrawOverlayPMAlign(Display, PMAlignResult_List);
        }
        public static void DrawOverlayPMAlign(CogRecordDisplay Display, PMResult[] PMResult0)
        {
            List<PMResult> PMAlignResult_List = new List<PMResult>();
            for (int i = 0; i < PMResult0.Length; i++)
                PMAlignResult_List.Add(PMResult0[i]);
            DrawOverlayPMAlign(Display, PMAlignResult_List);
          //  DrawOverlayTrayPMAlign(Display, PMAlignResult_List);
        }
        public static void DrawOverlayPMAlign(CogRecordDisplay Display, PMResult PMResult0, PMResult PMResult1)
        {
            List<PMResult> PMAlignResult_List = new List<PMResult>();
            PMAlignResult_List.Add(PMResult0);
            PMAlignResult_List.Add(PMResult1);
            DrawOverlayPMAlign(Display, PMAlignResult_List);
        }
        public static void DrawOverlayPMAlign(CogRecordDisplay Display, PMResult PMResult0, PMResult PMResult1, PMResult PMResult2, PMResult PMResult3)
        {
            List<PMResult> PMAlignResult_List = new List<PMResult>();
            PMAlignResult_List.Add(PMResult0);
            PMAlignResult_List.Add(PMResult1);
            PMAlignResult_List.Add(PMResult2);
            PMAlignResult_List.Add(PMResult3);
            DrawOverlayPMAlign(Display, PMAlignResult_List);
        }
 
        public static void DrawOverlayPMAlign(CogRecordDisplay Display, List<PMResult> PMAlignResult)
        {
            try
            {
                List<CogCompositeShape> ResultGraphic = new List<CogCompositeShape>();
                List<CogRectangle> ResultSearchRegion = new List<CogRectangle>();
                List<string> nMessageList = new List<string>();
                List<CogColorConstants> nColorList = new List<CogColorConstants>();

                List<double> nPosXs = new List<double>();
                List<double> nPosYs = new List<double>();

                double nPosX = 0;
                double nPosY = 0;

                for (int i = 0; i < PMAlignResult.Count; i++)
                {
                    if (PMAlignResult[i].ManuMathchFlag)
                    {
                        DrawOverlayManual_Match(Display, PMAlignResult[i]);
                    }
                    else
                    {
                        if (PMAlignResult[i].CNLSearchResult != null || PMAlignResult[i].PMAlignResult != null)
                        {
                            //-------------------------------------------------------------------------------------------------------------------------------------------
                            bool nRetSearch_CNL = false;
                            bool nRetSearch_PMA = false;
                            if (PMAlignResult[i].CNLSearchResult != null)
                            {
                                if (PMAlignResult[i].CNLSearchResult.Count >= 1) nRetSearch_CNL = true;
                            }
                            if (PMAlignResult[i].PMAlignResult != null)
                            {
                                if (PMAlignResult[i].PMAlignResult.Count >= 1) nRetSearch_PMA = true;
                            }
                            //if (PMAlignResult[i].CNLSearchResult.Count >= 1 || PMAlignResult[i].PMAlignResult.Count >= 1)
                            if (nRetSearch_CNL || nRetSearch_PMA)
                            {
                                string[] nMessage = new string[2];
                                CogColorConstants[] nColor = new CogColorConstants[2];

                                if (nRetSearch_CNL)
                                {
                                    if (PMAlignResult[i].CNLSearchResult.Count >= 1)
                                    {
                                        if (Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName == "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName == "FPC_TRAY2")
                                            nMessage[0] = "P" + (i + 1).ToString() + "-> " + (PMAlignResult[i].CNLSearchResult[0].Score * 100).ToString("0.00");
                                        else
                                            nMessage[0] = " SCORE: " + (PMAlignResult[i].CNLSearchResult[0].Score * 100).ToString("0.00");

                                        if (PMAlignResult[i].CNLSearchResult[0].Score >= PMAlignResult[i].m_ACCeptScore)
                                        {
                                            nMessage[0] += " OK";
                                            nColor[0] = CogColorConstants.Green;
                                        }
                                        else
                                        {
                                            nMessage[0] += " NG";
                                            nColor[0] = CogColorConstants.Red;
                                        }

                                        if (Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName != "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName != "FPC_TRAY2")
                                        {
                                            nMessage[0] += " PIXEL: " + PMAlignResult[i].m_Pixel[DEFINE.X].ToString("0.000") + ", " + PMAlignResult[i].m_Pixel[DEFINE.Y].ToString("0.000");
                                            nMessage[0] += " -> " + PMAlignResult[i].m_RobotPosX.ToString("0.000") + ", " + PMAlignResult[i].m_RobotPosY.ToString("0.000");
                                        }
                                        nMessageList.Add(nMessage[0]);
                                        nColorList.Add(nColor[0]);

                                        if (PMAlignResult[i].SearchRegion != null)
                                        {
                                            nPosXs.Add(PMAlignResult[i].SearchRegion.X);
                                            nPosYs.Add(PMAlignResult[i].SearchRegion.Y);
                                        }
                                    }
                                }

                                if (nRetSearch_PMA)
                                {
                                    if (PMAlignResult[i].PMAlignResult.Count >= 1)
                                    {
                                        if (Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName == "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName == "FPC_TRAY2")
                                            nMessage[1] = "P" + (i + 1).ToString() + "-> " + (PMAlignResult[i].PMAlignResult[0].Score * 100).ToString("0.00");
                                        else
                                            nMessage[1] = "GSCORE: " + (PMAlignResult[i].PMAlignResult[0].Score * 100).ToString("0.00");

                                        if (PMAlignResult[i].PMAlignResult[0].Score >= PMAlignResult[i].m_GACCeptScore)
                                        {
                                            nMessage[1] += " OK";
                                            nColor[1] = CogColorConstants.Green;
                                        }
                                        else
                                        {
                                            nMessage[1] += " NG";
                                            nColor[1] = CogColorConstants.Red;
                                        }

                                        if (Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName != "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[i].m_Align_No].m_AlignName != "FPC_TRAY2")
                                        {
                                            nMessage[1] += " PIXEL: " + PMAlignResult[i].m_Pixel[DEFINE.X].ToString("0.000") + ", " + PMAlignResult[i].m_Pixel[DEFINE.Y].ToString("0.000");
                                            nMessage[1] += " -> " + PMAlignResult[i].m_RobotPosX.ToString("0.000") + ", " + PMAlignResult[i].m_RobotPosY.ToString("0.000");
                                        }
                                        nMessageList.Add(nMessage[1]);
                                        nColorList.Add(nColor[1]);

                                        if (PMAlignResult[i].SearchRegion != null)
                                        {
                                            nPosXs.Add(PMAlignResult[i].SearchRegion.X);
                                            nPosYs.Add(PMAlignResult[i].SearchRegion.Y + 20);
                                        }
                                    }
                                }

                                if (PMAlignResult[i].m_PMAlign)
                                {
                                    if (!PMAlignResult[i].SearchResult || nRetSearch_CNL)
                                    {
                                        if (PMAlignResult[i].CNLSearchResult.Count >= 1)
                                        {
                                            for (int j = 0; j < PMAlignResult[i].CNLSearchResult.Count; j++)
                                            {
                                                if (PMAlignResult[i].CNLSearchResult[j] != null)
                                                {
                                                    ResultGraphic.Add(PMAlignResult[i].CNLSearchResult[j].CreateResultGraphics(Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.MatchRegion | Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.Origin));
                                                }
                                            }
                                        }
                                    }
                                    if (nRetSearch_PMA)
                                    {
                                        if (PMAlignResult[i].PMAlignResult.Count >= 1 && PMAlignResult[i].m_PMAlign_Use)
                                        {

                                            for (int j = 0; j < PMAlignResult[i].PMAlignResult.Count; j++)
                                            {
                                                if (PMAlignResult[i].PMAlignResult[j] != null)
                                                {
                                                    ResultGraphic.Add(PMAlignResult[i].PMAlignResult[j].CreateResultGraphics(CogPMAlignResultGraphicConstants.MatchRegion | CogPMAlignResultGraphicConstants.MatchFeatures | CogPMAlignResultGraphicConstants.Origin));
                                                }
                                            }
                                            //                                         if (PMAlignResult[i].PMAlignResult[0] != null)
                                            //                                             ResultGraphic.Add(PMAlignResult[i].PMAlignResult[0].CreateResultGraphics(CogPMAlignResultGraphicConstants.MatchRegion | CogPMAlignResultGraphicConstants.MatchFeatures | CogPMAlignResultGraphicConstants.Origin));
                                        }
                                    }
                                }
                                else    // not if (PMAlignResult[i].m_PMAlign)
                                {
                                    if (nRetSearch_CNL)
                                    {
                                        if (PMAlignResult[i].CNLSearchResult.Count >= 1)
                                        {
                                            for (int j = 0; j < PMAlignResult[i].CNLSearchResult.Count; j++)
                                            {
                                                if (PMAlignResult[i].CNLSearchResult[j] != null)
                                                {
                                                    ResultGraphic.Add(PMAlignResult[i].CNLSearchResult[j].CreateResultGraphics(Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.MatchRegion | Cognex.VisionPro.SearchMax.CogSearchMaxResultGraphicConstants.Origin));
                                                }
                                            }

                                        }
                                    }

                                }

                                if (PMAlignResult[i].SearchResult)
                                {
                                    nPosX = Main.AlignUnit[PMAlignResult[i].m_Align_No].PAT[PMAlignResult[i].m_PatTagNo, PMAlignResult[i].m_PatNo].FixtureTrans.TranslationX;
                                    nPosY = Main.AlignUnit[PMAlignResult[i].m_Align_No].PAT[PMAlignResult[i].m_PatTagNo, PMAlignResult[i].m_PatNo].FixtureTrans.TranslationY;
                                }


                                if (!Main.ALIGNINSPECTION_USE(PMAlignResult[i].m_Align_No))
                                    CrossLine(Display, PMAlignResult[i].m_CamNo, nPosX, nPosY);

                                if (PMAlignResult[i].SearchRegion != null)
                                {
                                    PMAlignResult[i].SearchRegion.X -= nPosX;
                                    PMAlignResult[i].SearchRegion.Y -= nPosY;
                                    PMAlignResult[i].SearchRegion.Color = CogColorConstants.Purple;
                                    ResultSearchRegion.Add(PMAlignResult[i].SearchRegion);
                                }

                            }
                            else    // not if (nRetSearch_CNL || nRetSearch_PMA)
                            {
                                string[] nMessage1 = new string[1];
                                nMessage1[0] = "Mark NG";
                                if (nRetSearch_PMA)
                                {
                                    if (Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName != "FBD_MONITORING" || Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName != "FOB_INSPECT" || Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName != "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName != "FPC_TRAY2 ")
                                        DrawOverlayMessage(Display, nMessage1);
                                }
                                else
                                {
                                    nMessage1[0] = "Grab!";
                                    DrawOverlayMessage(Display, nMessage1);
                                }
                            }
                        }
                        else    // not if (PMAlignResult[i].CNLSearchResult != null || PMAlignResult[i].PMAlignResult != null)
                        {
                            string[] nMessage1 = new string[1];
                            if (Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName == "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName == "FPC_TRAY2")
                            {
                                nMessage1[0] = "P" + i.ToString() + "->  " + (PMAlignResult[i].Score * 100).ToString("0.00") + " NG";
                                nMessageList.Add(nMessage1[0]);
                                ResultSearchRegion.Add(PMAlignResult[i].SearchRegion);
                                nColorList.Add(CogColorConstants.Red);

                                //                   nPosXs.Add(PMAlignResult[i].m_TrayPocketPos.X);
                                //                    nPosYs.Add(PMAlignResult[i].m_TrayPocketPos.Y);
                                if (PMAlignResult[i].SearchRegion != null)
                                {
                                    nPosXs.Add(PMAlignResult[i].SearchRegion.X);
                                    nPosYs.Add(PMAlignResult[i].SearchRegion.Y);
                                }
                            }
                            else
                            {
                                nMessage1[0] = "Mark NG";
                                DrawOverlayMessage(Display, nMessage1);
                            }
                        }
                    }



                }

                if (Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName == "FPC_TRAY1" || Main.AlignUnit[PMAlignResult[0].m_Align_No].m_AlignName == "FPC_TRAY2")
                    DrawOverlayMessage(Display, nMessageList, nColorList, nPosXs, nPosYs);
                else
                    DrawOverlayMessage(Display, nMessageList, nColorList, nPosX, nPosY, false);


                //-------------------------------------------------------------------------------------------------------------------------------------------
                for (int i = 0; i < ResultGraphic.Count; i++)
                {
                    Display.StaticGraphics.Add(ResultGraphic[i] as ICogGraphic, "Mark");
                }
                for (int i = 0; i < ResultSearchRegion.Count; i++)
                {
                    Display.StaticGraphics.Add(ResultSearchRegion[i] as ICogGraphic, "Search Region");
                }
            }
            catch
            {

            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Blob Overlay관련
        public static void DrawOverlayTrayBlobTool(CogRecordDisplay Display, List<TrayBlobResult> PMBlobResults)
        {
            List<BlobResult> BLOBList = new List<BlobResult>();
            for (int i = 0; i < PMBlobResults.Count; i++)
            {
                if (PMBlobResults[i].TrayBlob.m_UseCheck == true)
                {
                    BLOBList.Add(PMBlobResults[i].TrayBlob);
                }
            }
            if (BLOBList.Count > 0) DrawOverlayBlobTool(Display, BLOBList);
        }
        public static void DrawOverlayTrayBlobTool(CogRecordDisplay Display, TrayBlobResult PMBlobResults)
        {
            List<BlobResult> BLOBList = new List<BlobResult>();
             BLOBList.Add(PMBlobResults.TrayBlob);
            if (BLOBList.Count > 0) DrawOverlayBlobTool(Display, BLOBList);
        }
        public static void DrawOverlayBlobTool(CogRecordDisplay Display, BlobResult[] PMBlobResults)
        {
            List<BlobResult> BLOBList = new List<BlobResult>();
            for (int i = 0; i < PMBlobResults.Length; i++)
            {
                if (PMBlobResults[i].m_UseCheck == true)
                {
                    BLOBList.Add(PMBlobResults[i]);
                }
            }
            if (BLOBList.Count > 0) DrawOverlayBlobTool(Display, BLOBList);
        }

        public static void DrawOverlayBlobTool(CogRecordDisplay Display, BlobResult[] PMBlobResults, FindLineResult[] PMResult0)
        {
            List<BlobResult> BLOBList = new List<BlobResult>();
            for (int i = 0; i < PMBlobResults.Length; i++)
            {
                if (PMBlobResults[i].m_UseCheck == true)
                {
                    BLOBList.Add(PMBlobResults[i]);
                }
            }
            List<FindLineResult> PMFindLineResult_List = new List<FindLineResult>();
            for (int i = 0; i < PMResult0.Length; i++)
            {
                if (PMResult0[i] != null) PMFindLineResult_List.Add(PMResult0[i]);
            }
        }
        public static void DrawOverlayBlobTool(CogRecordDisplay Display, List<BlobResult> PMBlobResults)
        {
            try
            {
                List<CogPolygon> ResultBoundary = new List<CogPolygon>();   // BLOB 찾은결과 경계 영역
                CogRectangleAffine[] SearchRegion = new CogRectangleAffine[PMBlobResults.Count]; //BLOB 설정영역
                CogGraphicLabel[] Label = new CogGraphicLabel[PMBlobResults.Count]; //Results Text 

                double[] BlobArea = new double[PMBlobResults.Count];
                double Score = new double();
                bool AOI_Flag = false;

                if (ALIGNINSPECTION_USE(PMBlobResults[0].m_AlignNum)) AOI_Flag = true;

                float nFontSize = 0;
                nFontSize = (float)((Display.Image.Height / Main.DEFINE.FontSize) * Display.Zoom);


              



//                 double nFontpitch = 0;
//                 nFontpitch = (nFontSize + (nFontSize / Display.Zoom));


                for (int i = 0; i < PMBlobResults.Count; i++)
                {
                     Label[i] = new CogGraphicLabel();
                    Label[i].Font = new Font(Main.DEFINE.FontStyle, nFontSize); //-> Display.Zoom 화면의 크기에 따라서 글자크기 ..

                    SearchRegion[i] = new CogRectangleAffine(PMBlobResults[i].SearchRegion);      //------------------------BLOB SEARCH AREA 

                    if (PMBlobResults[i].BlobToolResult != null)
                    {

                       bool ret_NG = false;

                        if(PMBlobResults[i].BlobToolResult.GetBlobs().Count > 0)
                            ret_NG = true;

                       if(PMBlobResults[i].m_AlignName == "FPC_TRAY1" ||PMBlobResults[i].m_AlignName == "FPC_TRAY2")
                       {
                           nFontSize = (float)((Display.Image.Height / 80.0f) * Display.Zoom);
                           Label[i].Font = new Font(Main.DEFINE.FontStyle, nFontSize);

                        
                           if (PMBlobResults[i].BlobToolResult.GetBlobs().Count <= 0)
                               ret_NG = true;
                           else
                               ret_NG = false;
                       }
                       if (ret_NG)
                        {
                            int GetBlobsCount = 1;

                            if (!AOI_Flag || (AOI_Flag && Main.Status.MC_MODE == Main.DEFINE.MC_TEACHFORM))
                            {
                                GetBlobsCount = PMBlobResults[i].BlobToolResult.GetBlobs().Count;
                            }
                            for (int j = 0; j < GetBlobsCount; j++) //PMBlobResults[i].BlobToolResult.GetBlobs().Count
                            {
                                ResultBoundary.Add(PMBlobResults[i].BlobToolResult.GetBlobs()[j].GetBoundary());  //--BLOB RESULTS BOUNDARY
                                BlobArea[i] += PMBlobResults[i].BlobToolResult.GetBlobs()[j].Area;
                            }
                            if (Main.BLOBINSPECTION_USE(PMBlobResults[0].m_AlignNum) && !AOI_Flag && i < Main.DEFINE.BLOB_INSP_LIMIT_CNT)
                            {
                                Label[i].Text = PMBlobResults[i].m_Index.ToString();
                                Label[i].Color = CogColorConstants.Blue;
                            }
                            else
                            {
                                Score = 100 - ((BlobArea[i] / SearchRegion[i].Area) * 100);
                                Label[i].Text = PMBlobResults[i].m_Index.ToString() + ":NG " + Score.ToString("0.0") + "%";
                                Label[i].Color = CogColorConstants.Red;
                            }
                        }
                        else
                        {
                            if (PMBlobResults[i].m_AlignName == "FPC_TRAY1" || PMBlobResults[i].m_AlignName == "FPC_TRAY2")
                            {
                                try
                                {
                                    for (int j = 0; j < PMBlobResults[i].BlobToolResult.GetBlobs().Count; j++) //PMBlobResults[i].BlobToolResult.GetBlobs().Count
                                    {
                                        ResultBoundary.Add(PMBlobResults[i].BlobToolResult.GetBlobs()[j].GetBoundary());  //--BLOB RESULTS BOUNDARY
                                        BlobArea[i] += PMBlobResults[i].BlobToolResult.GetBlobs()[j].Area;
                                    }
                                }
                                catch
                                {

                                }
                            }

                            Score = 100;
                            Label[i].Text = PMBlobResults[i].m_Index.ToString() + ":OK " + Score.ToString("0.0") + "%";
                            Label[i].Color = CogColorConstants.Green;
                        }
                    }
                }


               

                //-----------------------SEARCH AREA--------------------------------------------
                for (int i = 0; i < SearchRegion.Length; i++)
                {
                    SearchRegion[i].Color = CogColorConstants.Yellow;
                    Display.StaticGraphics.Add(SearchRegion[i] as ICogGraphic, "SEARCH AREA");
                }
                //-----------------------RESULT TEXT--------------------------------------------
                for (int i = 0; i < Label.Length; i++)
                {
                    Label[i].X = SearchRegion[i].CornerOriginX + 30;
                    Label[i].Y = SearchRegion[i].CornerOriginY - 20;


                    if (!AOI_Flag) Display.StaticGraphics.Add(Label[i] as ICogGraphic, "Result Text");
                }
                    //-------------------------------------------------------------------------------
                //-------------------------------------------------------------------------------
                //------------------------BLOB RESULT BOUNDARY-----------------------------------
                for (int i = 0; i < ResultBoundary.Count; i++)
                {
                    if (!AOI_Flag) 
                    ResultBoundary[i].Color = CogColorConstants.Red;
                    Display.StaticGraphics.Add(ResultBoundary[i] as ICogGraphic, "BLOB");
                }
                //-------------------------------------------------------------------------------
                if (Main.BLOBINSPECTION_USE(PMBlobResults[0].m_AlignNum))
                {
                    int POS_X = 0, POS_Y = 1;
                    int MIN_Y = 2, MAX_Y = 3;
                    int BlobCnt = 0, IncCnt = 0;
                    BlobCnt = Main.AlignUnit[PMBlobResults[0].m_AlignNum].PAT[Main.AlignUnit[PMBlobResults[0].m_AlignNum].m_PatTagNo, PMBlobResults[0].m_AlignNum].m_Blob_InspCnt;
                    IncCnt = 2;
                    CogPointMarker MarkPoint = new CogPointMarker();
                    MarkPoint.GraphicDOFEnable = CogPointMarkerDOFConstants.All;
                    MarkPoint.Color = CogColorConstants.Purple; // CogColorConstants.Yellow;
                    MarkPoint.SizeInScreenPixels = 50;

                    if (Main.Status.MC_STATUS == Main.DEFINE.MC_RUN)
                    {
                        for (int i = 0; i < BlobCnt; i++) //Auto
                        {
                            MarkPoint.X = PMBlobResults[i * IncCnt].VertexResults[MIN_Y, POS_X];
                            MarkPoint.Y = PMBlobResults[i * IncCnt].VertexResults[MIN_Y, POS_Y];
                            Display.StaticGraphics.Add(MarkPoint as ICogGraphic, "Search Region");
                      //    MarkPoint.X = PMBlobResults[i * IncCnt + 1].VertexResults[MAX_Y, POS_X];
                            MarkPoint.X = PMBlobResults[i * IncCnt].VertexResults[MIN_Y, POS_X];    //X위치를 맞추어서 뿌리기 위해서 MIN_Y의 X값을 넣어줌. 
                            MarkPoint.Y = PMBlobResults[i * IncCnt + 1].VertexResults[MAX_Y, POS_Y];
                            Display.StaticGraphics.Add(MarkPoint as ICogGraphic, "Search Region");
                        }
                    }
                 }
            }
            catch
            {
            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Caliper Overlay관련
        public static void DrawOverlayCaliper(CogRecordDisplay Display, CaliperResult[] PMResult0)
        {
            List<CaliperResult> PMCaliperResult_List = new List<CaliperResult>();
            for (int i = 0; i < PMResult0.Length; i++)
            {
                if (PMResult0[i] != null && PMResult0[i].m_UseCheck == true) 
                    PMCaliperResult_List.Add(PMResult0[i]);
            }
            if (PMCaliperResult_List.Count > 0) DrawOverlayCaliper(Display, PMCaliperResult_List);
        }
        public static void DrawOverlayCaliper(CogRecordDisplay Display, List<CaliperResult> PMCaliperResult)
        {
            try
            {
                List<CogCompositeShape> ResultGraphic = new List<CogCompositeShape>();
                List<CogRectangleAffine> ResultSearchRegion = new List<CogRectangleAffine>();
                CogCoordinateAxes PatMaxORGPoint = new CogCoordinateAxes();

//                CogGraphicLabel[] Label = new CogGraphicLabel[PMCaliperResult.Count]; //Results Text 

                float nFontSize = 0;
                nFontSize = (float)((Display.Image.Height / Main.DEFINE.FontSize) * Display.Zoom);

                bool EdgePairMode = false;

                string[] sMsg = new string[DEFINE.CALIPER_MAX + 1];

                for (int i = 0; i < PMCaliperResult.Count; i++)
                {
                    //-------------------------------------------------------------------------------------------------------------------------------------------
                    //                     Label[i] = new CogGraphicLabel();
                    //                     Label[i].Font = new Font(Main.DEFINE.FontStyle, nFontSize); //-> Display.Zoom 화면의 크기에 따라서 글자크기 ..
                    if (PMCaliperResult[i].CaliperTool != null)
                    {
                        if (Main.GetCaliperPairMode(PMCaliperResult[i].CaliperTool.RunParams.EdgeMode)) EdgePairMode = true;
                        if (Main.BLOBINSPECTION_USE(PMCaliperResult[i].m_AlignNum)) EdgePairMode = true;
                        if (PMCaliperResult[i].CaliperTool.Results != null)
                        {
                            if (PMCaliperResult[i].CaliperTool.Results.Count >= 1)
                            {

                                switch (Main.GetCaliperDirection(Main.GetCaliperDirection(PMCaliperResult[i].CaliperTool.Region.Rotation)))
                                {
                                    case Main.DEFINE.X:
                                        /*                                        PatMaxORGPoint.OriginX = PMCaliperResult[i].Pixel[DEFINE.X]*/
                                        ;
                                        PatMaxORGPoint.OriginX = PMCaliperResult[i].CaliperTool.Results[0].Edge0.PositionX;
                                        break;

                                    case Main.DEFINE.Y:
                                        /*                                        PatMaxORGPoint.OriginY = PMCaliperResult[i].Pixel[DEFINE.Y];*/
                                        PatMaxORGPoint.OriginY = PMCaliperResult[i].CaliperTool.Results[0].Edge0.PositionY;
                                        break;

                                }


                                for (int j = 0; j < PMCaliperResult[i].CaliperTool.Results.Count; j++)
                                {
                                    ResultGraphic.Add(PMCaliperResult[i].CaliperTool.Results[j].CreateResultGraphics(CogCaliperResultGraphicConstants.All));
                                }
                            }
                        }
                        else
                        {
                            //서치 영역만 
                        }
                        //                         Label[i].X = PMCaliperResult[i].CaliperTool.Results[0].Edge0.PositionX;
                        //                         Label[i].Y = PMCaliperResult[i].CaliperTool.Results[0].Edge0.PositionY;
                        //                         Label[i].Text = PMCaliperResult[i].m_Index.ToString();
                        //                         Label[i].Color = CogColorConstants.Green;
                        ResultSearchRegion.Add(PMCaliperResult[i].CaliperTool.Region);

                        if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                        {
                            // JHKIM
                            if (PMCaliperResult[i].CaliperTool.Results != null)
                            {
                                if (i > DEFINE.CALIPER_MAX) continue;

                                if (PMCaliperResult[i].SearchResult == true)
                                    sMsg[i + 1] = "OK -> ";
                                else
                                    sMsg[i + 1] = "FL -> ";

                                if (i == 0)
                                    sMsg[i + 1] += "Beam Size Top : " + PMCaliperResult[i].RobotWidth.ToString("0.000");
                                else if (i == 1)
                                    sMsg[i + 1] += "Beam Size Right : " + PMCaliperResult[i].RobotWidth.ToString("0.000");
                                else if (i == 2)
                                    sMsg[i + 1] += "Beam Size Bottom : " + PMCaliperResult[i].RobotWidth.ToString("0.000");
                                else if (i == 3)
                                    sMsg[i + 1] += "Beam Size Left : " + PMCaliperResult[i].RobotWidth.ToString("0.000");
                            }
                        }
                    }
                }

                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                    DrawOverlayMessage(Display, sMsg);

                //-------------------------------------------------------------------------------------------------------------------------------------------
                for (int i = 0; i < ResultGraphic.Count; i++)
                {
                    Display.StaticGraphics.Add(ResultGraphic[i] as ICogGraphic, "Mark");
                }
                for (int i = 0; i < ResultSearchRegion.Count; i++)
                {
                    ResultSearchRegion[i].Color = CogColorConstants.Orange;
                    ResultSearchRegion[i].SelectedColor = CogColorConstants.Orange;
//                     if (!EdgePairMode)
//                         Display.StaticGraphics.Add(ResultSearchRegion[i] as ICogGraphic, "Search Region");
                }
                //-----------------------RESULT TEXT--------------------------------------------
//                 for (int i = 0; i < Label.Length; i++)
//                 {
//                     Display.StaticGraphics.Add(Label[i] as ICogGraphic, "Result Text");
//                 }
                if (ResultGraphic.Count > 1 && !EdgePairMode) 
                    Display.StaticGraphics.Add(PatMaxORGPoint, "Mark");

            }
            catch
            {

            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region FindLine Overlay관련
        public static void DrawOverlayFindLine(CogRecordDisplay Display, FindLineResult[] PMResult0)
        {
            List<FindLineResult> PMFindLineResult_List = new List<FindLineResult>();
            for (int i = 0; i < PMResult0.Length; i++)
            {
                if (PMResult0[i] != null) PMFindLineResult_List.Add(PMResult0[i]);
            }
            if (PMFindLineResult_List.Count > 0) DrawOverlayFindLine(Display, PMFindLineResult_List);
        }
        public static void DrawOverlayFindLine(CogRecordDisplay Display, List<FindLineResult> PMFindLineResult)
        {
            try
            {
                List<CogCompositeShape> ResultGraphic = new List<CogCompositeShape>();
                List<CogRectangle> ResultSearchRegion = new List<CogRectangle>();
                List<CogLine> ResultLine = new List<CogLine>();
                CogIntersectLineLineTool LineLineTool = new CogIntersectLineLineTool();
                List<CogLineSegment> ResultLineSegment = new List<CogLineSegment>();
                List<CogPointMarker> MarkPoint = new List<CogPointMarker>();

                CogGraphicInteractiveCollection resultGraphics = new CogGraphicInteractiveCollection();

                string[] sMsg = new string[4];

                for (int i = 0; i < PMFindLineResult.Count; i++)
                {
                    if (PMFindLineResult[i].m_UsedLineMax)
                    {
                        if (PMFindLineResult[i].LineMaxTool.Results != null)
                        {
                            if (PMFindLineResult[i].LineMaxTool.Results.Count >= 1)
                            {
                                if (i < 3)
                                    ResultLine.Add(PMFindLineResult[i].LineMaxTool.Results[0].GetLine());

                                if (PMFindLineResult[i].m_LineMaxSelIdx < 0 || PMFindLineResult[i].m_LineMaxSelIdx >= PMFindLineResult[i].LineMaxTool.Results.Count)
                                    ResultLineSegment.Add(PMFindLineResult[i].LineMaxTool.Results[0].GetLineSegment());
                                else
                                    ResultLineSegment.Add(PMFindLineResult[i].LineMaxTool.Results[PMFindLineResult[i].m_LineMaxSelIdx].GetLineSegment());

                                for (int j = 0; j < PMFindLineResult[i].LineMaxTool.Results.Count; j++)
                                {
                                    resultGraphics.Add(PMFindLineResult[i].LineMaxTool.Results[j].CreateResultGraphics(CogLineMaxResultGraphicConstants.Inliers));
                                }
                            }
                        }
                    }
                    else
                    {
                        LineLineTool.InputImage = PMFindLineResult[0].FindLineTool.InputImage;
                        if (PMFindLineResult[i].FindLineTool.Results != null)
                        {
                            if (PMFindLineResult[i].FindLineTool.Results.Count >= 1 && PMFindLineResult[i].SearchResult == true)
                            {
                                if (i < 3)
                                {
                                    ResultLine.Add(PMFindLineResult[i].FindLineTool.Results.GetLine());

                                    // Test 필요 (Insp 측정 위치)
                                    //if (Main.CCLink_IsBit(DEFINE.CCLINK_IN_STAGE1_INSPECTION_MODE) && PMFindLineResult[i].SearchResult == true)
                                    //{
                                    //    CogLineSegment tempLineSeg = new CogLineSegment();
                                    //    List<string> strMessage = new List<string>();
                                    //    tempLineSeg.StartX = PMFindLineResult[i].FindLineTool.Results.GetLineSegment().MidpointX;
                                    //    tempLineSeg.StartY = PMFindLineResult[i].FindLineTool.Results.GetLineSegment().MidpointY;
                                    //    tempLineSeg.EndX = AlignUnit[PMFindLineResult[i].m_AlignNum].PAT[PMFindLineResult[i].m_PatTagNum, PMFindLineResult[i].m_PatNum].InspectionPosRobot_X[i];
                                    //    tempLineSeg.EndY = AlignUnit[PMFindLineResult[i].m_AlignNum].PAT[PMFindLineResult[i].m_PatTagNum, PMFindLineResult[i].m_PatNum].InspectionPosRobot_Y[i];
                                    //    ResultLineSegment.Add(tempLineSeg);
                                    //    if (PMFindLineResult[i].m_AlignNum == 0)
                                    //        strMessage.Add(INSP_RESULT.m_dPoint[0].ToString("0.000"));
                                    //    else if (PMFindLineResult[i].m_AlignNum == 1)
                                    //        strMessage.Add(INSP_RESULT.m_dPoint[5].ToString("0.000"));
                                    //    else if (PMFindLineResult[i].m_AlignNum == 2)
                                    //    {
                                    //        if (i == 0)
                                    //            strMessage.Add(INSP_RESULT.m_dPoint[1].ToString("0.000"));
                                    //        else if (i == 1)
                                    //            strMessage.Add(INSP_RESULT.m_dPoint[2].ToString("0.000"));
                                    //        else
                                    //            strMessage.Add(INSP_RESULT.m_dPoint[6].ToString("0.000"));
                                    //    }
                                    //    else if (PMFindLineResult[i].m_AlignNum == 3)
                                    //    {
                                    //        if (i == 0)
                                    //            strMessage.Add(INSP_RESULT.m_dPoint[4].ToString("0.000"));
                                    //        else if (i == 1)
                                    //            strMessage.Add(INSP_RESULT.m_dPoint[3].ToString("0.000"));
                                    //        else
                                    //            strMessage.Add(INSP_RESULT.m_dPoint[7].ToString("0.000"));
                                    //    }
                                    //    DrawOverlayMessage(Display, strMessage, tempLineSeg.EndX, tempLineSeg.EndY);
                                    //}
                                }
                                ResultLineSegment.Add(PMFindLineResult[i].FindLineTool.Results.GetLineSegment());

                                for (int j = 0; j < PMFindLineResult[i].FindLineTool.Results.Count; j++)
                                {
                                    resultGraphics.Add(PMFindLineResult[i].FindLineTool.Results[j].CreateResultGraphics(CogFindLineResultGraphicConstants.DataPoint));
                                }
                            }
                        }
                    }

                    if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                    {
                        // JHKIM
                        if (PMFindLineResult[i].m_bInterSearched == true)
                        {
                            if (i > 2) continue;

                            if (PMFindLineResult[i].m_bInterResult == true)
                                sMsg[i] = "OK -> ";
                            else
                                sMsg[i] = "NG -> ";

                            if (i == 0)
                                sMsg[i] += "X-Y CrossPoint : X " + PMFindLineResult[i].CrossPointList[0].X2.ToString("0.000") + ", Y " + PMFindLineResult[i].CrossPointList[0].Y2.ToString("0.000");
                            else if (i == 1)
                                sMsg[i] += "X-C CrossPoint : X " + PMFindLineResult[i].CrossPointList[0].X2.ToString("0.000") + ", Y " + PMFindLineResult[i].CrossPointList[0].Y2.ToString("0.000");
                            else if (i == 2)
                                sMsg[i] += "Y-C CrossPoint : X " + PMFindLineResult[i].CrossPointList[0].X2.ToString("0.000") + ", Y " + PMFindLineResult[i].CrossPointList[0].Y2.ToString("0.000");
                        }

                        // Glass - POL Distance Display
                        if (i == 3 && PMFindLineResult.Count > 3)
                        {
                            if (PMFindLineResult[i].SearchResult == true)
                            {
                                sMsg[i] = "OK -> ";
                                sMsg[i] += "Glass - POL Dist : " + PMFindLineResult[i].CrossPointList[0].X2.ToString("0.000");
                            }
                        }

                        for (int j = 0; j < PMFindLineResult[i].CrossPointList.Count; j++)
                        {
                            if (PMFindLineResult[i].m_bInterResult == false)
                                continue;

                            CogPointMarker CrossPoint = new CogPointMarker();

                            CrossPoint.X = PMFindLineResult[i].CrossPointList[j].X;
                            CrossPoint.Y = PMFindLineResult[i].CrossPointList[j].Y;

                            CrossPoint.GraphicDOFEnable = CogPointMarkerDOFConstants.All;
                            CrossPoint.SizeInScreenPixels = 80; //화면에 표시 되는 + 모양 크기
                            CrossPoint.Color = CogColorConstants.Green;
                            MarkPoint.Add(CrossPoint);
                        }

                        for (int k = 0; k < MarkPoint.Count; k++)
                            Display.StaticGraphics.Add(MarkPoint[k], "Mark");
                    }
                }

                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                    DrawOverlayMessage(Display, sMsg);
                else
                {
                    for (int i = 0; i < PMFindLineResult[0].CrossPointList.Count; i++)
                    {
                        CogPointMarker CrossPoint = new CogPointMarker();

                        CrossPoint.X = PMFindLineResult[0].CrossPointList[i].X;
                        CrossPoint.Y = PMFindLineResult[0].CrossPointList[i].Y;

                        CrossPoint.GraphicDOFEnable = CogPointMarkerDOFConstants.All;
                        CrossPoint.SizeInScreenPixels = 80; //화면에 표시 되는 + 모양 크기
                        CrossPoint.Color = CogColorConstants.Green;
                        MarkPoint.Add(CrossPoint);
                        Display.StaticGraphics.Add(MarkPoint[i], "Mark");
                    }
                }

                for (int i = 0; i < resultGraphics.Count; i++)
                {
                 //   Display.InteractiveGraphics.AddList(resultGraphics, "RESULT", false);
                }
                for (int i = 0; i < ResultLine.Count; i++)
                {
                    Display.StaticGraphics.Add(ResultLine[i] as ICogGraphic, "Line");
                }
                for (int i = 0; i < ResultLineSegment.Count; i++)
                {
                     ResultLineSegment[i].Color = CogColorConstants.Green;
                     Display.StaticGraphics.Add(ResultLineSegment[i] as ICogGraphic, "Line");
                }
            }
            catch
            {

            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Circle Overlay관련
        public static void DrawOverlayCircle(CogRecordDisplay Display, CircleResult[] PMResult0)
        {
            List<CircleResult> PMResult_List = new List<CircleResult>();
            for (int i = 0; i < PMResult0.Length; i++)
            {
                if (PMResult0[i] != null && PMResult0[i].m_UseCheck == true)
                    PMResult_List.Add(PMResult0[i]);
            }
            if (PMResult_List.Count > 0) DrawOverlayCircle(Display, PMResult_List);
        }
        public static void DrawOverlayCircle(CogRecordDisplay Display, List<CircleResult> PMResult)
        {
            try
            {
                CogGraphicInteractiveCollection ResultGraphic = new CogGraphicInteractiveCollection();
                CogColorConstants[] nTempColor = new CogColorConstants[2];
                nTempColor[0] = CogColorConstants.Purple;
                nTempColor[1] = CogColorConstants.Green;
                string[] sMsg = new string[2];
                for (int i = 0; i < PMResult.Count; i++)
                {
                    //-------------------------------------------------------------------------------------------------------------------------------------------
                    if (PMResult[i].CircleTool != null)
                    {
                        if (PMResult[i].CircleTool.Results != null)
                        {
                            if (PMResult[i].CircleTool.Results.Count >= 1)
                            {

                                for (int j = 0; j < PMResult[i].CircleTool.Results.Count; j++)
                                {
                                    ResultGraphic.Add(PMResult[i].CircleTool.Results[j].CreateResultGraphics(/*CogFindCircleResultGraphicConstants.CaliperEdge | */CogFindCircleResultGraphicConstants.DataPoint));
                                }

                                CogCircle tempCircle = PMResult[i].CircleTool.Results.GetCircle();
                                ResultGraphic.Add(tempCircle);

                                CogPointMarker nCircleCenter = new CogPointMarker();
                                if (i < 2) nCircleCenter.Color = nTempColor[i];
                                int nCrossSize = 0;
                                nCrossSize = (int)(PMResult[i].CircleTool.Results.GetCircle().Radius);
                                nCircleCenter.SetCenterRotationSize(PMResult[i].CircleTool.Results.GetCircle().CenterX, PMResult[i].CircleTool.Results.GetCircle().CenterY, 0, nCrossSize);
                                ResultGraphic.Add(nCircleCenter);

                                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
                                {
                                    // JHKIM
                                    if (i != 0) continue;

                                    if (PMResult[i].SearchResult == true)
                                        sMsg[1] = "OK -> ";
                                    else
                                        sMsg[1] = "NG -> ";

                                    sMsg[1] += "Round Center : X " + PMResult[0].m_RobotPosX.ToString("0.000") + ", Y " + PMResult[0].m_RobotPosY.ToString("0.000") + ", R " + PMResult[0].m_RobotRadius.ToString("0.000");

                                    DrawOverlayMessage(Display, sMsg);
                                }
                            }
                        }
                        else
                        {
                            //서치 영역만 
                        }
                    }

                }
                //-------------------------------------------------------------------------------------------------------------------------------------------
                Display.InteractiveGraphics.AddList(ResultGraphic, "RESULT", false);

            }
            catch
            {

            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region OverlayMessage
        public static void DrawOverlayMessage(CogRecordDisplay nDisplay, string [] nMessage)
        {
            try
            {
                
                CogGraphicLabel[] Label = new CogGraphicLabel[nMessage.Length]; //Results Text 
                float nFontSize = 0;
                double nFontpitch = 0;
                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
                {
                    nFontSize = (float)((nDisplay.Height / Main.DEFINE.FontSize));
                    nFontpitch = (nFontSize * 4);
                }
                else
                {
                    nFontSize = (float)((nDisplay.Image.Height / Main.DEFINE.FontSize) * nDisplay.Zoom);
                    nFontpitch = (nFontSize + (nFontSize / nDisplay.Zoom));
                }

                for(int i = 0; i < nMessage.Length; i++)
                {
                    Label[i] = new CogGraphicLabel();
                    Label[i].Font = new Font(Main.DEFINE.FontStyle, nFontSize, FontStyle.Bold); 
                    Label[i].Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
                    Label[i].GraphicDOFEnable = CogGraphicLabelDOFConstants.All;
                    Label[i].Text = nMessage[i];
                    Label[i].Color = CogColorConstants.Magenta;
                }
                //-----------------------RESULT TEXT--------------------------------------------
                for (int i = 0; i < Label.Length; i++)
                {
                    Label[i].X = 0;
                    Label[i].Y = (i * nFontpitch);
                    nDisplay.StaticGraphics.Add(Label[i] as ICogGraphic, "Result Text");
                    //nDisplay.InteractiveGraphics.Add(Label[i] as ICogGraphicInteractive, "Result Text", true);
                }
            }
            catch (System.Exception ex)
            {
            }
        }
        public static void DrawOverlayMessage(CogRecordDisplay nDisplay, List<string> nMessage, double nPosX, double nPosY)
        {
            try
            {
                List<string> nListMessage = new List<string>();
                List<CogColorConstants> nColors = new List<CogColorConstants>();
                CogColorConstants nColor = new CogColorConstants();
                nColor = CogColorConstants.Magenta;
                for (int i = 0; i < nMessage.Count; i++)
                {
                    nListMessage.Add(nMessage[i]);
                    nColors.Add(nColor);
                }
                DrawOverlayMessage(nDisplay, nListMessage, nColors, nPosX, nPosY , true);
            }
            catch (System.Exception ex)
            {
            }
        }
        public static void DrawOverlayMessage(CogRecordDisplay nDisplay, List<string> nMessage, List<CogColorConstants> nColor, List<double> nPosX, List<double> nPosY)
        {
            try
            {

                CogGraphicLabel[] Label = new CogGraphicLabel[nMessage.Count]; //Results Text 
                float nFontSize = 0;
                nFontSize = (float)((nDisplay.Image.Height / Main.DEFINE.FontSize) * nDisplay.Zoom);
                nFontSize /= 2;
                double nFontpitch = 0;
                nFontpitch = (nFontSize + (nFontSize / nDisplay.Zoom));

                for (int i = 0; i < nMessage.Count; i++)
                {
                    Label[i] = new CogGraphicLabel();
                    Label[i].Font = new Font(Main.DEFINE.FontStyle, nFontSize, FontStyle.Bold);
                    Label[i].Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
                    Label[i].Text = nMessage[i];
                    Label[i].Color = nColor[i];
                }
                //-----------------------RESULT TEXT--------------------------------------------
                for (int i = 0; i < Label.Length; i++)
                {
                    Label[i].X = nPosX[i];
                    Label[i].Y = nPosY[i];
                    nDisplay.StaticGraphics.Add(Label[i] as ICogGraphic, "Result Text");
                    //nDisplay.InteractiveGraphics.Add(Label[i] as ICogGraphicInteractive, "Result Text", true);

                }
            }
            catch (System.Exception ex)
            {
            }
        }
        public static void DrawOverlayMessage(CogRecordDisplay nDisplay, List<string> nMessage, List<CogColorConstants> nColor, double nPosX, double nPosY , bool nFlag)
        {
            try
            {

                CogGraphicLabel[] Label = new CogGraphicLabel[nMessage.Count]; //Results Text 
                float nFontSize = 0;
                nFontSize = (float)((nDisplay.Image.Height / Main.DEFINE.FontSize) * nDisplay.Zoom);
                double nFontpitch = 0;
                nFontpitch = (nFontSize + (nFontSize / nDisplay.Zoom));

                for (int i = 0; i < nMessage.Count; i++)
                {
                    Label[i] = new CogGraphicLabel();
                    Label[i].Font = new Font(Main.DEFINE.FontStyle, nFontSize, FontStyle.Bold);
                    Label[i].Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
                    Label[i].Text = nMessage[i];
                    Label[i].Color = nColor[i];
                }
                //-----------------------RESULT TEXT--------------------------------------------
                for (int i = 0; i < Label.Length; i++)
                {
                    Label[i].X = 0 - nPosX;
                    if (nFlag)
                        Label[i].Y = nDisplay.Image.Height - ((i+1) * nFontpitch) - nPosY;
                    else
                        Label[i].Y = (i * nFontpitch) - nPosY;
                    nDisplay.StaticGraphics.Add(Label[i] as ICogGraphic, "Result Text");
                }
            }
            catch (System.Exception ex)
            {
            }
        }
        #endregion
        //-------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static void DrawCalibration(CogRecordDisplay Display, int m_CamNo, double X, double Y)
        {
            CogLine mCogLine1 = new CogLine();
            CogLine mCogLine2 = new CogLine();
            mCogLine1.Color = CogColorConstants.Yellow;
            mCogLine2.Color = CogColorConstants.Green;

            mCogLine1.SetFromStartXYEndXY(0, Y, (double)(double)Main.vision.IMAGE_SIZE_X[m_CamNo], Y);
            Display.StaticGraphics.Add(mCogLine1 as ICogGraphic, "Find MarkerPos");

            mCogLine2.SetFromStartXYEndXY(X, 0, X, (double)(double)Main.vision.IMAGE_SIZE_Y[m_CamNo]);
            Display.StaticGraphics.Add(mCogLine2 as ICogGraphic, "Find MarkerPos");
        }
        public static void DrawCalibration_Theta(CogRecordDisplay Display, CogFitCircleTool mCogfitCircleTool)
        {
            if (mCogfitCircleTool.Result != null)
            {
                CogGraphicLabel Label = new CogGraphicLabel(); //Results Text 
                Label.Alignment = CogGraphicLabelAlignmentConstants.TopLeft;
                Label.Text = mCogfitCircleTool.Result.RMSError.ToString("0.00000000000000000000");

                if (mCogfitCircleTool.Result.RMSError > Main.DEFINE.RMSERROR)
                {
                    Label.Color = CogColorConstants.Red;
                }

                Display.StaticGraphics.Add(Label as ICogGraphic, "A");
                Display.StaticGraphics.AddList(mCogfitCircleTool.Result.CreateResultGraphics(CogFitCircleResultGraphicConstants.All), "A");   

            //    Display.Record = mCogfitCircleTool.CreateLastRunRecord();
            }
        }
        public static void CrossLine(CogDisplay nDisplay, int m_CamNo)
        {
             CogLine mCogLine1 = new CogLine();
             CogLine mCogLine2 = new CogLine();
             mCogLine1.Color = CogColorConstants.Green;
             mCogLine2.Color = CogColorConstants.Green;
             mCogLine1.SetFromStartXYEndXY(0, (double)Main.vision.IMAGE_CENTER_Y[m_CamNo], (double)Main.vision.IMAGE_SIZE_X[m_CamNo], (double)Main.vision.IMAGE_CENTER_Y[m_CamNo]);
             nDisplay.StaticGraphics.Add(mCogLine1 as ICogGraphic, "Find MarkerPos");
             mCogLine2.SetFromStartXYEndXY((double)Main.vision.IMAGE_CENTER_X[m_CamNo], 0, (double)Main.vision.IMAGE_CENTER_X[m_CamNo], (double)Main.vision.IMAGE_SIZE_Y[m_CamNo]);
             nDisplay.StaticGraphics.Add(mCogLine2 as ICogGraphic, "Find MarkerPos");

            if (Main.vision.USE_CUSTOM_CROSS[m_CamNo] == true)
            {
                CogLine mCogLine3 = new CogLine();
                CogLine mCogLine4 = new CogLine();
                mCogLine3.Color = CogColorConstants.Magenta;
                mCogLine4.Color = CogColorConstants.Magenta;
                mCogLine3.SetFromStartXYEndXY(0, (double)Main.vision.CUSTOM_CROSS_Y[m_CamNo], (double)Main.vision.IMAGE_SIZE_X[m_CamNo], (double)Main.vision.CUSTOM_CROSS_Y[m_CamNo]);
                nDisplay.StaticGraphics.Add(mCogLine3 as ICogGraphic, "Find MarkerPos");
                mCogLine4.SetFromStartXYEndXY((double)Main.vision.CUSTOM_CROSS_X[m_CamNo], 0, (double)Main.vision.CUSTOM_CROSS_X[m_CamNo], (double)Main.vision.IMAGE_SIZE_Y[m_CamNo]);
                nDisplay.StaticGraphics.Add(mCogLine4 as ICogGraphic, "Find MarkerPos");
            }
        }
        public static void CrossLine(CogDisplay nDisplay, int m_CamNo, double nPosX, double nPosY)
        {
            CogLine mCogLine1 = new CogLine();
            CogLine mCogLine2 = new CogLine();
            mCogLine1.Color = CogColorConstants.Purple;
            mCogLine2.Color = CogColorConstants.Purple;
            mCogLine1.SetFromStartXYEndXY(0, (double)Main.vision.IMAGE_CENTER_Y[m_CamNo] - nPosY, (double)Main.vision.IMAGE_SIZE_X[m_CamNo], (double)Main.vision.IMAGE_CENTER_Y[m_CamNo] - nPosY);
            nDisplay.StaticGraphics.Add(mCogLine1 as ICogGraphic, "Find MarkerPos");
            mCogLine2.SetFromStartXYEndXY((double)Main.vision.IMAGE_CENTER_X[m_CamNo] - nPosX, 0, (double)Main.vision.IMAGE_CENTER_X[m_CamNo] - nPosX, (double)Main.vision.IMAGE_SIZE_Y[m_CamNo]);
            nDisplay.StaticGraphics.Add(mCogLine2 as ICogGraphic, "Find MarkerPos");
        }
        public static void DrawOrigin(CogRecordDisplay Display, double X, double Y)
        {
            CogLineSegment[] line = new CogLineSegment[4];

            double SX = 30;
            double SY = 30;

            for (int i = 0; i < 4; i++)
            {
                line[i] = new CogLineSegment();
                line[i].Color = CogColorConstants.Red;
                switch (i)
                {
                    case 0:
                        line[i].StartX = X; line[i].StartY = Y; line[i].EndX = X - SX; line[i].EndY = Y;
                        break;
                    case 1:
                        line[i].StartX = X; line[i].StartY = Y; line[i].EndX = X + SX; line[i].EndY = Y;
                        break;
                    case 2:
                        line[i].StartX = X; line[i].StartY = Y; line[i].EndX = X; line[i].EndY = Y - SY;
                        break;
                    case 3:
                        line[i].StartX = X; line[i].StartY = Y; line[i].EndX = X; line[i].EndY = Y + SY;
                        break;
                }
                Display.StaticGraphics.Add(line[i] as ICogGraphic, "Pattern Pos4");
            }
        }
        public static void DrawTrainedPattern(CogRecordDisplay Display, CogSearchMaxTool PMAlignTool)
        {
            if (PMAlignTool.Pattern.Trained)
            {            
                Display.Image = PMAlignTool.Pattern.GetTrainedPatternImage();
                Display.Fit();
            }
            else
            {
                Display.Image = null;
            }

        }

    }
}
