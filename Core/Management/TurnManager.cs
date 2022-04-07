using Legend.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Management {
    public class TurnManager {

        private static List<Character> characters = new List<Character>();

        public static Character ActiveCharacter = null;

        private static int turnCount = 1;

        private static int characterTurnCount = 0;

        public TurnManager() {



        }

        public static List<Character> Characters
        {

            get { return characters; }

        }

            public static void AddCharacter(Character character)
        {

            Characters.Add(character);

        }

        public static void StartGlobalTurn()
        {

            ActiveCharacter = Characters[characterTurnCount];

        }

        public static void SetNextCharacterTurn()
        {

            ActiveCharacter = Characters[characterTurnCount];

        }

        public static void UpdateCharatcerTurnCount(){


            characterTurnCount++;

        }

        public static void GlobalTurnComplete()
        {

            foreach (Character character in Characters)
            {

                character.IsTurnComplete = false;

            }

            characterTurnCount = 0;
            turnCount++;

        }

        public static Boolean IsGlobalTurnComplete(){

            if (characterTurnCount < characters.Count)
            {

                return false;

            }
            else
            {

                return true;

            }

        }

    }

}
