﻿using FolkerKinzel.RecentFiles.WPF.Intls;
using FolkerKinzel.RecentFiles.WPF.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FolkerKinzel.RecentFiles.WPF
{
    /// <summary>
    /// Klasse, die der WPF-Anwendung ein Menü mit den zuletzt verwendeten Dateien hinzufügt.
    /// </summary>
    /// <remarks>
    /// <para>Fügen Sie Ihrer Anwendung ein <see cref="MenuItem"/> hinzu, neben dem <see cref="RecentFilesMenu"/> angezeigt werden soll und 
    /// übergeben Sie dieses der Methode <see cref="RecentFilesMenu.Initialize(MenuItem)"/> - dann ist das Menü startklar.</para>
    /// <para>Um einen Dateinamen zum Menü hinzuzufügen, muss <see cref="AddRecentFileAsync(string)"/>
    /// aufgerufen werden. Das sollte immer nach dem Öffnen einer Datei geschehen (z.B. in einer Property "CurrentFileName").</para>
    /// <para>Um eine Datei zu öffnen, muss das Event <see cref="RecentFileSelected"/> abonniert werden. Der Dateiname wird in den 
    /// <see cref="RecentFileSelectedEventArgs"/> geliefert.</para> 
    /// <para><see cref="RecentFilesMenu"/> persistiert sich in kleinen Textdateien mit der Namenskonvention
    /// [<see cref="Environment.MachineName"/>].[<see cref="Environment.UserName"/>].RF.txt</para>
    /// <para>Beim Beenden der Anwendung sollten Sie sämtliche offenen Tasks mit <see cref="Task.WhenAll(IEnumerable{Task})"/> abwarten und dann 
    /// <see cref="RecentFilesMenu.Dispose()"/> aufrufen.</para>
    /// </remarks>
    /// <example>
    /// <para>Initialisieren von <see cref="RecentFilesMenu"/>:</para>
    /// <code language="cs" source="..\WpfExample\App.xaml.cs" />
    /// <para>Einbinden von <see cref="RecentFilesMenu"/> in ein WPF-<see cref="Window"/>:</para>
    /// <code language="cs" source="..\WpfExample\MainWindow.xaml.cs" />
    /// </example>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Keine allgemeinen Ausnahmetypen abfangen", Justification = "<Ausstehend>")]
    public sealed class RecentFilesMenu : IRecentFilesMenu, IDisposable
    {
        const int MAX_DISPLAYED_FILE_PATH_LENGTH = 60;


        /// <summary>
        /// Event, das gefeuert wird, wenn der Benutzer im Menü eine Datei zum Öffnen auswählt.
        /// </summary>
        public event EventHandler<RecentFileSelectedEventArgs>? RecentFileSelected;


        #region private fields

        private readonly RecentFilesPersistence _persistence;
        readonly ICommand _openRecentFileCommand;
        readonly ICommand _clearRecentFilesCommand;
        readonly IconCache _icons = new IconCache();

        MenuItem? _miRecentFiles;


        private readonly int _maxFiles;
        private readonly string? _clearListText;

        #endregion


        #region ctor

        /// <summary>
        /// Initialisiert ein neues <see cref="RecentFilesMenu"/>.
        /// </summary>
        /// <param name="persistenceDirectoryPath">Absoluter Pfad des Verzeichnisses, in das <see cref="RecentFilesMenu"/>
        /// persistiert wird.</param>
        /// <param name="maxFiles">Maximalanzahl der im Menü anzuzeigenden Dateinamen (zwischen 1 und 10).</param>
        /// <param name="clearListText">Text für den Menüpunkt "Liste leeren" oder <c>null</c>, um den Text aus den Ressourcen zu 
        /// nutzen. (Es gibt Ressourcen für Deutsch und Englisch.)</param>
        /// <exception cref="ArgumentNullException"><paramref name="persistenceDirectoryPath"/> ist <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxFiles"/> ist kleiner als 1 oder größer als 10.</exception>
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
        /// </para></exception>
        public RecentFilesMenu(string persistenceDirectoryPath, int maxFiles = 10, string? clearListText = null)
        {
            if (maxFiles < 1 || maxFiles > 10)
            {
                throw new ArgumentOutOfRangeException(nameof(maxFiles));
            }

            _maxFiles = maxFiles;
            this._clearListText = string.IsNullOrWhiteSpace(clearListText) ? null : clearListText;

            _persistence = new RecentFilesPersistence(persistenceDirectoryPath);
            _openRecentFileCommand = new OpenRecentFile(new Action<object>(OpenRecentFile_Executed));
            _clearRecentFilesCommand = new ClearRecentFiles(new Action(ClearRecentFiles_Executed));
        }

        #endregion


        #region public Methods

        /// <summary>
        /// Weist dem <see cref="RecentFilesMenu"/> das <see cref="MenuItem"/> zu, als dessen Child das
        /// <see cref="RecentFilesMenu"/> angezeigt wird. Diese Methode muss vor allen anderen aufgerufen werden!
        /// </summary>
        /// <param name="miRecentFiles">Das <see cref="MenuItem"/> zu, als dessen Child das
        /// <see cref="RecentFilesMenu"/> angezeigt wird.</param>
        /// <exception cref="ArgumentNullException"><paramref name="miRecentFiles"/> ist <c>null</c>.</exception>
        public void Initialize(MenuItem miRecentFiles)
        {
            if (miRecentFiles is null)
            {
                throw new ArgumentNullException(nameof(miRecentFiles));
            }

            _miRecentFiles = miRecentFiles;
            _miRecentFiles.Loaded += miRecentFiles_Loaded;
        }

        /// <summary>
        /// Fügt <paramref name="fileName"/> zur Liste hinzu, wenn 
        /// <paramref name="fileName"/> einen Dateinamen enthält.
        /// </summary>
        /// <param name="fileName">Ein hinzuzufügender Dateiname. Wenn <paramref name="fileName"/>&#160;<c>null</c>, 
        /// leer oder Whitespace ist, wird nichts hinzugefügt.</param>
        /// <returns>Der <see cref="Task"/>, auf dessen Beendigung gewartet werden kann.</returns>
        public async Task AddRecentFileAsync(string fileName)
        {
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                await _persistence.LoadAsync().ConfigureAwait(false);

                var recentFiles = _persistence.RecentFiles;

                lock (recentFiles)
                {
                    recentFiles.Remove(fileName);
                    recentFiles.Insert(0, fileName);

                    if (recentFiles.Count > _maxFiles)
                    {
                        recentFiles.RemoveAt(_maxFiles);
                    }
                }

                await _persistence.SaveAsync().ConfigureAwait(false);
            }
        }



        /// <summary>
        /// Enfernt einen Dateinamen aus der Liste.
        /// </summary>
        /// <param name="fileName">Der zu entfernende Dateiname.</param>
        /// <returns>Der <see cref="Task"/>, auf dessen Beendigung gewartet werden kann.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Keine allgemeinen Ausnahmetypen abfangen", Justification = "<Ausstehend>")]
        public Task RemoveRecentFileAsync(string fileName)
        {
            bool result;
            lock (_persistence.RecentFiles)
            {
                result = _persistence.RecentFiles.Remove(fileName);
            }

            return result ? _persistence.SaveAsync() : Task.CompletedTask;
        }


        /// <summary>
        /// Gibt den Namen der zuletzt geöffneten Datei zurück oder <c>null</c>, wenn dieser nicht existiert.
        /// </summary>
        /// <returns>Name der zuletzt geöffneten Datei oder <c>null</c>, wenn dieser nicht existiert.</returns>
        public async Task<string?> GetMostRecentFileAsync()
        {
            await _persistence.LoadAsync().ConfigureAwait(false);

            lock (_persistence.RecentFiles)
            {
                return _persistence.RecentFiles.FirstOrDefault();
            }
        }


        /// <summary>
        /// Gibt die Ressourcen frei.
        /// </summary>
        /// <remarks>
        /// Eine solche Ressource ist der systemweite <see cref="Mutex"/>.
        /// </remarks>
        public void Dispose()
        {
            _persistence.Dispose();
        }

        #endregion


        #region private


        #region miRecentFiles_Loaded

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Benennungsstile", Justification = "<Ausstehend>")]
        private async void miRecentFiles_Loaded(object sender, RoutedEventArgs e)
        {
            if (_miRecentFiles is null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Res.NotInitialized, nameof(Initialize)));
            }

            await _persistence.LoadAsync().ConfigureAwait(true);

            _miRecentFiles.Items.Clear();

            if (_persistence.RecentFiles.Count == 0)
            {
                _miRecentFiles.IsEnabled = false;
            }
            else
            {
                try
                {
                    _miRecentFiles.IsEnabled = true;

                    var recentFiles = _persistence.RecentFiles;

                    lock (recentFiles)
                    {
                        for (int i = 0; i < recentFiles.Count; i++)
                        {
                            string currentFile = recentFiles[i];
                            //if (currentFile is null)
                            //{
                            //    continue;
                            //}

                            var mi = new MenuItem
                            {
                                Header = GetMenuItemHeaderFromFilename(currentFile, i),
                                Command = _openRecentFileCommand,
                                CommandParameter = recentFiles[i],

                                Icon = new Image()
                                {
                                    Width = 16.0,
                                    Height = 16.0,
                                    Source = _icons.GetIcon(currentFile)
                                }
                            };

                            _miRecentFiles.Items.Add(mi);
                        }
                    }

                    var menuItemClearList = new MenuItem
                    {
                        Header = _clearListText ?? Res.ClearList,
                        Command = _clearRecentFilesCommand
                    };

                    _miRecentFiles.Items.Add(new Separator());
                    _miRecentFiles.Items.Add(menuItemClearList);
                }
                catch
                {
                    _miRecentFiles.Items.Clear();
                    _miRecentFiles.IsEnabled = false;

                    ClearRecentFiles_Executed();
                }
            }

            ////////////////////////////////////////////////////////////////////

            static string GetMenuItemHeaderFromFilename(string fileName, int i)
            {
                if (fileName.Length > MAX_DISPLAYED_FILE_PATH_LENGTH)
                {
                    int fileNameLength = Path.GetFileName(fileName).Length + 1;
                    int restLength = MAX_DISPLAYED_FILE_PATH_LENGTH - fileNameLength - 3;
                    fileName = restLength >= 0 ?
                        fileName.Substring(0, restLength) + "..." + fileName.Substring(fileName.Length - fileNameLength) :
                        fileName;
                }

                if (i < 9)
                {
                    fileName = "_" + (i + 1).ToString(CultureInfo.InvariantCulture) + ": " + fileName;
                }
                else if (i == 9)
                {
                    fileName = "1_0: " + fileName;
                }

                return fileName;
            }
        }

        #endregion


        #region Command-Execute-Handler

        private void OpenRecentFile_Executed(object fileName)
        {
            OnRecentFileSelected((string)fileName);
        }


        private void ClearRecentFiles_Executed()
        {
            lock (_persistence.RecentFiles)
            {
                _persistence.RecentFiles.Clear();
            }
            _persistence.SaveAsync();
        }

        #endregion


        private void OnRecentFileSelected(string fileName)
        {
            RecentFileSelected?.Invoke(this, new RecentFileSelectedEventArgs(fileName));
        }

        #endregion

    }//class

}