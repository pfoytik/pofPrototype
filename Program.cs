using System;
using System.Collections.Generic;

namespace pofPrototype
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            for (int i = 0; i < 30; i++)
            {
                //create the simController telling it the number of peer nodes
                //and the total number of available system resources
                simController sc = new simController(5, 100);

                //run the simulation by telling it the number of transactions
                sc.runSimulation(100);
                sc.report();
            }
        }
    }
}