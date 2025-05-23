﻿using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public static Rocket Create(Vector2 position, List<string> activeModules = null, bool sync = true, Action < Rocket> action = null)
        {
            // Rocket will not be managed.. we have to avoid ever reaching this  
            if (RocketManager.ActiveRocketCount > RocketManager.MaxRockets)
            {
                Utility.Chat("Max rockets reached. Should not ever reach this point during normal gameplay.", Color.Red);
                throw new System.Exception("Max rockets reached. Should not ever reach this point during normal gameplay.");
            }

            Rocket rocket = new(activeModules)
            {
                Position = position,
                Active = true,
                Transparency = 0f
            };

            RocketManager.AddRocket(rocket);
            rocket.CurrentWorld = MacrocosmSubworld.CurrentID;

            action?.Invoke(rocket);

            if(sync) 
                rocket.SyncEverything();

            return rocket;
        }
    }
}
