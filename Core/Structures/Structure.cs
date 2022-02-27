using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Objects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Structures {
    public class Structure:GameSprite {

        public Structure(Texture2D texture, int width, int height) : base(texture, width, height)
        {

            this.drawLayer = Renderer.GAME_STRUCTURE_DEPTH_LAYER;

        }

    }
}
