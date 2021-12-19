using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class RecentFilesPersistenceTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RecentFilesPersistenceTest1()
    {
        using var persistence = new RecentFilesPersistence(null!);
    }



    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RecentFilesPersistenceTest2()
    {
        using var persistence = new RecentFilesPersistence("test.txt");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RecentFilesPersistenceTest3()
    {
        string path = Path.GetTempFileName();
        try
        {
            using var persistence = new RecentFilesPersistence(path);
        }
        catch (ArgumentException)
        {
            File.Delete(path);
            throw;
        }
    }

    [TestMethod]
    public void RecentFilesPersistenceTest4()
    {
        using var persistence = new RecentFilesPersistence(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RecentFilesPersistenceTest5()
    {
        using var persistence = new RecentFilesPersistence("C:\\te\"st.txt");
    }
}
