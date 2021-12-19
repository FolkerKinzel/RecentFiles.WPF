using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolkerKinzel.RecentFiles.WPF.Tests
{
    internal static class TestFiles
    {
        private const string TEST_FILE_DIRECTORY_NAME = "Resources";

        public static string ProjectDirectory { get; }

        private static readonly string _testFileDirectory;

        static TestFiles()
        {
            ProjectDirectory = Resources.Res.ProjDir.Trim();
            _testFileDirectory = Path.Combine(ProjectDirectory, TEST_FILE_DIRECTORY_NAME);
        }


        internal static string TestTxt => Path.Combine(_testFileDirectory, "Test.txt");

        public static string Test2Txt => Path.Combine(_testFileDirectory, "Test2.txt");

        public static string UnregisteredFileTypeExtension => Path.Combine(_testFileDirectory, "Test.uvwxyz");

    }
}
