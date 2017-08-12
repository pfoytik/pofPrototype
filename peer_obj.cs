using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
        
        //stats
        public double avgStake, avgLead, totLead, totalCheekLead, totalStake;
        
        public peer_obj(int i)
        {
            ID = i;
            leader_stat = false;
            current_stake = 0;
            desired_stake = 0;
            ledger = new ledger_obj(i);
            totLead = 0;
            totalCheekLead = 0;
            totalStake = 0;

            //for debuging include int seed
            //localDist = new distributions(1);
        }

        public double getAvgStake(int t)
        {
            avgStake = totalStake / t;
            return avgStake;
        }

        public double getAvgLead(int t)
        {
            avgLead = totLead / t;
            return avgLead;
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
            //artificial delay
            //int mili = localDist.getUniformBetween(10, 20);
            //Thread.Sleep(mili);
            
            //current_stake = desired_stake * localDist.getNormal(desired_stake, 1);
            current_stake = Convert.ToDouble(localDist.getNormal(desired_stake, 10));
            if (current_stake > 100)
                current_stake = 100;
            else
            {
                if (current_stake < 0)
                    current_stake = 0;                                    
            }

            totalStake += current_stake;             
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
            //int mili = localDist.getUniformBetween(1, 5);
            //Thread.Sleep(mili);
            
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
            bool cheekflag = false;
            foreach (KeyValuePair<int, peer_obj> kvp in ctrl)
            {
                if (kvp.Value.current_stake > current_stake)
                    cheekflag = true;
                if (kvp.Value.recieveLeaderRequest(ID, current_stake, weightedList))
                    tally++;
            }

            if (tally > (peerStakes.Count / 2))
            {
                totLead++;
                
                if (cheekflag)
                    totalCheekLead++;
                
                return true;
            }
            
            return false;
        }
        
    }
}