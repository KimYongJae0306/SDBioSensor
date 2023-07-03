using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32; //Regedit_Password
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.CNLSearch;
using Cognex.VisionPro.Caliper;
using Cognex.VisionPro.Blob;
using Cognex.VisionPro.SearchMax;
using Cognex.VisionPro.LineMax;
using Cognex.VisionPro.ImageProcessing;

namespace COG
{
    public partial class Main
    {
        public class DataFileTag
        {
            #region DataFileDefine
            String m_FileName;
            [DllImport("kernel32.dll")]
            private static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath);

            [DllImport("kernel32.dll")]
            private static extern long WritePrivateProfileString(String section, String key, String val, String filePath);

            public void SetFileName(String FileName)
            {
                m_FileName = FileName;
            }
            public void SetData(String Section, String key, int DataValue)
            {
                WritePrivateProfileString(Section, key, DataValue.ToString(), m_FileName);
            }
            public void SetData(String Section, String key, double DataValue)
            {
                WritePrivateProfileString(Section, key, DataValue.ToString(), m_FileName);
            }
            public void SetData(String Section, String key, string DataValue)
            {
                WritePrivateProfileString(Section, key, DataValue, m_FileName);
            }
            public void SetData(String Section, String key, string DataValue, string nFileName)
            {
                long nRet = WritePrivateProfileString(Section, key, DataValue, nFileName);
                if (nRet == 0)
                {
                    nRet = Marshal.GetLastWin32Error();
                    //Main.AlignUnit[0].LogdataDisplay("ERROR : " + nRet.ToString(), true);
                }
            }
            public void SetData(String Section, String key, bool DataValue)
            {
                WritePrivateProfileString(Section, key, DataValue.ToString(), m_FileName);
            }
            public int GetIData(String Section, String Key)
            {
                StringBuilder temp = new StringBuilder(80);
                string temp_return;
                int i = GetPrivateProfileString(Section, Key, "0", temp, 80, m_FileName);
                temp_return = temp.ToString();
                return Convert.ToInt32(temp_return);
            }
            public double GetFData(String Section, String Key)
            {
                StringBuilder temp = new StringBuilder(80);
                string temp_return;
                int i = GetPrivateProfileString(Section, Key, "0", temp, 80, m_FileName);
                temp_return = temp.ToString();
                return Convert.ToDouble(temp_return);
            }
            public String GetSData(String Section, String Key)
            {
                StringBuilder temp = new StringBuilder(80);
                int i = GetPrivateProfileString(Section, Key, " ", temp, 80, m_FileName);
                return temp.ToString();
            }
            public bool GetBData(String Section, String Key)
            {
                bool Ret;
                StringBuilder temp = new StringBuilder(80);
                int i = GetPrivateProfileString(Section, Key, "false", temp, 80, m_FileName);
                bool.TryParse(temp.ToString(), out Ret);
                return Ret;
            }
            #endregion
        }


        public static DataFileTag SystemFile = new DataFileTag();
        public static DataFileTag OldLogCheckFile = new DataFileTag();
        public static DataFileTag ModelFile = new DataFileTag();

        public static string ProjectName;
        public static string ProjectInfo;
        public static string SysdataPath;
        public static string ModelPath;
        public static string LogdataPath;
        public static string ErrdataPath;
        public static string CamdataPath;



