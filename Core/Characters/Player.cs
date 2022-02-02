using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Objects {
    public class Player {

        private List<Unit> units = new List<Unit>();
        private Boolean isTurn = false;
        private Boolean isTurnComplete = false;
        private string username = "";

        public Player(string username)
        {

            this.username = username;

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
