using FGame.Core;
using FGame.Events;
using FGame.Grid;
using FGame.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Legend.Objects {
    public class Unit:Sprite {

        private bool isMoving = false;
        protected List<Cell> path;
        private float movementCooldown = 0.5f;

        public Boolean TurnStarted = false;
        public Boolean TurnComplete = false;

        public Player IsOwnedBy = null;
        public Boolean IsTurnOver = false;

        internal double elapsedTime;
        internal double elapsedTravelTime = 0;
        internal float lerpAmount = 0;
        internal float movementSpeed = 300f;
        internal int distance = 0;
        internal Vector2 destinationLocation;
        internal Vector2 stepLocation;
        internal Vector2 predictedPosition;
        internal double mCooldown = 0;
        internal int step = 0;
        internal int totalSteps = 0;

        internal int maxActionPoints = 5;
        internal int freeActionPoints = 5;

        private Boolean automatedMovement = false;

        public Unit(Texture2D texture, int width, int height):base(texture,width,height)
        {

            this.drawLayer = Renderer.GAME_DEPTH_LAYER;

        }

        public Boolean AutomatedMovement
        {

            set { this.automatedMovement = value; }
            get { return this.automatedMovement; }

        }

        public Boolean IsMoving
        {

            get { return isMoving; }
            set { this.isMoving = value; }

        }

        public List<Cell> Path
        {

            get { return this.path; }

        }

        public void PrepForMovememnt(List<Cell> path)
        {

            this.path = path;
            totalSteps = path.Count - 1;
            freeActionPoints--;

            if (totalSteps > maxActionPoints)
            {

                Debug.WriteLine("Journey will require automated movement");
                automatedMovement = true;

            }

            stepLocation = new Vector2(path[0].WorldX, path[0].WorldY);

            step = 1;

            destinationLocation = new Vector2(path[step].WorldX, path[step].WorldY);
            distance = FGameMath.GetDistanceBetweenPoints(stepLocation, destinationLocation);

            IsMoving = true;

        }

        protected void Moving(GameTime gameTime)
        {

            //if movement is not currently on cooldown..
            if (mCooldown == 0f)
            {
                //set the elapsed time
                elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
                //added it to the elapsed travel time
                elapsedTravelTime += elapsedTime;
                //set the lerp amount..
                lerpAmount = MathHelper.Min(1, (float)(elapsedTravelTime / (distance / movementSpeed)));
                //prediect the poisiton between the step location and destination
                predictedPosition = Vector2.Lerp(stepLocation, destinationLocation, lerpAmount);

            }

            //if the movememnt is complete
            if (lerpAmount == 1)
            {
                //Reset the elapsed travel time
                elapsedTravelTime = 0;
                //Begin cooldown
                mCooldown += gameTime.ElapsedGameTime.TotalSeconds;

                //If we have any free action points available
                if (freeActionPoints > 0  && step < totalSteps)
                {

                    //And the cooldown period for movement has been complete
                    if (mCooldown >= movementCooldown)
                    {

                        //Get the location of the current step of the path the unit is on
                        stepLocation = new Vector2(path[step].WorldX, path[step].WorldY);

                        //Iterate to the next step
                        step++;
                        //Remove a free action point for the cost of movement
                        freeActionPoints--;
                        //Set the destination to the iterated step
                        destinationLocation = new Vector2(path[step].WorldX, path[step].WorldY);
                        //Get the distance between the step location and the destination
                        distance = FGame.Core.FGameMath.GetDistanceBetweenPoints(stepLocation, destinationLocation);
                        //Movement off cooldown
                        mCooldown = 0f;

                        Debug.WriteLine(freeActionPoints);

                    }
                    

                }
                //If unit has no free action points left, it cant be moved for the remainder of the turn..
                else
                {
                    //If the path has been fully stepped through, reset the unit..
                    if (step == totalSteps)
                    {

                        JourneyReset();

                        Debug.WriteLine("Journey Over");

                    }

                    if (!IsTurnOver)
                    {

                        IsTurnOver = true;
                        Debug.WriteLine("Turn Over");

                    }

                }

            }

            WorldX = (int)predictedPosition.X;
            WorldY = (int)predictedPosition.Y;

        }

        public int FreeActionPoints
        {

            set { this.freeActionPoints = value; }
            get {  return freeActionPoints; }

        }

        public void TurnActionReset()
        {

            freeActionPoints = maxActionPoints;
            IsTurnOver = false;

        }

        public void JourneyReset()
        {

            mCooldown = 0f;
            step = 0;
            IsMoving = false;
            automatedMovement = false;
            path = null;
            distance = 0;

        }

        public virtual void Update(GameTime gameTime)
        {

            if (IsMoving)
            {

                Moving(gameTime);

            }          

        }

    }

}
