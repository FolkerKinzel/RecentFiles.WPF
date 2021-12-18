using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class ClearRecentFileTests
{
    [TestMethod]
    public void ClearRecentFileTest1()
    {
        var comm = new ClearRecentFiles(null!);
        Assert.IsNotNull(comm);
    }

    [TestMethod]
    public void CanExecuteTest1()
    {
        var comm = new ClearRecentFiles(null!);
        Assert.IsTrue(comm.CanExecute(null));
    }

    [TestMethod]
    public void ExecuteTest1()
    {
        bool executed = false;
        var comm = new ClearRecentFiles(() =>  executed = true);
        comm.Execute(null);
        Assert.IsTrue(executed);
    }

}
