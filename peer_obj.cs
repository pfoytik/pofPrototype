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
        public Dictionary<int, double> peerStakes;

        public peer_obj(int i)
        {
            ID = i;
            leader_stat = false;
            current_stake = 0;
            desired_stake = 0;
            ledger = new ledger_obj(i);
            
            //for debuging include int seed
            //localDist = new distributions(1);
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
            //current_stake = desired_stake * localDist.getNormal(desired_stake, 1);
            current_stake = Convert.ToDouble(localDist.getUniformBetween(Convert.ToInt16(desired_stake), 100));
            return current_stake;
        }

        public List<int> createListPortion()
        {
            List<int> sublist = new List<int>();
            for(int i=0; i<current_stake; i++)
            {
                sublist.Add(ID);
            }
            
            //Console.WriteLine("total sublist for " + ID + " " + sublist.Count);
            return sublist;
        }
        
        public void recieveBroadcast(int key, byte[] val)
        {
            ledger.add_block(key, val);
        }

        public bool recieveLeaderRequest(int key, double stake, List<int> weightedList)
        {
            if (peerStakes.ContainsKey(key))
            {
                if (peerStakes[key] == stake)
                {
                    if(key == weightedList[localDist.getUniformBetween(0, weightedList.Count)])
                        return true;
                }
                else
                    return false;
            }
            else
            {
                return false;
            }

            return false;
        }

        public void recieveStakes(Dictionary<int, double> steakDinner)
        {
            peerStakes = steakDinner;
        }
            
        public void broadCastPeers(Dictionary<int, peer_obj> ctrl, int key, byte[] val)
        {
            recieveBroadcast(key, val);
            //assuming the info is good to submit, probably would need to be checked though...
            Parallel.ForEach(ctrl, (pair, state, arg3) =>
            {
                if(pair.Key!=ID)
                    pair.Value.recieveBroadcast(key, val);  
            });
        }

        public bool leaderRequest(Dictionary<int, peer_obj> ctrl, List<int> weightedList)
        {
            int tally = 0;
            foreach (KeyValuePair<int, peer_obj> kvp in ctrl)
            {
                if (kvp.Value.recieveLeaderRequest(ID, current_stake, weightedList))
                    tally++;
            }

            if (tally > (peerStakes.Count / 2))
            {
                return true;
            }
            
            return false;
        }
        
    }
}