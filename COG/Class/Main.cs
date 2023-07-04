//#define BP_INSPECTION_PC1_MODE
#define BP_INSPECTION_PC2_MODE

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

namespace COG
{
    public partial class Main
    {
        public partial struct DEFINE
        {
       //     public const bool OPEN_F = false;
            public const bool OPEN_CAM = false;

#if NON_STANDARD
            public const bool NON_STANDARD = true;
#else
            public const bool NON_STANDARD = false;
#endif
            public const string CAM_SETDIR = "VPP_CAM\\";
            public const string LOG_DATADIR = "logdata\\";
            public const string ERROR_DATADIR = "Error_Data\\";
            public const string SYS_DATADIR = "D:\\Systemdata_" + PROGRAM_TYPE + "\\";

            public const string PASSWORD_DIR = "Software\\JAStech"; //Regedit : HKEY_CURRENT_USER-> SOFTWARE-> JASTECHJAS_DAWIN
            public const string LANGUAGE_DIR = "Software\\JAStechLan"; //Regedit : HKEY_CURRENT_USER-> SOFTWARE-> JASTECHJAS_DAWIN
            //---------------------------Model따라변경할것-------------------------
            public const string MODEL_DATADIR = "VISION";


            //HEAD MODE ------------------------------------------------------------
            public const string T_MOVE = "T_MOVE";
            public const string XYT_MOVE = "XYT_MOVE";
            public const string HEAD_OBJ = "OBJ";
            public const string HEAD_TAR = "TAR";
            //PBD MODE
            public const string COF_TYPE = "COF";
            public const string REAR_STAGE = "FOF";   // 뒷 스테이지
            public const string FRONT_STAGE = "FOP";   // 앞 스테이지
            //---------------------------------------------------------------------
            public const int Board_Max = 1;

            public const int CAM0 = 0;
            public const int CAM1 = 1;
            public const int CAM2 = 2;
            public const int CAM3 = 3;

            public const int CAM4 = 4;
            public const int CAM5 = 5;
            public const int CAM6 = 6;
            public const int CAM7 = 7;

            public const int CAM8 = 8;
            public const int CAM9 = 9; 
            public const int CAM10 = 10;
            public const int CAM11 = 11;

            //-----------------------------------------------------------------------------
            //public const String PatName0 = "__LEFT";
            //public const String PatName1 = "_RIGHT";
#if BP_INSPECTION_PC1_MODE
            public const String TARName = "_TARGET";
            public const String OBJName = "_CAMERA";

            public const String PatName0 = "_ALIGN";
            public const String PatName1 = "__INSP";
#elif BP_INSPECTION_PC2_MODE
            public const String TARName = "_TARGET";
            public const String OBJName = "_CAMERA";

            public const String PatName0 = "___UP";
            public const String PatName1 = "_DOWN";
#else
            public const String TARName = "_TARGET";
            public const String OBJName = "_OBJECT";

            public const String PatName0 = "_FRONT_L";
            public const String PatName1 = "_FRONT_R";
#endif
            public const String PatName2 = "_REAR_L";
            public const String PatName3 = "_REAR_R";

            //----------------------LOG----------------------------------------------------
            public const String CMD        = "CMD";
            public const String INSPECTION = "INSPECTION";
            public const String PIXEL      = "PIXEL_ROBOT";
            public const String ALIGN      = "ALIGN";
            public const String ERROR      = "ERROR";
            public const String TABLENGTH  = "TABLENGTH";
            public const String DATA = "DATA";
            public const String MARKSCORE = "MARK_SCORE";
            public const String LIGHTCTRL = "LIGHT_CTRL";
            public const String CHANGEPARA = "CHANGE_PARA";

            //----------------------LIGHT--------------------------------------------------
            public const String M_CONTROL_ON  = "1";
            public const String M_CONTROL_OFF = "0";
            public const int Light_ToolMaxCount = 2; // Pattern에 속해 있는 Tool별 조명을 따로 가져가기 위해서, Tool의 최대갯수를 입력하면됨(CNLSEARCH , CALIPER , BLOB)
            public const int M_LIGHT_CNL        = 0;
            public const int M_LIGHT_BLOB       = 1;
            public const int M_LIGHT_CALIPER    = 2;
            public const int M_LIGHT_LINE       = 3;
            public const int M_LIGHT_CIRCLE     = 4;

            public const byte LVS_LIGHT_CMD_START           = 0x01;
            public const byte LVS_LIGHT_CMD_WRITE           = 0x00;
            public const byte LVS_LIGHT_CMD_READ            = 0x01;
            public const byte LVS_LIGHT_CMD_END             = 0x04;
            public const byte LVS_LIGHT_CMD_ACK             = 0x06;
            public const byte LVS_LIGHT_CMD_NAK             = 0x15;
            public const byte LVS_LIGHT_CMD_RIGISTER_RTR    = 0x00; //Read TO Register
            public const byte LVS_LIGHT_CMD_RIGISTER_RWTR   = 0x04; //Read/Write Test Register
            public const byte LVS_LIGHT_CMD_RIGISTER_CSR    = 0x20; //Channel Select Register
            public const byte LVS_LIGHT_CMD_RIGISTER_SVR    = 0x28; //Step Value Register
            public const byte LVS_LIGHT_CMD_RIGISTER_SCR    = 0x2C; //System Control Register
            public const byte LVS_LIGHT_CMD_RIGISTER_RCR    = 0x2D; //Reset Control Register
            public const byte LVS_LIGHT_CMD_RIGISTER_SOR    = 0x2E; //System Option Register
            public const byte LVS_LIGHT_CMD_RIGISTER_SSR    = 0x30; //System Status Register
            public const byte LVS_LIGHT_CMD_RIGISTER_COR    = 0x34; //Channel On/OFF Register
            public const byte LVS_LIGHT_CMD_RIGISTER_PCR    = 0x38; //Page Control Register
            public const byte LVS_LIGHT_CMD_RIGISTER_PACR   = 0x39; //Page Count Register
            public const byte LVS_LIGHT_CMD_RIGISTER_PASR   = 0x3C; //Page Select Register
            public const byte LVS_LIGHT_CMD_RIGISTER_EEAR   = 0x40; //Ethernet Environment Address Register
            public const byte LVS_LIGHT_CMD_RIGISTER_EEDR   = 0x44; //Ethernet Environment Data Register
            public const byte LVS_LIGHT_CMD_RTR_DATA        = 0x34;

            public const int LVS_LIGHT_RESP_TIMEOUT = 500;

            //-----------------------------------------------------------------------------
            public const int M_CNLSEARCHTOOL    = 0;
            public const int M_BLOBTOOL         = 1;
            public const int M_CALIPERTOOL      = 2;
            public const int M_FINDLINETOOL     = 3;
            public const int M_FINDCIRCLETOOL   = 4;
            public const int M_PMALIGNTOOL      = 5;
            public const int M_INSPECTION       = 5;
            public const int M_ALIGNINPECTION   = 6;
            //-----------------------------------------------------------------------------
            public const double RMSERROR = 1.5;
            //-----------------------------------------------------------------------------
            //-----------------------------------------------------------------------------

