//-------------------------SDT, IT TABLET (OLB+COP+COG)
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//#define OLB_ACF_BONDER_PC1_MODE // ACF PRE 1, ACF CUT 1, ACF BLOB 1, USC+PLASMA PRE
//#define OLB_ACF_BONDER_PC2_MODE // ACF PRE 2, ACF CUT 2, ACF BLOB 2
//#define OLB_PRE_BONDER_PC3_MODE // PBD/BACKUP INSPECT 1, PBD_STAGE 1, PBD_TOOLTIP 1, PBD/BACKUP INSPECT 2, PBD_STAGE 2, PBD_TOOLTIP 2
//#define OLB_COF_LOADER_PC4_MODE // REEL ALIGN 1, REEL ALIGN 2, COF ALIGN+CUTTING 1, COF ALIGN 1, I/C CHIP PRE ALIGN 1
//#define OLB_COF_LOADER_PC5_MODE // REEL ALIGN 3, REEL ALIGN 4, COF ALIGN+CUTTING 2, COF ALIGN 2, I/C CHIP PRE ALIGN 2
//#define OLB_FINAL_BONDER_PC6_MODE // FBD ALIGN 1, FBD 석영이물검사 1, FBD ALIGN 2, FBD 석영이물검사 2
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------SDT, IT TABLET (FOF+FOP+FOG)
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//#define FOF_FOP_ACF_BONDER_PC1_MODE // FOP ACF PRE 1, FOP ACF CUT 1, FOP ACF BLOB 1, PANEL PAD CLEANER
//#define FOF_FOP_ACF_BONDER_PC2_MODE // FOP ACF PRE 2, FOP ACF CUT 2, FOP ACF BLOB 2, FOP INSPECTION
//#define FOF_PRE_BONDER_PC3_MODE // FOP &FOF PBD(Align & 이물검사) 1, FPC ALIGN * BACKUP INSEPCTION 1, FOF INSPECTION 1
//#define FOF_PRE_BONDER_PC4_MODE // FOP &FOF PBD(Align & 이물검사) 2, FPC ALIGN * BACKUP INSEPCTION 2, FOF INSPECTION 2
//#define FOF_FINAL_BONDER_PC5_MODE // FBD ALIGN 1, FBD 석영이물검사 1, FBD ALIGN 2, FBD 석영이물검사 2
//#define FOF_FPC_LOADER_PC6_MODE // FPC TRAY ALIGN 1, FPC CLEANER 1, FPC CLEARNR 2, FPC TRAY ALIGN 2, FPC CLEARNER 3, FPC CLEANER 4
//#define FOF_FOF_ACF_BONDER_PC7_MODE // FOF ACF PRE ALIGN 1-1, FOF ACF BLOB+INSPECTION 1-1, FOF ACF CUT 1, FOF ACF PRE ALIGN 1-2, FOF ACF BLOB+INSPECTION 1-2
//#define FOF_FOF_ACF_BONDER_PC8_MODE // FOF ACF PRE ALIGN 2-1, FOF ACF BLOB+INSPECTION 2-1, FOF ACF CUT 2, FOF ACF PRE ALIGN 2-2, FOF ACF BLOB+INSPECTION 2-2
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------T-FOG
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//#define TFOG_FOP_ACF_BONDER_PC1_MODE // PANEL PAD CLEANER, FOP ACF PRE ALIGN 1, ACF BLOB1+BACKUP INSPECTION, FOP ACF CUT ALIGN,//
//#define TFOG_PRE_BONDER_PC2_MODE // PBD PANEL ALIGN, PBD FPC ALIGN, FPC PRE ALIGN, FPC_TRAY
//#define TFOG_FINAL_BONDER_PC3_MODE // FBD ALIGN1, FBD 석영이물검사1, FBD ALIGN2, FBD 석영이물검사2
//#define TFOG_ALIGN_INSPECT_PC4_MODE // FBD 목시용, BUFFER PRE ALIGN
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------FOB
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//#define FOB_PANEL_ALIGN_PC1_MODE // PANEL ALIGN
//#define FOB_FINAL_BONDER_PC2_MODE // FBD ALIGN 1, FBD 석영이물검사 1, FPC ALIGN 1
//#define FOB_FINAL_BONDER_PC3_MODE // FBD ALIGN 2, FBD 석영이물검사 2, FPC ALIGN 2
//#define FOB_INSPECT_PC4_MODE // A/I PRE ALIGN, FOB ALIGN INSPECT, ART PRE ALIGN ,ART PROBE ALIGN, MARKER PRE ALIGN
//#define FOB_ACF_BONDER_PC5_MODE // FOF ACF PRE ALIGN 1, FOF ACF CUT ALIGN 1, FOF ACF BLOB+INSPECTION 1, FPC TRAY ALIGN 1, PCB CLEANER 1
//#define FOB_ACF_BONDER_PC6_MODE // FOF ACF PRE ALIGN 2, FOF ACF CUT ALIGN 2, FOF ACF BLOB+INSPECTION 2, FPC TRAY ALIGN 2, PCB CLEANER 2
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//-------------------------CRD 
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//#define ENCAP_CRD_PC1_MODE // PANEL ALIGN, CRD PRE ALIGN & 도포검사 1, CRD PRE ALIGN & 도포검사 2, CRD PRE ALIGN & 도포검사 3, CRD PRE ALIGN & 도포검사 4, THICKNESS PRE ALIGN
//#define CRD_TUFFY_PC1_MODE // PANEL ALIGN, CRD PRE ALIGN & 도포검사 1, CRD PRE ALIGN & 도포검사 2, CRD PRE ALIGN & 도포검사 3, CRD PRE ALIGN & 도포검사 4, THICKNESS PRE ALIGN
//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//#define BP_INSPECTION_PC1_MODE
//#define BP_INSPECTION_PC2_MODE
//-------------------------ATT
#define ATT_AREA_PC1_MODE    // PRE ALIGN 1-1, PRE ALIGN 1-2, ATT 1-1, ATT 1-2, PRE ALIGN 2-1, PRE ALIGN 2-2, ATT 2-1, ATT 2-2

#define SDBIO_VENT  //전면
//#define SDBIO_PATH  //후면
//#define ATT_AREA_PC2_MODE    // PRE ALIGN 1-1, PRE ALIGN 1-2, ATT 1-1, ATT 1-2
//#define ATT_LINE_PC1_MODE    // PRE ALIGN 1-1, PRE ALIGN 1-2, ATT 1-1, ATT 1-2

//#define NON_STANDARD //대칭
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

using System.Runtime.Serialization.Formatters.Binary;

namespace COG
{
    #region BP_INSPECTION
#if BP_INSPECTION_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "QD_LPA_PC1";
            public const string UNIT_TYPE = "2755";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int MeasurePoint_Max = 8;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 2;
            public const int MIL_CAM_PER_BOARD = 2;
            public const int MIL_CAM_MAX = MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 4;

