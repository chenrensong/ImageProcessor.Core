using ImageProcessor.Core.Imaging;
using ImageProcessor.Processors;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ImageProcessor.Core.Processors
{
    /// <summary>
    /// 缩放
    /// </summary>
    public class Scale : IGraphicsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flip"/> class.
        /// </summary>
        public Scale() => this.Settings = new Dictionary<string, string>();

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
            var source = (Bitmap)factory.Image;
            ScaleLayer scaleLayer = this.DynamicParameter;
            int w = (int)(source.Width * scaleLayer.Scale);
            int h = (int)(source.Height * scaleLayer.Scale);
            if (scaleLayer.Width != 0 && scaleLayer.Height != 0)
            {
                w = scaleLayer.Width;
                h = scaleLayer.Height;
            }
            Bitmap destinationBitmap = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(destinationBitmap))
            {
                //设置高质量插值法   
                g.InterpolationMode = InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度   
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                //消除锯齿 
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(source, new Rectangle(0, 0, w, h), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
                g.Flush();
            }
            source.Dispose();
            return destinationBitmap;
        }
    }
}
