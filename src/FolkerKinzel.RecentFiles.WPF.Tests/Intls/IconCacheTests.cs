using System.Windows.Media;
using FolkerKinzel.RecentFiles.WPF.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class IconCacheTests
{
    [TestMethod]
    public void IconCacheTest1()
    {
        var cache = new IconCache();
        Assert.IsNotNull(cache);
    }

    [TestMethod]
    public void GetIconTest1()
    {
        var cache = new IconCache();

        ImageSource img = cache.GetIcon("C:\\");
        
        Assert.IsNotNull(img);
        Assert.IsTrue(img.Width > 15);
        Assert.IsTrue(img.Height > 15);
        Assert.IsTrue(img.Width < 17);
        Assert.IsTrue(img.Height < 17);

    }

    [TestMethod]
    public void GetIconTest2()
    {
        var cache = new IconCache();

        ImageSource img = cache.GetIcon(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));

        Assert.IsNotNull(img);
        Assert.IsTrue(img.Width > 15);
        Assert.IsTrue(img.Height > 15);
        Assert.IsTrue(img.Width < 17);
        Assert.IsTrue(img.Height < 17);

    }

    [TestMethod]
    public void GetIconTest3()
    {
        var cache = new IconCache();

        ImageSource img = cache.GetIcon(TestFiles.TestTxt);

        Assert.IsNotNull(img);
        Assert.IsTrue(img.Width > 15);
        Assert.IsTrue(img.Height > 15);
        Assert.IsTrue(img.Width < 17);
        Assert.IsTrue(img.Height < 17);

        ImageSource img2 = cache.GetIcon(TestFiles.Test2Txt);

        Assert.AreSame(img, img2);

    }

    [TestMethod]
    public void GetIconTest4()
    {
        var cache = new IconCache();

        ImageSource img = cache.GetIcon("C:\\nixda.abc");

        Assert.IsNotNull(img);
        Assert.IsTrue(img.Width > 15);
        Assert.IsTrue(img.Height > 15);
        Assert.IsTrue(img.Width < 17);
        Assert.IsTrue(img.Height < 17);
    }

    [TestMethod]
    public void GetIconTest5()
    {
        var cache = new IconCache();

        string path = TestFiles.UnregisteredFileTypeExtension;

        ImageSource img = cache.GetIcon(path);

        Assert.IsNotNull(img);
        Assert.IsTrue(img.Width > 15);
        Assert.IsTrue(img.Height > 15);
        Assert.IsTrue(img.Width < 17);
        Assert.IsTrue(img.Height < 17);

        ImageSource img2 = cache.GetIcon(path);

        Assert.AreSame(img, img2);

    }

    [TestMethod]
    public void GetIconTest6()
    {
        var cache = new IconCache();

        string path = "C:\\NoExtension";

        ImageSource img = cache.GetIcon(path);

        Assert.IsNotNull(img);
        Assert.IsTrue(img.Width > 15);
        Assert.IsTrue(img.Height > 15);
        Assert.IsTrue(img.Width < 17);
        Assert.IsTrue(img.Height < 17);

    }
}
