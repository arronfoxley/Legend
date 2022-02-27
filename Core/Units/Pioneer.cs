using Legend.Core.Structures;
using Legend.Objects;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Collections.Generic;
using FGame.Core;
using System;

namespace Legend.Core.Units {
    public class Pioneer : Unit {

        protected float buildTimeInTurns = 1f;
        protected float elapsedBuildTimeInTurns = 0f;

        protected Boolean isBuilding = false;

        private List<BuildObject> buildList = new List<BuildObject>();

        private int buildSpeed = 1;

        public Pioneer(Texture2D texture, int width, int height) : base(texture, width, height)
        {

            drawLayer = Renderer.GAME_DEPTH_LAYER;

        }

        public Boolean IsBuilding
        {

            set { this.isBuilding = value; }
            get { return isBuilding; }

        }

        public void StartBuild(string structureType, Texture2D texture, int width, int height)
        {

            Structure structure = null;
            int buildTime = 0;
            isBuilding = true;

            switch (structureType)
            {

                case "settlement": 
                structure = new Settlement( texture,  width,  height);
                buildTime = 2;
                
                break;

            }

            structure.Color = Color.Black;

            structure.WorldX = this.WorldX;
            structure.WorldY = this.WorldY;

            structure.DrawX = structure.WorldX;
            structure.DrawY = structure.WorldY;


            BuildObject buildObject = new BuildObject(structure, buildTime);

            buildList.Add(buildObject);

            Renderer.AddGameSprite(structure);

            structure.Active = true;

        }

        private void CompleteBuild(Structure structure)
        {

            isBuilding = false;
            structure.Color = Color.SpringGreen;

        }

        public void UpdateBuildList()
        {

            foreach (BuildObject buildObject in buildList)
            {

                buildObject.UpdateBuildTime(buildSpeed);

                if (buildObject.buildComplete)
                {

                    CompleteBuild(buildObject.structure);
                    

                }

            }

        }

        public override void Update(GameTime gameTime)
        {

            if (IsMoving)
            {

                Moving(gameTime);

            }

        }

    }

}
