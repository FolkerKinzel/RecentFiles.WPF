using System.IO;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

internal static class Utility
{
#if NET462

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="System.Security.SecurityException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="PathTooLongException"></exception>
    internal static bool IsPathFullyQualified(string path)
    {
        try
        {
            return StringComparer.OrdinalIgnoreCase.Equals(Path.GetFullPath(path), path);
        }
        catch(ArgumentException)
        {
            throw;
        }
        catch(SystemException e)
        { 
            throw new ArgumentException(e.Message, "path", e);
        }
    }
#endif

    internal static bool IsPathDirectory(string path)
    {
        try
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.Directory);
        }
        catch
        {
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static bool IsPathDrive(string path)
    {
        try
        {
            return Directory.GetParent(path) is null;
        }
        catch
        {
            return false;
        }
    }
}
