namespace FolkerKinzel.RecentFiles.WPF;

/// <summary> <see cref="EventArgs" /> for the 
/// <see cref="RecentFilesMenu.RecentFileSelected" /> event.</summary>
public sealed class RecentFileSelectedEventArgs : EventArgs
{
    /// <summary>Initializes a <see cref="RecentFileSelectedEventArgs" /> object.</summary>
    /// <param name="fileName">The filename of the selected file.</param>
    internal RecentFileSelectedEventArgs(string fileName) => FileName = fileName;

    /// <summary>The selected filename.</summary>
    public string FileName { get; }
}
