using System;
using System.IO;
using System.Security.Cryptography;
using YeeTech.VerificationCode.Interface;
#if NETSTANDARD2_0
using System.DrawingCore;
using System.DrawingCore.Imaging;
#else
using System.Drawing;
using System.Drawing.Imaging;

#endif

namespace YeeTech.VerificationCode.Image
{
    public class ImageVerificationCodeProvider : IImageVerificationCodeFactory
    {
        private static readonly byte[] randb = new byte[4];
        private static readonly RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();

        private readonly IVerificationCode _verificationCode;

        public ImageVerificationCodeProvider(IVerificationCode verificationCode = null)
        {
            _verificationCode = verificationCode ?? new GeneralVerificationCode();
        }

        public bool Twist { get; set; } = true;

        public bool Line { get; set; } = true;

        public bool Noise { get; set; } = true;

        public int FontSize { get; set; } = 14;

        public VerificationCodoeExceptionHandlerDelegate VerificationCodoeExceptionHandler { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ImageDrawCompletedHandlerDelegate ImageDrawCompletedHandler { get; set; }

        public byte[] Draw()
        {
            byte[] buffer;
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var image = DrawImage())
                    {
                        image.Save(ms, ImageFormat.Jpeg);
                        ms.Position = 0;
                        buffer = new byte[ms.Length];
                        ms.Read(buffer, 0, Convert.ToInt32(ms.Length));
                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                VerificationCodoeExceptionHandler?.Invoke(ex);
                return null;
            }

            return buffer;
        }

        private Bitmap DrawImage()
        {
            var image = new Bitmap(Width, Height);
            var graphics = Graphics.FromImage(image);
            graphics.Clear(Color.White);

            // draw lines
            if (Line)
                for (var i = 0; i < 10; i++)
                {
                    var x1 = Next(Width - 1);
                    var x2 = Next(Width - 1);
                    var y1 = Next(Height - 1);
                    var y2 = Next(Height - 1);
                    graphics.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

            Font[] fonts =
            {
                new Font(new FontFamily("Times New Roman"), FontSize + Next(1), FontStyle.Bold),
                new Font(new FontFamily("Georgia"), FontSize + Next(1), FontStyle.Bold),
                new Font(new FontFamily("Arial"), FontSize + Next(1), FontStyle.Bold),
                new Font(new FontFamily("Comic Sans MS"), FontSize + Next(1), FontStyle.Bold)
            };
            var code = _verificationCode.Generate(out var result);
            // draw text
            int x = -10, y, codeLen = code.Length, maxIndex = fonts.Length - 1;
            for (var i = 0; i < codeLen; i++)
            {
                x += Next(12, 16);
                y = Next(-2, 2);
                var @char = code.Substring(i, 1);
                @char = Next(1) == 1 ? @char.ToLower() : @char.ToUpper();
                Brush newBrush = new SolidBrush(GetRandomColor());
                var thePos = new Point(x, y);
                graphics.DrawString(@char, fonts[Next(maxIndex)], newBrush, thePos);
            }

            // draw noise
            if (Noise)
                for (var i = 0; i < 40; i++)
                {
                    x = Next(Width - 1);
                    y = Next(Height - 1);
                    image.SetPixel(x, y, Color.FromArgb(Next(0, 255), Next(0, 255), Next(0, 255)));
                }

            if (Twist) image = TwistImage(image, true, Next(1, 3), Next(4, 6));

            graphics.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, Width - 1, Height - 1);

            ImageDrawCompletedHandler?.Invoke(result);

            return image;
        }


        private static Color GetRandomColor()
        {
            var red = Next(180);
            var green = Next(180);
            var blue = red + green > 300 ? 0 : 400 - red - green;
            blue = blue > 255 ? 255 : blue;
            return Color.FromArgb(red, green, blue);
        }

        private static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultValue, double dPhase)
        {
            const double PI = 6.283185307179586476925286766559;
            var destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            var graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            var dBaseAxisLen = bXDir ? destBmp.Height : (double) destBmp.Width;
            for (var i = 0; i < destBmp.Width; i++)
            for (var j = 0; j < destBmp.Height; j++)
            {
                var dx = bXDir ? PI * j / dBaseAxisLen : PI * i / dBaseAxisLen;
                dx += dPhase;
                var dy = Math.Sin(dx);
                var nOldX = bXDir ? i + (int) (dy * dMultValue) : i;
                var nOldY = bXDir ? j : j + (int) (dy * dMultValue);

                var color = srcBmp.GetPixel(i, j);
                if (nOldX >= 0 && nOldX < destBmp.Width
                               && nOldY >= 0 && nOldY < destBmp.Height)
                    destBmp.SetPixel(nOldX, nOldY, color);
            }

            srcBmp.Dispose();
            return destBmp;
        }


        private static int Next(int max)
        {
            rand.GetBytes(randb);
            var value = BitConverter.ToInt32(randb, 0);
            value = value % (max + 1);
            if (value < 0) value = -value;
            return value;
        }


        private static int Next(int min, int max)
        {
            var value = Next(max - min) + min;
            return value;
        }
    }
}