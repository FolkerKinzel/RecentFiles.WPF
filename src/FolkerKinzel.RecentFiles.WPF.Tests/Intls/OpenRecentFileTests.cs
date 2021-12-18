using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class OpenRecentFileTests
{
    [TestMethod]
    public void OpenRecentFileTest1()
    {
        var comm = new OpenRecentFile(null!);
        Assert.IsNotNull(comm);
    }

    [TestMethod]
    public void CanExecuteTest1()
    {
        var comm = new OpenRecentFile(null!);
        Assert.IsTrue(comm.CanExecute(null));
    }

    [TestMethod]
    public void ExecuteTest1()
    {
        bool executed = false;
        int i = 0;
        var comm = new OpenRecentFile((o) => { i = (int)o!; executed = true; });
        comm.Execute(42);
        Assert.IsTrue(executed);
        Assert.AreEqual(42, i);
    }

}
