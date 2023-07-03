using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COG
{
    public partial class Main
    {
        public class RCS_PanelData
        {
            public string P00_ITEM;
            public string P01_CELITEM;
            public string P02_MODULETYPE;
            public string P03_PROD_TYPE;
            public string P04_MODULEID;
            public string P05_DEVICEID;
            public string P06_STEPID;
            public string P07_H_CELLID;
            public string P08_E_CELLID;
            public string P09_P_CELLID;
            public string P10_OPERID;
            public string P11_C_COUNT;
            public string P12_C_PPID;
            public string P13_C_GRADE;
            public string P14_C_CODE;
            public string P15_R_C_GRADE;
            public string P16_CELL_IMAGE;
            public string P17_L_TIME;
            public string P18_U_TIME;
            public string P19_S_TIME;
            public string P20_E_TIME;
            public string[] P_COM_ITEM = new string[10];
        }

        public class RCS_MeasureData
        {
            public string M00_ITEM;
            public string M01_MESITEM;
            public string M02_MES_ID;
            public string M03_CELLID;
            public string M04_MES_NO;
            public string M05_COORD_X1;
            public string M06_COORD_Y1;
            public string M07_COORD_X2;
            public string M08_COORD_Y2;
            public string M09_GATENO_01;
            public string M10_DATANO_01;
            public string M11_GATENO_02;
            public string M12_DATANO_02;
            public string M13_IMAGENAME;
            public string[] M_ADD_ITEM = new string[20];
        }
    }
}
