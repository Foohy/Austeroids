using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austeroids
{
    class Program
    {
        static void Main(string[] args)
        {
            //audioGen.MakeSomeNoise();
            World world = new World();
            world.Create<Entities.Ship>();
            //world.Create<Entities.Ship>(new Vector(50, 50));

            for (int i = 0; i < 10; i++)
            {
                world.Create<Entities.Asteroid>();
            }

            world.StartGame();

            while (true)
            {
                world.Tick();
            }
        }
    }
}
