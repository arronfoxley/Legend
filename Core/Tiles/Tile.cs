using FGame.Core;
using FGame.Objects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Tiles {
    public class Tile : Sprite {

        public Tile(Texture2D texture, int width, int height):base(texture, width, height)
        {

            this.drawLayer = Renderer.BACKGROUND_DEPTH_LAYER;

        }

    }

}
