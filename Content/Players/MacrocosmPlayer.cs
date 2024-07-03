using Macrocosm.Common.Config;
using Macrocosm.Common.Netcode;
using Macrocosm.Common.Subworlds;
using Macrocosm.Common.Systems;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Biomes;
using Macrocosm.Content.Debuffs;
using Macrocosm.Content.LoadingScreens;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.ModLoader.ModContent;

namespace Macrocosm.Content.Players
{
    public enum SpaceHazard
    {
        None,
        Vacuum
    }

    /// <summary>
    /// Miscellaneous class for storing custom player data. 
    /// Complex, very specific systems should be implemented in a separate ModPlayer.
    /// </summary>
    public class MacrocosmPlayer : ModPlayer
    {
        /// <summary> 
        /// The player's space protection level.
        /// Not synced.
        /// </summary>
        public float SpaceProtection { get; set; } = 0f;

        /// <summary> 
        /// The radiation effect intensity for this player. 
        /// Not synced.
        /// </summary>
        public float RadNoiseIntensity = 0f;

        /// <summary> 
        /// Chandrium whip hit stacks. 
        /// Not synced.
        /// </summary>
        public int ChandriumWhipStacks = 0;

        /// <summary> 
        /// Whether this player is aware that they can use zombie fingers to unlock chests.
        /// Persistent. Not synced.
        /// </summary>
        public bool KnowsToUseZombieFinger = false;

        /// <summary> 
        /// Chance to not consume ammo from equipment and weapons, stacks additively with the vanilla chance 
        /// Not synced.
        /// </summary>
        public float ChanceToNotConsumeAmmo
        {
            get => chanceToNotConsumeAmmo;
            set => chanceToNotConsumeAmmo = MathHelper.Clamp(value, 0f, 1f);
        }

        private float chanceToNotConsumeAmmo = 0f;

        public override void ResetEffects()
        {
            SpaceProtection = 0f;
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

        public override void PreUpdate()
        {
        }

        public override void PostUpdateBuffs()
        {
            if (!Player.GetModPlayer<RocketPlayer>().InRocket)
            {
                if (SubworldSystem.AnyActive<Macrocosm>())
                    Player.AddBuff(BuffType<Depressurized>(), 2);

                //if(Player.InModBiome<IrradiationBiome>())
                //    Player.AddBuff(BuffType<Irradiated>(), 2);
            }
        }

        public override void PostUpdateEquips()
        {
            if (SpaceProtection >= (float)SpaceHazard.Vacuum * 3)
                Player.buffImmune[BuffType<Depressurized>()] = true;

            //if (SpaceProtection >= (float)SpaceHazard.Radiation * 3)
            //    Player.buffImmune[BuffType<Irradiated>()] = true;

            if (SpaceProtection >= 3f)
                Player.setBonus = Language.GetTextValue("Mods.Macrocosm.Items.SetBonuses.SpaceProtection_" + (int)(SpaceProtection / 3f));
        }

        public override void PostUpdateMiscEffects()
        {
            if (SubworldSystem.AnyActive<Macrocosm>())
                Main.SceneMetrics.GraveyardTileCount = 0;
            else
                Main.SceneMetrics.GraveyardTileCount += TileCounts.Instance.GraveyardModTileCount;

            if (!Main.dedServ)
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
        }

        public override void OnEnterWorld()
        {
            CircuitSystem.SearchCircuits();
        }

        public override void CopyClientState(ModPlayer clientClone)
        {
            // TODO: copy data that has to be netsynced
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            // TODO: SyncPlayer if netsynced data is different
        }

        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.SyncMacrocosmPlayer);
            packet.Write((byte)Player.whoAmI);

            // TODO: add netsynced data here 
        }

        public static void ReceiveSyncPlayer(BinaryReader reader, int whoAmI)
        {
            int playerWhoAmI = reader.ReadByte();
            MacrocosmPlayer macrocosmPlayer = Main.player[playerWhoAmI].GetModPlayer<MacrocosmPlayer>();

            // TODO: read netsynced data here
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
