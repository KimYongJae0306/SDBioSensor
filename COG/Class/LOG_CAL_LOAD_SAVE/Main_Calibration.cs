using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


using System.Reflection;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

using JAS.Calibration;

namespace COG
{
    public partial class Main
    {
        public partial struct DEFINE
        {
            public const int XPOS = 0;
            public const int YPOS = 1;
            public const int CAL_POINT_THETA = 3;
            public static int CAL_POINT = 9;
            // 0   ,    1   ,   2
            // 3   ,    4   ,   5
            // 6   ,    7   ,   8
        }

        public static Calibration CAL = new Calibration();
        
        public partial class PatternTag
        {
            public bool GrabComplete = false;
            public void V2R(ref double nRobot_X, ref double nRobot_Y)
            {
                if (CALMATRIX_NOTUSE)
                    V2RByCAL(ref nRobot_X, ref nRobot_Y);
                else
                {
                    //                     if (machine.CAL_9POINT_USE)
                    //                         Main.CAL.V2R(CALMATRIX_9P, Pixel[DEFINE.X], Pixel[DEFINE.Y], ref nRobot_X, ref nRobot_Y);
                    //                     else
                    //Main.CAL.V2R(CALMATRIX, Pixel[DEFINE.X], Pixel[DEFINE.Y], ref nRobot_X, ref nRobot_Y);
                    Main.ConvertVisionToReal(CALMATRIX, Pixel[DEFINE.X], Pixel[DEFINE.Y], ref nRobot_X, ref nRobot_Y);
                }

            }
            public void V2R(double nPixel_X, double nPixel_Y, ref double nRobot_X, ref double nRobot_Y)
            {
                if (CALMATRIX_NOTUSE)
                    V2RByCAL(nPixel_X, nPixel_Y, ref nRobot_X, ref nRobot_Y);
                else
                {

                    //                     if (machine.CAL_9POINT_USE)
                    //                         Main.CAL.V2R(CALMATRIX_9P, nPixel_X, nPixel_Y, ref nRobot_X, ref nRobot_Y);
                    //                     else
                    //Main.CAL.V2R(CALMATRIX, nPixel_X, nPixel_Y, ref nRobot_X, ref nRobot_Y);
                    Main.ConvertVisionToReal(CALMATRIX, nPixel_X, nPixel_Y, ref nRobot_X, ref nRobot_Y);

                }


            }
            public void V2RByCAL(ref double nRobot_X, ref double nRobot_Y)
            {
                double nTempX, nTempY, tempX, tempY;
                double theta;

                nTempX = Pixel[DEFINE.X] - vision.IMAGE_CENTER_X[m_CamNo];
                nTempY = vision.IMAGE_CENTER_Y[m_CamNo] - Pixel[DEFINE.Y];

                theta = -CAMCCDTHETA[0, DEFINE.YPOS] * DEFINE.radian;

                tempX = nTempX * Math.Cos(theta) - nTempY * Math.Sin(theta);
                tempY = nTempX * Math.Sin(theta) + nTempY * Math.Cos(theta);

                nRobot_X = tempX * m_CalX[0];
                nRobot_Y = tempY * m_CalY[0];
            }
            public void V2RByCAL(double nPixel_X, double nPixel_Y, ref double nRobot_X, ref double nRobot_Y)
            {
                double nTempX, nTempY, tempX, tempY;
                double theta;

                nTempX = nPixel_X - vision.IMAGE_CENTER_X[m_CamNo];
                nTempY = vision.IMAGE_CENTER_Y[m_CamNo] - nPixel_Y;

                theta = -CAMCCDTHETA[0, DEFINE.YPOS] * DEFINE.radian;

                tempX = nTempX * Math.Cos(theta) - nTempY * Math.Sin(theta);
                tempY = nTempX * Math.Sin(theta) + nTempY * Math.Cos(theta);

                nRobot_X = tempX * m_CalX[0];
                nRobot_Y = tempY * m_CalY[0];
            }
            public void V2RPIXEL(ref double nRobot_X, ref double nRobot_Y)
            {
                V2RByCALPixel(ref nRobot_X, ref nRobot_Y);
            }
            public void V2RByCALPixel(ref double nRobot_X, ref double nRobot_Y)
            {
                double nTempX, nTempY, tempX, tempY;
                double theta;

                nTempX = Pixel[DEFINE.X] - vision.IMAGE_CENTER_X[m_CamNo];
                nTempY = vision.IMAGE_CENTER_Y[m_CamNo] - Pixel[DEFINE.Y];

                theta = -CAMCCDTHETA[0, DEFINE.YPOS] * DEFINE.radian;

                tempX = nTempX * Math.Cos(theta) - nTempY * Math.Sin(theta);
                tempY = nTempX * Math.Sin(theta) + nTempY * Math.Cos(theta);

                nRobot_X = tempX;
                nRobot_Y = tempY;
            }

            public void V2RScalar(double nPixel, ref double nRobot)
            {
                double XScale = Math.Abs(nPixel * CALMATRIX[0]);
                double YScale = Math.Abs(nPixel * CALMATRIX[4]);
                nRobot = (XScale + YScale) / 2.0;
            }
        }
        public static int Calibration_Matrix(double[,] nPixelPos, double[,] nRobotPos, int nCalCount, ref double[] nCalMatrix)
        {
            return Main.CAL.Calibration_Matrix(nPixelPos, nRobotPos, nCalCount, ref nCalMatrix);
        }
        public static void GetCenter(double x1, double y1, double x2, double y2, double theta, ref double nCenter_X, ref double nCenter_Y)
        {
            Main.CAL.GetCenter(x1, y1, x2, y2, theta, ref nCenter_X, ref nCenter_Y);
        }


        public static void ConvertVisionToReal(double[] nCalMatrix, double nPixel_X, double nPixel_Y, ref double nRobot_X, ref double nRobot_Y)
        {
            double nTemp_T;

            nRobot_X = (nPixel_X * nCalMatrix[0]) + (nPixel_Y * nCalMatrix[1]) + nCalMatrix[2];
            nRobot_Y = (nPixel_X * nCalMatrix[3]) + (nPixel_Y * nCalMatrix[4]) + nCalMatrix[5];
            nTemp_T = (nRobot_X * nCalMatrix[6]) + (nRobot_Y * nCalMatrix[7]) + 1.0;

            nRobot_X /= nTemp_T;
            nRobot_Y /= nTemp_T;
        }
    }
}
