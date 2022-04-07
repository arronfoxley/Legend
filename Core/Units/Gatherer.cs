using FGame.Core.Objects.Game;
using Legend.Core.Actions;
using Legend.Objects;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Legend.Core.Units {
    public class Gatherer : Unit {

        private int resourceUnloadCount;

        public Gatherer(GameSprite gamesprite) : base(gamesprite)
        {



        }

        public void PerformAction(GatherAction action)
        {

            performingAction = true;
            action.GatheringResource = true;
            activeAction = action;

        }

        public override void UpdateAction()
        {

            GatherAction gatherAction = (GatherAction)activeAction;

            if (gatherAction != null)
            {

                //If no longer gathering
                if (!gatherAction.GatheringResource)
                {
                    if (gatherAction.CurrentResourceCount > 0)
                    {
                        //Not at home and has resources
                        if (currentCell != home.cell)
                        {

                            //Return home
                            PrepForMovememnt(gatherAction.GatherPath);
                            Debug.WriteLine("Travelling Home");

                        }
                        //If at home
                        else
                        {
                            //Set unloading to true
                            if (!gatherAction.UnloadingResource)
                            {
                                gatherAction.UnloadingResource = true;

                            }

                            //Unload resources
                            resourceUnloadCount = gatherAction.ResourceCount / gatherAction.CompletionTime;
                            home.DeliverResource(gatherAction.ResourceType, resourceUnloadCount);
                            Debug.WriteLine("Unloading " + resourceUnloadCount + " " + gatherAction.ResourceType);

                        }

                    }


                }

                //If no longer unloading resources
                if (!gatherAction.UnloadingResource)
                {

                    //And still at home
                    if (currentCell == home.cell)
                    {
                        //Move back to the resource
                        PrepForMovememnt(gatherAction.ReturnPath);
                        Debug.WriteLine("Travelling to resource");

                        //Or if the already at the resource and looking to 
                    }
                    else if (currentCell == gatherAction.Terrain.Cell && gatherAction.CurrentResourceCount == 0)
                    {

                        gatherAction.GatheringResource = true;
                        Debug.WriteLine("Travelling complete");

                    }



                }

                Debug.WriteLine(gatherAction .ResourceType+ " counts at: " + home.StoredLumberCount);
                gatherAction.Update();

            }

        }

    }

}
