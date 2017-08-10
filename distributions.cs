using System;

namespace pofPrototype
{
    public class distributions
    {
        private Random rand;

        public distributions()
        {
            rand = new Random();
        }

        public distributions(int seed)
        {
            rand = new Random(seed);
        }

        public double getNormal(double avg, double stdDev)
        {
            double u1 = 1.0 - rand.NextDouble();
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
            double randNormal = avg + stdDev * randStdNormal;

            return randNormal;
        }

        public double getUniform()
        {
            return rand.NextDouble();
        }

        public int getUniformBetween(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}