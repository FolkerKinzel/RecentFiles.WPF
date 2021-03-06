﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
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
                if (_iconDic.TryGetValue(DIRECTORY, out icon))
                {
                    return icon;
                }
                else
                {
                    using Stream stream = 
                        Assembly
                        .GetExecutingAssembly()
                        .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DirectoryIcon.png")!;

                    icon = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);

                    _iconDic[DIRECTORY] = icon;

                    return icon;

                }
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


        private static ImageSource ToImageSource(System.Drawing.Icon icon)
        {
            using var ms = new MemoryStream();
            icon.Save(ms);
            ms.Position = 0;
            return BitmapFrame.Create(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }


        private static ImageSource GetDefaultFileIcon()
        {
            using Stream stream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream("FolkerKinzel.RecentFiles.WPF.Resources.Icons.DefaultFileIcon.ico")!;

            return BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        }
    }
}
