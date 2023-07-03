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
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.LineMax;
using Cognex.VisionPro.ImageProcessing;

namespace COG
{
    public partial class Main
    {
        public partial class PatternTag
        {
            public int m_PatNo = 0;
            public int m_PatAlign_No = 0;
            public int m_CamNo = 0;
            public int m_DisNo = 0;
            public int m_Cmd = 0;
            public CogImageFileTool ImageBuf;
            public ICogImage m_ResultImage;
            public string m_PatternName = " ";
            public string m_PatternName_Short = " ";
            public string m_AlignName = "";

            public int m_DrawPat = 0;
            public int m_DrawPat_CAL = 0;
            public int m_DrawPat_CAL_THETA = 0;
            //--------------------------OPTION-----------------------------------------
            public bool m_Manu_Match_Use = false;
            public bool m_PMAlign_Use = false;
            //-------------------------------------------------------------------------
            //           public List<double> m_LogLengthX = new List<double>();
            public List<double> m_LogLengthY = new List<double>();

            public double[] m_Length = new double[2];//길이측정한값저장. TAB길이측정사용중
            public double[] PixelCaliper = new double[2];
            public double[] PixelFindLine = new double[2];
            public double[] Pixel = new double[2];
            public double m_RobotPosX, m_RobotPosY;
            public double Angle;
            public double m_CamOffsetX = 0, m_CamOffsetY = 0;   // BP, Align - Inspection Camera Center Distance Offset in mm

            public bool SearchResult;
            public bool Caliper_SearchResult;
            public bool Caliper_Only_SearchResult;
            public bool ImageSaveResult;

            public double[] m_CalX = new double[2];
            public double[] m_CalY = new double[2];

            public double m_CenterX = new double();
            public double m_CenterY = new double();

            public double[] InspectionPosRobot_X = new double[Main.DEFINE.FINDLINE_MAX];    // 3 -> X, Y, Diag
            public double[] InspectionPosRobot_Y = new double[Main.DEFINE.FINDLINE_MAX];

            public double[] InspectionSizeRobot_X = new double[Main.DEFINE.FINDLINE_MAX];
            public double[] InspectionSizeRobot_Y = new double[Main.DEFINE.FINDLINE_MAX];

            public double m_CAMFixPos_X = new double(); //Stage Center
            public double m_CAMFixPos_Y = new double(); //Stage Center

            public double m_CenterX_Fix = new double(); //Stage Center
            public double m_CenterY_Fix = new double(); //Stage Center

            public double[] CALMATRIX = new double[9];
            public double[,] CAMCCDTHETA = new double[1, 2];
            public bool CALMATRIX_NOTUSE = new bool();

            public double[] m_CAL_MOTPOS = new double[4];

            public int m_PatSubNum = 0;
            public int SearchSubNum = 0;
            public int m_ToolMode = 0;//CNL , BLOB , CALIPER

            private MTickTimer m_Timer = new MTickTimer();

            public double m_ACCeptScore = 0.75;
            public double m_GACCeptScore = 0.90;
            public CogSearchMaxTool[] Pattern = new CogSearchMaxTool[Main.DEFINE.SUBPATTERNMAX];
            public CogPMAlignTool[] GPattern = new CogPMAlignTool[Main.DEFINE.SUBPATTERNMAX];

            public bool[] Pattern_USE = new bool[Main.DEFINE.SUBPATTERNMAX];
            public PMResult PMAlignResult = new PMResult();

            public bool TrayDraw = false;

            public Queue<ICogImage> ImageQue = new Queue<ICogImage>();

            private CogImage8Grey _tempImage = new CogImage8Grey();
            public CogImage8Grey TempImage
            {
                get
                {
                    CogImage8Grey grey = null;
                    lock(_tempImage)
                        grey = _tempImage;
                    return grey;
                }
                set
                {
                    lock(_tempImage)
                        _tempImage = value;
                }

            }

