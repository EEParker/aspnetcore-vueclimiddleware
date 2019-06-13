using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VueCliMiddleware.Tests
{
    [TestClass]
    public class PidUtilsTests 
    {
        [TestMethod]
        public void KillPort_8080_KillsVueServe()
        {
            bool success = PidUtils.KillPort(8080, true, true);
            Assert.IsTrue(success);
        }
    }
}
