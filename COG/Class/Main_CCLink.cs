using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

//using mdWrapper;

namespace COG
{
#if true
    public partial class Main
    {
        // 200706 CCLink
  //      private static MCCLinkWrapper CCLink = new MCCLinkWrapper(Main.DEFINE.CCLINK_MASTER, 0);

        #region CCLINK_DEFINE
        public partial struct DEFINE
        {
            #region BP_LPA_PC1
            public const int CCLINK_BX_MAX_NUM = 2;
            public const int CCLINK_BYI_MAX_NUM = 14;
            public const int CCLINK_BYO_MAX_NUM = 6;
            public const int CCLINK_WR_MAX_NUM = 2;
            public const int CCLINK_WW_MAX_NUM = 4;

            /// <summary>
            /// BIT IN(X-PLC) 영역
            /// </summary>
            //[X9E0]
            public const ushort CCLINK_IN_ALIVE = 1000;
            public const ushort CCLINK_IN_AUTO_READY = 1001;
            public const ushort CCLINK_IN_AUTO_RUN = 1002;
            public const ushort CCLINK_IN_ERROR = 1003;
            public const ushort CCLINK_IN_WARNING = 1004;
            public const ushort CCLINK_IN_PM_MODE = 1005;
            public const ushort CCLINK_IN_1CYCLE_MODE = 1006;
            public const ushort CCLINK_IN_EMERGENCY_STOP = 1007;
            public const ushort CCLINK_IN_DOOR_LOCK = 1008;
            public const ushort CCLINK_IN_1009 = 1009;
            public const ushort CCLINK_IN_1010 = 1010;
            public const ushort CCLINK_IN_ERROR_RESET = 1011;
            public const ushort CCLINK_IN_1012 = 1012;
            public const ushort CCLINK_IN_1013 = 1013;
            public const ushort CCLINK_IN_1014 = 1014;
            public const ushort CCLINK_IN_1015 = 1015;

            //[X9F0]
            public const ushort CCLINK_IN_LOC_PPID_COPY_REQ = 1016;
            public const ushort CCLINK_IN_LOC_PPID_DELETE_REQ = 1017;
            public const ushort CCLINK_IN_LOC_PPID_BODY_CHANGE_REQ = 1018;
            public const ushort CCLINK_IN_LOC_PPID_MODEL_CHANGE_REQ = 1019;
            public const ushort CCLINK_IN_LOC_ECM_CHANGE_REQ = 1020;
            public const ushort CCLINK_IN_1021 = 1021;
            public const ushort CCLINK_IN_1022 = 1022;
            public const ushort CCLINK_IN_1023 = 1023;
            public const ushort CCLINK_IN_RMT_PPID_CREATE_REQ = 1024;
            public const ushort CCLINK_IN_RMT_PPID_DELETE_REQ = 1025;
            public const ushort CCLINK_IN_RMT_PPID_BODY_CHANGE_REQ = 1026;
            public const ushort CCLINK_IN_RMT_PPID_BODY_SEARCH_REQ = 1027;
            public const ushort CCLINK_IN_1028 = 1028;
            public const ushort CCLINK_IN_TIME_CHANGE_REQ = 1029;
            public const ushort CCLINK_IN_STAGE1_INIT_REQ = 1030;
            public const ushort CCLINK_IN_STAGE2_INIT_REQ = 1031;

            /// <summary>
            /// BIT IN(Y-PC) 영역
            /// </summary>
            //[Y900]
            public const ushort CCLINK_IN_PC_ALIVE = 2000;
            public const ushort CCLINK_IN_PC_AUTO_READY = 2001;
            public const ushort CCLINK_IN_PC_AUTO_RUN = 2002;
            public const ushort CCLINK_IN_PC_ERROR = 2003;
            public const ushort CCLINK_IN_PC_WARNING = 2004;
            public const ushort CCLINK_IN_PC_PM_MODE = 2005;
            public const ushort CCLINK_IN_PC_1CYCLE_MODE = 2006;
            public const ushort CCLINK_IN_2007 = 2007;
            public const ushort CCLINK_IN_PC_MOTOR_BUSY = 2008;
            public const ushort CCLINK_IN_PC_LASER_BUSY = 2009;
            public const ushort CCLINK_IN_2010 = 2010;
            public const ushort CCLINK_IN_2011 = 2011;
            public const ushort CCLINK_IN_2012 = 2012;
            public const ushort CCLINK_IN_2013 = 2013;
            public const ushort CCLINK_IN_PC_ERROR_RESET = 2014;
            public const ushort CCLINK_IN_PC_BUZZER_OFF = 2015;

            //[Y910]
            public const ushort CCLINK_IN_PC_LOC_PPID_COPY_COMP = 2016;
            public const ushort CCLINK_IN_PC_LOC_PPID_DELETE_COMP = 2017;
            public const ushort CCLINK_IN_PC_LOC_PPID_BODY_CHANGE_COMP = 2018;
            public const ushort CCLINK_IN_PC_LOC_PPID_MODEL_CHANGE_COMP = 2019;
            public const ushort CCLINK_IN_PC_LOC_ECM_CHANGE_COMP = 2020;
            public const ushort CCLINK_IN_2021 = 2021;
            public const ushort CCLINK_IN_2022 = 2022;
            public const ushort CCLINK_IN_2023 = 2023;
            public const ushort CCLINK_IN_PC_RMT_PPID_CREATE_COMP = 2024;
            public const ushort CCLINK_IN_PC_RMT_PPID_DELETE_COMP = 2025;
            public const ushort CCLINK_IN_PC_RMT_PPID_BODY_CHANGE_COMP = 2026;
            public const ushort CCLINK_IN_PC_RMT_PPID_BODY_SEARCH_COMP = 2027;
            public const ushort CCLINK_IN_2028 = 2028;
            public const ushort CCLINK_IN_PC_TIME_CHANGE_COMP = 2029;
            public const ushort CCLINK_IN_PC_STAGE1_INIT_COMP = 2030;
            public const ushort CCLINK_IN_PC_STAGE2_INIT_COMP = 2031;

            //[Y920]
            public const ushort CCLINK_IN_STAGE1_CMD_READY = 2032;
            public const ushort CCLINK_IN_STAGE1_CMD_BUSY = 2033;
            public const ushort CCLINK_IN_STAGE1_PWR_CHK_BUSY = 2034;
            public const ushort CCLINK_IN_2035 = 2035;
            public const ushort CCLINK_IN_2036 = 2036;
            public const ushort CCLINK_IN_2037 = 2037;
            public const ushort CCLINK_IN_2038 = 2038;
            public const ushort CCLINK_IN_2039 = 2039;
            public const ushort CCLINK_IN_STAGE1_LOAD_POS = 2040;
            public const ushort CCLINK_IN_STAGE1_CUT_COMP = 2041;
            public const ushort CCLINK_IN_STAGE1_INSP_COMP = 2042;
            public const ushort CCLINK_IN_STAGE1_REMOVE_POS = 2043;
            public const ushort CCLINK_IN_STAGE1_UNLOAD_POS = 2044;
            public const ushort CCLINK_IN_STAGE1_CLEAN_POS = 2045;
            public const ushort CCLINK_IN_STAGE1_REJECT_POS = 2046;
            public const ushort CCLINK_IN_2047 = 2047;

