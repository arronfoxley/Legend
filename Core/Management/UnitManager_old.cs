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
    public class UnitManager_old {

        private List<Unit> units;
        private List<Unit> automatedUnits = new List<Unit>();
        private Unit activeUnit = null;

        private Boolean automationMode = false;
        private int autoUnitNumber = 0;

        private Grid grid;

        public UnitManager_old(Grid grid)
        {

            this.grid = grid;

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
        public Unit GetUnitFromGameSprite(GameSprite gameSprite)
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
        public void UpdatePlayerUnitsForNextTurn(List<Character> players)
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
        public void Update(GameTime gameTime)
        {

            foreach (Unit unit in units)
            {

                if (automationMode)
                {

                    Renderer.Camera.Target = automatedUnits[autoUnitNumber].GameSprite;
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

                if (unit.CurrentCell != grid.GetCellByXY(unit.GameSprite.WorldX / grid.CellSize, unit.GameSprite.WorldY / grid.CellSize))
                {

                    unit.CurrentCell = grid.GetCellByXY(unit.GameSprite.WorldX / grid.CellSize, unit.GameSprite.WorldY / grid.CellSize);
                    Debug.WriteLine("X: " + unit.CurrentCell.GridX + " Y: " + unit.CurrentCell.GridY);

                    if (unit.Home != null)
                    {

                        Debug.WriteLine("Home X: " + unit.Home.cell.GridX + " Home Y: " + unit.Home.cell.GridY);

                    }


                }


                unit.Update(gameTime);

            }

            //Check for automated units - done 
            //If one exists set automation mode on - done
            //Get automated unit - done
            //Complete automated unit turn
            //Check for next automated unit
            //If one exists repeat steps 2 and 3
            //If one no longer exists set automation mode off

        }
    }

}
