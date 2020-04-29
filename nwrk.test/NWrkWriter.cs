using System;
using NUnit.Framework;
using System.IO;

namespace nwrk.test
{
    public class NWrkWriter
    {
        readonly string _path = "/Users/michael/Projects/netcore/nwrk/testdata/result.txt";
        readonly string _source = "/Users/michael/Projects/netcore/nwrk/testdata/source.txt";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Write()
        {
            using var reader = new app.NWrkCsvReader(_source);
            using var writer = new app.NWrkCsvWriter(_path);
            while (reader.Read())
            {
                writer.WriteLine(reader.Record);
            }
            writer.Dispose();
            Assert.AreEqual(File.ReadAllLines(_source).Length, File.ReadAllLines(_path).Length);
        }
    }
}
