using FGame.Core;
using FGame.Events;
using FGame.Objects;
using Legend.Objects;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend.Core.Units {
    public class Pioneer : Unit {

        public Pioneer(Texture2D texture, int width, int height) : base(texture, width, height)
        {

            this.DrawLayer = 0.5f;

        }

        public void Build()
        {

            Debug.WriteLine("Build");

        }

    }

}
