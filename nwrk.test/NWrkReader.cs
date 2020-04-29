using NUnit.Framework;
using System.IO;

namespace nwrk.test
{
    public class NWrkReader
    {
        readonly string _path = "/Users/michael/Projects/netcore/nwrk/testdata/source.txt";
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Read()
        {
            using app.NWrkCsvReader reader = new app.NWrkCsvReader(_path);
            var line = 0;
            while (reader.Read())
            {
                var first = reader[0];
                var record = reader.Record;
                line++;
            }
            Assert.AreEqual(File.ReadAllLines(_path).Length, line);
        }

        public void generate()
        {
            var file = new StreamWriter(_path, false, System.Text.Encoding.UTF8);
            for (int i = 0; i < 100000; i++)
            {
                file.WriteLine($"id-{i + 1},name-{i + 1}");
            }
            file.Close();
        }
    }
}