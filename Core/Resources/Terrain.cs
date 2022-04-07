using FGame.Core.Objects.Game;
using FGame.Grid;
using Legend.Core.Display;
using Microsoft.Xna.Framework.Graphics;

namespace Legend.Core.Resources {
    public class Terrain {

        private Cell cell;

        private string type = "";

        private GameSprite gameSprite;

        public Terrain(GameSprite gameSprite, string type)
        {

            this.gameSprite = gameSprite;
            this.gameSprite.DrawLayer = GameDrawDepthList.GAME_RESOURCE_DEPTH_LAYER;
            this.type = type;

        }

        public string Type
        {

            get { return this.type; }
            set { this.type = value; }

        }

        public Cell Cell
        {

            get { return this.cell; }
            set { this.cell = value; }

        }

        public GameSprite GameSprite
        {

            get { return this.gameSprite; }
            set { this.gameSprite = value; }

        }

    }

}
