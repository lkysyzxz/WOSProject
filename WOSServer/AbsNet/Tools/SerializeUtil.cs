using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AbsNet
{
    public class SerializeUtil
    {
        public static byte[] encode(object value)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, value);
            byte[] result = new byte[ms.Length];
            Buffer.BlockCopy(ms.GetBuffer(), 0, result, 0, (int)ms.Length);
            ms.Close();
            return result;
        }

        public static object decode(byte[] value)
        {
            MemoryStream ms = new MemoryStream(value);
            BinaryFormatter bf = new BinaryFormatter();
            object result = bf.Deserialize(ms);
            ms.Close();
            return result;
        }
    }
}
