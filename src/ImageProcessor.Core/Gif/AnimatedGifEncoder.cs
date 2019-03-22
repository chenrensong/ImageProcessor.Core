using System;
using System.Drawing;
using System.IO;

namespace ImageProcessor.Gif
{
	public class AnimatedGifEncoder
	{
		protected int width;

		protected int height;

		protected Color transparent = Color.Empty;

		protected int transIndex;

		protected int repeat = -1;

		protected int delay;

		protected bool started;

		protected Stream fs;

		protected Image image;

		protected byte[] pixels;

		protected byte[] indexedPixels;

		protected int colorDepth;

		protected byte[] colorTab;

		protected bool[] usedEntry = new bool[256];

		protected int palSize = 7;

		protected int dispose = -1;

		protected bool closeStream;

		protected bool firstFrame = true;

		protected bool sizeSet;

		protected int sample = 10;

		public void SetDelay(int ms)
		{
			delay = (int)Math.Round((float)ms / 10f);
		}

		public void SetDispose(int code)
		{
			if (code >= 0)
			{
				dispose = code;
			}
		}

		public void SetRepeat(int iter)
		{
			if (iter >= 0)
			{
				repeat = iter;
			}
		}

		public void SetTransparent(Color c)
		{
			transparent = c;
		}

		public bool AddFrame(Image im)
		{
			if (im == null || !started)
			{
				return false;
			}
			bool result = true;
			try
			{
				if (!sizeSet)
				{
					SetSize(im.Width, im.Height);
				}
				image = im;
				GetImagePixels();
				AnalyzePixels();
				if (firstFrame)
				{
					WriteLSD();
					WritePalette();
					if (repeat >= 0)
					{
						WriteNetscapeExt();
					}
				}
				WriteGraphicCtrlExt();
				WriteImageDesc();
				if (!firstFrame)
				{
					WritePalette();
				}
				WritePixels();
				firstFrame = false;
				return result;
			}
			catch (IOException)
			{
				return false;
			}
		}

		public bool Finish()
		{
			if (!started)
			{
				return false;
			}
			bool result = true;
			started = false;
			try
			{
				fs.WriteByte(59);
				fs.Flush();
				if (closeStream)
				{
					fs.Close();
				}
			}
			catch (IOException)
			{
				result = false;
			}
			transIndex = 0;
			fs = null;
			image = null;
			pixels = null;
			indexedPixels = null;
			colorTab = null;
			closeStream = false;
			firstFrame = true;
			return result;
		}

		public void SetFrameRate(float fps)
		{
			if (fps != 0f)
			{
				delay = (int)Math.Round(100f / fps);
			}
		}

		public void SetQuality(int quality)
		{
			if (quality < 1)
			{
				quality = 1;
			}
			sample = quality;
		}

		public void SetSize(int w, int h)
		{
			if (!started || firstFrame)
			{
				width = w;
				height = h;
				if (width < 1)
				{
					width = 320;
				}
				if (height < 1)
				{
					height = 240;
				}
				sizeSet = true;
			}
		}

		public bool Start(Stream os)
		{
			if (os == null)
			{
				return false;
			}
			bool flag = true;
			closeStream = false;
			fs = os;
			try
			{
				WriteString("GIF89a");
			}
			catch (IOException)
			{
				flag = false;
			}
			return started = flag;
		}

		public bool Start(string file)
		{
			bool flag = true;
			try
			{
				fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
				flag = Start(fs);
				closeStream = true;
			}
			catch (IOException)
			{
				flag = false;
			}
			return started = flag;
		}

		protected void AnalyzePixels()
		{
			int num = pixels.Length;
			int num2 = num / 3;
			indexedPixels = new byte[num2];
			NeuQuant neuQuant = new NeuQuant(pixels, num, sample);
			colorTab = neuQuant.Process();
			int num3 = 0;
			for (int i = 0; i < num2; i++)
			{
				int num7 = neuQuant.Map(pixels[num3++] & 0xFF, pixels[num3++] & 0xFF, pixels[num3++] & 0xFF);
				usedEntry[num7] = true;
				indexedPixels[i] = (byte)num7;
			}
			pixels = null;
			colorDepth = 8;
			palSize = 7;
			if (transparent != Color.Empty)
			{
				transIndex = FindClosest(transparent);
			}
		}

