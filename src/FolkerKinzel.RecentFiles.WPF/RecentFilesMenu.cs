using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using FolkerKinzel.RecentFiles.WPF.Intls;
using FolkerKinzel.RecentFiles.WPF.Resources;

namespace FolkerKinzel.RecentFiles.WPF;

/// <summary>Class that adds a menu of recently used files to the WPF application.</summary>
/// <remarks>
/// <para>
/// Add a <see cref="MenuItem" /> to your application as its sub-menu 
/// <see cref="RecentFilesMenu" /> is to be displayed and pass this to the method 
/// <see cref="RecentFilesMenu.Initialize(MenuItem)" /> - then the menu is ready to start.
/// </para>
/// <para>
/// To add a filename to the menu, <see cref="AddRecentFileAsync(string)" /> has
/// to be called. Do this always after opening or saving a file (e.g., in a property
/// "CurrentFileName").
/// </para>
/// <para>
/// To open a file selected from the menu, the application has to subscribe to the
/// event <see cref="RecentFilesMenu.RecentFileSelected" />. The filename is delivered
/// in the <see cref="RecentFileSelectedEventArgs" />.
/// </para>
/// <para>
/// <see cref="RecentFilesMenu" /> persists in small text files with the Naming
/// convention 
/// [<see cref="Environment.MachineName" />].[<see cref="Environment.UserName" />].RF.txt. 
/// Note that the program name is not included. Therefore <see cref="RecentFilesMenu" /> 
/// should be persisted in a folder that is not used by any other program.
/// </para>
/// <para>
/// When exiting the application you should wait for all open tasks of the <see
/// cref="RecentFilesMenu" /> (e.g., with <see cref="Task.WhenAll(IEnumerable{Task})" />) 
/// and then call <see cref="RecentFilesMenu.Dispose()" /> to release the system
/// wide <see cref="Mutex" />, which is used to synchronize the persistence of the
/// <see cref="RecentFilesMenu" /> between multiple instances of the application.
/// </para>
/// </remarks>
/// <example>
/// <para>
/// Initializing the <see cref="RecentFilesMenu" />:
/// </para>
/// <code language="cs" source="..\WpfExample\App.xaml.cs" />
/// <para>
/// Including a <see cref="RecentFilesMenu" /> into a WPF-<see cref="Window" />:
/// </para>
/// <code language="cs" source="..\WpfExample\MainWindow.xaml.cs" />
/// </example>
public sealed class RecentFilesMenu : IRecentFilesMenu, IDisposable
{

    /// <summary>Event that is fired when the user selects a file to open from the menu.</summary>
    public event EventHandler<RecentFileSelectedEventArgs>? RecentFileSelected;

    private readonly RecentFilesPersistence _persistence;
    private readonly ICommand _openRecentFileCommand;
    private readonly ICommand _clearRecentFilesCommand;
    private readonly IconCache _icons = new();

    private MenuItem? _miRecentFiles;

    private readonly int _maxFiles;
    private readonly string? _clearListText;

    /// <summary>Initializes a <see cref="RecentFilesMenu" />.</summary>
    /// <param name="persistenceDirectoryPath">Absolute path of the directory into which the
    /// <see cref="RecentFilesMenu" /> persists. This should be a folder that
    /// is not used by any other program.</param>
    /// <param name="maxFiles">Maximum number of file names to be displayed in the menu
    /// (between 1 and 10).</param>
    /// <param name="clearListText">Text for the menu item "Clear list" or <c>null</c>
    /// to add the English text from the resources.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="persistenceDirectoryPath"
    /// /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"> <paramref name="maxFiles" />
    /// is less than 1 or greater than 10.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="persistenceDirectoryPath" /> contains at least one of the invalid
    /// characters defined in <see cref="Path.GetInvalidPathChars" />
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="persistenceDirectoryPath" /> is not an absolute path
    /// </para>
    /// <para>
    /// - or -
    /// </para>
    /// <para>
    /// <paramref name="persistenceDirectoryPath" /> does not refer to an existing directory.
    /// </para>
    /// </exception>
    public RecentFilesMenu(string persistenceDirectoryPath,
                           int maxFiles = 10,
                           string? clearListText = null)
    {
        if (maxFiles is < 1 or > 10)
        {
            throw new ArgumentOutOfRangeException(nameof(maxFiles));
        }

        _maxFiles = maxFiles;
        this._clearListText = string.IsNullOrWhiteSpace(clearListText) ? null : clearListText;

        _persistence = new RecentFilesPersistence(persistenceDirectoryPath);
        _openRecentFileCommand = new OpenRecentFile(new Action<object?>(OpenRecentFile_Executed));
        _clearRecentFilesCommand = new ClearRecentFiles(new Action(ClearRecentFiles_Executed));
    }

    /// <summary>Assigns the <see cref="RecentFilesMenu" /> the <see cref="MenuItem"
    /// /> as its submenu the <see cref="RecentFilesMenu" /> is displayed. The 
    /// <see cref="RecentFilesMenu" /> is only usable after this method has been
    /// called.</summary>
    /// <param name="miRecentFiles">The <see cref="MenuItem" /> as its submenu the <see
    /// cref="RecentFilesMenu" /> is displayed.</param>
    /// <exception cref="ArgumentNullException"> <paramref name="miRecentFiles" /> is
    /// <c>null</c>.</exception>
    public void Initialize(MenuItem miRecentFiles)
    {
        if (miRecentFiles is null)
        {
            throw new ArgumentNullException(nameof(miRecentFiles));
        }

        _miRecentFiles = miRecentFiles;
        _miRecentFiles.Loaded += miRecentFiles_Loaded;
    }

