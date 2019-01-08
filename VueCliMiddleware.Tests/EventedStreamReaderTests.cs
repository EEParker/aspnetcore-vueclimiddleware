using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VueCliMiddleware.Tests
{
    [TestClass]
    public class EventedStreamReaderTests
    {
        [TestMethod]
        public async Task ReadChunks_MultipleNewLines_OnCompleteLineFiresEachNewline()
        {

            string testMessage = "this \nis \na \ntest \nof \nmultiple \nnewlines\n  trailing data";
            int numNewLines = testMessage.Split('\n').Length - 1;
            int chunksReceived = 0;
            int linesReceived = 0;

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(testMessage)))
            {
                var streamReader = new StreamReader(ms);

                var esr = new EventedStreamReader(streamReader);
                esr.OnReceivedChunk += (e) => Interlocked.Increment(ref chunksReceived);
                esr.OnReceivedLine += (e) => Interlocked.Increment(ref linesReceived);
                await Task.Delay(200);
            }

            Assert.AreEqual(numNewLines, linesReceived);
        }
    }
}
