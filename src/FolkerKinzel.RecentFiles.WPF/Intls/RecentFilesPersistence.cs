using System.Globalization;
using System.IO;
using FolkerKinzel.RecentFiles.WPF.Resources;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

internal sealed class RecentFilesPersistence : IDisposable
{
    private const int MUTEX_TIMEOUT = 3000;

    private readonly string _fileName;
    private readonly Mutex _mutex;

    public List<string> RecentFiles { get; } = new List<string>();


    /// <summary>
    /// Initialisiert ein <see cref="RecentFilesPersistence"/>-Objekt.
    /// </summary>
    /// <param name="persistenceDirectoryPath">Absoluter Pfad des Verzeichnisses, in das <see cref="RecentFilesMenu"/>
    /// persistiert wird.</param>
    /// <exception cref="ArgumentNullException"><paramref name="persistenceDirectoryPath"/> ist <c>null</c>.</exception>
    /// <exception cref="ArgumentException">
    /// <para>
    /// <paramref name="persistenceDirectoryPath"/> enthält mindestens eines der in <see cref="Path.GetInvalidPathChars"/> definierten ungültigen Zeichen
    /// </para>
    /// <para>- oder -</para>
    /// <para>
    /// <paramref name="persistenceDirectoryPath"/> ist kein absoluter Pfad
    /// </para>
    /// <para>- oder -</para>
    /// <para>
    /// <paramref name="persistenceDirectoryPath"/> verweist nicht auf einen existierenden Ordner.
    /// </para>
    /// </exception>
    public RecentFilesPersistence(string persistenceDirectoryPath)
    {
        if (persistenceDirectoryPath is null)
        {
            throw new ArgumentNullException(persistenceDirectoryPath);
        }

        _fileName = Path.Combine(persistenceDirectoryPath, $"{Environment.MachineName}.{Environment.UserName}.RF.txt");

#if NET461
            if (!Utility.IsPathFullyQualified(_fileName))
#else
        if (!Path.IsPathFullyQualified(_fileName))
#endif
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Res.FilePathNotFullyQualified, nameof(persistenceDirectoryPath)), nameof(persistenceDirectoryPath));
        }

        if (!Utility.IsPathDirectory(persistenceDirectoryPath))
        {
            throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, Res.NotAnExistingDirectory, nameof(persistenceDirectoryPath)), nameof(persistenceDirectoryPath));
        }

        this._mutex = new Mutex(false, $"Global\\{_fileName.Replace('\\', '_')}");
    }

    public Task LoadAsync()
    {
        return Task.Run(() =>
        {
            if (File.Exists(_fileName))
            {
                string[] arr = Array.Empty<string>();
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
                catch (AbandonedMutexException)
                {
                    return;
                }
                catch (ObjectDisposedException)
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

    public Task SaveAsync()
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
            catch (AbandonedMutexException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
        });
    }

    public void Dispose() => _mutex.Dispose();
}
