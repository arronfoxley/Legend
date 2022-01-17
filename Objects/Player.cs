using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Objects {
    public class Player {

        public List<Unit> units = new List<Unit>();
        public Boolean isTurn = false;
        public Boolean isTurnComplete = false;

        public Player()
        {


        }

        public void AddUnitToUnitList(Unit unit)
        {

            units.Add(unit);
            unit.IsOwnedBy = this;

        }

    }

}