            public const int ALIGNDATA_MAX = 30;  //LogView Cnt
            public const int ALIGNINSPECDATA_MAX = 30;  //LogView Cnt

#if BP_INSPECTION_PC1_MODE
            public const double DIRECTORY_LIMIT_GB = 1600; //GB
#elif BP_INSPECTION_PC2_MODE
            public const double DIRECTORY_LIMIT_GB = 1600; //GB
#else
            public const double DIRECTORY_LIMIT_GB = 60; //GB
#endif

            public const double PI = 3.141592653589;
            public const double radian = PI / 180.0; //
            public const double degree = 180.0 / PI;
            public const String FontStyle = "test";
            public const float FontSize = 35.0f; //화면을 이거를 기준으로 등분함.. 값이 작을수록 글자가 커진다

            public const int M_1CAM1SHOT = 100;   // 1 camera 1 shot , Center Target
            public const int M_1CAM2SHOT = 200;   // 1 camera 2 shot , Center Target            
            public const int M_2CAM1SHOT = 300;   // 2 camera 1 shot , Center Target
            public const int M_2CAM2SHOT = 400;  // 2 camera 2 shot , Target 인식 
            public const int M_4CAM1SHOT = 500;   // 4 camera 1 shot , Target 인식 
            public const int M_1CAM4SHOT = 600;

            public const int M_CENTER = 3;
            public const int M_LEFT = 10;
            public const int M_RIGHT = 20;
            public const int M_TOP = 30;
            public const int M_BOTTOM = 40;  

//          public const int BLOB_INSPECT_ALIGNNO = 9; // 9  이상부터 검사 가능 유닛.

            public const int MOT_U = 0;
            public const int MOT_V = 1;
            public const int MOT_W = 2;
            public const int MOT_TRANS_PNP = 3;


            public const int FINDLINE_CONNER_NUM = 3;



            //-------------ToolBlock Define----------------/
            public const string CogPMAlignTool_1 = "CogPMAlignTool1";
            public const string CogPMAlignTool_2 = "CogPMAlignTool2";
            public const string CogPMAlignTool_3 = "CogPMAlignTool3";
            public const string CogPMAlignTool_4 = "CogPMAlignTool4";

            public const int CogPMBlobTool1 = 0;
            //             public const int PMAlignScore  = 1;
            //-------------SEARCH RETURN------------------//
            public const int MAIN = 0; //MainPattern
            public const int SUBPATTERNMAX = 6; //10개 이상으로 변경시에 Teach창에 디스플레이쪽 영역 넓혀야됨. Max 10개로 영역잡아놓음 
            public const double DEFAULT_ACCEPT_SCORE = 0.55;
            public const double DEFAULT_CONFUSION_SCORE = 0.9;

            public const double DEFAULT_GACCEPT_SCORE = 0.6;
            public const double DEFAULT_ACCEPT_ANGLE = 2;

            public const double m_ACCeptScore100 = 1.00;


            public const int DEFAULT_SEARCH_AREA = 30;



            public const int PMAlignResult = 0;
            public const int PMAlignScore = 1;
            public const int PMAlignResultPixelX = 2;
            public const int PMAlignResultPixelY = 3;
            public const int PMAlignSearchArea = 4;
            public const int PMAlignPatternArea = 5;

            public const int PBD_L = 0;
            public const int PBD_R = 1;

            //---------설비 상태--------------------//
            public const int MC_STOP = 0;
            public const int MC_RUN = 1;
            public const int MC_ERROR = 2;
            public const int MC_WARNING = 3;
            public const int MC_RESET = 4;
            public const int MC_TEACHFORM = 5;
            public const int MC_MAINFORM = 6;
            public const int MC_SETUPFORM = 7;
            public const int MC_LIVEFORM = 8;
            public const int MC_CAMERAFORM = 9;
            public const int MC_RCSFORM = 10;
            public const int MC_PERMISSIONFORM = 11;
            //--------조명 상태--------------------//
            public const int MC_LIGHT_ON = 1;
            public const int MC_LIGHT_OFF = 2;

            //SW_SHOW, SW_HIDE
            public const int SW_SHOW = 5;
            public const int SW_HIDE = 0;

            public const int OBJ_L = 0;
            public const int OBJ_R = 1;
            public const int TAR_L = 2;
            public const int TAR_R = 3;

            public const int OBJ_ALL    = 4;
            public const int TAR_ALL    = 5;
            public const int LEFT_ALL   = 6;
            public const int RIGHT_ALL  = 7;
            public const int OBJTAR_ALL = 8;
            public const int CHIPPAT_ALL= 9;

            public const int FRONT_L = 0;
            public const int FRONT_R = 1;
            public const int REAR__L = 2;
            public const int REAR__R = 3;

            public const int WINDOW = 4;
            public const int PANEL_ = 0;

            public const int VCM = 0;
            public const int FPC = 1;

            public const bool USE = true;
            public const bool NOT_USE = false;

            public const int WINDOW_CENTER = 1;
            public const int PANEL__CENTER = 0;

            public const int X = 0;
            public const int Y = 1;
            public const int T = 2;
            public const int T2 = 3;

            public const int FRONT_ALL = 10;
            public const int REAR__ALL = 20;
            public const int LIGHT_OFF = 30;
            //--------------------------------------------------------------------
            //LENGTH CKECK
            public const int LCHECK_L1 = 0;
            public const int LCHECK_L2 = 1;
            public const int LCHECK_L3 = 2;
            public const int LCHECK_L4 = 3;
            //--------------------------------------------------------------------
            //INSPECTION
            public const int WIDTH_ = 0;
            public const int HEIGHT = 1;
            public const int EXTENSION = 2;

            public const int FIRST_ = 0;
            public const int SECOND = 1;

            public const int EDGE = 0;
            public const int BLOB = 1;

            public const int FRONT_L__Width = 0;
            public const int FRONT_L_Height = 1;

            public const int FRONT_R__Width = 2;
            public const int FRONT_R_Height = 3;

            public const int REAR_L__Width = 4;
            public const int REAR_L_Height = 5;

            public const int REAR_R__Width = 6;
            public const int REAR_R_Height = 7;

            public const int FRONT = 0;
            public const int REAR_ = 2;
            //--------------------------------------------------------------------
            public const int CAL_X = 0;
            public const int CAL_Y = 1;
            public const int CAL_XY = 2;
            public const int CAL_T = 3;

            public const int CMD_CHECK_TIMEOUT = 1000;
            public const int INSECTION_WAIT_TIMEOUT = 2000;
            public const int COMP_WAIT_TIMEOUT = 1000;
            public const int MOVE_TIMEOUT = 10000;
            public const int MOVE_TIMEOUT_CAL = 30000;
            public const int GRAB_TIMEOUT = 10000;

