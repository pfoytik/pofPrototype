using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace pofPrototype
{
    public class ledger_obj
    {
        public int ID { get; set; }
        public List<block_obj> ledger { get; set; }
        public SHA256 mySHA256 { get; set; }
        
        public ledger_obj(int id)
        {
            ID = id;
            ledger = new List<block_obj>();
            mySHA256 = SHA256Managed.Create();
        }

        public void dup_ledger(List<block_obj> l)
        {
            ledger = l;
        }

        public void gen_ledger(int key, byte[] val)
        {
            byte[] hash;
            block_obj genesis;
            
            hash = mySHA256.ComputeHash(val);
            genesis = new block_obj(hash, key, val);
            ledger.Add(genesis);
        }

        public void add_block(int key, byte[] val)
        {
            byte[] hash, current;
            block_obj newBlock;

            current = new byte[val.Length + ledger.Last().hash.Length];
            hash = mySHA256.ComputeHash(current); 
            newBlock = new block_obj(hash, key, val);
            ledger.Add(newBlock);
        }

        public byte[] get_latestState()
        {
            return ledger.Last().hash;
        }
    }
}