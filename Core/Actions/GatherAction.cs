using FGame.Grid;
using Legend.Core.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Legend.Core.Actions {
    public class GatherAction : CultivationAction {

        private string resourceType;
        private int resourceCount;

        private int currentResourceCount = 0;

        private List<Cell> gatherPath;
        private List<Cell> returnPath;

        private Boolean gatheringResource;
        private Boolean unloadingResource;     

        public GatherAction(int actionSpeed, int completionTime, string resourceType, int resourceCount, Terrain terrain) :base(terrain, null, actionSpeed, completionTime)
        {

            this.actionSpeed = actionSpeed;
            this.completionTime = completionTime;
            this.resourceType = resourceType;
            this.resourceCount = resourceCount;
            this.terrain = terrain;

        }
        public string ResourceType
        {

            get { return this.resourceType; }
            set { this.resourceType = value; }

        }
        public Terrain Terrain
        {

            get { return this.terrain; }

        }
        public Boolean GatheringResource
        {

            set { this.gatheringResource = value; }
            get { return this.gatheringResource; }

        }
        public int CurrentResourceCount
        {

            get { return this.currentResourceCount; }

        }
        public Boolean UnloadingResource
        {

            set { this.unloadingResource = value; }
            get { return this.unloadingResource; }

        }
        public List<Cell> GatherPath
        {

            set { this.gatherPath = value; }
            get { return this.gatherPath; }

        }
        public List<Cell> ReturnPath
        {

            set { this.returnPath = value; }
            get { return this.returnPath; }

        }
        public int ResourceCount
        {

            get { return this.resourceCount; }

        }
        public override void Update()
        {

            if (gatheringResource)
            {

                elapsedTime += actionSpeed;
                currentResourceCount += resourceCount / completionTime;

                if (elapsedTime == completionTime)
                {

                    gatheringResource = false;
                    Debug.WriteLine("Gather complete: " + currentResourceCount + " collected");

                }

            }

            if (unloadingResource)
            {

                currentResourceCount -= resourceCount / completionTime;
                Debug.WriteLine("Current "+resourceType+" resources left for delivery: " + currentResourceCount);

                if (currentResourceCount == 0)
                {

                    elapsedTime = 0;
                    unloadingResource = false;
                    Debug.WriteLine("Unload complete");

                }

            }

        }

    }

}
