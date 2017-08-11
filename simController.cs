﻿using System;
using System.Collections.Generic;
using System.Text;
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
        public static Random theRand = new Random(1);
        public distributions dist = new distributions(theRand);
        
        public simController(int n, double res)
        {
            ledger_obj gen = new ledger_obj(1);
            byte[] a = new byte[100];
            gen.gen_ledger(dist.getUniformBetween(1,100),a);
            
            double chosenStake, desire;
            sys_Time = DateTime.Now;
            peersResource = new Dictionary<int, peer_obj>();
            
            //create the peer objects
            n_peers = n;
            for (int i = 0; i < n; i++)
            {
                peer_obj p = new peer_obj(i);
                p.localDist = dist;
                p.ledger.gen_ledger(gen.ledger[0].key, gen.ledger[0].value);
                desire = p.gen_desiredStake(10);
                chosenStake = p.chooseStake();
                peersResource.Add(i, p);
                
                Console.WriteLine("created peer : " + i + " : " + desire + " : " + chosenStake + " " + peersResource.Count);
            }                                                            
        }

        //Main start to the simulation
        //controlled by number of transactions
        public void runSimulation(int transactions)
        {
            
            for (int i = 0; i < transactions; i++)
            {
                Console.WriteLine("starting transaction " + i);
                int leader = selectLeader();

                Console.WriteLine("You are the chosen one! " + leader);
                byte[] x = new byte[100];
                //generate random byte array
                x = dist.getByte(x);
                
                peersResource[leader].broadCastPeers(peersResource, i, x);
            }
            
        }

        //creates a weighted list and randomly selects and id from it
        public int selectLeader()
        {
            distributions d = new distributions(1);
            int theChosenOne;
            createWeightedList();

            theChosenOne = d.getUniformBetween(0, weightedPeer_IDs.Count);
            return weightedPeer_IDs[theChosenOne];
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
            //Parallel.ForEach(peersResource, (pair, state, arg3) =>

            foreach(KeyValuePair<int, peer_obj> pair in peersResource)
            {
                Console.WriteLine("counting parallel " + peersResource.Count);
                weightedPeer_IDs.AddRange(pair.Value.createListPortion());
                
            }
            //});
                        
        }

        public void report()
        {
            Console.WriteLine("----------------------------------");
            foreach (KeyValuePair<int, peer_obj> kvp in peersResource)
            {
                Console.WriteLine("peer " + kvp.Key + " ledger length : " + kvp.Value.ledger.ledger.Count +
                                  " latest state : " + Encoding.Default.GetString(kvp.Value.ledger_state()));
            }
            Console.WriteLine("----------------------------------");
        }

    }
}