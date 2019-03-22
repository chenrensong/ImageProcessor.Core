using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageProcessor.Core.Imaging
{
    internal class ColorHelper
    {
        internal static bool isBlack(Color color)
        {
            if (color.R + color.G + color.B <= 300)
            {
                return true;
            }
            return false;
        }
    }
}
