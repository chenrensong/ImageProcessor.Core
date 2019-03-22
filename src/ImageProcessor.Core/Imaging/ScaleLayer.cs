using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessor.Core.Imaging
{
    public class ScaleLayer
    {
        public ScaleLayer(double scale)
        {
            Scale = scale;
        }

        public ScaleLayer(int height, int width)
        {
            Height = height;
            Width = width;
        }

        public double Scale { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }
    }
}
