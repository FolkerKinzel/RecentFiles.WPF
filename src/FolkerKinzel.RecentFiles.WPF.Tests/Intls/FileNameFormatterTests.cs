using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class FileNameFormatterTests
{
    [TestMethod]
    public void GetMenuItemHeaderFromFilenameTest1()
    {
        const string input = "C:\\one\\two\\three\\four\\five\\six\\seven\\eight\\nine\\ten\\eleven\\twelve\\thirteen\\fourteen\\fifteen\\sixteen";

        string output = FileNameFormatter.GetMenuItemHeaderFromFilename(input, 0);

        Assert.IsTrue(output.StartsWith("_1: C:\\one"));
        Assert.IsTrue(output.EndsWith("fifteen\\sixteen"));
        StringAssert.Contains(output, FileNameFormatter.GetReplacement());
    }

    [TestMethod]
    public void GetMenuItemHeaderFromFilenameTest2()
    {
        string output = FileNameFormatter.GetMenuItemHeaderFromFilename("C:\\test.txt", 9);
        Assert.AreEqual(output, "1_0: C:\\test.txt", false);
    }
}
