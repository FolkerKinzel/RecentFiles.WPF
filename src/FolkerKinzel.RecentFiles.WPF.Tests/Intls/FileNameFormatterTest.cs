using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FolkerKinzel.RecentFiles.WPF.Intls.Tests;

[TestClass]
public class FileNameFormatterTest
{
    [TestMethod]
    public void GetMenuItemHeaderFromFilenameTest1()
    {
        const string input = "C:\\one\\two\\three\\four\\five\\six\\seven\\eight\\nine\\ten\\eleven\\twelve\\thirteen\\fourteen\\fifteen\\sixteen";

        string output = FileNameFormatter.GetMenuItemHeaderFromFilename(input, 0);

        Assert.IsTrue(output.StartsWith("_1: C:\\one"));
        Assert.IsTrue(output.EndsWith("fifteen\\sixteen"));
    }
}