        #region PASSWORD
        public static void WriteRegistry(string _Mode, string _Password)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Main.DEFINE.PASSWORD_DIR, RegistryKeyPermissionCheck.ReadWriteSubTree);
            regKey.SetValue(_Mode, _Password, RegistryValueKind.String);
        }
        public static string ReadRegistry(string _Mode)
        {
            RegistryKey reg = Registry.CurrentUser;
            reg = reg.OpenSubKey(Main.DEFINE.PASSWORD_DIR, true);

            if (reg == null) return "";

            if (null != reg.GetValue(_Mode))
            {
                return Convert.ToString(reg.GetValue(_Mode));
            }
            else
            {
                return "";
            }
        }
        public static void DeleteRegistry()
        {
            Registry.CurrentUser.DeleteSubKey(Main.DEFINE.PASSWORD_DIR);
        }
        #endregion
        #region LANGUAGE
        public static void WriteRegistryLan(string _Mode)
        {
            RegistryKey regKey = Registry.CurrentUser.CreateSubKey(Main.DEFINE.LANGUAGE_DIR, RegistryKeyPermissionCheck.ReadWriteSubTree);

            regKey.SetValue("language", _Mode, RegistryValueKind.String);
        }
        public static int ReadRegistryLan()
        {
            string _Mode = "language";
            RegistryKey reg = Registry.CurrentUser;
            reg = reg.OpenSubKey(Main.DEFINE.LANGUAGE_DIR, true);

            if (reg == null) return Main.DEFINE.KOREA;

            if (null != reg.GetValue(_Mode))
            {
                return Convert.ToInt32(reg.GetValue(_Mode));
            }
            else
            {
                return Convert.ToInt32(Main.DEFINE.KOREA);
            }
        }
        #endregion


        #region INITIAL
        public static void System_Initial()
        {
            string buf;

            SysdataPath = DEFINE.SYS_DATADIR;
            ModelPath = DEFINE.SYS_DATADIR + "MODEL_" + DEFINE.MODEL_DATADIR + "\\";
            LogdataPath = DEFINE.SYS_DATADIR + DEFINE.LOG_DATADIR;
            ErrdataPath = DEFINE.SYS_DATADIR + DEFINE.ERROR_DATADIR;
            CamdataPath = DEFINE.SYS_DATADIR + DEFINE.CAM_SETDIR;

            buf = SysdataPath + "SYSTEM_" + DEFINE.MODEL_DATADIR + ".ini";
            SystemFile.SetFileName(buf);

            buf = SysdataPath + "OLD_LOG_CHECK_FILE.dat";
            OldLogCheckFile.SetFileName(buf);

            if (!Directory.Exists(SysdataPath)) Directory.CreateDirectory(SysdataPath);
            if (!Directory.Exists(CamdataPath)) Directory.CreateDirectory(CamdataPath);
            if (!Directory.Exists(ModelPath)) Directory.CreateDirectory(ModelPath);
            if (!Directory.Exists(LogdataPath)) Directory.CreateDirectory(LogdataPath);
            if (!Directory.Exists(ErrdataPath)) Directory.CreateDirectory(ErrdataPath);

            Status.MC_STATUS = DEFINE.MC_STOP;

        }
        public static void Model_Initial()
        {
            string buf;

            ModelPath = DEFINE.SYS_DATADIR + "MODEL_" + DEFINE.MODEL_DATADIR + "\\";
            buf = ModelPath + ProjectName + "\\Model.ini";
            ModelFile.SetFileName(buf);

            if (!Directory.Exists(ModelPath)) Directory.CreateDirectory(ModelPath);
        }
        #endregion

        #region Project
        public static bool ProjectRename(string _modelName, string _modelInfo)
        {
            bool nRet = true;

            if (!Directory.Exists(ModelPath))
            {
                nRet = false;
            }
            if (!Directory.Exists(ModelPath + _modelName))
            {
                nRet = false;
            }

            string buf;
            buf = ModelPath + _modelName + "\\Model.ini";
            ModelFile.SetData("PROJECT", "NAME", _modelInfo, buf);
            return nRet;
        }
        public static void ProjectSave(string _modelName, string _modelInfo)
        {
            string buf;
            ProjectName = _modelName;
            ProjectInfo = _modelInfo;

            buf = ModelPath + ProjectName + "\\Model.ini";
            ModelFile.SetFileName(buf);
            SystemFile.SetData("SYSTEM", "LAST_PROJECT", ProjectName);

            if (!Directory.Exists(ModelPath)) Directory.CreateDirectory(ModelPath);
            if (!Directory.Exists(ModelPath + ProjectName)) Directory.CreateDirectory(ModelPath + ProjectName);

            ModelFile.SetData("PROJECT", "NAME", _modelInfo);

            for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
            {
                for (int j = 0; j < Main.AlignUnit[i].m_AlignPatTagMax; j++)
                {
                    AlignUnit[i].Save(j);
                }
            }
        }
        public static bool ProjectLoad(string _modelName)
        {
            string buf;
            if (!Directory.Exists(ModelPath))
            {
                MessageBox.Show(ModelPath + "not Directory", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (!Directory.Exists(ModelPath + _modelName))
            {
                return false;
            }
            if (ProjectName == _modelName)
            {
                return true;
            }
            ProjectName = _modelName;
            buf = ModelPath + ProjectName + "\\Model.ini";
            ModelFile.SetFileName(buf);
            ProjectInfo = ModelFile.GetSData("PROJECT", "NAME");
            SystemFile.SetData("SYSTEM", "LAST_PROJECT", ProjectName);

            //2022 05 09 YSH
            Main.WriteDevice(PLCDataTag.BASE_RW_ADDR + DEFINE.CURRENT_MODEL_CODE, Convert.ToInt16(Main.ProjectName));

            //int[] setValue = new int[1];
            //setValue[0] = Convert.ToInt16(Main.ProjectName);
            //Main.PLCsocket.WriteDevice_W((PLCDataTag.BASE_RW_ADDR + Main.DEFINE.CURRENT_MODEL_CODE).ToString(), 1, setValue);


            Main.ProgerssBar_Unit(Main.formProgressBar, DEFINE.AlignUnit_Max, true, 0);
            for (int i = 0; i < Main.DEFINE.AlignUnit_Max; i++)
            {
                //  for (int j = 0; j < Main.AlignUnit[i].m_AlignPatTagMax; j++)
                for (int j = AlignUnit[i].m_AlignPatTagMax - 1; j >= 0; j--)
                {
                    AlignUnit[i].Load(j);
                }
                Main.ProgerssBar_Unit(Main.formProgressBar, DEFINE.AlignUnit_Max, true, i + 1);
            }
            return true;
        }

        public static bool ProjectCopy(string _sourceName, string _targetName)
        {
            string strModelInfo = "";
            if (!Directory.Exists(ModelPath))
            {
                AlignUnit[0].LogdataDisplay(ModelPath + " Directory not exists", true);
                return false;
            }
            if (!Directory.Exists(ModelPath + _sourceName))
            {
                AlignUnit[0].LogdataDisplay(ModelPath + _sourceName + " Directory not exists", true);
                return false;
            }
            if (Directory.Exists(ModelPath + _targetName))
            {
                AlignUnit[0].LogdataDisplay(ModelPath + _targetName + " Directory already exists", true);
                return false;
            }

            if (FolderCopy(ModelPath + _sourceName, ModelPath + _targetName))
            {
                for (int i = 0; i < 10; i++)
                {
                    char m_CharData;
                    long dataNum;
                    string m_strData;

                    dataNum = PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.PPID_COPY_MODEL_NAME + i] & 0x00ff;
                    m_CharData = Convert.ToChar(dataNum);
                    m_strData = m_CharData.ToString();
                    if (m_strData == "\0") break;
                    strModelInfo += m_strData;

                    dataNum = (PLCDataTag.RData[DEFINE.MX_ARRAY_RSTAT_OFFSET + Main.DEFINE.PPID_COPY_MODEL_NAME + i] >> 8) & 0x00ff;
                    m_CharData = Convert.ToChar(dataNum);
                    m_strData = m_CharData.ToString();
                    if (m_strData == "\0") break;
                    strModelInfo += m_strData;
                }

                AlignUnit[0].LogdataDisplay(_targetName + " - " + strModelInfo + " Model Copy Success", true);

                MODEL_COPY = true;
                MODEL_COPY_NAME = _targetName;
                MODEL_COPY_INFO = strModelInfo;

                ProjectRename(_targetName, strModelInfo);

                return true;
            }
            else
            {
                //AlignUnit[0].LogdataDisplay(_targetName + " Model Copy Fail", true);
                return false;
            }
        }

        public static bool ProjectDelete(string _modelName)
        {
            if (Directory.Exists(Main.ModelPath + _modelName))
            {
                string[] arrFile = Directory.GetFiles(Main.ModelPath + _modelName);

                for (int i = 0; i < arrFile.Length; i++)
                {
                    DirectoryInfo DI = new DirectoryInfo(arrFile[i]);
                    File.Delete(DI.FullName);
                }
            }
            else
            {
                Main.AlignUnit[0].LogdataDisplay("Source Or Dest Path  Not Exist", true);
                return false;
            }

            Directory.Delete(Main.ModelPath + _modelName);

            return true;
        }

        #endregion
        public static void CenterXYSave(int m_AlignNo)
        {
            for (int j = 0; j < Main.AlignUnit[m_AlignNo].m_AlignPatTagMax; j++)
            {
                SystemFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CENTER_X_" + j.ToString(), AlignUnit[m_AlignNo].m_CenterX[j]);
                SystemFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CENTER_Y_" + j.ToString(), AlignUnit[m_AlignNo].m_CenterY[j]);
                ModelFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CENTER_X_" + j.ToString(), AlignUnit[m_AlignNo].m_CenterX[j]);
                ModelFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CENTER_Y_" + j.ToString(), AlignUnit[m_AlignNo].m_CenterY[j]);

                SystemFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALMOTOR_X_" + j.ToString(), AlignUnit[m_AlignNo].m_CalMotoPosX[j]);
                SystemFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALMOTOR_Y_" + j.ToString(), AlignUnit[m_AlignNo].m_CalMotoPosY[j]);
                ModelFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALMOTOR_X_" + j.ToString(), AlignUnit[m_AlignNo].m_CalMotoPosX[j]);
                ModelFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALMOTOR_Y_" + j.ToString(), AlignUnit[m_AlignNo].m_CalMotoPosY[j]);

                SystemFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALDISPLAY_X_" + j.ToString(), AlignUnit[m_AlignNo].m_CalDisplayCX[j]);
                SystemFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALDISPLAY_Y_" + j.ToString(), AlignUnit[m_AlignNo].m_CalDisplayCY[j]);
                ModelFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALDISPLAY_X_" + j.ToString(), AlignUnit[m_AlignNo].m_CalDisplayCX[j]);
                ModelFile.SetData(AlignUnit[m_AlignNo].m_AlignName, "CALDISPLAY_Y_" + j.ToString(), AlignUnit[m_AlignNo].m_CalDisplayCY[j]);
            }
        }
        public static void SystemSave()
        {
            //-------------------------------------------
            // OPTION
            //-------------------------------------------
            SystemFile.SetData("OPTION", "OVELAY_IMAGE_SAVE", machine.Overlay_Image_Onf);
            SystemFile.SetData("OPTION", "GAP_LOG_MSG", machine.LogMsg_Onf);
            SystemFile.SetData("OPTION", "INSPECTION", machine.Inspection_Onf);
            SystemFile.SetData("OPTION", "L_CHECK", machine.LengthCheck_Onf);
            SystemFile.SetData("OPTION", "BMP", machine.BMP_ImageSave_Onf);
            SystemFile.SetData("OPTION", "OLD_LOG_PERIOD", machine.m_nOldLogCheckPeriod);
            SystemFile.SetData("OPTION", "OLD_LOG_SPACE", machine.m_nOldLogCheckSpace);
            SystemFile.SetData("OPTION", "CCLINK_COMM_DELAY", machine.m_nCCLinkCommDelay_ms);
            if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1")
            {
                SystemFile.SetData("OPTION", "USE_LOADING_LIMIT", machine.m_bUseLoadingLimit);
                SystemFile.SetData("OPTION", "LOADING_LIMIT_X", machine.m_nLoadingLimitX_um);
                SystemFile.SetData("OPTION", "LOADING_LIMIT_Y", machine.m_nLoadingLimitY_um);
                SystemFile.SetData("OPTION", "INSP_LIMIT_LOW", machine.m_nInspLowLimit_um);
                SystemFile.SetData("OPTION", "INSP_LIMIT_HIGH", machine.m_nInspHighLimit_um);
            }
            else if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2")
            {
                SystemFile.SetData("OPTION", "USE_ALIGN1_ANGLE_LIMIT", machine.m_bUseAlign1AngleLimit);
                SystemFile.SetData("OPTION", "ALIGN1_CORNER_LIMIT", machine.m_f1stAlignCornerAngleLimit);
                SystemFile.SetData("OPTION", "ALIGN1_VERTICAL_LIMIT", machine.m_f1stAlignVerticalAngleLimit);
            }
            SystemFile.SetData("SYSTEM", "PLC_READ_DATA", PLCDataTag.BASE_RW_ADDR);
            SystemFile.SetData("SYSTEM", "LAST_PROJECT", ProjectName);
            SystemFile.SetData("UVW", "STAGE_R", Main.UVW.STAGE_R);
            SystemFile.SetData("UVW", "LIMIT_X", Main.Common.Limit_X);
            SystemFile.SetData("UVW", "LIMIT_Y", Main.Common.Limit_Y);
            SystemFile.SetData("UVW", "LIMIT_T", Main.Common.Limit_T);
            SystemFile.SetData("LIMIT", "LIMIT_ANGLE", Main.Common.Limit_Angle);
            SystemFile.SetData("RETRY", "USE", Main.machine.m_bRetryUse);
            SystemFile.SetData("RETRY", "COUNT", Main.machine.m_RetryCount);

            Main.ProgerssBar_Unit(Main.formProgressBar, DEFINE.AlignUnit_Max, true, 0);

            for (int i = 0; i < DEFINE.AlignUnit_Max; i++)
            {

                // Memory Address
                SystemFile.SetData(AlignUnit[i].m_AlignName, "ALIGN_UNIT_ADDR", AlignUnit[i].ALIGN_UNIT_ADDR);
                // parameter
                SystemFile.SetData(AlignUnit[i].m_AlignName, "CAL_MOVE_X", AlignUnit[i].m_Cal_MOVE_X);
                SystemFile.SetData(AlignUnit[i].m_AlignName, "CAL_MOVE_Y", AlignUnit[i].m_Cal_MOVE_Y);
                SystemFile.SetData(AlignUnit[i].m_AlignName, "CAL_MOVE_T1", AlignUnit[i].m_Cal_MOVE_T1);
                SystemFile.SetData(AlignUnit[i].m_AlignName, "CAL_MOVE_T2", AlignUnit[i].m_Cal_MOVE_T2);

                SystemFile.SetData(AlignUnit[i].m_AlignName, "CAM_OFFSET_X", AlignUnit[i].m_CamOffsetX);
                SystemFile.SetData(AlignUnit[i].m_AlignName, "CAM_OFFSET_Y", AlignUnit[i].m_CamOffsetY);

                SystemFile.SetData(AlignUnit[i].m_AlignName, "STANDARD_MARK_T", AlignUnit[i].m_StandardMark_T);
                // Motor AXIS DIR
                SystemFile.SetData(AlignUnit[i].m_AlignName, "DIR_X", AlignUnit[i].m_DirX);

                SystemFile.SetData(AlignUnit[i].m_AlignName, "GD_IMAGE", AlignUnit[i].m_GD_ImageSave_Use);
                SystemFile.SetData(AlignUnit[i].m_AlignName, "NG_IMAGE", AlignUnit[i].m_NG_ImageSave_Use);

                //                  SystemFile.SetData(AlignUnit[i].m_AlignName, "LENGTH_CHECK_USE", AlignUnit[i].m_LengthCheck_Use);
                // 
                //                  SystemFile.SetData(AlignUnit[i].m_AlignName, "STANDARDMARK_LENGTH_OBJ", AlignUnit[i].m_OBJ_Standard_Length);
                //                  SystemFile.SetData(AlignUnit[i].m_AlignName, "STANDARDMARK_LENGTH_TAR", AlignUnit[i].m_TAR_Standard_Length);

                for (int j = 0; j < AlignUnit[i].m_AlignPatTagMax; j++)
                {
                    SystemFile.SetData(AlignUnit[i].m_AlignName, "CENTER_X_" + j.ToString(), AlignUnit[i].m_CenterX[j]);
                    SystemFile.SetData(AlignUnit[i].m_AlignName, "CENTER_Y_" + j.ToString(), AlignUnit[i].m_CenterY[j]);
                    ModelFile.SetData(AlignUnit[i].m_AlignName, "CENTER_X_" + j.ToString(), AlignUnit[i].m_CenterX[j]);
                    ModelFile.SetData(AlignUnit[i].m_AlignName, "CENTER_Y_" + j.ToString(), AlignUnit[i].m_CenterY[j]);

                    SystemFile.SetData(AlignUnit[i].m_AlignName, "CALMOTOR_X_" + j.ToString(), AlignUnit[i].m_CalMotoPosX[j]);
                    SystemFile.SetData(AlignUnit[i].m_AlignName, "CALMOTOR_Y_" + j.ToString(), AlignUnit[i].m_CalMotoPosY[j]);
                    ModelFile.SetData(AlignUnit[i].m_AlignName, "CALMOTOR_X_" + j.ToString(), AlignUnit[i].m_CalMotoPosX[j]);
                    ModelFile.SetData(AlignUnit[i].m_AlignName, "CALMOTOR_Y_" + j.ToString(), AlignUnit[i].m_CalMotoPosY[j]);

                    SystemFile.SetData(AlignUnit[i].m_AlignName, "CALDISPLAY_X_" + j.ToString(), AlignUnit[i].m_CalDisplayCX[j]);
                    SystemFile.SetData(AlignUnit[i].m_AlignName, "CALDISPLAY_Y_" + j.ToString(), AlignUnit[i].m_CalDisplayCY[j]);
                    ModelFile.SetData(AlignUnit[i].m_AlignName, "CALDISPLAY_X_" + j.ToString(), AlignUnit[i].m_CalDisplayCX[j]);
                    ModelFile.SetData(AlignUnit[i].m_AlignName, "CALDISPLAY_Y_" + j.ToString(), AlignUnit[i].m_CalDisplayCY[j]);

                    AlignUnit[i].Save(j);
                }
                Main.ProgerssBar_Unit(Main.formProgressBar, DEFINE.AlignUnit_Max, true, i + 1);
            }
        }
        public static void SystemLoad()
        {
            //-------------------------------------------
            // OPTION
            //-------------------------------------------
            machine.Overlay_Image_Onf = SystemFile.GetBData("OPTION", "OVELAY_IMAGE_SAVE");
            machine.LogMsg_Onf = SystemFile.GetBData("OPTION", "GAP_LOG_MSG");
            machine.Inspection_Onf = SystemFile.GetBData("OPTION", "INSPECTION");
            machine.LengthCheck_Onf = SystemFile.GetBData("OPTION", "L_CHECK");
            machine.BMP_ImageSave_Onf = SystemFile.GetBData("OPTION", "BMP");
            machine.m_nOldLogCheckPeriod = SystemFile.GetIData("OPTION", "OLD_LOG_PERIOD");
            machine.m_nOldLogCheckSpace = SystemFile.GetIData("OPTION", "OLD_LOG_SPACE");
            machine.m_nCCLinkCommDelay_ms = SystemFile.GetIData("OPTION", "CCLINK_COMM_DELAY");
            machine.m_RetryCount = SystemFile.GetIData("RETRY", "COUNT");
            machine.m_bRetryUse = SystemFile.GetBData("RETRY", "USE");
            machine.m_EngineerPassword = SystemFile.GetSData("PERMISSION_ENGINEER", "PASSWORD");
            machine.m_MakerPassword = SystemFile.GetSData("PERMISSION_MAKER", "PASSWORD");
         
            //-------------------------------------------
            // SYSTEM
            //-------------------------------------------
            ProjectName = SystemFile.GetSData("SYSTEM", "LAST_PROJECT");
            Main.Common.Limit_X = SystemFile.GetIData("UVW", "LIMIT_X");
            Main.Common.Limit_Y = SystemFile.GetIData("UVW", "LIMIT_Y");
            Main.Common.Limit_T = SystemFile.GetIData("UVW", "LIMIT_T");

            Main.Common.Limit_Angle = SystemFile.GetFData("LIMIT", "LIMIT_ANGLE");
            Model_Initial();
            ProjectInfo = ModelFile.GetSData("PROJECT", "NAME");

            Main.ProgerssBar_Unit(Main.formProgressBar, DEFINE.AlignUnit_Max, true, 0);
            //Task T1 = Task.Factory.StartNew(() =>
            //{
            //Parallel.For(0, DEFINE.AlignUnit_Max, (i) =>
            for (int i = 0; i < DEFINE.AlignUnit_Max; i++)
            {
                // parameter
                AlignUnit[i].m_Cal_MOVE_X = SystemFile.GetIData(AlignUnit[i].m_AlignName, "CAL_MOVE_X");
                AlignUnit[i].m_Cal_MOVE_Y = SystemFile.GetIData(AlignUnit[i].m_AlignName, "CAL_MOVE_Y");
                AlignUnit[i].m_Cal_MOVE_T1 = SystemFile.GetIData(AlignUnit[i].m_AlignName, "CAL_MOVE_T1");
                AlignUnit[i].m_Cal_MOVE_T2 = SystemFile.GetIData(AlignUnit[i].m_AlignName, "CAL_MOVE_T2");
                AlignUnit[i].m_StandardMark_T = SystemFile.GetFData(AlignUnit[i].m_AlignName, "STANDARD_MARK_T");
                //  Motor AXIS DIR
                AlignUnit[i].m_CamOffsetX = SystemFile.GetIData(AlignUnit[i].m_AlignName, "CAM_OFFSET_X");
                AlignUnit[i].m_CamOffsetY = SystemFile.GetIData(AlignUnit[i].m_AlignName, "CAM_OFFSET_Y");

                AlignUnit[i].m_GD_ImageSave_Use = SystemFile.GetBData(AlignUnit[i].m_AlignName, "GD_IMAGE");
                AlignUnit[i].m_NG_ImageSave_Use = SystemFile.GetBData(AlignUnit[i].m_AlignName, "NG_IMAGE");

                for (int j = AlignUnit[i].m_AlignPatTagMax - 1; j >= 0; j--)
                {
                    AlignUnit[i].m_CenterX[j] = ModelFile.GetFData(AlignUnit[i].m_AlignName, "CENTER_X_" + j.ToString());
                    AlignUnit[i].m_CenterY[j] = ModelFile.GetFData(AlignUnit[i].m_AlignName, "CENTER_Y_" + j.ToString());

                    AlignUnit[i].m_CalMotoPosX[j] = ModelFile.GetFData(AlignUnit[i].m_AlignName, "CALMOTOR_X_" + j.ToString());
                    AlignUnit[i].m_CalMotoPosY[j] = ModelFile.GetFData(AlignUnit[i].m_AlignName, "CALMOTOR_Y_" + j.ToString());

                    AlignUnit[i].m_CalDisplayCX[j] = ModelFile.GetFData(AlignUnit[i].m_AlignName, "CALDISPLAY_X_" + j.ToString());
                    AlignUnit[i].m_CalDisplayCY[j] = ModelFile.GetFData(AlignUnit[i].m_AlignName, "CALDISPLAY_Y_" + j.ToString());
                    AlignUnit[i].Load(j);
                }


                Main.ProgerssBar_Unit(Main.formProgressBar, DEFINE.AlignUnit_Max, true, i + 1);
            }
            // });
            //Task.WaitAll(T1);

        }
        public static void SaveOldLogCheckFile()
        {
            OldLogCheckFile.SetData("SYSTEM", "LAST_CHECK", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public static bool FileCopy(string strOriginFile, string strCopyFile)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(strOriginFile);
            long iSize = 0;
            long iTotalSize = fi.Length; //1024 버퍼 사이즈 임의로...
            byte[] bBuf = new byte[104857600]; //동일 파일이 존재하면 삭제 하고 다시하기 위해... 

            if (System.IO.File.Exists(strCopyFile))
            {
                System.IO.File.Delete(strCopyFile);
            } //원본 파일 열기...
            System.IO.FileStream fsIn = new System.IO.FileStream(strOriginFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read); //대상 파일 만들기...
            System.IO.FileStream fsOut = new System.IO.FileStream(strCopyFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            while (iSize < iTotalSize)
            {
                try
                {
                    int iLen = fsIn.Read(bBuf, 0, bBuf.Length); iSize += iLen; fsOut.Write(bBuf, 0, iLen);
                }
                catch (Exception ex)
                { //파일 연결 해제...
                    fsOut.Flush();
                    fsOut.Close();
                    fsIn.Close(); //에러시 삭제... 
                    if (System.IO.File.Exists(strCopyFile))
                    {
                        System.IO.File.Delete(strCopyFile);
                    }
                }
                return false;
            }
            //파일 연결 해제... 
            fsOut.Flush();
            fsOut.Close();
            fsIn.Close();
            return true;

        }

        public static bool FolderCopy(string strOriginFolder, string strCopyFolder)
        {
            //폴더가 없으면 만듬...
            if (!System.IO.Directory.Exists(strCopyFolder))
            {
                System.IO.Directory.CreateDirectory(strCopyFolder);
            }
            //파일 목록 불러오기...
            string[] files = System.IO.Directory.GetFiles(strOriginFolder);
            //폴더 목록 불러오기... 
            string[] folders = System.IO.Directory.GetDirectories(strOriginFolder);

            Main.ProgerssBar_Unit(Main.formProgressBar, files.Length, true, 0);
            int i = 0;
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileName(file);
                string dest = System.IO.Path.Combine(strCopyFolder, name);

                FileCopy(file, dest);
                Main.ProgerssBar_Unit(Main.formProgressBar, files.Length, true, i++ + 1);
            }
            // foreach 안에서 재귀 함수를 통해서 폴더 복사 및 파일 복사 진행 완료  
            foreach (string folder in folders)
            {
                string name = System.IO.Path.GetFileName(folder);
                string dest = System.IO.Path.Combine(strCopyFolder, name);

                FolderCopy(folder, dest);
            }

            return true;
        }

        public partial class AlignUnitTag
        {
            public void Save(int m_PatTagNo)
            {
                Main.ProgerssBar_PAT(Main.formProgressBar_1, m_AlignPatMax[m_PatTagNo], true, 0, "Saving");
                for (int i = 0; i < m_AlignPatMax[m_PatTagNo]; i++)
                {
                    PAT[m_PatTagNo, i].Save();
                    Main.ProgerssBar_PAT(Main.formProgressBar_1, m_AlignPatMax[m_PatTagNo], true, i + 1, "Saving");
                }

                ModelFile.SetData(m_AlignName, "CENTER_X_" + m_PatTagNo.ToString(), m_CenterX[m_PatTagNo]);
                ModelFile.SetData(m_AlignName, "CENTER_Y_" + m_PatTagNo.ToString(), m_CenterY[m_PatTagNo]);

                ModelFile.SetData(m_AlignName, "CALMOTOR_X_" + m_PatTagNo.ToString(), m_CalMotoPosX[m_PatTagNo]);
                ModelFile.SetData(m_AlignName, "CALMOTOR_Y_" + m_PatTagNo.ToString(), m_CalMotoPosY[m_PatTagNo]);

                ModelFile.SetData(m_AlignName, "CALDISPLAY_X_" + m_PatTagNo.ToString(), m_CalDisplayCX[m_PatTagNo]);
                ModelFile.SetData(m_AlignName, "CALDISPLAY_Y_" + m_PatTagNo.ToString(), m_CalDisplayCY[m_PatTagNo]);

                //--------------------------------------------------------------------------------------------------
                ModelFile.SetData("OPTION", "MAP_FUNCTION", machine.MAP_Function_Onf);
                ModelFile.SetData("OPTION", "MAP_FUNCTION_DATA", machine.MAP_Function_Data);

                ModelFile.SetData("OPTION", "MAP_Limit", machine.MAP_Limit_Onf);
                ModelFile.SetData("OPTION", "MAP_HIGH", machine.MAP_High);
                ModelFile.SetData("OPTION", "MAP_LOW", machine.MAP_Low);

                /*
                ModelFile.SetData("OPTION", "PICKER1_DIS_X", machine.m_Fpcpicker1_Dis_X);
                ModelFile.SetData("OPTION", "PICKER1_DIS_Y", machine.m_Fpcpicker1_Dis_Y);
                ModelFile.SetData("OPTION", "PICKER2_DIS_X", machine.m_Fpcpicker2_Dis_X);
                ModelFile.SetData("OPTION", "PICKER2_DIS_Y", machine.m_Fpcpicker2_Dis_Y);
                */
                //PARAMETER
                ModelFile.SetData(m_AlignName, "STANDARD_X", m_Standard[Main.DEFINE.X]);
                ModelFile.SetData(m_AlignName, "STANDARD_Y", m_Standard[Main.DEFINE.Y]);
                ModelFile.SetData(m_AlignName, "STANDARD_T", m_Standard[Main.DEFINE.T]);
                ModelFile.SetData(m_AlignName, "REPEATE", m_RepeatLimit);
                ModelFile.SetData(m_AlignName, "LENGTH_CHECK_USE", m_LengthCheck_Use);
                ModelFile.SetData(m_AlignName, "STANDARDMARK_LENGTH_OBJ", m_OBJ_Standard_Length);
                ModelFile.SetData(m_AlignName, "STANDARDMARK_LENGTH_TAR", m_TAR_Standard_Length);
                ModelFile.SetData(m_AlignName, "LENGTH_TOL", m_Length_Tolerance);
                ModelFile.SetData(m_AlignName, "TRAY_POCKET_X", m_Tray_Pocket_X);
                ModelFile.SetData(m_AlignName, "TRAY_POCKET_Y", m_Tray_Pocket_Y);
                ModelFile.SetData(m_AlignName, "TRAY_BLOB_MODE", TrayBlobMode);





                ModelFile.SetData(m_AlignName, "ALIGN_DELAY", m_AlignDelay);
                ModelFile.SetData(m_AlignName, "BLOB_NG_VIEW", m_Blob_NG_View_Use);
                //--------------------------------------------------------------------------------------------------
                Main.ProgerssBar_PAT(Main.formProgressBar_1, m_AlignPatMax[m_PatTagNo], false, 0, "Saving");
            }

            public void Load(int m_PatTagNo)
            {
                Main.ProgerssBar_PAT(Main.formProgressBar_1, m_AlignPatMax[m_PatTagNo], true, 0, "Loading");

                //              for (int i = 0; i < m_AlignPatMax; i++)
                for (int i = m_AlignPatMax[m_PatTagNo] - 1; i >= 0; i--)
                {
                    PAT[m_PatTagNo, i].Load();
                    if (i == DEFINE.CAM_SELECT_INSPECT)
                    {
                        PAT[m_PatTagNo, i].m_CamOffsetX = this.m_CamOffsetX / 1000.0;   // um to mm
                        PAT[m_PatTagNo, i].m_CamOffsetY = this.m_CamOffsetY / 1000.0;
                    }
                    Main.ProgerssBar_PAT(Main.formProgressBar_1, m_AlignPatMax[m_PatTagNo], true, i + 1, "Loading");

                }
                //--------------------------------------------------------------------------------------------------
                machine.MAP_Function_Onf = ModelFile.GetBData("OPTION", "MAP_FUNCTION");
                machine.MAP_Function_Data = ModelFile.GetFData("OPTION", "MAP_FUNCTION_DATA");

                machine.MAP_Limit_Onf = ModelFile.GetBData("OPTION", "MAP_Limit");
                machine.MAP_High = ModelFile.GetIData("OPTION", "MAP_HIGH");
                machine.MAP_Low = ModelFile.GetIData("OPTION", "MAP_LOW");

                /*
                machine.m_Fpcpicker1_Dis_X = ModelFile.GetIData("OPTION", "PICKER1_DIS_X");
                machine.m_Fpcpicker1_Dis_Y = ModelFile.GetIData("OPTION", "PICKER1_DIS_Y");
                machine.m_Fpcpicker2_Dis_X = ModelFile.GetIData("OPTION", "PICKER2_DIS_X");
                machine.m_Fpcpicker2_Dis_Y = ModelFile.GetIData("OPTION", "PICKER2_DIS_Y");
                */
                // PARAMETER
                m_Standard[Main.DEFINE.X] = ModelFile.GetFData(m_AlignName, "STANDARD_X");
                m_Standard[Main.DEFINE.Y] = ModelFile.GetFData(m_AlignName, "STANDARD_Y");
                m_Standard[Main.DEFINE.T] = ModelFile.GetFData(m_AlignName, "STANDARD_T");
                m_RepeatLimit = ModelFile.GetIData(m_AlignName, "REPEATE");

                m_LengthCheck_Use = ModelFile.GetBData(m_AlignName, "LENGTH_CHECK_USE");
                m_OBJ_Standard_Length = ModelFile.GetFData(m_AlignName, "STANDARDMARK_LENGTH_OBJ");
                m_TAR_Standard_Length = ModelFile.GetFData(m_AlignName, "STANDARDMARK_LENGTH_TAR");
                m_Length_Tolerance = ModelFile.GetFData(m_AlignName, "LENGTH_TOL");

                if (ModelFile.GetIData(m_AlignName, "TRAY_POCKET_X") > 0)
                    m_Tray_Pocket_X = ModelFile.GetIData(m_AlignName, "TRAY_POCKET_X");
                else
                    m_Tray_Pocket_X = 1;

                if (ModelFile.GetIData(m_AlignName, "TRAY_POCKET_Y") > 0)
                    m_Tray_Pocket_Y = ModelFile.GetIData(m_AlignName, "TRAY_POCKET_Y");
                else
                    m_Tray_Pocket_Y = 1;

                TrayBlobMode = ModelFile.GetBData(m_AlignName, "TRAY_BLOB_MODE");

                m_AlignDelay = ModelFile.GetIData(m_AlignName, "ALIGN_DELAY");
                m_Blob_NG_View_Use = ModelFile.GetBData(m_AlignName, "BLOB_NG_VIEW");

                m_CenterX[m_PatTagNo] = ModelFile.GetFData(m_AlignName, "CENTER_X_" + m_PatTagNo.ToString());
                m_CenterY[m_PatTagNo] = ModelFile.GetFData(m_AlignName, "CENTER_Y_" + m_PatTagNo.ToString());

                m_CalMotoPosX[m_PatTagNo] = ModelFile.GetFData(m_AlignName, "CALMOTOR_X_" + m_PatTagNo.ToString());
                m_CalMotoPosY[m_PatTagNo] = ModelFile.GetFData(m_AlignName, "CALMOTOR_Y_" + m_PatTagNo.ToString());

                m_CalDisplayCX[m_PatTagNo] = ModelFile.GetFData(m_AlignName, "CALDISPLAY_X_" + m_PatTagNo.ToString());
                m_CalDisplayCY[m_PatTagNo] = ModelFile.GetFData(m_AlignName, "CALDISPLAY_Y_" + m_PatTagNo.ToString());



                //--------------------------------------------------------------------------------------------------
                Main.ProgerssBar_PAT(Main.formProgressBar_1, m_AlignPatMax[m_PatTagNo], false, 0, "Loading");
            }
        }
        public partial class PatternTag
        {
            public void Save()
            {
                string buf;


                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    buf = "LIGHTCTRL_" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_LightCtrl[i]);
                    buf = "LIGHTCH_" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_LightCH[i]);
                    buf = "LIGHTNAME_" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_Light_Name[i]);
                }
                //---------------------------ModelFile-----------------------------------
                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    for (int j = 0; j < Main.DEFINE.Light_ToolMaxCount; j++)
                    {
                        buf = "LIGHT" + i.ToString() + "_" + j.ToString();
                        ModelFile.SetData(m_PatternName, buf, m_LightValue[i, j]);
                    }
                }
                //------------------------SystemFile-------------------------------------
                for (int i = 0; i < 2; i++)
                {
                    buf = "CAL_X" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_CalX[i]);
                    buf = "CAL_Y" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_CalY[i]);
                }

                for (int i = 0; i < 9; i++)
                {
                    buf = "CALMATRIX" + i.ToString();
                    //                    if(!Main.DEFINE.OPEN_F)
                    SystemFile.SetData(m_PatternName, buf, CALMATRIX[i]);
                }
                buf = "CCD_T_X";
                SystemFile.SetData(m_PatternName, buf, CAMCCDTHETA[0, 0]);
                buf = "CCD_T_Y";
                SystemFile.SetData(m_PatternName, buf, CAMCCDTHETA[0, 1]);

                buf = "MANU_MATCH_USE";
                ModelFile.SetData(m_PatternName, buf, m_Manu_Match_Use);

                buf = "PMALIGN_USE";
                ModelFile.SetData(m_PatternName, buf, m_PMAlign_Use);

                buf = "LINEMAX_USE";
                ModelFile.SetData(m_PatternName, buf, m_UseLineMax);

                buf = "CUSTOM_CROSS_USE";
                ModelFile.SetData(m_PatternName, buf, Main.vision.USE_CUSTOM_CROSS[m_CamNo]);

                buf = "CUSTOM_CROSS_X";
                ModelFile.SetData(m_PatternName, buf, Main.vision.CUSTOM_CROSS_X[m_CamNo]);

                buf = "CUSTOM_CROSS_Y";
                ModelFile.SetData(m_PatternName, buf, Main.vision.CUSTOM_CROSS_Y[m_CamNo]);



                String ModelDir = ModelPath;
                if (Main.Status.MC_MODE != Main.DEFINE.MC_SETUPFORM && Main.Status.MC_STATUS != Main.DEFINE.MC_RUN)
                {
                    //----------------------------------------------------------------------


                    #region PATTERN

                    String PatFileName;
                    for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                    {
                        PatFileName = ModelDir + ProjectName + "\\" + m_PatternName + "_" + i.ToString() + ".vpp";
                        try
                        {
                            Pattern[i].InputImage = null;
                            CogSerializer.SaveObjectToFile(Pattern[i], PatFileName);
                        }
                        catch (System.Exception ex)
                        {
                            //   MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                        }
                    }
                    for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                    {
                        PatFileName = ModelDir + ProjectName + "\\" + m_PatternName + "_" + "G" + i.ToString() + ".vpp";
                        try
                        {
                            GPattern[i].InputImage = null;
                            CogSerializer.SaveObjectToFile(GPattern[i], PatFileName);
                        }
                        catch (System.Exception ex)
                        {
                            //   MessageBox.Show(ex.Source + ex.Message + ex.StackTrace);
                        }
                    }

                    for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                    {
                        buf = "PATUSE" + i.ToString();
                        ModelFile.SetData(m_PatternName, buf, Pattern_USE[i]);
                    }
                    buf = "ACCEPT_SCORE";
                    ModelFile.SetData(m_PatternName, buf, m_ACCeptScore);
                    buf = "ACCEPT_GSCORE";
                    ModelFile.SetData(m_PatternName, buf, m_GACCeptScore);

                    buf = "CALIPER_MARKUSE";
                    ModelFile.SetData(m_PatternName, buf, Caliper_MarkUse);

                    buf = "BLOB_MARKUSE";
                    ModelFile.SetData(m_PatternName, buf, Blob_MarkUse);

                    buf = "BLOB_CALIPERUSE";
                    ModelFile.SetData(m_PatternName, buf, Blob_CaliperUse);

                    buf = "BLOB_INSPECT_CNT";
                    ModelFile.SetData(m_PatternName, buf, m_Blob_InspCnt);

                    buf = "FINDLINE_MARKUSE";
                    ModelFile.SetData(m_PatternName, buf, FINDLine_MarkUse);

                    buf = "CIRCLE_MARKUSE";
                    ModelFile.SetData(m_PatternName, buf, Circle_MarkUse);
                    #endregion


                }
                for (int i = 0; i < 2; i++)
                {
                    buf = "Left Origin" + i.ToString();
                    ModelFile.SetData(m_PatternName, buf, LeftOrigin[i]);

                    buf = "Right Origin" + i.ToString();
                    ModelFile.SetData(m_PatternName, buf, RightOrigin[i]);
                }
                #region SD BIO
                string InspectionName;
                //String ModelDir = ModelPath;
                //if (m_PatternName == "INSPECTION_1" || m_PatternName == "INSPECTION_2")
                {

                    var SaveData = m_InspParameter;
                    for (int i = 0; i < m_InspParameter.Count; i++)
                    {
                        InspectionName = m_PatternName + "_" + i.ToString();

                        buf = "Insp_Spec_Dist" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].dSpecDistance);

                        buf = "Insp_Spec_Dist_Max" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].dSpecDistanceMax);

                        buf = "Insp_Dist_Ingnore" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].IDistgnore);

                        buf = "Insp_Edge_Threshold_Use" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].bThresholdUse);

                        buf = "Insp_Edge_Threshold" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iThreshold);

                        if (i == 0)
                        {
                            buf = "COUNT";
                            ModelFile.SetData(InspectionName, buf, m_InspParameter.Count);
                            buf = "IMAGE CENTER X";
                            ModelFile.SetData(InspectionName, buf, SaveData[0].CenterX);
                            buf = "IMAGE CENTER Y";
                            ModelFile.SetData(InspectionName, buf, SaveData[0].CenterY);
                            buf = "IMAGE LENTH X";
                            ModelFile.SetData(InspectionName, buf, SaveData[0].LenthX);
                            buf = "IMAGE LENTH Y";
                            ModelFile.SetData(InspectionName, buf, SaveData[0].LenthY);
                            buf = "Histogram ROI Count";
                            ModelFile.SetData(InspectionName, buf, SaveData[0].iHistogramROICnt);
                            int HistogramRoiCnt = SaveData[0].iHistogramROICnt;
                         
                            for (int j = 0; j < HistogramRoiCnt; j++)
                            {
                                string FileName2 = ModelDir + ProjectName + "\\" + "Historam" + InspectionName + "_" + j.ToString() + ".Vpp";
                                CogSerializer.SaveObjectToFile(SaveData[i].m_CogHistogramTool[j], FileName2, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.ExcludeDataBindings);
                                buf = " Histogram Spec" + j.ToString();
                                ModelFile.SetData(InspectionName, buf, SaveData[0].iHistogramSpec[j]);
                            }

                        }
                        buf = "INSPECTION TYPE" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, Convert.ToInt32(SaveData[i].m_enumROIType));
                        string FileName = ModelDir + ProjectName + "\\" + "FindLine_" + InspectionName + ".VPP";
                        CogSerializer.SaveObjectToFile(SaveData[i].m_FindLineTool, FileName, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.ExcludeDataBindings);
                        FileName = ModelDir + ProjectName + "\\" + "Circle_" + InspectionName + ".VPP";
                        CogSerializer.SaveObjectToFile(SaveData[i].m_FindCircleTool, FileName, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.ExcludeDataBindings);


                        buf = "Insp_Top_Cut_Pixel" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iTopCutPixel);
                        buf = "Insp_Bottom_Cut_Pixel" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iBottomCutPixel);
                        buf = "Insp_Masking_Value" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iMaskingValue);
                        buf = "Insp_Ignore_Size" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iIgnoreSize);

                        buf = "Insp_Edge_Caliper_TH" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iEdgeCaliperThreshold);

                        buf = "Insp_Edge_Caliper_Filter_Size" + i.ToString();
                        ModelFile.SetData(InspectionName, buf, SaveData[i].iEdgeCaliperFilterSize);
                    }

                }
                //if (m_PatternName == "INSPECTION_1ALIGN INSPECTION" || m_PatternName == "INSPECTION_2ALIGN INSPECTION") //cyh0811
                //{
                for (int i = 0; i < 4; i++)
                {
                    InspectionName = m_PatternName + "_" + i.ToString();
                    string FileName = ModelDir + ProjectName + "\\" + "TrackingLine" + InspectionName + ".VPP";
                    CogSerializer.SaveObjectToFile(m_TrackingLine[i], FileName, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.ExcludeDataBindings);
                }
                //}
                //Bonding Align Save - shkang_s
                for (int i = 0; i < 4; i++)
                {
                    InspectionName = m_PatternName + "_" + i.ToString();
                    string FileName = ModelDir + ProjectName + "\\" + "BondingAlignCaliperLine" + InspectionName + ".VPP";
                    CogSerializer.SaveObjectToFile(m_BondingAlignLine[i], FileName, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.ExcludeDataBindings);
                }

                buf = "BondingAlign_OriginDistX";
                ModelFile.SetData(m_PatternName, buf, m_dOriginDistanceX);
                buf = "BondingAlign_OriginDistY";
                ModelFile.SetData(m_PatternName, buf, m_dOriginDistanceY);
                buf = "BondingAlign_DistSpecX";
                ModelFile.SetData(m_PatternName, buf, m_dDistanceSpecX);
                buf = "BondingAlign_DistSpecY";
                ModelFile.SetData(m_PatternName, buf, m_dDistanceSpecY);
                buf = "Object_Distance_X";
                ModelFile.SetData(m_PatternName, buf, m_dObjectDistanceX);
                buf = "Object_Distance_X_Spec";
                ModelFile.SetData(m_PatternName, buf, m_dObjectDistanceSpecX);
                //shkang_e

                /////////////////////////////////////
                //ROI Finealign
                for (int i = 0; i < Main.DEFINE.Pattern_Max; i++)
                {
                    for (int j = 0; j < Main.DEFINE.SUBPATTERNMAX; j++)
                    {
                        InspectionName = m_PatternName + "_" + i.ToString() + j.ToString();
                        string FileName = ModelDir + ProjectName + "\\" + "ROIFineAlign" + InspectionName + ".VPP";
                        CogSerializer.SaveObjectToFile(m_FinealignMark[i, j], FileName, typeof(System.Runtime.Serialization.Formatters.Binary.BinaryFormatter), CogSerializationOptionsConstants.ExcludeDataBindings);
                    }
                }
                buf = "ROIFinealign_Flag";
                ModelFile.SetData(m_PatternName, buf, m_bFInealignFlag);

                buf = "ROIFinealign_T_Spec";
                ModelFile.SetData(m_PatternName, buf, m_FinealignThetaSpec);

                buf = "ROIFinealign_MarkScore";
                ModelFile.SetData(m_PatternName, buf, m_FinealignMarkScore);
                /////////////////////////////////////

                buf = "InspDirectionChange_Flag";
                ModelFile.SetData(m_PatternName, buf, m_bInspDirectionChangeFlag);

                #endregion

                #region CALIPER

                //string m_CaliName;
                //for (int i = 0; i < Main.DEFINE.CALIPER_MAX; i++)
                //{
                //    m_CaliName = m_PatternName + "_CALIPER";
                //    buf = "CALIPER_CENTERX" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].Region.CenterX);

                //    buf = "CALIPER_CENTERY" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].Region.CenterY);

                //    buf = "CALIPER_SIZEX" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].Region.SideXLength);

                //    buf = "CALIPER_SIZEY" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].Region.SideYLength);

                //    buf = "CALIPER_THRESHOLD" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].RunParams.ContrastThreshold);

                //    buf = "CALIPER_DIRECTION" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].Region.Rotation);

                //    buf = "CALIPER_POLARLITY" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, Convert.ToInt16(CaliperTools[i].RunParams.Edge0Polarity));

                //    buf = "CALIPER_EDGE_MODE" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, Convert.ToInt16(CaliperTools[i].RunParams.EdgeMode));

                //    buf = "CALIPER_POSITION_0" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].RunParams.Edge0Position);

                //    buf = "CALIPER_POSITION_1" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].RunParams.Edge1Position);

                //    buf = "CALIPER_USE" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperPara[i].m_UseCheck);

                //    buf = "Caliper FilterHSPixel" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperTools[i].RunParams.FilterHalfSizeInPixels);

                //    buf = "CALIPER_COP_MODE" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperPara[i].m_bCOPMode);

                //    buf = "CALIPER_COP_DVDCOUNT" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperPara[i].m_nCOPROICnt);

                //    buf = "CALIPER_COP_DVDOFFSET" + i.ToString();
                //    ModelFile.SetData(m_CaliName, buf, CaliperPara[i].m_nCOPROIOffset);

                //    for (int k = 0; k < Main.DEFINE.M_TOOLMAXCOUNT; k++)
                //    {
                //        buf = "TARGET_TO_CENTER_X" + i.ToString() + "_" + k.ToString();
                //        ModelFile.SetData(m_CaliName, buf, CaliperPara[i].m_TargetToCenter[k].X);
                //        buf = "TARGET_TO_CENTER_Y" + i.ToString() + "_" + k.ToString();
                //        ModelFile.SetData(m_CaliName, buf, CaliperPara[i].m_TargetToCenter[k].Y);
                //    }
                //}
                #endregion

                #region BLOB 수정한거
                //CogRectangleAffine BlobRegion;
                //string m_BlobName;
                //for (int i = 0; i < Main.DEFINE.BLOB_CNT_MAX; i++)
                //{
                //    BlobRegion = new CogRectangleAffine(BlobTools[i].Region as CogRectangleAffine);

                //    m_BlobName = m_PatternName + "_BLOB";
                //    //----------------------------------------------------------------------------------------------------------
                //    buf = "BLOBUSE" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobPara[i].m_UseCheck);
                //    //------------REGION----------------------------------------------------------------------------------------
                //    buf = "BLOBREGION_CENTERX" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobRegion.CenterX);

                //    buf = "BLOBREGION_CENTERY" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobRegion.CenterY);

                //    buf = "BLOBREGION_WIDTH" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobRegion.SideXLength);

                //    buf = "BLOBREGION_HEIGHT" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobRegion.SideYLength);

                //    buf = "BLOBREGION_ROTATION" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobRegion.Rotation);

                //    buf = "BLOBREGION_SKEW" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobRegion.Skew);
                //    //----------------------------------------------------------------------------------------------------------
                //    buf = "BLOB_POLARITY" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, Convert.ToInt16(BlobTools[i].RunParams.SegmentationParams.Polarity));

                //    buf = "BLOB_MINPIXELS" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobTools[i].RunParams.ConnectivityMinPixels);

                //    buf = "BLOB_THRESHOLD" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobTools[i].RunParams.SegmentationParams.HardFixedThreshold);

                //    buf = "BLOB_AREA_MIN" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobTools[i].RunParams.RunTimeMeasures[0].FilterRangeLow);
                //    buf = "BLOB_AREA_HIGH" + i.ToString();
                //    ModelFile.SetData(m_BlobName, buf, BlobTools[i].RunParams.RunTimeMeasures[0].FilterRangeHigh);

                //    for (int k = 0; k < Main.DEFINE.M_TOOLMAXCOUNT; k++)
                //    {
                //        buf = "TARGET_TO_CENTER_X" + i.ToString() + "_" + k.ToString();
                //        ModelFile.SetData(m_BlobName, buf, BlobPara[i].m_TargetToCenter[k].X);
                //        buf = "TARGET_TO_CENTER_Y" + i.ToString() + "_" + k.ToString();
                //        ModelFile.SetData(m_BlobName, buf, BlobPara[i].m_TargetToCenter[k].Y);
                //    }
                //    //----------------------------------------------------------------------------------------------------------
                //}
                #endregion

                #region FINDLINE
                //buf = "TRAY_GUIDE_DIS_X";
                //ModelFile.SetData(m_PatternName, buf, TRAY_GUIDE_DISX);

                //buf = "TRAY_GUIDE_DIS_Y";
                //ModelFile.SetData(m_PatternName, buf, TRAY_GUIDE_DISY);

                //buf = "TRAY_PITCH_DIS_X";
                //ModelFile.SetData(m_PatternName, buf, TRAY_PITCH_DISX);

                //buf = "TRAY_PITCH_DIS_Y";
                //ModelFile.SetData(m_PatternName, buf, TRAY_PITCH_DISY);

                //string m_FINDLineName, m_LineMaxName;
                //for (int ii = 0; ii < Main.DEFINE.SUBLINE_MAX; ii++)
                //{
                //    for (int i = 0; i < Main.DEFINE.FINDLINE_MAX; i++)
                //    {
                //        m_FINDLineName = m_PatternName + "_FINDLine_" + ii.ToString();
                //        buf = "FINDLINE_STARTX" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.ExpectedLineSegment.StartX);

                //        buf = "FINDLINE_STARTY" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.ExpectedLineSegment.StartY);

                //        buf = "FINDLINE_LENGTH" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.ExpectedLineSegment.Length);

                //        buf = "FINDLINE_DIRECTION" + i.ToString();
                //        if (FINDLineTools[ii, i].RunParams.ExpectedLineSegment.StartX != FINDLineTools[ii, i].RunParams.ExpectedLineSegment.EndX
                //            && FINDLineTools[ii, i].RunParams.ExpectedLineSegment.StartY != FINDLineTools[ii, i].RunParams.ExpectedLineSegment.EndY)
                //            ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.ExpectedLineSegment.Rotation);
                //        else
                //            ModelFile.SetData(m_FINDLineName, buf, 0);

                //        buf = "FINDLINE_CALIPER_CNT" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.NumCalipers);

                //        buf = "FINDLINE_NUMTOLGNORE" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, Convert.ToInt16(FINDLineTools[ii, i].RunParams.NumToIgnore));

                //        buf = "FINDLINE_CALIPERX" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.CaliperProjectionLength); // Caliper X

                //        buf = "FINDLINE_CALIPERY" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.CaliperSearchLength);  // Caliper Y

                //        buf = "FINDLINE_CALIPER_THRESHOLD" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.CaliperRunParams.ContrastThreshold);

                //        buf = "FINDLINE_CALIPER_POLARLITY" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, Convert.ToInt16(FINDLineTools[ii, i].RunParams.CaliperRunParams.Edge0Polarity));

                //        buf = "FINDLINE_CALIPER_USE" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_UseCheck);

                //        buf = "FINDLINE_CALIPER_PAIR_USE" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_UsePairCheck);

                //        buf = "FINDLINE_CALIPER_POLARLITY_1" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, Convert.ToInt16(FINDLineTools[ii, i].RunParams.CaliperRunParams.Edge1Polarity));

                //        buf = "FINDLINE_CALIPER_FINDLine1POS" + i.ToString();    //Pos 간격 이하만 찾아라.
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.CaliperRunParams.Edge1Position);

                //        buf = "FINDLINE_CALIPER_FINDLineMODE" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, Convert.ToInt16(FINDLineTools[ii, i].RunParams.CaliperRunParams.EdgeMode));

                //        buf = "FINDLINE_CALIPER_SEARCHDIRECTION" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLineTools[ii, i].RunParams.CaliperSearchDirection);

                //        buf = "FINDLINE_FILTERHALFSIZE" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, Convert.ToInt16(FINDLineTools[ii, i].RunParams.CaliperRunParams.FilterHalfSizeInPixels));

                //        buf = "FINDLINE_CALIPER_METHOD" + i.ToString();
                //        ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_LineCaliperMethod);

                //        for (int k = 0; k < Main.DEFINE.M_TOOLMAXCOUNT; k++)
                //        {
                //            buf = "TARGET_TO_CENTER_X" + i.ToString() + "_" + k.ToString();
                //            ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_TargetToCenter[k].X);
                //            buf = "TARGET_TO_CENTER_Y" + i.ToString() + "_" + k.ToString();
                //            ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_TargetToCenter[k].Y);

                //            buf = "TARGET_TO_CENTER_2X" + i.ToString() + "_" + k.ToString();
                //            ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_TargetToCenter[k].X2);
                //            buf = "TARGET_TO_CENTER_2Y" + i.ToString() + "_" + k.ToString();
                //            ModelFile.SetData(m_FINDLineName, buf, FINDLinePara[ii, i].m_TargetToCenter[k].Y2);
                //        }

                //        //==================== LINEMAX ====================//
                //        m_LineMaxName = m_PatternName + "_LineMax_" + ii.ToString();

                //        buf = "LINEMAX_CENTERX" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, (LineMaxTools[ii, i].Region as CogRectangleAffine).CenterX);

                //        buf = "LINEMAX_CENTERY" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, (LineMaxTools[ii, i].Region as CogRectangleAffine).CenterY);

                //        buf = "LINEMAX_EXPLINE_ANGLE" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.ExpectedLineNormal.Angle);

                //        buf = "LINEMAX_GRAKERNEL_SIZE" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.EdgeDetectionParams.GradientKernelSizeInPixels);

                //        buf = "LINEMAX_PROJECTION_LENGTH" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.EdgeDetectionParams.ProjectionLengthInPixels);

                //        buf = "LINEMAX_CONTRAST_THRES" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.EdgeDetectionParams.ContrastThreshold);

                //        buf = "LINEMAX_POLARITY" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, Convert.ToInt16(LineMaxTools[ii, i].RunParams.Polarity));

                //        buf = "LINEMAX_EDGE_ANGLE_TOL" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.EdgeAngleTolerance);

                //        buf = "LINEMAX_EDGE_DIST_TOL" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.DistanceTolerance);

                //        buf = "LINEMAX_MAX_LINES" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.MaxNumLines);

                //        buf = "LINEMAX_LINE_ANGLE_TOL" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.LineAngleTolerance);

                //        buf = "LINEMAX_COVERAGE_THRES" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.CoverageThreshold);

                //        buf = "LINEMAX_LENGTH_THRES" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, LineMaxTools[ii, i].RunParams.LengthThreshold);

                //        buf = "LINEMAX_HORIZONTAL_COND" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, FINDLinePara[ii, i].m_LineMaxHCond);

                //        buf = "LINEMAX_VERTICAL_COND" + i.ToString();
                //        ModelFile.SetData(m_LineMaxName, buf, FINDLinePara[ii, i].m_LineMaxVCond);
                //    }
                //}
                #endregion

                #region CIRCLE
                //string m_CircleName;
                //for (int i = 0; i < Main.DEFINE.CIRCLE_MAX; i++)
                //{
                //    m_CircleName = m_PatternName + "_CIRCLE";
                //    buf = "CIRCLE_CENTERX" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.ExpectedCircularArc.CenterX);

                //    buf = "CIRCLE_CENTERY" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.ExpectedCircularArc.CenterY);

                //    buf = "CIRCLE_RADIUS" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.ExpectedCircularArc.Radius);

                //    buf = "RUNPARAMS_NUMCALIPERTS" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.NumCalipers);

                //    buf = "RUNPARAMS_SEARCHLENGTH" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.CaliperSearchLength);

                //    buf = "RUNPARAMS_SEARCHDIRECTION" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, Convert.ToInt16(CircleTools[i].RunParams.CaliperSearchDirection));

                //    buf = "RUNPARAMS_PROJECTIONLENGTH" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.CaliperProjectionLength);

                //    buf = "RUNPARAMS_RADIUSCONSTRAINT" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.RadiusConstraint);

                //    buf = "RUNPARAMS_RADIUSCONSTRAINT_ENABLE" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.RadiusConstraintEnabled);

                //    buf = "RUNPARAMS_NUMTOLGNORE" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.NumToIgnore);

                //    buf = "RUNPARAMS_ANGLESTART" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.ExpectedCircularArc.AngleStart);

                //    buf = "RUNPARAMS_ANGLESPAN" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.ExpectedCircularArc.AngleSpan);

                //    buf = "CALIPER_THRESHOLD" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.CaliperRunParams.ContrastThreshold);

                //    buf = "CALIPER_EDGE_MODE" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, Convert.ToInt16(CircleTools[i].RunParams.CaliperRunParams.EdgeMode));

                //    buf = "CALIPER_POLARLITY" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, Convert.ToInt16(CircleTools[i].RunParams.CaliperRunParams.Edge0Polarity));

                //    //                         buf = "CALIPER_POSITION_0" + i.ToString();
                //    //                         ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.CaliperRunParams.Edge0Position);
                //    // 
                //    //                         buf = "CALIPER_POSITION_1" + i.ToString();
                //    //                         ModelFile.SetData(m_CircleName, buf, CircleTools[i].RunParams.CaliperRunParams.Edge1Position);

                //    buf = "CIRCLE_USE" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CirclePara[i].m_UseCheck);

                //    buf = "CIRCLE_CALIPER_METHOD" + i.ToString();
                //    ModelFile.SetData(m_CircleName, buf, CirclePara[i].m_CircleCaliperMethod);

                //    for (int k = 0; k < Main.DEFINE.M_TOOLMAXCOUNT; k++)
                //    {
                //        buf = "TARGET_TO_CENTER_X" + i.ToString() + "_" + k.ToString();
                //        ModelFile.SetData(m_CircleName, buf, CirclePara[i].m_TargetToCenter[k].X);
                //        buf = "TARGET_TO_CENTER_Y" + i.ToString() + "_" + k.ToString();
                //        ModelFile.SetData(m_CircleName, buf, CirclePara[i].m_TargetToCenter[k].Y);
                //    }
                //}
                #endregion

            }
            public Main.PatternTag.SDParameter InspRest()
            {
                Main.PatternTag.SDParameter ResetData = new SDParameter();
                ResetData.m_enumROIType = new SDParameter.enumROIType();
                ResetData.m_FindCircleTool = new CogFindCircleTool();
                ResetData.m_FindLineTool = new CogFindLineTool();
                ResetData.CenterX = 0;
                ResetData.CenterY = 0;
                ResetData.LenthX = 0;
                ResetData.LenthY = 0;
                return ResetData;
            }
            public void Load()
            {
                string buf;
                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    buf = "LIGHTCTRL_" + i.ToString();
                    m_LightCtrl[i] = SystemFile.GetIData(m_PatternName, buf);

                    buf = "LIGHTCH_" + i.ToString();
                    m_LightCH[i] = SystemFile.GetIData(m_PatternName, buf);

                    buf = "LIGHTNAME_" + i.ToString();
                    m_Light_Name[i] = SystemFile.GetSData(m_PatternName, buf);
                }
                //---------------------------ModelFile-----------------------------------
                for (int i = 0; i < Main.DEFINE.Light_PatMaxCount; i++)
                {
                    //                 for (int j = 0; j < Main.DEFINE.Light_ToolMaxCount; j++)
                    for (int j = Main.DEFINE.Light_ToolMaxCount - 1; j >= 0; j--)
                    {
                        buf = "LIGHT" + i.ToString() + "_" + j.ToString();
                        m_LightValue[i, j] = ModelFile.GetIData(m_PatternName, buf);
                        if (Main.Status.MC_MODE != Main.DEFINE.MC_SETUPFORM)
                            SetLight(i, m_LightValue[i, j]);
                    }
                }

                for (int i = 0; i < 2; i++)
                {
                    buf = "CAL_X" + i.ToString();
                    m_CalX[i] = SystemFile.GetFData(m_PatternName, buf);
                    buf = "CAL_Y" + i.ToString();
                    m_CalY[i] = SystemFile.GetFData(m_PatternName, buf);
                }

                for (int i = 0; i < 9; i++)
                {
                    buf = "CALMATRIX" + i.ToString();
                    CALMATRIX[i] = SystemFile.GetFData(m_PatternName, buf);
                }
                if (Main.DEFINE.OPEN_F)
                {
                    #region
                    CALMATRIX[0] = 0.00230390066009215;
                    CALMATRIX[1] = -3.2820097040512E-07;
                    CALMATRIX[2] = -2.98789960112807;
                    CALMATRIX[3] = -2.4528505176532E-07;
                    CALMATRIX[4] = -0.00230483660374504;
                    CALMATRIX[5] = 2.24360308486755;
                    CALMATRIX[6] = -5.09295780651445E-07;
                    CALMATRIX[7] = 1.28121274314743E-06;
                    CALMATRIX[8] = 1;
                    m_CalX[0] = 1;
                    m_CalX[1] = 1;
                    m_CalY[0] = 1;
                    m_CalY[1] = 1;
                    #endregion
                }


                buf = "CCD_T_X";
                CAMCCDTHETA[0, 0] = SystemFile.GetFData(m_PatternName, buf);
                buf = "CCD_T_Y";
                CAMCCDTHETA[0, 1] = SystemFile.GetFData(m_PatternName, buf);

                buf = "MANU_MATCH_USE";
                m_Manu_Match_Use = ModelFile.GetBData(m_PatternName, buf);

                buf = "PMALIGN_USE";
                m_PMAlign_Use = ModelFile.GetBData(m_PatternName, buf);

                buf = "LINEMAX_USE";
                m_UseLineMax = ModelFile.GetBData(m_PatternName, buf);

                buf = "CUSTOM_CROSS_USE";
                Main.vision.USE_CUSTOM_CROSS[m_CamNo] = ModelFile.GetBData(m_PatternName, buf);

                buf = "CUSTOM_CROSS_X";
                Main.vision.CUSTOM_CROSS_X[m_CamNo] = ModelFile.GetIData(m_PatternName, buf);

                buf = "CUSTOM_CROSS_Y";
                Main.vision.CUSTOM_CROSS_Y[m_CamNo] = ModelFile.GetIData(m_PatternName, buf);

                V2R(Main.vision.CUSTOM_CROSS_X[m_CamNo], Main.vision.CUSTOM_CROSS_Y[m_CamNo], ref m_dCustomCrossX, ref m_dCustomCrossY);

                if (Main.Status.MC_MODE != Main.DEFINE.MC_SETUPFORM)
                {
                    //--------------------------TEACH창에서 적용 되는 것들 -------------------
                    //-----------------------------------------------------------------------
                    String ModelDir = ModelPath;
                    String PatFileName;

                    double Temp_Angle;
                    if (m_AlignName == "LOAD_ALIGN")
                        Temp_Angle = 50;    // 스카라 로딩에서는 각도가 많이 필요.
                    else
                        Temp_Angle = Main.DEFINE.DEFAULT_ACCEPT_ANGLE;

                    for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                    {
                        PatFileName = ModelDir + ProjectName + "\\" + m_PatternName + "_" + i.ToString() + ".vpp";
                        try
                        {
                            Pattern[i].Dispose();
                            Pattern[i] = CogSerializer.LoadObjectFromFile(PatFileName) as CogSearchMaxTool;
                            Pattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * Temp_Angle);
                            Pattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * Temp_Angle);
                            Pattern[i].RunParams.TimeoutEnabled = true;
                            Pattern[i].RunParams.Timeout = Main.DEFINE.PATTERN_TIMEOUT;
                        }
                        catch (System.Exception ex)
                        {
                            Pattern[i] = new CogSearchMaxTool();
                            Pattern[i].Pattern.TrainRegion = new CogRectangle();
                            Pattern[i].SearchRegion = new CogRectangle();
                            (Pattern[i].SearchRegion as CogRectangle).SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[m_CamNo], Main.vision.IMAGE_CENTER_Y[m_CamNo], Main.vision.IMAGE_SIZE_X[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA, Main.vision.IMAGE_SIZE_Y[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA);


                            Pattern[i].RunParams.RunAlgorithm = CogSearchMaxRunAlgorithmConstants.HighAccuracy;

                            if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "TRAY_ALIGN")
                            {
                                Pattern[i].RunParams.RunAlgorithm = CogSearchMaxRunAlgorithmConstants.Standard;
                            }

                            Pattern[i].RunParams.AcceptThreshold = Main.DEFINE.DEFAULT_ACCEPT_SCORE;
                            Pattern[i].RunParams.ConfusionThreshold = Main.DEFINE.DEFAULT_CONFUSION_SCORE;

                            //                             if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE" && m_PatNo == Main.DEFINE.TAR_L)
                            //                             {
                            //                                 Pattern[i].RunParams.ZoneUsePattern = true;
                            //                             }
                            //                             else
                            Pattern[i].RunParams.ZoneUsePattern = false;

                            if (Main.AlignUnit[m_PatAlign_No].m_AlignName == "CHIP_PRE")
                            {
                                Pattern[i].RunParams.ContrastEnabled = true;
                                Pattern[i].RunParams.ContrastRangeLow = 0.5;
                                Pattern[i].RunParams.ContrastRangeHigh = 1.3;
                            }

                            Pattern[i].RunParams.ZoneAngle.Configuration = CogSearchMaxZoneConstants.LowHigh;
                            Pattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * Temp_Angle);
                            Pattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * Temp_Angle);
                            Pattern[i].RunParams.ZoneScale.Configuration = CogSearchMaxZoneConstants.Nominal;
                            Pattern[i].RunParams.TimeoutEnabled = true;
                            Pattern[i].RunParams.Timeout = Main.DEFINE.PATTERN_TIMEOUT;
                        }
                    }
                    for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                    {
                        PatFileName = ModelDir + ProjectName + "\\" + m_PatternName + "_" + "G" + i.ToString() + ".vpp";
                        try
                        {
                            GPattern[i].Dispose();
                            GPattern[i] = CogSerializer.LoadObjectFromFile(PatFileName) as CogPMAlignTool;
                            GPattern[i].RunParams.ZoneScale.Configuration = CogPMAlignZoneConstants.Nominal;

                            GPattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * Temp_Angle);
                            GPattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * Temp_Angle);
                            GPattern[i].RunParams.TimeoutEnabled = true;
                            GPattern[i].RunParams.Timeout = Main.DEFINE.PATTERN_TIMEOUT;
                        }
                        catch (System.Exception ex)
                        {
                            GPattern[i] = new CogPMAlignTool();
                            GPattern[i].Pattern.TrainAlgorithm = CogPMAlignTrainAlgorithmConstants.PatMaxHighSensitivity;
                            GPattern[i].SearchRegion = new CogRectangle();
                            (GPattern[i].SearchRegion as CogRectangle).SetCenterWidthHeight(Main.vision.IMAGE_CENTER_X[m_CamNo], Main.vision.IMAGE_CENTER_Y[m_CamNo], Main.vision.IMAGE_SIZE_X[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA, Main.vision.IMAGE_SIZE_Y[m_CamNo] - Main.DEFINE.DEFAULT_SEARCH_AREA);

                            GPattern[i].RunParams.ScoreUsingClutter = false;
                            GPattern[i].RunParams.AcceptThreshold = Main.DEFINE.DEFAULT_GACCEPT_SCORE;
                            GPattern[i].RunParams.TimeoutEnabled = true;
                            GPattern[i].RunParams.Timeout = Main.DEFINE.PATTERN_TIMEOUT;

                            GPattern[i].RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
                            GPattern[i].RunParams.ZoneAngle.Low = -(Main.DEFINE.radian * Temp_Angle);
                            GPattern[i].RunParams.ZoneAngle.High = (Main.DEFINE.radian * Temp_Angle);
                            GPattern[i].RunParams.ZoneScale.Configuration = CogPMAlignZoneConstants.Nominal;
                            GPattern[i].RunParams.SaveMatchInfo = true;
                            GPattern[i].LastRunRecordDiagEnable = CogPMAlignLastRunRecordDiagConstants.InputImageByReference | CogPMAlignLastRunRecordDiagConstants.ResultsMatchFeatures;
                        }
                    }

                    for (int i = 0; i < Main.DEFINE.SUBPATTERNMAX; i++)
                    {
                        buf = "PATUSE" + i.ToString();
                        Pattern_USE[i] = ModelFile.GetBData(m_PatternName, buf);
                        if (i == Main.DEFINE.MAIN) Pattern_USE[i] = true;
                    }

                    buf = "ACCEPT_SCORE";
                    double ntemp;
                    ntemp = ModelFile.GetFData(m_PatternName, buf);
                    if (ntemp != 0)
                        m_ACCeptScore = ntemp;

                    buf = "ACCEPT_GSCORE";
                    ntemp = ModelFile.GetFData(m_PatternName, buf);
                    if (ntemp != 0)
                        m_GACCeptScore = ntemp;

                    buf = "CALIPER_MARKUSE";
                    Caliper_MarkUse = ModelFile.GetBData(m_PatternName, buf);

                    buf = "BLOB_MARKUSE";
                    Blob_MarkUse = ModelFile.GetBData(m_PatternName, buf);

                    buf = "BLOB_CALIPERUSE";
                    Blob_CaliperUse = ModelFile.GetBData(m_PatternName, buf);

                    buf = "BLOB_INSPECT_CNT";
                    m_Blob_InspCnt = ModelFile.GetIData(m_PatternName, buf);

                    buf = "FINDLINE_MARKUSE";
                    FINDLine_MarkUse = ModelFile.GetBData(m_PatternName, buf);

                    buf = "CIRCLE_MARKUSE";
                    Circle_MarkUse = ModelFile.GetBData(m_PatternName, buf);
                    for (int i = 0; i < 2; i++)
                    {
                        buf = "Left Origin" + i.ToString();
                        LeftOrigin[i] = ModelFile.GetFData(m_PatternName, buf);
                        buf = "Right Origin" + i.ToString();
                        RightOrigin[i] = ModelFile.GetFData(m_PatternName, buf);
                    }
                    #region SD BIO

                    string InspectionName;

                    //if (m_PatternName == "INSPECTION_1" || m_PatternName == "INSPECTION_2")
                    {
                        int iCount = 0;

                        m_InspParameter = new List<SDParameter>();
                        string TempCountName = m_PatternName + "_" + "0";
                        buf = "COUNT";
                        iCount = ModelFile.GetIData(TempCountName, buf);
                        //if (iCount == 0) iCount = 1; 
                        Task Load = Task.Factory.StartNew(() =>
                        {
                            for (int i = 0; i < iCount; i++)
                            {
                                try
                                {
                                    InspectionName = m_PatternName + "_" + i.ToString();
                                    m_InspParameter.Add(Reset());
                                    var LoadData = m_InspParameter[i];
                                    if (i == 0)
                                    {
                                        buf = "IMAGE CENTER X";
                                        LoadData.CenterX = ModelFile.GetFData(InspectionName, buf);
                                        buf = "IMAGE CENTER Y";
                                        LoadData.CenterY = ModelFile.GetFData(InspectionName, buf);
                                        buf = "IMAGE LENTH X";
                                        LoadData.LenthX = ModelFile.GetFData(InspectionName, buf);
                                        buf = "IMAGE LENTH Y";
                                        LoadData.LenthY = ModelFile.GetFData(InspectionName, buf);
                                        buf = "Histogram ROI Count";
                                        LoadData.iHistogramROICnt = ModelFile.GetIData(InspectionName, buf);
                                        for (int j = 0; j < LoadData.iHistogramROICnt; j++)
                                        {
                                            try
                                            {
                                                string FileName2 = ModelDir + ProjectName + "\\" + "Historam" + InspectionName + "_" + j.ToString() + ".Vpp";
                                                LoadData.m_CogHistogramTool[j] = CogSerializer.LoadObjectFromFile(FileName2) as CogHistogramTool;
                                                buf = " Histogram Spec" + j.ToString();
                                                LoadData.iHistogramSpec[j] = ModelFile.GetIData(InspectionName, buf);
                                            }
                                            catch { }
                                        }
                                    }
                                    buf = "INSPECTION TYPE" + i.ToString();
                                    LoadData.m_enumROIType = (SDParameter.enumROIType)ModelFile.GetIData(InspectionName, buf);

                                    buf = "Insp_Spec_Dist" + i.ToString();
                                    LoadData.dSpecDistance = ModelFile.GetFData(InspectionName, buf);

                                    buf = "Insp_Spec_Dist_Max" + i.ToString();
                                    LoadData.dSpecDistanceMax = ModelFile.GetFData(InspectionName, buf);

                                    buf = "Insp_Dist_Ingnore" + i.ToString();
                                    LoadData.IDistgnore = ModelFile.GetIData(InspectionName, buf);

                                 
                                    string FileName = ModelDir + ProjectName + "\\" + "FindLine_" + InspectionName + ".VPP";
                                    if (File.Exists(FileName))
                                        LoadData.m_FindLineTool = CogSerializer.LoadObjectFromFile(FileName) as CogFindLineTool;

                                    FileName = ModelDir + ProjectName + "\\" + "Circle_" + InspectionName + ".VPP";
                                    if (File.Exists(FileName))
                                        LoadData.m_FindCircleTool = CogSerializer.LoadObjectFromFile(FileName) as CogFindCircleTool;

                                    buf = "Insp_Edge_Threshold_Use" + i.ToString();
                                    LoadData.bThresholdUse = ModelFile.GetBData(InspectionName, buf);

                                    buf = "Insp_Edge_Threshold" + i.ToString();
                                    int value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iThreshold = value == 0 ? LoadData.iThreshold : value;

                                    buf = "Insp_Top_Cut_Pixel" + i.ToString();
                                    value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iTopCutPixel = value == 0 ? LoadData.iTopCutPixel : value;

                                    buf = "Insp_Bottom_Cut_Pixel" + i.ToString();
                                    value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iBottomCutPixel = value == 0 ? LoadData.iBottomCutPixel : value;

                                    buf = "Insp_Masking_Value" + i.ToString();
                                    value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iMaskingValue = value == 0 ? LoadData.iMaskingValue : value;

                                    buf = "Insp_Ignore_Size" + i.ToString();
                                    value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iIgnoreSize = value == 0 ? LoadData.iIgnoreSize : value;

                                    buf = "Insp_Edge_Caliper_TH" + i.ToString();
                                    value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iEdgeCaliperThreshold = value == 0 ? LoadData.iEdgeCaliperThreshold : value;

                                    buf = "Insp_Edge_Caliper_Filter_Size" + i.ToString();
                                    value = ModelFile.GetIData(InspectionName, buf);
                                    LoadData.iEdgeCaliperFilterSize = value == 0 ? LoadData.iEdgeCaliperFilterSize : value;

                                    m_InspParameter[i] = LoadData;
                                }

                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        });
                        Task.WaitAll(Load);
                    }



                    //if (m_PatternName == "INSPECTION_1ALIGN INSPECTION" || m_PatternName == "INSPECTION_2ALIGN INSPECTION") //cyh0811
                    //{ 
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            InspectionName = m_PatternName + "_" + i.ToString();
                            m_TrackingLine[i] = new CogFindLineTool();
                            string FileName = ModelDir + ProjectName + "\\" + "TrackingLine" + InspectionName + ".VPP";
                            m_TrackingLine[i] = CogSerializer.LoadObjectFromFile(FileName) as CogFindLineTool;
                        }
                        catch { }

                    }
                    //}
                    //Bonding Align Load - shkang_s
                    for (int i = 0; i < 4; i++)
                    {
                        try
                        {
                            InspectionName = m_PatternName + "_" + i.ToString();
                            m_BondingAlignLine[i] = new CogCaliperTool();
                            string FileName = ModelDir + ProjectName + "\\" + "BondingAlignCaliperLine" + InspectionName + ".VPP";
                            m_BondingAlignLine[i] = CogSerializer.LoadObjectFromFile(FileName) as CogCaliperTool;
                        }
                        catch { }
                    }

                    /////////////////////////////////////
                    //ROI Finealign
                    try
                    {
                        for (int i = 0; i < Main.DEFINE.Pattern_Max; i++)
                        {
                            for (int j = 0; j < Main.DEFINE.SUBPATTERNMAX; j++)
                            {
                                InspectionName = m_PatternName + "_" + i.ToString() + j.ToString();
                                string FileName = ModelDir + ProjectName + "\\" + "ROIFineAlign" + InspectionName + ".VPP";
                                if (File.Exists(FileName))
                                {
                                    m_FinealignMark[i, j] = CogSerializer.LoadObjectFromFile(FileName) as CogSearchMaxTool;
                                }
                            }
                        }
                    }
                    catch
                    {

                    }


                    buf = "ROIFinealign_Flag";
                    m_bFInealignFlag = ModelFile.GetBData(m_PatternName, buf);
                    buf = "ROIFinealign_T_Spec";
                    m_FinealignThetaSpec = ModelFile.GetFData(m_PatternName, buf);
                    buf = "ROIFinealign_MarkScore";
                    m_FinealignMarkScore = ModelFile.GetFData(m_PatternName, buf);
                    /////////////////////////////////////

                    buf = "InspDirectionChange_Flag";
                    m_bInspDirectionChangeFlag = ModelFile.GetBData(m_PatternName, buf);

                    buf = "BondingAlign_OriginDistX";
                    m_dOriginDistanceX = ModelFile.GetFData(m_PatternName, buf);
                    buf = "BondingAlign_OriginDistY";
                    m_dOriginDistanceY = ModelFile.GetFData(m_PatternName, buf);
                    buf = "BondingAlign_DistSpecX";
                    m_dDistanceSpecX = ModelFile.GetFData(m_PatternName, buf);
                    buf = "BondingAlign_DistSpecY";
                    m_dDistanceSpecY = ModelFile.GetFData(m_PatternName, buf);
                    buf = "Object_Distance_X";
                    m_dObjectDistanceX = ModelFile.GetFData(m_PatternName, buf);
                    buf = "Object_Distance_X_Spec";
                    m_dObjectDistanceSpecX = ModelFile.GetFData(m_PatternName, buf);
                    //shkang_e
                    #endregion
                }

            }

            public void SaveCal()
            {
                string buf;
                for (int i = 0; i < 2; i++)
                {
                    buf = "CAL_X" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_CalX[i]);
                    buf = "CAL_Y" + i.ToString();
                    SystemFile.SetData(m_PatternName, buf, m_CalY[i]);
                }

                for (int i = 0; i < 9; i++)
                {
                    buf = "CALMATRIX" + i.ToString();
                    //                    if(!Main.DEFINE.OPEN_F)
                    SystemFile.SetData(m_PatternName, buf, CALMATRIX[i]);
                }
                buf = "CCD_T_X";
                SystemFile.SetData(m_PatternName, buf, CAMCCDTHETA[0, 0]);
                buf = "CCD_T_Y";
                SystemFile.SetData(m_PatternName, buf, CAMCCDTHETA[0, 1]);
            }
        }

    }
}
