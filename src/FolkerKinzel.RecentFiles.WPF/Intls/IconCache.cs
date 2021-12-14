using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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

        private readonly Dictionary<string, ImageSource> _iconDic = new Dictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);

        internal IconCache() => _iconDic[DIRECTORY] = GetDirectoryIcon();

        internal ImageSource GetIcon(string path)
        {
            string? extension;

            try
            {
                extension = Path.GetExtension(path);
            }
            catch (ArgumentException)
            {
                return GetDefaultFileIcon();
            }

            if (extension != null && _iconDic.TryGetValue(extension, out ImageSource? icon))
            {
                return icon;
            }
            else if (Utility.IsPathDirectory(path))
            {
                return _iconDic[DIRECTORY];
            }
            else
            {
                if (TryGetFileIcon(path, out icon))
                {
                    if (extension != null)
                    {
                        _iconDic[extension] = icon;
                    }

                    return icon;
                }
                else
                {
                    return GetDefaultFileIcon();
                }
            }
        }


        private static ImageSource GetDirectoryIcon()
        {
            using Stream stream = Assembly
                                    .GetExecutingAssembly()
                                    .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DirectoryIcon.png")!;
            return BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }


        private static bool TryGetFileIcon(string path, [NotNullWhen(true)] out ImageSource? icon)
        {
            icon = null;
            try
            {
                var ic = Icon.ExtractAssociatedIcon(path);

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
            var bmp = icon.ToBitmap();

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return BitmapFrame.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
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
