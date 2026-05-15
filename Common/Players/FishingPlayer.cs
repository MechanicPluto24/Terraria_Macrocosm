using Macrocosm.Content.Biomes;
using Macrocosm.Common.Subworlds;
using Macrocosm.Content.Items.Fishes;
using Macrocosm.Content.Items.Furniture.Industrial;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Macrocosm.Common.Players;

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
    attempt.uncommon -> Uncommon (e.g. Quest Fish, Iron Crate, Armored Cavefish, Bomb Fish)
    attempt.rare -> Rare (e.g. Biome Crates, Swordfish)	
    attempt.veryrare -> Very Rare (e.g Gold Crate, Reaver Shark)
    attempt.legendary -> Extremely Rare (e.g. Accesories, Evil Fish Weapons, Gold Crate)
    */
    public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
    {
        bool inWater = !attempt.inLava && !attempt.inHoney;

        if (OrbitSubworld.AnyActive()) // No fishing, for now
        {
            itemDrop = -1;
            npcSpawn = -1;
            return;
        }

        // Moon fishing
        if (SubworldSystem.IsActive<Moon>())
        {
            if (inWater)
            {
                // Crates
                if (attempt.crate)
                {
                    // Always drop MB crates on the Moon (for now...)
                    itemDrop = ModContent.ItemType<IndustrialCrate>();
                    return;
                }

                int[] moonQuestFishes = [
                    ModContent.ItemType<DunceCraterfish>()
                ];
                if (attempt.uncommon && moonQuestFishes.Contains(attempt.questFish))
                {
                    itemDrop = attempt.questFish;
                    return;
                }

                itemDrop = ModContent.ItemType<Craterfish>();
                return;
            }
        }

        if (SubworldSystem.AnyActive<Macrocosm>())
        {
            itemDrop = -1;
            npcSpawn = -1;
            return;
        }

        // Earth fishing
        if (inWater)
        {
            if (Player.InModBiome<PollutionBiome>())
            {
                int[] pollutionQuestFishes = [
                    ModContent.ItemType<SmogWispfish>(),
                    ModContent.ItemType<HermitCan>(),
                    ModContent.ItemType<MutatedGoldfish>()
                ];

                if (attempt.uncommon && pollutionQuestFishes.Contains(attempt.questFish))
                {
                    itemDrop = attempt.questFish;
                    return;
                }
            }
        }
    }

    public override bool? CanConsumeBait(Item bait)
    {
        return null;
    }

    public override void ModifyCaughtFish(Item fish)
    {
    }
}
