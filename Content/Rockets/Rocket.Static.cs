
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Macrocosm.Content.Rockets
{
    public partial class Rocket
    {
        public static readonly List<string> DefaultModuleNames = new()
        {
            "CommandPod",
            "ServiceModule",
            "ReactorModule",
            "EngineModule",
            "BoosterLeft",
            "BoosterRight"
        };

        public static Rocket Create(Vector2 position)
        {
            // Rocket will not be managed.. we have to avoid ever reaching this  
            if (RocketManager.ActiveRocketCount > RocketManager.MaxRockets)
            {
                Utility.Chat("Max rockets reached. Should not ever reach this point during normal gameplay.", Color.Red);
                throw new System.Exception("Max rockets reached. Should not ever reach this point during normal gameplay.");
            }

            Rocket rocket = new()
            {
                Position = position,
                Active = true
            };

            RocketManager.AddRocket(rocket);
            rocket.OnCreation();
            return rocket;
        }
    }
}
