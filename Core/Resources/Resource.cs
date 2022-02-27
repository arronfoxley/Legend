using FGame.Core;
using FGame.Core.Objects.Game;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Resources {
    public class Resource:GameSprite {

        public int cellX = 0;
        public int cellY = 0;

        public Resource(Texture2D texture, int width, int height):base(texture, width, height)
        {

            drawLayer = Renderer.GAME_RESOURCE_DEPTH_LAYER;

        }

    }
}
