using Legend.Core.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend.Core {
    public class BuildObject {

        public Structure structure = null;
        public int buildTime = 0;
        public int elapsedBuildTime = 0;
        public bool buildComplete = false;
        public BuildObject(Structure structure, int buildTime) {

            this.structure = structure;
            this.buildTime = buildTime;

        }

        public void UpdateBuildTime(int elapsedBuildTime)
        {

            this.elapsedBuildTime += elapsedBuildTime;

            if (this.elapsedBuildTime == buildTime)
            {

                buildComplete = true;

            }

        }

    }

}
