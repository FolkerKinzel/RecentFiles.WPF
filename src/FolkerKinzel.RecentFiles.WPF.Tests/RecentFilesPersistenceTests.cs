using FolkerKinzel.RecentFiles.WPF.Intls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Tests;

[TestClass]
public class RecentFilesPersistenceTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void RecentFilesPersistenceTest1()
    {
        using var per = new RecentFilesPersistence("IAmRelative");
    }

    [TestMethod]
    public async Task LoadAsyncTest1()
    {
        using var per = new RecentFilesPersistence(Environment.CurrentDirectory);
        per.Dispose();
        await per.LoadAsync();
    }

    [TestMethod]
    public async Task SaveAsyncTest1()
    {
        using var per = new RecentFilesPersistence(Environment.CurrentDirectory);
        per.Dispose();
        await per.SaveAsync();
    }
}
