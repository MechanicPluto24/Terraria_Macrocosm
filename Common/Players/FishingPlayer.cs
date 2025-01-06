using Macrocosm.Content.Items.Furniture.Industrial;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players
{
    public class FishingPlayer : ModPlayer
    {
        public override void ResetEffects()
        {
        }

        public override void ModifyFishingAttempt(ref FishingAttempt attempt)
        {
        }

        /* 
        Rarity flags, see https://terraria.wiki.gg/wiki/Fishing
        none -> Plentiful (e.g. Bass, Wooden Crate)
        attempt.common -> Common (e.g. Specular Fish,  Neon Tetra)
        attempt.uncommon -> Uncommon (e.g. Iron Crate, Armored Cavefish, Bomb Fish)
        attempt.rare -> Rare (e.g. Biome Crates, Swordfish)	
        attempt.veryrare -> Very Rare (e.g Gold Crate, Reaver Shark)
        attempt.legendary -> Extremely Rare (e.g. Accesories, Evil Fish Weapons, Gold Crate)
        */
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            bool inWater = !attempt.inLava && !attempt.inHoney;

            // Moon fishing
            if (SubworldSystem.IsActive<Moon>())
            {
                if (inWater)
                {
                    if (attempt.crate)
                    {
                        // Always drop MB crates on the Moon
                        itemDrop = ModContent.ItemType<IndustrialCrate>();
                    }
                }
                /*
                else if (attempt.inLava)
                {
                }
                else if (attempt.inHoney)
                {
                }
                else
                {
                }
                */
            }
        }

        public override bool? CanConsumeBait(Item bait)
        {
            return base.CanConsumeBait(bait);
        }

        public override void ModifyCaughtFish(Item fish)
        {
        }
    }
}
