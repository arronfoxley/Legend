using FGame.Core.Objects.Game;
using FGame.Grid;
using Legend.Core.Display;
using Microsoft.Xna.Framework.Graphics;

namespace Legend.Core.Structures {
    public class Structure {

        public Cell cell;

        private GameSprite gameSprite = null;
        public Structure(GameSprite gameSprite)
        {

            this.gameSprite = gameSprite;
            this.gameSprite.DrawLayer = GameDrawDepthList.GAME_STRUCTURE_DEPTH_LAYER;

        }

        public GameSprite GameSprite
        {

            get { return this.gameSprite; }
            set { this.gameSprite = value; }

        }

    }
}
