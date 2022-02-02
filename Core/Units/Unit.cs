using FGame.Core;
using FGame.Events;
using FGame.Grid;
using FGame.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Objects {
    public class Unit:Sprite {

        private bool isMoving = false;
        private List<Cell> Path;
        private float movementCooldown = 0.3f;

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
        internal Vector2 startLocation;
        internal Vector2 predictedPosition;
        internal double mCooldown = 0;
        internal int step = 0;
        internal int totalSteps = 0;

        public Unit(Texture2D texture, int width, int height):base(texture,width,height)
        {

            this.drawLayer = Renderer.GAME_DEPTH_LAYER;

        }

        public Boolean IsMoving
        {

            get { return isMoving; }
            set { this.isMoving = value; }

        }

        public void PrepForMovememnt(List<Cell> path)
        {

            this.Path = path;
            totalSteps = Path.Count - 1;

            startLocation = new Vector2(Path[0].WorldX, Path[0].WorldY);

            step = 1;

            destinationLocation = new Vector2(Path[step].WorldX, Path[step].WorldY);
            distance = FGameMath.GetDistanceBetweenPoints(startLocation, destinationLocation);

            IsMoving = true;

        }

        private void Moving(GameTime gameTime)
        {

            if (mCooldown == 0f)
            {

                elapsedTime = gameTime.ElapsedGameTime.TotalSeconds;
                elapsedTravelTime += elapsedTime;
                lerpAmount = MathHelper.Min(1, (float)(elapsedTravelTime / (distance / movementSpeed)));

                predictedPosition = Vector2.Lerp(startLocation, destinationLocation, lerpAmount);

            }

            if (lerpAmount == 1)
            {
                elapsedTravelTime = 0;
                mCooldown += gameTime.ElapsedGameTime.TotalSeconds;

                if (step < totalSteps)
                {

                    if (mCooldown >= movementCooldown)
                    {

                        startLocation = new Vector2(Path[step].WorldX, Path[step].WorldY);
                        step++;
                        destinationLocation = new Vector2(Path[step].WorldX, Path[step].WorldY);
                        distance = FGame.Core.FGameMath.GetDistanceBetweenPoints(startLocation, destinationLocation);
                        mCooldown = 0f;

                    }

                }
                else
                {

                    mCooldown = 0f;
                    step = 0;
                    IsMoving = false;
                    IsTurnOver = true;

                }


            }

            X = (int)predictedPosition.X;
            Y = (int)predictedPosition.Y;

        }

        public void Update(GameTime gameTime)
        {

            if (IsMoving)
            {

                Moving(gameTime);

            }

        }

    }

}
