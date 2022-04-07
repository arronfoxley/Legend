using Legend.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Legend {
    public class TurnManager_old {

        private List<Character> players = new List<Character>();
        private Character activePlayer = null;

        private int currentActivePlayerID = 0;

        public TurnManager_old()
        {


        }

        public void AddPlayerToTurnList(Character player)
        {

            players.Add(player);

        }

        public void RemovePlayerToTurnList(Character player)
        {

            players.Remove(player);

        }

        public Character ActivePlayer
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