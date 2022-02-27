using FGame.Core;
using Legend.Core.Units;
using Legend.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend.Core.Management {
    public class UnitManager {

        private List<Unit> units;
        private List<Unit> automatedUnits = new List<Unit>();
        private Unit activeUnit = null;

        private Boolean automationMode = false;
        private int autoUnitNumber = 0;

        public UnitManager()
        {


        }

        public void AddUnits(List<Unit> units)
        {

            this.units = units;

        }

        public void RemoveUnits()
        {

            this.units.Clear();

        }

        public Boolean AutomationMode
        {

            set { this.automationMode = value; }
            get { return this.automationMode; }

        }

        public Unit ActiveUnit
        {

            get { return this.activeUnit; }
            set { this.activeUnit = value; }

        }

        public void Reset()
        {

            activeUnit = null;
            autoUnitNumber = 0;

        }

        public bool UnitOfActivePlayer(Unit unit)
        {

            if (units.Contains(unit))
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        public Boolean CheckForAutomatedUnitTurns()
        {

            Boolean automated = false;

            for (int i = 0; i < units.Count; i++)
            {

                if (units[i].Path != null && units[i].Path.Count > 0)
                {

                    automatedUnits.Add(units[i]);
                    automated = true;

                }

            }

            if (automated)
            {

                automationMode = true;
                Debug.WriteLine("Turn part automated");

            }
            else
            {

                automationMode = false;
                Debug.WriteLine("Turn not part automated");

            }

            return automated;

        }

        public void UpdatePlayerUnitsForNextTurn(List<Player> players)
        {

            for (int i = 0; i < players.Count; i++)
            {

                for (int o = 0; o < players[i].Units.Count; o++)
                {
                  
                    if (players[i].Units[o] is Pioneer)
                    {

                        Pioneer pioneer = (Pioneer)players[i].Units[o];
                        pioneer.UpdateBuildList();

                    }

                    players[i].Units[o].TurnActionReset();

                }

            }

        }

        public void Update(GameTime gameTime)
        {

            //Check for automated units - done 
            //If one exists set automation mode on - done
            //Get automated unit - done
            //Complete automated unit turn
            //Check for next automated unit
            //If one exists repeat steps 2 and 3
            //If one no longer exists set automation mode off

            if (automationMode)
            {

                Renderer.Camera.Target = automatedUnits[autoUnitNumber];
                activeUnit = automatedUnits[autoUnitNumber];


                if (activeUnit.IsTurnOver)
                {

                    if (autoUnitNumber < automatedUnits.Count - 1)
                    {

                        autoUnitNumber++;

                    }
                    else
                    {

                        automationMode = false;
                        autoUnitNumber = 0;
                        automatedUnits.Clear();

                    }

                }

            }

            if (activeUnit != null)
            {

                activeUnit.Update(gameTime);

            }

        }

    }
}
