using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace FolkerKinzel.RecentFiles.WPF.Tests;

public class WpfTestMethodAttribute : TestMethodAttribute
{
    public override TestResult[] Execute(ITestMethod testMethod)
    {
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
        {
            return WpfTestMethodAttribute.Invoke(testMethod);
        }

        TestResult[]? result = null;
        var thread = new Thread(() => result = WpfTestMethodAttribute.Invoke(testMethod));
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        return result!;
    }

    private static TestResult[] Invoke(ITestMethod testMethod) => new[] { testMethod.Invoke(null) };
}
