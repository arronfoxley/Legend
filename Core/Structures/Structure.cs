using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Objects;
using Legend.Core.Display;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Structures {
    public class Structure:GameSprite {

        public int cellX = 0;
        public int cellY = 0;

        public Structure(Texture2D texture, int width, int height) : base(texture, width, height)
        {

            this.drawLayer = GameDrawDepthList.GAME_STRUCTURE_DEPTH_LAYER;

        }

    }
}
