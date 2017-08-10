using System.Security.Cryptography;
using System.Xml.Schema;

namespace pofPrototype
{
    public class block_obj
    {
        public byte[] hash { get; set;}
        public int key { get; set; }
        public byte[] value { get; set; }

        public block_obj(byte[] h, int k, byte[] v)
        {
            hash = h;
            key = k;
            value = v;
        }
    }
}