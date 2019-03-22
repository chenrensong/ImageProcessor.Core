using ImageProcessor.Core.Imaging;
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
    public class Gray : IGraphicsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flip"/> class.
        /// </summary>
        public Gray() => this.Settings = new Dictionary<string, string>();

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
            GrayMode grayMode = this.DynamicParameter;
            Func<int, int, int, int> getGrayValue;
            switch (grayMode)
            {
                case GrayMode.Max:
                    getGrayValue = GetGrayValueByMax;
                    break;
                case GrayMode.Avg:
                    getGrayValue = GetGrayValueByAvg;
                    break;
                default:
                    getGrayValue = GetGrayValueByWeightAvg;
                    break;
            }
            int height = bitmap.Height;
            int width = bitmap.Width;
            var bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int v = getGrayValue(ptr[0], ptr[1], ptr[2]);
                        ptr[0] = ptr[1] = ptr[2] = (byte)v;
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            bitmap.UnlockBits(bdata);
            return bitmap;
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int GetGrayValueByMax(int r, int g, int b)
        {
            int max = r;
            max = max > g ? max : g;
            max = max > b ? max : b;
            return max;
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int GetGrayValueByAvg(int r, int g, int b)
        {
            return (r + g + b) / 3;
        }

        /// <summary>
        /// 加权平均
        /// </summary>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static int GetGrayValueByWeightAvg(int b, int g, int r)
        {
            return (int)(r * 0.299 + g * 0.587 + b * 0.114);
        }
    }
}
