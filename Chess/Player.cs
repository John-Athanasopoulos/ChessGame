using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Player
    {
        public Player(int stSec,string n)
        {
            currentSeconds = stSec;
            name = n;
        }
        public int currentSeconds;
        public bool enable = false;
        public string name;
       
		public void calculateRemainingSeconds()
        {
            if (enable)
            {
                currentSeconds--;
            }
        }
    }
}
