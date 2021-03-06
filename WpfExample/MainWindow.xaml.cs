using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FolkerKinzel.RecentFiles.WPF;
using Microsoft.Win32;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ConcurrentBag<Task> _tasks = new ConcurrentBag<Task>();
        private readonly IRecentFilesMenu _recentFilesMenu;
        private string? _currentFile;


        public MainWindow(IRecentFilesMenu recentFilesMenu)
        {
            this._recentFilesMenu = recentFilesMenu;
            InitializeComponent();
        }


        public string? CurrentFile
        {
            get => _currentFile;
            private set
            {
                _currentFile = value;
                OnPropertyChanged();

                if (value != null)
                {
                    // Adds the current file to the RecentFilesMenu.
                    // If the RecentFilesMenu already contains the file,
                    // it's moved now to the first position.
                    _tasks.Add(_recentFilesMenu.AddRecentFileAsync(value));
                }
            }
        }


        private void Window_Loaded(object? sender, RoutedEventArgs e)
        {
            // Assign the RecentFilesMenu the MenuItem next to which the 
            // RecentFilesMenu is to be displayed:
            _recentFilesMenu.Initialize(_miRecentFiles);

            // Register at the RecentFilesMenu.RecentFileSelected event, to open
            // the file, that the user has selected in the RecentFilesMenu:
            _recentFilesMenu.RecentFileSelected += RecentFilesMenu_RecentFileSelected;
        }


        private async void Window_Closed(object? sender, EventArgs e)
        {
            // Wait all tasks to be finished before disposing the
            // recent files menu:
            await Task.WhenAll(_tasks.ToArray()).ConfigureAwait(false);
            _recentFilesMenu.Dispose();
        }


        private void Quit_Click(object? sender, RoutedEventArgs e) => Close();


        private void RecentFilesMenu_RecentFileSelected(
            object? sender,
            RecentFileSelectedEventArgs e) => OpenFile(e.FileName);


        private void Open_Executed(object? sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog(this) == true)
            {
                OpenFile(dialog.FileName);
            }
        }


        private void OpenFile(string fileName)
        {
            try
            {
                //  Open the file here!
            }
            catch (IOException)
            {
                _tasks.Add(_recentFilesMenu.RemoveRecentFileAsync(fileName));
                CurrentFile = null;
                return;
            }

            CurrentFile = fileName;
        }


        private void OnPropertyChanged([CallerMemberName] string propName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
