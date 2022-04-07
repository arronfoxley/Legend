using FGame.Core;
using FGame.Grid;
using Legend.Core.Structures;
using System.Diagnostics;

namespace Legend.Core {
    public class BuildAction : Action {

        private Structure structure;
        public BuildAction(Structure structure, Cell targetCell, int actionSpeed, int completionTime)
        {

            this.structure = structure;
            this.targetCell = targetCell;
            this.actionSpeed = actionSpeed;
            this.completionTime = completionTime;

        }

        public override void Update()
        {

            if (!isComplete)
            {

                elapsedTime += actionSpeed;

                if (elapsedTime == completionTime)
                {

                    structure.cell = targetCell;

                    structure.GameSprite.WorldX = targetCell.WorldX;
                    structure.GameSprite.WorldY = targetCell.WorldY;

                    structure.GameSprite.DrawX = targetCell.WorldX;
                    structure.GameSprite.DrawY = targetCell.WorldY;

                    Renderer.AddGameSprite(structure.GameSprite);

                    structure.GameSprite.Active = true;

                    isComplete = true;

                    Debug.WriteLine("Build complete");
                    

                }

            }

        }

    }

}