            public const int CAMEARA_TIMEOUT = 3000;
            public const double PATTERN_TIMEOUT = 1000;

            public const int CMD_HANDSHAKE_TIMEOUT = 2000;
            public const int NORMAL_HANDSHAKE_TIMEOUT = 1000;
            public const int MANUAL_HANDSHAKE_TIMEOUT = 300000;

            //-----------------------------------------------------------------------
            public const int M_TOOLMAXCOUNT     = 5;
            //-----------------------------------------------------------------------
            //---------------------BLOB 관련-----------------------------------------
            public const int BLOB_INSP_LIMIT_CNT = 6;
            public const int MIN_X = 0, MAX_X = 1, MIN_Y = 2, MAX_Y = 3;
            //----------------------CALIPER 관련-------------------------------------
            public const int CALIPER_MAX = 4;
            public const int LEFT_TO_RIGHT = 0;
            public const int TOP_TO_BOTTOM = 1;
            public const int RIGHT_TO_LEFT = 2;
            public const int BOTTOM_TO_TOP = 3;
            //-------------------FINDLINE--------------------------------------------
            public const int FINDLINE_MAX = 4;
            public const int SUBLINE_MAX = 3;
            public const int TRAY_POCKET_LIMIT = 300;
            public const int LINEMAX_H_YMIN = 0;
            public const int LINEMAX_H_YMAX = 1;
            public const int LINEMAX_V_XMIN = 0;
            public const int LINEMAX_V_XMAX = 1;
            public const int CLP_METHOD_SCORE = 0;
            public const int CLP_METHOD_POS = 1;
            //-------------------FINCIRCLE--------------------------------------------
            public const int CIRCLE_MAX = 1;
            //-------------------FIND CORNER-----------------------------------------
            public const int CORNER_MAX = 1;
            //-----------------------------------------------------------------------

            public const int ENGLISH    = 1033;//"en-us"
            public const int KOREA      = 1042;//"ko-kr"
            public const int VIETNAM    = 1066;//"vi-VN"
            public const int CHINA      = 2052;//"zh-cn"


            public const int FOP_TAG_MODE = 0;
            public const int FOF_TAG_MODE = 1;

        }
        public partial struct DEFINE //
        {
            public const int CURRENT_MODEL_CODE = 000; // ERROR_CODE
            public const int ERROR_BIT          = 002;
            public const int ALIVE              = 010;
            public const int ERROR_CODE = 020;  
            public const int PPID_MODEL_NAME    = 022;

            public const int VIS_ALIGN_X = 000;
            public const int VIS_ALIGN_Y = 002;
            public const int VIS_ALIGN_T = 004;
            public const int VIS_STATUS = 006;
            public const int VIS_MOVE_REQ = 007;
            public const int VIS_READY = 004;
            public const int VIS_ALARM = 005;

            public const int MAP_DATA_PBD = 204;
            public const int LENGTH_ADD = 206;

            public const int SCORE = 700; // VISION -> PLC
            public const int TIME = 300; // PLC -> VISION

            public const int PLC_MODEL_CODE = 100;
            public const int PLC_AXIS_X = 100;
            public const int PLC_AXIS_Y = 102;
            public const int PLC_AXIS_T = 104;
            public const int PLC_CMD = 106;
            public const int PLC_STAGE1_CMD = 106;
            public const int PLC_MOVE_END = 107;
            public const int PLC_VISION_SKIP_MODE = 107;
            public const int PLC_TAR_MARK_LENTH = 008;
            public const int PLC_OBJ_MARK_LENTH = 108;

            public const int PLC_ALIVE = 200;
            public const int PBD_REALIGN_DIS_Y = 201; // PBD ReAlign Y Distance 
            public const int PLC_OFFSET_X = 206; // PLC -> VISION 
            public const int PLC_OFFSET_Y = 207; // PLC -> VISION 
            public const int PLC_OFFSET_T = 208; // PLC -> VISION 
            public const int MANUAL_MATCH = 209;
            public const int STAGE_NUM = 212; // STAGE NUMBER
            //---------------------------------------------------------------------------------------------------------------------------------
            public const int PLC_LIGHTCHANGE = 203;
            //   public const int CELLID_POS = 300;
            public const int CELLID_READ_CMD = 300;
            public const int CELLID_NUM = 1500;
            public const int CELLID_NUM2 = 400;
            public const int MODULED_NUM = 80;

            //-------------------------------------------------------------------------------------------------------------------------------
            public const int CL_X   = 0;
            public const int OK_X   = 1;
            public const int CL_Y   = 2;
            public const int OK_Y   = 3;
            public const int OK_C_R = 4;
            public const int LOAD_NG = 5;

            public const int COMP_1ST = 0;
            public const int OK_1ST = 1;
            public const int NG_1ST = 2;
            public const int COMP_2ND = 3;
            public const int OK_2ND = 4;
            public const int NG_2ND = 5;
            public const int CROSS_NG = 10;
            public const int VERTI_NG = 11;

            public const int SEARCH1_X     = 0;
            public const int SEARCH1_Y     = 2;
            public const int SEARCH1_R_T   = 4;
            public const int SEARCH1_SCORE = 6;

            public const int SEARCH2_X     = 8;
            public const int SEARCH2_Y     = 10;
            public const int SEARCH2_R_T   = 12;
            public const int SEARCH2_SCORE = 14;

            public const int SEARCH3_X     = 16;
            public const int SEARCH3_Y     = 18;
            public const int SEARCH3_R_T   = 20;
            public const int SEARCH3_SCORE = 22;

            public const int CAM_SELECT_ALIGN = 0;
            public const int CAM_SELECT_INSPECT = 1;
            public const int CAM_SELECT_1ST_ALIGN = 0;
            public const int CAM_SELECT_PRE_ALIGN_LEFT = 0;
            public const int CAM_SELECT_PRE_ALIGN_RIGHT = 1;
            public const int CAM_SELECT_PRE_ALIGN = 4;

            public const int JOB_PANEL_PAD = 0;
            public const int JOB_PANEL_ROUND = 1;

            public const int CAM_NO_SCANHEAD = 0;
            public const int CAM_NO_CAMHEAD1 = 1;
            public const int CAM_NO_CAMHEAD2 = 2;
            public const int CAM_NO_CAMHEAD3 = 3;

            public const int PROC_DATA_OFFSET = 1500;
            public const int L_PC_DATA_OFFSET = 1200;
            public const int PLC_READ_OFFSET = 10000;

            public const int MX_ARRAY_WSTAT_OFFSET = 000;
            public const int MX_ARRAY_WPROC_OFFSET = 400;
            public const int MX_ARRAY_RSTAT_OFFSET = 200;
            public const int MX_ARRAY_RPROC_OFFSET = 100;

            //-------------------------------------------------------------------------------------------------------------------------------

            public const int YEAR = 100;
            public const int MONTH = 101;
            public const int DAY = 102;
            public const int HOUR = 103;
            public const int MINUTE = 104;
            public const int SECONDS = 105;
            public const int RUN_MODE = 106;
            public const int PANEL_SIZE_X = 108;
            public const int PANEL_SIZE_Y = 110;
            public const int PANEL_THICKNESS = 112;

