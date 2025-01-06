using Macrocosm.Content.WorldGeneration.Structures;
using Macrocosm.Content.WorldGeneration.Structures.LunarOutposts;
using Macrocosm.Content.WorldGeneration.Structures.Shrines;
using Microsoft.Xna.Framework;
using Macrocosm.Common.WorldGeneration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.WorldBuilding;
using static Macrocosm.Common.Utils.Utility;
using static Terraria.ModLoader.ModContent;
using Macrocosm.Content.Rockets;

namespace Macrocosm.Content.Subworlds
{
    public partial class EarthOrbitSubworld
    {
        [Task]
        private void PlaceSpawn(GenerationProgress progress)
        {

            Structure modual = new BaseSpaceStationModual();
            int x, y;
            x = (int)(Main.maxTilesX/2);
            y = (int)(Main.maxTilesX/2);
            Point16 origin = new (Main.spawnTileX-(int)(modual.Size.X / 2), Main.spawnTileY-(int)(modual.Size.Y));
            modual.Place(origin,null);
        }
    }
}
