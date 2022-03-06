using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Grid;
using FGame.Objects;
using Legend.Core.Display;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Tiles {
    public class Tile : GameSprite {



        public Tile(Texture2D texture, int width, int height):base(texture, width, height)
        {

            this.drawLayer = GameDrawDepthList.GAME_BACKGROUND_DEPTH_LAYER;


        }

    }

}
