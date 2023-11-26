using System.Globalization;
using System.IO;
using FolkerKinzel.RecentFiles.WPF.Resources;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

internal sealed class RecentFilesPersistence : IDisposable
{
    private const int MUTEX_TIMEOUT = 3000;

    private readonly string _fileName;
    private readonly Mutex _mutex;

    internal List<string> RecentFiles { get; } = [];

    /// <summary>Initializes a <see cref="RecentFilesPersistence" /> instance.</summary>
    /// <param name="persistenceDirectoryPath">Absolute path of the directory, into
    /// the <see cref="RecentFilesMenu" /> is persisted.</param>
    /// <exception cref="ArgumentNullException"> 
    /// <paramref name="persistenceDirectoryPath" /> is <c>null</c>.
    /// </exception>
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
    internal RecentFilesPersistence(string persistenceDirectoryPath)
    {
        if (persistenceDirectoryPath is null)
        {
            throw new ArgumentNullException(persistenceDirectoryPath);
        }

        _fileName = Path.Combine(persistenceDirectoryPath,
                                 $"{Environment.MachineName}.{Environment.UserName}.RF.txt");

#if NET462
        if (!Utility.IsPathFullyQualified(_fileName))
#else
        if (!Path.IsPathFullyQualified(_fileName))
#endif
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture,
                              Res.FilePathNotFullyQualified,
                              nameof(persistenceDirectoryPath)), nameof(persistenceDirectoryPath));
        }

        if (!Utility.IsPathDirectory(persistenceDirectoryPath))
        {
            throw new ArgumentException(
                string.Format(CultureInfo.InvariantCulture,
                              Res.NotAnExistingDirectory,
                              nameof(persistenceDirectoryPath)), nameof(persistenceDirectoryPath));
        }

        this._mutex = new Mutex(false, $"Global\\{_fileName.Replace('\\', '_')}");
    }

    internal Task LoadAsync()
    {
        return Task.Run(() =>
        {
            if (File.Exists(_fileName))
            {
                string[] arr = [];

                try
                {
                    if (_mutex.WaitOne(MUTEX_TIMEOUT))
                    {
                        try
                        {
                            arr = File.ReadAllLines(_fileName);
                        }
                        catch { return; }
                        finally
                        {
                            _mutex.ReleaseMutex();
                        }
                    }
                }
                catch
                {
                    return;
                }

                lock (RecentFiles)
                {
                    RecentFiles.Clear();
                    RecentFiles.AddRange(arr);
                }
            }
        });
    }

    internal Task SaveAsync()
    {
        return Task.Run(() =>
        {
            try
            {
                if (_mutex.WaitOne(MUTEX_TIMEOUT))
                {
                    try
                    {
                        lock (RecentFiles)
                        {
                            File.WriteAllLines(_fileName, RecentFiles);
                        }
                    }
                    catch { }
                    finally
                    {
                        _mutex.ReleaseMutex();
                    }
                }
            }
            catch
            {
            }
        });
    }

    public void Dispose() => _mutex.Dispose();
}
