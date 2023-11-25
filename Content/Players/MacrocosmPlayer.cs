using Macrocosm.Common.Systems;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Buffs.Debuffs;
using Macrocosm.Content.Subworlds;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Players
{
    public enum SpaceProtection
    {
        None,
        Tier1,
        Tier2,
        Tier3
    }

    /// <summary>
    /// The default class for storing custom player data. 
    /// If your custom player data has more members than one field or field+property, 
    /// or some other complex update logic, use a separate ModPlayer class.
    /// </summary>
    public class MacrocosmPlayer : ModPlayer
    {
        /// <summary> 
        /// The player's space protection level.
        /// Handled locally.
        /// </summary>
        public SpaceProtection SpaceProtection { get; set; } = SpaceProtection.None;

        /// <summary> 
        /// The radiation effect intensity for this player. 
        /// Handled locally. 
        /// </summary>
        public float RadNoiseIntensity = 0f;

        /// <summary> 
        /// Chandrium whip hit stacks. 
        /// Handled locally.
        /// </summary>
        public int ChandriumWhipStacks = 0;

        /// <summary> 
        /// Whether this player is aware that they can use zombie fingers to unlock chests.
        /// Handled locally.
        /// </summary>
        public bool KnowsToUseZombieFinger = false;

        /// <summary> 
        /// Chance to not consume ammo from equipment and weapons, stacks additively with the vanilla chance 
        /// Handled locally.
        /// </summary>
        public float ChanceToNotConsumeAmmo
        {
            get => chanceToNotConsumeAmmo;
            set => chanceToNotConsumeAmmo = MathHelper.Clamp(value, 0f, 1f);
        }

        private float chanceToNotConsumeAmmo = 0f;

        public override void ResetEffects()
        {
            SpaceProtection = SpaceProtection.None;
            RadNoiseIntensity = 0f;
            ChanceToNotConsumeAmmo = 0f;
            Player.buffImmune[BuffType<Depressurized>()] = false;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            bool consumeAmmo = true;

            if (Main.rand.NextFloat() < ChanceToNotConsumeAmmo)
                consumeAmmo = false;

            return consumeAmmo;
        }

        public override void PostUpdateBuffs()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
            {
                UpdateSpaceEnvironmentalDebuffs();
            }
        }

        public void UpdateSpaceEnvironmentalDebuffs()
        {
            if (!Player.GetModPlayer<RocketPlayer>().InRocket)
            {
                if (SubworldSystem.IsActive<Moon>())
                {
                    if (SpaceProtection == SpaceProtection.None)
                        Player.AddBuff(BuffType<Depressurized>(), 2);
                    //if (protectTier <= SpaceProtection.Tier1)
                    //Player.AddBuff(BuffTpye<Irradiated>(), 2);
                }
                //else if (SubworldSystem.IsActive<Mars>())
            }
        }

        public override void PostUpdateMiscEffects()
        {
            //UpdateGravity();
            UpdateFilterEffects();
        }

        public override void PostUpdateEquips()
        {
            UpdateSpaceArmourImmunities();
            if (Player.GetModPlayer<MacrocosmPlayer>().SpaceProtection > SpaceProtection.None)
                Player.setBonus = Language.GetTextValue("Mods.Macrocosm.Items.SetBonuses.SpaceProtection_" + SpaceProtection.ToString());
        }

        public void UpdateSpaceArmourImmunities()
        {
            if (SpaceProtection > SpaceProtection.None)
                Player.buffImmune[BuffType<Depressurized>()] = true;
            if (SpaceProtection > SpaceProtection.Tier1)
            {
            }
        }

        //private void UpdateGravity()
        //{
        //	if (MacrocosmSubworld.AnyActive)
        //		Player.gravity = Player.defaultGravity * MacrocosmSubworld.Current.GravityMultiplier;
        //}

        private void UpdateFilterEffects()
        {
            if (Player.InModBiome<IrradiationBiome>())
            {
                if (!Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                    Filters.Scene.Activate("Macrocosm:RadiationNoise");

                RadNoiseIntensity += 0.189f * Utility.InverseLerp(400, 10000, TileCounts.Instance.IrradiatedRockCount, clamped: true);

                Filters.Scene["Macrocosm:RadiationNoise"].GetShader().UseIntensity(RadNoiseIntensity);
            }
            else
            {
                if (Filters.Scene["Macrocosm:RadiationNoise"].IsActive())
                    Filters.Scene.Deactivate("Macrocosm:RadiationNoise");
            }
        }

        public override void SaveData(TagCompound tag)
        {
            if (KnowsToUseZombieFinger)
                tag[nameof(KnowsToUseZombieFinger)] = true;
        }

        public override void LoadData(TagCompound tag)
        {
            KnowsToUseZombieFinger = tag.ContainsKey(nameof(KnowsToUseZombieFinger));
        }
    }
}