            public const int CAM_MAX = MIL_CAM_MAX + GIGE_CAM_MAX;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.123.11";

            public const int ReadLocalPort = 20002;
            public const int WriteLocalPort = 20003;

            public const int ReadRemotePor = 20000;
            public const int WriteRemotePort = 20001;
            //public const string RemoteIP = "192.168.123.10";

            //public const int ReadLocalPort = 12290;
            //public const int WriteLocalPort = 12291;

            //public const int ReadRemotePor = 12288;
            //public const int WriteRemotePort = 12289;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 320000; //PLC BASE ADDRESS
        public static int BASE_MAIN_PC_RW_ADDR = 300000; //LASER PC BASE ADDRESS
    }
#endif
#if BP_INSPECTION_PC2_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "QD_LPA_PC2";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 2;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int MeasurePoint_Max = 0;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 0;
            public const int MIL_CAM_PER_BOARD = 0;
            public const int MIL_CAM_MAX = MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 3;

            public const int CAM_MAX = MIL_CAM_MAX + GIGE_CAM_MAX;
            public const int DISPLAY_MAX = 3;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.123.11";

            public const int ReadLocalPort = 20102;
            public const int WriteLocalPort = 20103;

            public const int ReadRemotePor = 20100;
            public const int WriteRemotePort = 20101;
            //public const string RemoteIP = "192.168.123.10";

            //public const int ReadLocalPort = 12290;
            //public const int WriteLocalPort = 12291;

            //public const int ReadRemotePor = 12288;
            //public const int WriteRemotePort = 12289;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 340000; //PLC BASE ADDRESS
        public static int BASE_MAIN_PC_RW_ADDR = 300000; //LASER PC BASE ADDRESS
    }
#endif
    #endregion
    #region OLB+COP+COG
    #region OLB_ACF_BONDER_PC1_MODE
#if OLB_ACF_BONDER_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "OLB_PC1";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 2;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 8;
            public const int MeasurePoint_Max = 0;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 1;
            public const int MIL_CAM_PER_BOARD = 1;
            public const int MIL_CAM_MAX = 0;// MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 2;

            //public const int CAM_MAX = MIL_CAM_MAX + GIGE_CAM_MAX;
            //public const int DISPLAY_MAX = 4;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePort = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region OLB_ACF_BONDER_PC2_MODE
#if OLB_ACF_BONDER_PC2_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "OLB_PC2";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 3;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 3;
            public const int DISPLAY_MAX = 6;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 31000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region OLB_PRE_BONDER_PC3_MODE
#if OLB_PRE_BONDER_PC3_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "OLB_PC3";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 6;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 6;
            public const int DISPLAY_MAX = 6;

            public const int MeasurePoint_Max = 0;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 1;
            public const int MIL_CAM_PER_BOARD = 1;
            public const int MIL_CAM_MAX = 0;// MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 2;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePort = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region OLB_COF_LOADER_PC4_MODE
#if OLB_COF_LOADER_PC4_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "OLB_PC4";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 31000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region OLB_COF_LOADER_PC5_MODE
#if OLB_COF_LOADER_PC5_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "OLB_PC5";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9015;
            public const int WriteLocalPort = 9016;

            public const int ReadRemotePor = 9005;
            public const int WriteRemotePort = 9006;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 32000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region OLB_FINAL_BONDER_PC6_MODE
#if OLB_FINAL_BONDER_PC6_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "OLB_PC6";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 8;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9015;
            public const int WriteLocalPort = 9016;

            public const int ReadRemotePor = 9005;
            public const int WriteRemotePort = 9006;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 32000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #endregion
    #region FOF+FOP+FOG
    #region FOF_FOP_ACF_BONDER_PC1_MODE
#if FOF_FOP_ACF_BONDER_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC1";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 4;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_FOP_ACF_BONDER_PC2_MODE
#if FOF_FOP_ACF_BONDER_PC2_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC2";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 4;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 31000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_PRE_BONDER_PC3_MODE
#if FOF_PRE_BONDER_PC3_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC3";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 2;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 6;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_PRE_BONDER_PC4_MODE
#if FOF_PRE_BONDER_PC4_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC4";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 2;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 6;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 31000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_FINAL_BONDER_PC5_MODE
#if FOF_FINAL_BONDER_PC5_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC5";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 8;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9015;
            public const int WriteLocalPort = 9016;

            public const int ReadRemotePor = 9005;
            public const int WriteRemotePort = 9006;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 32000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_FPC_LOADER_PC6_MODE
#if FOF_FPC_LOADER_PC6_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC6";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 6;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 6;
            public const int DISPLAY_MAX = 10;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9015;
            public const int WriteLocalPort = 9016;

            public const int ReadRemotePor = 9005;
            public const int WriteRemotePort = 9006;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 32000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_FOF_ACF_BONDER_PC7_MODE
#if FOF_FOF_ACF_BONDER_PC7_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC7";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 10;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9017;
            public const int WriteLocalPort = 9018;

            public const int ReadRemotePor = 9007;
            public const int WriteRemotePort = 9008;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 33000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOF_FOF_ACF_BONDER_PC8_MODE
#if FOF_FOF_ACF_BONDER_PC8_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOF_PC8";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 10;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9019;
            public const int WriteLocalPort = 9020;

            public const int ReadRemotePor = 9009;
            public const int WriteRemotePort = 9010;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 34000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #endregion
    #region ENCAP CRD
    #region ENCAP_CRD_PC1_MODE
#if ENCAP_CRD_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "ENCAP_CRD_PC1";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 6;
            public const int PATTERNTAG_MAX = 2;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 6;
            public const int DISPLAY_MAX = 12;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #endregion
    #region T-FOG
    #region TFOG_FOP_ACF_BONDER_PC1_MODE
#if TFOG_FOP_ACF_BONDER_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "TFOG_PC1";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 4;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 100000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region TFOG_PRE_BONDER_PC2_MODE
#if TFOG_PRE_BONDER_PC2_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "TFOG_PC2";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 7;
            public const int DISPLAY_MAX = 7;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 101000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region TFOG_FINAL_BONDER_PC3_MODE
#if TFOG_FINAL_BONDER_PC3_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "TFOG_PC3";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 8;
            public const int DISPLAY_MAX = 8;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9015;
            public const int WriteLocalPort = 9016;

            public const int ReadRemotePor = 9005;
            public const int WriteRemotePort = 9006;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 102000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region TFOG_ALIGN_INSPECT_PC4_MODE
#if TFOG_ALIGN_INSPECT_PC4_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "TFOG_PC4";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 2;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 2;
            public const int DISPLAY_MAX = 4;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9017;
            public const int WriteLocalPort = 9018;

            public const int ReadRemotePor = 9007;
            public const int WriteRemotePort = 9008;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 103000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #endregion
    #region FOB
    #region FOB_PANEL_ALIGN_PC1_MODE