            public CogImage8Grey FixtureImage = new CogImage8Grey();
            public CogFixtureTool FixtureTool = new CogFixtureTool();
            public CogTransform2DLinear FixtureTrans = new CogTransform2DLinear();
            public CogTransform2DLinear TempFixtureTrans = new CogTransform2DLinear();
            #region SD BIO
            public List<SDParameter> m_InspParameter = new List<SDParameter>();
            public CogGraphicInteractiveCollection resultGraphics = new CogGraphicInteractiveCollection();
            public CogGraphicInteractiveCollection resultDipGraphics = new CogGraphicInteractiveCollection();
            //public CogFindCornerTool[] m_CogFindCorner = new CogFindCornerTool[4];
            public CogFindLineTool[] m_TrackingLine = new CogFindLineTool[4];
            public CogCaliperTool[] m_BondingAlignLine = new CogCaliperTool[4];
            public CogSearchMaxTool[,] m_FinealignMark = new CogSearchMaxTool[Main.DEFINE.Pattern_Max, Main.DEFINE.SUBPATTERNMAX];
            public double m_FinealignMarkScore = 0.7;
            public double m_FinealignThetaSpec;
            public bool m_bFInealignFlag = true;
            public double m_FinealignGapT;
            public bool m_bInspDirectionChangeFlag = false;

            public double[] LeftOrigin = new double[2];
            public double[] RightOrigin = new double[2];
            public bool InspComplete = false;
            public bool bResult = true;
            public enumResultType ResultType;
            public double m_dTopAlignX;
            public double m_dTopAlignY;
            public double m_dBottomAlignX;
            public double m_dBottomAlignY;
            //BondingAlign spec - shkang
            public double m_dOriginDistanceX;
            public double m_dOriginDistanceY;
            public double m_dDistanceSpecX;
            public double m_dDistanceSpecY;
            public double m_dObjectDistanceX;
            public double m_dObjectDistanceSpecX;

            #endregion
            #region CALIPER
            public CaliperTagData[] CaliperPara = new CaliperTagData[Main.DEFINE.CALIPER_MAX];
            public CogCaliperTool[] CaliperTools = new CogCaliperTool[Main.DEFINE.CALIPER_MAX];
            public CaliperResult[] CaliResults = new CaliperResult[Main.DEFINE.CALIPER_MAX];
            public bool CaliperDraw = false;
            public bool Caliper_MarkUse = false;
            public bool Caliper_MarkPosUse = false;
            public double[] Caliper_MarkPos = new double[2];
            public double[,] BlobRegionPos = new double[2, Main.DEFINE.BLOB_CNT_MAX];
            #endregion

            #region BLOB
            public BlobTagData[] BlobPara = new BlobTagData[Main.DEFINE.BLOB_CNT_MAX];
            public CogBlobTool[] BlobTools = new CogBlobTool[Main.DEFINE.BLOB_CNT_MAX];  //티칭창 초기 region 영역 받기 위해 필요
            public BlobResult[] BlobResults = new BlobResult[Main.DEFINE.BLOB_CNT_MAX];
            public bool BlobDraw = false;
            public bool Blob_MarkUse = false;
            public bool Blob_CaliperUse = false;
            public int m_Blob_InspCnt = 0;
            double[,] Y_MAXGAP = new double[4, 2];
            double[,] Y_MINGAP = new double[4, 2];
            public string BlobScore;
            #endregion
            #region Inspection
            public CogFindCircleTool[,] m_CogFindCircleTool;
            public CogFindLineTool[,] m_CogFindLineTool;
            public int[,] m_iCount;
            public int[,] m_iType;
            public double[,] m_dPX1;
            public double[,] m_dPY1;
            public double[,] m_dPX2;
            public double[,] m_dPY2;
            public double[,] m_dPX3;
            public double[,] m_dPY3;
            public double[] m_ImageCenterX;
            public double[] m_ImageCenterY;
            public double[] m_ImageCenterLenthX;
            public double[] m_ImageCenterLenthY;
            #endregion
            #region FINDLINE
            public FindLineTagData[,] FINDLinePara = new FindLineTagData[Main.DEFINE.SUBLINE_MAX, Main.DEFINE.FINDLINE_MAX];
            public CogFindLineTool[,] FINDLineTools = new CogFindLineTool[Main.DEFINE.SUBLINE_MAX, Main.DEFINE.FINDLINE_MAX];
            public CogLineMaxTool[,] LineMaxTools = new CogLineMaxTool[Main.DEFINE.SUBLINE_MAX, Main.DEFINE.FINDLINE_MAX];
            public FindLineResult[] FINDLineResults = new FindLineResult[Main.DEFINE.FINDLINE_MAX];

            public double TRAY_GUIDE_DISX;
            public double TRAY_GUIDE_DISY;
            public double TRAY_PITCH_DISX;
            public double TRAY_PITCH_DISY;

