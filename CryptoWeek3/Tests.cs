using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using XMLib;
using XMLib.Log;

using NUnit.Framework;

namespace CryptoWeek3
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Initialization()
        {
            Logger.InitializeConsoleLogger();
        }

        [Test]
        public void ProgrammingAssignmentTest()
        {
            var samplesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sample");
            var files = Directory.GetFiles(samplesDir,"*.*");

            Logger.DebugFormat("Files({0}):\r\n\t{1}\r\n",files.Length,String.Join("\r\n\t",files));
            foreach (var file in files)
            {
                Logger.DebugFormat("Processing file {0}",file);
                var fileHash = StringUtils.FromHex(Path.GetFileNameWithoutExtension(file));
                Logger.DebugFormat("ExpectHash - {0}", StringUtils.ToHex(fileHash));
                var hasher = new StreamHasher();

                var hasherHash = hasher.ComputeHash(File.OpenRead(file));
                Logger.DebugFormat("ResultHash - {0}", StringUtils.ToHex(hasherHash));

                Assert.AreEqual(fileHash,hasherHash);
            }
        }
    }
}
