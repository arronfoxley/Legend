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

        private int GlobalTurnNumber = 1;
        private int PlayerTurnNumber = 0;
        private int PlayerUnitTurnNumber = 0;

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

                //If it is, check if there are units awaiting a turn..
                if (PlayerUnitTurnNumber < activePlayer.Units.Count -1)
                {

                    activeUnit = activePlayer.Units[PlayerUnitTurnNumber];
                    PlayerUnitTurnNumber++;

                }
                //If there aren't, the players turn is over...
                else
                {

                    Debug.WriteLine("No more units for this player");

                    //So check if any players are awaiting a turn
                    if (PlayerTurnNumber < players.Count - 1)
                    {

                        PlayerUnitTurnNumber = 0;
                        PlayerTurnNumber++;

                        activePlayer = players[PlayerTurnNumber];
                        activeUnit = activePlayer.Units[PlayerUnitTurnNumber];

                        Debug.WriteLine("More players left, select next player");

                    }
                    //If there are no further players awaiting a trun, reset each players units and start the process again
                    else
                    {

                        foreach (Player player in players)
                        {

                            for (int i = 0; i < player.Units.Count; i++)
                            {

                                player.Units[i].IsTurnOver = false;

                            }

                        }

                        PlayerUnitTurnNumber = 0;
                        PlayerTurnNumber = 0;

                        activePlayer = players[PlayerTurnNumber];
                        activeUnit = activePlayer.Units[PlayerUnitTurnNumber];
                        GlobalTurnNumber++;
                        Debug.WriteLine("No more players left, need to reset");

                    }

                }

            }

        }

    }

}