            public const int NORMAL_RUN = 1;
            public const int DRY_RUN = 2;
            public const int PASS_RUN = 3;
            //-------------------------------------------------------------------------------------------------------------------------------

            public const int PPID_TARGET_MODEL_NUMBER = 200;
            public const int PPID_COPY_MODEL_NUMBER = 202;
            public const int PPID_COPY_MODEL_NAME = 204;
            public const int PPID_DELETE_MODEL_NUMBER = 224;
            public const int PPID_CHANGE_MODEL_NUMBER = 226;

            //-------------------------------------변경가능 주소 ----------------------------------------------------------------------------
            public const int LENGTH_OBJTAR = 102;		//OBJ , TAR Length 
            public const int VIS_FPCTRAY_ALIGN_DATA_FOF = 85000;
            public const int VIS_FPCTRAY_ALIGN_DATA_TFOG = 104000;
            public const int VIS_FPCTRAY_ALIGN_DATA_FOB = 18000;
            public const int PLC_TRAY_COUNT_X = 201;
            public const int PLC_TRAY_COUNT_Y = 202;

            public const int GET_TRAY_COUNT_X = 201;
            public const int GET_TRAY_COUNT_Y = 202;

            public const int VIS_CHIP_SEARCH_COUNT = 402;
            public const int FBD_TOOL_NUM = 401;
            //---------------------------------------------------------------------------------------------------------------------------------
            public const int TAB_NUM = 203;
            //   //---------------------------------------필수 주소 ----------------------------------------------------------------------------
            //   public const int CURRENT_MODEL_CODE     = 000;  
            //   public const int VIS_ALIGN_X            = 000;  
            //   public const int VIS_ALIGN_Y            = 002;  
            //   public const int VIS_ALIGN_T            = 004;  
            //   public const int VIS_STATUS             = 006;
            //   public const int VIS_MOVE_REQ           = 007;
            //   public const int VIS_READY              = 004;
            //   public const int VIS_ALARM              = 005;

            //   public const int MAP_DATA_PBD           = 204;
            //   public const int LENGTH_ADD             = 206;

            //   public const int SCORE                  = 700; // VISION -> PLC
            //   public const int TIME                   = 300; // PLC -> VISION


            //   public const int PLC_MODEL_CODE         = 100;
            //   public const int PLC_AXIS_X             = 100;
            //   public const int PLC_AXIS_Y             = 102;
            //   public const int PLC_AXIS_T             = 104;
            //   public const int PLC_CMD                = 106;
            //   public const int PLC_MOVE_END           = 107;
            //   public const int PLC_TAR_MARK_LENTH     = 008;
            //   public const int PLC_OBJ_MARK_LENTH     = 108;

            //   public const int PBD_REALIGN_DIS_Y      = 201; // PBD ReAlign Y Distance 
            //   public const int PLC_OFFSET_X           = 206; // PLC -> VISION 
            //   public const int PLC_OFFSET_Y           = 207; // PLC -> VISION 
            //   public const int PLC_OFFSET_T           = 208; // PLC -> VISION 
            //   public const int MANUAL_MATCH           = 209;
            //   public const int STAGE_NUM              = 212; // STAGE NUMBER
            //   //---------------------------------------------------------------------------------------------------------------------------------
            //   public const int PLC_LIGHTCHANGE = 203;
            ////   public const int CELLID_POS = 300;
            //   public const int CELLID_READ_CMD = 300;
            //   public const int CELLID_NUM = 300;
            //   public const int CELLID_NUM2 = 400;
            //   //-------------------------------------------------------------------------------------------------------------------------------

            //   public const int YEAR = 300;
            //   public const int MONTH = 301;
            //   public const int DAY = 302;
            //   public const int HOUR = 303;
            //   public const int MINUTE = 304;
            //   public const int SECONDS = 305;

            //   //-------------------------------------변경가능 주소 ----------------------------------------------------------------------------
            //   public const int LENGTH_OBJTAR = 102;		//OBJ , TAR Length 
            //   public const int VIS_FPCTRAY_ALIGN_DATA_FOF = 85000;
            //   public const int VIS_FPCTRAY_ALIGN_DATA_TFOG = 104000;
            //   public const int VIS_FPCTRAY_ALIGN_DATA_FOB = 18000;
            //   public const int PLC_TRAY_COUNT_X = 201;
            //   public const int PLC_TRAY_COUNT_Y = 202;

            //   public const int GET_TRAY_COUNT_X = 201;
            //   public const int GET_TRAY_COUNT_Y = 202;

            //   public const int VIS_CHIP_SEARCH_COUNT  = 402;
            //   public const int FBD_TOOL_NUM = 401; 
            //   //---------------------------------------------------------------------------------------------------------------------------------
            //   public const int TAB_NUM = 203;

            // 200707 CCLink
            public const int CCLINK_MASTER = 0xFF;
            public const int CCLINK_SLAVE = 0x00;
            public const int CCLINK_CHANEL = 81;
            public const int CCLINK_NET = 0x00;

            public const string DEF_IO_MAP_FILE = SYS_DATADIR + "InfIoMapCaps_ATT.dat";
            public const string DEF_ERR_MAP_FILE = SYS_DATADIR + "InfErrMsg_ATT.dat";
        }

        //public ePermission Permission = ePermission.OPERATOR;
        public enum ePermission
        {
            OPERATOR = 1,
            ENGINEER = 2,
            MAKER = 3,
        }


        public static bool bVisionSkip = new bool();
        public static Dictionary<string, int> item = new Dictionary<string, int>();
        private static void CAMNAME(string[] nCam)
        {            
            for(int i = 0; i < nCam.Length; i++)
            {
                vision.CamName[i] = nCam[i].ToString();
            }
        }
        private static int[] CAMNUM(int C0, int C1)
        {
            int[] nCamNo = new int[2];
            nCamNo[0] = C0;
            nCamNo[1] = C1;
            return nCamNo;
        }
        private static int[] CAMNUM(int C0, int C1, int C2)
        {
            int[] nCamNo = new int[3];
            nCamNo[0] = C0;
            nCamNo[1] = C1;
            nCamNo[2] = C2;
            return nCamNo;
        }
        private static int[] CAMNUM(int C0, int C1, int C2, int C3)
        {
            int[] nCamNo = new int[4];
            nCamNo[0] = C0;
            nCamNo[1] = C1;
            nCamNo[2] = C2;
            nCamNo[3] = C3;
            return nCamNo;
        }
        private static int[] ATYPE(int C0, int C1)
        {
            int[] nAlignType = new int[2];
            nAlignType[0] = C0;
            nAlignType[1] = C1;
            return nAlignType;
        }
        private static int[] ATYPE(int C0, int C1, int C2)
        {
            int[] nAlignType = new int[3];
            nAlignType[0] = C0;
            nAlignType[1] = C1;
            nAlignType[2] = C2;
            return nAlignType;
        }
        private static int[] ATYPE(int C0, int C1, int C2, int C3)
        {
            int[] nAlignType = new int[4];
            nAlignType[0] = C0;
            nAlignType[1] = C1;
            nAlignType[2] = C2;
            nAlignType[3] = C3;
            return nAlignType;
        }

