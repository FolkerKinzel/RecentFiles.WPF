using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FolkerKinzel.RecentFiles.WPF.Intls
{
    internal static class Utility
    {
        internal static bool IsPathFullyQualified(string path) => StringComparer.OrdinalIgnoreCase.Equals(Path.GetFullPath(path), path);


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Keine allgemeinen Ausnahmetypen abfangen", Justification = "<Ausstehend>")]
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
    }
}
