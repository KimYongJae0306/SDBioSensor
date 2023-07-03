using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.PMAlign;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.Blob;

using System.Threading;

namespace COG
{
    public partial class Main
    {

		private static void ThreadDIR_Delete()
        {
            while (threadFlag)
            {
                try
                {
                    if (Main.machine.LogDirDeleteFlag == false) //여기가 원래는 true로 들어가야함
                    {
                        Main.machine.LogDirDeleteFlag = false;
                        Main.DirectorySizeCheck(Main.LogdataPath);
                        Main.SaveOldLogCheckFile();
                    }
                }
                catch
                {
                    Main.machine.LogDirDeleteFlag = false;
                }
                Thread.Sleep(50);
            }
        }


        public static void DirectorySizeCheck(string nDirectorypath)
        {
            double nMAX_SizeMB;
            double nDir_SizeMB;
            if (machine.m_nOldLogCheckSpace < 100)
                nMAX_SizeMB = Main.DEFINE.DIRECTORY_LIMIT_GB * 1000;
            else
                nMAX_SizeMB = machine.m_nOldLogCheckSpace * 1000;

            nDir_SizeMB = GetDirectorySize(nDirectorypath);

            int nPostDay = 30;  // 최대 180일 이전의 로그부터 삭제 체크 -> 이거를 설정 값으로 받아야할듯

            while (nDir_SizeMB < nMAX_SizeMB) // 부등호 방향이 반대였는데.. dir - 해당 logdata 용량인듯
            {
                if (Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC1" || Main.DEFINE.PROGRAM_TYPE == "QD_LPA_PC2" || Main.DEFINE.PROGRAM_TYPE == "ATT_AREA_PC1")
                    //DeleteOldFolderFrom(nDirectorypath, nPostDay);
                    DeleteOldFolderFrom(nDirectorypath, Main.machine.m_nOldLogCheckPeriod);
                else
                    DirectoryDelete(nDirectorypath);

                nDir_SizeMB = GetDirectorySize(nDirectorypath);
                if (nDir_SizeMB < nMAX_SizeMB) break;

                if (nPostDay <= Main.machine.m_nOldLogCheckPeriod)
                    
                    break;
                else
                        nPostDay--;
            }

        }

        public static void DeleteOldFolderFrom(String _Path, int nDay)
        {
            try
            {
                String[] files = Directory.GetDirectories(_Path);
                
                TimeSpan DeleteDate = new TimeSpan(nDay * (-1), 0, 0, 0); // 오늘 기준 삭제 날짜 
                foreach (string file in files)
                {
                    DirectoryInfo info = new DirectoryInfo(file);
                    if (info.LastWriteTime < DateTime.Now.Add(DeleteDate))
                        DirectoryDelete(file);
                }
            }
            catch
            {

            }
        }

        public static void FolderDelete(String _Path)
        {
            try
            {
                String[] files = Directory.GetDirectories(_Path);

                TimeSpan DeleteDate = new TimeSpan(-30, 0, 0, 0); //-30은 오늘 기준 삭제 날짜 
                foreach (string file in files)
                {
                    DirectoryInfo info = new DirectoryInfo(file);
                    if (info.LastWriteTime < DateTime.Now.Add(DeleteDate))
                        DirectoryDelete(file);
                }
            }
            catch
            {

            }
        }

        public static void DirectoryDelete(string nDirectorypath)
        {
            string strLog = "";
            string[] dirs;
            dirs = Directory.GetDirectories(nDirectorypath);
            while (dirs.Length > 0)
            {
                DirectoryInfo Delinfo = new DirectoryInfo(dirs[0]);
                try
                {
                    Delinfo.Delete(true);
                }
                catch (System.Exception ex)
                {

                }
                dirs = Directory.GetDirectories(nDirectorypath);
            }
            DirectoryInfo SubRootinfo = new DirectoryInfo(nDirectorypath);
            try
            {
                SubRootinfo.Delete(true);
                strLog = "LOG PATH \" " + nDirectorypath + " \" was deleted!";
                Main.AlignUnit[0].LogdataDisplay(strLog, true);
            }
            catch (System.Exception ex)
            {

            }
        }
        public static double GetDirectorySize(string nDirectorypath)
        {
            string[] dirs;
            double nSize = 0;
            try
            {
                dirs = Directory.GetDirectories(Main.LogdataPath);
               
                if (dirs.Length > 0)
                {
                    foreach (string path in dirs)
                    {
                        string[] ndirs;
                        ndirs = Directory.GetDirectories(path);
                        string[] Log;
                        Log = Directory.GetFiles(path);
                        for (int j = 0; j < Log.Length; j++) //LOG File Size
                        {
                            FileInfo nFile = new FileInfo(Log[j]);
                            double nKBSize;
                            nKBSize = nFile.Length / 1024.0;
                            nKBSize = Math.Ceiling(nKBSize);// 올림
                            nSize += nKBSize;
                        }
                        foreach (string npath in ndirs)
                        {
                            string[] nndirs;
                            nndirs = Directory.GetDirectories(npath);
                            foreach (string nnpath in nndirs)
                            {
                                string[] nnndirs;
                                nnndirs = Directory.GetFiles(nnpath);
                                for (int j = 0; j < nnndirs.Length; j++) // IMAGE Folder Size
                                {
                                    FileInfo nFile = new FileInfo(nnndirs[j]);
                                    double nKBSize;
                                    nKBSize = nFile.Length / 1024.0;
                                    nKBSize = Math.Ceiling(nKBSize);// 올림
                                    nSize += nKBSize;
                                }
                            }
                        }
                    }
                    nSize /= 1024.0;
                }
            }
            catch (System.Exception ex)
            {

            }
            return nSize;
        }
        public partial class PatternTag
        {
            object syncLock = new object();
            public void Save_Image_Copy()
            {
                ImageQue.Clear();
              //  ImageQue.Enqueue(vision.CogCamBuf[m_CamNo]);
                ImageQue.Enqueue(TempImage);
            }
            public void Save_Image(string nMode, CogRecordDisplay Display) //nMode = OK:NG
            {
                string fileName;
                ICogImage nImage = null;
                System.Drawing.Bitmap dispImage = null;
                CogImage24PlanarColor nImageOverlay = null;
                CreateImageDirectory();
                m_strImageLogName = DateTime.Now.ToString("HH_mm_ss_fff") + "_" + AlignUnit[m_PatAlign_No].m_Cell_ID + "_" + m_PatternName;
                fileName = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + nMode + "\\" + AlignUnit[m_PatAlign_No].m_AlignName + "\\" + m_strImageLogName;

                lock (syncLock)
                {
                    try
                    {
                        nImage = ImageQue.Dequeue();
                        ImageQue.Clear();

                        if (Main.machine.BMP_ImageSave_Onf || nMode == "NG")
                            BMP_SAVE(fileName, nImage, nImageOverlay);
                        else
                            JPG_SAVE(fileName, nImage, dispImage, false);

                     
                        if (machine.Overlay_Image_Onf)
                        {
                            dispImage = (System.Drawing.Bitmap)Display.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image, null, 0);
                            nImageOverlay = new CogImage24PlanarColor(dispImage);

                            if (Main.machine.BMP_ImageSave_Onf)
                                BMP_SAVE(fileName + "_OV", nImage, nImageOverlay, machine.Overlay_Image_Onf);
                            else
                                JPG_SAVE(fileName + "_OV", nImage, dispImage, machine.Overlay_Image_Onf);

                            if (Main.machine.BMP_ImageSave_Onf)
                                BMP_SAVE(fileName + "_OV", nImage, nImageOverlay, machine.Overlay_Image_Onf);
                            else
                                JPG_SAVE(fileName + "_OV", nImage, dispImage, machine.Overlay_Image_Onf);
                        }
                        else
                        {
                            if (Main.machine.BMP_ImageSave_Onf)
                                BMP_SAVE(fileName + "_OV", nImage, nImageOverlay, machine.Overlay_Image_Onf);
                            else
                                JPG_SAVE(fileName + "_OV", nImage, dispImage, machine.Overlay_Image_Onf);
                        }
                        dispImage.Dispose();
                    }
                    catch
                    {

                    }
                }
            }

            public void Save_ORGImage(string nMode) //nMode = OK:NG
            {
                string fileName;
                ICogImage nImage = null;
                CreateImageDirectory();
             //   fileName = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + nMode + "\\" + AlignUnit[m_PatAlign_No].m_AlignName + "\\" + m_PatternName + "_" + AlignUnit[m_PatAlign_No].Cell_ID;
                fileName = LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + nMode + "\\" + AlignUnit[m_PatAlign_No].m_AlignName + "\\" + DateTime.Now.ToString("HH_mm_ss_fff") + "_"
                            + AlignUnit[m_PatAlign_No].m_Cell_ID + "_" + m_PatternName;
                lock (syncLock)
                {
                    try
                    {
                        nImage = vision.CogCamBuf[m_CamNo];

//                         if (Main.machine.BMP_ImageSave_Onf)
//                             BMP_SAVE(fileName, nImage);
//                         else
                        JPG_ORGSAVE(fileName, nImage);
                        //                         PNG_SAVE(fileName, nImage, nImageOverlay);
                    }
                    catch
                    {

                    }
                }
            }

            private void BMP_SAVE(string fileName, ICogImage nImage, CogImage24PlanarColor nImageOverlay, bool OverLay_Flag = false)
            {
                CogImageFileBMP imagecontrol = new CogImageFileBMP();
                //        fileName = fileName + "_" + DateTime.Now.ToString("HH_mm_ss_fff") + ".bmp";
                fileName = fileName + ".bmp";
                m_strImageLogName = m_strImageLogName + ".bmp";
                try
                {
                    imagecontrol.Open(fileName, CogImageFileModeConstants.Write);
                    if (OverLay_Flag)
                        imagecontrol.Append(nImageOverlay);
                    else
                        imagecontrol.Append(nImage);
                    imagecontrol.Close();
                }
                catch
                {

                }
            }
            private void JPG_SAVE(string fileName, ICogImage nImage, System.Drawing.Bitmap nImageOverlay, bool OverLay_Flag = false)
            {
                //fileName = fileName + "_" + DateTime.Now.ToString("HH_mm_ss_fff") + ".jpg";
                fileName = fileName + ".jpg";
                m_strImageLogName = m_strImageLogName + ".jpg";
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                try
                {
                    if (OverLay_Flag)
                    {
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        nImageOverlay.Save(fileName, jgpEncoder, myEncoderParameters);
                        nImageOverlay.Dispose();
                    }
                    else
                    {
                        Bitmap bmp = nImage.ToBitmap();
                        myEncoderParameters.Param[0] = myEncoderParameter;
                        bmp.Save(fileName, jgpEncoder, myEncoderParameters);
                        bmp.Dispose();
                    }
                    myEncoderParameter.Dispose();
                    myEncoderParameters.Dispose();
                }
                catch
                {

                }
            }
            private void PNG_SAVE(string fileName, ICogImage nImage, CogImage24PlanarColor nImageOverlay)
            {
                CogImageFilePNG imagecontrol = new CogImageFilePNG();
           //     fileName = fileName + "_" + DateTime.Now.ToString("HH_mm_ss_fff") + ".png";
                fileName = fileName + ".png";
                try
                {
                    imagecontrol.Open(fileName, CogImageFileModeConstants.Write);
                    if (machine.Overlay_Image_Onf)
                        imagecontrol.Append(nImageOverlay);
                    else
                        imagecontrol.Append(nImage);
                    imagecontrol.Close();
                }
                catch
                {

                }
            }
            private void JPG_ORGSAVE(string fileName, ICogImage nImage)
            {
              //  fileName = fileName + "_" + DateTime.Now.ToString("HH_mm_ss_fff") + ".jpg";
                fileName = fileName + ".jpg";
                ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 50L);
                try
                {
                    Bitmap bmp = nImage.ToBitmap();
                    myEncoderParameters.Param[0] = myEncoderParameter;
                    bmp.Save(fileName, jgpEncoder, myEncoderParameters);
                }
                catch(Exception ex)
                {

                }
            }
            private ImageCodecInfo GetEncoder(ImageFormat format)
            {

                ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

                foreach (ImageCodecInfo codec in codecs)
                {
                    if (codec.FormatID == format.Guid)
                    {
                        return codec;
                    }
                }
                return null;
            }
            private void CreateImageDirectory()
            {
                Directory.CreateDirectory(LogdataPath);
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\");
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "OK");
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "OK" + "\\" + AlignUnit[m_PatAlign_No].m_AlignName);
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "NG");
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "NG" + "\\" + AlignUnit[m_PatAlign_No].m_AlignName);
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "GRAB CONFIRM");
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "GRAB CONFIRM" + "\\" + AlignUnit[m_PatAlign_No].m_AlignName);
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "ORG");
                Directory.CreateDirectory(LogdataPath + DateTime.Now.ToString("yyyyMMdd") + "\\" + "ORG" + "\\" + AlignUnit[m_PatAlign_No].m_AlignName);
            }

        }
    }
}
