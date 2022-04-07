using Legend.Core.Characters;
using System;
using System.Collections.Generic;

namespace Legend.Objects {
    public class Character {

        private List<Unit> units = new List<Unit>();
        private Boolean isTurn = false;
        private Boolean isTurnComplete = false;
        private string username = "";

        private int gold = 0;
        private int lumber = 0;
        private int ore = 0;

        public Character(string username, int gold, int ore, int lumber)
        {

            this.username = username;
            this.gold = gold;
            this.ore = ore;
            this.lumber = lumber;

        }

        public int Gold
        {

            set { this.gold = value; }
            get { return gold; }

        }
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

        public void AddUnitToUnitList(Unit unit)
        {

            units.Add(unit);
            unit.IsOwnedBy = this;

        }

        public string Username
        {

            get { return this.username; }
            set { this.username = value; }

        }

        public Boolean IsTurn
        {
            get { return this.isTurn; }
            set { this.isTurn = value; }
        }

        public Boolean IsTurnComplete
        {
            get { return this.isTurnComplete; }
            set { this.isTurnComplete = value; }
        }

        public List<Unit> Units
        {

            get { return this.units; }
            set { this.units = value; }

        }

    }

}