            //[Y930]
            public const ushort CCLINK_IN_STAGE1_LOAD_POS_MOVE_COMP = 2048;
            public const ushort CCLINK_IN_STAGE1_CUT_RUN_COMP = 2049;
            public const ushort CCLINK_IN_STAGE1_INSP_RUN_COMP = 2050;
            public const ushort CCLINK_IN_STAGE1_REMOVE_POS_MOVE_COMP = 2051;
            public const ushort CCLINK_IN_STAGE1_UNLOAD_POS_MOVE_COMP = 2052;
            public const ushort CCLINK_IN_STAGE1_CLEAN_POS_MOVE_COMP = 2053;
            public const ushort CCLINK_IN_STAGE1_REJECT_POS_MOVE_COMP = 2054;
            public const ushort CCLINK_IN_2055 = 2055;
            public const ushort CCLINK_IN_2056 = 2056;
            public const ushort CCLINK_IN_2057 = 2057;
            public const ushort CCLINK_IN_2058 = 2058;
            public const ushort CCLINK_IN_2059 = 2059;
            public const ushort CCLINK_IN_2060 = 2060;
            public const ushort CCLINK_IN_2061 = 2061;
            public const ushort CCLINK_IN_2062 = 2062;
            public const ushort CCLINK_IN_2063 = 2063;

            //[Y940]
            public const ushort CCLINK_IN_STAGE2_CMD_READY = 2064;
            public const ushort CCLINK_IN_STAGE2_CMD_BUSY = 2065;
            public const ushort CCLINK_IN_STAGE2_PWR_CHK_BUSY = 2066;
            public const ushort CCLINK_IN_2067 = 2067;
            public const ushort CCLINK_IN_2068 = 2068;
            public const ushort CCLINK_IN_2069 = 2069;
            public const ushort CCLINK_IN_2070 = 2070;
            public const ushort CCLINK_IN_2071 = 2071;
            public const ushort CCLINK_IN_STAGE2_LOAD_POS = 2072;
            public const ushort CCLINK_IN_STAGE2_CUT_COMP = 2073;
            public const ushort CCLINK_IN_STAGE2_INSP_COMP = 2074;
            public const ushort CCLINK_IN_STAGE2_REMOVE_POS = 2075;
            public const ushort CCLINK_IN_STAGE2_UNLOAD_POS = 2076;
            public const ushort CCLINK_IN_STAGE2_CLEAN_POS = 2077;
            public const ushort CCLINK_IN_STAGE2_REJECT_POS = 2078;
            public const ushort CCLINK_IN_2079 = 2079;

            //[Y950]
            public const ushort CCLINK_IN_STAGE2_LOAD_POS_MOVE_COMP = 2080;
            public const ushort CCLINK_IN_STAGE2_CUT_RUN_COMP = 2081;
            public const ushort CCLINK_IN_STAGE2_INSP_RUN_COMP = 2082;
            public const ushort CCLINK_IN_STAGE2_REMOVE_POS_MOVE_COMP = 2083;
            public const ushort CCLINK_IN_STAGE2_UNLOAD_POS_MOVE_COMP = 2084;
            public const ushort CCLINK_IN_STAGE2_CLEAN_POS_MOVE_COMP = 2085;
            public const ushort CCLINK_IN_STAGE2_REJECT_POS_MOVE_COMP = 2086;
            public const ushort CCLINK_IN_2087 = 2087;
            public const ushort CCLINK_IN_2088 = 2088;
            public const ushort CCLINK_IN_2089 = 2089;
            public const ushort CCLINK_IN_2090 = 2090;
            public const ushort CCLINK_IN_2091 = 2091;
            public const ushort CCLINK_IN_2092 = 2092;
            public const ushort CCLINK_IN_2093 = 2093;
            public const ushort CCLINK_IN_2094 = 2094;
            public const ushort CCLINK_IN_2095 = 2095;

            //[Y960]
            public const ushort CCLINK_IN_2096 = 2096;
            public const ushort CCLINK_IN_2097 = 2097;
            public const ushort CCLINK_IN_2098 = 2098;
            public const ushort CCLINK_IN_2099 = 2099;
            public const ushort CCLINK_IN_2100 = 2100;
            public const ushort CCLINK_IN_2101 = 2101;
            public const ushort CCLINK_IN_2102 = 2102;
            public const ushort CCLINK_IN_2103 = 2103;
            public const ushort CCLINK_IN_2104 = 2104;
            public const ushort CCLINK_IN_2105 = 2105;
            public const ushort CCLINK_IN_2106 = 2106;
            public const ushort CCLINK_IN_2107 = 2107;
            public const ushort CCLINK_IN_SCALE_BAR_AIR_BLW_X = 2108;
            public const ushort CCLINK_IN_SCALE_BAR_AIR_BLW_Y1_Y2 = 2109;
            public const ushort CCLINK_IN_SCALE_BAR_AIR_BLW_Y3 = 2110;
            public const ushort CCLINK_IN_SCALE_BAR_AIR_BLW_Y4 = 2111;

            //[Y970]
            public const ushort CCLINK_IN_2112 = 2112;
            public const ushort CCLINK_IN_2113 = 2113;
            public const ushort CCLINK_IN_2114 = 2114;
            public const ushort CCLINK_IN_2115 = 2115;
            public const ushort CCLINK_IN_PWR_CHK_FWD_SOURCE = 2116;
            public const ushort CCLINK_IN_PWR_CHK_FWD_SCANNER = 2117;
            public const ushort CCLINK_IN_PWR_CHK_FAN_PWR_SOURCE = 2118;
            public const ushort CCLINK_IN_PWR_CHK_FAN_PWR_SCANNER = 2119;
            public const ushort CCLINK_IN_LASER_OPTIC_OPEN = 2120;
            public const ushort CCLINK_IN_LASER_INTERLOCK_ON = 2121;
            public const ushort CCLINK_IN_2122 = 2122;
            public const ushort CCLINK_IN_2123 = 2123;
            public const ushort CCLINK_IN_2124 = 2124;
            public const ushort CCLINK_IN_2125 = 2125;
            public const ushort CCLINK_IN_2126 = 2126;
            public const ushort CCLINK_IN_2127 = 2127;

            //[Y980]
            public const ushort CCLINK_IN_CAM1_SCALE_HEAD_AIR_BLW = 2128;
            public const ushort CCLINK_IN_CAM2_SCALE_HEAD_AIR_BLW = 2129;
            public const ushort CCLINK_IN_CAM3_SCALE_HEAD_AIR_BLW = 2130;
            public const ushort CCLINK_IN_CAM4_SCALE_HEAD_AIR_BLW = 2131;
            public const ushort CCLINK_IN_2132 = 2132;
            public const ushort CCLINK_IN_2133 = 2133;
            public const ushort CCLINK_IN_2134 = 2134;
            public const ushort CCLINK_IN_2135 = 2135;
            public const ushort CCLINK_IN_SCANNER_SUCTION_CUP_BLW = 2136;
            public const ushort CCLINK_IN_LASER_GAS_GENR_BLW = 2137;
            public const ushort CCLINK_IN_CAM1_LENS_BLW = 2138;
            public const ushort CCLINK_IN_CAM2_LENS_BLW = 2139;
            public const ushort CCLINK_IN_CAM3_LENS_BLW = 2140;
            public const ushort CCLINK_IN_2141 = 2141;
            public const ushort CCLINK_IN_2142 = 2142;
            public const ushort CCLINK_IN_SCANNER_Z_BRAKE = 2143;

