using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class UtilityTests
{
    [TestMethod]
    public void IsPathDirectoryTest1()
    {
        Assert.IsFalse(Utility.IsPathDirectory("\""));
    }

    [TestMethod]
    public void IsPathDirectoryTest2()
    {
        Assert.IsTrue(Utility.IsPathDirectory(Environment.CurrentDirectory));
    }

    [TestMethod]
    public void IsPathDriveTest2()
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        Assert.IsTrue(Utility.IsPathDirectory(path));
        Assert.IsFalse(Utility.IsPathDrive(path));
    }

    [TestMethod]
    public void IsPathDriveTest3()
    {
        const string path = "C:\\";
        Assert.IsTrue(Utility.IsPathDirectory(path));
        Assert.IsTrue(Utility.IsPathDrive(path));
    }

}
