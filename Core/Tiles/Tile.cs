using FGame.Core.Objects.Game;
using Legend.Core.Display;
using Microsoft.Xna.Framework.Graphics;

namespace Legend.Core.Tiles {
    public class Tile{

        private GameSprite gameSprite;

        public Tile(GameSprite gameSprite)
        {

            this.gameSprite = gameSprite;
            this.gameSprite.DrawLayer = GameDrawDepthList.GAME_BACKGROUND_DEPTH_LAYER;

        }

        public GameSprite GameSprite
        {

            get { return this.gameSprite; }
            set { this.gameSprite = value; }

        }

    }

}