            //[Y990]
            public const ushort CCLINK_IN_WORK_TABLE_LEFT_LIGHT_FWD = 2144;
            public const ushort CCLINK_IN_WORK_TABLE_RIGHT_LIGHT_FWD = 2145;
            public const ushort CCLINK_IN_WORK_TABLE_SCALE_AIR_BLW = 2146;
            public const ushort CCLINK_IN_WORK_TABLE_POINT_AIR_BLW = 2147;
            public const ushort CCLINK_IN_2148 = 2148;
            public const ushort CCLINK_IN_2149 = 2149;
            public const ushort CCLINK_IN_2150 = 2150;
            public const ushort CCLINK_IN_2151 = 2151;
            public const ushort CCLINK_IN_2152 = 2152;
            public const ushort CCLINK_IN_2153 = 2153;
            public const ushort CCLINK_IN_2154 = 2154;
            public const ushort CCLINK_IN_2155 = 2155;
            public const ushort CCLINK_IN_2156 = 2156;
            public const ushort CCLINK_IN_2157 = 2157;
            public const ushort CCLINK_IN_2158 = 2158;
            public const ushort CCLINK_IN_2159 = 2159;

            //[Y9A0]
            public const ushort CCLINK_IN_STAGE1_FINE_ALIGN_MODE = 2160;
            public const ushort CCLINK_IN_STAGE1_INSPECTION_MODE = 2161;
            public const ushort CCLINK_IN_STAGE1_R_CUT_MODE = 2162;
            public const ushort CCLINK_IN_STAGE1_C_CUT_MODE = 2163;
            public const ushort CCLINK_IN_STAGE1_SHAPE_CUT_MODE = 2164;
            public const ushort CCLINK_IN_STAGE1_ROUND_CAM_SELECT = 2165;
            public const ushort CCLINK_IN_STAGE1_PAD_CAM_SELECT = 2166;
            public const ushort CCLINK_IN_STAGE1_BEAM_SZ_CHK_REQ = 2167;
            public const ushort CCLINK_IN_STAGE1_CAM_LIVE_REQ = 2168;
            public const ushort CCLINK_IN_STAGE1_CAM_FREEZE_REQ = 2169;
            public const ushort CCLINK_IN_STAGE1_CAM_GRAB_REQ = 2170;
            public const ushort CCLINK_IN_STAGE1_CAM_FORCE_GRAB_REQ = 2171;
            public const ushort CCLINK_IN_STAGE1_CAM_CALIB_REQ = 2172;
            public const ushort CCLINK_IN_2173 = 2173;
            public const ushort CCLINK_IN_STAGE1_CAM_RCS_REQ = 2174;
            public const ushort CCLINK_IN_STAGE1_MOVE_COMP = 2175;

            //[Y9B0]
            public const ushort CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_X = 2176;
            public const ushort CCLINK_IN_2177 = 2177;
            public const ushort CCLINK_IN_2178 = 2178;
            public const ushort CCLINK_IN_STAGE1_CAM1_SEARCH_REQ_Y = 2179;
            public const ushort CCLINK_IN_2180 = 2180;
            public const ushort CCLINK_IN_2181 = 2181;
            public const ushort CCLINK_IN_2182 = 2182;
            public const ushort CCLINK_IN_2183 = 2183;
            public const ushort CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_X = 2184;
            public const ushort CCLINK_IN_2185 = 2185;
            public const ushort CCLINK_IN_2186 = 2186;
            public const ushort CCLINK_IN_STAGE1_CAM2_SEARCH_REQ_Y = 2187;
            public const ushort CCLINK_IN_2188 = 2188;
            public const ushort CCLINK_IN_2189 = 2189;
            public const ushort CCLINK_IN_2190 = 2190;
            public const ushort CCLINK_IN_2191 = 2191;

            //[Y9C0]
            public const ushort CCLINK_IN_STAGE2_FINE_ALIGN_MODE = 2192;
            public const ushort CCLINK_IN_STAGE2_INSPECTION_MODE = 2193;
            public const ushort CCLINK_IN_STAGE2_R_CUT_MODE = 2194;
            public const ushort CCLINK_IN_STAGE2_C_CUT_MODE = 2195;
            public const ushort CCLINK_IN_STAGE2_SHAPE_CUT_MODE = 2196;
            public const ushort CCLINK_IN_STAGE2_ROUND_CAM_SELECT = 2197;
            public const ushort CCLINK_IN_STAGE2_PAD_CAM_SELECT = 2198;
            public const ushort CCLINK_IN_STAGE2_BEAM_SZ_CHK_REQ = 2199;
            public const ushort CCLINK_IN_STAGE2_CAM_LIVE_REQ = 2200;
            public const ushort CCLINK_IN_STAGE2_CAM_FREEZE_REQ = 2201;
            public const ushort CCLINK_IN_STAGE2_CAM_GRAB_REQ = 2202;
            public const ushort CCLINK_IN_STAGE2_CAM_FORCE_GRAB_REQ = 2203;
            public const ushort CCLINK_IN_STAGE2_CAM_CALIB_REQ = 2204;
            public const ushort CCLINK_IN_2205 = 2205;
            public const ushort CCLINK_IN_STAGE2_CAM_RCS_REQ = 2206;
            public const ushort CCLINK_IN_STAGE2_MOVE_COMP = 2207;

            //[Y9D0]
            public const ushort CCLINK_IN_STAGE2_CAM1_SEARCH_REQ_X = 2208;
            public const ushort CCLINK_IN_2209 = 2209;
            public const ushort CCLINK_IN_2210 = 2210;
            public const ushort CCLINK_IN_STAGE2_CAM1_SEARCH_REQ_Y = 2211;
            public const ushort CCLINK_IN_2212 = 2212;
            public const ushort CCLINK_IN_2213 = 2213;
            public const ushort CCLINK_IN_2214 = 2214;
            public const ushort CCLINK_IN_2215 = 2215;
            public const ushort CCLINK_IN_STAGE2_CAM2_SEARCH_REQ_X = 2216;
            public const ushort CCLINK_IN_2217 = 2217;
            public const ushort CCLINK_IN_2218 = 2218;
            public const ushort CCLINK_IN_STAGE2_CAM2_SEARCH_REQ_Y = 2219;
            public const ushort CCLINK_IN_2220 = 2220;
            public const ushort CCLINK_IN_2221 = 2221;
            public const ushort CCLINK_IN_2222 = 2222;
            public const ushort CCLINK_IN_2223 = 2223;

