using System;
using System.Windows.Input;

namespace FolkerKinzel.RecentFiles
{ 
    /// <summary>
    /// <see cref="ICommand"/> zum Löschen der Liste der zuletzt benutzten Dateien.
    /// </summary>
    public class ClearRecentFiles : ICommand
    {
        private readonly Action _executeHandler;

        /// <summary>
        /// Initialisiert das <see cref="ICommand"/>.
        /// </summary>
        /// <param name="execute"><see cref="Action"/>-Delegate, das das <see cref="ICommand"/> ausführt.</param>
        public ClearRecentFiles(Action execute)
        {
            _executeHandler = execute;
        }

        /// <summary>
        /// Methode, die prüft, ob das <see cref="ICommand"/> ausgeführt werden kann.
        /// </summary>
        /// <param name="parameter">Vom <see cref="ICommand"/> verwendete Daten oder <c>null</c>.</param>
        /// <returns><c>true</c>, wenn das <see cref="ICommand"/> ausgeführt werden kann.</returns>
        public bool CanExecute(object? parameter) => true;


        /// <summary>
        /// Event, dass gefeuert wird, wenn sich der Rückgabewert von <see cref="CanExecute(object)"/> ändert.
        /// </summary>
#pragma warning disable 67
        public event EventHandler? CanExecuteChanged;


        /// <summary>
        /// Methode, die das <see cref="ICommand"/> ausführt.
        /// </summary>
        /// <param name="parameter">Vom <see cref="ICommand"/> verwendete Daten oder <c>null</c>.</param>
        public void Execute(object? parameter)
        {
            _executeHandler();
        }
    }

}
