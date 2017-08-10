using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace pofPrototype
{
    public class simController
    {
        public double totalSystemResource { get; set; }
        public int n_peers { get; set; }
        public Dictionary<int, peer_obj> peersResource;
        public DateTime sys_Time { get; set; }
        public List<int> weightedPeer_IDs;

        public simController(int n, double res)
        {
            double chosenStake, desire;
            sys_Time = DateTime.Now;            
            
            //create the peer objects
            n_peers = n;
            for (int i = 0; i < n; i++)
            {
                peer_obj p = new peer_obj(i);
                desire = p.gen_desiredStake(10);
                chosenStake = p.chooseStake();
                peersResource.Add(i, p);
                
                Console.WriteLine("created peer : " + i + " : " + desire + " : " + chosenStake);
            }                                                            
        }

        //Main start to the simulation
        //controlled by number of transactions
        public void runSimulation(int transactions)
        {
            distributions dist = new distributions(1);
            for (int i = 0; i < transactions; i++)
            {
                int leader = selectLeader();                
                
                byte[] x = new byte
                //generate random byte array
                for (int i = 0; i < 100; i++)
                {
                    
                }
                
                peersResource[leader].broadCastPeers(peersResource, i, );
            }
            
        }

        //creates a weighted list and randomly selects and id from it
        public int selectLeader()
        {
            distributions d = new distributions(1);
            int theChosenOne;
            createWeightedList();

            theChosenOne = d.getUniformBetween(0, weightedPeer_IDs.Count);
            return theChosenOne;
        }

        //creates a list of ids based on their chosen weight
        //duplicate id's exist in the weighted list, but to grab
        //a weighted selection you just need to grab a random value
        //in the list
        public void createWeightedList()
        {
            weightedPeer_IDs = new List<int>();
            
            //uses a parallel function to efficiently let the peer objects
            //construct their sublists which is then combined
            Parallel.ForEach(peersResource, (pair, state, arg3) =>
            {
                pair.Value.createListPortion();  
            });
            
            foreach (KeyValuePair<int, peer_obj> kvp in peersResource)
            {               
                weightedPeer_IDs.AddRange(kvp.Value.sublist);
            }            
        }

    }
}