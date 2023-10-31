using System.Windows.Controls;

namespace FolkerKinzel.RecentFiles.WPF;

/// <summary>Interface that represents the public interface of the 
/// <see cref="RecentFilesMenu" /> class.</summary>
public interface IRecentFilesMenu : IDisposable
{
    /// <summary>Event that is fired when the user selects a file to open from the menu.</summary>
    event EventHandler<RecentFileSelectedEventArgs>? RecentFileSelected;

    /// <summary>Assigns the <see cref="RecentFilesMenu" /> the <see cref="MenuItem"
    /// /> as its submenu the <see cref="RecentFilesMenu" /> is displayed. This method
    /// must be called before everyone else!</summary>
    /// <param name="miRecentFiles">The <see cref="MenuItem" /> as its submenu the <see
    /// cref="RecentFilesMenu" /> is displayed.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="miRecentFiles" /> is
    /// <c>null</c>.</exception>
    void Initialize(MenuItem miRecentFiles);

    /// <summary>Adds <paramref name="fileName" /> to the menu if <paramref name="fileName"
    /// /> contains a filename.</summary>
    /// <param name="fileName">A filename to add. If <paramref name="fileName" /> is
    /// <c>null</c>, empty or whitespace, nothing is added.</param>
    /// <returns>The <see cref="Task" /> that can be awaited.</returns>
    Task AddRecentFileAsync(string fileName);

    /// <summary>Returns the name of the most recently opened file or <c>null</c> if
    /// the menu is empty.</summary>
    /// <returns>Name of the most recently opened file or <c>null</c> if the menu is
    /// empty.</returns>
    Task<string?> GetMostRecentFileAsync();

    /// <summary>Removes a filename from the menu.</summary>
    /// <param name="fileName">The filename to remove.</param>
    /// <returns>The <see cref="Task" /> that can be awaited.</returns>
    Task RemoveRecentFileAsync(string fileName);
}
