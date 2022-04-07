using FGame.Grid;
using System;
using System.Diagnostics;

namespace Legend.Core {
    public class Action {

        protected Cell targetCell;
        protected Boolean isComplete = false;
        protected int completionTime = 0;
        protected int elapsedTime = 0;
        protected int actionSpeed = 0;

        public Cell TargetCell
        {

            set { this.targetCell = value; }
            get { return this.targetCell; }

        }

        public Boolean IsComplete
        {

            set { this.isComplete = value; }
            get { return this.isComplete; }

        }

        public int ElapsedTime
        {

            set { this.elapsedTime = value; }
            get { return this.elapsedTime; }

        }

        public int CompletionTime
        {

            set { this.completionTime = value; }
            get { return this.completionTime; }

        }

        public int ActionSpeed
        {

            set { this.actionSpeed = value; }
            get { return this.actionSpeed; }

        }

        public virtual void Update()
        {

            if (!isComplete)
            {

                elapsedTime += actionSpeed;

                if (elapsedTime == completionTime)
                {

                    isComplete = true;
                    Debug.WriteLine("Action complete");

                }

            }

        }

    }

}
