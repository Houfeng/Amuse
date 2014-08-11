using Amuse.Demo.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Amuse.Demo.Test
{
    [TestClass]
    public class AmuseTest
    {
        [TestMethod]
        public void Test()
        {
            Container container = Container.Create();
            var a = container.Get<IA>("a");
            Assert.IsNotNull(a, "查找对象");
            Assert.IsNotNull(a.B, "检查通过属性注入的 Bean 对象");
            Assert.AreEqual<int>(a.Value1, 1, "检查属性注入的值一");
            Assert.AreEqual<int>(a.Value2, 2, "检查属性注入的值二");
        }
    }
}