        public struct MTickTimer
        {
            private bool bStarted;
            public DateTime timeStart;
            public DateTime timeEnd;

            public bool IsStarted()
            {
                return bStarted;
            }

            public void StartTimer()
            {
                timeStart = DateTime.Now;
                bStarted = true;
            }

            public long GetElapsedTime()
            {
                timeEnd = DateTime.Now;
                return (timeEnd.Ticks - timeStart.Ticks) / 10000; //리턴값 1ms 
            }
        }
        public struct UVW
        {
            public const double CRB_X1 = 90;
            public const double CRB_X2 = 270;
            public const double CRB_Y1 = 180;
            public const double CRB_Y2 = 0;
            public static double STAGE_R = 39; //mm
        }

        public struct Machine
        {
            public bool Overlay_Image_Onf;
            public bool LogMsg_Onf;
            public bool Manu_match_Onf;
            public bool LengthCheck_Onf;
            public bool Inspection_Onf;
            public bool BMP_ImageSave_Onf;
            public int m_RetryCount;    //shkang
            public bool m_bRetryUse;     //shkang
            public string m_EngineerPassword; //shkang engineer=maker
            public string m_MakerPassword;

            public bool MAP_Limit_Onf;
            public int MAP_High;
            public int MAP_Low;

            public bool MAP_Function_Onf;
            public double MAP_Function_Data;

       //     public bool EngineerMode; //EngineerMode Status
            public bool LogDirDeleteFlag;

            public int m_Fpcpicker1_Dis_X;
            public int m_Fpcpicker1_Dis_Y;

            public int m_Fpcpicker2_Dis_X;
            public int m_Fpcpicker2_Dis_Y;
            
            // QD Laser
            public int m_nOldLogCheckPeriod;
            public int m_nOldLogCheckSpace;
            public int m_nCCLinkCommDelay_ms;
            public bool m_bUseLoadingLimit;
            public int m_nLoadingLimitX_um;
            public int m_nLoadingLimitY_um;
            public bool m_bUseInspLimit;
            public int m_nInspLowLimit_um;
            public int m_nInspHighLimit_um;
            public string m_strModuleID;
            public bool m_bUseAlign1AngleLimit;
            public float m_f1stAlignCornerAngleLimit;
            public float m_f1stAlignVerticalAngleLimit;

            public ePermission Permission;
            public ePermission PermissionCheck;
        }
        public static Machine machine;
        public struct StatusTag
        {
            public int MC_MODE;
            public int MC_STATUS;
            public int MC_LIGHT;
        }
        public static StatusTag Status;

        public static bool PLCALIVE = false;
        public static bool RCSCHECK = false;
        public static bool PLC_AUTO_READY = false;
        public static bool MODEL_COPY = false;
        public static string MODEL_COPY_NAME = "";
        public static string MODEL_COPY_INFO = "";

        public class InspectionResult
        {
            public bool[] m_bUnitComp = new bool[DEFINE.AlignUnit_Max];
            public double[] m_dPoint = new double[DEFINE.MeasurePoint_Max];

            public void Clear()
            {
                for (int i = 0; i < DEFINE.AlignUnit_Max; i++)
                    m_bUnitComp[i] = false;

                for (int i = 0; i < DEFINE.MeasurePoint_Max; i++)
                    m_dPoint[i] = 0;
            }
        }

        public static InspectionResult INSP_RESULT = new InspectionResult();

        public class PMResult
        {
            public CogSearchMaxResults CNLSearchResult;// = new CogSearchMaxResults();
            public CogPMAlignResults PMAlignResult = new CogPMAlignResults();
            public CogRectangle SearchRegion = new CogRectangle();

            public double m_RobotPosX, m_RobotPosY;
            public double[] m_Pixel = new double[2];
            public double m_Rotation = new double();
            public double[] m_FixturePixel = new double[2];
            public bool ManuMathchFlag;
            public string m_PatternName;
            public int m_CamNo;
            public double Score;
            public bool m_PMAlign;
            public bool m_PMAlign_Use;

            public double m_ACCeptScore;
            public double m_GACCeptScore;

            public int m_PatNo;
            public int m_Align_No;
            public int m_PatTagNo;
            public bool SearchResult;

            public DoublePoint m_TrayPocketPos = new DoublePoint();

        }
        public static void PMResultCopy(PMResult nDestination, PMResult nSource)
        {
            if (nSource.CNLSearchResult != null)
                nDestination.CNLSearchResult = new CogSearchMaxResults(nSource.CNLSearchResult);
            else
                nDestination.CNLSearchResult = null;

            if (nSource.PMAlignResult != null)
                nDestination.PMAlignResult = new CogPMAlignResults(nSource.PMAlignResult);
            else
                nDestination.PMAlignResult = null;
            
            nDestination.SearchRegion       = new CogRectangle(nSource.SearchRegion);

            nDestination.m_RobotPosX        = nSource.m_RobotPosX;
            nDestination.m_RobotPosY        = nSource.m_RobotPosY;

            nDestination.m_Pixel[Main.DEFINE.X] = nSource.m_Pixel[Main.DEFINE.X];
            nDestination.m_Pixel[Main.DEFINE.Y] = nSource.m_Pixel[Main.DEFINE.Y];

            nDestination.m_Rotation = nSource.m_Rotation;

            nDestination.m_FixturePixel[Main.DEFINE.X] = nSource.m_FixturePixel[Main.DEFINE.X];
            nDestination.m_FixturePixel[Main.DEFINE.Y] = nSource.m_FixturePixel[Main.DEFINE.Y];

            nDestination.ManuMathchFlag     = nSource.ManuMathchFlag;
            nDestination.m_PatternName      = nSource.m_PatternName;
            nDestination.m_CamNo            = nSource.m_CamNo;
            nDestination.Score              = nSource.Score;
            nDestination.m_PMAlign          = nSource.m_PMAlign;
            nDestination.m_PMAlign_Use      = nSource.m_PMAlign_Use;
            nDestination.m_ACCeptScore      = nSource.m_ACCeptScore;
            nDestination.m_GACCeptScore     = nSource.m_GACCeptScore;
            nDestination.m_PatNo            = nSource.m_PatNo;
            nDestination.m_Align_No         = nSource.m_Align_No;
            nDestination.m_PatTagNo         = nSource.m_PatTagNo;
            nDestination.SearchResult       = nSource.SearchResult;
        }

        public struct SystemTime
        {
            public ushort sYear;
            public ushort sMonth;
            public ushort sDayOfWeek;
            public ushort sDay;
            public ushort sHour;
            public ushort sMinute;
            public ushort sSecond;
            public ushort sMilliseconds;
        }

