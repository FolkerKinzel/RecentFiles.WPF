using FolkerKinzel.RecentFiles.WPF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            new MainWindow(new RecentFilesMenu(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName)!)).Show();
        }
    }
}
