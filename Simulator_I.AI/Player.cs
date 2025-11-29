using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator_I.AI
{
    public class Player
    {
        public int Energy;
        public int Mental;

        public Player()
        {
            Energy = 100;
            Mental = 100;
        }

        public void ChanceEnergy(int value)
        {
            Energy += value;
            if (Energy > 100) Energy = 100;
            if (Energy < 0) Energy = 0;
        }
        public void ChanceMental(int value)
        {
            Mental += value;
            if (Mental > 100) Mental = 100;
            if (Mental < 0) Mental = 0;

        }
    }
}
