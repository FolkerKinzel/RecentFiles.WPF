﻿using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FolkerKinzel.RecentFiles.WPF;

namespace WpfExample;

public sealed partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly ConcurrentBag<Task> _tasks = [];
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
        await Task.WhenAll([.. _tasks]).ConfigureAwait(false);
        _recentFilesMenu.Dispose();
    }

    private void Quit_Click(object? sender, RoutedEventArgs e) => Close();

    private void RecentFilesMenu_RecentFileSelected(
        object? sender,
        RecentFileSelectedEventArgs e) => OpenFile(e.FileName);

    private void OpenDirectory_Executed(object? sender, ExecutedRoutedEventArgs e)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog();
        if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            OpenFile(dialog.SelectedPath);
        }
    }

    private void OpenFile_Executed(object? sender, ExecutedRoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog();

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
