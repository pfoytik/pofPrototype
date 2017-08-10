using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace pofPrototype
{
    public class peer_obj
    {
        public int ID { get; set; }
        public bool leader_stat { get; set; }
        public double desired_stake { get; set; }
        public double current_stake { get; set; }
        public ledger_obj ledger;
        public distributions localDist;
        public List<int> sublist;

        public peer_obj(int i)
        {
            ID = i;
            leader_stat = false;
            current_stake = 0;
            desired_stake = 0;
            ledger = new ledger_obj(i);
            
            //for debuging include int seed
            localDist = new distributions(1);
        }

        public void set_desiredStake(double desired)
        {
            desired_stake = desired;
        }

        public double gen_desiredStake(int min)
        {
            desired_stake = System.Convert.ToDouble(localDist.getUniformBetween(min, 100));
            return desired_stake;
        }

        public byte[] ledger_state()
        {
            return ledger.get_latestState();
        }

        public double chooseStake()
        {
            current_stake = desired_stake * localDist.getNormal(desired_stake, 1);
            return current_stake;
        }

        public void createListPortion()
        {
            List<int> sublist = new List<int>();
            for(int i=0; i<current_stake; i++)
            {
                sublist.Add(ID);
            }            
        }
        
        public void recieveBroadcast(int key, byte[] val)
        {
            ledger.add_block(key, val);
        }
            
        public void broadCastPeers(Dictionary<int, peer_obj> ctrl, int key, byte[] val)
        {            
            //assuming the info is good to submit, probably would need to be checked though...
            Parallel.ForEach(ctrl, (pair, state, arg3) =>
            {
                pair.Value.recieveBroadcast(key, val);  
            });
        }
        
    }
}