        public struct CommonTag
        {
            public bool nTabCtlChange;
            public int nSelectedTab;
            public int Limit_X;
            public int Limit_Y;
            public int Limit_T;
            public double Limit_Angle;

            public string[] VIEW_NAME;
            public string[] VIEW_Pos;
            public string[] VIEW_Size;
            public int[] VIEW_WIDTH_CNT;

            public string HEAD_MODE;
            public string PBD_TYPE;
        }
        public static CommonTag Common;
        public class BlobTagData
        {
            public bool m_UseCheck;
            public DoublePoint[] m_TargetToCenter = new DoublePoint[Main.DEFINE.M_TOOLMAXCOUNT];
        }
        public class BlobResult
        {
            public CogBlobResults BlobToolResult = new CogBlobResults();
            public CogRectangleAffine SearchRegion = new CogRectangleAffine();
            public double[] Pixel = new double[2];
            public double[,] VertexResults = new double[4, 2];
            public bool m_UseCheck;
            public int m_Index, m_AlignNum, m_PatTagNum, m_PatNum;
            public string m_AlignName;
            public bool SearchResult;
            public int m_PlusMinus = 1;
            //tray
            public int m_TrayBlobNo;
            public DoublePoint TrayPocketPoint = new DoublePoint();
            public double m_Score;
        }

        public class CaliperTagData
        {
            public bool m_UseCheck;
            public DoublePoint[] m_TargetToCenter = new DoublePoint[Main.DEFINE.M_TOOLMAXCOUNT];
            // 210202 ATT
            public bool m_bCOPMode;
            public int m_nCOPROICnt;
            public int m_nCOPROIOffset;
        }
        public class CaliperResult
        {
            public CogCaliperTool CaliperTool =  new CogCaliperTool();
            public CogRectangleAffine SearchRegion = new CogRectangleAffine();
            public double[] Pixel = new double[2];
            public double[,] PixelPosX = new double[2, 2]; //Align 검사용
            public double[,] PixelPosY = new double[2, 2]; //Align 검사용
            public bool m_UseCheck;
            public int m_Index, m_AlignNum, m_PatTagNum, m_PatNum;
            public int m_CALAlignNo, m_CALPatTagNo, m_CALPatNo;
            public bool SearchResult;
            public int m_PlusMinus = 1;
            public double RobotWidth;
        }

        public class FindLineResult
        {
            public CogIntersectLineLineTool IntersectTool = new CogIntersectLineLineTool();
            public CogFindLineTool FindLineTool = new CogFindLineTool();
            public CogLineMaxTool LineMaxTool = new CogLineMaxTool();

            public DoublePoint[] DrawPocketPoint2 = new DoublePoint[Main.DEFINE.TRAY_POCKET_LIMIT];
            public double[] Pixel = new double[2];
            
            public double RectangleAngle = new double();
            public bool CenterSearch = new bool();

            public List<DoublePoint> CrossPointList = new List<DoublePoint>();
            public Main.DoublePoint CenterPixel = new Main.DoublePoint();
            
            public bool ManuMatchComplete; // ManualMatchingFlag
            public bool m_UseCheck;
            public bool SearchResult;
            public bool m_UsedLineMax;
            public int m_LineMaxSelIdx;
            public bool m_bInterSearched;
            public bool m_bInterResult;
            public int m_Index, m_AlignNum, m_PatTagNum, m_PatNum;
            public int m_FoundSubLineNum;

            public bool m_bLoadingLimitOver_X;
            public bool m_bLoadingLimitOver_Y;
            public bool m_bAngleLimit;
        }
        public class FindLineTagData
        {
            public bool m_UseCheck;
            public bool m_UsePairCheck;
            public ushort m_LinePosition;
            public DoublePoint[] m_TargetToCenter = new DoublePoint[Main.DEFINE.M_TOOLMAXCOUNT];
            public int m_LineMaxHCond;
            public int m_LineMaxVCond;
            public int m_LineCaliperMethod;
        }
        public class TrayBlobResult
        {
            public BlobResult TrayBlob = new BlobResult();
            public bool SearchBlobResult;
        }
        public class DoublePoint
        {
            public double X;
            public double Y;
            public double T;

            public double X2;
            public double Y2;
            public double T2;
            public DoublePoint()
            {
                this.X = 0;
                this.Y = 0;
                this.T = 0;

                this.X2 = 0;
                this.Y2 = 0;
                this.T2 = 0;
            }
        }
        public class FindCircleTagData
        {
            public bool m_UseCheck;
            public DoublePoint[] m_TargetToCenter = new DoublePoint[Main.DEFINE.M_TOOLMAXCOUNT];
            public int m_CircleCaliperMethod;
        }
        public class CircleResult
        {
            public CogFindCircleTool CircleTool = new CogFindCircleTool();
            public double[] Pixel = new double[2];
            public double R;
            public double m_RobotPosX, m_RobotPosY, m_RobotRadius;
            public bool m_UseCheck;
            public bool SearchResult;
            public int m_Index, m_AlignNum, m_PatTagNum, m_PatNum;
        }

        public class FindCornerTagData
        {

        }
        public class CornerResult
        {
            public CogFindCornerTool CornerTool = new CogFindCornerTool();
            public double[] Pixel = new double[2];
            public double m_RobotPosX, m_RobotPosY;
            public bool m_UseCheck;
            public bool SearchResult;
            public int m_Index, m_AlignNum, m_PatTagNum, m_PatNum;
        }
        public class DAlignData
        {
            public List<int> NUM = new List<int>();
            public List<string> CELLID = new List<string>();
            public List<double[,]> nLength = new List<double[,]>();

            public List<double> nLengthWidth = new List<double>();
            public List<double> nLengthHeight = new List<double>();

            public List<string> m_Message = new List<string>();
            public List<bool> Result_sts = new List<bool>();

            public void RemoveAt()
            {

                try
                {
                    if (NUM.Count > 0)
                    {
                        NUM.RemoveAt(0);
                        CELLID.RemoveAt(0);
                        nLength.RemoveAt(0);

                         nLengthWidth.RemoveAt(0);
                         nLengthHeight.RemoveAt(0);

                        m_Message.RemoveAt(0);
                        Result_sts.RemoveAt(0);
                    }
                }
                catch
                {

                }

            }
            public void Clear()
            {
                try
                {
                    if (NUM.Count > 0)
                    {
                        NUM.Clear();
                        CELLID.Clear();
                        nLength.Clear();
                        nLengthWidth.Clear();
                        nLengthHeight.Clear();
                        m_Message.Clear();
                        Result_sts.Clear();
                    }
                }
                catch
                {

                }

            }
        }
        //------------------------------------------------------------------------CAM
        public static Form_ProgressBar formProgressBar = new Form_ProgressBar();
        public static Form_ProgressBar_Sub formProgressBar_1 = new Form_ProgressBar_Sub();    
        //-----------------------------------------------------------------------------------------------------------------------------------
        delegate void dProgerssBar_Unit(Form_ProgressBar nformx, int nMaxValue, bool nSelect, int nValue);
        public static void ProgerssBar_Unit(Form_ProgressBar nform, int nMaxValue, bool nSelect, int nValue)
        {
            if (nform.InvokeRequired)
            {
                dProgerssBar_Unit call = new dProgerssBar_Unit(ProgerssBar_Unit);
                nform.Invoke(call, nform, nMaxValue, nSelect, nValue);
            }
            else
            {
                if(nSelect)
                {
                    if(nValue == 0)
                    {
                        nform.nMessage = "Unit";
                        nform.nMaximum = nMaxValue;
                        nform.Show();
                        nform.Form_ProgressMaxSet();
                    }
                    else
                    {
                        nform.progressBar1.Value = nValue;
                    }
                }
                else
                {
                    nform.Hide();
                }
            }
        }

