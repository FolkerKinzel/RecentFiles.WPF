using FolkerKinzel.RecentFiles.WPF;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Construct a new RecentFilesMenu, that persist its data in the same directory,
            // where the program exe-file is:
            var rfm = new RecentFilesMenu(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)!);

            new MainWindow(rfm).Show();
        }
    }
}