#if FOB_PANEL_ALIGN_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOB_PC1";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 1;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 1;
            public const int DISPLAY_MAX = 2;
            
            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOB_FINAL_BONDER_PC2_MODE
#if FOB_FINAL_BONDER_PC2_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOB_PC2";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 3;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 6;
            public const int DISPLAY_MAX = 6;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 14000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOB_FINAL_BONDER_PC3_MODE
#if FOB_FINAL_BONDER_PC3_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOB_PC3";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 3;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 6;
            public const int DISPLAY_MAX = 6;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 15000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOB_INSPECT_PC4_MODE
#if FOB_INSPECT_PC4_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOB_PC4";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 2;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 6;
            public const int DISPLAY_MAX = 9;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;
            
            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9013;
            public const int WriteLocalPort = 9014;

            public const int ReadRemotePor = 9003;
            public const int WriteRemotePort = 9004;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 31000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOB_ACF_BONDER_PC5_MODE
#if FOB_ACF_BONDER_PC5_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOB_PC5";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 9;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9015;
            public const int WriteLocalPort = 9016;

            public const int ReadRemotePor = 9005;
            public const int WriteRemotePort = 9006;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 16000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #region FOB_ACF_BONDER_PC6_MODE
#if FOB_ACF_BONDER_PC6_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "FOB_PC6";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 5;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 4;
            public const int CAM_MAX = 5;
            public const int DISPLAY_MAX = 9;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9017;
            public const int WriteLocalPort = 9018;

            public const int ReadRemotePor = 9007;
            public const int WriteRemotePort = 9008;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 17000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #endregion
    #region CRD TUFFY
    #region CRD_TUFFY_PC1_MODE
#if CRD_TUFFY_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "CRD_TUFFY_PC1";
            public const string UNIT_TYPE = "";
            public const int AlignUnit_Max = 8;
            public const int PATTERNTAG_MAX = 2;
            public const int Pattern_Max = 2;
            public const int CAM_MAX = 7;
            public const int DISPLAY_MAX = 16;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.1";

            public const int ReadLocalPort = 9011;
            public const int WriteLocalPort = 9012;

            public const int ReadRemotePor = 9001;
            public const int WriteRemotePort = 9002;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 30000; //PLC BASE ADDRESS
    }
#endif
    #endregion
    #endregion
    #region ATT
#if ATT_AREA_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "ATT_AREA_PC1";
            //public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 2;
            public const int PATTERNTAG_MAX = 2; //stage
            public const int Pattern_Max = 2;    //left right
            public const int MeasurePoint_Max = 0;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 1;
            public const int MIL_CAM_PER_BOARD = 1;
            public const int MIL_CAM_MAX = 0;// MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 2;

            public const int CAM_MAX = MIL_CAM_MAX + GIGE_CAM_MAX;
            public const int DISPLAY_MAX = 4;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 1;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.130.120";



#if SDBIO_VENT
            public const string UNIT_TYPE = "VENT";
            public const int ReadLocalPort = 5013; // vent - 5013, path - 5015
            public const int WriteLocalPort = 5012; // vent - 5012, path - 5014
            public const int ReadRemotePort = 5013; // vent - 5013, path - 5015
            public const int WriteRemotePort = 5012; // vent = 5012, path - 5014
#endif
#if SDBIO_PATH
            public const string UNIT_TYPE = "PATH";
            public const int ReadLocalPort = 5015; // vent - 5013, path - 5015
            public const int WriteLocalPort = 5014; // vent - 5012, path - 5014
            public const int ReadRemotePort = 5015; // vent - 5013, path - 5015
            public const int WriteRemotePort = 5014; // vent = 5012, path - 5014
#endif

            //public const string RemoteIP = "192.168.123.10";64

            //public const int ReadLocalPort = 12290;\
            //public const int WriteLocalPort = 12291;

            //public const int ReadRemotePor = 12288;
            //public const int WriteRemotePort = 12289;
            public static BindingList<T> CloneBindingList<T>(BindingList<T> oldList)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, oldList);
                stream.Position = 0;
                return (BindingList<T>)formatter.Deserialize(stream);
            }
        }
    }
    public partial class PLCDataTag
    {
#if SDBIO_VENT
        public static int BASE_RW_ADDR = 27000; //PLC BASE ADDRESS vent - 27000, path - 28000
        public static int BASE_MAIN_PC_RW_ADDR = 27000; //LASER PC BASE ADDRESS vent - 27000, path - 28000
#endif
#if SDBIO_PATH
        public static int BASE_RW_ADDR = 28000; //PLC BASE ADDRESS vent - 27000, path - 28000
        public static int BASE_MAIN_PC_RW_ADDR = 28000; //LASER PC BASE ADDRESS vent - 27000, path - 28000
#endif

    }
#endif
#if ATT_AREA_PC2_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "ATT_AREA_PC2";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 3;
            public const int MeasurePoint_Max = 0;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 1;
            public const int MIL_CAM_PER_BOARD = 1;
            public const int MIL_CAM_MAX = MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 2;

            public const int CAM_MAX = MIL_CAM_MAX + GIGE_CAM_MAX;
            public const int DISPLAY_MAX = 10;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.123.11";

            public const int ReadLocalPort = 20002;
            public const int WriteLocalPort = 20003;

            public const int ReadRemotePor = 20000;
            public const int WriteRemotePort = 20001;
            //public const string RemoteIP = "192.168.123.10";

            //public const int ReadLocalPort = 12290;
            //public const int WriteLocalPort = 12291;

            //public const int ReadRemotePor = 12288;
            //public const int WriteRemotePort = 12289;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 320000; //PLC BASE ADDRESS
        public static int BASE_MAIN_PC_RW_ADDR = 300000; //LASER PC BASE ADDRESS
    }
#endif
#if ATT_LINE_PC1_MODE
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const string PROGRAM_TYPE = "ATT_LINE_PC1";
            public const string UNIT_TYPE = "";

            public const int AlignUnit_Max = 4;
            public const int PATTERNTAG_MAX = 1;
            public const int Pattern_Max = 3;
            public const int MeasurePoint_Max = 0;

            // 200611 MIL CAM 추가
            public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_2tap_8bit_85MHz_c.dcf";
            //public const string DCFPath = SYS_DATADIR + "DCF\\" + "sol_m9u43_XCL-SG1240_3tap_8bit_85MHz_c.dcf";
            public const int MIL_BOARD_MAX = 1;
            public const int MIL_CAM_PER_BOARD = 1;
            public const int MIL_CAM_MAX = MIL_BOARD_MAX * MIL_CAM_PER_BOARD;
            public const int GIGE_CAM_MAX = 1;

            public const int CAM_MAX = MIL_CAM_MAX + GIGE_CAM_MAX;
            public const int DISPLAY_MAX = 4;

            //---------------------BLOB 관련-----------------------------
            public const int BLOB_CNT_MAX = 10;

            public const int Light_Control_Max = 2;  // 조명 컨트럴의 최대갯수
            public const int Light_PatMaxCount = 4; //패턴 별 조명 최대갯수

            public const int nCopyNum = 0;
            public const string RemoteIP = "192.168.123.11";

            public const int ReadLocalPort = 20002;
            public const int WriteLocalPort = 20003;

            public const int ReadRemotePor = 20000;
            public const int WriteRemotePort = 20001;
            //public const string RemoteIP = "192.168.123.10";

            //public const int ReadLocalPort = 12290;
            //public const int WriteLocalPort = 12291;

            //public const int ReadRemotePor = 12288;
            //public const int WriteRemotePort = 12289;
        }
    }
    public partial class PLCDataTag
    {
        public static int BASE_RW_ADDR = 320000; //PLC BASE ADDRESS
        public static int BASE_MAIN_PC_RW_ADDR = 300000; //LASER PC BASE ADDRESS
    }
