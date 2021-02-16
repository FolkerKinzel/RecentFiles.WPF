using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using FolkerKinzel.RecentFiles.WPF;

namespace WpfExample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            // Construct a new RecentFilesMenu which persist its data in the same
            // directory, where the program exe-file is:
            string persistenceDirectoryPath =
                Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!;
            var rfm = new RecentFilesMenu(persistenceDirectoryPath);

            new MainWindow(rfm).Show();
        }
    }
}
