using System.IO;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

internal static class Utility
{
#if NET462
    internal static bool IsPathFullyQualified(string path) 
        => StringComparer.OrdinalIgnoreCase.Equals(Path.GetFullPath(path), path);
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
    internal static bool IsPathDrive(string path) => Directory.GetParent(path) is null;
}
