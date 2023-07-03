using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.ComponentModel;

using Cognex.VisionPro.Implementation;
using Cognex.VisionPro;
using Cognex.VisionPro.Caliper;
namespace COG
{
    [Serializable]
    public class cls_InspParameter
    {
        public int ID { get; set; }
        public double P1X { get; set; }
        public double P1Y { get; set; }
        public double P2X { get; set; }
        public double P2Y { get; set; }
        public double P3X { get; set; }
        public double P3Y { get; set; }
        public double[] ImageCenterX { get; set; }
        public double[] ImageCenterY { get; set; }
        public double[] ImageLenthX { get; set; }
        public double[] ImageLenthY { get; set; }
        public double ToolType { get; set; }
    
       
      
    }
}