            /// <summary>
            /// BIT OUT Y(VIS) 영역
            /// </summary>
            //[Y9E0]
            public const ushort CCLINK_OUT_ALIVE = 3000;
            public const ushort CCLINK_OUT_AUTO_READY = 3001;
            public const ushort CCLINK_OUT_AUTO_RUN = 3002;
            public const ushort CCLINK_OUT_ERROR = 3003;
            public const ushort CCLINK_OUT_WARNING = 3004;
            public const ushort CCLINK_OUT_PM_MODE = 3005;
            public const ushort CCLINK_OUT_1CYCLE_MODE = 3006;
            public const ushort CCLINK_OUT_3007 = 3007;
            public const ushort CCLINK_OUT_VISION_BUSY = 3008;
            public const ushort CCLINK_OUT_3009 = 3009;
            public const ushort CCLINK_OUT_3020 = 3020;
            public const ushort CCLINK_OUT_3011 = 3011;
            public const ushort CCLINK_OUT_3012 = 3012;
            public const ushort CCLINK_OUT_3013 = 3013;
            public const ushort CCLINK_OUT_3014 = 3014;
            public const ushort CCLINK_OUT_BUZZER_OFF = 3015;

            //[Y9F0]
            public const ushort CCLINK_OUT_LOC_PPID_COPY_COMP = 3016;
            public const ushort CCLINK_OUT_LOC_PPID_DELETE_COMP = 3017;
            public const ushort CCLINK_OUT_LOC_PPID_BODY_CHANGE_COMP = 3018;
            public const ushort CCLINK_OUT_LOC_PPID_MODEL_CHANGE_COMP = 3019;
            public const ushort CCLINK_OUT_LOC_ECM_CHANGE_COMP = 3020;
            public const ushort CCLINK_OUT_3021 = 3021;
            public const ushort CCLINK_OUT_3022 = 3022;
            public const ushort CCLINK_OUT_3023 = 3023;
            public const ushort CCLINK_OUT_RMT_PPID_CREATE_COMP = 3024;
            public const ushort CCLINK_OUT_RMT_PPID_DELETE_COMP = 3025;
            public const ushort CCLINK_OUT_RMT_PPID_BODY_CHANGE_COMP = 3026;
            public const ushort CCLINK_OUT_RMT_PPID_BODY_SEARCH_COMP = 3027;
            public const ushort CCLINK_OUT_3028 = 3028;
            public const ushort CCLINK_OUT_TIME_CHANGE_COMP = 3029;
            public const ushort CCLINK_OUT_STAGE1_INIT_COMP = 3030;
            public const ushort CCLINK_OUT_STAGE2_INIT_COMP = 3031;

            //[YA00]
            public const ushort CCLINK_OUT_STAGE1_CAM_FORCE_GRAB_COMP = 3032;
            public const ushort CCLINK_OUT_STAGE1_CAM_FORCE_GRAB_CANCEL = 3033;
            public const ushort CCLINK_OUT_STAGE1_CAM_RCS_COMP = 3034;
            public const ushort CCLINK_OUT_STAGE1_CAM_RCS_CANCEL = 3035;
            public const ushort CCLINK_OUT_3036 = 3036;
            public const ushort CCLINK_OUT_3037 = 3037;
            public const ushort CCLINK_OUT_3038 = 3038;
            public const ushort CCLINK_OUT_STAGE1_BEAM_SZ_CHK_COMP = 3039;
            public const ushort CCLINK_OUT_STAGE1_CAM_LIVE_COMP = 3040;
            public const ushort CCLINK_OUT_STAGE1_CAM_FREEZE_COMP = 3041;
            public const ushort CCLINK_OUT_STAGE1_CAM_GRAB_COMP = 3042;
            public const ushort CCLINK_OUT_3043 = 3043;
            public const ushort CCLINK_OUT_STAGE1_CAM_CALIB_COMP = 3044;
            public const ushort CCLINK_OUT_STAGE1_CAM_CALIB_FAIL = 3045;
            public const ushort CCLINK_OUT_3046 = 3046;
            public const ushort CCLINK_OUT_STAGE1_MOVE_REQ = 3047;

            //[YA10]
            public const ushort CCLINK_OUT_STAGE1_CAM1_SEARCH_COMP_X = 3048;
            public const ushort CCLINK_OUT_STAGE1_CAM1_RESULT_OK_X = 3049;
            public const ushort CCLINK_OUT_STAGE1_CAM1_SEARCH_COMP_Y = 3050;
            public const ushort CCLINK_OUT_STAGE1_CAM1_RESULT_OK_Y = 3051;
            public const ushort CCLINK_OUT_STAGE1_CAM1_RESULT_OK_C_R = 3052;
            public const ushort CCLINK_OUT_STAGE1_CAM1_PANEL_LIMIT = 3053;
            public const ushort CCLINK_OUT_STAGE1_CAM1_INSP_CAM_NG = 3054;
            public const ushort CCLINK_OUT_3055 = 3055;
            public const ushort CCLINK_OUT_STAGE1_CAM2_SEARCH_COMP_X = 3056;
            public const ushort CCLINK_OUT_STAGE1_CAM2_RESULT_OK_X = 3057;
            public const ushort CCLINK_OUT_STAGE1_CAM2_SEARCH_COMP_Y = 3058;
            public const ushort CCLINK_OUT_STAGE1_CAM2_RESULT_OK_Y = 3059;
            public const ushort CCLINK_OUT_STAGE1_CAM2_RESULT_OK_C_R = 3060;
            public const ushort CCLINK_OUT_STAGE1_CAM2_PANEL_LIMIT = 3061;
            public const ushort CCLINK_OUT_STAGE1_CAM2_INSP_CAM_NG = 3062;
            public const ushort CCLINK_OUT_3063 = 3063;

            //[YA20]
            public const ushort CCLINK_OUT_STAGE2_CAM_FORCE_GRAB_COMP = 3064;
            public const ushort CCLINK_OUT_STAGE2_CAM_FORCE_GRAB_CANCEL = 3065;
            public const ushort CCLINK_OUT_STAGE2_CAM_RCS_COMP = 3066;
            public const ushort CCLINK_OUT_STAGE2_CAM_RCS_CANCEL = 3067;
            public const ushort CCLINK_OUT_3068 = 3068;
            public const ushort CCLINK_OUT_3069 = 3069;
            public const ushort CCLINK_OUT_3070 = 3070;
            public const ushort CCLINK_OUT_STAGE2_BEAM_SZ_CHK_COMP = 3071;
            public const ushort CCLINK_OUT_STAGE2_CAM_LIVE_COMP = 3072;
            public const ushort CCLINK_OUT_STAGE2_CAM_FREEZE_COMP = 3073;
            public const ushort CCLINK_OUT_STAGE2_CAM_GRAB_COMP = 3074;
            public const ushort CCLINK_OUT_3075 = 3075;
            public const ushort CCLINK_OUT_STAGE2_CAM_CALIB_COMP = 3076;
            public const ushort CCLINK_OUT_STAGE2_CAM_CALIB_FAIL = 3077;
            public const ushort CCLINK_OUT_3078 = 3078;
            public const ushort CCLINK_OUT_STAGE2_MOVE_REQ = 3079;