        delegate void dProgerssBar_PAT(Form_ProgressBar_Sub nformx, int nMaxValue, bool nSelect, int nValue, string nMessage);
        public static void ProgerssBar_PAT(Form_ProgressBar_Sub nform, int nMaxValue, bool nSelect, int nValue , string nMessage)
        {
            if (nform.InvokeRequired)
            {
                dProgerssBar_PAT call = new dProgerssBar_PAT(ProgerssBar_PAT);
                nform.Invoke(call, nform, nMaxValue, nSelect, nValue , nMessage);
            }
            else
            {
                if (nSelect)
                {
                    if (nValue == 0)
                    {
                        nform.nMaximum = nMaxValue;
                        nform.nMessage = nMessage;
                        nform.Show();
                        nform.Form_ProgressMaxSet();
                    }
                    else
                    {
                        nform.progressBar2.Value = nValue;
                    }
                }
                else
                {
                    nform.Hide();
                }
            }
        }

#region Caliper
        public static double SetCaliperDirection(int Mode)
        {
            double TempReturn = 0;
                switch (Mode)
            {
                case DEFINE.LEFT_TO_RIGHT:
                        TempReturn = 0 * Main.DEFINE.radian;    //                 0.00000000000000 
                    break;

                case DEFINE.TOP_TO_BOTTOM:
                        TempReturn = 90 * Main.DEFINE.radian;   //                 1.57079632679490
                    break;

                case DEFINE.RIGHT_TO_LEFT:
                        TempReturn = -180 * Main.DEFINE.radian; //                 -3.14159265358979
                    break;

                case DEFINE.BOTTOM_TO_TOP:
                        TempReturn = -90 * Main.DEFINE.radian;  //                 -1.57079632679490
                    break;                                
            }
           return TempReturn;
        }
        public static int GetCaliperDirection(double Rotation)
        {
            int  TempReturn =0;
            int mode;
            mode = (int)(Rotation * Main.DEFINE.degree);
            switch (mode)
            {
                case 0://                 0.00000000000000 
                    TempReturn = DEFINE.LEFT_TO_RIGHT;
                    break;

                case 90://                 1.57079632679490 
                    TempReturn = DEFINE.TOP_TO_BOTTOM;
                    break;

                case -180://                 -3.14159265358979
                    TempReturn = DEFINE.RIGHT_TO_LEFT;
                    break;

                case -90: //                 -1.57079632679490
                    TempReturn = DEFINE.BOTTOM_TO_TOP;
                    break;

                default:
                    TempReturn = DEFINE.LEFT_TO_RIGHT;
                    break;
            }
            return TempReturn;
        }
        public static int GetCaliperDirection(int Rotation)
        {
            int TempReturn = 0;

            switch (Rotation)
            {
                case DEFINE.LEFT_TO_RIGHT:
                case DEFINE.RIGHT_TO_LEFT:
                    TempReturn = DEFINE.X;
                    break;

                case DEFINE.BOTTOM_TO_TOP:
                case DEFINE.TOP_TO_BOTTOM:
                    TempReturn = DEFINE.Y;
                    break;
                default:
                    TempReturn = DEFINE.X;
                    break;
            }
            return TempReturn;
        }
        public static bool GetCaliperPairMode(Cognex.VisionPro.Caliper.CogCaliperEdgeModeConstants TempMode)
        {
            bool TempReturn = false;
            switch (TempMode)
            {
                case Cognex.VisionPro.Caliper.CogCaliperEdgeModeConstants.Pair:
                    TempReturn = true;
                    break;
                default:
                    TempReturn = false;
                    break;
            }
            return TempReturn;
        }
        public static void SetCaliperPairPolarity(CogCaliperTool nCaliper)
        {

            switch (nCaliper.RunParams.Edge0Polarity)
            {
                case CogCaliperPolarityConstants.DarkToLight:
                    nCaliper.RunParams.Edge1Polarity = CogCaliperPolarityConstants.LightToDark;
                    break;
                case CogCaliperPolarityConstants.LightToDark:
                    nCaliper.RunParams.Edge1Polarity = CogCaliperPolarityConstants.DarkToLight;
                    break;
                default:
                    nCaliper.RunParams.Edge1Polarity = CogCaliperPolarityConstants.DontCare;
                    break;
            }

              nCaliper.RunParams.TwoEdgeScorers.Clear();
//            CogCaliperScorerPositionNeg nItem = new CogCaliperScorerPositionNeg();
//            nCaliper.RunParams.TwoEdgeScorers.Add(nItem);
              CogCaliperScorerContrast nItem1 = new CogCaliperScorerContrast();
              nCaliper.RunParams.TwoEdgeScorers.Add(nItem1);
              CogCaliperScorerSizeDiffNorm nItem2 = new CogCaliperScorerSizeDiffNorm();
              nItem2.SetXYParameters(0, 100, 150, 1, 0);
              nCaliper.RunParams.TwoEdgeScorers.Add(nItem2);
        }
#endregion

         //public static Mutex Mutex_lock_CaliperTool = new Mutex();
         //public static Mutex Mutex_lock_BlobTool = new Mutex();
         //public static Mutex Mutex_lock_CircleTool = new Mutex();

         public static void MatrixNotUse(int[] nUnit)
         {
             for (int k = 0; k < nUnit.Length; k++)
             {
                 for (int i = 0; i < Main.AlignUnit[nUnit[k]].m_AlignPatTagMax; i++)
                 {
                     for (int j = 0; j < Main.AlignUnit[nUnit[k]].m_AlignPatMax[i]; j++)
                     {
                         Main.AlignUnit[nUnit[k]].PAT[i, j].CALMATRIX_NOTUSE = true;
                     }
                 }
             }
         }

         public static void MotorResol(int[] nUnit)  //모터 제어 방식 1um Or 0.1um //사용시 포함
         {
             for (int i = 0; i < nUnit.Length; i++)
                 Main.AlignUnit[nUnit[i]].nMotResol = 10;
         }
         public static void ACFCUTMODE(int[] nUnit)  
         {
             for (int i = 0; i < nUnit.Length; i++)
                 Main.AlignUnit[nUnit[i]].M_ACFCUT_MODE = true;
         }

