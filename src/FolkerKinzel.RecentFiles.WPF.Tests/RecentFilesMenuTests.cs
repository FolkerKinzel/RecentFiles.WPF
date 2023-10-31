using FolkerKinzel.RecentFiles.WPF;
using FolkerKinzel.RecentFiles.WPF.Intls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FolkerKinzel.RecentFiles.WPF.Tests;

[TestClass]
public class IconCacheTests
{
    [TestMethod]
    public void GetIconTest()
    {
        var cache = new IconCache();
        Assert.IsNotNull(cache.GetIcon(new string(Path.GetInvalidPathChars())));
    }
}

[TestClass]
public class RecentFilesMenuTests
{
    private readonly string _fileName = Path.Combine(Environment.CurrentDirectory, $"{Environment.MachineName}.{Environment.UserName}.RF.txt");

    [TestMethod()]
    public void RecentFilesMenuTest1()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory, 1);
        Assert.IsNotNull(menu);
    }

    [DataTestMethod()]
    [DataRow(0)]
    [DataRow(11)]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void RecentFilesMenuTest2(int maxFiles)
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory, maxFiles);
        Assert.IsNotNull(menu);
    }

    [TestMethod()]
    public void RecentFilesMenuTest3()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory, 7, "foo");
        Assert.IsNotNull(menu);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentNullException))]
    public void InitializeTest1()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);
        menu.Initialize(null!);
    }

    [WpfTestMethod()]
    public void InitializeTest2()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);

        menu.Initialize(new System.Windows.Controls.MenuItem());
    }

    [TestMethod()]
    public async Task AddRecentFileAsyncTest1()
    {
        if (File.Exists(_fileName))
        {
            File.Delete(_fileName);
        }
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);

        string path = "test";

        Assert.IsFalse(Path.IsPathRooted(path));

        await menu.AddRecentFileAsync(path);

        string? mostRecent = await menu.GetMostRecentFileAsync();
        Assert.IsNotNull(mostRecent);
        Assert.IsTrue(Path.IsPathRooted(mostRecent));
    }

    [TestMethod()]
    public async Task AddRecentFileAsyncTest2()
    {
        if (File.Exists(_fileName))
        {
            File.Delete(_fileName);
        }

        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);

        await menu.AddRecentFileAsync(new string(Path.GetInvalidPathChars()));

        string? mostRecent = await menu.GetMostRecentFileAsync();
        Assert.IsNull(mostRecent);
    }

    [TestMethod()]
    public async Task AddRecentFileAsyncTest3()
    {
        if (File.Exists(_fileName))
        {
            File.Delete(_fileName);
        }

        using var menu = new RecentFilesMenu(Environment.CurrentDirectory, 2);
        string one = Path.Combine(Environment.CurrentDirectory, "one");
        string two = Path.Combine(Environment.CurrentDirectory, "two");
        string three = Path.Combine(Environment.CurrentDirectory, "three");

        await menu.AddRecentFileAsync(one);
        await menu.AddRecentFileAsync(two);
        await menu.AddRecentFileAsync(three);

        string? mostRecent = await menu.GetMostRecentFileAsync();
        Assert.AreEqual(three, mostRecent);
        await menu.RemoveRecentFileAsync(three);
        mostRecent = await menu.GetMostRecentFileAsync();
        Assert.AreEqual(two, mostRecent);
        await menu.RemoveRecentFileAsync(two);
        mostRecent = await menu.GetMostRecentFileAsync();
        Assert.IsNull(mostRecent);
        await menu.RemoveRecentFileAsync(one);
    }

    [TestMethod()]
    public async Task RemoveRecentFileAsyncTest()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);

        string path = "remove test";

        Assert.IsFalse(Path.IsPathRooted(path));

        await menu.AddRecentFileAsync(path);

        string? mostRecent = await menu.GetMostRecentFileAsync();
        Assert.IsNotNull(mostRecent);
        Assert.IsTrue(Path.IsPathRooted(mostRecent));

        await menu.RemoveRecentFileAsync(mostRecent);
        string? menuContent = await menu.GetMostRecentFileAsync();
        Assert.AreNotEqual(mostRecent, menuContent);
    }

    [TestMethod()]
    public async Task GetMostRecentFileAsyncTest()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);

        string path = "test";

        Assert.IsFalse(Path.IsPathRooted(path));

        await menu.AddRecentFileAsync(path);

        string? mostRecent = await menu.GetMostRecentFileAsync();
        Assert.IsNotNull(mostRecent);
        Assert.IsTrue(Path.IsPathRooted(mostRecent));

        string? fileName = Path.GetFileName(mostRecent);
        Assert.AreEqual(path, fileName);
    }


    

}