            //[YA30]
            public const ushort CCLINK_OUT_STAGE2_CAM1_SEARCH_COMP_X = 3080;
            public const ushort CCLINK_OUT_STAGE2_CAM1_RESULT_OK_X = 3081;
            public const ushort CCLINK_OUT_STAGE2_CAM1_SEARCH_COMP_Y = 3082;
            public const ushort CCLINK_OUT_STAGE2_CAM1_RESULT_OK_Y = 3083;
            public const ushort CCLINK_OUT_STAGE2_CAM1_RESULT_OK_C_R = 3084;
            public const ushort CCLINK_OUT_STAGE2_CAM1_PANEL_LIMIT = 3085;
            public const ushort CCLINK_OUT_STAGE2_CAM1_INSP_CAM_NG = 3086;
            public const ushort CCLINK_OUT_3087 = 3087;
            public const ushort CCLINK_OUT_STAGE2_CAM2_SEARCH_COMP_X = 3088;
            public const ushort CCLINK_OUT_STAGE2_CAM2_RESULT_OK_X = 3089;
            public const ushort CCLINK_OUT_STAGE2_CAM2_SEARCH_COMP_Y = 3090;
            public const ushort CCLINK_OUT_STAGE2_CAM2_RESULT_OK_Y = 3091;
            public const ushort CCLINK_OUT_STAGE2_CAM2_RESULT_OK_C_R = 3092;
            public const ushort CCLINK_OUT_STAGE2_CAM2_PANEL_LIMIT = 3093;
            public const ushort CCLINK_OUT_STAGE2_CAM2_INSP_CAM_NG = 3094;
            public const ushort CCLINK_OUT_3095 = 3095;

            /// <summary>
            /// Read Word 영역
            /// </summary>
            //[D1024]
            public const ushort CCLINK_WW_PANEL1_CELL_ID = 4000;
            public const ushort CCLINK_WW_PANEL1_CELL_ID_1 = 4002;
            public const ushort CCLINK_WW_PANEL1_CELL_ID_2 = 4004;
            public const ushort CCLINK_WW_PANEL1_CELL_ID_3 = 4006;
            public const ushort CCLINK_WW_PANEL1_CELL_ID_4 = 4008;
            public const ushort CCLINK_WW_PANEL2_CELL_ID = 4010;
            public const ushort CCLINK_WW_PANEL2_CELL_ID_1 = 4012;
            public const ushort CCLINK_WW_PANEL2_CELL_ID_2 = 4014;
            public const ushort CCLINK_WW_PANEL2_CELL_ID_3 = 4016;
            public const ushort CCLINK_WW_PANEL2_CELL_ID_4 = 4018;
            public const ushort CCLINK_WW_CAMERA_CALIB_SELECT = 4020;
            public const ushort CCLINK_WW_CAMERA_CALIB_NO = 4022;
            public const ushort CCLINK_WW_STAGE_X_CUR_POS = 4024;
            public const ushort CCLINK_WW_STAGE_Y_CUR_POS = 4026;
            public const ushort CCLINK_WW_PANEL_CORNER_OFFSET = 4028;
            public const ushort CCLINK_WW_4030 = 4030;

            /// <summary>
            /// Write Word 영역
            /// </summary>
            //[D1056]
            public const ushort CCLINK_WW_CAMERA1_SEARCH1_X = 5000;
            public const ushort CCLINK_WW_CAMERA1_SEARCH1_Y = 5002;
            public const ushort CCLINK_WW_CAMERA1_SEARCH1_R_T = 5004;   // Beam Size Check Top
            public const ushort CCLINK_WW_CAMERA1_SEARCH1_SCORE = 5006; // Beam Size Check Right
            public const ushort CCLINK_WW_CAMERA1_SEARCH2_X = 5008;
            public const ushort CCLINK_WW_CAMERA1_SEARCH2_Y = 5010;
            public const ushort CCLINK_WW_CAMERA1_SEARCH2_R_T = 5012;   // Beam Size Check Bottom
            public const ushort CCLINK_WW_CAMERA1_SEARCH2_SCORE = 5014; // Beam Size Check Left
            public const ushort CCLINK_WW_CAMERA1_SEARCH3_X = 5016;
            public const ushort CCLINK_WW_CAMERA1_SEARCH3_Y = 5018;
            public const ushort CCLINK_WW_CAMERA1_SEARCH3_R_T = 5020;
            public const ushort CCLINK_WW_CAMERA1_SEARCH3_SCORE = 5022;
            public const ushort CCLINK_WW_CAMERA2_SEARCH1_X = 5024;
            public const ushort CCLINK_WW_CAMERA2_SEARCH1_Y = 5026;
            public const ushort CCLINK_WW_CAMERA2_SEARCH1_R_T = 5028;   // Beam Size Check Top
            public const ushort CCLINK_WW_CAMERA2_SEARCH1_SCORE = 5030; // Beam Size Check Right

            //[D1048]
            public const ushort CCLINK_WW_CAMERA2_SEARCH2_X = 5032;
            public const ushort CCLINK_WW_CAMERA2_SEARCH2_Y = 5034;
            public const ushort CCLINK_WW_CAMERA2_SEARCH2_R_T = 5036;   // Beam Size Check Bottom
            public const ushort CCLINK_WW_CAMERA2_SEARCH2_SCORE = 5038; // Beam Size Check Left
            public const ushort CCLINK_WW_CAMERA2_SEARCH3_X = 5050;
            public const ushort CCLINK_WW_CAMERA2_SEARCH3_Y = 5042;
            public const ushort CCLINK_WW_CAMERA2_SEARCH3_R_T = 5044;
            public const ushort CCLINK_WW_CAMERA2_SEARCH3_SCORE = 5046;
            public const ushort CCLINK_WW_5048 = 5048;
            public const ushort CCLINK_WW_CAMERA_CALIB_STAGE_MOVE_DIST_X = 5050;
            public const ushort CCLINK_WW_CAMERA_CALIB_STAGE_MOVE_DIST_Y = 5052;
            public const ushort CCLINK_WW_CAMERA_CALIB_STAGE_MOVE_ANGLE_T = 5054;
            public const ushort CCLINK_WW_5056 = 5056;
            public const ushort CCLINK_WW_5058 = 5058;
            public const ushort CCLINK_WW_5060 = 5060;
            public const ushort CCLINK_WW_5062 = 5062;
            #endregion

            #region BP_LPA_PC2
            public const int CCLINK2_BX_MAX_NUM = 4;
            public const int CCLINK2_BYI_MAX_NUM = 0;
            public const int CCLINK2_BYO_MAX_NUM = 4;
            public const int CCLINK2_WR_MAX_NUM = 32;
            public const int CCLINK2_WW_MAX_NUM = 64;

