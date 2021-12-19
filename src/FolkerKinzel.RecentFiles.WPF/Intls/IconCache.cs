using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FolkerKinzel.RecentFiles.WPF.Intls
{
    internal sealed class IconCache
    {
        private static class PathTypes
        {
            public const string Directory = @"\DIRECTORY\";
            public const string Drive = @"\DRIVE\";
            public const string Default = @"\DEFAULT\";
        }

        private readonly Dictionary<string, ImageSource> _iconDic = new Dictionary<string, ImageSource>(4, StringComparer.OrdinalIgnoreCase);

        private const string iconsPath = "FolkerKinzel.RecentFiles.WPF.Resources.Icons.";
        private const string directoryIconName = "DirectoryIcon.png";
        private const string driveIconName = "DriveIcon.png";
        private const string defaultIconName = "DefaultFileIcon.png";



        internal IconCache()
        {
            //var assembly = Assembly.GetExecutingAssembly();
            //using (Stream stream = assembly
            //                        .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DirectoryIcon.png")!)
            //{
            //    _iconDic[PathTypes.Directory] = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            //}

            //using (Stream stream = assembly
            //                        .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DriveIcon.png")!)
            //{
            //    _iconDic[PathTypes.Drive] = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            //}

            _ = GetDirectoryIcon();
            _ = GetDriveIcon();

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

            if (extension.Length == 0)
            {
                if (Utility.IsPathDirectory(path))
                {
                    return Utility.IsPathDrive(path) ? GetDriveIcon() : GetDirectoryIcon();
                }
                return GetDefaultFileIcon();
            }

            if (_iconDic.TryGetValue(extension, out ImageSource? icon))
            {
                return icon;
            }
            //else if (Utility.IsPathDirectory(path))
            //{
            //    return Utility.IsPathDrive(path) ? _iconDic[PathTypes.Drive] : _iconDic[PathTypes.Directory];
            //}
            //else if (extension.Length != 0)
            //{
            if (TryGetFileIcon(path, out icon))
            {
                _iconDic[extension] = icon;
                return icon;
            }
            //}

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImageSource GetDefaultFileIcon() => GetResourceIcon(PathTypes.Default, defaultIconName);
        //{
        //    using Stream stream = Assembly
        //        .GetExecutingAssembly()
        //        .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DefaultFileIcon.png")!;

        //    return BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImageSource GetDriveIcon() => GetResourceIcon(PathTypes.Drive, driveIconName);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ImageSource GetDirectoryIcon() => GetResourceIcon(PathTypes.Directory, directoryIconName);

        private ImageSource GetResourceIcon(string pathType, string iconName)
        {
            if (_iconDic.TryGetValue(pathType, out ImageSource? icon))
            {
                return icon;
            }

            using Stream stream = Assembly
                                    .GetExecutingAssembly()
                                    .GetManifestResourceStream(string.Concat(iconsPath, iconName))!;
            icon = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
            _iconDic[pathType] = icon;
            return icon;
        }
    }
}
