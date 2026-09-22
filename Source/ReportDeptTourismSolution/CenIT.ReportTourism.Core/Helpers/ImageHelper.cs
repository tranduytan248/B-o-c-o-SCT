using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CenIT.ReportTourism.Core.Helpers
{
    public class ImageHelper
    {
        public static Image ScaleImage(Image image, int maxWidth = 0, int maxHeight = 0)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = maxWidth; // (int)(image.Width * ratio);
            var newHeight = maxHeight; // (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static Image GetThumbnailImage(Image originalImage, Size thumbSize)
        {
            var thWidth = thumbSize.Width;
            var thHeight = thumbSize.Height;
            var i = originalImage;
            var w = i.Width;
            var h = i.Height;
            double ratio = 1;
            var th = thHeight < h ? thHeight : h;
            var tw = thWidth < w ? (int)(ratio * thWidth) : w;

            var target = new Bitmap(tw, th);
            var g = Graphics.FromImage(target);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.High;
            var rect = new Rectangle(0, 0, tw, th);
            g.DrawImage(i, rect, 0, 0, w, h, GraphicsUnit.Pixel);

            g.CompositingMode = CompositingMode.SourceCopy;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            var iResizeImage = target;
            var target1 = new Bitmap(700, 540);
            var g1 = Graphics.FromImage(target1);

            g1.CompositingMode = CompositingMode.SourceCopy;
            g1.CompositingQuality = CompositingQuality.HighQuality;
            g1.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g1.SmoothingMode = SmoothingMode.HighQuality;
            g1.PixelOffsetMode = PixelOffsetMode.HighQuality;

            g1.DrawImage(iResizeImage, new Rectangle(0, 0, 700, 540), 0, 0, 700, 540, GraphicsUnit.Pixel);
            return target1;
        }

        private Size CalcSize(int w, int h)
        {
            var iW = 700;
            var iH = h * 540 / w;

            if (iH > 540)
            {
                iW = w * 540 / h;
                return iW < 700 ? new Size(700 * 2, 700 * h / w * 2) : new Size(iW * 2, 540 * 2);
            }

            iW = 540 * w / h;
            return new Size(iW * 2, 540 * 2);
        }
    }
}