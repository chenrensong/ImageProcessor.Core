// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Flip.cs" company="James Jackson-South">
//   Copyright (c) James Jackson-South.
//   Licensed under the Apache License, Version 2.0.
// </copyright>
// <summary>
//   Flips an image horizontally or vertically.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ImageProcessor.Processors
{
    using ImageProcessor.Common.Exceptions;
    using ImageProcessor.Imaging;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Threading.Tasks;

    /// <summary>
    /// 马赛克
    /// </summary>
    public class Mosaic : IGraphicsProcessor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Flip"/> class.
        /// </summary>
        public Mosaic() => this.Settings = new Dictionary<string, string>();

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

            MosaicLayer mosaicLayer = this.DynamicParameter;
            int width = source.Width;
            int height = source.Height;

            using (var sourceBitmap = new FastBitmap(source))
            {
                var maxWidth = Math.Min(mosaicLayer.X + mosaicLayer.Width, width);
                var maxHeight = Math.Min(mosaicLayer.Y + mosaicLayer.Height, height);

                Parallel.For(mosaicLayer.X, maxWidth, widthOffset =>
                  {
                      if ((widthOffset - mosaicLayer.X) % mosaicLayer.EffectSize.Width != 0)
                      {
                          return;
                      }
                      for (int heightOffset = mosaicLayer.Y; heightOffset < maxHeight; heightOffset += mosaicLayer.EffectSize.Height)
                      {
                          int avgR = 0, avgG = 0, avgB = 0;
                          int blurPixelCount = 0;

                          var currentMaxWidth = Math.Min(widthOffset + mosaicLayer.EffectSize.Width, width);
                          var currentMaxHeight = Math.Min(heightOffset + mosaicLayer.EffectSize.Height, height);

                          for (int x = widthOffset; x < currentMaxWidth; x++)
                          {
                              for (int y = heightOffset; y < currentMaxHeight; y++)
                              {
                                  var pixel = sourceBitmap.GetPixel(x, y);
                                  //三元颜色平均值
                                  avgR += pixel.R;
                                  avgG += pixel.G;
                                  avgB += pixel.B;
                                  //统计
                                  blurPixelCount++;
                              }
                          }

                          // 计算范围平均
                          avgR = avgR / blurPixelCount;
                          avgG = avgG / blurPixelCount;
                          avgB = avgB / blurPixelCount;


                          for (int x = widthOffset; x < currentMaxWidth; x++)
                          {
                              for (int y = heightOffset; y < currentMaxHeight; y++)
                              {
                                  Color newColor = Color.FromArgb(avgR, avgG, avgB);
                                  sourceBitmap.SetPixel(x, y, newColor);
                              }
                          }

                          //Parallel.For(widthOffset, currentMaxWidth, x =>
                          //{
                          //    for (int y = heightOffset; y < currentMaxHeight; y++)
                          //    {
                          //        var pixel = sourceBitmap.GetPixel(x, y);
                          //        //三元颜色平均值
                          //        avgR += pixel.R;
                          //        avgG += pixel.G;
                          //        avgB += pixel.B;
                          //        //统计
                          //        blurPixelCount++;
                          //    }
                          //});

                          //// 计算范围平均
                          //avgR = avgR / blurPixelCount;
                          //avgG = avgG / blurPixelCount;
                          //avgB = avgB / blurPixelCount;

                          //avgR = avgR > 255 ? 255 : avgR;
                          //avgG = avgG > 255 ? 255 : avgG;
                          //avgB = avgB > 255 ? 255 : avgB;


                          //Parallel.For(widthOffset, currentMaxWidth, x =>
                          //{
                          //    for (int y = heightOffset; y < currentMaxHeight; y++)
                          //    {
                          //        Color newColor = Color.FromArgb(avgR, avgG, avgB);
                          //        sourceBitmap.SetPixel(x, y, newColor);
                          //    }
                          //});
                      }

                  });
            }


            return source;



        }
    }
}
