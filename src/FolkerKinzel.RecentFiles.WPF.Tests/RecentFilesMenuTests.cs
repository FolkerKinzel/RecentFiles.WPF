using FolkerKinzel.RecentFiles.WPF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolkerKinzel.RecentFiles.WPF.Tests;

[TestClass()]
public class RecentFilesMenuTests
{
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
    public async Task AddRecentFileAsyncTest()
    {
        using var menu = new RecentFilesMenu(Environment.CurrentDirectory);

        string path = "test";

        Assert.IsFalse(Path.IsPathRooted(path));

        await menu.AddRecentFileAsync(path);

        string? mostRecent = await menu.GetMostRecentFileAsync();
        Assert.IsNotNull(mostRecent);
        Assert.IsTrue(Path.IsPathRooted(mostRecent));
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
