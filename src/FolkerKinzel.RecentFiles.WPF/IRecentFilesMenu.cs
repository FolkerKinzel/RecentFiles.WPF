using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FolkerKinzel.RecentFiles.WPF
{
    /// <summary>
    /// Interface, das die öffentliche Schnittstelle von <see cref="RecentFilesMenu"/> darstellt.
    /// </summary>
    public interface IRecentFilesMenu : IDisposable
    {
        /// <summary>
        /// Event, das gefeuert wird, wenn der Benutzer im Menü eine Datei zum Öffnen auswählt.
        /// </summary>
        event EventHandler<RecentFileSelectedEventArgs>? RecentFileSelected;

        /// <summary>
        /// Weist dem <see cref="RecentFilesMenu"/> das <see cref="MenuItem"/> zu, als dessen Submenü das
        /// <see cref="RecentFilesMenu"/> angezeigt wird. Diese Methode muss vor allen anderen aufgerufen werden!
        /// </summary>
        /// <param name="miRecentFiles">Das <see cref="MenuItem"/>, als dessen Submenü das
        /// <see cref="RecentFilesMenu"/> angezeigt wird.</param>
        /// <exception cref="ArgumentNullException"><paramref name="miRecentFiles"/> ist <c>null</c>.</exception>
        void Initialize(MenuItem miRecentFiles);

        /// <summary>
        /// Fügt <paramref name="fileName"/> zur Liste hinzu, wenn 
        /// <paramref name="fileName"/> einen Dateinamen enthält.
        /// </summary>
        /// <param name="fileName">Ein hinzuzufügender Dateiname. Wenn <paramref name="fileName"/>&#160;<c>null</c>, 
        /// leer oder Whitespace ist, wird nichts hinzugefügt.</param>
        /// <returns>Der <see cref="Task"/>, auf dessen Beendigung gewartet werden kann.</returns>
        Task AddRecentFileAsync(string fileName);

        /// <summary>
        /// Gibt den Namen der zuletzt geöffneten Datei zurück oder <c>null</c>, wenn dieser nicht existiert.
        /// </summary>
        /// <returns>Name der zuletzt geöffneten Datei oder <c>null</c>, wenn dieser nicht existiert.</returns>
        Task<string?> GetMostRecentFileAsync();

        /// <summary>
        /// Enfernt einen Dateinamen aus der Liste.
        /// </summary>
        /// <param name="fileName">Der zu entfernende Dateiname.</param>
        /// <returns>Der <see cref="Task"/>, auf dessen Beendigung gewartet werden kann.</returns>
        Task RemoveRecentFileAsync(string fileName);
    }
}