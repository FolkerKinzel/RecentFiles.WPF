using System;

namespace FolkerKinzel.RecentFiles.WPF
{
    /// <summary>
    /// <see cref="EventArgs"/> für das <see cref="RecentFilesMenu.RecentFileSelected"/>-Event.
    /// </summary>
    public class RecentFileSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Initialisiert ein <see cref="RecentFileSelectedEventArgs"/>-Objekt.
        /// </summary>
        /// <param name="fileName">Der Dateiname der ausgewählten Datei.</param>
        public RecentFileSelectedEventArgs(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Der ausgewählte Dateiname.
        /// </summary>
        public string FileName { get; }
    }

}
