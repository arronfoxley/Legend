using FGame.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Legend.Core.Display {
    public class GameDrawDepthList:RendererDrawDepthList {

        //Depth layer list order, list goes from 1 being on top to 0 being on the bottom
        //UI Elements will always be @ 1
        //Game assets will then take the order below

        //Mountains cast shadows over everything currently, top of the list as they cant be cast on itself..
        public const float GAME_MOUNTAIN_DEPTH_LAYER = 0.59f;
        //Mountains shadow over everything currently, second in the list..
        public const float GAME_MOUNTAIN_SHADOW_DEPTH_LAYER = 0.58f;
        //Units next..
        public const float GAME_UNIT_DEPTH_LAYER = 0.54f;
        //Structures next..
        public const float GAME_STRUCTURE_DEPTH_LAYER = 0.53f;
        //Resources next..
        public const float GAME_RESOURCE_DEPTH_LAYER = 0.52f;
        //Backgrounds last..
        public const float GAME_BACKGROUND_DEPTH_LAYER = 0.51f;

    }

}
