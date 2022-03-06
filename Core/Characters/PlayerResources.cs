using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Characters {
    public class PlayerResources {

        private int lumber = 100;
        private int ore = 200;      

        public int Ore
        {

            set { this.ore = value; }
            get { return ore; }

        }

        public int Lumber
        {

            set { this.lumber = value; }
            get { return lumber; }

        }

    }

}
