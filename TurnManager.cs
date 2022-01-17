using Legend.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend {
    public class TurnManager {

        public List<Unit> unitsAwaitingTurn = new List<Unit>();

        public bool PlayerTurn = true;
        public bool EnemyTurn = false;

        public Unit activeUnit = null;

        public int TurnNumber = 1;

        public TurnManager()
        {

        }

        public void AddUnitToTurnList(Unit unit)
        {

            unitsAwaitingTurn.Add(unit);

        }

        public Unit ActiveUnit{

            get { return this.activeUnit;  }
            set { this.activeUnit = value; }

        }


        public void Update()
        {

            if (CheckTurnOver())
            {

                ResetTurns();
                activeUnit = CheckForPlayerUnit();

            }

            if (activeUnit.TurnStarted && !activeUnit.TurnComplete && activeUnit.step == activeUnit.totalSteps)
            {
               
                activeUnit.TurnComplete = true;
                Debug.WriteLine("Turn over");
                //check if player unit turn list complete
                if(CheckForPlayerUnit() != null)
                {

                    activeUnit = CheckForPlayerUnit();

                }
                else
                {

                    Debug.WriteLine("Enemy Turn");
                    activeUnit = CheckForEnemyUnit();
                    Debug.WriteLine(activeUnit);

                }

            }

        }

        private void ResetTurns()
        {
            
            foreach (Unit unit in unitsAwaitingTurn)
            {

                unit.TurnComplete = false;
                unit.TurnStarted = false;

            }

        }

        private bool CheckTurnOver()
        {
            Boolean turnOver = true;

            for (int i = 0; i < unitsAwaitingTurn.Count; i++)
            {

                if ( !unitsAwaitingTurn[i].TurnComplete)
                {

                    turnOver = false;

                }

            }

            return turnOver;
        }

        private Unit CheckForPlayerUnit()
        {

            Unit unit = null;

            for (int i = 0; i < unitsAwaitingTurn.Count;i++)
            {

                if(unitsAwaitingTurn[i] is Player && !unitsAwaitingTurn[i].TurnComplete)
                {

                    unit = unitsAwaitingTurn[i];                  

                }

            }

            return unit;

        }

        private Unit CheckForEnemyUnit()
        {

            Unit unit = null;

            for (int i = 0; i < unitsAwaitingTurn.Count; i++)
            {

                if (unitsAwaitingTurn[i] is Enemy && !unitsAwaitingTurn[i].TurnComplete)
                {

                    unit = unitsAwaitingTurn[i];

                }

            }

            return unit;

        }

    }

}
