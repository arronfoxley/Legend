using FGame.Core;
using FGame.Objects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Structures {
    public class Structure:Sprite {

        public Structure(Texture2D texture, int width, int height) : base(texture, width, height)
        {

            this.drawLayer = Renderer.GAME_DEPTH_LAYER;

        }

    }
}
