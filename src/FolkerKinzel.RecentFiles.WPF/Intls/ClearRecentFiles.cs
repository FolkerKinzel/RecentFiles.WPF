using System.Windows.Input;

namespace FolkerKinzel.RecentFiles.WPF.Intls;

/// <summary> <see cref="ICommand" /> to delete the list of recently used files.</summary>
internal sealed class ClearRecentFiles : ICommand
{
    private readonly Action _executeHandler;

    /// <summary>Initializes the <see cref="ICommand" />.</summary>
    /// <param name="execute"> <see cref="Action" /> delegate that executes the <see
    /// cref="ICommand" />.</param>
    public ClearRecentFiles(Action execute) => _executeHandler = execute;

    /// <summary>Method that checks whether the <see cref="ICommand" /> can be executed.</summary>
    /// <param name="parameter">Data used by the <see cref="ICommand" /> or <c>null</c>.</param>
    /// <returns> <c>true</c> if the <see cref="ICommand" /> can be executed.</returns>
    public bool CanExecute(object? parameter) => true;

    /// <summary>Event that is fired if the return value of 
    /// <see cref="M:FolkerKinzel.RecentFiles.WPF.OpenRecentFile.CanExecute(System.Object)" /> 
    /// has changed.</summary>
#pragma warning disable 67
    public event EventHandler? CanExecuteChanged;
#pragma warning restore 67


    /// <summary>Method that executes the <see cref="ICommand" />.</summary>
    /// <param name="parameter">Data used by the <see cref="ICommand" /> or <c>null</c>.</param>
    public void Execute(object? parameter) => _executeHandler();
}
