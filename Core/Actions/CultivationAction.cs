using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Grid;
using Legend.Core.Resources;
using Legend.Core.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend.Core.Actions {
    public class CultivationAction:Action {

        protected Terrain terrain;

        public CultivationAction(Terrain terrain, Cell targetCell, int actionSpeed, int completionTime)
        {

            this.terrain = terrain;

            this.actionSpeed = actionSpeed;
            this.completionTime = completionTime;

            this.targetCell = targetCell;

        }

        public override void Update()
        {

            if (!isComplete)
            {

                elapsedTime += actionSpeed;

                if (elapsedTime == completionTime)
                {

                    terrain.GameSprite.WorldX = targetCell.WorldX;
                    terrain.GameSprite.WorldY = targetCell.WorldY;

                    terrain.GameSprite.DrawX = terrain.GameSprite.WorldX;
                    terrain.GameSprite.DrawY = terrain.GameSprite.WorldY;     

                    terrain.GameSprite.Active = true;
                    Renderer.AddGameSprite(terrain.GameSprite);

                    isComplete = true;
                    Debug.WriteLine("Cultivation complete");

                }

            }

        }

    }

}
