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
    public class cls_ScnceData
    {
        public cls_InspParameter m_clsInspParameter;
        public cls_ScnceData()
        {
            m_clsInspParameter = new cls_InspParameter();
        }
        public List<double> ImageCenterX { get; set; }
        public List<double> ImageCenterY { get; set; }
        public List<double> ImageLenthX { get; set; }
        public List<double> ImageLenthY { get; set; }
        public List<double> X1 { get; set; }
        public List<double> Y1 { get; set; }
        public List<double> X2 { get; set; }
        public List<double> Y2 { get; set; }
        public List<double> X3 { get; set; }
        public List<double> Y3 { get; set; }
        public void AddImageLenth()
        {
            for (int i = 0; i < 6; i++)
            {
                ImageCenterX.Add(m_clsInspParameter.ImageCenterX[i]);
                ImageCenterY.Add(m_clsInspParameter.ImageCenterY[i]);
                ImageLenthX.Add(m_clsInspParameter.ImageLenthX[i]);
                ImageLenthY.Add(m_clsInspParameter.ImageLenthY[i]);
            }
           
        }
    }
}