            /// <summary>
            /// BIT IN(X-PLC) 영역
            /// </summary>
            //[X5E0]
            public const ushort CCLINK2_IN_ALIVE = 1000;
            public const ushort CCLINK2_IN_AUTO_READY = 1001;
            public const ushort CCLINK2_IN_AUTO_RUN = 1002;
            public const ushort CCLINK2_IN_ERROR = 1003;
            public const ushort CCLINK2_IN_WARNING = 1004;
            public const ushort CCLINK2_IN_PM_MODE = 1005;
            public const ushort CCLINK2_IN_1CYCLE_MODE = 1006;
            public const ushort CCLINK2_IN_EMERGENCY_STOP = 1007;
            public const ushort CCLINK2_IN_DOOR_LOCK = 1008;
            public const ushort CCLINK2_IN_1009 = 1009;
            public const ushort CCLINK2_IN_1020 = 1020;
            public const ushort CCLINK2_IN_1011 = 1011;
            public const ushort CCLINK2_IN_1012 = 1012;
            public const ushort CCLINK2_IN_1013 = 1013;
            public const ushort CCLINK2_IN_1014 = 1014;
            public const ushort CCLINK2_IN_1015 = 1015;

            //[X5F0]
            public const ushort CCLINK2_IN_LOC_PPID_COPY_REQ = 1016;
            public const ushort CCLINK2_IN_LOC_PPID_DELETE_REQ = 1017;
            public const ushort CCLINK2_IN_LOC_PPID_BODY_CHANGE_REQ = 1018;
            public const ushort CCLINK2_IN_LOC_PPID_MODEL_CHANGE_REQ = 1019;
            public const ushort CCLINK2_IN_LOC_ECM_CHANGE_REQ = 1020;
            public const ushort CCLINK2_IN_1021 = 1021;
            public const ushort CCLINK2_IN_1022 = 1022;
            public const ushort CCLINK2_IN_1023 = 1023;
            public const ushort CCLINK2_IN_RMT_PPID_CREATE_REQ = 1024;
            public const ushort CCLINK2_IN_RMT_PPID_DELETE_REQ = 1025;
            public const ushort CCLINK2_IN_RMT_PPID_BODY_CHANGE_REQ = 1026;
            public const ushort CCLINK2_IN_RMT_PPID_BODY_SEARCH_REQ = 1027;
            public const ushort CCLINK2_IN_1028 = 1028;
            public const ushort CCLINK2_IN_TIME_CHANGE_REQ = 1029;
            public const ushort CCLINK2_IN_1030 = 1030;
            public const ushort CCLINK2_IN_1031 = 1031;

            //[X600]
            public const ushort CCLINK2_IN_FIRST_ALIGN_CALIB_REQ = 1032;
            public const ushort CCLINK2_IN_PRE_ALIGN1_CALIB_REQ = 1033;
            public const ushort CCLINK2_IN_PRE_ALIGN2_CALIB_REQ = 1034;
            public const ushort CCLINK2_IN_FIRST_ALIGN_FORCE_GRAB_REQ = 1035;
            public const ushort CCLINK2_IN_PRE_ALIGN1_FORCE_GRAB_REQ = 1036;
            public const ushort CCLINK2_IN_PRE_ALIGN2_FORCE_GRAB_REQ = 1037;
            public const ushort CCLINK2_IN_1038 = 1038;
            public const ushort CCLINK2_IN_1039 = 1039;
            public const ushort CCLINK2_IN_1040 = 1040;
            public const ushort CCLINK2_IN_1041 = 1041;
            public const ushort CCLINK2_IN_1042 = 1042;
            public const ushort CCLINK2_IN_1043 = 1043;
            public const ushort CCLINK2_IN_1044 = 1044;
            public const ushort CCLINK2_IN_1045 = 1045;
            public const ushort CCLINK2_IN_FIRST_ALIGN_STAGE_MOVE_COMP = 1046;
            public const ushort CCLINK2_IN_PRE_ALIGN_STAGE_MOVE_COMP = 1047;

            //[X610]
            public const ushort CCLINK2_IN_FIRST_ALIGN_SEARCH_REQ_X   = 1048;
            public const ushort CCLINK2_IN_FIRST_ALIGN_RESULT_OK_CHK_X = 1049;
            public const ushort CCLINK2_IN_FIRST_ALIGN_RESULT_NG_CHK_X = 1050;
            public const ushort CCLINK2_IN_PRE_ALIGN1_SEARCH_REQ_X    = 1051;
            public const ushort CCLINK2_IN_PRE_ALIGN1_RESULT_OK_CHK_X = 1052;
            public const ushort CCLINK2_IN_PRE_ALIGN1_RESULT_NG_CHK_X = 1053;
            public const ushort CCLINK2_IN_PRE_ALIGN2_SEARCH_REQ_X    = 1054;
            public const ushort CCLINK2_IN_PRE_ALIGN2_RESULT_OK_CHK_X = 1055;
            public const ushort CCLINK2_IN_PRE_ALIGN2_RESULT_NG_CHK_X = 1056;
            public const ushort CCLINK2_IN_1057 = 1057;
            public const ushort CCLINK2_IN_1058 = 1058;
            public const ushort CCLINK2_IN_1059 = 1059;
            public const ushort CCLINK2_IN_1060 = 1060;
            public const ushort CCLINK2_IN_1061 = 1061;
            public const ushort CCLINK2_IN_1062 = 1062;
            public const ushort CCLINK2_IN_1063 = 1063;

            /// <summary>
            /// BIT IN(Y-PC) 영역
            /// </summary>


            /// <summary>
            /// BIT OUT Y(VIS) 영역
            /// </summary>
            //[Y5E0]
            public const ushort CCLINK2_OUT_ALIVE = 3000;
            public const ushort CCLINK2_OUT_AUTO_READY = 3001;
            public const ushort CCLINK2_OUT_AUTO_RUN = 3002;
            public const ushort CCLINK2_OUT_ERROR = 3003;
            public const ushort CCLINK2_OUT_WARNING = 3004;
            public const ushort CCLINK2_OUT_PM_MODE = 3005;
            public const ushort CCLINK2_OUT_1CYCLE_MODE = 3006;
            public const ushort CCLINK2_OUT_3007 = 3007;
            public const ushort CCLINK2_OUT_VISION_BUSY = 3008;
            public const ushort CCLINK2_OUT_3009 = 3009;
            public const ushort CCLINK2_OUT_3020 = 3020;
            public const ushort CCLINK2_OUT_3011 = 3011;
            public const ushort CCLINK2_OUT_3012 = 3012;
            public const ushort CCLINK2_OUT_3013 = 3013;
            public const ushort CCLINK2_OUT_3014 = 3014;
            public const ushort CCLINK2_OUT_BUZZER_OFF = 3015;

            //[Y5F0]
            public const ushort CCLINK2_OUT_LOC_PPID_COPY_COMP = 3016;
            public const ushort CCLINK2_OUT_LOC_PPID_DELETE_COMP = 3017;
            public const ushort CCLINK2_OUT_LOC_PPID_BODY_CHANGE_COMP = 3018;
            public const ushort CCLINK2_OUT_LOC_PPID_MODEL_CHANGE_COMP = 3019;
            public const ushort CCLINK2_OUT_LOC_ECM_CHANGE_COMP = 3020;
            public const ushort CCLINK2_OUT_3021 = 3021;
            public const ushort CCLINK2_OUT_3022 = 3022;
            public const ushort CCLINK2_OUT_3023 = 3023;
            public const ushort CCLINK2_OUT_RMT_PPID_CREATE_COMP = 3024;
            public const ushort CCLINK2_OUT_RMT_PPID_DELETE_COMP = 3025;
            public const ushort CCLINK2_OUT_RMT_PPID_BODY_CHANGE_COMP = 3026;
            public const ushort CCLINK2_OUT_RMT_PPID_BODY_SEARCH_COMP = 3027;
            public const ushort CCLINK2_OUT_3028 = 3028;
            public const ushort CCLINK2_OUT_TIME_CHANGE_COMP = 3029;
            public const ushort CCLINK2_OUT_3030 = 3030;
            public const ushort CCLINK2_OUT_3031 = 3031;