#endif
    #endregion

    public partial class Main
    {
        public partial struct DEFINE
        {
            public const bool OPEN_F = false; // laptop mode - true, machine mode - false
            public const string IMAGE_FILE = SYS_DATADIR + "1-1.bmp";//"QDIDB.idb";//"D:\\SystemData\\20.idb";
            //public const string IMAGE_FILE = SYS_DATADIR + "MODEL_VISION\\INSPECTION_1\\10_29_31_892__INSPECTION_1_1_CAMERA___UP.bmp";
        }
        public static void AlignUnit_Initial_UNIT()
        {
            #region PC별 UNIT 초기화
            string[] nUnit_Name;
            int[] nPatTagMax;
            int[] nPatDispNum;
            int[,] nPatDisplayNum;
            int[] M_Resolution, nMatrixNotuse , M_ACFCutMode;
            string[] nViewName, nView_Pos, nViewSize;
            int[] nViewCnt;

            switch (Main.DEFINE.PROGRAM_TYPE)
            {
                case "OLB_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_2CAM1SHOT, "PLASMA_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM4);

                    M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 2, 3 }, { 2, 3 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 }, { 4, 5 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 }, { 6, 7 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1 }, { 0, 1 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "PLASMA_PRE_L", "PLASMA_PRE_R", "ACF_PRE1_L", "ACF_PRE1_R", "ACF_BLOB1_L", "ACF_BLOB1_R", "ACF_CUT1_L", "ACF_CUT1_R" }; // 이름

                    if (Main.DEFINE.NON_STANDARD)    //대칭
                        nView_Pos = new string[] { "1-3", "1-4", "1-1", "1-2", "1-7", "1-8", "1-5", "1-6" };
                    else
                        nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "OLB_PC2":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);

                      M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "ACF_PRE2_L", "ACF_PRE2_R", "ACT_CUT2_L", "ACF_CUT2_R", "ACF_BLOB2_L", "ACF_BLOB2_R" }; // 이름

                    if (Main.DEFINE.NON_STANDARD)    //대칭
                        nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6" };
                    else
                        nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-7" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "2*1", "2*1" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "OLB_PC3":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD_STAGE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM1SHOT, "PBD_TOOLTIP1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);

                    AlignUnit[3].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM4, DEFINE.CAM3, DEFINE.CAM4);
                    AlignUnit[4].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD_STAGE2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM4);
                    AlignUnit[5].PatAlloc(DEFINE.M_1CAM1SHOT, "PBD_TOOLTIP2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM5, DEFINE.CAM5);

                    nPatDisplayNum = new int[,] { { 0, 1, 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 2 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 3, 4, 3, 4 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 3, 4 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 5, 5 } }; AlignUnit[5].DisplayAlloc(nPatDisplayNum);

                //    M_Resolution = new int[] { 0, 3 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어 

                    AlignUnit[2].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[5].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "PBD1_L", "PBD1_R", "PBD_TOOLTIP1", "PBD2_L", "PBD2_R", "PBD_TOOLTIP2" };         // 이름
                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6" };           // 위치( POSITION )
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };             // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 3, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )

//                     nViewName = new string[] { "PBD1_L", "PBD1_R", "PBD_TOOLTIP1", "PBD2_L", "PBD2_R", "PBD_TOOLTIP2" };         // 이름
//                     nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-5", "1-6", "1-7" };           // 위치( POSITION )
//                     nViewSize = new string[] { "1*1", "1*1", "2*1", "1*1", "1*1", "2*1" };             // 크기( S_I_Z_E_ )
//                     nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )

                    Common.HEAD_MODE = DEFINE.XYT_MOVE;
                    Common.PBD_TYPE = DEFINE.FRONT_STAGE;

                    #endregion
                    break;

                case "OLB_PC4":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "REEL_ALIGN_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "REEL_ALIGN_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "COF_CUTTING_ALIGN", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "COF_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "IC_PRE_ALIGN", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4);



                    nPatDispNum = new int[] { 0, 0 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 1, 1 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[3].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 6, 7 }; AlignUnit[4].DisplayAlloc(0, nPatDispNum);

                    AlignUnit[0].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[1].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);



                    AlignUnit[2].CALMOTORNotUse(DEFINE.USE, DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[3].CALMOTORNotUse(DEFINE.USE, DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "REEL_ALIGN1", "REEL_ALIGN2", "COF_CUTTING_ALIGN1_L(下)", "COF_CUTTING_ALIGN1_R(上)", "COF_PRE_ALIGN1_L", "COF_PRE_ALIGN1_R", "IC_CHIP_PRE_ALIGN1_L", "IC_CHIP_PRE_ALIGN1_R" };

                    nView_Pos = new string[] { "1-2", "1-6", "1-5", "1-1", "1-3", "1-4", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )// 크기( S_I_Z_E_ )

                    #endregion
                    break;

                case "OLB_PC5":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "REEL_ALIGN_3", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "REEL_ALIGN_4", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "COF_CUTTING_ALIGN", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "COF_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "IC_PRE_ALIGN", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4);




                    nPatDispNum = new int[] { 0, 0 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 1, 1 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[3].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 6, 7 }; AlignUnit[4].DisplayAlloc(0, nPatDispNum);

                    AlignUnit[0].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[1].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    AlignUnit[2].CALMOTORNotUse(DEFINE.USE, DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[3].CALMOTORNotUse(DEFINE.USE, DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    nViewName = new string[] { "REEL_ALIGN3", "REEL_ALIGN4", "COF_CUTTING_ALIGN2_L(下)", "COF_CUTTING_ALIGN2_R(上)", "COF_PRE_ALIGN2_L", "COF_PRE_ALIGN2_R", "IC_CHIP_PRE_ALIGN2_L", "IC_CHIP_PRE_ALIGN2_R" };

                    nView_Pos = new string[] { "1-2", "1-6", "1-5", "1-1", "1-3", "1-4", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };               // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "OLB_PC6":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM2SHOT, "BACKUP_INSEPCTION1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM3, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM5);
                    AlignUnit[3].PatAlloc(DEFINE.M_2CAM2SHOT, "BACKUP_INSEPCTION2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM6, DEFINE.CAM7, DEFINE.CAM6, DEFINE.CAM7);

                    nPatDispNum = new int[] { 0, 1 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3, 2, 3 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 6, 7, 6, 7 }; AlignUnit[3].DisplayAlloc(0, nPatDispNum);

                    nViewName = new string[] { "FBD1_LEFT", "FBD1_RIGHT", "BACKUP_INSPECT1_L", "BACKUP_INSPECT1_R", "FBD2_LEFT", "FBD2_RIGHT", "BACKUP_INSPECT2_L", "BACKUP_INSPECT2_R" };         // 이름
                    if (Main.DEFINE.NON_STANDARD)//대칭
                        nView_Pos = new string[] { "1-3", "1-4", "1-1", "1-2", "1-7", "1-8", "1-5", "1-6" };
                    else
                        nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )

//                     Common.HEAD_MODE = DEFINE.XYT_MOVE;
//                     Common.PBD_TYPE = DEFINE.COF_TYPE;

                    #endregion
                    break;

                case "FOF_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "PAD_CLEANER", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);

                    M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "ACF_PRE1_L", "ACF_PRE1_R", "ACF_BLOB1_L", "ACF_BLOB1_R", "FOP_ACF_CUT1_L", "FOP_ACF_CUT1_R","PAD_CLEANER_L", "PAD_CLEANER_R" }; // 이름

                    nView_Pos = new string[] { "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "1-1", "1-2" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOF_PC2":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "FOP_INSPECTION", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);

                     M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "ACF_PRE2_L", "ACF_PRE2_R", "ACF_BLOB2_L", "ACF_BLOB2_R", "ACF_CUT2_L", "ACF_CUT2_R", "FOP_INSPECT_L","FOP_INSPECT_R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-5", "1-6", "1-3", "1-4","1-7","1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1","1*1","1*1" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOF_PC3":
                    #region
                    AlignUnit[0].PatAlloc(new int[] { DEFINE.M_2CAM1SHOT, DEFINE.M_4CAM1SHOT }, "PBD1", DEFINE.PATTERNTAG_MAX, new int[] { DEFINE.CAM0, DEFINE.CAM0 }, new int[] { DEFINE.CAM1, DEFINE.CAM1 }, new int[] { DEFINE.CAM0, DEFINE.CAM2 }, new int[] { DEFINE.CAM1, DEFINE.CAM3 });                   
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD_STAGE1"      , 1, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FPC_ALIGN1"      , 1, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "FOF_INSPECTION1" , 1, DEFINE.CAM4, DEFINE.CAM4);

                    nPatDisplayNum = new int[,] { { 0, 1, 0, 1 }, { 0, 1, 2, 3 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[0].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "PBD1_L", "PBD1_R", "FPC_PRE1_L", "FPC_PRE1_R", "FOF_INSPECTION1_L", "FOF_INSPECTION1_R" };         // 이름
                    nView_Pos = new string[] { "1-5", "1-6", "1-1", "1-2", "1-3", "1-4" };           // 위치( POSITION )
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*2", "1*2" };             // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )

                    Common.HEAD_MODE = DEFINE.XYT_MOVE;
                    Common.PBD_TYPE = DEFINE.FRONT_STAGE;

                    #endregion
                    break;

                case "FOF_PC4":
                    #region
                    AlignUnit[0].PatAlloc(new int[] { DEFINE.M_2CAM1SHOT, DEFINE.M_4CAM1SHOT }, "PBD2", DEFINE.PATTERNTAG_MAX, new int[] { DEFINE.CAM0, DEFINE.CAM0 }, new int[] { DEFINE.CAM1, DEFINE.CAM1 }, new int[] { DEFINE.CAM0, DEFINE.CAM2 }, new int[] { DEFINE.CAM1, DEFINE.CAM3 });  
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD_STAGE2", 1, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FPC_ALIGN2", 1, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "FOF_INSPECTION2", 1, DEFINE.CAM4, DEFINE.CAM4);

                    nPatDisplayNum = new int[,] { { 0, 1, 0, 1 }, { 0, 1, 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[0].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "PBD2_L", "PBD2_R", "FPC_PRE2_L", "FPC_PRE2_R", "FOF_INSPECTION2_L", "FOF_INSPECTION2_R" };         // 이름
                    nView_Pos = new string[] { "1-5", "1-6", "1-1", "1-2", "1-3", "1-4" };           // 위치( POSITION )
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*2", "1*2" };             // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )

                    Common.HEAD_MODE = DEFINE.XYT_MOVE;
                    Common.PBD_TYPE = DEFINE.FRONT_STAGE;
                    #endregion
                    break;

                case "FOF_PC5":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "BACKUP_INSEPCTION1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM3, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM5);
                    AlignUnit[3].PatAlloc(DEFINE.M_2CAM1SHOT, "BACKUP_INSEPCTION2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM6, DEFINE.CAM7, DEFINE.CAM6, DEFINE.CAM7);


                    AlignUnit[1].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[3].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nPatDispNum = new int[] { 0, 1 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3, 2, 3 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 6, 7, 6, 7 }; AlignUnit[3].DisplayAlloc(0, nPatDispNum);

                    nViewName = new string[] { "FBD1_LEFT", "FBD1_RIGHT", "BACKUP_INSPECT1_L", "BACKUP_INSPECT1_R", "FBD2_LEFT", "FBD2_RIGHT", "BACKUP_INSPECT2_L", "BACKUP_INSPECT2_R" };         // 이름
                    if (Main.DEFINE.NON_STANDARD)//대칭
                        nView_Pos = new string[] { "1-3", "1-4", "1-1", "1-2", "1-7", "1-8", "1-5", "1-6" };
                    else
                        nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOF_PC6":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM1SHOT, "FPC_TRAY1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "FPC_CLEANER1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "FPC_CLEANER2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM1SHOT, "FPC_TRAY2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "FPC_CLEANER3", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4);
                    AlignUnit[5].PatAlloc(DEFINE.M_1CAM2SHOT, "FPC_CLEANER4", DEFINE.PATTERNTAG_MAX, DEFINE.CAM5, DEFINE.CAM5);

                    nPatDispNum = new int[] { 8, 8 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 0, 1 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 9, 9 }; AlignUnit[3].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[4].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 6, 7 }; AlignUnit[5].DisplayAlloc(0, nPatDispNum);

                    nViewName = new string[] { "FPC_CLEANER1_L", "FPC_CLEANER1_R", "FPC_CLEANER2_L", "FPC_CLEANER2_R", "FPC_CLEANER3_L", "FPC_CLEANER3_R", "FPC_CLEANER4_L", "FPC_CLEANER4_R", "FPC_TRAY1", "FPC_TRAY2" };         // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1", "2-2" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*2", "1*2" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOF_PC7":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE1_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM4SHOT, "ACF_BLOB1_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE1_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM4SHOT, "ACF_BLOB1_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4, DEFINE.CAM4, DEFINE.CAM4);

                     M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3, 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 8, 9 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7, 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);


                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "FOF_ACF_PRE1-1L", "FOF_ACF_PRE1-1_R", "FOF_ACF_BLOB1-1L", "FOF_ACF_BLOB1-1R", "FOF_ACF_PRE1-2L", "FOF_ACF_PRE1-2R", "FOF_ACF_BLOB1-2L", "FOF_ACF_BLOB1-2R", "FOF_ACF_CUT1L", "FOF_ACF_CUT1R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1", "2-2" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*2", "1*2" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOF_PC8":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE2_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB2_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE2_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB2_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4, DEFINE.CAM4, DEFINE.CAM4);

                    M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3, 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 8, 9 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7, 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);


                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);


                    nViewName = new string[] { "FOF_ACF_PRE2-1L", "FOF_ACF_PRE2-1_R", "FOF_ACF_BLOB2-1L", "FOF_ACF_BLOB2-1R", "FOF_ACF_PRE2-2L", "FOF_ACF_PRE2-2R", "FOF_ACF_BLOB2-2L", "FOF_ACF_BLOB2-2R", "FOF_ACF_CUT2L", "FOF_ACF_CUT2R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1", "2-2" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*2", "1*2" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "TFOG_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "PANEL_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);

                     M_ACFCutMode = new int[] { 2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3, 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "ACF_PRE1_L", "ACF_PRE1_R", "ACF_BLOB1_L", "ACF_BLOB1_R", "ACF_CUT1_L", "ACF_CUT1_R", "PANEL_PRE_L", "PANEL_PRE_R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1*6", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "TFOG_PC2":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_4CAM1SHOT, "PBD1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1,DEFINE.CAM2, DEFINE.CAM3 ); // HEAD X,T
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "PBD_STAGE1", DEFINE.PATTERNTAG_MAX,DEFINE.CAM2, DEFINE.CAM3); // STAGE X Y T
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FPC_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM5);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM1SHOT, "FPC_TRAY1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM6, DEFINE.CAM6);

                    nPatDisplayNum = new int[,] { { 0, 1 ,2, 3} };  AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } };       AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } };       AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 6 } };       AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[0].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "PBD_L", "PBD_R", "PBD_STAGE_L", "PBD_STAGE_R", "FPC_PRE_L", "FPC_PRE_R", "FPC_TRAY" };         // 이름
                    nView_Pos = new string[] { "1-1", "1-2", "1-5", "1-6", "1-3", "1-4", "1-7" };           // 위치( POSITION )
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "2*1" };             // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )

                    Common.HEAD_MODE = DEFINE.XYT_MOVE;
                    Common.PBD_TYPE = DEFINE.FRONT_STAGE;

                    #endregion
                    break;

                case "TFOG_PC3":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_ALIGN1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "BACKUP_INSPECTION1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM3, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_ALIGN2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM5);
                    AlignUnit[3].PatAlloc(DEFINE.M_2CAM1SHOT, "BACKUP_INSPECTION2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM6, DEFINE.CAM7, DEFINE.CAM6, DEFINE.CAM7);


                    AlignUnit[1].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[3].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);


                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3, 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7, 6, 7 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    nViewName = new string[] { "FBD1_LEFT", "FBD1_RIGHT", "BACKUP_INSPECT1_L", "BACKUP_INSPECT1_R", "FBD2_LEFT", "FBD2_RIGHT", "BACKUP_INSPECT2_L", "BACKUP_INSPECT2_R" };         // 이름
                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "TFOG_PC4":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "BUFFER_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "FBD_MONITORING", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);


                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);

                    nViewName = new string[] { "BUFFER_PRE_L", "BUFFER_PRE_R", "FBD_MONITORING_L", "FBD_MONITORING_R" };         // 이름
                    nView_Pos = new string[] { "1-1", "1-3", "1-5", "1-7" };
                    nViewSize = new string[] { "2*1", "2*1", "2*1", "2*1" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOB_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "PANEL_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);

                 //   M_Resolution = new int[] { 0 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어
                    nViewName = new string[] { "PANEL_PRE_L", "PANEL_PRE_R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2" };
                    nViewSize = new string[] { "1*2", "1*2" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOB_PC2":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_ALIGN1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM2SHOT, "BACKUP_INSPECTION1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM3, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FPC_PRE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM5);

                    AlignUnit[1].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[2].MatrixNotUse(DEFINE.NOT_USE, DEFINE.USE, DEFINE.USE, DEFINE.USE);

                    nPatDispNum = new int[] { 0, 1 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3, 2, 3 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);

                    nViewName = new string[] { "FBD1_LEFT", "FBD1_RIGHT", "BACKUP_INSPECT1_L", "BACKUP_INSPECT1_R", "FPC_PRE1_L", "FPC_PRE1_R" };         // 이름
                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6" };
                    nViewSize = new string[] { "1*1", "1*1", "1*2", "1*2", "1*1", "1*1" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOB_PC3":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "FBD_ALIGN2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM2SHOT, "BACKUP_INSPECTION2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM3, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "FPC_PRE2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM5);


                    AlignUnit[1].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);
                    AlignUnit[2].MatrixNotUse(DEFINE.NOT_USE, DEFINE.USE, DEFINE.USE, DEFINE.USE);

                    nPatDispNum = new int[] { 0, 1 }; AlignUnit[0].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 2, 3, 2, 3 }; AlignUnit[1].DisplayAlloc(0, nPatDispNum);
                    nPatDispNum = new int[] { 4, 5 }; AlignUnit[2].DisplayAlloc(0, nPatDispNum);

                    nViewName = new string[] { "FBD2_LEFT", "FBD2_RIGHT", "BACKUP_INSPECT2_L", "BACKUP_INSPECT2_R", "FPC_PRE2_L", "FPC_PRE2_R" };         // 이름
                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6" };
                    nViewSize = new string[] { "1*1", "1*1", "1*2", "1*2", "1*1", "1*1" };       // 크기( S_I_Z_E_ )                                                                  // 크기( S_I_Z_E_ )
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOB_PC4":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "AI_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "FOB_INSPECT", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "ART_PRE", 1, DEFINE.CAM2, DEFINE.CAM3);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM1SHOT, "ART_PROBE", 1, DEFINE.CAM4, DEFINE.CAM4);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "MARKER_PRE", 1, DEFINE.CAM5, DEFINE.CAM5);

                    nPatDisplayNum = new int[,] { { 0, 1 },{ 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 },{ 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 8, 8 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[1].MatrixNotUse(DEFINE.NOT_USE, DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "AI_PRE_L", "AI_PRE_R", "FOB_INSPECT_L", "FOB_INSPECT_R", "ART_PRE_L", "ART_PRE_R", "MARKER_PRE_L", "MARKER_PRE_R", "ART_PROBE" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "2*2" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOB_PC5":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM1SHOT, "FPC_TRAY1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "PCB_CLEANER1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4);

                    M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode
                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3, 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 8, 8 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "FOF_ACF_PRE1L", "FOF_ACF_PRE1_R", "FOF_ACF_BLOB1_L", "FOF_ACF_BLOB1_R", "FOF_ACF_CUT1_L", "FOF_ACF_CUT1_R", "PCB_CLEANER1_L", "PCB_CLEANER1_R", "FPC_TRAY1" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "2*2" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "FOB_PC6":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_PRE2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_BLOB2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "ACF_CUT_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM1SHOT, "FPC_TRAY2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "PCB_CLEANER2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4);

                    M_ACFCutMode = new int[] {2 }; Main.ACFCUTMODE(M_ACFCutMode);     //선택하면 ACF Cut Mode

                    nPatDisplayNum = new int[,] { { 0, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3, 2, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 8, 8 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[2].MatrixNotUse(DEFINE.USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    nViewName = new string[] { "FOF_ACF_PRE2_L", "FOF_ACF_PRE2_R", "FOF_ACF_BLOB2_L", "FOF_ACF_BLOB2_R", "FOF_ACF_CUT2_L", "FOF_ACF_CUT2_R", "PCB_CLEANER2_L", "PCB_CLEANER2_R", "FPC_TRAY2" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "2*2" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "ENCAP_CRD_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "PANEL_ALIGN", 1, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE3", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE4", DEFINE.PATTERNTAG_MAX, DEFINE.CAM4, DEFINE.CAM4);
                    AlignUnit[5].PatAlloc(DEFINE.M_1CAM2SHOT, "THICKNESS_PRE", DEFINE.PATTERNTAG_MAX, DEFINE.CAM5, DEFINE.CAM5);

                    nPatDisplayNum = new int[,] { { 8, 9 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1 }, { 0, 1 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 }, { 2, 3 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 }, { 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 }, { 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 10, 11 }, { 10, 11 } }; AlignUnit[5].DisplayAlloc(nPatDisplayNum);
                    nViewName = new string[] { "UPPER_DIS1_L", "UPPER_DIS1_R", "UPPER_DIS2_L", "UPPER_DIS2_R", "LOW_DIS1_L", "LOW_DIS1_R", "LOW_DIS2_L", "LOW_DIS2_R", "PANEL_ALIGN_L", "PANEL_ALIGN_R", "THICKNESS_PRE_L", "THICKNESS_PRE_R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1", "2-2", "2-3", "2-4" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;
                case "CRD_TUFFY_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "PANEL_ALIGN", 1, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE1", 2, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE2", 2, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE3", 2, DEFINE.CAM3, DEFINE.CAM3);
                    AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "CRD_PRE4", 2, DEFINE.CAM4, DEFINE.CAM4);
                    AlignUnit[5].PatAlloc(DEFINE.M_1CAM2SHOT, "THICKNESS_PRE", 2, DEFINE.CAM5, DEFINE.CAM5);

                    AlignUnit[6].PatAlloc(DEFINE.M_1CAM2SHOT, "UPPER_INSPECT", DEFINE.PATTERNTAG_MAX, DEFINE.CAM5, DEFINE.CAM5);
                    AlignUnit[7].PatAlloc(DEFINE.M_1CAM2SHOT, "LOW_INSPECT", DEFINE.PATTERNTAG_MAX, DEFINE.CAM6, DEFINE.CAM6);

                    nPatDisplayNum = new int[,] { { 8, 9 } };           AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1 }, { 0, 1 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 }, { 2, 3 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 }, { 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 6, 7 }, { 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 10, 11 }, { 10, 11 } }; AlignUnit[5].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 12, 13 }, { 12, 13 }}; AlignUnit[6].DisplayAlloc(nPatDisplayNum);
                     nPatDisplayNum = new int[,] { { 14, 15 }, { 14, 15 } }; AlignUnit[7].DisplayAlloc(nPatDisplayNum);

                    nViewName = new string[] { "UPPER_DIS1_L", "UPPER_DIS1_R", "UPPER_DIS2_L", "UPPER_DIS2_R", "LOW_DIS1_L", "LOW_DIS1_R", "LOW_DIS2_L", "LOW_DIS2_R", "PANEL_ALIGN_L", "PANEL_ALIGN_R", "THICKNESS_PRE_L", "THICKNESS_PRE_R","UPPER_INSPECT_L", "UPPER_INSPECT_R","LOW_INSPECT_L", "LOW_INSPECT_R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8", "2-1", "2-2", "2-3", "2-4", "2-5", "2-6", "2-7", "2-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1"};                                                   // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 4 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "QD_LPA_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_2CAM1SHOT, "SCANNER HEAD CAM1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM4);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "ALIGN INSP CAM2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM5);
                    AlignUnit[2].PatAlloc(DEFINE.M_2CAM1SHOT, "ALIGN INSP CAM3", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM6);
                    AlignUnit[3].PatAlloc(DEFINE.M_2CAM1SHOT, "ALIGN INSP CAM4", DEFINE.PATTERNTAG_MAX, DEFINE.CAM3, DEFINE.CAM7);

                    nPatDisplayNum = new int[,] { { 1, 3 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 5, 7 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 2 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 6 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    //   M_Resolution = new int[] { 0 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어
                    nViewName = new string[] { "ALIGN CAM3", "ALIGN SCANHEAD", "INSPECTION CAM3", "INSPECTION SCANHEAD", "ALIGN CAM4", "ALIGN CAM2", "INSPECTION CAM4", "INSPECTION CAM2" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "1-7", "1-8" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "QD_LPA_PC2":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM1SHOT, "1st PREALIGN", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_2CAM1SHOT, "2nd PREALIGN", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM1);

                    nPatDisplayNum = new int[,] { { 0, 0 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 1, 2 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);

                    AlignUnit[0].MatrixNotUse(DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE, DEFINE.NOT_USE);

                    //   M_Resolution = new int[] { 0 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어
                    nViewName = new string[] { "1st PREALIGN", "2nd PREALIGN UP", "2nd PREALIGN DOWN" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-3", "1-7" };   // 아래 Cnt 구성 중 위치
                    nViewSize = new string[] { "2*2", "2*1", "2*1" };   // 아래 Cnt 구성 중 잡아먹는 크기 열*행
                    nViewCnt = new int[] { 4, 2 };   // 메인화면 표시 열
                    #endregion
                    break;
                case "SD_BIO":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "INSPECTION1", 1, DEFINE.CAM0, DEFINE.CAM0);//, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "INSPECTION2", 1, DEFINE.CAM1, DEFINE.CAM1);//, DEFINE.CAM0, DEFINE.CAM1, DEFINE.CAM1);

                    nPatDisplayNum = new int[,] { { 0, 1 }}; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 3 }}; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nViewName = new string[] { "INSPECTION 1", "INSPECTION 2", "ALIGN INSPECITON 1", "ALIGN INSPECITON 2"}; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4" };//, "1-3", "1-4", "1-5", "1-6", "2-1", "2-2", "2-3", "2-4" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1" };//, "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4,2};   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;
                case "ATT_AREA_PC1":
                    #region
                    //여기임
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "INSPECTION_1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);//, DEFINE.CAM1);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "INSPECTION_2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);//, DEFINE.CAM0, DEFINE.CAM1, DEFINE.CAM1);

                    //nPatDisplayNum = new int[,] { { 2, 2 }, { 3, 3 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    //nPatDisplayNum = new int[,] { { 1, 1 }, { 0, 0 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 0 }, { 1, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 2, 2 }, { 3, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nViewName = new string[] { "INSPECTION 1", "INSPECTION 3", "INSPECTION 2", "INSPECTION 4" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-3", "1-2", "1-4" };//, "1-3", "1-4", "1-5", "1-6", "2-1", "2-2", "2-3", "2-4" };
                    nViewSize = new string[] { "1*2", "1*2", "1*2", "1*2" };//, "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    //AlignUnit[0].PatAlloc(DEFINE.M_1CAM1SHOT, "INSPECTION", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    //AlignUnit[1].PatAlloc(DEFINE.M_1CAM1SHOT, "ALIGN INSPECTION", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM1);
                    ////AlignUnit[2].PatAlloc(DEFINE.M_1CAM1SHOT, "ALIGN INSPECTION 1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    ////AlignUnit[3].PatAlloc(DEFINE.M_1CAM2SHOT, "ALIGN INSPECTION 2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    ////AlignUnit[4].PatAlloc(DEFINE.M_1CAM2SHOT, "PRE2-1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    ////AlignUnit[5].PatAlloc(DEFINE.M_1CAM2SHOT, "PRE2-2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    ////AlignUnit[6].PatAlloc(DEFINE.M_1CAM4SHOT, "ATT ALIGN INSP2-1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2, DEFINE.CAM2);
                    ////AlignUnit[7].PatAlloc(DEFINE.M_1CAM4SHOT, "ATT ALIGN INSP2-2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2, DEFINE.CAM2);

                    //nPatDisplayNum = new int[,] { { 0, 0 }, { 1, 1 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    //nPatDisplayNum = new int[,] { { 2, 2 }, { 3, 3 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    ////nPatDisplayNum = new int[,] { { 0, 1, 2 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    ////nPatDisplayNum = new int[,] { { 3, 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);
                    ////nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[4].DisplayAlloc(nPatDisplayNum);
                    ////nPatDisplayNum = new int[,] { { 8, 9 } }; AlignUnit[5].DisplayAlloc(nPatDisplayNum);
                    ////nPatDisplayNum = new int[,] { { 0, 1, 2 } }; AlignUnit[6].DisplayAlloc(nPatDisplayNum);
                    ////nPatDisplayNum = new int[,] { { 3, 4, 5 } }; AlignUnit[7].DisplayAlloc(nPatDisplayNum);

                    ////   M_Resolution = new int[] { 0 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어
                    //nViewName = new string[] { "INSPECTION 1", "INSPECTION 2","ALIGN INSPECITON 1", "ALIGN INSPECITON 2" }; // 이름

                    //nView_Pos = new string[] { "1-1", "1-3", "1-5", "1-7" };//, "1-3", "1-4", "1-5", "1-6", "2-1", "2-2", "2-3", "2-4" };
                    //nViewSize = new string[] { "2*1", "2*1", "2*1", "2*1" };//, "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    //nViewCnt = new int[] { 4, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "ATT_AREA_PC2":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "PRE1-1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "PRE1-2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM4SHOT, "ATT ALIGN INSP1-1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2, DEFINE.CAM2);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM4SHOT, "ATT ALIGN INSP1-2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM2, DEFINE.CAM2, DEFINE.CAM2);

                    nPatDisplayNum = new int[,] { { 6, 7 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 8, 9 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0, 1, 2 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 3, 4, 5 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    //   M_Resolution = new int[] { 0 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어
                    nViewName = new string[] { "ATT ALIGN INSP1L", "ATT ALIGN INSP1C", "ATT ALIGN INSP1R", "ATT ALIGN INSP2L", "ATT ALIGN INSP2C", "ATT ALIGN INSP2R", "PRE ALIGN1L", "PRE ALIGN1R", "PRE ALIGN2L", "PRE ALIGN2R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "1-3", "1-4", "1-5", "1-6", "2-1", "2-2", "2-3", "2-4" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 3, 4 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

                case "ATT_LINE_PC1":
                    #region
                    AlignUnit[0].PatAlloc(DEFINE.M_1CAM2SHOT, "PRE1-1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[1].PatAlloc(DEFINE.M_1CAM2SHOT, "PRE1-2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM0, DEFINE.CAM0);
                    AlignUnit[2].PatAlloc(DEFINE.M_1CAM1SHOT, "ATT ALIGN INSP1-1", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);
                    AlignUnit[3].PatAlloc(DEFINE.M_1CAM1SHOT, "ATT ALIGN INSP1-2", DEFINE.PATTERNTAG_MAX, DEFINE.CAM1, DEFINE.CAM1);

                    nPatDisplayNum = new int[,] { { 2, 3 } }; AlignUnit[0].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 4, 5 } }; AlignUnit[1].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 0 } }; AlignUnit[2].DisplayAlloc(nPatDisplayNum);
                    nPatDisplayNum = new int[,] { { 1 } }; AlignUnit[3].DisplayAlloc(nPatDisplayNum);

                    //   M_Resolution = new int[] { 0 }; Main.MotorResol(M_Resolution);     //선택하면 0.1um 제어
                    nViewName = new string[] { "ATT AKKON INSP1", "ATT AKKON INSP2", "PRE ALIGN1L", "PRE ALIGN1R", "PRE ALIGN2L", "PRE ALIGN2R" }; // 이름

                    nView_Pos = new string[] { "1-1", "1-2", "2-1", "2-2", "2-3", "2-4" };
                    nViewSize = new string[] { "1*1", "1*1", "1*1", "1*1", "1*1", "1*1" };      // 크기( S_I_Z_E_ 
                    nViewCnt = new int[] { 1, 2 };   //메인화면 표시 열 갯수// 크기( S_I_Z_E_ // 크기( S_I_Z_E_ )
                    #endregion
                    break;

            }
            #region View Name,위치,사이즈 초기화
            Common.VIEW_NAME = new string[Main.DEFINE.DISPLAY_MAX];
            Common.VIEW_Pos = new string[Main.DEFINE.DISPLAY_MAX];
            Common.VIEW_Size = new string[Main.DEFINE.DISPLAY_MAX];

            Common.VIEW_NAME = nViewName;
            Common.VIEW_Pos = nView_Pos;
            Common.VIEW_Size = nViewSize;
            Common.VIEW_WIDTH_CNT = nViewCnt;
            #endregion
            #endregion

        } //AlignUnit_Initial`

    }
}