    /// <summary>Adds <paramref name="fileName" /> to the menu if <paramref name="fileName"
    /// /> contains a filename.</summary>
    /// <param name="fileName">A filename to add. If <paramref name="fileName" /> is
    /// <c>null</c>, empty or whitespace, nothing is added.</param>
    /// <returns>The <see cref="Task" /> that can be awaited.</returns>
    public async Task AddRecentFileAsync(string fileName)
    {
        try
        {
            fileName = Path.GetFullPath(fileName);
        }
        catch
        {
            return;
        }

        await _persistence.LoadAsync().ConfigureAwait(false);

        List<string> recentFiles = _persistence.RecentFiles;

        lock (recentFiles)
        {
            _ = recentFiles.Remove(fileName);
            recentFiles.Insert(0, fileName);

            if (recentFiles.Count > _maxFiles)
            {
                recentFiles.RemoveAt(_maxFiles);
            }
        }

        await _persistence.SaveAsync().ConfigureAwait(false);
    }

    /// <summary>Removes a filename from the <see cref="RecentFilesMenu"/>.</summary>
    /// <param name="fileName">The filename to remove.</param>
    /// <returns>A <see cref="Task" /> that can be awaited.</returns>
    public async Task RemoveRecentFileAsync(string fileName)
    {
        bool result;

        lock (_persistence.RecentFiles)
        {
            result = _persistence.RecentFiles.Remove(fileName);
        }

        if (result)
        {
            await _persistence.SaveAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Returns the name of the most recently opened file or <c>null</c> if
    /// the menu is empty.</summary>
    /// <returns>Name of the most recently opened file or <c>null</c> if the menu is
    /// empty.</returns>
    public async Task<string?> GetMostRecentFileAsync()
    {
        await _persistence.LoadAsync().ConfigureAwait(false);

        lock (_persistence.RecentFiles)
        {
            return _persistence.RecentFiles.FirstOrDefault();
        }
    }

    /// <summary>Releases the resources.</summary>
    /// <remarks>Such a resource is the system wide <see cref="Mutex" />.</remarks>
    public void Dispose() => _persistence.Dispose();


    #region private

    #region miRecentFiles_Loaded

    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
    private async void miRecentFiles_Loaded(object? sender, RoutedEventArgs e)
    {
        // this must be the FIRST code line in the method 
        // (otherwise it might not be executed async and blocks the UI thread)
        // see https://weblog.west-wind.com/posts/2022/Apr/22/Async-and-Async-Void-Event-Handling-in-WPF
        await _persistence.LoadAsync().ConfigureAwait(false);

        Debug.Assert(_miRecentFiles != null);
        await _miRecentFiles.Dispatcher.InvokeAsync(HandleLoaded, DispatcherPriority.Send);
    }

    private void HandleLoaded()
    {
        Debug.Assert(_miRecentFiles != null);

        _miRecentFiles.Items.Clear();

        if (_persistence.RecentFiles.Count == 0)
        {
            _miRecentFiles.IsEnabled = false;
        }
        else
        {
            _miRecentFiles.IsEnabled = true;

            List<string> recentFiles = _persistence.RecentFiles;

            lock (recentFiles)
            {
                for (int i = 0; i < recentFiles.Count; i++)
                {
                    string currentFile = recentFiles[i];

                    if (string.IsNullOrWhiteSpace(currentFile))
                    {
                        recentFiles.RemoveAt(i);
                        --i;
                        continue;
                    }

                    var mi = new MenuItem
                    {
                        Header = FileNameFormatter.GetMenuItemHeaderFromFilename(currentFile, i),
                        Command = _openRecentFileCommand,
                        CommandParameter = recentFiles[i],

                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        VerticalContentAlignment = VerticalAlignment.Stretch,

                        Icon = new Image()
                        {
                            //Width = 16.0,
                            //Height = 16.0,
                            Source = _icons.GetIcon(currentFile)
                        }
                    };

                    _ = _miRecentFiles.Items.Add(mi);
                }
            }

            var menuItemClearList = new MenuItem
            {
                Header = _clearListText ?? Res.ClearList,
                Command = _clearRecentFilesCommand
            };

            _ = _miRecentFiles.Items.Add(new Separator());
            _ = _miRecentFiles.Items.Add(menuItemClearList);
        }
    }

    #endregion

    #region Command-Execute-Handler

    private void OpenRecentFile_Executed(object? fileName)
    {
        if (fileName is string s)
        {
            RecentFileSelected?.Invoke(this, new RecentFileSelectedEventArgs(s));
        }
    }

    private void ClearRecentFiles_Executed()
    {
        lock (_persistence.RecentFiles)
        {
            _persistence.RecentFiles.Clear();
        }

        Task.Run(_persistence.SaveAsync);
    }

    #endregion

    #region Unit test helpers

    /// <summary>
    /// Helper method to unit test <see cref="ClearRecentFiles_Executed"/>.
    /// </summary>
    internal void TestClearRecentFiles() => ClearRecentFiles_Executed();

    /// <summary>
    /// Helper method to unit test <see cref="OpenRecentFile_Executed(object?)"/>
    /// </summary>
    /// <param name="fileName">The file name to open.</param>
    internal void TestOpenRecentFile(object? fileName) => OpenRecentFile_Executed(fileName);

    /// <summary>
    /// Helper method to support unit tests.
    /// </summary>
    /// <returns>The number of items that are currently in the instance.</returns>
    internal async Task<int> GetFilesCount()
    {
        await _persistence.LoadAsync();
        return _persistence.RecentFiles.Count;
    }

    #endregion

    #endregion
}//class
