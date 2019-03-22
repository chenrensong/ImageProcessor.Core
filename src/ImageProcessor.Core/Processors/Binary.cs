using ImageProcessor.Processors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace ImageProcessor.Core.Processors
{
    /// <summary>
    /// 二值化处理
    /// </summary>
    public class Binary : IGraphicsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flip"/> class.
        /// </summary>
        public Binary() => this.Settings = new Dictionary<string, string>();

        /// <summary>
        /// Gets or sets DynamicParameter.
        /// </summary>
        public dynamic DynamicParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any additional settings required by the processor.
        /// </summary>
        public Dictionary<string, string> Settings
        {
            get;
            set;
        }

        public Image ProcessImage(ImageFactory factory)
        {
            var bitmap = (Bitmap)factory.Image;
            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* start = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (start[0] != 255)
                        {
                            start[0] = start[1] = start[2] = 0;
                        }
                        start += 4;
                    }
                    start += bdata.Stride - width * 4;
                }
            }
            bitmap.UnlockBits(bdata);
            return bitmap;
        }
    }
}