            //[Y600]
            public const ushort CCLINK2_OUT_FIRST_ALIGN_CALIB_COMP = 3032;
            public const ushort CCLINK2_OUT_PRE_ALIGN1_CALIB_COMP = 3033;
            public const ushort CCLINK2_OUT_PRE_ALIGN2_CALIB_COMP = 3034;
            public const ushort CCLINK2_OUT_FIRST_ALIGN_FORCE_GRAB_COMP = 3035;
            public const ushort CCLINK2_OUT_PRE_ALIGN1_FORCE_GRAB_COMP = 3036;
            public const ushort CCLINK2_OUT_PRE_ALIGN2_FORCE_GRAB_COMP = 3037;
            public const ushort CCLINK2_OUT_3038 = 3038;
            public const ushort CCLINK2_OUT_3039 = 3039;
            public const ushort CCLINK2_OUT_3040 = 3040;
            public const ushort CCLINK2_OUT_3041 = 3041;
            public const ushort CCLINK2_OUT_3042 = 3042;
            public const ushort CCLINK2_OUT_3043 = 3043;
            public const ushort CCLINK2_OUT_3044 = 3044;
            public const ushort CCLINK2_OUT_3045 = 3045;
            public const ushort CCLINK2_OUT_FIRST_ALIGN_STAGE_MOVE_REQ = 3046;
            public const ushort CCLINK2_OUT_PRE_ALIGN_STAGE_MOVE_REQ = 3047;

            //[Y610]
            public const ushort CCLINK2_OUT_FIRST_ALIGN_SEARCH_COMP_X = 3048;
            public const ushort CCLINK2_OUT_FIRST_ALIGN_RESULT_OK_X = 3049;
            public const ushort CCLINK2_OUT_FIRST_ALIGN_RESULT_NG_X = 3050;
            public const ushort CCLINK2_OUT_PRE_ALIGN1_SEARCH_COMP_X = 3051;
            public const ushort CCLINK2_OUT_PRE_ALIGN1_RESULT_OK_X = 3052;
            public const ushort CCLINK2_OUT_PRE_ALIGN1_RESULT_NG_X = 3053;
            public const ushort CCLINK2_OUT_PRE_ALIGN2_SEARCH_COMP_X = 3054;
            public const ushort CCLINK2_OUT_PRE_ALIGN2_RESULT_OK_X = 3055;
            public const ushort CCLINK2_OUT_PRE_ALIGN2_RESULT_NG_X = 3056;
            public const ushort CCLINK2_OUT_3057 = 3057;
            public const ushort CCLINK2_OUT_FIRST_ALIGN_CROSS_ANGLE_NG = 3058;
            public const ushort CCLINK2_OUT_FIRST_ALIGN_VERTICAL_ANGLE_NG = 3059;
            public const ushort CCLINK2_OUT_3060 = 3060;
            public const ushort CCLINK2_OUT_3061 = 3061;
            public const ushort CCLINK2_OUT_3062 = 3062;
            public const ushort CCLINK2_OUT_3063 = 3063;

            /// <summary>
            /// Read Word 영역
            /// </summary>
            //[D2000]
            public const ushort CCLINK2_WR_PANEL1_CELL_ID = 4000;
            public const ushort CCLINK2_WR_PANEL1_CELL_ID_1 = 4002;
            public const ushort CCLINK2_WR_PANEL1_CELL_ID_2 = 4004;
            public const ushort CCLINK2_WR_PANEL1_CELL_ID_3 = 4006;
            public const ushort CCLINK2_WR_PANEL1_CELL_ID_4 = 4008;
            public const ushort CCLINK2_WR_4010 = 4010;
            public const ushort CCLINK2_WR_4012 = 4012;
            public const ushort CCLINK2_WR_4014 = 4014;
            public const ushort CCLINK2_WR_FIRST_ALIGN_STAGE_X_POS = 4016;
            public const ushort CCLINK2_WR_FIRST_ALIGN_STAGE_Y_POS = 4018;
            public const ushort CCLINK2_WR_FIRST_ALIGN_STAGE_T_POS = 4020;
            public const ushort CCLINK2_WR_PRE_ALIGN_STAGE_X_POS = 4022;
            public const ushort CCLINK2_WR_PRE_ALIGN_STAGE_Y_POS = 4024;
            public const ushort CCLINK2_WR_PRE_ALIGN_STAGE_T_POS = 4026;
            public const ushort CCLINK2_WR_4028 = 4028;
            public const ushort CCLINK2_WR_4030 = 4030;

            /// <summary>
            /// Write Word 영역
            /// </summary>
            //[D1056]
            public const ushort CCLINK2_WW_FIRST_ALIGN_SEARCH_X = 5000;
            public const ushort CCLINK2_WW_FIRST_ALIGN_SEARCH_Y = 5002;
            public const ushort CCLINK2_WW_FIRST_ALIGN_SEARCH_R_T = 5004;  
            public const ushort CCLINK2_WW_FIRST_ALIGN_SEARCH_SCORE = 5006;
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH1_X = 5008;
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH1_Y = 5010;
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH1_R_T = 5012;   
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH1_SCORE = 5014;       
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH2_X = 5016;
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH2_Y = 5018;
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH2_R_T = 5020;
            public const ushort CCLINK2_WW_PRE_ALIGN_SEARCH2_SCORE = 5022;
            public const ushort CCLINK2_WW_5024 = 5024;
            public const ushort CCLINK2_WW_5026 = 5026;
            public const ushort CCLINK2_WW_5028 = 5028;
            public const ushort CCLINK2_WW_5030 = 5030;
            #endregion
        }
        #endregion

