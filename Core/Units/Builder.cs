using FGame.Core.Objects.Game;
using Legend.Objects;
using System.Diagnostics;

namespace Legend.Core.Units {
    public class Builder : Unit {

        public Builder(GameSprite gameSprite):base(gameSprite)
        {


        }

        public void PerformAction(Action action)
        {

            Debug.WriteLine(action);

            performingAction = true;
            activeAction = action;

        }


    }

}