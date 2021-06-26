using System.Diagnostics;
using System.IO;
using System.Windows;
using FolkerKinzel.RecentFiles.WPF;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object? sender, StartupEventArgs e)
        {
            // The example initializes a new RecentFilesMenu which persists its data in
            // the same directory, where the program exe-file is. The constructor has 
            // optional parameters to substitute the text "Clear list" to something in
            // another language and to control the maximum number of files to be displayed.
            string persistenceDirectoryPath =
                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;

            var rfm = new RecentFilesMenu(persistenceDirectoryPath);

            new MainWindow(rfm).Show();
        }
    }
}
