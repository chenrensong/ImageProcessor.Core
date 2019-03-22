using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageProcessor.Imaging
{
    public class MosaicLayer
    {
        public MosaicLayer(int x, int y, int width, int height, Size effectSize)
        {
            if (x < 0 || y < 0 || width < 0 || height < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.EffectSize = effectSize;
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        public Size EffectSize { get; set; }
        /// <summary>
        /// Gets or sets the left coordinate of the crop layer.
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the top coordinate of the crop layer.
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the right coordinate of the crop layer.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the bottom coordinate of the crop layer.
        /// </summary>
        public int Height { get; set; }

    }
}
