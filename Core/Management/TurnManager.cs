using FGame.Core;
using Legend.Core.Management;
using Legend.Core.Units;
using Legend.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend {
    public class TurnManager {

        private List<Player> players = new List<Player>();
        private Player activePlayer = null;

        private int currentActivePlayerID = 0;

        public TurnManager()
        {


        }

        public List<Player> Players
        {

            get { return this.players; }

        }

        public void AddPlayerToTurnList(Player player)
        {

            players.Add(player);

        }

        public void RemovePlayerToTurnList(Player player)
        {

            players.Remove(player);

        }

        public Player ActivePlayer
        {

            get { return this.activePlayer; }
            set { this.activePlayer = value; }

        }

        public void SetPlayerTurn()
        {

            activePlayer = players[currentActivePlayerID];

        }

        public void ResetPlayers()
        {

            activePlayer = null;

            for (int i = 0; i < players.Count; i++)
            {

                players[i].IsTurnComplete = false;
 
            }

        }

        public Boolean CheckGlobalTurnComplete()
        {

            Boolean turnComplete = true;


            for (int i = 0; i < players.Count; i++)
            {

                if (!players[i].IsTurnComplete)
                {

                    turnComplete = false;
                    break;

                }

            }

            return turnComplete;

        }

        private Boolean CheckPlayersUnitsTurnsAreOver()
        {

            Boolean isTurnOver = false;


            for (int i = 0; i < activePlayer.Units.Count; i++)
            {

                if (activePlayer.Units[i].IsTurnOver)
                {
              
                    isTurnOver = true;
                    break;

                }

            }

            return isTurnOver;

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

            

        }

    }

}