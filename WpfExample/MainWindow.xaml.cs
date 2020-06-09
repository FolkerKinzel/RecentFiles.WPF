using FolkerKinzel.RecentFiles.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IRecentFilesMenu _recentFilesMenu;
        private string? _currentFile;

        public event PropertyChangedEventHandler? PropertyChanged;

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
                    _recentFilesMenu.AddRecentFileAsync(value);
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
            _recentFilesMenu.Dispose();
        }

        private void RecentFilesMenu_RecentFileSelected(object? sender, RecentFileSelectedEventArgs e)
        {
            CurrentFile = e.FileName;
        }


        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog(this) == true)
            {
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
