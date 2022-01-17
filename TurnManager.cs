using Legend.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend {
    public class TurnManager {

        private List<Player> players = new List<Player>();

        private Player activePlayer = null;
        private Unit activeUnit = null;

        private int PlayerTurnNumber = 0;
        public int PlayerUnitTurnNumber = 0;

        public TurnManager()
        {

        }

        public void AddPlayerToTurnList(Player player)
        {

            players.Add(player);

        }

        public void RemovePlayerToTurnList(Player player)
        {

            players.Remove(player);

        }

        public Unit ActiveUnit{

            get { return this.activeUnit;  }
            set { this.activeUnit = value; }

        }

        public Player ActivePlayer
        {

            get { return this.activePlayer; }
            set { this.activePlayer = value; }

        }


        public void Update()
        {

            //Check if active units turn is over..
            if (activeUnit.IsTurnOver)
            {

                //If it is, check for the next unit..
                if (PlayerUnitTurnNumber < activePlayer.units.Count -1)
                {

                    activeUnit = activePlayer.units[PlayerUnitTurnNumber];
                    PlayerUnitTurnNumber++;

                }
                else
                {

                    Debug.WriteLine("No more units for this player");

                    if (PlayerTurnNumber < players.Count - 1)
                    {

                        PlayerUnitTurnNumber = 0;
                        PlayerTurnNumber++;

                        activePlayer = players[PlayerTurnNumber];
                        activeUnit = activePlayer.units[PlayerUnitTurnNumber];

                        Debug.WriteLine("More players left, select next player");

                    }
                    else
                    {

                        foreach (Player player in players)
                        {

                            for (int i = 0; i < player.units.Count; i++)
                            {

                                player.units[i].IsTurnOver = false;

                            }

                        }

                        PlayerUnitTurnNumber = 0;
                        PlayerTurnNumber = 0;

                        activePlayer = players[PlayerTurnNumber];
                        activeUnit = activePlayer.units[PlayerUnitTurnNumber];
                        Debug.WriteLine("No more players left, need to reset");

                    }

                }

            }

        }

    }

}

/*
 * 
 * 
                    //Check for next player
                    if (PlayerTurnNumber == players.Count - 1)
                    {

                        PlayerTurnNumber = 0;

                        foreach (Player player in players)
                        {

                            for (int i = 0; i < player.units.Count; i++)
                            {

                                player.units[i].IsTurnOver = false;

                            }

                        }

                        activePlayer = players[PlayerTurnNumber];
                        activeUnit = activePlayer.units[PlayerUnitTurnNumber];

                        Debug.WriteLine("All Turns are Over, start again");                       

                    }
                    //If no players are waiting, restart..
                    else
                    {

                        PlayerTurnNumber++;

                        activePlayer = players[PlayerTurnNumber];
                        activeUnit = activePlayer.units[PlayerUnitTurnNumber];
                        Debug.WriteLine("Turn Over");

                    }
 * 
 */