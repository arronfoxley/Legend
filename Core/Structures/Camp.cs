using FGame.Core.Objects.Game;
using Microsoft.Xna.Framework.Graphics;

namespace Legend.Core.Structures {
    public class Camp : Structure {

        private int storedLumberCount = 0;
        private int storedOreCount = 0;
        public Camp(GameSprite gameSprite) : base(gameSprite)
        {


        }

        public int StoredLumberCount
        {

            get { return this.storedLumberCount; }

        }

        public int StoredOreCount
        {

            get { return this.storedOreCount; }

        }

        public void DeliverResource(string resourceType, int count)
        {

            switch (resourceType)
            {

                case "Lumber": storedLumberCount += count; break;
                case "Ore": storedOreCount += count; break;

            }

        }

    }

}
