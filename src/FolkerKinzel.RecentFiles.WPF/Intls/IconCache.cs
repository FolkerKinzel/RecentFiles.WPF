using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

internal sealed class IconCache
{
    private static class PathTypes
    {
        public const string Directory = @"\DIRECTORY\";
        public const string Drive = @"\DRIVE\";
        public const string Default = @"\DEFAULT\";
    }

    private readonly Dictionary<string, ImageSource> _iconDic = new(1, StringComparer.OrdinalIgnoreCase);

    private const string ICONS_PATH = "FolkerKinzel.RecentFiles.WPF.Resources.Icons.";
    private const string DIRECTORY_ICON_NAME = "DirectoryIcon.png";
    private const string DRIVE_ICON_NAME = "DriveIcon.png";
    private const string DEFAULT_ICON_NAME = "DefaultFileIcon.png";


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
            return Utility.IsPathDirectory(path) ? Utility.IsPathDrive(path) ? GetDriveIcon() : GetDirectoryIcon()
                                                 : GetDefaultFileIcon();
        }

        if (_iconDic.TryGetValue(extension, out ImageSource? icon))
        {
            return icon;
        }

        if (TryGetFileIcon(path, extension, out icon))
        {
            _iconDic[extension] = icon;
            return icon;
        }

        return GetDefaultFileIcon();
    }


    private static bool TryGetFileIcon(string path, string extension, [NotNullWhen(true)] out ImageSource? icon)
    {
        FileInfo? tmpFile = null;
        if (!File.Exists(path))
        {
            // The method Icon.ExtractAssociatedIcon(path) throws an exception if the file
            // does not exist. So the hack is to create an empty temporary file an delete it
            // afterwards. (Elsewhere a default icon would be displayed.)
            path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension);

            while (File.Exists(path)) // Don't overwrite anything!
            {
                path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + extension);
            }

            try
            {
                tmpFile = new FileInfo(path);
                tmpFile.Create().Close();
            }
            catch { }
        }

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
        finally
        {
            if (tmpFile != null)
            {
                try
                {
                    tmpFile.Delete();
                }
                catch { }
            }
        }
    }


    private static ImageSource ToImageSource(Icon icon)
    {
        using Bitmap bmp2 = icon.Width == 16 && icon.Height == 16 ? icon.ToBitmap() : ResizeIcon(icon, 16, 16);

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
    private ImageSource GetDefaultFileIcon() => GetResourceIcon(PathTypes.Default, DEFAULT_ICON_NAME);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ImageSource GetDriveIcon() => GetResourceIcon(PathTypes.Drive, DRIVE_ICON_NAME);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ImageSource GetDirectoryIcon() => GetResourceIcon(PathTypes.Directory, DIRECTORY_ICON_NAME);

    private ImageSource GetResourceIcon(string pathType, string iconName)
    {
        if (_iconDic.TryGetValue(pathType, out ImageSource? icon))
        {
            return icon;
        }

        using Stream stream = Assembly
                                .GetExecutingAssembly()
                                .GetManifestResourceStream(string.Concat(ICONS_PATH, iconName))!;
        icon = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
        _iconDic[pathType] = icon;
        return icon;
    }
}
