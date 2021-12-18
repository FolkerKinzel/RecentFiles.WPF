using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Tests;

[TestClass]
public class RecentFileSelectedEventArgsTests
{
    [TestMethod]
    public void RecentFilesSelectedEventArgsTest1()
    {
        const string fileName = "File Name";
        var args = new RecentFileSelectedEventArgs(fileName);
        Assert.AreEqual(fileName, args.FileName);
    }
}
