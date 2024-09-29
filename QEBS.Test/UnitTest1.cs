using System.Runtime.InteropServices;

namespace QEBS.Test;

[TestClass]
public class BaseTest
{
    [TestMethod]
    public void TestMethod1()
    {
        Object obj = new object();
        GCHandle handle1 = GCHandle.Alloc(obj);
        IntPtr pointer = (IntPtr) handle1;

    }
}