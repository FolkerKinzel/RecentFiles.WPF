using FolkerKinzel.RecentFiles.WPF.Intls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