            public bool FINDLineDraw = false;
            public bool FINDLine_MarkUse = false;

            // 200716 BP
            public ushort m_FindLinePositions = 0;
            public bool m_UseLineMax = false;

            public double m_dCustomCrossX = 0;  // 20201117
            public double m_dCustomCrossY = 0;  // 모니터링용으로 요청했던 custom cross로 loading 편차 체크까지...
            public bool m_bCompLight = false;
            public string m_strImageLogName = "";
            #endregion

            #region CIRCLE
            public CogFindCircleTool[] CircleTools = new CogFindCircleTool[Main.DEFINE.CIRCLE_MAX];
            public FindCircleTagData[] CirclePara = new FindCircleTagData[Main.DEFINE.CIRCLE_MAX];
            public CircleResult[] CircleResults = new CircleResult[Main.DEFINE.CIRCLE_MAX];
            public bool CircleDraw = false;
            public bool Circle_MarkUse = false;
            #endregion

            #region FindCorner
            public CogFindCornerTool[] CornerTools = new CogFindCornerTool[Main.DEFINE.CORNER_MAX];
            public CornerResult[] CornerResults = new CornerResult[Main.DEFINE.CORNER_MAX];
            #endregion

            //Tray
            //    public PMResult[] TRAYResults = new PMResult[Main.DEFINE.TRAY_POCKET_LIMIT];

            public List<PMResult> TRAYResults = new List<PMResult>();
            public List<TrayBlobResult> TRAYBlobResult = new List<TrayBlobResult>();

            public int m_TrayCount_OK = 0;
            public int nPocketNum = -1;



            public CogFitCircleTool FitCicleTool = new CogFitCircleTool();
            /////////////////SD Bio//////////
            public class SDParameter
            {
                public enum enumROIType { Line = 0, Circle = 1 };
                public enumROIType m_enumROIType;
                public CogFindLineTool m_FindLineTool;
                public CogFindCircleTool m_FindCircleTool;
                public CogBlobTool[] m_CogBlobTool;
                public CogHistogramTool[] m_CogHistogramTool;
                public double CenterX;
                public double CenterY;
                public double LenthX;
                public double LenthY;
                public double dSpecDistance;
                public double dSpecDistanceMax;
                public int IDistgnore;
                public int iHistogramROICnt;
                public int[] iHistogramSpec;
                public bool bThresholdUse;
                public int iThreshold { get; set; } = 32;
                public int iTopCutPixel { get; set; } = 15;
                public int iIgnoreSize { get; set; } = 20;
                public int iBottomCutPixel { get; set; } = 10;
                public int iMaskingValue { get; set; } = 210;
                public int iEdgeCaliperThreshold { get; set; } = 55;
                public int iEdgeCaliperFilterSize { get; set; } = 10;
            }

            public SDParameter Reset()
            {
                SDParameter RestData = new SDParameter();
                RestData.m_FindCircleTool = new CogFindCircleTool();
                RestData.m_FindCircleTool = new CogFindCircleTool();
                RestData.m_CogBlobTool = new CogBlobTool[10];
                RestData.m_CogHistogramTool = new CogHistogramTool[32];
                RestData.iHistogramSpec = new int[32];
                for (int i = 0; i < 10; i++)
                {
                    RestData.m_CogBlobTool[i] = new CogBlobTool();
                    RestData.m_CogBlobTool[i].RunParams.SegmentationParams.Mode = CogBlobSegmentationModeConstants.HardFixedThreshold;
                    RestData.m_CogBlobTool[i].RunParams.SegmentationParams.Polarity = CogBlobSegmentationPolarityConstants.LightBlobs;
                }
                for (int i = 0; i < 32; i++)
                {
                    RestData.m_CogHistogramTool[i] = new CogHistogramTool();
                    RestData.iHistogramSpec[i] = new int();
                }
                RestData.m_enumROIType = new SDParameter.enumROIType();
                RestData.CenterX = 0;
                RestData.CenterY = 0;
                RestData.LenthX = 0;
                RestData.LenthY = 0;
                RestData.dSpecDistance = 0;
                RestData.dSpecDistanceMax = 0;
                RestData.IDistgnore = 0;
                return RestData;
            }
            public SDParameter[] m_SDParameter = new SDParameter[6];

        }//PatternTag
    }//Main
}//COG
