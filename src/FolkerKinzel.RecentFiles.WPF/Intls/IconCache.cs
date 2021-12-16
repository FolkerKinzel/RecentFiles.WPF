using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FolkerKinzel.RecentFiles.WPF.Intls
{
    internal sealed class IconCache
    {
        private const string DIRECTORY = @"\DIRECTORY\";
        private const string DRIVE = @"\DRIVE\";

        private readonly Dictionary<string, ImageSource> _iconDic = new Dictionary<string, ImageSource>(4, StringComparer.OrdinalIgnoreCase);

        internal IconCache()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly
                                    .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DirectoryIcon.png")!)
            {
                _iconDic[DIRECTORY] = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            }

            using (Stream stream = assembly
                                    .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DriveIcon.png")!)
            {
                _iconDic[DRIVE] = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            }
        }

        internal ImageSource GetIcon(string path)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(path));
            string? extension;

            try
            {
                extension = Path.GetExtension(path);
            }
            catch (ArgumentException)
            {
                return GetDefaultFileIcon();
            }

            Debug.Assert(extension != null);

            if (_iconDic.TryGetValue(extension, out ImageSource? icon))
            {
                return icon;
            }
            else if (Utility.IsPathDirectory(path))
            {
                return Directory.GetParent(path) is null ? _iconDic[DRIVE] : _iconDic[DIRECTORY];
            }
            else if (extension.Length != 0)
            {
                if (TryGetFileIcon(path, out icon))
                {
                    _iconDic[extension] = icon;
                    return icon;
                }
            }
            
            return GetDefaultFileIcon();
        }

        private static bool TryGetFileIcon(string path, [NotNullWhen(true)] out ImageSource? icon)
        {
            icon = null;
            try
            {
                using var ic = Icon.ExtractAssociatedIcon(path);

                if (ic is null)
                {
                    return false;
                }

                icon = ToImageSource(ic);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static ImageSource ToImageSource(Icon icon)
        {
            using Bitmap bmp2 = icon.Width == 16 && icon.Height == 16 ? icon.ToBitmap() : ResizeIcon(icon, 16, 16);
            //using Bitmap bmp2 = icon.ToBitmap();


            using var ms = new MemoryStream();
            bmp2.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return BitmapFrame.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }

        /// <summary>
        /// Resize the icon to the specified width and height.
        /// </summary>
        /// <param name="icon">The icon to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeIcon(Icon icon, int width, int height)
        {
            using var image = icon.ToBitmap();
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
            }

            return destImage;
        }

        private static ImageSource GetDefaultFileIcon()
        {
            using Stream stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DefaultFileIcon.png")!;

            return BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }
    }
}
