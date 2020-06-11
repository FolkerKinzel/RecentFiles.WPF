using FolkerKinzel.RecentFiles.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
                    // it's moved now to position 1.
                    _tasks.Add(_recentFilesMenu.AddRecentFileAsync(value));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _recentFilesMenu.Initialize(_miRecentFiles);
            _recentFilesMenu.RecentFileSelected += RecentFilesMenu_RecentFileSelected;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Wait all tasks to be finished before disposing the
            // recent files menu:
            Task.WhenAll(_tasks.ToArray());
            _recentFilesMenu.Dispose();
        }

        private void RecentFilesMenu_RecentFileSelected(object? sender, RecentFileSelectedEventArgs e)
        {
            // in Reality: Open the file here!
            CurrentFile = e.FileName;
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog(this) == true)
            {
                // in Reality: Open the file here!
                CurrentFile = dialog.FileName;
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
