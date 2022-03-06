using FGame.Grid;
using Legend.Core.Resources;
using Legend.Objects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend.Core.Units {
    public class LumberJack:Unit {

        public Terrain gatherTarget;

        public int currentLumberCount = 0;
        public int maxLumberCount = 6;
        public int lumberPerTurn = 2;

        public LumberJack(Texture2D texture, int width, int height):base(texture, width, height){


        }

        public void PerfromAction(Terrain terrain)
        {

            this.gatherTarget = terrain;
            performingAction = true;
            Debug.WriteLine("Gather");

        }

        public Boolean UpdateActions()
        {

            Boolean actionComplete = false;

            if (currentLumberCount < maxLumberCount)
            {
                currentLumberCount += lumberPerTurn;

            }
            else
            {

                actionComplete = true;
                performingAction = false;               
                Debug.WriteLine("Gathered max amount of lumber, time to head home");

            }

            return actionComplete;
                       

        }

    }

}
