using System;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public class NWrkRandomReader : INWrkReader
    {
        object _lockRead = new object();
        
        public int FieldCount { get; set; }
        public int FieldLength { get; set; }
        public int Count { get; set; }
        Random _rand = new Random((int)DateTime.Now.Ticks);
        StringBuilder _buffer = new StringBuilder();
        int _count;

        public void Dispose()
        {

        }

        public void ReadConfig(IConfigurationSection section)
        {
            var t = section["FieldCount"];
            if (!int.TryParse(t, out int fieldCount))
            {
                fieldCount = 1;
            }
            FieldCount = fieldCount;

            t = section["FieldLength"];
            if (!int.TryParse(t, out int fieldLength))
            {
                fieldLength = 3;
            }
            FieldLength = fieldLength;

            t = section["Count"];
            if (!int.TryParse(t, out int count))
            {
                count = 100;
            }
            Count = count;
        }

        public void Reset()
        {
            _count = 0;
        }

        public string[] ReadLine()
        {
            lock (_lockRead)
            {
                if (_count >= Count)
                    return null;

                var fields = new string[FieldCount];
                for (int i = 0; i < FieldCount; i++)
                {
                    fields[i] = RandomText();
                }
                _count++;
                return fields;
            }
        }

        string RandomText()
        {
            _buffer.Clear();
            for (int i = 0; i < FieldLength; i++)
            {
                _buffer.Append((char)('A' + _rand.Next(26)));
            }
            return _buffer.ToString();
        }
    }
}