     public static void SearchDelay(int nAlignDelay)
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
                    if (m_Timer.GetElapsedTime() >= nAlignDelay)
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

        public static PLC PLCsocket = new PLC();
 
    } //class Main
    public class SEQ
    {
        public const int INIT_SEQ = 0;
        public const int X_CAL_SEQ = 100;
        public const int Y_CAL_SEQ = 200;
        public const int T1_CAL_SEQ = 300;
        public const int T2_CAL_SEQ = 400;
        public const int T3_CAL_SEQ = 500;

        public const int OK_SEQ = 100;
        public const int NG_SEQ = 200;

        public const int ERROR_SEQ = 900;
        public const int COMPLET_SEQ = 1000;
        public const int COMPLET_XY = 999;
        public const int TIME_OUT = 2000;

        public const int SEARCH_SEQ = 50;
        public const int RESEARCH_SEQ = 60;
        public const int MANUAL_SEQ = 70;
        public const int GRAB_SEQ = 80;
    }
    public class CMD
    {
        public const int TAR_SEARCH_LEFT                    = 1001;
        public const int TAR_SEARCH_RIGHT                   = 1002;
        public const int TAR_SEARCH_ALL                     = 1003;

        public const int OBJ_SEARCH_LEFT                    = 1011;
        public const int OBJ_SEARCH_RIGHT                   = 1012;
        public const int OBJ_SEARCH_ALL                     = 1013;

        public const int LEFT_SEARCH_ALL                    = 1014;
        public const int RIGHT_SEARCH_ALL                   = 1015;
        public const int OBJTAR_SEARCH_ALL                  = 1016;

        public const int GRAB_ALL                           = 1018;
        public const int GRAB_LEFT                          = 1019;
        public const int GRAB_RIGHT                         = 1020;

        public const int ALIGN_OBJECT                       = 1031;
        public const int ALIGN_OBJECT_REALIGN               = 1032;

        public const int ALIGN_CENTER                       = 1033;
        public const int ALIGN_CENTER_REALIGN               = 1034;

        public const int ALIGN_TARGET                       = 1035;
        public const int ALIGN_TARGET_REALIGN               = 1036;

        public const int ALIGN_OBJTAR_ALL                   = 1037;
        public const int ALIGN_OBJTAR_ALL_REALIGN           = 1038;

        public const int BLOB_TRAY_SEARCH                   = 1049;
        public const int BLOB_TRAY_TRIGER_PICKER            = 1050;

        public const int ALIGN_TRAY_SEARCH                  = 1085;
        public const int ALIGN_TRAY_TRIGER                  = 1040;

        public const int ALIGN_1CAM_2SHOT_LEFT              = 1043;
        public const int ALIGN_1CAM_2SHOT_RIGHT             = 1044;

        public const int ALIGN_1CAM_4SHOT_LEFT              = 1053;
        public const int ALIGN_1CAM_4SHOT_RIGHT             = 1054;
        public const int ALIGN_1CAM_4SHOT_TAR_LEFT          = 1055;
        public const int ALIGN_1CAM_4SHOT_TAR_RIGHT         = 1056;
        public const int REEL_ALIGN                         = 1061;

        public const int CRD_ALIGN_LEFT                     = 1071;
        public const int CRD_ALIGN_RIGHT                    = 1072;

        public const int REQ_INSPECTION                     = 1100;

        public const int ACF_BLOB_TAR_LEFT                  = 1158;
        public const int ACF_BLOB_TAR_RIGHT                 = 1159;
        public const int ACF_BLOB_TAR_ALL                   = 1160;

        public const int ACF_BLOB_LEFT                      = 1161;
        public const int ACF_BLOB_RIGHT                     = 1162;
        public const int ACF_BLOB_OBJ_ALL                   = 1163;

        public const int CALIPER_SEARCH_LEFT                = 1165;
        public const int CALIPER_SEARCH_LEFT_ACFCUT = 1065;
        public const int CALIPER_SEARCH_RIGHT               = 1166;
        public const int CALIPER_SEARCH_RIGHT_ACFCUT        = 1066;
        public const int CALIPER_OBJ_SEARCH_ALL             = 1164;

        public const int CALIPER_ALIGN_1CAM_2SHOT_LEFT      = 1167;
        public const int CALIPER_ALIGN_1CAM_2SHOT_RIGHT     = 1168;
        public const int CALIPER_ALIGN_CENTER               = 1169;

        public const int DOPO_INSPECT_LEFT                  = 1171;
        public const int DOPO_INSPECT_RIGHT                 = 1172;

        public const int CIRCLE_OBJ_SEARCH_LEFT             = 1174;
        public const int CIRCLE_OBJ_SEARCH_RIGHT            = 1175;
        public const int CIRCLE_OBJ_SEARCH_ALL              = 1176;

        public const int CIRCLE_TAR_SEARCH_LEFT             = 1177;
        public const int CIRCLE_TAR_SEARCH_RIGHT            = 1178;
        public const int CIRCLE_TAR_SEARCH_ALL              = 1179;

        public const int FINDLINE_OBJ_LEFT                  = 1181;
        public const int FINDLINE_OBJ_RIGHT                 = 1182;
        public const int FINDLINE_OBJ_ALL                   = 1183;

        public const int ALIGN_INSPECT_LEFT                 = 1261;
        public const int ALIGN_INSPECT_RIGHT                = 1262;

        public const int ALIGN_CIRCLE_INSPECT_LEFT          = 1274;
        public const int ALIGN_CIRCLE_INSPECT_RIGHT         = 1275;

        public const int OBJ_CALRIBRATION_LEFT              = 1091;
        public const int OBJ_CALRIBRATION_RIGHT             = 1092;
        public const int TAR_CALRIBRATION_LEFT              = 1093;
        public const int TAR_CALRIBRATION_RIGHT             = 1094;
        public const int HEAD_CALRIBRATION                  = 1095;

        public const int OBJ_CAL_POS_LEFT                   = 1195;
        public const int OBJ_CAL_POS_RIGHT                  = 1196;
    }
    public class ERR
    {
        public const int SUCCESS = 0;
        public const int LENTH = 20;
        public const int INSPEC = 30;
        public const int E_3POINT = 40;
        public const int INTERSECTANGLE = 41;
        public const int PLC_MOVE_END = 42;   //50
        public const int MOTER_LIMIT = 43;    //60
        public const int ALIGN_REPEAT_LIMIT = 44;    //60
    }
    public partial class PLCDataTag
    {
        public static int VisionStatusSize = 100;
        public static int PLCStatusSize = 300;
        public static int ProcessSize = 100;
        public static int ReadSize = 310;//500;
        public static bool Readflag = false;
        public static int[] BData = new int[ReadSize];
        public static Int16[] RData = new Int16[ReadSize];
        public static int[] SData = new int[PLCStatusSize];
        //public static int[] PData = new int[ProcessSize];
    }


}// namespace COG
