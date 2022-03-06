using FGame.Core;
using FGame.Core.Objects.Game;
using Legend.Core.Display;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Resources {
    public class Terrain:GameSprite {

        public int cellX = 0;
        public int cellY = 0;

        public string type = "";

        public Terrain(Texture2D texture, int width, int height, string type):base(texture, width, height)
        {

            drawLayer = GameDrawDepthList.GAME_RESOURCE_DEPTH_LAYER;
            this.type = type;

        }

    }

}
