using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Grid;
using Legend.Core.Units;
using Legend.Objects;
using Microsoft.Xna.Framework;
using PathFinding.PathFinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Legend.Core.Management {
    public class UnitManager {

        private static List<Unit> units;
        private static Unit activeUnit = null;

        private static Grid grid;

        public static Grid Grid
        {

            set { grid = value; }
            get { return grid; }

        }

        public static List<Unit> Units
        {

            set { units = value; }
            get { return units; }

        }

        public static void RemoveUnits()
        {

            units.Clear();

        }

        public static Unit ActiveUnit
        {

            get { return activeUnit; }
            set { activeUnit = value; }

        }

        public static void ResetAllCharacterUnits(List<Character> players)
        {

            for (int i = 0; i < players.Count; i++)
            {

                for (int o = 0; o < players[i].Units.Count; o++)
                {

                    players[i].Units[o].UpdateAction();
                    players[i].Units[o].TurnActionReset();

                }

            }

        }

        public static Unit GetUnitFromGameSprite(GameSprite gameSprite)
        {

            Unit unit = null;

            for(int i = 0; i < units.Count; i++)
            {

                if (units[i].GameSprite == gameSprite)
                {

                    unit = units[i];
                    break;

                }

            }

            return unit;

        }

        public static void Update(GameTime gameTime)
        {

            foreach (Unit unit in units) {

                unit.Update(gameTime);

            }

        }

    }

}