		protected int FindClosest(Color c)
		{
			if (colorTab == null)
			{
				return -1;
			}
			int r = c.R;
			int g = c.G;
			int b = c.B;
			int result = 0;
			int num = 16777216;
			int num2 = colorTab.Length;
			for (int i = 0; i < num2; i++)
			{
				int num4 = r - (colorTab[i++] & 0xFF);
				int num6 = g - (colorTab[i++] & 0xFF);
				int num7 = b - (colorTab[i] & 0xFF);
				int num8 = num4 * num4 + num6 * num6 + num7 * num7;
				int num9 = i / 3;
				if (usedEntry[num9] && num8 < num)
				{
					num = num8;
					result = num9;
				}
			}
			return result;
		}

		protected void GetImagePixels()
		{
			int num = this.image.Width;
			int num2 = this.image.Height;
			if (num != width || num2 != height)
			{
				Image image = new Bitmap(width, height);
				Graphics graphics = Graphics.FromImage(image);
				graphics.DrawImage(this.image, 0, 0);
				this.image = image;
				graphics.Dispose();
			}
			pixels = new byte[3 * this.image.Width * this.image.Height];
			int num3 = 0;
			Bitmap bitmap = new Bitmap(this.image);
			for (int i = 0; i < this.image.Height; i++)
			{
				for (int j = 0; j < this.image.Width; j++)
				{
					Color pixel = bitmap.GetPixel(j, i);
					pixels[num3] = pixel.R;
					num3++;
					pixels[num3] = pixel.G;
					num3++;
					pixels[num3] = pixel.B;
					num3++;
				}
			}
		}

		protected void WriteGraphicCtrlExt()
		{
			fs.WriteByte(33);
			fs.WriteByte(249);
			fs.WriteByte(4);
			int num;
			int num2;
			if (transparent == Color.Empty)
			{
				num = 0;
				num2 = 0;
			}
			else
			{
				num = 1;
				num2 = 2;
			}
			if (dispose >= 0)
			{
				num2 = (dispose & 7);
			}
			num2 <<= 2;
			fs.WriteByte(Convert.ToByte(num2 | num));
			WriteShort(delay);
			fs.WriteByte(Convert.ToByte(transIndex));
			fs.WriteByte(0);
		}

		protected void WriteImageDesc()
		{
			fs.WriteByte(44);
			WriteShort(0);
			WriteShort(0);
			WriteShort(width);
			WriteShort(height);
			if (firstFrame)
			{
				fs.WriteByte(0);
			}
			else
			{
				fs.WriteByte(Convert.ToByte(0x80 | palSize));
			}
		}

		protected void WriteLSD()
		{
			WriteShort(width);
			WriteShort(height);
			fs.WriteByte(Convert.ToByte(0xF0 | palSize));
			fs.WriteByte(0);
			fs.WriteByte(0);
		}

		protected void WriteNetscapeExt()
		{
			fs.WriteByte(33);
			fs.WriteByte(byte.MaxValue);
			fs.WriteByte(11);
			WriteString("NETSCAPE2.0");
			fs.WriteByte(3);
			fs.WriteByte(1);
			WriteShort(repeat);
			fs.WriteByte(0);
		}

		protected void WritePalette()
		{
			fs.Write(colorTab, 0, colorTab.Length);
			int num = 768 - colorTab.Length;
			for (int i = 0; i < num; i++)
			{
				fs.WriteByte(0);
			}
		}

		protected void WritePixels()
		{
			LZWEncoder lZWEncoder = new LZWEncoder(width, height, indexedPixels, colorDepth);
			lZWEncoder.Encode(fs);
		}

		protected void WriteShort(int value)
		{
			fs.WriteByte(Convert.ToByte(value & 0xFF));
			fs.WriteByte(Convert.ToByte((value >> 8) & 0xFF));
		}

		protected void WriteString(string s)
		{
			char[] array = s.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				fs.WriteByte((byte)array[i]);
			}
		}
	}
}
