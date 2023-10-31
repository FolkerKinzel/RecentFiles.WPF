using System.Windows.Input;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

/// <summary> <see cref="ICommand" /> to open a file selected from the 
/// <see cref="RecentFilesMenu" />.</summary>
internal sealed class OpenRecentFile : ICommand
{
    private readonly Action<object?> _executeHandler;

    /// <summary>Initializes the <see cref="ICommand" />.</summary>
    /// <param name="execute"> <see cref="Action" /> delegate that executes the 
    /// <see cref="ICommand" />.</param>
    public OpenRecentFile(Action<object?> execute) => _executeHandler = execute;

    /// <summary>Method that checks whether the <see cref="ICommand" /> can be executed.</summary>
    /// <param name="parameter">Data used by the <see cref="ICommand" /> or <c>null</c>.</param>
    /// <returns> <c>true</c> if the <see cref="ICommand" /> can be executed.</returns>
    public bool CanExecute(object? parameter) => true;

    /// <summary>Event that is fired if the return value of 
    /// <see cref="OpenRecentFile.CanExecute(object)" /> has changed.</summary>
    public event EventHandler? CanExecuteChanged;

    /// <summary>Method that executes the <see cref="ICommand" />.</summary>
    /// <param name="parameter">The filename selected for opening.</param>
    public void Execute(object? parameter) => _executeHandler(parameter);
}