        public static void CCLink_Initialize(int xStartAddr, int yReadStartAddr, int yWriteStartAddr, int wrStartAddr, int wwStartAddr, int xUsedSize, int yReadUsedSize, int yWriteUsedSize, int wrUsedSize, int wwUsedSize, int xMapStartIdx, int yMapReadStartIdx, int yMapWriteStartIdx, int wrMapStartIdx, int wwMapStartIdx, int wwMapEndIdx)
        {
       //    int iResult = ERR.SUCCESS;
       //
       //    Main.CCLink.SetAddress(xStartAddr, yReadStartAddr, yWriteStartAddr, wrStartAddr, wwStartAddr, xMapStartIdx, yMapReadStartIdx, yMapWriteStartIdx, wrMapStartIdx, wwMapStartIdx, wwMapEndIdx);
       //    Main.CCLink.SetArrSize(xUsedSize, yReadUsedSize, yWriteUsedSize, wrUsedSize, wwUsedSize);
       //
       //    iResult = Main.CCLink.Initialize();
       //    if (iResult != ERR.SUCCESS)
       //    {
       //        //Update_Error(ERR_CCLINK_IO_COMM_FAIL);
       //        MessageBox.Show("CCLink, Initial Board Fail !", Main.DEFINE.CMD);
       //    }
       //
       //    iResult = CCLink.LoadMap(Main.DEFINE.DEF_IO_MAP_FILE);
       //    if (iResult != ERR.SUCCESS)
       //    {
       //        //Update_Error(ERR_CCLINK_IO_COMM_FAIL);
       //        //Save_SystemLog("CCLink, Load Map Fail !", Main.DEFINE.CMD);
       //    }
       //
       //    iResult = CCLink.RunThread();
       //    if (iResult != ERR.SUCCESS)
       //    {
       //        //Update_Error(ERR_CCLINK_IO_COMM_FAIL);
       //        MessageBox.Show("CCLink, Run Thread Fail !", Main.DEFINE.CMD);
       //    }
        }

        /// <summary>
        /// Command 받고 수행 후 완료 handshake로 주로 쓰임
        /// </summary>
        /// <param name="nWriteAddr">내가 처음 살릴 비트</param>
        /// <param name="nReadAddr">꺼지는지 확인할 상대 (커맨드) 비트</param>
        /// <param name="iTimeoutms">체크 타임아웃</param>
        /// <returns></returns>
        public static bool CCLink_CommandHandshake(ushort nWriteAddr, ushort nReadAddr, long iTimeoutms, int iAlignUnitNo)
        {
          // MTickTimer tTimer = new MTickTimer();
          //
          // string strMsg;
          //
          // Main.CCLink_PutBit(nWriteAddr, true);
          // Thread.Sleep(100);
          //
          // tTimer.StartTimer();
          //
          // while (tTimer.GetElapsedTime() <= iTimeoutms)
          // {
          //     if (Main.CCLink_IsBit(nReadAddr) == false)
          //     {
          //         Thread.Sleep(30);
          //
          //         Main.CCLink_PutBit(nWriteAddr, false);
          //
          //         //strMsg = "[Command Handshake] " + nWriteAddr.ToString() + "->" + nReadAddr.ToString();
          //         //AlignUnit[iAlignUnitNo].LogdataDisplay(strMsg, true);
          //
          //         return true; 
          //     }
          //
          //     Thread.Sleep(10);
          // }
          //
          // if (tTimer.GetElapsedTime() > iTimeoutms)
          // {
          //     // Time Over Err
          //     strMsg = "[Err] Command Handshake Time Out Err";
          //     Main.CCLink_PutBit(nWriteAddr, false);
          //     AlignUnit[iAlignUnitNo].LogdataDisplay(strMsg, true);
          //     
          //     return false;
          // }

            return true;
        }

        /// <summary>
        /// 상대 특정 비트 켜지는거 확인 후 내 비트를 끔
        /// </summary>
        /// <param name="nReadAddr">켜졌는지 확인할 상대 비트</param>
        /// <param name="nWriteAddr">nReadAddr가 켜졌을 때 끌 나의 비트</param>
        /// <param name="iTimeoutms">체크 타임아웃</param>
        /// <returns></returns>
        public static bool CCLink_OnCheckOffHandshake(ushort nReadAddr, ushort nWriteAddr, long iTimeoutms, int iAlignUnitNo)
        {
        ///MTickTimer tTimer = new MTickTimer();
        ///
        ///string strMsg;
        ///
        ///tTimer.StartTimer();
        ///
        ///while (tTimer.GetElapsedTime() <= iTimeoutms)
        ///{
        ///	if (Main.CCLink_IsBit(nReadAddr) == true)
        ///	{
        ///        Thread.Sleep(30);
        ///
        ///        Main.CCLink_PutBit(nWriteAddr, false);
        ///
        ///        //strMsg = "[OnCheckOff Handshake] " + nReadAddr + ", " + nWriteAddr;
        ///        //AlignUnit[iAlignUnitNo].LogdataDisplay(strMsg, true);
        ///
        ///        return true;
        ///    }
        ///
        ///    Thread.Sleep(10);
        ///}
        ///
        ///if (tTimer.GetElapsedTime() > iTimeoutms)
        ///{
        ///    // Time Over Err
        ///    strMsg = "[Err] Complete Handshake Time Out Err";
        ///    Main.CCLink_PutBit(nWriteAddr, false);
        ///    AlignUnit[iAlignUnitNo].LogdataDisplay(strMsg, true);
        ///
        ///    return false;
        ////}

            return true;
        }

        /// <summary>
        /// 상대 특정 비트 꺼지는거 확인 후 내 비트를 끔
        /// </summary>
        /// <param name="nReadAddr">꺼졌는지 확인할 상대 비트</param>
        /// <param name="nWriteAddr">nReadAddr가 켜졌을 때 끌 나의 비트</param>
        /// <param name="iTimeoutms">체크 타임아웃</param>
        /// <returns></returns>
        public static bool CCLink_OffCheckOffHandshake(ushort nReadAddr, ushort nWriteAddr, long iTimeoutms, int iAlignUnitNo)
        {
         //  MTickTimer tTimer = new MTickTimer();
         //
         //  string strMsg;
         //
         //  tTimer.StartTimer();
         //
         //  while (tTimer.GetElapsedTime() <= iTimeoutms)
         //  {
         //      if (Main.CCLink_IsBit(nReadAddr) == false)
         //      {
         //          Thread.Sleep(30);
         //
         //          Main.CCLink_PutBit(nWriteAddr, false);
         //
         //          //strMsg = "[OffCheckOff Handshake] " + nReadAddr + ", " + nWriteAddr;
         //          //AlignUnit[iAlignUnitNo].LogdataDisplay(strMsg, true);
         //
         //          return true;
         //      }
         //
         //      Thread.Sleep(10);
         //  }
         //
         //  if (tTimer.GetElapsedTime() > iTimeoutms)
         //  {
         //      // Time Over Err
         //      strMsg = "[Err] Complete Handshake Time Out Err";
         //
         //      Main.CCLink_PutBit(nWriteAddr, false);
         //
         //      AlignUnit[iAlignUnitNo].LogdataDisplay(strMsg, true);
         //
         //      return false;
         //  }

            return true;
        }

        public static bool CCLink_IsBit(ushort usMap)
        {
            return false;

           // return Main.CCLink.IsBit(usMap);
        }

        public static int CCLink_PutBit(ushort usMap, bool bVal)
        {
            return 1;
        }

        public static int CCLink_WriteWord(ushort usMap, short sVal)
        {
            return 1;
        }

        public static int CCLink_WriteDWord(ushort usMap, int nVal)
        {
            return 1;
        }

        public static short CCLink_ReadWord(ushort usMap)
        {
            return 1;
        }

        public static int CCLink_ReadDWord(ushort usMap)
        {
            return 1;
        }
        
        public static void CCLinkTerminate()
        {
            //CCLink.Terminate();
        }
    }
#endif
}
