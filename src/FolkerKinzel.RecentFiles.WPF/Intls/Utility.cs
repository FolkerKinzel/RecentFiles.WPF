using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace FolkerKinzel.RecentFiles.WPF.Intls
{
    internal static class Utility
    {
#if NET461
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
}
